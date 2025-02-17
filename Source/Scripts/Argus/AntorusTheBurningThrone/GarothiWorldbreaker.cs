// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;
using Game.Scripting.Interfaces;
using Game.Scripting.Interfaces.IAreaTrigger;
using Game.Scripting.Interfaces.IAura;
using Game.Scripting.Interfaces.ISpell;
using Game.Spells;

namespace Scripts.Argus.AntorusTheBurningThrone.GarothiWorldbreaker;

internal struct TextIds
{
	// Garothi Worldbreaker
	public const uint SayAggro = 0;
	public const uint SayDisengage = 1;
	public const uint SayAnnounceApocalypseDrive = 2;
	public const uint SayApocalypseDrive = 3;
	public const uint SayAnnounceEradication = 4;
	public const uint SayFinishApocalypseDrive = 5;
	public const uint SayDecimation = 6;
	public const uint SayAnnihilation = 7;
	public const uint SayAnnounceFelBombardment = 8;
	public const uint SaySlay = 9;
	public const uint SayDeath = 10;

	// Decimator
	public const uint SayAnnounceDecimation = 0;
}

internal struct SpellIds
{
	// Garothi Worldbreaker
	public const uint Melee = 248229;
	public const uint ApocalypseDrive = 244152;
	public const uint ApocalypseDrivePeriodicDamage = 253300;
	public const uint ApocalypseDriveFinalDamage = 240277;
	public const uint Eradication = 244969;
	public const uint Empowered = 245237;
	public const uint RestoreHealth = 246012;
	public const uint AnnihilatorCannonEject = 245527;
	public const uint DecimatorCannonEject = 245515;
	public const uint FelBombardmentSelector = 244150;
	public const uint FelBombardmentWarning = 246220;
	public const uint FelBombardmentDummy = 245219;
	public const uint FelBombardmentPeriodic = 244536;
	public const uint CannonChooser = 245124;
	public const uint SearingBarrageAnnihilator = 246368;
	public const uint SearingBarrageDecimator = 244395;
	public const uint SearingBarrageDummyAnnihilator = 244398;
	public const uint SearingBarrageDummyDecimator = 246369;
	public const uint SearingBarrageSelector = 246360;
	public const uint SearingBarrageDamageAnnihilator = 244400;
	public const uint SearingBarrageDamageDecimator = 246373;
	public const uint Carnage = 244106;

	// Decimator
	public const uint DecimationSelector = 244399;
	public const uint DecimationWarning = 244410;
	public const uint DecimationCastVisual = 245338;
	public const uint DecimationMissile = 244448;

	// Annihilator
	public const uint AnnihilationSummon = 244790;
	public const uint AnnihilationSelector = 247572;
	public const uint AnnihilationDummy = 244294;
	public const uint AnnihilationDamageUnsplitted = 244762;

	// Annihilation
	public const uint AnnihilationAreaTrigger = 244795;
	public const uint AnnihilationWarning = 244799;

	// Garothi Worldbreaker (Surging Fel)
	public const uint SurgingFelAreaTrigger = 246655;
	public const uint SurgingFelDamage = 246663;
}

internal struct EventIds
{
	// Garothi Worldbreaker
	public const uint ReengagePlayers = 1;
	public const uint FelBombardment = 2;
	public const uint SearingBarrage = 3;
	public const uint CannonChooser = 4;
	public const uint SurgingFel = 5;
}

internal struct MiscConst
{
	public const uint MinTargetsSize = 2;
	public const uint MaxTargetsSize = 6;

	public const byte SummonGroupIdSurgingFel = 0;
	public const ushort AnimKitIdCannonDestroyed = 13264;
	public const uint DataLastFiredCannon = 0;

	public const uint MaxApocalypseDriveCount = 2;
	public static Position AnnihilationCenterReferencePos = new(-3296.72f, 9767.78f, -60.0f);

