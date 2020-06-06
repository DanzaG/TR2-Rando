﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRLevelReader.Serialization;

namespace TRLevelReader.Model
{
    public class TRTexImage16 : ISerializableCompact
    {
        public ushort[] Pixels { get; set; }

        public byte[] Serialize()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    foreach (ushort pixel in Pixels)
                    {
                        writer.Write(pixel);
                    }
                }

                return stream.ToArray();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());

            sb.Append("\n");

            int Count = 1;
            foreach (ushort pixel in Pixels)
            {
                sb.Append(pixel + " ");

                Count++;

                if (Count % 8 == 0)
                {
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }
    }
}
