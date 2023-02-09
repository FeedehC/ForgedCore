﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.Spells.Monk;

[Script] // 115069 - Stagger
internal class spell_monk_stagger : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override bool Validate(SpellInfo spellInfo)
	{
		return ValidateSpellInfo(MonkSpells.StaggerLight, MonkSpells.StaggerModerate, MonkSpells.StaggerHeavy);
	}

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectAbsorbHandler(AbsorbNormal, 1, false, AuraScriptHookType.EffectAbsorb));
		AuraEffects.Add(new AuraEffectAbsorbHandler(AbsorbMagic, 2, false, AuraScriptHookType.EffectAbsorb));
	}

	public static Aura FindExistingStaggerEffect(Unit unit)
	{
		Aura auraLight = unit.GetAura(MonkSpells.StaggerLight);

		if (auraLight != null)
			return auraLight;

		Aura auraModerate = unit.GetAura(MonkSpells.StaggerModerate);

		if (auraModerate != null)
			return auraModerate;

		Aura auraHeavy = unit.GetAura(MonkSpells.StaggerHeavy);

		if (auraHeavy != null)
			return auraHeavy;

		return null;
	}

	private void AbsorbNormal(AuraEffect aurEff, DamageInfo dmgInfo, ref uint absorbAmount)
	{
		Absorb(dmgInfo, 1.0f);
	}

	private void AbsorbMagic(AuraEffect aurEff, DamageInfo dmgInfo, ref uint absorbAmount)
	{
		AuraEffect effect = GetEffect(4);

		if (effect == null)
			return;

		Absorb(dmgInfo, effect.GetAmount() / 100.0f);
	}

	private void Absorb(DamageInfo dmgInfo, float multiplier)
	{
		// Prevent default Action (which would remove the aura)
		PreventDefaultAction();

		// make sure Damage doesn't come from stagger Damage spell SPELL_MONK_STAGGER_DAMAGE_AURA
		SpellInfo dmgSpellInfo = dmgInfo.GetSpellInfo();

		if (dmgSpellInfo != null)
			if (dmgSpellInfo.Id == MonkSpells.StaggerDamageAura)
				return;

		AuraEffect effect = GetEffect(0);

		if (effect == null)
			return;

		Unit  target     = GetTarget();
		float agility    = target.GetStat(Stats.Agility);
		float baseAmount = MathFunctions.CalculatePct(agility, effect.GetAmount());
		float K          = Global.DB2Mgr.EvaluateExpectedStat(ExpectedStatType.ArmorConstant, target.GetLevel(), -2, 0, target.GetClass());

		float newAmount = (baseAmount / (baseAmount + K));
		newAmount *= multiplier;

		// Absorb X percentage of the Damage
		float absorbAmount = dmgInfo.GetDamage() * newAmount;

		if (absorbAmount > 0)
		{
			dmgInfo.AbsorbDamage((uint)absorbAmount);

			// Cast stagger and make it tick on each tick
			AddAndRefreshStagger(absorbAmount);
		}
	}

	private void AddAndRefreshStagger(float amount)
	{
		Unit target      = GetTarget();
		Aura auraStagger = FindExistingStaggerEffect(target);

		if (auraStagger != null)
		{
			AuraEffect effStaggerRemaining = auraStagger.GetEffect(1);

			if (effStaggerRemaining == null)
				return;

			float newAmount = effStaggerRemaining.GetAmount() + amount;
			uint  spellId   = GetStaggerSpellId(target, newAmount);

			if (spellId == effStaggerRemaining.GetSpellInfo().Id)
			{
				auraStagger.RefreshDuration();
				effStaggerRemaining.ChangeAmount((int)newAmount, false, true /* reapply */);
			}
			else
			{
				// amount changed the stagger Type so we need to change the stagger amount (e.g. from medium to light)
				GetTarget().RemoveAura(auraStagger);
				AddNewStagger(target, spellId, newAmount);
			}
		}
		else
		{
			AddNewStagger(target, GetStaggerSpellId(target, amount), amount);
		}
	}

	private uint GetStaggerSpellId(Unit unit, float amount)
	{
		const float StaggerHeavy    = 0.6f;
		const float StaggerModerate = 0.3f;

		float staggerPct = amount / unit.GetMaxHealth();

		return (staggerPct >= StaggerHeavy)    ? MonkSpells.StaggerHeavy :
		       (staggerPct >= StaggerModerate) ? MonkSpells.StaggerModerate :
		                                         MonkSpells.StaggerLight;
	}

	private void AddNewStagger(Unit unit, uint staggerSpellId, float staggerAmount)
	{
		// We only set the total stagger amount. The amount per tick will be set by the stagger spell script
		unit.CastSpell(unit, staggerSpellId, new CastSpellExtraArgs(SpellValueMod.BasePoint1, (int)staggerAmount).SetTriggerFlags(TriggerCastFlags.FullMask));
	}
}