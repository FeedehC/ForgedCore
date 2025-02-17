﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Shaman;

// 200071 - Undulation
[SpellScript(200071)]
internal class spell_sha_undulation_passive : AuraScript, IHasAuraEffects
{
	private byte _castCounter = 1; // first proc happens after two casts, then one every 3 casts
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.Dummy, AuraScriptHookType.EffectProc));
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		if (++_castCounter == 3)
		{
			Target.CastSpell(Target, ShamanSpells.UndulationProc, true);
			_castCounter = 0;
		}
	}
}