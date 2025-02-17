﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;

namespace Scripts.Spells.Warlock;

// 117828 - Backdraft
[SpellScript(117828)]
internal class spell_warlock_backdraft : AuraScript, IAuraCheckProc
{
	public bool CheckProc(ProcEventInfo UnnamedParameter)
	{
		var caster = Caster;

		if (caster == null)
			return false;

		if (caster.VariableStorage.GetValue("Spells.BackdraftCD", DateTime.MinValue) > GameTime.Now())
			return false;

		caster.VariableStorage.Set("Spells.BackdraftCD", GameTime.Now() + TimeSpan.FromMilliseconds(500));

		return true;
	}
}