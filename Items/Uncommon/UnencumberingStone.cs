﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerRoguelike.Managers;
using TerRoguelike.Systems;
using TerRoguelike.TerPlayer;
using Microsoft.Xna.Framework.Graphics;
using TerRoguelike.Items.Common;

namespace TerRoguelike.Items.Uncommon
{
    public class UnencumberingStone : BaseRoguelikeItem, ILocalizedModType
    {
        public override int modItemID => ModContent.ItemType<UnencumberingStone>();
        public override bool UtilityItem => true;
        public override int itemTier => 1;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void ItemEffects(Player player)
        {
            player.GetModPlayer<TerRoguelikePlayer>().unencumberingStone += Item.stack;
        }
    }
}
