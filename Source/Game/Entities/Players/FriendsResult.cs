﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

namespace Game.Entities;

public enum FriendsResult
{
	DbError = 0x00,
	ListFull = 0x01,
	Online = 0x02,
	Offline = 0x03,
	NotFound = 0x04,
	Removed = 0x05,
	AddedOnline = 0x06,
	AddedOffline = 0x07,
	Already = 0x08,
	Self = 0x09,
	Enemy = 0x0a,
	IgnoreFull = 0x0b,
	IgnoreSelf = 0x0c,
	IgnoreNotFound = 0x0d,
	IgnoreAlready = 0x0e,
	IgnoreAdded = 0x0f,
	IgnoreRemoved = 0x10,
	IgnoreAmbiguous = 0x11, // That Name Is Ambiguous, Type More Of The Player'S Server Name
	MuteFull = 0x12,
	MuteSelf = 0x13,
	MuteNotFound = 0x14,
	MuteAlready = 0x15,
	MuteAdded = 0x16,
	MuteRemoved = 0x17,
	MuteAmbiguous = 0x18, // That Name Is Ambiguous, Type More Of The Player'S Server Name
	Unk1 = 0x19,          // no message at client
	Unk2 = 0x1A,
	Unk3 = 0x1B,
	Unknown = 0x1C // Unknown friend response from server
}