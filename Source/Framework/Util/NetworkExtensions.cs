﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

using System;
using System.Net;

public static class NetworkExtensions
{
	public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
	{
		var ipAdressBytes = address.GetAddressBytes();
		var subnetMaskBytes = subnetMask.GetAddressBytes();

		if (ipAdressBytes.Length != subnetMaskBytes.Length)
			throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

		var broadcastAddress = new byte[ipAdressBytes.Length];

		for (var i = 0; i < broadcastAddress.Length; i++)
			broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));

		return new IPAddress(broadcastAddress);
	}
}