﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Mage;

[CreatureScript(31216)]
public class npc_mirror_imageAI : CasterAI
{
	public npc_mirror_imageAI(Creature creature) : base(creature) { }

	public override void IsSummonedBy(WorldObject owner)
	{
		if (owner == null || !owner.IsPlayer())
			return;

		if (!me.HasUnitState(UnitState.Follow))
		{
			me.GetMotionMaster().Clear();
			me.GetMotionMaster().MoveFollow(owner.ToUnit(), SharedConst.PetFollowDist, me.GetFollowAngle(), MovementSlot.Active);
		}

		// me->SetMaxPower(me->GetPowerType(), owner->GetMaxPower(me->GetPowerType()));
		me.SetFullPower(me.GetPowerType());
		me.SetMaxHealth(owner.ToUnit().GetMaxHealth());
		me.SetHealth(owner.ToUnit().GetHealth());
		me.SetReactState(ReactStates.Defensive);

		me.CastSpell(owner, eSpells.INHERIT_MASTER_THREAT, true);

		// here mirror image casts on summoner spell (not present in client dbc) 49866
		// here should be auras (not present in client dbc): 35657, 35658, 35659, 35660 selfcasted by mirror images (stats related?)

		for (uint attackType = 0; attackType < (int)WeaponAttackType.Max; ++attackType)
		{
			var attackTypeEnum = (WeaponAttackType)attackType;
			me.SetBaseWeaponDamage(attackTypeEnum, WeaponDamageRange.MaxDamage, owner.ToUnit().GetWeaponDamageRange(attackTypeEnum, WeaponDamageRange.MaxDamage));
			me.SetBaseWeaponDamage(attackTypeEnum, WeaponDamageRange.MinDamage, owner.ToUnit().GetWeaponDamageRange(attackTypeEnum, WeaponDamageRange.MinDamage));
		}

		me.UpdateAttackPowerAndDamage();
	}

	public override void JustEngagedWith(Unit who)
	{
		var owner = me.GetOwner();

		if (owner == null)
			return;

		var ownerPlayer = owner.ToPlayer();

		if (ownerPlayer == null)
			return;

		var spellId = eSpells.FROSTBOLT;

		switch (ownerPlayer.GetPrimarySpecialization())
		{
			case TalentSpecialization.MageArcane:
				spellId = eSpells.ARCANE_BLAST;

				break;
			case TalentSpecialization.MageFire:
				spellId = eSpells.FIREBALL;

				break;
			default:
				break;
		}

		_events.ScheduleEvent(spellId, TimeSpan.Zero); ///< Schedule cast
		me.GetMotionMaster().Clear();
	}

	public override void EnterEvadeMode(EvadeReason UnnamedParameter)
	{
		if (me.IsInEvadeMode() || !me.IsAlive())
			return;

		var owner = me.GetOwner();

		me.CombatStop(true);

		if (owner != null && !me.HasUnitState(UnitState.Follow))
		{
			me.GetMotionMaster().Clear();
			me.GetMotionMaster().MoveFollow(owner.ToUnit(), SharedConst.PetFollowDist, me.GetFollowAngle(), MovementSlot.Active);
		}
	}

	public override void Reset()
	{
		var owner = me.GetOwner();

		if (owner != null)
		{
			owner.CastSpell(me, eSpells.INITIALIZE_IMAGES, true);
			owner.CastSpell(me, eSpells.CLONE_CASTER, true);
		}
	}


	public override bool CanAIAttack(Unit target)
	{
		/// Am I supposed to attack this target? (ie. do not attack polymorphed target)
		return target != null && !target.HasBreakableByDamageCrowdControlAura();
	}

	public override void UpdateAI(uint diff)
	{
		_events.Update(diff);

		var l_Victim = me.GetVictim();

		if (l_Victim != null)
		{
			if (CanAIAttack(l_Victim))
			{
				/// If not already casting, cast! ("I'm a cast machine")
				if (!me.HasUnitState(UnitState.Casting))
				{
					var spellId = _events.ExecuteEvent();

					if (_events.ExecuteEvent() != 0)
					{
						DoCast(spellId);
						var castTime = me.GetCurrentSpellCastTime(spellId);
						_events.ScheduleEvent(spellId, TimeSpan.FromSeconds(5), Global.SpellMgr.GetSpellInfo(spellId, Difficulty.None).ProcCooldown);
					}
				}
			}
			else
			{
				/// My victim has changed state, I shouldn't attack it anymore
				if (me.HasUnitState(UnitState.Casting))
					me.CastStop();

				me.GetAI().EnterEvadeMode();
			}
		}
		else
		{
			/// Let's choose a new target
			var target = me.SelectVictim();

			if (target == null)
			{
				/// No target? Let's see if our owner has a better target for us
				var owner = me.GetOwner();

				if (owner != null)
				{
					var ownerVictim = owner.GetVictim();

					if (ownerVictim != null && me.CanCreatureAttack(ownerVictim))
						target = ownerVictim;
				}
			}

			if (target != null)
				me.GetAI().AttackStart(target);
		}
	}

	public struct eSpells
	{
		public const uint FROSTBOLT = 59638;
		public const uint FIREBALL = 133;
		public const uint ARCANE_BLAST = 30451;
		public const uint GLYPH = 63093;
		public const uint INITIALIZE_IMAGES = 102284;
		public const uint CLONE_CASTER = 60352;
		public const uint INHERIT_MASTER_THREAT = 58838;
	}
}