﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;

public class GameTime
{
	static readonly long StartTime = Time.UnixTime;

	static long _gameTime = Time.UnixTime;
	static uint _gameMSTime = 0;

	static DateTime _gameTimeSystemPoint = DateTime.MinValue;
	static DateTime _gameTimeSteadyPoint = DateTime.MinValue;

	static DateTime _dateTime;

	public static long GetStartTime()
	{
		return StartTime;
	}

	public static long GetGameTime()
	{
		return _gameTime;
	}

	public static uint GetGameTimeMS()
	{
		return _gameMSTime;
	}

	public static DateTime GetSystemTime()
	{
		return _gameTimeSystemPoint;
	}

	public static DateTime Now()
	{
		return _gameTimeSteadyPoint;
	}

	public static uint GetUptime()
	{
		return (uint)(_gameTime - StartTime);
	}

	public static DateTime GetDateAndTime()
	{
		return _dateTime;
	}

	public static void UpdateGameTimers()
	{
		_gameTime = Time.UnixTime;
		_gameMSTime = Time.MSTime;
		_gameTimeSystemPoint = DateTime.Now;
		_gameTimeSteadyPoint = DateTime.Now;

		_dateTime = Time.UnixTimeToDateTime(_gameTime);
	}
}