// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Maps;
using Game.Scripting;
using Game.Scripting.Interfaces.IAura;
using Game.Spells;

namespace Scripts.EasternKingdoms.Karazhan.Nightbane;

internal struct SpellIds
{
	public const uint BellowingRoar = 36922;
	public const uint CharredEarth = 30129;
	public const uint Cleave = 30131;
	public const uint DistractingAsh = 30130;
	public const uint RainOfBones = 37098;
	public const uint SmokingBlast = 30128;
	public const uint SmokingBlastT = 37057;
	public const uint SmolderingBreath = 30210;
	public const uint SummonSkeleton = 30170;
	public const uint TailSweep = 25653;
}

internal struct TextIds
{
	public const uint EmoteSummon = 0;
	public const uint YellAggro = 1;
	public const uint YellFlyPhase = 2;
	public const uint YellLandPhase = 3;
	public const uint EmoteBreath = 4;
}

internal struct PointIds
{
	public const uint IntroStart = 0;
	public const uint IntroEnd = 1;
	public const uint IntroLanding = 2;
	public const uint PhaseTwoFly = 3;
	public const uint PhaseTwoPreFly = 4;
	public const uint PhaseTwoLanding = 5;
	public const uint PhaseTwoEnd = 6;
}

internal struct SplineChainIds
{
	public const uint IntroStart = 1;
	public const uint IntroEnd = 2;
	public const uint IntroLanding = 3;
	public const uint SecondLanding = 4;
	public const uint PhaseTwo = 5;
}

internal enum NightbanePhases
{
	Intro = 0,
	Ground,
	Fly
}

internal struct MiscConst
{
	public const int ActionSummon = 0;
	public const uint PathPhaseTwo = 13547500;

	public const uint GroupGround = 1;
	public const uint GroupFly = 2;

	public static Position FlyPosition = new(-11160.13f, -1870.683f, 97.73876f, 0.0f);
	public static Position FlyPositionLeft = new(-11094.42f, -1866.992f, 107.8375f, 0.0f);
	public static Position FlyPositionRight = new(-11193.77f, -1921.983f, 107.9845f, 0.0f);
}

[Script]
internal class boss_nightbane : BossAI
{
	private byte _flyCount;
	private NightbanePhases phase;

	public boss_nightbane(Creature creature) : base(creature, DataTypes.Nightbane) { }

	public override void Reset()
	{
		_Reset();
		_flyCount = 0;
		Me.SetDisableGravity(true);
		HandleTerraceDoors(true);
		var urn = ObjectAccessor.GetGameObject(Me, Instance.GetGuidData(DataTypes.GoBlackenedUrn));

		if (urn)
			urn.RemoveFlag(GameObjectFlags.InUse);
	}

	public override void EnterEvadeMode(EvadeReason why)
	{
		Me.SetDisableGravity(true);
		base.EnterEvadeMode(why);
	}

	public override void JustReachedHome()
	{
		_DespawnAtEvade();
	}

	public override void JustDied(Unit killer)
	{
		_JustDied();
		HandleTerraceDoors(true);
	}

	public override void DoAction(int action)
	{
		if (action == MiscConst.ActionSummon)
		{
			Talk(TextIds.EmoteSummon);
			phase = NightbanePhases.Intro;
			Me.SetActive(true);
			Me.SetFarVisible(true);
			Me.RemoveUnitFlag(UnitFlags.Uninteractible);
			Me.MotionMaster.MoveAlongSplineChain(PointIds.IntroStart, SplineChainIds.IntroStart, false);
			HandleTerraceDoors(false);
		}
	}

	public override void JustEngagedWith(Unit who)
	{
		base.JustEngagedWith(who);
		Talk(TextIds.YellAggro);
		SetupGroundPhase();
	}

	public override void DamageTaken(Unit attacker, ref double damage, DamageEffectType damageType, SpellInfo spellInfo = null)
	{
		if (phase == NightbanePhases.Fly)
		{
			if (damage >= Me.Health)
				damage = (uint)(Me.Health - 1);

			return;
		}

		if ((_flyCount == 0 && HealthBelowPct(75)) ||
			(_flyCount == 1 && HealthBelowPct(50)) ||
			(_flyCount == 2 && HealthBelowPct(25)))
		{
			phase = NightbanePhases.Fly;
			StartPhaseFly();
		}
	}

