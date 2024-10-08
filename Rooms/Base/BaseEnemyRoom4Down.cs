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
    public class BaseEnemyRoom4Down : Room
    {
        public override int AssociatedFloor => FloorDict["Base"];
        public override string Key => "BaseEnemyRoom4Down";
        public override string Filename => "Schematics/RoomSchematics/BaseEnemyRoom4Down.csch";
        public override bool CanExitRight => true;
        public override bool CanExitDown => true;
        public override void InitializeRoom()
        {
            base.InitializeRoom();
            AddRoomNPC(PercentPosition(0.1f, 0.45f) + Vector2.UnitY * 24, ModContent.NPCType<Ballista>(), 60, 120, 0.45f);
            AddRoomNPC(PercentPosition(0.9f, 0.45f) + Vector2.UnitY * 24, ModContent.NPCType<Ballista>(), 60, 120, 0.45f);
            AddRoomNPC(PercentPosition(0.1f, 0.13f) + Vector2.UnitY * 24, ChooseEnemy(AssociatedFloor, 1), 240, 120, 0.45f);
            AddRoomNPC(PercentPosition(0.9f, 0.13f) + Vector2.UnitY * 24, ChooseEnemy(AssociatedFloor, 1), 240, 120, 0.45f);
        }
    }
}
