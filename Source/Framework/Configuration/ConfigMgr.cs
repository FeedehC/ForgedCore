﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Framework.Collections;

namespace Framework.Configuration;

public class ConfigMgr
{
	static readonly Dictionary<string, string> _configList = new();
	static readonly Dictionary<string, object> _convertedVals = new();

	public static bool Load(string fileName)
	{
		var path = AppContext.BaseDirectory + fileName;

		if (!File.Exists(path))
		{
			Console.WriteLine("{0} doesn't exist!", fileName);

			return false;
		}

		var ConfigContent = File.ReadAllLines(path, Encoding.UTF8);

		var lineCounter = 0;

		try
		{
			foreach (var line in ConfigContent)
			{
				lineCounter++;

				if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("-"))
					continue;

				var configOption = new StringArray(line, '=');
				_configList.Add(configOption[0].Trim(), configOption[1].Replace("\"", "").Trim());
			}
		}
		catch
		{
			Console.WriteLine("Error in {0} on Line {1}", fileName, lineCounter);

			return false;
		}

		return true;
	}

	public static bool TryGetIfNotDefaultValue<T>(string name, T defaultValue, out T value)
	{
		value = GetDefaultValue(name, defaultValue);

		return value.GetHashCode() != defaultValue.GetHashCode();
	}

	public static T GetDefaultValue<T>(string name, T defaultValue)
	{
		if (_convertedVals.TryGetValue(name, out var val))
			return (T)val;

		var temp = _configList.LookupByKey(name);

		var type = typeof(T).IsEnum ? typeof(T).GetEnumUnderlyingType() : typeof(T);

		if (temp.IsEmpty())
		{
			val = Convert.ChangeType(defaultValue, type);
			_convertedVals[name] = val;

			return (T)val;
		}

		if (Type.GetTypeCode(typeof(T)) == TypeCode.Boolean && temp.IsNumber())
		{
			val = Convert.ChangeType(temp == "1", typeof(T));
			_convertedVals[name] = val;

			return (T)val;
		}

		val = Convert.ChangeType(temp, type);
		_convertedVals[name] = val;

		return (T)val;
	}


	public static IEnumerable<string> GetKeysByString(string name)
	{
		return _configList.Where(p => p.Key.Contains(name)).Select(p => p.Key);
	}
}