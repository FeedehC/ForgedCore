﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Constants;
using Game.Entities;
using Game.Scripting.Interfaces;
using Game.Spells;

namespace Game.Scripting;

// helper class from which SpellScript and SpellAura derive, use these classes instead
public class BaseSpellScript : IBaseSpellScript
{
	public byte CurrentScriptState { get; set; }
	public string ScriptName { get; set; }

	public uint ScriptSpellId { get; set; }

	// internal use classes & functions
	// DO NOT OVERRIDE THESE IN SCRIPTS
	public BaseSpellScript()
	{
		CurrentScriptState = (byte)SpellScriptState.None;
	}

	public virtual bool _Validate(SpellInfo entry)
	{
		if (!ValidateSpellInfo(entry.Id))
		{
			Log.outError(LogFilter.Scripts, "Spell `{0}` did not pass Validate() function of script `{1}` - script will be not added to the spell", entry.Id, ScriptName);

			return false;
		}

		return true;
	}

	public bool ValidateSpellInfo(params uint[] spellIds)
	{
		var allValid = true;

		foreach (var spellId in spellIds)
			if (!Global.SpellMgr.HasSpellInfo(spellId, Difficulty.None))
			{
				Log.outError(LogFilter.Scripts, "BaseSpellScript::ValidateSpellInfo: Spell {0} does not exist.", spellId);
				allValid = false;
			}

		return allValid;
	}

	public void _Register()
	{
		CurrentScriptState = (byte)SpellScriptState.Registration;
		Register();
		CurrentScriptState = (byte)SpellScriptState.None;
	}

	public void _Unload()
	{
		CurrentScriptState = (byte)SpellScriptState.Unloading;
		Unload();
		CurrentScriptState = (byte)SpellScriptState.None;
	}

	public void _Init(string scriptname, uint spellId)
	{
		CurrentScriptState = (byte)SpellScriptState.None;
		ScriptName = scriptname;
		ScriptSpellId = spellId;
	}

	public string _GetScriptName()
	{
		return ScriptName;
	}

	//
	// SpellScript/AuraScript interface base
	// these functions are safe to override, see notes below for usage instructions
	//
	// Function in which handler functions are registered, must be implemented in script
	public virtual void Register() { }

	// Function called when script is created, if returns false script will be unloaded afterwards
	// use for: initializing local script variables (DO NOT USE CONSTRUCTOR FOR THIS PURPOSE!)
	public virtual bool Load()
	{
		return true;
	}

	// Function called when script is destroyed
	// use for: deallocating memory allocated by script
	public virtual void Unload() { }
}

public class SpellScript : BaseSpellScript, ISpellScript
{
	private Spell _spell;
	private uint _hitPreventEffectMask;
	private uint _hitPreventDefaultEffectMask;

	public bool IsInCheckCastHook => CurrentScriptState == (byte)SpellScriptHookType.CheckCast;

	public bool IsInTargetHook
	{
		get
		{
			switch ((SpellScriptHookType)CurrentScriptState)
			{
				case SpellScriptHookType.LaunchTarget:
				case SpellScriptHookType.EffectHitTarget:
				case SpellScriptHookType.EffectSuccessfulDispel:
				case SpellScriptHookType.BeforeHit:
				case SpellScriptHookType.Hit:
				case SpellScriptHookType.AfterHit:
					return true;
			}

			return false;
		}
	}

	public bool IsInHitPhase => CurrentScriptState >= (byte)SpellScriptHookType.EffectHit && CurrentScriptState < (byte)SpellScriptHookType.AfterHit + 1;

	public bool IsInEffectHook => CurrentScriptState >= (byte)SpellScriptHookType.Launch && CurrentScriptState <= (byte)SpellScriptHookType.EffectHitTarget || CurrentScriptState == (byte)SpellScriptHookType.EffectSuccessfulDispel;

