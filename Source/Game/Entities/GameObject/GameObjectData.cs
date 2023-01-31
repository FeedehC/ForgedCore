﻿// Copyright (c) CypherCore <http://github.com/CypherCore> All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE. See LICENSE file in the project root for full license information.

using System.Numerics;
using Framework.Collections;
using Framework.Constants;
using Game.Maps;

namespace Game.Entities
{
    public class GameObjectData : SpawnData
    {
        public Quaternion Rotation;
        public uint Animprogress { get; set; }
        public GameObjectState GoState { get; set; }
        public uint ArtKit { get; set; }

        public GameObjectData() : base(SpawnObjectType.GameObject) { }
    }

}