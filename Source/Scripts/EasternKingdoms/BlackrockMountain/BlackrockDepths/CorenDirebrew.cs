// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Maps;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.IAura;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.EasternKingdoms.BlackrockMountain.BlackrockDepths.CorenDirebrew;

internal struct SpellIds
{
	public const uint MoleMachineEmerge = 50313;
	public const uint DirebrewDisarmPreCast = 47407;
	public const uint MoleMachineTargetPicker = 47691;
	public const uint MoleMachineMinionSummoner = 47690;
	public const uint DirebrewDisarmGrow = 47409;
	public const uint DirebrewDisarm = 47310;
	public const uint ChuckMug = 50276;
	public const uint PortToCoren = 52850;
	public const uint SendMugControlAura = 47369;
	public const uint SendMugTargetPicker = 47370;
	public const uint SendFirstMug = 47333;
	public const uint SendSecondMug = 47339;
	public const uint RequestSecondMug = 47344;
	public const uint HasDarkBrewmaidensBrew = 47331;
	public const uint BarreledControlAura = 50278;
	public const uint Barreled = 47442;
}

internal struct TextIds
{
	public const uint SayIntro = 0;
	public const uint SayIntro1 = 1;
	public const uint SayIntro2 = 2;
	public const uint SayInsult = 3;
	public const uint SayAntagonist1 = 0;
	public const uint SayAntagonist2 = 1;
	public const uint SayAntagonistCombat = 2;
}

internal struct ActionIds
{
	public const int StartFight = -1;
	public const int AntagonistSay1 = -2;
	public const int AntagonistSay2 = -3;
	public const int AntagonistHostile = -4;
}

internal struct CreatureIds
{
	public const uint IlsaDirebrew = 26764;
	public const uint UrsulaDirebrew = 26822;
	public const uint Antagonist = 23795;
}

internal enum DirebrewPhases
{
	All = 1,
	Intro,
	One,
	Two,
	Three
}

internal struct MiscConst
{
	public const uint GossipId = 11388;
	public const uint GoMoleMachineTrap = 188509;
	public const uint GossipOptionFight = 0;
	public const uint GossipOptionApologize = 1;
	public const int DataTargetGuid = 1;
	public const uint MaxAntagonists = 3;

	public static Position[] AntagonistPos =
	{
		new(895.3782f, -132.1722f, -49.66423f, 2.6529f), new(893.9837f, -133.2879f, -49.66541f, 2.583087f), new(896.2667f, -130.483f, -49.66249f, 2.600541f)
	};
}

[Script]
internal class boss_coren_direbrew : BossAI
{
	private DirebrewPhases phase;

	public boss_coren_direbrew(Creature creature) : base(creature, DataTypes.DataCoren) { }

	public override bool OnGossipSelect(Player player, uint menuId, uint gossipListId)
	{
		if (menuId != MiscConst.GossipId)
			return false;

		if (gossipListId == MiscConst.GossipOptionFight)
		{
			Talk(TextIds.SayInsult, player);
			DoAction(ActionIds.StartFight);
		}
		else if (gossipListId == MiscConst.GossipOptionApologize)
		{
			player.CloseGossipMenu();
		}

		return false;
	}

	public override void Reset()
	{
		_Reset();
		Me.SetImmuneToPC(true);
		Me.Faction = (uint)FactionTemplates.Friendly;
		phase = DirebrewPhases.All;
		SchedulerProtected.CancelAll();

		for (byte i = 0; i < MiscConst.MaxAntagonists; ++i)
			Me.SummonCreature(CreatureIds.Antagonist, MiscConst.AntagonistPos[i], TempSummonType.DeadDespawn);
	}

	public override void EnterEvadeMode(EvadeReason why)
	{
		_EnterEvadeMode();
		Summons.DespawnAll();
		_DespawnAtEvade(TimeSpan.FromSeconds(10));
	}

	public override void MoveInLineOfSight(Unit who)
	{
		if (phase != DirebrewPhases.All ||
			!who.IsPlayer)
			return;

		phase = DirebrewPhases.Intro;

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(6),
									introTask1 =>
									{
										Talk(TextIds.SayIntro1);

										introTask1.Schedule(TimeSpan.FromSeconds(4),
															introTask2 =>
															{
																EntryCheckPredicate pred = new(CreatureIds.Antagonist);
																Summons.DoAction(ActionIds.AntagonistSay1, pred);

																introTask2.Schedule(TimeSpan.FromSeconds(3),
																					introlTask3 =>
																					{
																						Talk(TextIds.SayIntro2);
																						EntryCheckPredicate pred = new(CreatureIds.Antagonist);
																						Summons.DoAction(ActionIds.AntagonistSay2, pred);
																					});
															});
									});

