![](Forged_Logo.png)
<h1 align="center"> </h1>

[![GPLv3 License](https://img.shields.io/badge/License-GPL%20v3-yellow.svg)](https://github.com/ForgedWoW/ForgedCore/blob/master/LICENSE)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/ForgedWoW/ForgedCore/dotnet.yml)](https://github.com/ForgedWoW/ForgedCore/actions)

[![GitHub last commit](https://img.shields.io/github/last-commit/ForgedWoW/ForgedCore)](https://github.com/ForgedWoW/ForgedCore/commits/master)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/w/ForgedWoW/ForgedCore)](https://github.com/ForgedWoW/ForgedCore/commits/master)
[![PR's Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat)](https://github.com/ForgedWoW/ForgedCore/pulls) 

Forged Core is an open source branch of CypherCore.
Forge Core is the core of the Forged WoW Server and aims to be the most complete and up to date core.

#### Forged Core also features:
* Many performance imporvements! Base server tick sub 1ms!
* A very extencible script loading system!
  - re-written from the ground up for easy additions to all types of gameplay. 
  - Scripts can be loaded into the script directory and reloaded during runtime.
  - Add spell, aura, gameobject and creature scripts without adding to the DB!
  - Easy to add hooks with calling foreach script in the script manager.
  - Interfaces for all script hooks allow for streamlined calls and easy implimentation 
* Enhanced multi-threding of maps, sockets, logging and database.
* Ultra customization of almost every aspect of modern wow.
  - Customize xpack level ranges
  - Open up allied races without requirements 
  - Open up additional race/class combos
* Many class spells/talents up to date and more on the way!

[Forged WoW Website](http://forgedwow.gg/)

CypherCore is an open source server project for World of Warcraft written in C#.

The current support game version is: 10.0.5.48317

### Prerequisites
* .NET 7.0 SDK [Download](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* MariaDB 10.6/Mysql 8 or higher [Download](https://mariadb.org/download/)
* Optional: Visual Studio 2022, Visual Studio Code or Jetbrains Rider

### Server Setup
* ~~Download and Complie the Extractor [Download](https://github.com/CypherCore/Tools)~~ Use TrinityCore extractors for now: [Download](https://ci.appveyor.com/project/DDuarte/trinitycore/branch/master/artifacts)
* Run all extractors in the wow directory
* Copy all created folders into server directory (ex: C:\CypherCore\Data)
* Make sure Conf files are updated and point the the correct folders and sql user and databases

### Installing the database
* Download the full Trinity Core database (TDB 1002.22121) [Download](https://github.com/TrinityCore/TrinityCore/releases)
* Extract the sql files into the core sql folder (ex: C:\CypherCore\sql)

### Playing
* Must use Arctium WoW Client Launcher [Download](https://arctium.io/wow)

### Support / General Info
* Check out our Discord [![Discord](https://img.shields.io/discord/920073768162963477)](https://discord.gg/forgedwow)
* The Discord also has a support channel for [Forged Core related questions/issue](https://discord.com/channels/920073768162963477/1080978458714329260)
* Check out Trinity Core Wiki as a few steps are the same [Here](https://trinitycore.atlassian.net/wiki/spaces/tc/pages/2130077/Installation+Guide)

### Legal
* Blizzard, Battle.net, World of Warcraft, and all associated logos and designs are trademarks or registered trademarks of Blizzard Entertainment.
* All other trademarks are the property of their respective owners. This project is **not** affiliated with Blizzard Entertainment or any of their family of sites.
