﻿using RectanglePacker.Defaults;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using TRModelTransporter.Model.Textures;
using TRTexture16Importer.Helpers;

namespace TRModelTransporter.Packing
{
    public class TexturedTile : DefaultTile<TexturedTileSegment>, IDisposable
    {
        public BitmapGraphics BitmapGraphics { get; set; }

        // Some room textures in Opera House (at least) overlap, so we need to 
        // allow this when initialising the tiles.
        public bool AllowOverlapping
        {
            get => _allowOverlapping;
            set => _allowOverlapping = value;
        }

        public bool BitmapChanged => _segmentAdded || _segmentRemoved;
        private bool _segmentAdded, _segmentRemoved;

        public TexturedTile()
        {
            _segmentAdded = _segmentRemoved = false;
        }

        public void AddTexture(AbstractIndexedTRTexture texture)
        {
            foreach (TexturedTileSegment segment in Rectangles)
            {
                if (segment.Bounds.Contains(texture.Bounds))
                {
                    // If so, just map the texture to the same segment
                    segment.AddTexture(texture);
                    return;
                }
            }

            // Otherwise, make a new segment
            TexturedTileSegment newSegment = new TexturedTileSegment(texture, BitmapGraphics.Extract(texture.Bounds));
            base.Add(newSegment, texture.Bounds.X, texture.Bounds.Y);
        }

        public List<TexturedTileSegment> GetObjectTextureIndexSegments(IEnumerable<int> indices)
        {
            List<TexturedTileSegment> segments = new List<TexturedTileSegment>();
            foreach (int index in indices)
            {
                foreach (TexturedTileSegment segment in Rectangles)
                {
                    if (segment.IsObjectTextureFor(index) && !segments.Contains(segment))
                    {
                        segments.Add(segment);
                    }
                }
            }
            return segments;
        }

        public List<TexturedTileSegment> GetSpriteTextureIndexSegments(IEnumerable<int> indices)
        {
            List<TexturedTileSegment> segments = new List<TexturedTileSegment>();
            foreach (int index in indices)
            {
                foreach (TexturedTileSegment segment in Rectangles)
                {
                    if (segment.IsSpriteTextureFor(index) && !segments.Contains(segment))
                    {
                        segments.Add(segment);
                    }
                }
            }
            return segments;
        }

        public void Commit()
        {
            foreach (TexturedTileSegment segment in Rectangles)
            {
                segment.Commit(Index);
            }
        }

        protected override bool Add(TexturedTileSegment segment, int x, int y)
        {
            bool added = base.Add(segment, x, y);
            if (added)
            {
                CheckBitmapStatus();
                BitmapGraphics.Import(segment.Bitmap, segment.MappedBounds);

                segment.Bind();
                _segmentAdded = true;
            }
            return added;
        }

        public override bool Remove(TexturedTileSegment segment)
        {
            bool removed = base.Remove(segment);
            if (removed)
            {
                CheckBitmapStatus();
                BitmapGraphics.Delete(segment.Bounds);
                segment.Unbind();
                _segmentRemoved = true;
            }
            return removed;
        }

        public override void PackingStarted()
        {
            // No overlapping during actual packing
            AllowOverlapping = false;

            // Only if a segment has been removed prior to packing having
            // started do we keep a hold of that fact. This prevents the
            // bitmap being saved back to the level if it's unchanged.
            if (!_segmentRemoved)
            {
                _segmentAdded = _segmentRemoved = false;
            }
        }

        private void CheckBitmapStatus()
        {
            if (BitmapGraphics == null)
            {
                BitmapGraphics = new BitmapGraphics(new Bitmap(Width, Height, PixelFormat.Format32bppArgb));
            }
        }

        public void Dispose()
        {
            if (BitmapGraphics != null)
            {
                BitmapGraphics.Dispose();
            }

            foreach (TexturedTileSegment segment in _rectangles)
            {
                segment.Dispose();
            }
        }
    }
}