		Talk(TextIds.SayIntro);
	}

	public override void DoAction(int action)
	{
		if (action == ActionIds.StartFight)
		{
			phase = DirebrewPhases.One;
			//events.SetPhase(PhaseOne);
			Me.SetImmuneToPC(false);
			Me.Faction = (uint)FactionTemplates.GoblinDarkIronBarPatron;
			DoZoneInCombat();

			EntryCheckPredicate pred = new(CreatureIds.Antagonist);
			Summons.DoAction(ActionIds.AntagonistHostile, pred);

			SchedulerProtected.Schedule(TimeSpan.FromSeconds(15),
										task =>
										{
											CastSpellExtraArgs args = new(TriggerCastFlags.FullMask);
											args.AddSpellMod(SpellValueMod.MaxTargets, 1);
											Me.CastSpell((WorldObject)null, SpellIds.MoleMachineTargetPicker, args);
											task.Repeat();
										});

			SchedulerProtected.Schedule(TimeSpan.FromSeconds(20),
										task =>
										{
											DoCastSelf(SpellIds.DirebrewDisarmPreCast, new CastSpellExtraArgs(true));
											task.Repeat();
										});
		}
	}

	public override void DamageTaken(Unit attacker, ref double damage, DamageEffectType damageType, SpellInfo spellInfo = null)
	{
		if (Me.HealthBelowPctDamaged(66, damage) &&
			phase == DirebrewPhases.One)
		{
			phase = DirebrewPhases.Two;
			SummonSister(CreatureIds.IlsaDirebrew);
		}
		else if (Me.HealthBelowPctDamaged(33, damage) &&
				phase == DirebrewPhases.Two)
		{
			phase = DirebrewPhases.Three;
			SummonSister(CreatureIds.UrsulaDirebrew);
		}
	}

	public override void SummonedCreatureDies(Creature summon, Unit killer)
	{
		if (summon.Entry == CreatureIds.IlsaDirebrew)
			SchedulerProtected.Schedule(TimeSpan.FromSeconds(1), task => { SummonSister(CreatureIds.IlsaDirebrew); });
		else if (summon.Entry == CreatureIds.UrsulaDirebrew)
			SchedulerProtected.Schedule(TimeSpan.FromSeconds(1), task => { SummonSister(CreatureIds.UrsulaDirebrew); });
	}

	public override void JustDied(Unit killer)
	{
		_JustDied();

		var players = Me.Map.Players;

		if (!players.Empty())
		{
			var group = players[0].Group;

			if (group)
				if (group.IsLFGGroup)
					Global.LFGMgr.FinishDungeon(group.GUID, 287, Me.Map);
		}
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim() &&
			phase != DirebrewPhases.Intro)
			return;

		SchedulerProtected.Update(diff, () => DoMeleeAttackIfReady());
	}

	private void SummonSister(uint entry)
	{
		Creature sister = Me.SummonCreature(entry, Me.Location, TempSummonType.DeadDespawn);

		if (sister)
			DoZoneInCombat(sister);
	}
}

internal class npc_coren_direbrew_sisters : ScriptedAI
{
	private ObjectGuid _targetGUID;

	public npc_coren_direbrew_sisters(Creature creature) : base(creature) { }

	public override void SetGUID(ObjectGuid guid, int id)
	{
		if (id == MiscConst.DataTargetGuid)
			_targetGUID = guid;
	}

	public override ObjectGuid GetGUID(int data)
	{
		if (data == MiscConst.DataTargetGuid)
			return _targetGUID;

		return ObjectGuid.Empty;
	}

	public override void JustEngagedWith(Unit who)
	{
		DoCastSelf(SpellIds.PortToCoren);

		if (Me.Entry == CreatureIds.UrsulaDirebrew)
			DoCastSelf(SpellIds.BarreledControlAura);
		else
			DoCastSelf(SpellIds.SendMugControlAura);

		SchedulerProtected.SetValidator(() => !Me.HasUnitState(UnitState.Casting));

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(2),
									mugChuck =>
									{
										var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, false, true, -(int)SpellIds.HasDarkBrewmaidensBrew);

										if (target)
											DoCast(target, SpellIds.ChuckMug);

										mugChuck.Repeat(TimeSpan.FromSeconds(4));
									});
	}

	public override void UpdateAI(uint diff)
	{
		SchedulerProtected.Update(diff, () => DoMeleeAttackIfReady());
	}
}

internal class npc_direbrew_minion : ScriptedAI
{
	private readonly InstanceScript _instance;

	public npc_direbrew_minion(Creature creature) : base(creature)
	{
		_instance = creature.InstanceScript;
	}

	public override void Reset()
	{
		Me.Faction = (uint)FactionTemplates.GoblinDarkIronBarPatron;
		DoZoneInCombat();
	}

	public override void IsSummonedBy(WorldObject summoner)
	{
		var coren = ObjectAccessor.GetCreature(Me, _instance.GetGuidData(DataTypes.DataCoren));

		if (coren)
			coren.AI.JustSummoned(Me);
	}
}

