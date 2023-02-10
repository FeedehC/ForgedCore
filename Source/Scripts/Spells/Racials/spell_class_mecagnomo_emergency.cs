﻿using System.Collections.Generic;
using Framework.Constants;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.IAura;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Spells.Racials
{
	[SpellScript(312916)]
	public class spell_class_mecagnomo_emergency : AuraScript, IAuraCheckProc, IHasAuraEffects
	{
		public List<IAuraEffectHandler> AuraEffects => new();

		private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
		{
			PreventDefaultAction();
			var caster = GetCaster();

			var triggerOnHealth = caster.CountPctFromMaxHealth(aurEff.GetAmount());
			var currentHealth   = caster.GetHealth();

			// Just falling below threshold
			if (currentHealth > triggerOnHealth && (currentHealth - caster.GetMaxHealth() * 25.0f / 100.0f) <= triggerOnHealth)
				caster.CastSpell(caster, 313010);
		}


		public bool CheckProc(ProcEventInfo eventInfo)
		{
			var caster = GetCaster();

			if (caster.HasAuraState(AuraStateType.Wounded20Percent))
			{
				caster.ModifyAuraState(AuraStateType.Wounded20Percent, false);

				return true;
			}

			return false;
		}


		public override void Register()
		{
			AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.ProcTriggerSpell, AuraScriptHookType.EffectProc));
		}
	}

	[SpellScript(313015)]
	public class spell_class_mecagnomo_emergency2 : AuraScript, IHasAuraEffects
	{
		public List<IAuraEffectHandler> AuraEffects => new();


		private void HandleHit(AuraEffect UnnamedParameter, AuraEffectHandleModes UnnamedParameter2)
		{
			if (!GetCaster().HasAura(313010))
				PreventDefaultAction();
		}

		public override void Register()
		{
			AuraEffects.Add(new AuraEffectApplyHandler(HandleHit, 0, AuraType.Dummy, AuraEffectHandleModes.Real));
		}
	}

	[SpellScript(313010)]
	public class spell_class_mecagnomo_emergency3 : SpellScript, IHasSpellEffects
	{
		public List<ISpellEffect> SpellEffects => new();

		private void HandleHit(uint effIndex)
		{
			if (!GetCaster().HasAura(313015))
				PreventHitDefaultEffect(effIndex);
		}

		private void HandleHeal(uint effIndex)
		{
			var caster = GetCaster();
			var heal   = caster.GetMaxHealth() * 25.0f / 100.0f;
			//caster->SpellHealingBonusDone(caster, GetSpellInfo(), caster->CountPctFromMaxHealth(GetSpellInfo()->GetEffect(effIndex)->BasePoints), DamageEffectType.Heal, GetEffectInfo());
			heal = caster.SpellHealingBonusTaken(caster, GetSpellInfo(), (uint)heal, DamageEffectType.Heal);
			SetHitHeal((int)heal);
			caster.CastSpell(caster, 313015, true);

			PreventHitDefaultEffect(effIndex);
		}

		public override void Register()
		{
			SpellEffects.Add(new EffectHandler(HandleHeal, 0, SpellEffectName.HealPct, SpellScriptHookType.EffectHitTarget));
			SpellEffects.Add(new EffectHandler(HandleHit, 1, SpellEffectName.TriggerSpell, SpellScriptHookType.Launch));
		}
	}
}