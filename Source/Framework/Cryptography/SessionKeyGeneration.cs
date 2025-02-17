﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Security.Cryptography;

namespace Framework.Cryptography
{
    public class SessionKeyGenerator256
    {
        public SessionKeyGenerator256(byte[] buff, int size = 0)
        {
            if (size == 0)
                size = buff.Length;

            int halfSize = size / 2;

            sh = SHA256.Create();
            sh.TransformFinalBlock(buff, 0, halfSize);
            o1 = sh.Hash;

            sh.Initialize();
            sh.TransformFinalBlock(buff, halfSize, size - halfSize);
            o2 = sh.Hash;

            FillUp();
        }

        public void Generate(byte[] buf, uint sz)
        {
            for (uint i = 0; i < sz; ++i)
            {
                if (taken == 32)
                    FillUp();

                buf[i] = o0[taken];
                taken++;
            }
        }

        void FillUp()
        {
            sh.Initialize();
            sh.TransformBlock(o1, 0, 32, o1, 0);
            sh.TransformBlock(o0, 0, 32, o0, 0);
            sh.TransformFinalBlock(o2, 0, 32);
            o0 = sh.Hash;

            taken = 0;
        }

        readonly SHA256 sh;        
        uint taken;
        byte[] o0 = new byte[32];
        readonly byte[] o1 = new byte[32];
        readonly byte[] o2 = new byte[32];
    }

    public class SessionKeyGenerator
    {
        public SessionKeyGenerator(byte[] buff, int size = 0)
        {
            if (size == 0)
                size = buff.Length;

            int halfSize = size / 2;

            sh = SHA1.Create();
            sh.TransformFinalBlock(buff, 0, halfSize);
            o1 = sh.Hash;

            sh.Initialize();
            sh.TransformFinalBlock(buff, halfSize, size - halfSize);
            o2 = sh.Hash;

            FillUp();
        }

        public void Generate(byte[] buf, uint sz)
        {
            for (uint i = 0; i < sz; ++i)
            {
                if (taken == 20)
                    FillUp();

                buf[i] = o0[taken];
                taken++;
            }
        }

        void FillUp()
        {
            sh.Initialize();
            sh.TransformBlock(o1, 0, 20, o1, 0);
            sh.TransformBlock(o0, 0, 20, o0, 0);
            sh.TransformFinalBlock(o2, 0, 20);
            o0 = sh.Hash;

            taken = 0;
        }

        readonly SHA1 sh;
        uint taken;
        byte[] o0 = new byte[32];
        readonly byte[] o1 = new byte[32];
        readonly byte[] o2 = new byte[32];
    }
}
