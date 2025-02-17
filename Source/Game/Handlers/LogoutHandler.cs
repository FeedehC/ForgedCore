﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;
using Game.Networking;
using Game.Networking.Packets;

namespace Game;

public partial class WorldSession
{
	[WorldPacketHandler(ClientOpcodes.LogoutRequest)]
	void HandleLogoutRequest(LogoutRequest packet)
	{
		var pl = Player;

		if (!Player.GetLootGUID().IsEmpty)
			Player.SendLootReleaseAll();

		var instantLogout = (pl.HasPlayerFlag(PlayerFlags.Resting) && !pl.IsInCombat ||
							pl.IsInFlight ||
							HasPermission(RBACPermissions.InstantLogout));

		var canLogoutInCombat = pl.HasPlayerFlag(PlayerFlags.Resting);

		var reason = 0;

		if (pl.IsInCombat && !canLogoutInCombat)
			reason = 1;
		else if (pl.IsFalling)
			reason = 3;                               // is jumping or falling
		else if (pl.Duel != null || pl.HasAura(9454)) // is dueling or frozen by GM via freeze command
			reason = 2;                               // FIXME - Need the correct value

		LogoutResponse logoutResponse = new();
		logoutResponse.LogoutResult = reason;
		logoutResponse.Instant = instantLogout;
		SendPacket(logoutResponse);

		if (reason != 0)
		{
			SetLogoutStartTime(0);

			return;
		}

		// instant logout in taverns/cities or on taxi or for admins, gm's, mod's if its enabled in worldserver.conf
		if (instantLogout)
		{
			LogoutPlayer(true);

			return;
		}

		// not set flags if player can't free move to prevent lost state at logout cancel
		if (pl.CanFreeMove())
		{
			if (pl.StandState == UnitStandStateType.Stand)
				pl.SetStandState(UnitStandStateType.Sit);

			pl.SetRooted(true);
			pl.SetUnitFlag(UnitFlags.Stunned);
		}

		SetLogoutStartTime(GameTime.GetGameTime());
	}

	[WorldPacketHandler(ClientOpcodes.LogoutCancel)]
	void HandleLogoutCancel(LogoutCancel packet)
	{
		// Player have already logged out serverside, too late to cancel
		if (!Player)
			return;

		SetLogoutStartTime(0);

		SendPacket(new LogoutCancelAck());

		// not remove flags if can't free move - its not set in Logout request code.
		if (Player.CanFreeMove())
		{
			//!we can move again
			Player.SetRooted(false);

			//! Stand Up
			Player.SetStandState(UnitStandStateType.Stand);

			//! DISABLE_ROTATE
			Player.RemoveUnitFlag(UnitFlags.Stunned);
		}
	}
}