	public static void PreferNonTankTargetsAndResizeTargets(List<WorldObject> targets, Unit caster)
	{
		if (targets.Empty())
			return;

		var targetsCopy = targets;
		var size = (byte)targetsCopy.Count;
		// Selecting our prefered Target size based on total targets (min 10 player: 2, max 30 player: 6)
		var preferedSize = (byte)(Math.Min(Math.Max(MathF.Ceiling(size / 5), MinTargetsSize), MaxTargetsSize));

		// Now we get rid of the tank as these abilities prefer non-tanks above tanks as long as there are alternatives
		targetsCopy.RemoveAll(new VictimCheck(caster, false));

		// We have less available nontank targets than we want, include tanks
		if (targetsCopy.Count < preferedSize)
		{
			targets.RandomResize(preferedSize);
		}
		else
		{
			// Our Target list has enough alternative targets, resize
			targetsCopy.RandomResize(preferedSize);
			targets.Clear();
			targets.AddRange(targetsCopy);
		}
	}
}

[Script]
internal class boss_garothi_worldbreaker : BossAI
{
	private readonly byte[] _apocalypseDriveHealthLimit = new byte[MiscConst.MaxApocalypseDriveCount];
	private readonly List<ObjectGuid> _surgingFelDummyGuids = new();
	private byte _apocalypseDriveCount;
	private bool _castEradication;
	private uint _lastCanonEntry;
	private ObjectGuid _lastSurgingFelDummyGuid;
	private uint _searingBarrageSpellId;

	public boss_garothi_worldbreaker(Creature creature) : base(creature, DataTypes.GarothiWorldbreaker)
	{
		_lastCanonEntry = CreatureIds.Decimator;
		SetCombatMovement(false);
		Me.ReactState = ReactStates.Passive;
	}

	public override void InitializeAI()
	{
		switch (GetDifficulty())
		{
			case Difficulty.MythicRaid:
			case Difficulty.HeroicRaid:
				_apocalypseDriveHealthLimit[0] = 65;
				_apocalypseDriveHealthLimit[1] = 35;

				break;
			case Difficulty.NormalRaid:
			case Difficulty.LFRNew:
				_apocalypseDriveHealthLimit[0] = 60;
				_apocalypseDriveHealthLimit[1] = 20;

				break;
			default:
				break;
		}
	}

	public override void JustAppeared()
	{
		Me.SummonCreatureGroup(MiscConst.SummonGroupIdSurgingFel);
	}

	public override void JustEngagedWith(Unit who)
	{
		Me.ReactState = ReactStates.Aggressive;
		base.JustEngagedWith(who);
		Talk(TextIds.SayAggro);
		DoCastSelf(SpellIds.Melee);
		Instance.SendEncounterUnit(EncounterFrameType.Engage, Me);
		Events.ScheduleEvent(EventIds.FelBombardment, TimeSpan.FromSeconds(9));
		Events.ScheduleEvent(EventIds.CannonChooser, TimeSpan.FromSeconds(8));
	}

	public override void EnterEvadeMode(EvadeReason why)
	{
		Talk(TextIds.SayDisengage);
		_EnterEvadeMode();
		Instance.SendEncounterUnit(EncounterFrameType.Disengage, Me);
		Events.Reset();
		CleanupEncounter();
		_DespawnAtEvade(TimeSpan.FromSeconds(30));
	}

	public override void KilledUnit(Unit victim)
	{
		if (victim.IsPlayer)
			Talk(TextIds.SaySlay, victim);
	}

	public override void JustDied(Unit killer)
	{
		_JustDied();
		Talk(TextIds.SayDeath);
		CleanupEncounter();
		Instance.SendEncounterUnit(EncounterFrameType.Disengage, Me);
	}

