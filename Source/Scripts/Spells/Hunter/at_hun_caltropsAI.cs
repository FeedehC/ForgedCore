﻿using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Hunter;

[Script]
public class at_hun_caltropsAI : AreaTriggerAI
{
	public int timeInterval;

	public enum UsedSpells
	{
		SPELL_HUNTER_CALTROPS_AURA = 194279
	}

	public at_hun_caltropsAI(AreaTrigger areatrigger) : base(areatrigger)
	{
		// How often should the action be executed
		timeInterval = 1000;
	}

	public override void OnUpdate(uint p_Time)
	{
		Unit caster = at.GetCaster();
		if (caster == null)
		{
			return;
		}

		if (caster.GetTypeId() != TypeId.Player)
		{
			return;
		}

		// Check if we can handle actions
		timeInterval += (int)p_Time;
		if (timeInterval < 1000)
		{
			return;
		}

		foreach (var guid in at.GetInsideUnits())
		{
			Unit unit = ObjectAccessor.Instance.GetUnit(caster, guid);
			if (unit != null)
			{
				if (!caster.IsFriendlyTo(unit))
				{
					caster.CastSpell(unit, UsedSpells.SPELL_HUNTER_CALTROPS_AURA, true);
				}
			}
		}

		timeInterval -= 1000;
	}
}