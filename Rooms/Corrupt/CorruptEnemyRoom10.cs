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
using static TerRoguelike.Managers.NPCManager;
using static TerRoguelike.Schematics.SchematicManager;
using Terraria.ModLoader;
using TerRoguelike.NPCs.Enemy;

namespace TerRoguelike.Rooms
{
    public class CorruptEnemyRoom10 : Room
    {
        public override int AssociatedFloor => FloorDict["Corrupt"];
        public override string Key => "CorruptEnemyRoom10";
        public override string Filename => "Schematics/RoomSchematics/CorruptEnemyRoom10.csch";
        public override bool CanExitRight => true;
        public override void InitializeRoom()
        {
            base.InitializeRoom();
            AddRoomNPC(MakeEnemySpawnPos(Center, 12, 1), ChooseEnemy(AssociatedFloor, 2), 60, 120, 0.45f, 0);
            AddRoomNPC(MakeEnemySpawnPos(Center, -7, -2), ModContent.NPCType<GiantSpider>(), 60, 120, 0.45f, 0);

            AddRoomNPC(MakeEnemySpawnPos(Bottom, -5, -4), ChooseEnemy(AssociatedFloor, 1), 60, 120, 0.45f, 1);
            AddRoomNPC(MakeEnemySpawnPos(Center, -12, 1), ChooseEnemy(AssociatedFloor, 2), 60, 120, 0.45f, 1);
        }
    }
}