	public override void OnSpellCast(SpellInfo spell)
	{
		switch (spell.Id)
		{
			case SpellIds.ApocalypseDriveFinalDamage:
				if (_apocalypseDriveCount < MiscConst.MaxApocalypseDriveCount)
					Events.Reset();

				Events.ScheduleEvent(EventIds.ReengagePlayers, TimeSpan.FromSeconds(3.5));
				HideCannons();
				Me.RemoveUnitFlag(UnitFlags.Uninteractible);

				break;
			default:
				break;
		}
	}

	public override void DamageTaken(Unit attacker, ref double damage, DamageEffectType damageType, SpellInfo spellInfo = null)
	{
		if (Me.HealthBelowPctDamaged(_apocalypseDriveHealthLimit[_apocalypseDriveCount], damage))
		{
			Me.AttackStop();
			Me.ReactState = ReactStates.Passive;
			Me.InterruptNonMeleeSpells(true);
			Me.SetFacingTo(Me.HomePosition.Orientation);
			Events.Reset();

			if (GetDifficulty() == Difficulty.MythicRaid ||
				GetDifficulty() == Difficulty.HeroicRaid)
				Events.ScheduleEvent(EventIds.SurgingFel, TimeSpan.FromSeconds(8));

			DoCastSelf(SpellIds.ApocalypseDrive);
			DoCastSelf(SpellIds.ApocalypseDriveFinalDamage);
			Talk(TextIds.SayAnnounceApocalypseDrive);
			Talk(TextIds.SayApocalypseDrive);
			Me.SetUnitFlag(UnitFlags.Uninteractible);

			var decimator = Instance.GetCreature(DataTypes.Decimator);

			if (decimator)
			{
				Instance.SendEncounterUnit(EncounterFrameType.Engage, decimator, 2);
				decimator.SetUnitFlag(UnitFlags.InCombat);
				decimator.RemoveUnitFlag(UnitFlags.Uninteractible);
			}

			var annihilator = Instance.GetCreature(DataTypes.Annihilator);

			if (annihilator)
			{
				Instance.SendEncounterUnit(EncounterFrameType.Engage, annihilator, 2);
				annihilator.SetUnitFlag(UnitFlags.InCombat);
				annihilator.RemoveUnitFlag(UnitFlags.Uninteractible);
			}

			++_apocalypseDriveCount;
		}
	}

	public override void JustSummoned(Creature summon)
	{
		Summons.Summon(summon);

		switch (summon.Entry)
		{
			case CreatureIds.Annihilation:
				summon.CastSpell(summon, SpellIds.AnnihilationWarning);
				summon.CastSpell(summon, SpellIds.AnnihilationAreaTrigger);

				break;
			case CreatureIds.Annihilator:
			case CreatureIds.Decimator:
				summon.ReactState = ReactStates.Passive;

				break;
			case CreatureIds.GarothiWorldbreaker:
				_surgingFelDummyGuids.Add(summon.GUID);

				break;
			default:
				break;
		}
	}

	public override void SummonedCreatureDies(Creature summon, Unit killer)
	{
		switch (summon.Entry)
		{
			case CreatureIds.Decimator:
			case CreatureIds.Annihilator:
				Me.InterruptNonMeleeSpells(true);
				Me.RemoveAura(SpellIds.ApocalypseDrive);
				Me.RemoveUnitFlag(UnitFlags.Uninteractible);

				if (summon.Entry == CreatureIds.Annihilator)
					_searingBarrageSpellId = SpellIds.SearingBarrageAnnihilator;
				else
					_searingBarrageSpellId = SpellIds.SearingBarrageDecimator;

				if (_apocalypseDriveCount < MiscConst.MaxApocalypseDriveCount)
					Events.Reset();

				Events.ScheduleEvent(EventIds.SearingBarrage, TimeSpan.FromSeconds(3.5));
				Events.ScheduleEvent(EventIds.ReengagePlayers, TimeSpan.FromSeconds(3.5));
				_castEradication = true;

				if (summon.Entry == CreatureIds.Decimator)
					DoCastSelf(SpellIds.DecimatorCannonEject);
				else
					DoCastSelf(SpellIds.AnnihilatorCannonEject);

				Me.PlayOneShotAnimKitId(MiscConst.AnimKitIdCannonDestroyed);
				HideCannons();

				break;
			default:
				break;
		}
	}

