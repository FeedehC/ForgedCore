﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Generic;
using Framework.Constants;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.IAura;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.m_Events.HallowsEnd;

internal struct SpellIds
{
	//HallowEndCandysSpells
	public const uint CandyOrangeGiant = 24924;        // Effect 1: Apply Aura: Mod Size, Value: 30%
	public const uint CandySkeleton = 24925;           // Effect 1: Apply Aura: Change Model (Skeleton). Effect 2: Apply Aura: Underwater Breathing
	public const uint CandyPirate = 24926;             // Effect 1: Apply Aura: Increase Swim Speed, Value: 50%
	public const uint CandyGhost = 24927;              // Effect 1: Apply Aura: Levitate / Hover. Effect 2: Apply Aura: Slow Fall, Effect 3: Apply Aura: Water Walking
	public const uint CandyFemaleDefiasPirate = 44742; // Effect 1: Apply Aura: Change Model (Defias Pirate, Female). Effect 2: Increase Swim Speed, Value: 50%
	public const uint CandyMaleDefiasPirate = 44743;   // Effect 1: Apply Aura: Change Model (Defias Pirate, Male).   Effect 2: Increase Swim Speed, Value: 50%

	//Trickspells
	public const uint PirateCostumeMale = 24708;
	public const uint PirateCostumeFemale = 24709;
	public const uint NinjaCostumeMale = 24710;
	public const uint NinjaCostumeFemale = 24711;
	public const uint LeperGnomeCostumeMale = 24712;
	public const uint LeperGnomeCostumeFemale = 24713;
	public const uint SkeletonCostume = 24723;
	public const uint GhostCostumeMale = 24735;
	public const uint GhostCostumeFemale = 24736;
	public const uint TrickBuff = 24753;

	//Trickortreatspells
	public const uint Trick = 24714;
	public const uint Treat = 24715;
	public const uint TrickedOrTreated = 24755;
	public const uint TrickyTreatSpeed = 42919;
	public const uint TrickyTreatTrigger = 42965;
	public const uint UpsetTummy = 42966;

	//Wand Spells
	public const uint HallowedWandPirate = 24717;
	public const uint HallowedWandNinja = 24718;
	public const uint HallowedWandLeperGnome = 24719;
	public const uint HallowedWandRandom = 24720;
	public const uint HallowedWandSkeleton = 24724;
	public const uint HallowedWandWisp = 24733;
	public const uint HallowedWandGhost = 24737;
	public const uint HallowedWandBat = 24741;
}

[Script] // 24930 - Hallow's End Candy
internal class spell_hallow_end_candy_SpellScript : SpellScript, IHasSpellEffects
{
	private readonly uint[] spells =
	{
		SpellIds.CandyOrangeGiant, SpellIds.CandySkeleton, SpellIds.CandyPirate, SpellIds.CandyGhost
	};

	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.Hit));
	}

	private void HandleDummy(int effIndex)
	{
		Caster.CastSpell(Caster, spells.SelectRandom(), true);
	}
}

[Script] // 24926 - Hallow's End Candy
internal class spell_hallow_end_candy_pirate_AuraScript : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(HandleApply, 0, AuraType.ModIncreaseSwimSpeed, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterApply));
		AuraEffects.Add(new AuraEffectApplyHandler(HandleRemove, 0, AuraType.ModIncreaseSwimSpeed, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterRemove));
	}

	private void HandleApply(AuraEffect aurEff, AuraEffectHandleModes mode)
	{
		var spell = Target.NativeGender == Gender.Female ? SpellIds.CandyFemaleDefiasPirate : SpellIds.CandyMaleDefiasPirate;
		Target.CastSpell(Target, spell, true);
	}

	private void HandleRemove(AuraEffect aurEff, AuraEffectHandleModes mode)
	{
		var spell = Target.NativeGender == Gender.Female ? SpellIds.CandyFemaleDefiasPirate : SpellIds.CandyMaleDefiasPirate;
		Target.RemoveAura(spell);
	}
}

[Script] // 24750 Trick
internal class spell_hallow_end_trick : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScript, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScript(int effIndex)
	{
		var caster = Caster;
		var target = HitPlayer;

		if (target)
		{
			var gender = target.NativeGender;
			var spellId = SpellIds.TrickBuff;

			switch (RandomHelper.URand(0, 5))
			{
				case 1:
					spellId = gender == Gender.Female ? SpellIds.LeperGnomeCostumeFemale : SpellIds.LeperGnomeCostumeMale;

					break;
				case 2:
					spellId = gender == Gender.Female ? SpellIds.PirateCostumeFemale : SpellIds.PirateCostumeMale;

					break;
				case 3:
					spellId = gender == Gender.Female ? SpellIds.GhostCostumeFemale : SpellIds.GhostCostumeMale;

					break;
				case 4:
					spellId = gender == Gender.Female ? SpellIds.NinjaCostumeFemale : SpellIds.NinjaCostumeMale;

					break;
				case 5:
					spellId = SpellIds.SkeletonCostume;

					break;
				default:
					break;
			}

			caster.CastSpell(target, spellId, true);
		}
	}
}

[Script] // 24751 Trick or Treat
internal class spell_hallow_end_trick_or_treat : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScript, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScript(int effIndex)
	{
		var caster = Caster;
		var target = HitPlayer;

		if (target)
		{
			caster.CastSpell(target, RandomHelper.randChance(50) ? SpellIds.Trick : SpellIds.Treat, true);
			caster.CastSpell(target, SpellIds.TrickedOrTreated, true);
		}
	}
}

[Script] // 44436 - Tricky Treat
internal class spell_hallow_end_tricky_treat : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScript, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScript(int effIndex)
	{
		var caster = Caster;

		if (caster.HasAura(SpellIds.TrickyTreatTrigger) &&
			caster.GetAuraCount(SpellIds.TrickyTreatSpeed) > 3 &&
			RandomHelper.randChance(33))
			caster.CastSpell(caster, SpellIds.UpsetTummy, true);
	}
}

[Script] // 24717, 24718, 24719, 24720, 24724, 24733, 24737, 24741
internal class spell_hallow_end_wand : SpellScript, ISpellOnHit
{
	public void OnHit()
	{
		var caster = Caster;
		var target = HitUnit;

		uint spellId;
		var female = target.NativeGender == Gender.Female;

		switch (SpellInfo.Id)
		{
			case SpellIds.HallowedWandLeperGnome:
				spellId = female ? SpellIds.LeperGnomeCostumeFemale : SpellIds.LeperGnomeCostumeMale;

				break;
			case SpellIds.HallowedWandPirate:
				spellId = female ? SpellIds.PirateCostumeFemale : SpellIds.PirateCostumeMale;

				break;
			case SpellIds.HallowedWandGhost:
				spellId = female ? SpellIds.GhostCostumeFemale : SpellIds.GhostCostumeMale;

				break;
			case SpellIds.HallowedWandNinja:
				spellId = female ? SpellIds.NinjaCostumeFemale : SpellIds.NinjaCostumeMale;

				break;
			case SpellIds.HallowedWandRandom:
				spellId = RandomHelper.RAND(SpellIds.HallowedWandPirate, SpellIds.HallowedWandNinja, SpellIds.HallowedWandLeperGnome, SpellIds.HallowedWandSkeleton, SpellIds.HallowedWandWisp, SpellIds.HallowedWandGhost, SpellIds.HallowedWandBat);

				break;
			default:
				return;
		}

		caster.CastSpell(target, spellId, true);
	}
}