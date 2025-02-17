﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using Game.Entities;
using Game.Networking.Packets;
using Game.Scripting.Interfaces.IWeather;

namespace Game;

public class Weather
{
	readonly uint _zone;
	readonly IntervalTimer _timer = new();
	readonly WeatherData _weatherChances;
	WeatherType _type;
	float _intensity;

	public uint Zone => _zone;

	public uint ScriptId => _weatherChances.ScriptId;

	public Weather(uint zoneId, WeatherData weatherChances)
	{
		_zone = zoneId;
		_weatherChances = weatherChances;
		_timer.Interval = 10 * Time.Minute * Time.InMilliseconds;
		_type = WeatherType.Fine;
		_intensity = 0;

		//Log.outInfo(LogFilter.General, "WORLD: Starting weather system for zone {0} (change every {1} minutes).", m_zone, (m_timer.GetInterval() / (Time.Minute * Time.InMilliseconds)));
	}

	public bool Update(uint diff)
	{
		if (_timer.Current >= 0)
			_timer.Update(diff);
		else
			_timer.Current = 0;

		// If the timer has passed, ReGenerate the weather
		if (_timer.Passed)
		{
			_timer.Reset();

			// update only if Regenerate has changed the weather
			if (ReGenerate())
				// Weather will be removed if not updated (no players in zone anymore)
				if (!UpdateWeather())
					return false;
		}

		Global.ScriptMgr.RunScript<IWeatherOnUpdate>(p => p.OnUpdate(this, diff), ScriptId);

		return true;
	}

	public bool ReGenerate()
	{
		if (_weatherChances == null)
		{
			_type = WeatherType.Fine;
			_intensity = 0.0f;

			return false;
		}

		// Weather statistics:
		// 30% - no change
		// 30% - weather gets better (if not fine) or change weather type
		// 30% - weather worsens (if not fine)
		// 10% - radical change (if not fine)
		var u = RandomHelper.URand(0, 99);

		if (u < 30)
			return false;

		// remember old values
		var old_type = _type;
		var old_intensity = _intensity;

		var gtime = GameTime.GetGameTime();
		var ltime = Time.UnixTimeToDateTime(gtime).ToLocalTime();
		var season = (uint)((ltime.DayOfYear - 78 + 365) / 91) % 4;

		string[] seasonName =
		{
			"spring", "summer", "fall", "winter"
		};

		Log.outTrace(LogFilter.Server, "Generating a change in {0} weather for zone {1}.", seasonName[season], _zone);

		if ((u < 60) && (_intensity < 0.33333334f)) // Get fair
		{
			_type = WeatherType.Fine;
			_intensity = 0.0f;
		}

		if ((u < 60) && (_type != WeatherType.Fine)) // Get better
		{
			_intensity -= 0.33333334f;

			return true;
		}

		if ((u < 90) && (_type != WeatherType.Fine)) // Get worse
		{
			_intensity += 0.33333334f;

			return true;
		}

		if (_type != WeatherType.Fine)
		{
			// Radical change:
			// if light . heavy
			// if medium . change weather type
			// if heavy . 50% light, 50% change weather type

			if (_intensity < 0.33333334f)
			{
				_intensity = 0.9999f; // go nuts

				return true;
			}
			else
			{
				if (_intensity > 0.6666667f)
				{
					// Severe change, but how severe?
					var rnd = RandomHelper.URand(0, 99);

					if (rnd < 50)
					{
						_intensity -= 0.6666667f;

						return true;
					}
				}

				_type = WeatherType.Fine; // clear up
				_intensity = 0;
			}
		}

		// At this point, only weather that isn't doing anything remains but that have weather data
		var chance1 = _weatherChances.Data[season].RainChance;
		var chance2 = chance1 + _weatherChances.Data[season].SnowChance;
		var chance3 = chance2 + _weatherChances.Data[season].StormChance;
		var rn = RandomHelper.URand(1, 100);

		if (rn <= chance1)
			_type = WeatherType.Rain;
		else if (rn <= chance2)
			_type = WeatherType.Snow;
		else if (rn <= chance3)
			_type = WeatherType.Storm;
		else
			_type = WeatherType.Fine;

		// New weather statistics (if not fine):
		// 85% light
		// 7% medium
		// 7% heavy
		// If fine 100% sun (no fog)

		if (_type == WeatherType.Fine)
		{
			_intensity = 0.0f;
		}
		else if (u < 90)
		{
			_intensity = (float)RandomHelper.NextDouble() * 0.3333f;
		}
		else
		{
			// Severe change, but how severe?
			rn = RandomHelper.URand(0, 99);

			if (rn < 50)
				_intensity = (float)RandomHelper.NextDouble() * 0.3333f + 0.3334f;
			else
				_intensity = (float)RandomHelper.NextDouble() * 0.3333f + 0.6667f;
		}

		// return true only in case weather changes
		return _type != old_type || _intensity != old_intensity;
	}