	// hooks are executed in following order, at specified event of spell:
	// 1. BeforeCast - executed when spell preparation is finished (when cast bar becomes full) before cast is handled
	// 2. OnCheckCast - allows to override result of CheckCast function
	// 3a. OnObjectAreaTargetSelect - executed just before adding selected targets to final Target list (for area targets)
	// 3b. OnObjectTargetSelect - executed just before adding selected Target to final Target list (for single unit targets)
	// 4. OnCast - executed just before spell is launched (creates missile) or executed
	// 5. AfterCast - executed after spell missile is launched and immediate spell actions are done
	// 6. OnEffectLaunch - executed just before specified effect handler call - when spell missile is launched
	// 7. OnEffectLaunchTarget - executed just before specified effect handler call - when spell missile is launched - called for each Target from spell Target map
	// 8. OnEffectHit - executed just before specified effect handler call - when spell missile hits dest
	// 9. BeforeHit - executed just before spell hits a Target - called for each Target from spell Target map
	// 10. OnEffectHitTarget - executed just before specified effect handler call - called for each Target from spell Target map
	// 11. OnHit - executed just before spell deals Damage and procs Auras - when spell hits Target - called for each Target from spell Target map
	// 12. AfterHit - executed just after spell finishes all it's jobs for Target - called for each Target from spell Target map

	//
	// methods allowing interaction with Spell object
	//
	// methods useable during all spell handling phases
	public Unit Caster => _spell.Caster.AsUnit;
	public GameObject GObjCaster => _spell.Caster.AsGameObject;
	public Unit OriginalCaster => _spell.OriginalCaster;
	public SpellInfo SpellInfo => _spell.SpellInfo;
	public Difficulty CastDifficulty => _spell.CastDifficulty;
	public SpellValue SpellValue => _spell.SpellValue;

	// methods useable after spell is prepared
	// accessors to the explicit targets of the spell
	// explicit Target - Target selected by caster (player, game client, or script - DoCast(explicitTarget, ...), required for spell to be cast
	// examples:
	// -shadowstep - explicit Target is the unit you want to go behind of
	// -chain heal - explicit Target is the unit to be healed first
	// -holy nova/arcane explosion - explicit Target = null because Target you are selecting doesn't affect how spell targets are selected
	// you can determine if spell requires explicit targets by dbc columns:
	// - Targets - mask of explicit Target types
	// - ImplicitTargetXX set to TARGET_XXX_TARGET_YYY, _TARGET_ here means that explicit Target is used by the effect, so spell needs one too

	// returns: WorldLocation which was selected as a spell destination or null
	public WorldLocation ExplTargetDest
	{
		get
		{
			if (_spell.Targets.HasDst)
				return _spell.Targets.DstPos;

			return null;
		}

		set => _spell.Targets.SetDst(value);
	}

	// returns: WorldObject which was selected as an explicit spell Target or null if there's no Target
	public WorldObject ExplTargetWorldObject => _spell.Targets.ObjectTarget;

	// returns: Unit which was selected as an explicit spell Target or null if there's no Target
	public Unit ExplTargetUnit => _spell.Targets.UnitTarget;

	// returns: GameObject which was selected as an explicit spell Target or null if there's no Target
	public GameObject ExplTargetGObj => _spell.Targets.GOTarget;

	// returns: Item which was selected as an explicit spell Target or null if there's no Target
	public Item ExplTargetItem => _spell.Targets.ItemTarget;

