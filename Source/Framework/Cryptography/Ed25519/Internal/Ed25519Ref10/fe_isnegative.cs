﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

namespace Framework.Cryptography.Ed25519.Internal.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
        return 1 if f is in {1,3,5,...,q-2}
        return 0 if f is in {0,2,4,...,q-1}

        Preconditions:
        |f| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
        */
        //int fe_isnegative(const fe f)
        public static int fe_isnegative(ref FieldElement f)
        {
            fe_reduce(out FieldElement fr, ref f);
            return fr.x0 & 1;
        }
    }
}