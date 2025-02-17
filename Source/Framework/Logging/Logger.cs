﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Logger
{
	readonly ConcurrentQueue<LogMessage> _logQueue = new();
	readonly AutoResetEvent _logTrigger = new(false);

	readonly string name;
	readonly Dictionary<byte, Appender> appenders = new();
	LogLevel level;

	public Logger(string _name, LogLevel _level)
	{
		name = _name;
		level = _level;

		Task.Run(() =>
		{
			while (true)
			{
				_logTrigger.WaitOne(500);

				while (_logQueue.Count > 0)
					if (_logQueue.TryDequeue(out var logMessage) && logMessage != null)
						foreach (var appender in appenders.Values)
							appender.Write(logMessage);
			}
		});
	}

	public void addAppender(byte id, Appender appender)
	{
		appenders[id] = appender;
	}

	public void delAppender(byte id)
	{
		appenders.Remove(id);
	}

	public void setLogLevel(LogLevel _level)
	{
		level = _level;
	}

	public string getName()
	{
		return name;
	}

	public LogLevel getLogLevel()
	{
		return level;
	}

	public void write(LogMessage message)
	{
		if (level == 0 || level > message.level || string.IsNullOrEmpty(message.text))
			return;

		_logQueue.Enqueue(message);
		_logTrigger.Set();
	}
}