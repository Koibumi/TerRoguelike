﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerRoguelike.Managers;
using TerRoguelike.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TerRoguelike.Rooms
{
    public class CrimsonEnemyRoom5 : Room
    {
        public override int ID => 24;
        public override string Key => "CrimsonEnemyRoom5";
        public override string Filename => "Schematics/RoomSchematics/CrimsonEnemyRoom5.csch";
        public override bool CanExitRight => true;
        public override bool CanExitDown => true;
        public override bool CanExitUp => true;
        public override void InitializeRoom()
        {
            base.InitializeRoom();
            AddRoomNPC(0, new Vector2((RoomDimensions.X * 16f / 4f), (RoomDimensions.Y * 16f / 2f) - 32f), NPCID.Crimera, 60, 120, 0.45f);
            AddRoomNPC(1, new Vector2((RoomDimensions.X * 16f / 4f * 3f), (RoomDimensions.Y * 16f / 2f) - 32f), NPCID.Crimera, 60, 120, 0.45f);
            AddRoomNPC(2, new Vector2((RoomDimensions.X * 16f) - 48f, (RoomDimensions.Y * 16f) - 32f), NPCID.BloodCrawler, 60, 120, 0.45f);
        }
    }
}
