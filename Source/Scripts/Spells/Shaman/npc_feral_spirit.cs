﻿using Framework.Constants;
using Game.AI;
using Game.Entities;
using Game.Scripting;

namespace Scripts.Spells.Shaman
{
	// 29264
	[CreatureScript(29264)]
	public class npc_feral_spirit : ScriptedAI
	{
		public npc_feral_spirit(Creature p_Creature) : base(p_Creature)
		{
		}

		public override void DamageDealt(Unit UnnamedParameter, ref uint UnnamedParameter2, DamageEffectType UnnamedParameter3)
		{
			var tempSum = me.ToTempSummon();

			if (tempSum != null)
			{
				var owner = tempSum.GetOwner();

				if (owner != null)
					if (owner.HasAura(ShamanSpells.SPELL_SHAMAN_FERAL_SPIRIT_ENERGIZE_DUMMY))
						if (owner.GetPower(PowerType.Maelstrom) <= 95)
							owner.ModifyPower(PowerType.Maelstrom, +5);
			}
		}
	}
}