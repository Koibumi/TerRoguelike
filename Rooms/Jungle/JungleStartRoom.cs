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
using static TerRoguelike.Schematics.SchematicManager;

namespace TerRoguelike.Rooms
{
    public class JungleStartRoom : Room
    {
        public override int AssociatedFloor => FloorDict["Jungle"];
        public override string Key => "JungleStartRoom";
        public override string Filename => "Schematics/RoomSchematics/JungleStartRoom.csch";
        public override bool CanExitRight => true;
        public override bool CanExitDown => true;
        public override bool CanExitUp => true;
        public override bool IsStartRoom => true;
        public override void InitializeRoom()
        {
            active = false;
        }
        public override void Update()
        {
            active = false;
        }
    }
}
