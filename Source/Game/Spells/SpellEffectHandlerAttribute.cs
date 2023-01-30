﻿// Copyright (c) CypherCore <http://github.com/CypherCore> All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE. See LICENSE file in the project root for full license information.

using System;
using Framework.Constants;

namespace Game.Entities
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SpellEffectHandlerAttribute : Attribute
    {
        public SpellEffectHandlerAttribute(SpellEffectName effectName)
        {
            EffectName = effectName;
        }

        public SpellEffectName EffectName { get; set; }
    }
}