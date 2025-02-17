﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

namespace Framework.Constants;

public enum RollMask
{
	Pass = 0x01,
	Need = 0x02,
	Greed = 0x04,
	Disenchant = 0x08,

	AllNoDisenchant = 0x07,
	AllMask = 0x0f
}