	public override uint GetData(uint type)
	{
		if (type == MiscConst.DataLastFiredCannon)
			return _lastCanonEntry;

		return 0;
	}

	public override void SetData(uint type, uint value)
	{
		if (type == MiscConst.DataLastFiredCannon)
			_lastCanonEntry = value;
	}

	public override void UpdateAI(uint diff)
	{
		if (!UpdateVictim())
			return;

		Events.Update(diff);

		if (Me.HasUnitState(UnitState.Casting) &&
			!Me.HasAura(SpellIds.ApocalypseDrive))
			return;

		Events.ExecuteEvents(eventId =>
		{
			switch (eventId)
			{
				case EventIds.ReengagePlayers:
					DoCastSelf(SpellIds.Empowered);
					DoCastSelf(SpellIds.RestoreHealth);

					if (_castEradication)
					{
						DoCastSelf(SpellIds.Eradication);
						Talk(TextIds.SayAnnounceEradication);
						Talk(TextIds.SayFinishApocalypseDrive);
						_castEradication = false;
					}

					Me.ReactState = ReactStates.Aggressive;
					Events.ScheduleEvent(EventIds.FelBombardment, TimeSpan.FromSeconds(20));
					Events.ScheduleEvent(EventIds.CannonChooser, TimeSpan.FromSeconds(18));

					break;
				case EventIds.FelBombardment:
					DoCastAOE(SpellIds.FelBombardmentSelector);
					Events.Repeat(TimeSpan.FromSeconds(20));

					break;
				case EventIds.SearingBarrage:
					DoCastSelf(_searingBarrageSpellId);

					break;
				case EventIds.CannonChooser:
					DoCastSelf(SpellIds.CannonChooser);
					Events.Repeat(TimeSpan.FromSeconds(16));

					break;
				case EventIds.SurgingFel:
				{
					_surgingFelDummyGuids.Remove(_lastSurgingFelDummyGuid);
					_lastSurgingFelDummyGuid = _surgingFelDummyGuids.SelectRandom();
					var dummy = ObjectAccessor.GetCreature(Me, _lastSurgingFelDummyGuid);

					if (dummy)
						dummy.CastSpell(dummy, SpellIds.SurgingFelAreaTrigger);

					Events.Repeat(TimeSpan.FromSeconds(8));

					break;
				}
				default:
					break;
			}
		});

		if (Me.Victim &&
			Me.Victim.IsWithinMeleeRange(Me))
			DoMeleeAttackIfReady();
		else
			DoSpellAttackIfReady(SpellIds.Carnage);
	}

	private void CleanupEncounter()
	{
		var decimator = Instance.GetCreature(DataTypes.Decimator);

		if (decimator)
			Instance.SendEncounterUnit(EncounterFrameType.Disengage, decimator);

		var annihilator = Instance.GetCreature(DataTypes.Annihilator);

		if (annihilator)
			Instance.SendEncounterUnit(EncounterFrameType.Disengage, annihilator);

		Instance.DoRemoveAurasDueToSpellOnPlayers(SpellIds.DecimationWarning);
		Instance.DoRemoveAurasDueToSpellOnPlayers(SpellIds.FelBombardmentWarning);
		Instance.DoRemoveAurasDueToSpellOnPlayers(SpellIds.FelBombardmentPeriodic);
		Summons.DespawnAll();
	}