internal class npc_direbrew_antagonist : ScriptedAI
{
	public npc_direbrew_antagonist(Creature creature) : base(creature) { }

	public override void DoAction(int action)
	{
		switch (action)
		{
			case ActionIds.AntagonistSay1:
				Talk(TextIds.SayAntagonist1);

				break;
			case ActionIds.AntagonistSay2:
				Talk(TextIds.SayAntagonist2);

				break;
			case ActionIds.AntagonistHostile:
				Me.SetImmuneToPC(false);
				Me.Faction = (uint)FactionTemplates.GoblinDarkIronBarPatron;
				DoZoneInCombat();

				break;
			default:
				break;
		}
	}

	public override void JustEngagedWith(Unit who)
	{
		Talk(TextIds.SayAntagonistCombat, who);
		base.JustEngagedWith(who);
	}
}

internal class go_direbrew_mole_machine : GameObjectAI
{
	public go_direbrew_mole_machine(GameObject go) : base(go) { }

	public override void Reset()
	{
		Me.SetLootState(LootState.Ready);

		Scheduler.Schedule(TimeSpan.FromSeconds(1),
							context =>
							{
								Me.UseDoorOrButton(10000);
								Me.CastSpell(null, SpellIds.MoleMachineEmerge, true);
							});

		Scheduler.Schedule(TimeSpan.FromSeconds(4),
							context =>
							{
								var trap = Me.LinkedTrap;

								if (trap)
								{
									trap.SetLootState(LootState.Activated);
									trap.UseDoorOrButton();
								}
							});
	}

	public override void UpdateAI(uint diff)
	{
		Scheduler.Update(diff);
	}
}

// 47691 - Summon Mole Machine Target Picker
internal class spell_direbrew_summon_mole_machine_target_picker : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScriptEffect, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScriptEffect(int effIndex)
	{
		Caster.CastSpell(HitUnit, SpellIds.MoleMachineMinionSummoner, true);
	}
}

// 47370 - Send Mug Target Picker
internal class spell_send_mug_target_picker : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 0, Targets.UnitSrcAreaEntry));
		SpellEffects.Add(new EffectHandler(HandleDummy, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void FilterTargets(List<WorldObject> targets)
	{
		var caster = Caster;

		targets.RemoveAll(new UnitAuraCheck<WorldObject>(true, SpellIds.HasDarkBrewmaidensBrew));

		if (targets.Count > 1)
			targets.RemoveAll(obj =>
			{
				if (obj.GUID == caster.AI.GetGUID(MiscConst.DataTargetGuid))
					return true;

				return false;
			});

		if (targets.Empty())
			return;

		var target = targets.SelectRandom();
		targets.Clear();
		targets.Add(target);
	}

	private void HandleDummy(int effIndex)
	{
		var caster = Caster;
		caster.AI.SetGUID(HitUnit.GUID, MiscConst.DataTargetGuid);
		caster.CastSpell(HitUnit, SpellIds.SendFirstMug, true);
	}
}

// 47344 - Request Second Mug
internal class spell_request_second_mug : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleScriptEffect, 0, SpellEffectName.ScriptEffect, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleScriptEffect(int effIndex)
	{
		HitUnit.CastSpell(Caster, SpellIds.SendSecondMug, true);
	}
}

// 47369 - Send Mug Control Aura
internal class spell_send_mug_control_aura : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(PeriodicTick, 0, AuraType.PeriodicDummy));
	}

	private void PeriodicTick(AuraEffect aurEff)
	{
		Target.CastSpell(Target, SpellIds.SendMugTargetPicker, true);
	}
}

// 50278 - Barreled Control Aura
internal class spell_barreled_control_aura : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(PeriodicTick, 0, AuraType.PeriodicTriggerSpell));
	}

	private void PeriodicTick(AuraEffect aurEff)
	{
		PreventDefaultAction();
		Target.CastSpell(null, SpellIds.Barreled, true);
	}
}

// 47407 - Direbrew's Disarm (precast)
internal class spell_direbrew_disarm : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(PeriodicTick, 1, AuraType.PeriodicDummy));
		AuraEffects.Add(new AuraEffectApplyHandler(OnApply, 1, AuraType.PeriodicDummy, AuraEffectHandleModes.Real, AuraScriptHookType.EffectApply));
	}

	private void PeriodicTick(AuraEffect aurEff)
	{
		var aura = Target.GetAura(SpellIds.DirebrewDisarmGrow);

		if (aura != null)
		{
			aura.SetStackAmount((byte)(aura.StackAmount + 1));
			aura.SetDuration(aura.Duration - 1500);
		}
	}

	private void OnApply(AuraEffect aurEff, AuraEffectHandleModes mode)
	{
		Target.CastSpell(Target, SpellIds.DirebrewDisarmGrow, true);
		Target.CastSpell(Target, SpellIds.DirebrewDisarm);
	}
}