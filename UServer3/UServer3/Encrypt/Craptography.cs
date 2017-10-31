﻿using System.IO;

namespace UServer3.Encrypt
{
    public static class Craptography
    {
        // Fields
        private static readonly byte[] hash = new byte[]
        {
            0x97, 160, 0x89, 0x5b, 90, 15, 0x83, 13, 0xc9, 0x5f, 0x60, 0x35, 0xc2, 0xe9, 7, 0xe1,
            140, 0x24, 0x67, 30, 0x45, 0x8e, 8, 0x63, 0x25, 240, 0x15, 10, 0x17, 190, 6, 0x94,
            0xf7, 120, 0xea, 0x4b, 0, 0x1a, 0xc5, 0x3e, 0x5e, 0xfc, 0xdb, 0xcb, 0x75, 0x23, 11, 0x20,
            0x39, 0xb1, 0x21, 0x58, 0xed, 0x95, 0x38, 0x57, 0xae, 20, 0x7d, 0x88, 0xab, 0xa8, 0x44, 0xaf,
            0x4a, 0xa5, 0x47, 0x86, 0x8b, 0x30, 0x1b, 0xa6, 0x4d, 0x92, 0x9e, 0xe7, 0x53, 0x6f, 0xe5, 0x7a,
            60, 0xd3, 0x85, 230, 220, 0x69, 0x5c, 0x29, 0x37, 0x2e, 0xf5, 40, 0xf4, 0x66, 0x8f, 0x36,
            0x41, 0x19, 0x3f, 0xa1, 1, 0xd8, 80, 0x49, 0xd1, 0x4c, 0x84, 0xbb, 0xd0, 0x59, 0x12, 0xa9,
            200, 0xc4, 0x87, 130, 0x74, 0xbc, 0x9f, 0x56, 0xa4, 100, 0x6d, 0xc6, 0xad, 0xba, 3, 0x40,
            0x34, 0xd9, 0xe2, 250, 0x7c, 0x7b, 5, 0xca, 0x26, 0x93, 0x76, 0x7e, 0xff, 0x52, 0x55, 0xd4,
            0xcf, 0xce, 0x3b, 0xe3, 0x2f, 0x10, 0x3a, 0x11, 0xb6, 0xbd, 0x1c, 0x2a, 0xdf, 0xb7, 170, 0xd5,
            0x77, 0xf8, 0x98, 2, 0x2c, 0x9a, 0xa3, 70, 0xdd, 0x99, 0x65, 0x9b, 0xa7, 0x2b, 0xac, 9,
            0x81, 0x16, 0x27, 0xfd, 0x13, 0x62, 0x6c, 110, 0x4f, 0x71, 0xe0, 0xe8, 0xb2, 0xb9, 0x70, 0x68,
            0xda, 0xf6, 0x61, 0xe4, 0xfb, 0x22, 0xf2, 0xc1, 0xee, 210, 0x90, 12, 0xbf, 0xb3, 0xa2, 0xf1,
            0x51, 0x33, 0x91, 0xeb, 0xf9, 14, 0xef, 0x6b, 0x31, 0xc0, 0xd6, 0x1f, 0xb5, 0xc7, 0x6a, 0x9d,
            0xb8, 0x54, 0xcc, 0xb0, 0x73, 0x79, 50, 0x2d, 0x7f, 4, 150, 0xfe, 0x8a, 0xec, 0xcd, 0x5d,
            0xde, 0x72, 0x43, 0x1d, 0x18, 0x48, 0xf3, 0x8d, 0x80, 0xc3, 0x4e, 0x42, 0xd7, 0x3d, 0x9c, 180
        };

        // Methods
        public static void XOR(uint seed, MemoryStream src, int srcOffset, MemoryStream dst, int dstOffset)
        {
            int num = ((int) src.Length) - srcOffset;
            int length = hash.Length;
            int num3 = (int) (((ulong) seed) % ((ulong) length));
            dst.SetLength((long) (dstOffset + num));
            byte[] buffer = src.GetBuffer();
            byte[] buffer2 = dst.GetBuffer();
            for (int i = 0; i < num; i++)
            {
                buffer2[dstOffset + i] = (byte) (buffer[srcOffset + i] ^ hash[(num3 + i) % length]);
            }
        }
    }
}