	/// <summary>
	///  useable only during spell hit on Target, or during spell launch on Target
	/// </summary>
	/// <returns> Target of current effect if it was Unit otherwise null </returns>
	public Unit HitUnit
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitUnit was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return null;
			}

			return _spell.UnitTarget;
		}
	}

	/// <summary>
	/// </summary>
	/// <returns> Target of current effect if it was Creature otherwise null </returns>
	public Creature HitCreature
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitCreature was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return null;
			}

			if (_spell.UnitTarget != null)
				return _spell.UnitTarget.AsCreature;
			else
				return null;
		}
	}

	/// <summary>
	/// </summary>
	/// <returns> Target of current effect if it was Player otherwise null </returns>
	public Player HitPlayer
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitPlayer was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return null;
			}

			if (_spell.UnitTarget != null)
				return _spell.UnitTarget.AsPlayer;
			else
				return null;
		}
	}

	/// <summary>
	/// </summary>
	/// <returns> Target of current effect if it was Item otherwise null </returns>
	public Item HitItem
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitItem was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return null;
			}

			return _spell.ItemTarget;
		}
	}

	/// <summary>
	/// </summary>
	/// <returns> Target of current effect if it was GameObject otherwise null </returns>
	public GameObject HitGObj
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitGObj was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return null;
			}

			return _spell.GameObjTarget;
		}
	}

	/// <summary>
	/// </summary>
	/// <returns> Target of current effect if it was Corpse otherwise nullptr </returns>
	public Corpse HitCorpse
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript::GetHitCorpse was called, but function has no effect in current hook!");

				return null;
			}

			return _spell.CorpseTarget;
		}
	}

	/// <summary>
	/// </summary>
	/// <returns> destination of current effect </returns>
	public WorldLocation HitDest
	{
		get
		{
			if (!IsInEffectHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitDest was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return null;
			}

			return _spell.DestTarget;
		}
	}

	// setter/getter for for Damage done by spell to Target of spell hit
	// returns Damage calculated before hit, and real dmg done after hit
	public double HitDamage
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitDamage was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return 0;
			}

			return _spell.DamageInEffects;
		}

		set
		{
			if (!IsInModifiableHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.SetHitDamage was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return;
			}

			_spell.DamageInEffects = value;
		}
	}

	// setter/getter for for heal done by spell to Target of spell hit
	// returns healing calculated before hit, and real dmg done after hit
	public double HitHeal
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitHeal was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return 0;
			}

			return _spell.HealingInEffects;
		}

		set
		{
			if (!IsInModifiableHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.SetHitHeal was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return;
			}

			_spell.HealingInEffects = value;
		}
	}

	public Spell Spell => _spell;

	/// <summary>
	/// </summary>
	/// <returns> true if spell critically hits current HitUnit </returns>
	public bool IsHitCrit
	{
		get
		{
			if (!IsInTargetHook)
			{
				Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript::IsHitCrit was called, but function has no effect in current hook!");

				return false;
			}

			var hitUnit = HitUnit;

			if (hitUnit != null)
			{
				var targetInfo = _spell.UniqueTargetInfoOrgi.Find(targetInfo => targetInfo.TargetGuid == hitUnit.GUID);

				return targetInfo.IsCrit;
			}

			return false;
		}
	}

	public SpellEffectInfo EffectInfo
	{
		get
		{
			return _spell.EffectInfo;
		}
	}

	// method avalible only in EffectHandler method
	public double EffectValue
	{
		get
		{
			if (!IsInEffectHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.PreventHitDefaultEffect was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return 0;
			}

			return _spell.Damage;
		}

		set
		{
			if (!IsInEffectHook)
			{
				Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.SetEffectValue was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

				return;
			}

			_spell.Damage = value;
		}
	}

	public double EffectVariance
	{
		get
		{
			if (!IsInEffectHook)
			{
				Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript::GetEffectVariance was called, but function has no effect in current hook!");

				return 0.0f;
			}

			return _spell.Variance;
		}

		set
		{
			if (!IsInEffectHook)
			{
				Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript::SetEffectVariance was called, but function has no effect in current hook!");

				return;
			}

			_spell.Variance = value;
		}
	}

	// returns: cast Item if present.
	public Item CastItem => _spell.CastItem;

	// Returns SpellInfo from the spell that triggered the current one
	public SpellInfo TriggeringSpell => _spell.TriggeredByAuraSpell;

	public Position TargetPosition
	{
		get
		{
			Position pos = ExplTargetWorldObject.Location;

			if (pos == null || pos.IsDefault || !pos.IsPositionValid)
				pos = _spell.Targets.Dst.Position;

			return pos;
		}
	}

	private bool IsAfterTargetSelectionPhase => IsInHitPhase || IsInEffectHook || CurrentScriptState == (byte)SpellScriptHookType.OnCast || CurrentScriptState == (byte)SpellScriptHookType.AfterCast || CurrentScriptState == (byte)SpellScriptHookType.CalcCritChance;

	private bool IsInModifiableHook
	{
		get
		{
			// after hit hook executed after Damage/healing is already done
			// modifying it at this point has no effect
			switch ((SpellScriptHookType)CurrentScriptState)
			{
				case SpellScriptHookType.LaunchTarget:
				case SpellScriptHookType.EffectHitTarget:
				case SpellScriptHookType.BeforeHit:
				case SpellScriptHookType.Hit:
					return true;
			}

			return false;
		}
	}

	public bool _Load(Spell spell)
	{
		_spell = spell;
		_PrepareScriptCall((SpellScriptHookType)SpellScriptState.Loading);
		var load = Load();
		_FinishScriptCall();

		return load;
	}

	public void _InitHit()
	{
		_hitPreventEffectMask = 0;
		_hitPreventDefaultEffectMask = 0;
	}

	public bool _IsEffectPrevented(int effIndex)
	{
		return Convert.ToBoolean(_hitPreventEffectMask & 1 << effIndex);
	}

	public bool _IsDefaultEffectPrevented(int effIndex)
	{
		return Convert.ToBoolean(_hitPreventDefaultEffectMask & 1 << effIndex);
	}

	public void _PrepareScriptCall(SpellScriptHookType hookType)
	{
		CurrentScriptState = (byte)hookType;
	}

	public void _FinishScriptCall()
	{
		CurrentScriptState = (byte)SpellScriptState.None;
	}

	public SpellEffectInfo GetEffectInfo(int effIndex)
	{
		return SpellInfo.GetEffect(effIndex);
	}

	public long GetUnitTargetCountForEffect(int effect)
	{
		if (!IsAfterTargetSelectionPhase)
		{
			Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript.GetUnitTargetCountForEffect was called, but function has no effect in current hook! (spell has not selected targets yet)");

			return 0;
		}

		return _spell.GetUnitTargetCountForEffect(effect);
	}

	public long GetGameObjectTargetCountForEffect(int effect)
	{
		if (!IsAfterTargetSelectionPhase)
		{
			Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript.GetGameObjectTargetCountForEffect was called, but function has no effect in current hook! (spell has not selected targets yet)");

			return 0;
		}

		return _spell.GetGameObjectTargetCountForEffect(effect);
	}

	public long GetItemTargetCountForEffect(int effect)
	{
		if (!IsAfterTargetSelectionPhase)
		{
			Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript.GetItemTargetCountForEffect was called, but function has no effect in current hook! (spell has not selected targets yet)");

			return 0;
		}

		return _spell.GetItemTargetCountForEffect(effect);
	}

	public long GetCorpseTargetCountForEffect(int effect)
	{
		if (!IsAfterTargetSelectionPhase)
		{
			Log.outError(LogFilter.Scripts, $"Script: `{ScriptName}` Spell: `{ScriptSpellId}`: function SpellScript::GetCorpseTargetCountForEffect was called, but function has no effect in current hook! (spell has not selected targets yet)");

			return 0;
		}

		return _spell.GetCorpseTargetCountForEffect(effect);
	}

	public void PreventHitDamage()
	{
		HitDamage = 0;
	}

	// returns current spell hit Target aura
	public Aura GetHitAura(bool dynObjAura = false)
	{
		if (!IsInTargetHook)
		{
			Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.GetHitAura was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

			return null;
		}

		Aura aura = _spell.SpellAura;

		if (dynObjAura)
			aura = _spell.DynObjAura;

		if (aura == null || aura.IsRemoved)
			return null;

		return aura;
	}

	// prevents applying aura on current spell hit Target
	public void PreventHitAura()
	{
		if (!IsInTargetHook)
		{
			Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.PreventHitAura was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

			return;
		}

		var unitAura = _spell.SpellAura;
		unitAura?.Remove();

		var dynAura = _spell.DynObjAura;
		dynAura?.Remove();
	}

	// prevents effect execution on current spell hit Target
	// including other effect/hit scripts
	// will not work on aura/Damage/heal
	// will not work if effects were already handled
	public void PreventHitEffect(int effIndex)
	{
		if (!IsInHitPhase && !IsInEffectHook)
		{
			Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.PreventHitEffect was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

			return;
		}

		_hitPreventEffectMask |= 1u << effIndex;
		PreventHitDefaultEffect(effIndex);
	}

	// prevents default effect execution on current spell hit Target
	// will not work on aura/Damage/heal effects
	// will not work if effects were already handled
	public void PreventHitDefaultEffect(int effIndex)
	{
		if (!IsInHitPhase && !IsInEffectHook)
		{
			Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.PreventHitDefaultEffect was called, but function has no effect in current hook!", ScriptName, ScriptSpellId);

			return;
		}

		_hitPreventDefaultEffectMask |= 1u << effIndex;
	}

	// Creates Item. Calls Spell.DoCreateItem method.
	public void CreateItem(uint itemId, ItemContext context)
	{
		_spell.DoCreateItem(itemId, context);
	}

	// finishes spellcast prematurely with selected error message
	public void FinishCast(SpellCastResult result, int? param1 = null, int? param2 = null)
	{
		_spell.SendCastResult(result, param1, param2);
		_spell.Finish(result);
	}

	public void SetCustomCastResultMessage(SpellCustomErrors result)
	{
		if (!IsInCheckCastHook)
		{
			Log.outError(LogFilter.Scripts, "Script: `{0}` Spell: `{1}`: function SpellScript.SetCustomCastResultMessage was called while spell not in check cast phase!", ScriptName, ScriptSpellId);

			return;
		}

		_spell.CustomErrors = result;
	}

	public void SelectRandomInjuredTargets(List<WorldObject> targets, uint maxTargets, bool prioritizePlayers)
	{
		if (targets.Count <= maxTargets)
			return;

		//List of all player targets.
		var tempPlayers = targets.Where(p => p.IsPlayer).ToList();

		//List of all injured non player targets.
		var tempInjuredUnits = targets.Where(target => target.IsUnit && !target.AsUnit.IsFullHealth).ToList();

		//List of all none injured non player targets.
		var tempNoneInjuredUnits = targets.Where(target => target.IsUnit && target.AsUnit.IsFullHealth).ToList();

		targets.Clear();

		if (prioritizePlayers)
			if (tempPlayers.Count < maxTargets)
			{
				// not enough players, add nonplayer targets
				// prioritize injured nonplayers over full health nonplayers
				if (tempPlayers.Count + tempInjuredUnits.Count < maxTargets)
				{
					// not enough players + injured nonplayers
					// fill remainder with random full health nonplayers
					targets.AddRange(tempPlayers);
					targets.AddRange(tempInjuredUnits);
					targets.AddRange(tempNoneInjuredUnits.Shuffle());
				}
				else if (tempPlayers.Count + tempInjuredUnits.Count > maxTargets)
				{
					// randomize injured nonplayers order
					// final list will contain all players + random injured nonplayers
					targets.AddRange(tempPlayers);
					targets.AddRange(tempInjuredUnits.Shuffle());
				}

				targets.Resize(maxTargets);

				return;
			}

		var lookupPlayers = tempPlayers.ToLookup(target => !target.AsUnit.IsFullHealth);

		if (lookupPlayers[true].Count() < maxTargets)
		{
			// not enough injured units
			// fill remainder with full health units
			targets.AddRange(lookupPlayers[true]);
			targets.AddRange(lookupPlayers[false].Shuffle());
		}
		else if (lookupPlayers[true].Count() > maxTargets)
		{
			// select random injured units
			targets.AddRange(lookupPlayers[true].Shuffle());
		}

		targets.Resize(maxTargets);
	}

	public bool TryGetCaster(out Unit result)
	{
		result = _spell.Caster?.AsUnit;

		return result != null;
	}

	public bool TryGetCaster(out Player result)
	{
		result = _spell.Caster?.AsPlayer;

		return result != null;
	}

	public bool TryGetExplTargetUnit(out Unit target)
	{
		target = _spell.Targets.UnitTarget;

		return target != null;
	}

	public void PreventHitHeal()
	{
		HitHeal = 0;
	}
}