	private void HideCannons()
	{
		var decimator = Instance.GetCreature(DataTypes.Decimator);

		if (decimator)
		{
			Instance.SendEncounterUnit(EncounterFrameType.Disengage, decimator);
			decimator.SetUnitFlag(UnitFlags.Uninteractible | UnitFlags.Immune);
		}

		var annihilator = Instance.GetCreature(DataTypes.Annihilator);

		if (annihilator)
		{
			Instance.SendEncounterUnit(EncounterFrameType.Disengage, annihilator);
			annihilator.SetUnitFlag(UnitFlags.Uninteractible | UnitFlags.Immune);
		}
	}
}

[Script]
internal class at_garothi_annihilation : AreaTriggerScript, IAreaTriggerOnCreate, IAreaTriggerOnUnitEnter, IAreaTriggerOnUnitExit
{
	private byte _playerCount;

	public void OnCreate()
	{
		Initialize();
	}

	public void OnUnitEnter(Unit unit)
	{
		if (!unit.IsPlayer)
			return;

		_playerCount++;

		var annihilation = At.GetCaster();

		if (annihilation)
			annihilation.RemoveAura(SpellIds.AnnihilationWarning);
	}

	public void OnUnitExit(Unit unit)
	{
		if (!unit.IsPlayer)
			return;

		_playerCount--;

		if (_playerCount == 0 &&
			!At.IsRemoved)
		{
			var annihilation = At.GetCaster();

			annihilation?.CastSpell(annihilation, SpellIds.AnnihilationWarning);
		}
	}

	private void Initialize()
	{
		_playerCount = 0;
	}
}

[Script]
internal class spell_garothi_apocalypse_drive : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(HandlePeriodic, 1, AuraType.PeriodicDummy));
	}

	private void HandlePeriodic(AuraEffect aurEff)
	{
		Target.CastSpell(Target, SpellIds.ApocalypseDrivePeriodicDamage, new CastSpellExtraArgs(aurEff));
	}
}

[Script]
internal class spell_garothi_fel_bombardment_selector : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 0, Targets.UnitSrcAreaEnemy, SpellScriptHookType.ObjectAreaTargetSelect));
		SpellEffects.Add(new EffectHandler(HandleWarningEffect, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void FilterTargets(List<WorldObject> targets)
	{
		if (targets.Empty())
			return;

		var caster = Caster;

		if (caster)
			targets.RemoveAll(new VictimCheck(caster, true));
	}

	private void HandleWarningEffect(int effIndex)
	{
		var caster = Caster ? Caster.AsCreature : null;

		if (!caster ||
			!caster.IsAIEnabled)
			return;

		var target = HitUnit;
		caster.AI.Talk(TextIds.SayAnnounceFelBombardment, target);
		caster.CastSpell(target, SpellIds.FelBombardmentWarning, true);
		caster.CastSpell(target, SpellIds.FelBombardmentDummy, true);
	}
}

[Script]
internal class spell_garothi_fel_bombardment_warning : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(AfterRemove, 0, AuraType.Dummy, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterRemove));
	}

	private void AfterRemove(AuraEffect aurEff, AuraEffectHandleModes mode)
	{
		if (TargetApplication.RemoveMode == AuraRemoveMode.Expire)
		{
			var caster = Caster;

			if (caster)
				caster.CastSpell(Target, SpellIds.FelBombardmentPeriodic, true);
		}
	}
}

[Script]
internal class spell_garothi_fel_bombardment_periodic : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectPeriodicHandler(HandlePeriodic, 0, AuraType.PeriodicTriggerSpell));
	}

	private void HandlePeriodic(AuraEffect aurEff)
	{
		var caster = Caster;

		if (caster)
			caster.CastSpell(Target, (uint)aurEff.GetSpellEffectInfo().CalcValue(caster), true);
	}
}

[Script]
internal class spell_garothi_searing_barrage_dummy : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleHit, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleHit(int effIndex)
	{
		HitUnit.CastSpell(HitUnit, SpellIds.SearingBarrageSelector, new CastSpellExtraArgs(TriggerCastFlags.FullMask).AddSpellMod(SpellValueMod.BasePoint0, (int)SpellInfo.Id));
	}
}

