﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;

namespace Framework.Cryptography.Ed25519.Internal.Ed25519Ref10
{
    internal static partial class Ed25519Operations
    {
        public static void crypto_sign_keypair(byte[] pk, int pkoffset, byte[] sk, int skoffset, byte[] seed, int seedoffset)
        {
            int i;

            Array.Copy(seed, seedoffset, sk, skoffset, 32);
            byte[] h = Sha512.Hash(sk, skoffset, 32);//ToDo: Remove alloc
            ScalarOperations.sc_clamp(h, 0);

            GroupOperations.ge_scalarmult_base(out GroupElementP3 A, h, 0);
            GroupOperations.ge_p3_tobytes(pk, pkoffset, ref A);

            for (i = 0; i < 32; ++i) sk[skoffset + 32 + i] = pk[pkoffset + i];
            CryptoBytes.Wipe(h);
        }
    }
}
