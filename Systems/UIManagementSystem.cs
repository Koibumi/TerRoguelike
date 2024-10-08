﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using TerRoguelike.Managers;
using TerRoguelike.TerPlayer;
using TerRoguelike.World;
using Terraria.ID;
using TerRoguelike.Systems;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using TerRoguelike.NPCs;
using static TerRoguelike.Schematics.SchematicManager;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using TerRoguelike.UI;

namespace TerRoguelike.Systems
{
    public class UIManagementSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                int deathTextIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Death Text");
                if (deathTextIndex != -1)
                {
                    GameInterfaceLayer layer = layers[deathTextIndex];
                    layer.Active = false;
                }
            }
            
            if (Main.myPlayer > -1 && Main.myPlayer < Main.maxPlayers)
            {
                Player player = Main.player[Main.myPlayer];
                TerRoguelikePlayer modPlayer = player.GetModPlayer<TerRoguelikePlayer>();

                if (modPlayer.deathEffectTimer != 0 || player.dead || CreditsSystem.creditsActive)
                {
                    for (int i = 0; i < layers.Count; i++)
                    {
                        GameInterfaceLayer layer = layers[i];
                        if (layer.Name == "Vanilla: Hotbar")
                        {
                            layer.Active = false;
                            continue;
                        }
                        if (layer.Name == "Vanilla: Map / Minimap")
                        {
                            layer.Active = false;
                            continue;
                        }
                        if (layer.Name == "Vanilla: Resource Bars")
                        {
                            layer.Active = false;
                            continue;
                        }
                        if (layer.Name == "Vanilla: Info Accessories Bar")
                        {
                            layer.Active = false;
                            continue;
                        }
                        if (layer.Name == "Vanilla: Inventory")
                        {
                            layer.Active = false;
                            continue;
                        }
                    }
                }

                if (player.dead && TerRoguelikeWorld.IsTerRoguelikeWorld)
                {
                    int deathIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
                    if (deathIndex != -1)
                    {
                        layers.Insert(deathIndex, new LegacyGameInterfaceLayer("Death UI", () =>
                        {
                            DeathUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                            return true;
                        }, InterfaceScaleType.None));
                    }
                }
                else if (CreditsSystem.creditsActive)
                {
                    int creditsIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
                    if (creditsIndex != -1)
                    {
                        layers.Insert(creditsIndex, new LegacyGameInterfaceLayer("Credits UI", () =>
                        {
                            CreditsUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                            return true;
                        }, InterfaceScaleType.None));
                    }
                }
            }

            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (mouseIndex == -1)
                return;

            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Debug UI", () =>
            {
                DebugUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                return true;
            }, InterfaceScaleType.None));

            if (CreditsSystem.creditsActive)
                return;

            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Barrier Bar", () =>
            {
                BarrierUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                return true;
            }, InterfaceScaleType.None));
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Item Basin UI", () =>
            {
                ItemBasinUI.Draw();
                return true;
            }, InterfaceScaleType.Game));
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Escape UI", () =>
            {
                EscapeUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Enemy Healthbar UI", () =>
            {
                EnemyHealthbarUI.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.UI));
        }
    }
}