	public override void MovementInform(MovementGeneratorType type, uint pointId)
	{
		if (type == MovementGeneratorType.SplineChain)
		{
			switch (pointId)
			{
				case PointIds.IntroStart:
					Me.SetStandState(UnitStandStateType.Stand);
					SchedulerProtected.Schedule(TimeSpan.FromMilliseconds(1), task => { Me.MotionMaster.MoveAlongSplineChain(PointIds.IntroEnd, SplineChainIds.IntroEnd, false); });

					break;
				case PointIds.IntroEnd:
					SchedulerProtected.Schedule(TimeSpan.FromSeconds(2), task => { Me.MotionMaster.MoveAlongSplineChain(PointIds.IntroLanding, SplineChainIds.IntroLanding, false); });

					break;
				case PointIds.IntroLanding:
					Me.SetDisableGravity(false);
					Me.HandleEmoteCommand(Emote.OneshotLand);

					SchedulerProtected.Schedule(TimeSpan.FromSeconds(3),
												task =>
												{
													Me.SetImmuneToPC(false);
													DoZoneInCombat();
												});

					break;
				case PointIds.PhaseTwoLanding:
					phase = NightbanePhases.Ground;
					Me.SetDisableGravity(false);
					Me.HandleEmoteCommand(Emote.OneshotLand);

					SchedulerProtected.Schedule(TimeSpan.FromSeconds(3),
												task =>
												{
													SetupGroundPhase();
													Me.ReactState = ReactStates.Aggressive;
												});

					break;
				case PointIds.PhaseTwoEnd:
					SchedulerProtected.Schedule(TimeSpan.FromMilliseconds(1), task => { Me.MotionMaster.MoveAlongSplineChain(PointIds.PhaseTwoLanding, SplineChainIds.SecondLanding, false); });

					break;
				default:
					break;
			}
		}
		else if (type == MovementGeneratorType.Point)
		{
			if (pointId == PointIds.PhaseTwoFly)
			{
				SchedulerProtected.Schedule(TimeSpan.FromSeconds(33),
											MiscConst.GroupFly,
											task =>
											{
												SchedulerProtected.CancelGroup(MiscConst.GroupFly);

												SchedulerProtected.Schedule(TimeSpan.FromSeconds(2),
																			MiscConst.GroupGround,
																			landTask =>
																			{
																				Talk(TextIds.YellLandPhase);
																				Me.SetDisableGravity(true);
																				Me.MotionMaster.MoveAlongSplineChain(PointIds.PhaseTwoEnd, SplineChainIds.PhaseTwo, false);
																			});
											});

				SchedulerProtected.Schedule(TimeSpan.FromSeconds(2),
											MiscConst.GroupFly,
											task =>
											{
												Talk(TextIds.EmoteBreath);

												task.Schedule(TimeSpan.FromSeconds(3),
															MiscConst.GroupFly,
															somethingTask =>
															{
																ResetThreatList();
																var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, true);

																if (target)
																{
																	Me.SetFacingToObject(target);
																	DoCast(target, SpellIds.RainOfBones);
																}
															});
											});

				SchedulerProtected.Schedule(TimeSpan.FromSeconds(21),
											MiscConst.GroupFly,
											task =>
											{
												var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, true);

												if (target)
													DoCast(target, SpellIds.SmokingBlastT);

												task.Repeat(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(7));
											});

				SchedulerProtected.Schedule(TimeSpan.FromSeconds(17),
											MiscConst.GroupFly,
											task =>
											{
												var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, true);

												if (target)
													DoCast(target, SpellIds.SmokingBlast);

												task.Repeat(TimeSpan.FromMilliseconds(1400));
											});
			}
			else if (pointId == PointIds.PhaseTwoPreFly)
			{
				SchedulerProtected.Schedule(TimeSpan.FromMilliseconds(1), task => { Me.MotionMaster.MovePoint(PointIds.PhaseTwoFly, MiscConst.FlyPosition, true); });
			}
		}
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim() &&
			phase != NightbanePhases.Intro)
			return;

		SchedulerProtected.Update(diff, () => DoMeleeAttackIfReady());
	}

	private void SetupGroundPhase()
	{
		phase = NightbanePhases.Ground;

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(0),
									TimeSpan.FromSeconds(15),
									MiscConst.GroupGround,
									task =>
									{
										DoCastVictim(SpellIds.Cleave);
										task.Repeat(TimeSpan.FromSeconds(6), TimeSpan.FromSeconds(15));
									});

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(4),
									TimeSpan.FromSeconds(23),
									MiscConst.GroupGround,
									task =>
									{
										var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, true);

										if (target)
											if (!Me.Location.HasInArc(MathF.PI, target.Location))
												DoCast(target, SpellIds.TailSweep);

										task.Repeat(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(30));
									});

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(48), MiscConst.GroupGround, task => { DoCastAOE(SpellIds.BellowingRoar); });

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(12),
									TimeSpan.FromSeconds(18),
									MiscConst.GroupGround,
									task =>
									{
										var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, true);

										if (target)
											DoCast(target, SpellIds.CharredEarth);

										task.Repeat(TimeSpan.FromSeconds(18), TimeSpan.FromSeconds(21));
									});

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(26),
									TimeSpan.FromSeconds(30),
									MiscConst.GroupGround,
									task =>
									{
										DoCastVictim(SpellIds.SmolderingBreath);
										task.Repeat(TimeSpan.FromSeconds(28), TimeSpan.FromSeconds(40));
									});

		SchedulerProtected.Schedule(TimeSpan.FromSeconds(82),
									MiscConst.GroupGround,
									task =>
									{
										var target = SelectTarget(SelectTargetMethod.Random, 0, 0.0f, true);

										if (target)
											DoCast(target, SpellIds.DistractingAsh);
									});
	}

	private void HandleTerraceDoors(bool open)
	{
		Instance.HandleGameObject(Instance.GetGuidData(DataTypes.MastersTerraceDoor1), open);
		Instance.HandleGameObject(Instance.GetGuidData(DataTypes.MastersTerraceDoor2), open);
	}

	private void StartPhaseFly()
	{
		++_flyCount;
		Talk(TextIds.YellFlyPhase);
		SchedulerProtected.CancelGroup(MiscConst.GroupGround);
		Me.InterruptNonMeleeSpells(false);
		Me.HandleEmoteCommand(Emote.OneshotLiftoff);
		Me.SetDisableGravity(true);
		Me.ReactState = ReactStates.Passive;
		Me.AttackStop();

		if (Me.GetDistance(MiscConst.FlyPositionLeft) < Me.GetDistance(MiscConst.FlyPosition))
			Me.MotionMaster.MovePoint(PointIds.PhaseTwoPreFly, MiscConst.FlyPositionLeft, true);
		else if (Me.GetDistance(MiscConst.FlyPositionRight) < Me.GetDistance(MiscConst.FlyPosition))
			Me.MotionMaster.MovePoint(PointIds.PhaseTwoPreFly, MiscConst.FlyPositionRight, true);
		else
			Me.MotionMaster.MovePoint(PointIds.PhaseTwoFly, MiscConst.FlyPosition, true);
	}
}

[Script] // 37098 - Rain of Bones
internal class spell_rain_of_bones_AuraScript : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(OnTrigger, 1, AuraType.PeriodicTriggerSpell));
	}

	private void OnTrigger(AuraEffect aurEff)
	{
		if (aurEff.GetTickNumber() % 5 == 0)
			Target.CastSpell(Target, SpellIds.SummonSkeleton, true);
	}
}

[Script]
internal class go_blackened_urn : GameObjectAI
{
	private readonly InstanceScript instance;

	public go_blackened_urn(GameObject go) : base(go)
	{
		instance = go.InstanceScript;
	}

	public override bool OnGossipHello(Player player)
	{
		if (Me.HasFlag(GameObjectFlags.InUse))
			return false;

		if (instance.GetBossState(DataTypes.Nightbane) == EncounterState.Done ||
			instance.GetBossState(DataTypes.Nightbane) == EncounterState.InProgress)
			return false;

		var nightbane = ObjectAccessor.GetCreature(Me, instance.GetGuidData(DataTypes.Nightbane));

		if (nightbane)
		{
			Me.SetFlag(GameObjectFlags.InUse);
			nightbane.AI.DoAction(MiscConst.ActionSummon);
		}

		return false;
	}
}