	public void SendWeatherUpdateToPlayer(Player player)
	{
		WeatherPkt weather = new(GetWeatherState(), _intensity);
		player.SendPacket(weather);
	}

	public static void SendFineWeatherUpdateToPlayer(Player player)
	{
		player.SendPacket(new WeatherPkt(WeatherState.Fine));
	}

	public bool UpdateWeather()
	{
		var player = Global.WorldMgr.FindPlayerInZone(_zone);

		if (player == null)
			return false;

		// Send the weather packet to all players in this zone
		if (_intensity >= 1)
			_intensity = 0.9999f;
		else if (_intensity < 0)
			_intensity = 0.0001f;

		var state = GetWeatherState();

		WeatherPkt weather = new(state, _intensity);

		//- Returns false if there were no players found to update
		if (!Global.WorldMgr.SendZoneMessage(_zone, weather))
			return false;

		// Log the event
		string wthstr;

		switch (state)
		{
			case WeatherState.Fog:
				wthstr = "fog";

				break;
			case WeatherState.LightRain:
				wthstr = "light rain";

				break;
			case WeatherState.MediumRain:
				wthstr = "medium rain";

				break;
			case WeatherState.HeavyRain:
				wthstr = "heavy rain";

				break;
			case WeatherState.LightSnow:
				wthstr = "light snow";

				break;
			case WeatherState.MediumSnow:
				wthstr = "medium snow";

				break;
			case WeatherState.HeavySnow:
				wthstr = "heavy snow";

				break;
			case WeatherState.LightSandstorm:
				wthstr = "light sandstorm";

				break;
			case WeatherState.MediumSandstorm:
				wthstr = "medium sandstorm";

				break;
			case WeatherState.HeavySandstorm:
				wthstr = "heavy sandstorm";

				break;
			case WeatherState.Thunders:
				wthstr = "thunders";

				break;
			case WeatherState.BlackRain:
				wthstr = "blackrain";

				break;
			case WeatherState.Fine:
			default:
				wthstr = "fine";

				break;
		}

		Log.outDebug(LogFilter.Server, "Change the weather of zone {0} to {1}.", _zone, wthstr);

		Global.ScriptMgr.RunScript<IWeatherOnChange>(p => p.OnChange(this, state, _intensity), ScriptId);

		return true;
	}

	public void SetWeather(WeatherType type, float grade)
	{
		if (_type == type && _intensity == grade)
			return;

		_type = type;
		_intensity = grade;
		UpdateWeather();
	}

	public WeatherState GetWeatherState()
	{
		if (_intensity < 0.27f)
			return WeatherState.Fine;

		switch (_type)
		{
			case WeatherType.Rain:
				if (_intensity < 0.40f)
					return WeatherState.LightRain;
				else if (_intensity < 0.70f)
					return WeatherState.MediumRain;
				else
					return WeatherState.HeavyRain;
			case WeatherType.Snow:
				if (_intensity < 0.40f)
					return WeatherState.LightSnow;
				else if (_intensity < 0.70f)
					return WeatherState.MediumSnow;
				else
					return WeatherState.HeavySnow;
			case WeatherType.Storm:
				if (_intensity < 0.40f)
					return WeatherState.LightSandstorm;
				else if (_intensity < 0.70f)
					return WeatherState.MediumSandstorm;
				else
					return WeatherState.HeavySandstorm;
			case WeatherType.BlackRain:
				return WeatherState.BlackRain;
			case WeatherType.Thunders:
				return WeatherState.Thunders;
			case WeatherType.Fine:
			default:
				return WeatherState.Fine;
		}
	}
}