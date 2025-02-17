﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Framework.Constants;

namespace Game.DataStorage;

public sealed class HolidaysRecord
{
	public uint Id;
	public ushort Region;
	public byte Looping;
	public uint HolidayNameID;
	public uint HolidayDescriptionID;
	public byte Priority;
	public sbyte CalendarFilterType;
	public byte Flags;
	public ushort[] Duration = new ushort[SharedConst.MaxHolidayDurations];
	public uint[] Date = new uint[SharedConst.MaxHolidayDates]; // dates in unix time starting at January, 1, 2000
	public byte[] CalendarFlags = new byte[SharedConst.MaxHolidayFlags];
	public int[] TextureFileDataID = new int[3];
}