﻿// Copyright (c) Forged WoW LLC <https://github.com/ForgedWoW/ForgedCore>
// Licensed under GPL-3.0 license. See <https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE> for full information.

namespace Game.Entities;

public struct CreateObjectBits
{
	public bool NoBirthAnim;
	public bool EnablePortals;
	public bool PlayHoverAnim;
	public bool MovementUpdate;
	public bool MovementTransport;
	public bool Stationary;
	public bool CombatVictim;
	public bool ServerTime;
	public bool Vehicle;
	public bool AnimKit;
	public bool Rotation;
	public bool AreaTrigger;
	public bool GameObject;
	public bool SmoothPhasing;
	public bool ThisIsYou;
	public bool SceneObject;
	public bool ActivePlayer;
	public bool Conversation;

	public void Clear()
	{
		NoBirthAnim = false;
		EnablePortals = false;
		PlayHoverAnim = false;
		MovementUpdate = false;
		MovementTransport = false;
		Stationary = false;
		CombatVictim = false;
		ServerTime = false;
		Vehicle = false;
		AnimKit = false;
		Rotation = false;
		AreaTrigger = false;
		GameObject = false;
		SmoothPhasing = false;
		ThisIsYou = false;
		SceneObject = false;
		ActivePlayer = false;
		Conversation = false;
	}
}