[Script]
internal class spell_garothi_searing_barrage_selector : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 0, Targets.UnitSrcAreaEntry, SpellScriptHookType.ObjectAreaTargetSelect));
		SpellEffects.Add(new EffectHandler(HandleHit, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void FilterTargets(List<WorldObject> targets)
	{
		MiscConst.PreferNonTankTargetsAndResizeTargets(targets, Caster);
	}

	private void HandleHit(int effIndex)
	{
		var spellId = EffectValue == SpellIds.SearingBarrageDummyAnnihilator ? SpellIds.SearingBarrageDamageAnnihilator : SpellIds.SearingBarrageDamageDecimator;
		var caster = Caster;

		if (caster)
			caster.CastSpell(HitUnit, spellId, true);
	}
}

[Script]
internal class spell_garothi_decimation_selector : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new ObjectAreaTargetSelectHandler(FilterTargets, 0, Targets.UnitSrcAreaEnemy));
		SpellEffects.Add(new EffectHandler(HandleHit, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void FilterTargets(List<WorldObject> targets)
	{
		MiscConst.PreferNonTankTargetsAndResizeTargets(targets, Caster);
	}

	private void HandleHit(int effIndex)
	{
		var caster = Caster;

		if (caster)
		{
			caster.CastSpell(HitUnit, SpellIds.DecimationWarning, true);
			var decimator = caster.AsCreature;

			if (decimator)
				if (decimator.IsAIEnabled)
					decimator.AI.Talk(TextIds.SayAnnounceDecimation, HitUnit);
		}
	}
}

[Script]
internal class spell_garothi_decimation_warning : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(AfterRemove, 0, AuraType.Dummy, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterRemove));
	}

	private void AfterRemove(AuraEffect aurEff, AuraEffectHandleModes mode)
	{
		if (TargetApplication.RemoveMode == AuraRemoveMode.Expire)
		{
			var caster = Caster;

			if (caster)
			{
				caster.CastSpell(Target, SpellIds.DecimationMissile, true);

				if (!caster.HasUnitState(UnitState.Casting))
					caster.CastSpell(caster, SpellIds.DecimationCastVisual);
			}
		}
	}
}

[Script]
internal class spell_garothi_carnage : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();

	public override void Register()
	{
		AuraEffects.Add(new AuraEffectProcHandler(HandleProc, 0, AuraType.PeriodicTriggerSpell, AuraScriptHookType.EffectProc));
	}

	private void HandleProc(AuraEffect aurEff, ProcEventInfo eventInfo)
	{
		// Usually we could just handle this via spell_proc but since we want
		// to silence the console message because it's not a spell trigger proc, we need a script here.
		PreventDefaultAction();
		Remove();
	}
}

[Script]
internal class spell_garothi_annihilation_selector : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleHit, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleHit(int effIndex)
	{
		var caster = Caster;

		if (caster)
			caster.CastSpell(HitUnit, (uint)EffectInfo.CalcValue(caster), true);
	}
}

[Script]
internal class spell_garothi_annihilation_triggered : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();


	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleHit, 1, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleHit(int effIndex)
	{
		var target = HitUnit;

		if (target.HasAura(SpellIds.AnnihilationWarning))
			target.CastSpell(target, SpellIds.AnnihilationDamageUnsplitted, true);

		target.RemoveAllAuras();
	}
}

[Script]
internal class spell_garothi_eradication : SpellScript, ISpellOnHit
{
	public void OnHit()
	{
		var caster = Caster;

		if (caster)
		{
			var damageReduction = (uint)MathFunctions.CalculatePct(HitDamage, HitUnit.GetDistance(caster));
			HitDamage = (int)(HitDamage - damageReduction);
		}
	}
}

[Script]
internal class spell_garothi_surging_fel : AuraScript, IHasAuraEffects
{
	public List<IAuraEffectHandler> AuraEffects { get; } = new();


