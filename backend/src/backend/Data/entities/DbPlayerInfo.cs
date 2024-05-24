﻿using backend.game;
using backend.game.entities;
using System.Diagnostics;

namespace backend.Data.entities
{
    internal class DbPlayerInfo : DbEntity
    {
        public DbPlayerInfo() { }
        public DbPlayerInfo(IPlayer player) : base(player.Id)
        {
            Id = player.Id;
            Username = player.Username;
        }
        public DbPlayerInfo(PlayerInfo playerInfo) : base(playerInfo)
        {
            Username = playerInfo.Username;
        }

        public string Username { get; set; } = string.Empty;
    }
}