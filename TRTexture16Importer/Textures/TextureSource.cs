﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TRTexture16Importer.Textures
{
    public class TextureSource : IDisposable
    {
        public string PNGPath { get; set; }
        public bool ChangeSkyBox { get; set; }
        public Dictionary<string, List<Rectangle>> VariantMap { get; set; }

        public string[] Variants => VariantMap.Keys.ToArray();

        private Bitmap _bitmap;
        public Bitmap Bitmap
        {
            get
            {
                if (_bitmap == null)
                {
                    _bitmap = new Bitmap(PNGPath);
                }
                return _bitmap;
            }
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
            }
        }
    }
}