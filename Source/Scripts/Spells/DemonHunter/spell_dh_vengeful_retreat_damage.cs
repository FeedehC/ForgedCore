﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.DemonHunter;

[SpellScript(198813)]
public class spell_dh_vengeful_retreat_damage : SpellScript, IHasSpellEffects, ISpellOnCast
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	private bool _targetHit;

	public override bool Validate(SpellInfo UnnamedParameter)
	{
		if (Global.SpellMgr.GetSpellInfo(DemonHunterSpells.SPELL_DH_PREPARED_FURY, Difficulty.None) != null)
			return false;

		return true;
	}

	private void CountTargets(List<WorldObject> targets)
	{
		_targetHit = targets.Count > 0;
	}

	public void OnCast()
	{
		var caster = GetCaster();

		if (caster != null)
		{
			if (caster.HasAura(DemonHunterSpells.SPELL_DH_PREPARED) && _targetHit)
				caster.CastSpell(caster, DemonHunterSpells.SPELL_DH_PREPARED_FURY, true);

			var aur = caster.GetAura(DemonHunterSpells.SPELL_DH_GLIMPSE);

			if (aur != null)
			{
				var aurEff = aur.GetEffect(0);

				if (aurEff != null)
				{
					var blur = caster.AddAura(DemonHunterSpells.SPELL_DH_BLUR_BUFF, caster);

					if (blur != null)
						blur.SetDuration(aurEff.GetBaseAmount());
				}
			}

			if (caster.HasAura(DemonHunterSpells.SPELL_DH_RUSHING_VAULT))
			{
				var chargeCatId = Global.SpellMgr.GetSpellInfo(DemonHunterSpells.SPELL_DH_FEL_RUSH, Difficulty.None).ChargeCategoryId;
				caster.GetSpellHistory().RestoreCharge(chargeCatId);
			}
		}
	}

	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(CountTargets, 0, Targets.UnitSrcAreaEnemy)); // 33
	}
}