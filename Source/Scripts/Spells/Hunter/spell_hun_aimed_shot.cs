﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Hunter;

[SpellScript(19434)]
public class spell_hun_aimed_shot : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDamage, 0, SpellEffectName.SchoolDamage, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDamage(int effIndex)
	{
		var distance = 30.0f;
		var damagePct = 50;
		var targetList = new List<Unit>();
		var victimList = new List<Unit>();
		var canApplyDamage = true;

		var modOwner = Caster.SpellModOwner;

		if (modOwner != null)
			if (modOwner.HasAura(199522))
			{
				var mainTarget = HitUnit;

				if (mainTarget != null)
				{
					mainTarget.GetAnyUnitListInRange(targetList, distance);

					if (targetList.Count > 0)
					{
						foreach (var target in targetList)
							if (!modOwner.IsFriendlyTo(target))
							{
								if (target == mainTarget)
									continue;

								if (target.HasAura(187131))
									canApplyDamage = false;

								victimList.Add(target);
							}

						if (canApplyDamage)
							damagePct += 15;

						foreach (var victim in victimList)
						{
							var args = new CastSpellExtraArgs();
							var castTime = 0;
							mainTarget.ModSpellCastTime(SpellInfo, ref castTime);
							args.AddSpellMod(SpellValueMod.BasePoint0, (int)damagePct);
							args.SetOriginalCaster(modOwner.GUID);
							args.SetTriggerFlags(TriggerCastFlags.FullMask);

							mainTarget.CastSpell(victim, 164340, args);
						}
					}
				}
			}
	}
}