	public override void Register()
	{
		AuraEffects.Add(new AuraEffectApplyHandler(AfterRemove, 0, AuraType.AreaTrigger, AuraEffectHandleModes.Real, AuraScriptHookType.EffectAfterRemove));
	}

	private void AfterRemove(AuraEffect aurEff, AuraEffectHandleModes mode)
	{
		if (TargetApplication.RemoveMode == AuraRemoveMode.Expire)
			Target.CastSpell(Target, SpellIds.SurgingFelDamage, true);
	}
}

[Script]
internal class spell_garothi_cannon_chooser : SpellScript, IHasSpellEffects
{
	public List<ISpellEffect> SpellEffects { get; } = new();

	public override void Register()
	{
		SpellEffects.Add(new EffectHandler(HandleDummyEffect, 0, SpellEffectName.Dummy, SpellScriptHookType.EffectHitTarget));
	}

	private void HandleDummyEffect(int effIndex)
	{
		var caster = HitCreature;

		if (!caster ||
			!caster.IsAIEnabled)
			return;

		var instance = caster.InstanceScript;

		if (instance == null)
			return;

		var decimator = instance.GetCreature(DataTypes.Decimator);
		var annihilator = instance.GetCreature(DataTypes.Annihilator);
		var lastCannonEntry = caster.AI.GetData(MiscConst.DataLastFiredCannon);

		if ((lastCannonEntry == CreatureIds.Annihilator && decimator) ||
			(decimator && !annihilator))
		{
			decimator.CastSpell(decimator, SpellIds.DecimationSelector, true);
			caster.AI.Talk(TextIds.SayDecimation, decimator);
			lastCannonEntry = CreatureIds.Decimator;
		}
		else if ((lastCannonEntry == CreatureIds.Decimator && annihilator) ||
				(annihilator && !decimator))
		{
			var count = (byte)(caster.Map.DifficultyID == Difficulty.MythicRaid ? MiscConst.MaxTargetsSize : Math.Max(MiscConst.MinTargetsSize, Math.Ceiling((double)caster.Map.GetPlayersCountExceptGMs() / 5)));

			for (byte i = 0; i < count; i++)
			{
				var x = MiscConst.AnnihilationCenterReferencePos.X + MathF.Cos(RandomHelper.FRand(0.0f, MathF.PI * 2)) * RandomHelper.FRand(15.0f, 30.0f);
				var y = MiscConst.AnnihilationCenterReferencePos.Y + MathF.Sin(RandomHelper.FRand(0.0f, MathF.PI * 2)) * RandomHelper.FRand(15.0f, 30.0f);
				var z = caster.Map.GetHeight(caster.PhaseShift, x, y, MiscConst.AnnihilationCenterReferencePos.Z);
				annihilator.CastSpell(new Position(x, y, z), SpellIds.AnnihilationSummon, new CastSpellExtraArgs(true));
			}

			annihilator.CastSpell(annihilator, SpellIds.AnnihilationDummy);
			annihilator.CastSpell(annihilator, SpellIds.AnnihilationSelector);
			caster.AI.Talk(TextIds.SayAnnihilation);
			lastCannonEntry = CreatureIds.Annihilator;
		}

		caster.AI.SetData(MiscConst.DataLastFiredCannon, lastCannonEntry);
	}
}

internal class VictimCheck : ICheck<WorldObject>
{
	private readonly Unit _caster;
	private readonly bool _keepTank; // true = remove all nontank targets | false = remove current tank

	public VictimCheck(Unit caster, bool keepTank)
	{
		_caster = caster;
		_keepTank = keepTank;
	}

	public bool Invoke(WorldObject obj)
	{
		var unit = obj.AsUnit;

		if (!unit)
			return true;

		if (_caster.Victim &&
			_caster.Victim != unit)
			return _keepTank;

		return false;
	}
}