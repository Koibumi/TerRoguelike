﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TerRoguelike.Items.Rare;
using TerRoguelike.Items.Weapons;
using TerRoguelike.Managers;
using TerRoguelike.NPCs;
using TerRoguelike.Projectiles;
using TerRoguelike.Systems;
using TerRoguelike.UI;
using TerRoguelike.Utilities;
using TerRoguelike.World;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using static TerRoguelike.Managers.TextureManager;
using TerRoguelike.Particles;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using TerRoguelike.MainMenu;
using Terraria.GameInput;
using TerRoguelike.Schematics;
using System.Diagnostics;
using TerRoguelike.NPCs.Enemy.Boss;
using static TerRoguelike.MainMenu.TerRoguelikeMenu;
using Terraria.WorldBuilding;
using TerRoguelike.Items.Common;
using TerRoguelike.Items.Uncommon;
using static Terraria.Player;

namespace TerRoguelike.TerPlayer
{
    public class TerRoguelikePlayer : ModPlayer
    {
        public static float BloodMoonIframeMultiplier = 0.75f;
        public static float NewMoonIframeMultiplier = 1.5f;
        public static readonly SoundStyle JetLegCooldown = new SoundStyle("TerRoguelike/Sounds/JetLegUp");
        public static readonly SoundStyle WayfarerProc = new SoundStyle("TerRoguelike/Sounds/WayfarerProc");
        public static readonly SoundStyle PrimevalRattleProc = new SoundStyle("TerRoguelike/Sounds/PrimevalRattleProc", 3);
        public static readonly SoundStyle PrimevalBoost = new SoundStyle("TerRoguelike/Sounds/PrimevalBoost");

        #region Item Variables
        public int coolantBarrel;
        public int clingyGrenade;
        public int pocketSpotter;
        public int antiqueLens;
        public int instigatorsBrace;
        public int hotPepper;
        public int brazenNunchucks;
        public int attackPlan;
        public int sanguineOrb;
        public int livingCrystal;
        public int soulstealCoating;
        public int bottleOfVigor;
        public int benignFungus;
        public int sentientPutty;
        public int memoryFoam;
        public int runningShoe;
        public int bunnyHopper;
        public int timesHaveBeenTougher;
        public int rustedShield;
        public int amberBead;
        public int flimsyPauldron;
        public int protectiveBubble;
        public int burningCharcoal;
        public int reactiveMicrobots;
        public int remedialTapeworm;

        public int lockOnMissile;
        public int evilEye;
        public int spentShell;
        public int heatSeekingChip;
        public int backupDagger;
        public int clusterBombSatchel;
        public int retaliatoryFist;
        public int bloodSiphon;
        public int enchantingEye;
        public int automaticDefibrillator;
        public int stimPack;
        public int barbedLasso;
        public int steamEngine;
        public int bouncyBall;
        public int airCanister;
        public int unencumberingStone;
        public int ballAndChain;
        public int soulOfLena;
        public int emeraldRing;
        public int amberRing;
        public int thrillOfTheHunt;
        public int giftBox;
        public int ancientTwig;
        public int disposableTurret;
        public int wayfarersWaistcloth;

        public int volatileRocket;
        public int theDreamsoul;
        public int droneBuddy;
        public int overclocker;
        public int shotgunComponent;
        public int sniperComponent;
        public int minigunComponent;
        public int cornucopia;
        public int nutritiousSlime;
        public int allSeeingEye;
        public int symbioticFungus;
        public int itemPotentiometer;
        public int barrierSynthesizer;
        public int jetLeg;
        public int giantDoorShield;
        public int trumpCard;
        public int portableGenerator;
        public int forgottenBioWeapon;
        public int lunarCharm;
        public int ceremonialCrown;
        public int thermitePowder;
        public int everlastingJellyfish;
        public int heartyHoneycomb;
        public int primevalRattle;

        public List<int> evilEyeStacks = new List<int>();
        public List<int> thrillOfTheHuntStacks = new List<int>();
        public int benignFungusCooldown = 0;
        public int storedDaggers = 0;
        public int soulOfLenaUses = 0;
        public bool soulOfLenaHurtVisual = false;
        public List<int> barbedLassoTargets = new List<int>();
        public int barbedLassoHitCooldown = 0;
        public List<int> steamEngineStacks = new List<int>();
        public int allSeeingEyeTarget = -1;
        public int droneBuddyState = 0; // 0 - passive, 1 - aggressive, 2 - healing
        public float droneBuddyAttackCooldown = 0;
        public int droneTarget = -1;
        public int droneBuddyHealTime = 0;
        public int allSeeingEyeHitCooldown = 30;
        public int overclockerTime = 0;
        public int portableGeneratorImmuneTime = 0;
        public int symbioticFungusHealCooldown = 60;
        public int ceremonialCrownStacks;
        public int oldCeremonialCrownStacks;
        public int thermitePowderCooldown = 0;
        public int ancientTwigLastStruckTimer = 0;
        public int ancientTwigCooldown = 0;
        public int ancientTwigSetRestoreRate = 100;
        public int wayfarersWaistclothDirTime = 0;
        public List<int> primevalRattleStacks = [];
        public bool primevalRattleBoost = false;
        #endregion

        #region Misc Variables
        public Floor currentFloor;
        public bool escaped = false;
        public bool escapeFail = false;
        public int shotsToFire = 1;
        public int extraDoubleJumps = 0;
        public int timesDoubleJumped = 0;
        public float jumpSpeedMultiplier;
        public float scaleMultiplier;
        public int procLuck = 0;
        public float swingAnimCompletion = 0;
        public bool lockDirection = false;
        public int bladeFlashTime = 0;
        public Vector2 playerToCursor = Vector2.Zero;
        public float barrierHealth = 0;
        public bool barrierFullAbsorbHit = false;
        public int barrierInHurt = 0;
        public float barrierFloor = 0;
        public int outOfDangerTime = 600;
        public bool dodgeAttack = false;
        public float healMultiplier = 1f;
        public float diminishingDR = 0f;
        public float barrierDiminishingDR = 0f;
        public bool onGround = false;
        public int standingStillTime = 0;
        public float previousBonusDamageMulti = 1f;
        public int timeAttacking = 0;
        public Vector2 lenaVisualPosition = Vector2.Zero;
        public Vector2 droneBuddyVisualPosition = Vector2.Zero;
        public float droneBuddyRotation = 0f;
        public float droneSeenRot = 0f;
        public List<VisualFungus> visualFungi = new List<VisualFungus>();
        public List<VisualFungus> primevalStreaks = [];
        public int currentRoom = -1;
        public int DashDir = 0;
        public int DashDelay = 0;
        public int DashTime = -6;
        public int DashDirCache = 0;
        public int deathEffectTimer = 0;
        public bool reviveDeathEffect = false;
        public int deadTime = 0;
        public int killerNPC = -1;
        public int killerNPCType = 0;
        public int killerProj = -1;
        public int killerProjType = 0;
        public bool brainSucked = false;
        public int brainSucklerTime = 0;
        public bool oldPulley = false;
        public int startDirection = 1;
        public ItemBasinEntity selectedBasin = null;
        public int cacheRoomListWarp = -1;
        public bool noThrow = false;
        public int escapeArrowTime = 0;
        public Vector2 escapeArrowTarget = Vector2.Zero;
        public bool moonLordVisualEffect = false;
        public bool moonLordSkyEffect = false;
        public int creditsViewTime = 0;
        public bool isDeletableOnExit = false;
        public bool enableCampfire = false;
        public Stopwatch playthroughTime = new Stopwatch();
        public float PlayerBaseDamageMultiplier { get { return Player.GetTotalDamage(DamageClass.Generic).ApplyTo(1f); } }
        #endregion

        #region Reset Variables
        public override void PreUpdate()
        {
            startDirection = Player.direction;

            coolantBarrel = 0;
            clingyGrenade = 0;
            pocketSpotter = 0;
            antiqueLens = 0;
            instigatorsBrace = 0;
            hotPepper = 0;
            brazenNunchucks = 0;
            attackPlan = 0;
            sanguineOrb = 0;
            livingCrystal = 0;
            soulstealCoating = 0;
            bottleOfVigor = 0;
            benignFungus = 0;
            sentientPutty = 0;
            memoryFoam = 0;
            runningShoe = 0;
            bunnyHopper = 0;
            timesHaveBeenTougher = 0;
            rustedShield = 0;
            amberBead = 0;
            flimsyPauldron = 0;
            protectiveBubble = 0;
            burningCharcoal = 0;
            reactiveMicrobots = 0;
            remedialTapeworm = 0;

            lockOnMissile = 0;
            evilEye = 0;
            spentShell = 0;
            heatSeekingChip = 0;
            backupDagger = 0;
            clusterBombSatchel = 0;
            retaliatoryFist = 0;
            bloodSiphon = 0;
            enchantingEye = 0;
            automaticDefibrillator = 0;
            stimPack = 0;
            barbedLasso = 0;
            steamEngine = 0;
            bouncyBall = 0;
            airCanister = 0;
            unencumberingStone = 0;
            ballAndChain = 0;
            soulOfLena = 0;
            emeraldRing = 0;
            amberRing = 0;
            thrillOfTheHunt = 0;
            giftBox = 0;
            ancientTwig = 0;
            disposableTurret = 0;
            wayfarersWaistcloth = 0;

            volatileRocket = 0;
            theDreamsoul = 0;
            droneBuddy = 0;
            overclocker = 0;
            shotgunComponent = 0;
            sniperComponent = 0;
            minigunComponent = 0;
            cornucopia = 0;
            nutritiousSlime = 0;
            allSeeingEye = 0;
            symbioticFungus = 0;
            itemPotentiometer = 0;
            barrierSynthesizer = 0;
            jetLeg = 0;
            giantDoorShield = 0;
            trumpCard = 0;
            portableGenerator = 0;
            forgottenBioWeapon = 0;
            lunarCharm = 0;
            ceremonialCrown = 0;
            thermitePowder = 0;
            everlastingJellyfish = 0;
            heartyHoneycomb = 0;
            primevalRattle = 0;

            shotsToFire = 1;
            jumpSpeedMultiplier = 0f;
            extraDoubleJumps = 0;
            procLuck = 0;
            scaleMultiplier = 1f;
            healMultiplier = 1f;
            diminishingDR = 0f;
            barrierDiminishingDR = 0f;
            previousBonusDamageMulti = 1f;

            onGround = (ParanoidTileRetrieval((int)(Player.Bottom.X / 16f), (int)((Player.Bottom.Y) / 16f)).IsTileSolidGround() && Math.Abs(Player.velocity.Y) <= 0.1f);
            if (Player.velocity.Y == 0 && !Player.controlDown && !Player.controlUp && !Player.controlLeft && !Player.controlRight)
                standingStillTime++;
            else
                standingStillTime = 0;

            if (selectedBasin != null)
            {
                Vector2 checkPos = selectedBasin.position.ToWorldCoordinates(24, 16);
                if (Player.Center.Distance(checkPos) > 200)
                {
                    selectedBasin = null;
                }
            }

            if (escapeArrowTime > 0)
                escapeArrowTime--;

            barrierFloor = 0;
            barrierFullAbsorbHit = false;
            barrierInHurt = 0;
        }
        public override void ResetEffects()
        {
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < 15)
            {
                DashDir = 1;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < 15)
            {
                DashDir = -1;
            }
            else
            {
                DashDir = 0;
            }

        }
        #endregion

        #region Always Active Effects
        public override void UpdateEquips()
        {
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                Player.noBuilding = !DebugUI.allowBuilding;
                Player.noFallDmg = true;
                Player.noKnockback = true;
                Player.GetCritChance(DamageClass.Generic) -= 3f;
                Player.runAcceleration *= 1.75f;
                Player.maxRunSpeed *= 1.15f;
                Player.accRunSpeed *= 1.15f;
                Player.runSlowdown *= 1.75f;
                Player.GetJumpState(ExtraJump.CloudInABottle).Enable();
                Player.lifeRegen += 4;
                if (NewMoonActive)
                    Player.lifeRegen += 4;

                Player.lifeRegenTime = 0;
                if (Player.controlDown)
                {
                    Player.gravity *= 1.5f;
                }
            }
            outOfDangerTime++;

            if (Player.itemAnimation > 0 && Player.HeldItem.damage > 0)
            {
                if (timeAttacking < 0)
                    timeAttacking = 0;

                timeAttacking++;
            }
            else
            {
                timeAttacking--;

                if (timeAttacking > 0)
                    timeAttacking = 0;

                if (overclocker > 0)
                {
                    if (overclockerTime < 360)
                    {
                        overclockerTime += overclocker;
                        if (overclockerTime > 360)
                            overclockerTime = 360;
                    }
                        
                }
            }

            if (!brainSucked)
            {
                if (brainSucklerTime > 0)
                {
                    brainSucklerTime -= 5;
                    if (brainSucklerTime < 0)
                        brainSucklerTime = 0;
                }
                    
            }
            else
            {
                if (brainSucklerTime < 600)
                    brainSucklerTime++;
            }
            brainSucked = false;
            if (cornucopia > 0)
            {
                float healMultiIncrease = cornucopia * 0.5f;
                healMultiplier += healMultiIncrease;
            }
            //max life effects happen before barrier calculations
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                int maxLifeIncrease = TerRoguelikeWorld.currentStage * 20;
                Player.statLifeMax2 += maxLifeIncrease;
            }
            if (bottleOfVigor > 0)
            {
                int maxLifeIncrease = bottleOfVigor * 15;
                Player.statLifeMax2 += maxLifeIncrease;
            }
            if (steamEngine > 0)
            {
                int maxLifeIncrease = steamEngine * steamEngineStacks.Count * 2;
                Player.statLifeMax2 += maxLifeIncrease;
                for (int i = 0; i < steamEngineStacks.Count; i++)
                {
                    steamEngineStacks[i]--;
                }
                steamEngineStacks.RemoveAll(x => x <= 0);
            }
            else if (steamEngineStacks.Count > 0)
            {
                for (int i = 0; i < steamEngineStacks.Count; i++)
                {
                    steamEngineStacks[i]--;
                }
                steamEngineStacks.RemoveAll(x => x <= 0);
            }

            //barrier calculations happen now
            if (rustedShield > 0)
            {
                float addedBarrier = 0.08f * Player.statLifeMax2 * rustedShield;
                barrierFloor += addedBarrier;
            }
            if (barrierFloor > Player.statLifeMax2)
                barrierFloor = Player.statLifeMax2;

            if (barrierHealth < barrierFloor && outOfDangerTime >= 420 && barrierFloor != 0)
            {
                barrierHealth += barrierFloor * 0.0083334f;
            }
            if (barrierHealth > barrierFloor)
            {
                float decayRate = 0.04f * 0.0166f;
                if (ancientTwig > 0 && ancientTwigLastStruckTimer > 0)
                {
                    decayRate *= (float)Math.Pow(0.85f, ancientTwig);
                }
                barrierHealth -= Player.statLifeMax2 * (decayRate);
                if (barrierHealth < barrierFloor)
                    barrierHealth = barrierFloor;
            }
            if (ancientTwigLastStruckTimer > 0)
            {
                if (ancientTwig > 0 && ancientTwigCooldown <= 0)
                {
                    int barrierHealAmt = ancientTwig * 2 + 3;
                    AddBarrierHealth(barrierHealAmt);
                    ancientTwigCooldown = ancientTwigSetRestoreRate;
                }
                ancientTwigLastStruckTimer--;
            }
            if (ancientTwigCooldown > 0)
                ancientTwigCooldown--;

            if (deathEffectTimer > 0)
            {
                deathEffectTimer--;
                if (!reviveDeathEffect && deathEffectTimer <= 1)
                    ZoomSystem.SetZoomAnimation(Main.GameZoomTarget, 60);
            }
                
            if (deathEffectTimer == 1 && reviveDeathEffect)
            {
                ExtraSoundSystem.ExtraSounds.Add(new(SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = 1f }, Player.Center), 2));
                for (int i = 0; i < 20; i++)
                {
                    Vector2 projVel = Main.rand.NextVector2CircularEdge(4, 4) * Main.rand.NextFloat(0.5f, 1f);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, projVel, ModContent.ProjectileType<TrumpCardProjectile>(), 300, 0.25f);
                }
            }
            if (deathEffectTimer > 0)
                return;

            //everything else
            if (coolantBarrel > 0)
            {
                float attackSpeedIncrease = coolantBarrel * 0.10f;
                Player.GetAttackSpeed(DamageClass.Generic) += attackSpeedIncrease;
            }
            if (pocketSpotter > 0)
            {
                float critIncrease = pocketSpotter * 10f;
                Player.GetCritChance(DamageClass.Generic) += critIncrease;
            }
            if (antiqueLens > 0)
            {
                float scaleIncrease = antiqueLens * 0.1f;
                scaleMultiplier += scaleIncrease;
            }
            if (livingCrystal > 0)
            {
                int regenIncrease = livingCrystal * 2;
                Player.lifeRegen += regenIncrease;
            }
            if (benignFungusCooldown > 0)
                benignFungusCooldown--;
            if (benignFungus > 0 && benignFungusCooldown == 0 && Math.Abs(Player.velocity.X) > 2.5f && onGround)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Bottom + new Vector2(0, -5f), Vector2.Zero, ModContent.ProjectileType<HealingFungus>(), 0, 0f, Player.whoAmI);
                benignFungusCooldown += Main.rand.Next(13, 16);
            }
            if (sentientPutty > 0 && outOfDangerTime == 120)
            {
                int healAmt = sentientPutty * 5;
                SoundEngine.PlaySound(new SoundStyle("TerRoguelike/Sounds/OrbHeal", 5) { Volume = 0.12f }, Player.Center);
                ScaleableHeal(healAmt);
            }
            if (memoryFoam > 0 && outOfDangerTime >= 420)
            {
                int regenIncrease = 2 + (memoryFoam * 4);
                Player.lifeRegen += regenIncrease;
            }
            if (runningShoe > 0)
            {
                float speedIncrease = runningShoe * 0.08f;
                Player.moveSpeed += speedIncrease;
            }
            if (bunnyHopper > 0)
            {
                float jumpSpeedIncrease = bunnyHopper * 0.12f;
                jumpSpeedMultiplier += jumpSpeedIncrease;
            }
            if (protectiveBubble > 0 && outOfDangerTime >= 420)
            {
                float drIncrease = protectiveBubble * 30f;
                diminishingDR += drIncrease;
            }
            if (reactiveMicrobots > 0 && Player.statLife <= Player.statLifeMax2 * 0.5f)
            {
                float drIncrease = reactiveMicrobots * 12;
                diminishingDR += drIncrease;
                ParticleManager.AddParticle(new Square(Main.rand.NextVector2FromRectangle(Player.getRect()), Main.rand.NextVector2Circular(3, 3), 7, Color.LightSteelBlue, new Vector2(1f), 0, 0.96f, 7));
            }
            if (NewMoonActive)
            {
                diminishingDR += 30;
            }

            if (evilEye > 0)
            {
                Player.GetCritChance(DamageClass.Generic) += 5;
                if (evilEyeStacks.Count > 0)
                {
                    for (int i = 0; i < evilEyeStacks.Count; i++)
                    {
                        evilEyeStacks[i]--;
                    }

                    evilEyeStacks.RemoveAll(time => time <= 0);

                    Player.GetAttackSpeed(DamageClass.Generic) += MathHelper.Clamp(evilEyeStacks.Count, 1, 4 + evilEye) * 0.1f * (float)evilEye;
                }
            }
            else if (evilEyeStacks.Count > 0)
                evilEyeStacks.Clear();

            if (enchantingEye > 0)
            {
                Player.GetCritChance(DamageClass.Generic) += 5;
            }
            if (spentShell > 0)
            {
                shotsToFire += spentShell;
            }
            if (backupDagger > 0)
            {
                int maxStocks = 3 + backupDagger;
                int stockRate = (int)(360f / (float)maxStocks);
                if (storedDaggers > maxStocks)
                {
                    storedDaggers = maxStocks;
                    SoundEngine.PlaySound(SoundID.Item7, Player.Center);
                }
                    
                int releaseCooldown = (int)(stockRate / 2f);
                if (releaseCooldown > 8)
                    releaseCooldown = 8;
                if (releaseCooldown < 1)
                    releaseCooldown = 1;

                if (timeAttacking > 0)
                {
                    if (stockRate < 1)
                        stockRate = 1;

                    if (timeAttacking % stockRate == 0 && storedDaggers < maxStocks)
                    {
                        storedDaggers++;
                        if (storedDaggers == maxStocks)
                            SoundEngine.PlaySound(SoundID.Item63 with { Volume = 1f }, Player.Center);
                        else
                            SoundEngine.PlaySound(SoundID.Item64 with { Volume = 0.5f }, Player.Center);
                    }
                }
                else if (Math.Abs(timeAttacking) % releaseCooldown == 0 && storedDaggers > 0)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (AimWorld() - Player.Center).SafeNormalize(Vector2.UnitX) * 2.2f, ModContent.ProjectileType<ThrownBackupDagger>(), 100, 1f, Player.whoAmI, -1f);
                    storedDaggers--;
                }
            }
            else
                storedDaggers = 0;

            if (stimPack > 0)
            {
                if (currentRoom != -1)
                {
                    if (RoomSystem.RoomList[currentRoom].roomTime <= 600)
                    {
                        int lifeRegenIncrease = stimPack * 16;
                        Player.lifeRegen += lifeRegenIncrease;
                    }
                }
            }

            if (barbedLasso > 0)
            {
                barbedLassoTargets.Clear();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.life <= 0 || npc.immortal || npc.dontTakeDamage || npc.friendly)
                        continue;

                    float requiredDistance = 128f;
                    float npcDistance = Player.Center.Distance(npc.ModNPC().ClosestPosition(npc.getRect().ClosestPointInRect(Player.Center), Player.Center, npc));
                    if (npcDistance <= requiredDistance)
                    {
                        barbedLassoTargets.Add(i);
                        if (barbedLassoTargets.Count == barbedLasso)
                            break;
                    }
                }

                if (barbedLassoHitCooldown <= 0 && barbedLassoTargets.Count > 0)
                {
                    int totalHealAmt = 0;
                    for (int i = 0; i < barbedLassoTargets.Count; i++)
                    {
                        NPC target = Main.npc[barbedLassoTargets[i]];
                        int hitDamage = (int)(Player.statLifeMax2 * 0.02f);

                        hitDamage = (int)(hitDamage * GetBonusDamageMulti(target, target.ModNPC().ClosestPosition(target.getRect().ClosestPointInRect(Player.Center), Player.Center, target)));

                        NPC.HitInfo info = new NPC.HitInfo();
                        info.HideCombatText = true;
                        int actualHitDamage = hitDamage * 10;
                        info.Damage = actualHitDamage;
                        if (target.life - actualHitDamage <= 0)
                        {
                            OnKillEffects(target);
                        }
                        info.InstantKill = false;
                        info.HitDirection = Main.rand.NextBool() ? -1 : 1;
                        info.Knockback = 0f;
                        info.Crit = false;

                        target.StrikeNPC(info);
                        NetMessage.SendStrikeNPC(target, info);
                        CombatText.NewText(target.getRect(), Color.DarkGreen, actualHitDamage);

                        totalHealAmt += hitDamage;
                    }

                    ScaleableHeal(totalHealAmt);

                    barbedLassoHitCooldown += 30;
                }
                if (barbedLassoHitCooldown > 0)
                    barbedLassoHitCooldown--;
            }
            else
            {
                barbedLassoTargets.Clear();
                if (barbedLassoHitCooldown > 0)
                    barbedLassoHitCooldown--;
            }

            if (airCanister > 0)
            {
                extraDoubleJumps += airCanister;
            }
            if (unencumberingStone > 0 && timeAttacking <= -60)
            {
                float speedIncrease = unencumberingStone * 0.25f;
                Player.moveSpeed += speedIncrease;
            }
            if (emeraldRing > 0 && timeAttacking > 0)
            {
                if (Player.HeldItem.CountsAsClass(DamageClass.Melee))
                {
                    int drIncrease = emeraldRing * 20;
                    diminishingDR += drIncrease;
                }
            }
            if (thrillOfTheHunt > 0)
            {
                if (thrillOfTheHuntStacks.Count > 0)
                {
                    for (int i = 0; i < thrillOfTheHuntStacks.Count; i++)
                    {
                        thrillOfTheHuntStacks[i]--;
                    }

                    thrillOfTheHuntStacks.RemoveAll(time => time <= 0);

                    Player.GetAttackSpeed(DamageClass.Generic) += MathHelper.Clamp(thrillOfTheHuntStacks.Count, 1, 4 + thrillOfTheHunt) * 0.15f * (float)thrillOfTheHunt;
                    Player.moveSpeed += MathHelper.Clamp(thrillOfTheHuntStacks.Count, 1, 4 + thrillOfTheHunt) * 0.15f;
                }
            }
            else if (thrillOfTheHuntStacks.Count > 0)
                thrillOfTheHuntStacks.Clear();

            if (wayfarersWaistcloth > 0)
            {
                if (!(Player.controlLeft && Player.controlRight))
                {
                    if (Player.controlLeft && Player.velocity.X != 0)
                    {
                        if (wayfarersWaistclothDirTime > 0)
                            wayfarersWaistclothDirTime = 0;
                        wayfarersWaistclothDirTime--;
                    }
                    else if (Player.controlRight && Player.velocity.X != 0)
                    {
                        if (wayfarersWaistclothDirTime < 0)
                            wayfarersWaistclothDirTime = 0;
                        wayfarersWaistclothDirTime++;
                    }
                    else
                    {
                        wayfarersWaistclothDirTime = 0;
                    }
                }
                else
                {
                    wayfarersWaistclothDirTime = 0;
                }
                if (Math.Abs(wayfarersWaistclothDirTime) >= 90)
                {
                    if (Math.Abs(wayfarersWaistclothDirTime) == 90)
                    {
                        SoundEngine.PlaySound(WayfarerProc with { Volume = 1f,  Pitch = 0.4f }, Player.Center);
                        int particleDir = -Math.Sign(wayfarersWaistclothDirTime);

                        for (int i = -1; i <= 1; i++)
                        {
                            Vector2 particlePos = Player.Center + new Vector2(Player.width * 0.5f * particleDir, Player.height * 0.25f * i + Player.gfxOffY);
                            Vector2 particleVel = Vector2.UnitX * particleDir * 2f;
                            ParticleManager.AddParticle(new ThinSpark(particlePos, particleVel, 30, Color.Lerp(Color.Blue, Color.Cyan, 0.5f), new Vector2(0.16f, 0.1f) * Main.rand.NextFloat(0.9f, 1f), 0, true, false));
                        }
                        for (int i = 0; i < 9; i++)
                        {
                            ParticleManager.AddParticle(new Ball(Player.Center, (i / 9f * MathHelper.TwoPi + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * 5 * Main.rand.NextFloat(0.3f, 1f) + Vector2.UnitX * particleDir,
                                37, Color.Lerp(Color.Blue, Color.Cyan, Main.rand.NextFloat(0.6f, 1f)), new Vector2(0.1f), 0, 0.9f, 24, false));
                        }
                    }
                    float speedIncrease = wayfarersWaistcloth * 0.2f;
                    Player.moveSpeed += speedIncrease;
                }
            }
            else
                wayfarersWaistclothDirTime = 0;

            if (theDreamsoul > 0)
            {
                int luckIncrease = theDreamsoul;
                procLuck += luckIncrease;
            }
            if (droneBuddy > 0)
            {
                if (droneBuddyState != 2 && droneBuddyHealTime < 600)
                    droneBuddyHealTime++;

                if (droneTarget != -1 && droneBuddyAttackCooldown <= 0)
                {
                    droneBuddyRotation = droneBuddyRotation % MathHelper.TwoPi;
                    NPC npc = Main.npc[droneTarget];
                    if (!npc.CanBeChasedBy() || npc.life <= 0 || npc.Center.Distance(Player.Center) > 960f || !Collision.CanHit(droneBuddyVisualPosition, 1, 1, npc.Center, 1, 1))
                        droneTarget = -1;
                    else
                        droneBuddyAttackCooldown += 20f / (1f + ((droneBuddy - 1) * 0.5f));
                }
                if (droneTarget == -1)
                {
                    float bestTargetDistance = 960f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.life <= 0 || !npc.CanBeChasedBy())
                            continue;

                        if (!Collision.CanHit(droneBuddyVisualPosition, 1, 1, npc.Center, 1, 1))
                            continue;

                        float distance = npc.Center.Distance(Player.Center);
                        if (distance <= bestTargetDistance)
                        {
                            bestTargetDistance = distance;
                            droneTarget = i;
                            droneBuddyAttackCooldown = 20;
                        }
                    }
                }
                if (droneTarget != -1)
                {
                    droneBuddyState = 1;
                    droneBuddyAttackCooldown--;
                    if (droneBuddyAttackCooldown <= 0)
                    {
                        Vector2 projSpawnPos = droneBuddyVisualPosition + (Vector2.UnitY.RotatedBy(droneSeenRot) * 11f * new Vector2(1.2f, 1));
                        droneBuddyAttackCooldown = 0;
                        int spawnedProjectile = Projectile.NewProjectile(Player.GetSource_FromThis(), projSpawnPos, (Main.npc[droneTarget].Center - projSpawnPos).SafeNormalize(Vector2.UnitY) * 11.25f, ModContent.ProjectileType<AdaptiveGunBullet>(), 100, 1f, Player.whoAmI);
                        Main.projectile[spawnedProjectile].scale = scaleMultiplier;
                        SoundEngine.PlaySound(SoundID.Item108 with { Volume = 0.3f }, droneBuddyVisualPosition);
                        Dust dust = Dust.NewDustDirect(projSpawnPos - new Vector2(2), 4, 4, DustID.YellowTorch);
                        dust.noLight = true;
                        dust.noLightEmittence = true;
                    }
                }
                else
                {
                    if (droneBuddyState != 2)
                        droneBuddyState = 0;
                    if (droneBuddyHealTime >= 600)
                    {
                        droneBuddyState = 2;
                    }
                    if (droneBuddyState == 2)
                    {
                        if (droneBuddyHealTime % 15 == 0)
                        {
                            int healAmt = 2 * droneBuddy;
                            ScaleableHeal(healAmt);
                        }
                        droneBuddyHealTime -= 2;
                        if (droneBuddyHealTime <= 0)
                            droneBuddyState = 0;
                    }
                }
                Vector2 desiredPos = droneBuddyState != 1 ? Player.Center - new Vector2(48 * Player.direction, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 6f) * 8f) : Player.Center + ((Main.npc[droneTarget].Center - Player.Center).SafeNormalize(Vector2.UnitY) * 48);
                SpriteEffects spriteEffects = droneBuddyState != 1 ? (droneBuddyVisualPosition.X > Player.Center.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) : (Main.npc[droneTarget].Center.X >= droneBuddyVisualPosition.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                float idleRotation = MathHelper.Clamp(droneBuddyVisualPosition.Distance(desiredPos) / 500f, -MathHelper.PiOver4, MathHelper.PiOver4) * (spriteEffects == SpriteEffects.FlipHorizontally ? -1 : 1);
                float desiredRot = droneBuddyState != 1 ? idleRotation + (spriteEffects == SpriteEffects.FlipHorizontally ? MathHelper.Pi : 0) : (Main.npc[droneTarget].Center - droneBuddyVisualPosition).ToRotation();

                if (Math.Abs(droneBuddyRotation - desiredRot) > MathHelper.PiOver2 * 3f)
                {
                    if (droneBuddyRotation < desiredRot)
                        droneBuddyRotation += MathHelper.TwoPi;
                    else if (desiredRot < droneBuddyRotation)
                        desiredRot += MathHelper.TwoPi;
                }
                if (droneBuddyVisualPosition == Vector2.Zero)
                {
                    droneBuddyVisualPosition = desiredPos;
                    droneBuddyRotation = 0f;
                }
                else
                {
                    droneBuddyVisualPosition = Vector2.Lerp(droneBuddyVisualPosition, desiredPos, 0.03f);
                    droneBuddyRotation = MathHelper.Lerp(droneBuddyRotation, desiredRot, 0.08f);

                }
                droneSeenRot = spriteEffects == SpriteEffects.FlipHorizontally ? droneBuddyRotation - MathHelper.Pi : droneBuddyRotation;
            }
            else
            {
                droneBuddyHealTime = 0;
            }

            if (soulOfLenaUses < soulOfLena || soulOfLenaHurtVisual)
            {
                Vector2 desiredPos = Player.Top - new Vector2(32 * Player.direction, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 6f) * 8f + 12f);
                if (lenaVisualPosition == Vector2.Zero)
                {
                    lenaVisualPosition = desiredPos;
                }
                else
                {
                    lenaVisualPosition = Vector2.Lerp(lenaVisualPosition, desiredPos, 0.02f);
                }
            }

            if (overclocker > 0)
            {
                if (overclockerTime > 0 && timeAttacking > 0)
                {
                    float attackSpeedIncrease = (float)Math.Pow(overclockerTime / 180f, 2f);
                    Player.GetAttackSpeed(DamageClass.Generic) += attackSpeedIncrease;
                    overclockerTime -= 3;
                }
            }
            else
                overclockerTime = 0;

            if (symbioticFungus > 0)
            {
                if (symbioticFungusHealCooldown > 0)
                    symbioticFungusHealCooldown--;
                if (standingStillTime >= 60 && symbioticFungusHealCooldown <= 0)
                {
                    int healAmt = (int)(Player.statLifeMax2 * (0.04f + (0.04f * symbioticFungus)));
                    ScaleableHeal(healAmt);
                    symbioticFungusHealCooldown += 60;
                }
            }
            else
            {
                symbioticFungusHealCooldown = 60;
                visualFungi.Clear();
            }

            if (giantDoorShield > 0)
            {
                float barrierDiminishingDRIncrease = 100f * giantDoorShield;
                barrierDiminishingDR += barrierDiminishingDRIncrease;
            }

            if (allSeeingEye > 0)
            {
                if (allSeeingEyeTarget != -1)
                {
                    Rectangle wantedScreenRect = new Rectangle((int)Main.Camera.ScaledPosition.X, (int)Main.Camera.ScaledPosition.Y, (int)Main.Camera.ScaledSize.X, (int)Main.Camera.ScaledSize.Y);
                    wantedScreenRect.Inflate(200, 200);

                    NPC npc = Main.npc[allSeeingEyeTarget];
                    if (npc.life <= 0 || !npc.CanBeChasedBy() || !wantedScreenRect.Contains(AimWorld().ToPoint()))
                        allSeeingEyeTarget = -1;
                }

                if (allSeeingEyeHitCooldown <= 0)
                {
                    Rectangle wantedScreenRect = new Rectangle((int)Main.Camera.ScaledPosition.X, (int)Main.Camera.ScaledPosition.Y, (int)Main.Camera.ScaledSize.X, (int)Main.Camera.ScaledSize.Y);
                    wantedScreenRect.Inflate(200, 200);
                    Point checkPoint = AimWorld().ToPoint();
                    allSeeingEyeTarget = -1;
                    if (wantedScreenRect.Contains(checkPoint))
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.life <= 0 || !npc.CanBeChasedBy())
                                continue;

                            var modNPC = npc.ModNPC();
                            if (modNPC.Segments.Count > 0 ? npc.ModNPC().IsPointInsideSegment(npc, checkPoint) :
                                (modNPC.specialAllSeeingEyeHoverBox != null ? ((Rectangle)modNPC.specialAllSeeingEyeHoverBox).Contains(checkPoint) : npc.getRect().Contains(checkPoint)))
                            {
                                allSeeingEyeTarget = i;
                                break;
                            }
                        }
                    }
                }

                if (allSeeingEyeTarget != -1 && allSeeingEyeHitCooldown <= 0)
                {
                    NPC target = Main.npc[allSeeingEyeTarget];
                    int hitDamage = (int)(Player.statLifeMax2 * 0.02f * allSeeingEye);
                    hitDamage = (int)(hitDamage * GetBonusDamageMulti(target, target.ModNPC().ClosestPosition(target.getRect().ClosestPointInRect(Player.Center), Player.Center, target)));

                    NPC.HitInfo info = new NPC.HitInfo();
                    info.HideCombatText = true;
                    int actualHitDamage = hitDamage * 10;
                    info.Damage = actualHitDamage;
                    if (target.life - actualHitDamage <= 0)
                    {
                        OnKillEffects(target);
                    }
                    info.InstantKill = false;
                    info.HitDirection = Main.rand.NextBool() ? -1 : 1;
                    info.Knockback = 0f;
                    info.Crit = false;

                    target.StrikeNPC(info);
                    NetMessage.SendStrikeNPC(target, info);
                    CombatText.NewText(target.getRect(), Color.DarkGreen, actualHitDamage);

                    allSeeingEyeHitCooldown += 30;
                    ScaleableHeal(hitDamage);
                }

                if (allSeeingEyeHitCooldown > 0)
                    allSeeingEyeHitCooldown--;
            }
            else
            {
                allSeeingEyeTarget = -1;
                allSeeingEyeHitCooldown = 30;
            }

            if (portableGenerator > 0)
            {
                if (portableGeneratorImmuneTime > 0)
                {
                    portableGeneratorImmuneTime--;
                    if (portableGeneratorImmuneTime == 0)
                    {
                        Player.SetImmuneTimeForAllTypes(60);
                        Player.immuneNoBlink = false;
                        Player.immune = true;
                        SoundEngine.PlaySound(SoundID.NPCHit43 with { Volume = 0.3f });
                    }
                    else
                    {
                        Player.SetImmuneTimeForAllTypes(2);
                        Player.immuneNoBlink = true;
                        Player.immune = true;
                    }
                }
            }
            else
                portableGeneratorImmuneTime = 0;

            oldCeremonialCrownStacks = ceremonialCrownStacks;
            ceremonialCrownStacks = 0;
            if (ceremonialCrown > 0)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.life <= 0 || npc.immortal || npc.dontTakeDamage || npc.friendly)
                        continue;
                    ceremonialCrownStacks++;
                }
            }
            if (oldCeremonialCrownStacks != ceremonialCrownStacks)
            {
                int fullCycle = 12;
                float baseRot = Main.GlobalTimeWrappedHourly * MathHelper.Pi;
                float baseOutDist = 40;
                if (oldCeremonialCrownStacks > ceremonialCrownStacks)
                {
                    for (int i = ceremonialCrownStacks; i < oldCeremonialCrownStacks; i++)
                    {
                        int cycleCount = i / fullCycle;
                        int dir = cycleCount % 2 == 0 ? 1 : -1;
                        float outerRot = cycleCount * MathHelper.PiOver4;
                        float extraRot = i / (float)fullCycle * MathHelper.TwoPi;
                        float outDist = baseOutDist + 15 * cycleCount;
                        float thisRot = extraRot + outerRot + baseRot * (1 - ((cycleCount + 1f) / (cycleCount + 2f))) * dir;
                        Vector2 drawPos = Player.Center + Vector2.UnitY * Player.gfxOffY + thisRot.ToRotationVector2() * outDist;
                        Color particleColor = i % 3 == 0 ? Color.Red : (i % 3 == 1 ? Color.LimeGreen : Color.Lerp(Color.Blue, Color.Cyan, 0.4f));
                        for (int p = 0; p < 5; p++)
                        {
                            ParticleManager.AddParticle(new Square(drawPos, (p / 6f * MathHelper.TwoPi + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(0.2f, 1f) + Player.velocity, 40, particleColor, new Vector2(1), Main.rand.NextFloat(MathHelper.TwoPi), 0.96f, 30, false));
                        }
                    }
                }
                else
                {
                    for (int i = oldCeremonialCrownStacks; i < ceremonialCrownStacks; i++)
                    {
                        int cycleCount = i / fullCycle;
                        int dir = cycleCount % 2 == 0 ? 1 : -1;
                        float outerRot = cycleCount * MathHelper.PiOver4;
                        float extraRot = i / (float)fullCycle * MathHelper.TwoPi;
                        float outDist = baseOutDist + 15 * cycleCount;
                        float thisRot = extraRot + outerRot + baseRot * (1 - ((cycleCount + 1f) / (cycleCount + 2f))) * dir;
                        Vector2 drawPos = Player.Center + Vector2.UnitY * Player.gfxOffY + thisRot.ToRotationVector2() * outDist;
                        Color particleColor = i % 3 == 0 ? Color.Red : (i % 3 == 1 ? Color.LimeGreen : Color.Lerp(Color.Blue, Color.Cyan, 0.4f));
                        for (int p = 0; p < 5; p++)
                        {
                            ParticleManager.AddParticle(new Square(drawPos, (p / 6f * MathHelper.TwoPi + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(0.2f, 1f) + Player.velocity, 40, particleColor, new Vector2(1), Main.rand.NextFloat(MathHelper.TwoPi), 0.96f, 40, false));
                        }
                    }
                }
            }

            if (thermitePowderCooldown > 0)
                thermitePowderCooldown--;
            if (thermitePowder > 0)
            {
                if (thermitePowderCooldown <= 0)
                {
                    int igniteDamage = 70 * thermitePowder;
                    int burnRate = 7;
                    bool procced = false;
                    int burnCap = igniteDamage / burnRate;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.life <= 0 || npc.immortal || npc.dontTakeDamage || npc.friendly)
                            continue;
                        var modNPC = npc.ModNPC();
                        if (modNPC == null)
                            continue;
                        procced = true;
                        modNPC.ignitedStacks.Add(new IgnitedStack(igniteDamage, Player.whoAmI, burnCap));
                    }
                    if (procced)
                    {
                        SoundEngine.PlaySound(SoundID.Item45 with { Volume = 0.9f, Pitch = 0.3f, PitchVariance = 0.25f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest }, Player.Center);
                        for (int i = 0; i < 32; i++)
                        {
                            float completion = i / 32f;
                            float rot = MathHelper.TwoPi * completion + Main.rand.NextFloat(-0.04f, 0.04f);
                            Color color = Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat(0.5f));
                            ParticleManager.AddParticle(new ThinSpark(
                                Player.Center + rot.ToRotationVector2() * 20, rot.ToRotationVector2() * 5 * Main.rand.NextFloat(0.3f, 1f),
                                34, color * 0.9f, new Vector2(0.13f, 0.27f) * Main.rand.NextFloat(0.7f, 1f) * 0.6f, rot, true, false));
                            if (i % 3 == 0)
                            {
                                ParticleManager.AddParticle(new Snow(Player.Center, rot.ToRotationVector2() * 5 * Main.rand.NextFloat(0.3f, 1f), 60, color, new Vector2(0.04f)));
                            }
                        }
                        thermitePowderCooldown = 60;
                    }
                }
            }

            if (heartyHoneycomb > 0)
            {
                int honeyStrength = (int)((1 - Player.statLife / (float)Player.statLifeMax2) / 0.05f);
                int regenIncrease = honeyStrength * (1 + heartyHoneycomb);
                Player.lifeRegen += regenIncrease;
                if (honeyStrength > 0 && (int)(Main.GlobalTimeWrappedHourly * 60) % (21 - honeyStrength) == 0)
                {
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        Vector2 offset = Main.rand.NextVector2CircularEdge(12, 24);
                        ParticleManager.AddParticle(new Ball(Player.Center + Vector2.UnitY * Player.gfxOffY + offset, offset.SafeNormalize(Vector2.UnitY) * 1 + Vector2.UnitY * 0.25f,
                            16, Color.Lerp(Color.Goldenrod, Color.Yellow, Main.rand.NextFloat()), new Vector2(Main.rand.NextFloat(0.14f, 0.2f)), 0, 0.98f, 10));
                    }
                    
                }
            }

            if (primevalRattle > 0)
            {
                if (primevalRattleStacks.Count > 0)
                {
                    for (int i = 0; i < primevalRattleStacks.Count; i++)
                    {
                        primevalRattleStacks[i]--;
                    }

                    primevalRattleStacks.RemoveAll(time => time <= 0);

                    int cap = 2 + 3 * primevalRattle;
                    int count = Math.Min(primevalRattleStacks.Count, cap);
                    if (count > 0)
                    {
                        float moveSpeedIncrease = 0.08f * count;
                        float drIncrease = 10 * count;
                        Player.moveSpeed += moveSpeedIncrease;
                        diminishingDR += drIncrease;
                    }
                    if (count == cap)
                    {
                        if (!primevalRattleBoost && primevalStreaks.Count == 0)
                        {
                            ExtraSoundSystem.ExtraSounds.Add(new(SoundEngine.PlaySound(PrimevalBoost with { Volume = 0.3f }, Player.Center), 1, -1, 1, true));
                        }
                        primevalRattleBoost = true;
                    }
                    else
                    {
                        primevalRattleBoost = false;
                    }
                }
            }
            else
            {
                if (primevalRattleStacks.Count > 0)
                    primevalRattleStacks.Clear();
                primevalRattleBoost = false;
            }
        }
        public override void PostUpdateEquips()
        {
            if (spentShell > 0)
            {
                float finalAttackSpeedMultiplier = 1f;
                for (int i = 0; i < spentShell; i++)
                {
                    finalAttackSpeedMultiplier *= 1 - (1f / (1.5f + (float)spentShell));
                }
                Player.GetAttackSpeed(DamageClass.Generic) *= finalAttackSpeedMultiplier;
            }
            if (shotgunComponent > 0)
            {
                shotsToFire *= 4 * shotgunComponent;
                float finalAttackSpeedMultiplier = 1f;
                for (int i = 0; i < shotgunComponent; i++)
                {
                    finalAttackSpeedMultiplier *= 1 - (1f / (1f + shotgunComponent));
                }
                Player.GetAttackSpeed(DamageClass.Generic) *= finalAttackSpeedMultiplier;
            }
            if (sniperComponent > 0)
            {
                float finalAttackSpeedMultiplier = 1f;
                float finalDamageMultiplier = 6f * sniperComponent;
                for (int i = 0; i < sniperComponent; i++)
                {
                    finalAttackSpeedMultiplier *= 1 - (2f / (2.5f + sniperComponent));
                }
                Player.GetAttackSpeed(DamageClass.Generic) *= finalAttackSpeedMultiplier;
                Player.GetDamage(DamageClass.Generic) *= finalDamageMultiplier;
            }
            if (minigunComponent > 0)
            {
                float finalAttackSpeedMultiplier = 1.5f + (2 * minigunComponent);
                float finalDamageMultiplier = 0.4f;
                Player.GetAttackSpeed(DamageClass.Generic) *= finalAttackSpeedMultiplier;
                Player.GetDamage(DamageClass.Generic) *= finalDamageMultiplier;
            }
            if (cornucopia > 0)
            {
                Player.lifeRegen = (int)((1f + healMultiplier) * Player.lifeRegen);
            }

            if (!Player.GetJumpState(ExtraJump.CloudInABottle).Available && timesDoubleJumped < extraDoubleJumps)
            {
                Player.GetJumpState(ExtraJump.CloudInABottle).Available = true;
                timesDoubleJumped++;
            }
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                Player.moveSpeed *= 1.30f;
                Player.maxRunSpeed *= 1.30f;
            }

            Player.jumpSpeedBoost += 5f * jumpSpeedMultiplier;

            if (barrierHealth > 0)
            {
                if (barrierHealth > Player.statLifeMax2)
                    barrierHealth = Player.statLifeMax2;
            }
            if (barrierHealth < 0)
                barrierHealth = 0;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Player.pulley && lockDirection)
            {
                Player.pulleyDir = 2;
                if (Player.direction == 1)
                {
                    if (Player.controlLeft && !(Player.controlUp || Player.controlDown))
                        Player.pulley = false;
                    else
                        Player.controlLeft = false;
                }
                else
                {
                    if (Player.controlRight && !(Player.controlUp || Player.controlDown))
                        Player.pulley = false;
                    else
                        Player.controlRight = false;
                }
                    
            }
        }
        public override void SetControls()
        {
            if (noThrow)
            {
                Player.controlThrow = false;
                Player.noThrow = 15;
                noThrow = false;
            }
            if (CutsceneSystem.cutsceneDisableControl)
            {
                Player.controlDown = false;
                Player.controlUp = false;
                Player.controlRight = false;
                Player.controlLeft = false;
                Player.controlMount = false;
                Player.controlHook = false;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.controlJump = false;
                Player.controlDownHold = false;
                Player.controlThrow = false;
                Player.gravControl = false;
                Player.gravControl2 = false;
            }
        }
        public override void PreUpdateMovement()
        {
            if (deathEffectTimer > 0)
            {
                Player.immuneTime = 360;
                Player.SetImmuneTimeForAllTypes(360);
                Player.immune = true;
                Player.immuneNoBlink = true;
                Player.velocity = Vector2.Zero;
                Player.controlDown = false;
                Player.controlUp = false;
                Player.controlRight = false;
                Player.controlLeft = false;
                Player.controlMount = false;
                Player.controlHook = false;
                Player.controlInv = false;
                Player.controlCreativeMenu = false;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.controlJump = false;
                Player.controlDownHold = false;
                Player.controlThrow = false;
                Player.gravControl = false;
                Player.gravControl2 = false;
                Player.pulley = false;
                Player.pulleyDir = 1;
                Player.itemTime = 1;
                Player.itemAnimation = 1;
            }
            else if (CutsceneSystem.cutsceneDisableControl)
            {
                Player.immuneTime = 60;
                Player.SetImmuneTimeForAllTypes(60);
                Player.immune = true;
                Player.immuneNoBlink = true;
                Player.controlDown = false;
                Player.controlUp = false;
                Player.controlRight = false;
                Player.controlLeft = false;
                Player.controlMount = false;
                Player.controlHook = false;
                Player.controlInv = false;
                Player.controlCreativeMenu = false;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.controlJump = false;
                Player.controlDownHold = false;
                Player.controlThrow = false;
                Player.gravControl = false;
                Player.gravControl2 = false;
            }

            if (Player.pulley && TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                Player.RefreshExtraJumps();
            }

            if (jetLeg > 0)
            {
                if (DashDir != 0 && DashDelay == 0)
                {
                    DashDirCache = DashDir;
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot with { Volume = 1f });
                    for (int i = -1; i < 5; i++)
                    {
                        Player.AddImmuneTime(i, 7);
                    }
                    DashTime += 15;
                    int dashDelayAdd = (int)(600 * 4 / (float)(jetLeg + 3));
                    if (dashDelayAdd < 30)
                        dashDelayAdd = 30;

                    DashDelay += dashDelayAdd;
                }
                if (DashTime > 0)
                {
                    JetLegDashMovement();
                    DashDelay++;
                }
            }

            if (DashTime > -6)
                DashTime--;
            if (DashDelay > 0)
            {
                DashDelay--;
                if (DashDelay == 0)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 randVel = Main.rand.NextVector2Circular(6, 12) + Player.velocity;
                        Dust.NewDustDirect(Player.Center + new Vector2(-8, -16), 16, 32, DustID.Torch, randVel.X, randVel.Y, 0, default, 1.3f);
                    }
                    
                    SoundEngine.PlaySound(JetLegCooldown with { Volume = 0.9f });
                }
            }

            if (DashTime <= -6)
            {
                DashDirCache = 0;
            }

            if (lockDirection && Player.pulley && !oldPulley)
            {
                if (Player.direction != startDirection)
                {
                    Player.direction = startDirection;
                }

            }

            oldPulley = Player.pulley;
        }
        public override void PreUpdateBuffs()
        {
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                Player.accWatch = 0;
                if (!enableCampfire)
                {
                    for (int i = 0; i < Player.buffType.Length; i++)
                    {
                        if (Player.buffType[i] == BuffID.Campfire)
                        {
                            Player.buffType[i] = 0;
                            Player.lifeRegen -= 1;
                        }
                    }
                }
            }
            enableCampfire = false;
        }
        public override void PostUpdate()
        {
            moonLordVisualEffect = false;
            moonLordSkyEffect = false;
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {    
                Player.accWatch = 0;
                for (int i = 0; i < Player.buffType.Length; i++)
                {
                    if (Player.buffType[i] == BuffID.MonsterBanner)
                        Player.buffType[i] = 0;
                }
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (Player.pulley && TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                bool upOrDown = Player.controlUp || Player.controlDown;
                if (Player.pulleyDir == 2)
                {
                    if (Player.direction == -1 && Player.controlLeft && !upOrDown)
                    {
                        Player.pulley = false;
                        if (Player.velocity.X == 0)
                        {
                            Player.velocity.X = -1;
                        }
                    }
                    else if (Player.direction == 1 && Player.controlRight && !upOrDown)
                    {
                        Player.pulley = false;
                        if (Player.velocity.X == 0)
                        {
                            Player.velocity.X = 1;
                        }
                    }
                }
                if (Player.pulley)
                {
                    if (Player.ItemAnimationActive && (Player.controlRight || Player.controlLeft) && !upOrDown)
                    {
                        Player.pulley = false;
                        if (Player.velocity.X == 0)
                        {
                            Player.velocity.X = Player.controlRight ? 1 : -1;
                        }
                    }
                }
            }
        }
        #endregion

        #region On Hit Enemy
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            TerRoguelikeGlobalProjectile modProj = proj.ModProj();
            TerRoguelikeGlobalNPC modNPC = target.ModNPC();

            if (previousBonusDamageMulti != 1f)
                hit.Damage = (int)(hit.Damage / previousBonusDamageMulti);

            if (target.life <= 0)
                OnKillEffects(target);

            if (clingyGrenade > 0 && !modProj.procChainBools.clinglyGrenadePreviously)
            {
                float chance;
                chance = clingyGrenade * 0.05f;
                if (ChanceRollWithLuck(chance, procLuck))
                {
                    float radius;
                    if (target.width < target.height)
                        radius = (float)target.width;
                    else
                        radius = (float)target.height;

                    radius *= 0.4f;

                    var modtarget = target.ModNPC();
                    Vector2 targetPos = modtarget.Segments.Count > 0 ? modtarget.Segments[modtarget.hitSegment].Position : target.Center;
                    Vector2 direction = (proj.Center - targetPos).SafeNormalize(Vector2.UnitY);
                    Vector2 spawnPosition = (direction * radius) + (targetPos);
                    int damage = (int)(hit.Damage * 1.5f / target.ModNPC().effectiveDamageTakenMulti);
                    if (hit.Crit)
                        damage /= 2;

                    int spawnedProjectile = Projectile.NewProjectile(proj.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<StuckClingyGrenade>(), damage, 0f, proj.owner, target.whoAmI);
                    TerRoguelikeGlobalProjectile spawnedModProj = Main.projectile[spawnedProjectile].ModProj();

                    spawnedModProj.procChainBools = new ProcChainBools(modProj.procChainBools);
                    spawnedModProj.procChainBools.originalHit = false;
                    spawnedModProj.procChainBools.clinglyGrenadePreviously = true;
                    if (hit.Crit)
                        spawnedModProj.procChainBools.critPreviously = true;
                }
            }
            if (sanguineOrb > 0)
            {
                float chance = 0.08f + (0.04f * (sanguineOrb - 1));
                if (ChanceRollWithLuck(chance, procLuck))
                {
                    int bleedDamage = 240;
                    modNPC.AddBleedingStackWithRefresh(new BleedingStack(bleedDamage, Player.whoAmI));
                }
            }
            if (burningCharcoal > 0)
            {
                float chance = 0.1f * burningCharcoal;
                if (ChanceRollWithLuck(chance, procLuck))
                {
                    int internalHitDamage = hit.Damage;
                    if (hit.Crit)
                        internalHitDamage /= 2;
                    int igniteDamage = (int)(internalHitDamage * 1.5f / target.ModNPC().effectiveDamageTakenMulti);
                    int burnCap = Math.Min(igniteDamage / 6, 50);
                    modNPC.ignitedStacks.Add(new IgnitedStack(igniteDamage, Player.whoAmI, burnCap));
                    Vector2 targetPos = modNPC.Segments.Count > 0 ? modNPC.Segments[modNPC.hitSegment].Position : target.Center;
                    int particleDir = -1;
                    if (proj.owner >= 0)
                    {
                        particleDir = targetPos.X > Main.player[proj.owner].Center.X ? 1 : -1;
                    }
                    for (int i = -5; i <= 5; i++)
                    {
                        float scale = 1f;
                        if (i % 5 != 0)
                            scale *= 0.6f;
                        float baseRot = particleDir == -1 ? MathHelper.Pi : 0;
                        baseRot += i * MathHelper.Pi * 0.06f;
                        ParticleManager.AddParticle(new Debris(targetPos, baseRot.ToRotationVector2() * 4f * Main.rand.NextFloat(0.5f, 1f) - Vector2.UnitY, 
                            20, Color.Lerp(Color.Black, Color.DarkGray, Main.rand.NextFloat()), new Vector2(0.75f) * scale, Main.rand.Next(3), Main.rand.NextFloat(MathHelper.TwoPi), 
                            Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.2f, 6, Main.rand.Next(10, 15), true));
                    }
                    SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite with { Volume = 0.44f, Pitch = 0.4f, PitchVariance = 0.13f }, targetPos);
                }
            }
            if (remedialTapeworm > 0)
            {
                Vector2 targetPos = modNPC.Segments.Count > 0 ? modNPC.Segments[modNPC.hitSegment].Position : target.Center;
                Vector2 targetVect = targetPos - proj.getRect().ClosestPointInRect(Player.Center);
                Projectile.NewProjectile(Player.GetSource_FromThis(), targetPos, targetVect.SafeNormalize(Vector2.UnitY).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(8f, 10f), ModContent.ProjectileType<RemedialHealingOrb>(), 0, 0);
            }

            if (lockOnMissile > 0 && !modProj.procChainBools.lockOnMissilePreviously)
            {
                float chance = 0.1f;
                if (ChanceRollWithLuck(chance, procLuck))
                {
                    Vector2 spawnPosition = Main.player[proj.owner].Top;
                    Vector2 direction = -Vector2.UnitY;
                    int damage = (int)(hit.Damage * 3f * lockOnMissile / target.ModNPC().effectiveDamageTakenMulti);
                    if (hit.Crit)
                        damage /= 2;

                    int spawnedProjectile = Projectile.NewProjectile(proj.GetSource_FromThis(), spawnPosition, direction * 2.2f, ModContent.ProjectileType<Missile>(), damage, 0f, proj.owner, target.whoAmI);
                    TerRoguelikeGlobalProjectile spawnedModProj = Main.projectile[spawnedProjectile].ModProj();

                    spawnedModProj.procChainBools = new ProcChainBools(modProj.procChainBools);
                    spawnedModProj.procChainBools.originalHit = false;
                    spawnedModProj.procChainBools.lockOnMissilePreviously = true;
                    if (hit.Crit)
                        spawnedModProj.procChainBools.critPreviously = true;
                }
            }
            if (evilEye > 0 && hit.Crit)
            {
                if (evilEyeStacks == null)
                    evilEyeStacks = new List<int>();

                evilEyeStacks.Add(180);
            }
            if (bloodSiphon > 0)
            {
                if (ChanceRollWithLuck(0.25f, procLuck))
                {
                    float multiplier = PlayerBaseDamageMultiplier * previousBonusDamageMulti * proj.ModProj().notedBoostedDamage;
                    int healAmt = (int)(bloodSiphon * multiplier);
                    if (healAmt < 1)
                        healAmt = 1;
                    ScaleableHeal(healAmt);
                }
            }
            if (enchantingEye > 0 && hit.Crit)
            {
                float multiplier = PlayerBaseDamageMultiplier * previousBonusDamageMulti * proj.ModProj().notedBoostedDamage;
                int healAmt = (int)(1 + enchantingEye * 2 * multiplier);
                if (healAmt < 1)
                    healAmt = 1;
                ScaleableHeal(healAmt);
            }
            if (ballAndChain > 0)
            {
                int slowTime = 120 * ballAndChain;
                if (forgottenBioWeapon > 0)
                    slowTime *= forgottenBioWeapon + 1;

                if (modNPC.ballAndChainSlow < slowTime)
                    modNPC.ballAndChainSlow = slowTime;
            }
            if (amberRing > 0 && proj.DamageType == DamageClass.Melee)
            {
                int barrierGainAmt = (int)(2 * amberRing * modProj.notedBoostedDamage);
                AddBarrierHealth(barrierGainAmt);
            }
            if (ancientTwig > 0)
            {
                ancientTwigLastStruckTimer = 300;
            }

            if (proj.type == ModContent.ProjectileType<ThrownBackupDagger>())
            {
                int bleedDamage = 120;
                modNPC.AddBleedingStackWithRefresh(new BleedingStack(bleedDamage, Player.whoAmI));
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {

            modifiers.FinalDamage *= GetBonusDamageMulti(target, proj.getRect().ClosestPointInRect(target.Center), proj);
        }
        public float GetBonusDamageMulti(NPC npc, Vector2 hitPosition, Projectile? projectile = null)
        {
            float bonusDamageMultiplier = 1f;
            if (instigatorsBrace > 0 && (npc.life / (float)npc.lifeMax) >= 0.9f)
            {
                float bonusDamage = 0.75f * instigatorsBrace;
                bonusDamageMultiplier *= 1 + bonusDamage;
            }
            if (brazenNunchucks > 0 && Vector2.Distance(Player.Center, hitPosition) <= 128f)
            {
                float bonusDamage = 0.2f * brazenNunchucks;
                bonusDamageMultiplier *= 1 + bonusDamage;
            }
            if (ceremonialCrownStacks > 0)
            {
                float bonusDamage = 0.1f * ceremonialCrownStacks * ceremonialCrown;
                bonusDamageMultiplier *= 1 + bonusDamage;
            }
            if (primevalRattle > 0 && primevalRattleBoost)
            {
                int cap = 2 + 3 * primevalRattle;
                float bonusDamage = 0.15f * cap;
                bonusDamageMultiplier *= 1 + bonusDamage;
            }
            previousBonusDamageMulti = bonusDamageMultiplier;
            return bonusDamageMultiplier;
        }
        #endregion

        #region On Kill Enemy
        public void OnKillEffects(NPC target)
        {
            TerRoguelikeGlobalNPC modTarget = target.ModNPC();

            if (hotPepper > 0 && !modTarget.activatedHotPepper)
            {
                float radius = 144f + (64f * (hotPepper - 1));
                int igniteDamage = 300 + (75 * (hotPepper - 1));
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == target.whoAmI)
                        continue;

                    NPC npc = Main.npc[i];
                    if (npc == null || !npc.active || npc.friendly || npc.immortal || npc.dontTakeDamage)
                        continue;

                    var modNPC = npc.ModNPC();
                    Vector2 npcPos = modNPC.ClosestPosition(npc.Center, target.Center, npc);
                    if (npcPos.Distance(target.Center) <= radius)
                    {
                        modNPC.ignitedStacks.Add(new IgnitedStack(igniteDamage, Player.whoAmI));
                    }
                }
                modTarget.activatedHotPepper = true;
            }
            if (soulstealCoating > 0 && !modTarget.activatedSoulstealCoating)
            {
                Projectile.NewProjectile(Projectile.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<SoulstealHealingOrb>(), 0, 0f, Player.whoAmI);
                modTarget.activatedSoulstealCoating = true;
            }
            if (amberBead > 0 && !modTarget.activatedAmberBead)
            {
                int barrierGainAmt = amberBead * 15;
                modTarget.activatedAmberBead = true;
                AddBarrierHealth(barrierGainAmt);
            }
            if (thrillOfTheHunt > 0 && !modTarget.activatedThrillOfTheHunt)
            {
                if (thrillOfTheHuntStacks == null)
                    thrillOfTheHuntStacks = new List<int>();

                thrillOfTheHuntStacks.Add(480);
                modTarget.activatedThrillOfTheHunt = true;
            }
            if (clusterBombSatchel > 0 && !modTarget.activatedClusterBombSatchel)
            {
                int damage = 350 + (150 * clusterBombSatchel);
                float scale = 1f + (0.12f * (clusterBombSatchel - 1));
                if (scale > 15f)
                    scale = 15f;
                int spawnedProj = Projectile.NewProjectile(Projectile.GetSource_None(), target.Center, Vector2.Zero, ModContent.ProjectileType<ClusterHandler>(), damage, 0f, Player.whoAmI);
                Main.projectile[spawnedProj].scale = scale;
                modTarget.activatedClusterBombSatchel = true;
            }
            if (steamEngine > 0 && !modTarget.activatedSteamEngine)
            {
                steamEngineStacks.Add(7200);
                modTarget.activatedSteamEngine = true;
            }
            if (disposableTurret > 0 && !modTarget.activatedDisposableTurret)
            {
                float chance = (disposableTurret + 0.5f) / (disposableTurret + 5f);
                if (ChanceRollWithLuck(chance, procLuck))
                {
                    Vector2 wantedPos = FindAirToPlayer(target.Center);
                    Point wantedTilePos = wantedPos.ToTileCoordinates();

                    bool found = false;
                    for (int y = 0; y < 200; y++)
                    {
                        Point belowTilePos = wantedTilePos + new Point(0, y);
                        if (ParanoidTileRetrieval(belowTilePos).IsTileSolidGround())
                        {
                            wantedPos = belowTilePos.ToWorldCoordinates(8, -12);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        wantedPos = target.Center;
                    int spawnedProj = Projectile.NewProjectile(Player.GetSource_FromThis(), wantedPos, Vector2.Zero, ModContent.ProjectileType<DisposableTurretMinion>(), 100, 0);
                    Main.projectile[spawnedProj].localAI[0] = wantedPos.X < Player.Center.X ? -1 : 1;
                }

                modTarget.activatedDisposableTurret = true;
            }
            if (nutritiousSlime > 0 && !modTarget.activatedNutritiousSlime)
            {
                int slimeCount = 4;
                for (int i = 0; i < slimeCount; i++)
                {
                    Vector2 velocity = (-Vector2.UnitY * 3f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 3f, (MathHelper.Pi / 3f) + float.Epsilon));
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<SlimeGlob>(), 0, 0f, Player.whoAmI);
                }
                modTarget.activatedNutritiousSlime = true;
            }
            if (itemPotentiometer > 0 && !modTarget.activatedItemPotentiometer)
            {
                float chance = 0.02f * itemPotentiometer;
                if (ChanceRollWithLuck(chance, procLuck))
                {
                    SpawnRoguelikeItem(target.Center);
                }
                modTarget.activatedItemPotentiometer = true;
            }
        }
        #endregion

        #region Player Hurt
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (dodgeAttack)
            {
                dodgeAttack = false;
                return true;
            }

            if (barrierHealth > 1 && info.Damage <= (int)barrierHealth && !barrierFullAbsorbHit)
            {
                BarrierHitEffect(info.Damage, info.Damage);
                return true;
            }

            return false;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (timesHaveBeenTougher > 0)
            {
                float chance = (0.17f * timesHaveBeenTougher) / ((0.17f * timesHaveBeenTougher + 1) * 1.8f);

                if (ChanceRollWithLuck(chance, procLuck))
                {
                    string blocked = Language.GetOrRegister("Mods.TerRoguelike.BlockedAlert").Value;
                    SoundEngine.PlaySound(new SoundStyle("TerRoguelike/Sounds/Squeak", 3) with { Volume = 0.1f }, Player.Center);
                    CombatText.NewText(Player.getRect(), Color.LightGray, blocked);
                    int addImmuneTime = 40;
                    if (BloodMoonActive)
                    {
                        addImmuneTime = (int)(addImmuneTime * BloodMoonIframeMultiplier);
                    }
                    else if (NewMoonActive)
                    {
                        addImmuneTime = (int)(addImmuneTime * NewMoonIframeMultiplier);
                    }
                    for (int i = -1; i < 5; i++)
                    {
                        Player.AddImmuneTime(i, addImmuneTime);
                    }
                    Player.immune = true;
                    dodgeAttack = true;
                    return;
                }
            }

            modifiers.ModifyHurtInfo += ModifyHurtInfo_TerRoguelike;
        }
        private void ModifyHurtInfo_TerRoguelike(ref Player.HurtInfo info)
        {
            if (flimsyPauldron > 0)
            {
                int reductedDamage = 3 + (flimsyPauldron - 1) * 2;
                info.Damage -= reductedDamage;
            }

            float damageMultiplierFromDR = 1;
            if (diminishingDR != 0f)
            {
                if (diminishingDR > 0)
                    damageMultiplierFromDR *= (100f / (100f + diminishingDR));
                else
                    damageMultiplierFromDR *= 2 - (100f / (100f - diminishingDR));
            }
            int newDamage = (int)(info.Damage * damageMultiplierFromDR);
            int damageDifference = info.Damage - newDamage;
            info.Damage = newDamage;

            if (everlastingJellyfish > 0 && damageDifference > barrierHealth)
            {
                int damageDifferenceAfterBarrierReduction = damageDifference - (int)barrierHealth;
                if (damageDifferenceAfterBarrierReduction > 0)
                {
                    float targetedHealingPercentage = ((everlastingJellyfish + 1f) / (everlastingJellyfish + 3f));
                    int finalHealingAmount = (int)(damageDifferenceAfterBarrierReduction * targetedHealingPercentage);
                    if (finalHealingAmount < 1)
                        finalHealingAmount = 1;
                    ScaleableHeal(finalHealingAmount);
                }
            }
            if (barrierHealth >= 1 && info.Damage > (int)barrierHealth)
            {
                int preBarrierDamage = info.Damage;
                info.Damage -= (int)barrierHealth;
                barrierFullAbsorbHit = true;
                BarrierHitEffect((int)barrierHealth, preBarrierDamage);
            }
        }
        public void BarrierHitEffect(int damageToBarrier, int fullHitDamage)
        {
            if (barrierDiminishingDR != 0)
            {
                int damageChange = 0;
                if (barrierDiminishingDR > 0)
                    damageChange = (int)(damageToBarrier * ((100f / (100f + diminishingDR + barrierDiminishingDR)) - (100f / (100f + diminishingDR))));
                else
                    damageChange = (int)(damageToBarrier * (2 - ((100f / (100f + diminishingDR + barrierDiminishingDR)) - (100f / (100f + diminishingDR))))) - damageToBarrier;

                damageToBarrier += damageChange;
                fullHitDamage += damageChange;
            }
            if (damageToBarrier < 1)
                damageToBarrier = 1;
            barrierInHurt = damageToBarrier;
            CombatText.NewText(Player.getRect(), Color.Gold, damageToBarrier > (int)barrierHealth ? -(int)barrierHealth : -damageToBarrier);
            SoundStyle soundStyle = damageToBarrier < (int)barrierHealth ? SoundID.NPCHit53 with { Volume = 0.5f } : SoundID.NPCDeath56 with { Volume = 0.3f };
            SoundEngine.PlaySound(soundStyle, Player.Center);
            barrierHealth -= damageToBarrier;
            int addImmuneTime = fullHitDamage == 1 ? 20 : 40;
            if (BloodMoonActive)
            {
                addImmuneTime = (int)(addImmuneTime * BloodMoonIframeMultiplier);
            }
            else if (NewMoonActive)
            {
                addImmuneTime = (int)(addImmuneTime * NewMoonIframeMultiplier);
            }
            for (int i = -1; i < 5; i++)
            {
                Player.AddImmuneTime(i, addImmuneTime);
            }
            Player.immune = true;
            HurtEffects(fullHitDamage);
            
        }
        public void HurtEffects(int damage)
        {
            if (protectiveBubble > 0 && outOfDangerTime >= 420)
            {
                SoundEngine.PlaySound(SoundID.Item87 with { Volume = 1f }, Player.Center);
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDust(Player.MountedCenter + new Vector2(-16, -16), 32, 32, DustID.BlueTorch);
                }
            }
            if (retaliatoryFist > 0)
            {
                bool playSound = false;
                float requiredDistance = 160f + (32f * retaliatoryFist);
                int projSpawnCount = 0;
                int projMaxSpawnCount = 1 + (retaliatoryFist * 2);
                int fistDamage = 125 + (retaliatoryFist * 125);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (projSpawnCount >= projMaxSpawnCount)
                        break;

                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.life <= 0 || npc.friendly || npc.immortal || npc.dontTakeDamage)
                        continue;

                    Vector2 npcPos = npc.ModNPC().Segments.Count > 0 ? npc.ModNPC().ClosestSegment(Player.Center) : npc.getRect().ClosestPointInRect(Player.Center);

                    if (Player.Center.Distance(npcPos) <= requiredDistance)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), npc.Center + new Vector2(0, -96), Vector2.Zero, ModContent.ProjectileType<MagicFist>(), fistDamage, 0f, Player.whoAmI, i);
                        playSound = true;
                        projSpawnCount++;
                    }
                }
                if (playSound)
                    SoundEngine.PlaySound(SoundID.Item105 with { Volume = 0.7f }, Player.Center);
            }
            outOfDangerTime = 0;
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (barrierInHurt <= 0)
                HurtEffects(info.Damage);
        }
        public override void PostHurt(Player.HurtInfo info)
        {
            if (BloodMoonActive)
            {
                Player.immuneTime = (int)(Player.immuneTime * BloodMoonIframeMultiplier);
                for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                {
                    Player.hurtCooldowns[i] = (int)(Player.hurtCooldowns[i] * BloodMoonIframeMultiplier);
                }
            }
            if (NewMoonActive)
            {
                Player.immuneTime = (int)(Player.immuneTime * NewMoonIframeMultiplier);
                for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                {
                    Player.hurtCooldowns[i] = (int)(Player.hurtCooldowns[i] * NewMoonIframeMultiplier);
                }
            }
            if (info.Damage == 1 && barrierInHurt > 0)
            {
                Player.immuneTime *= 2;
                for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                {
                    Player.hurtCooldowns[i] *= 2;
                }
            }
            if (soulOfLena > 0)
            {
                if (soulOfLenaUses < soulOfLena && Player.statLife / (float)Player.statLifeMax2 <= 0.25f)
                {
                    for (int i = -1; i < 5; i++)
                    {
                        Player.AddImmuneTime(i, 300);
                    }
                    Player.immuneNoBlink = true;
                    Player.immune = true;
                    soulOfLenaUses++;
                    soulOfLenaHurtVisual = true;
                    SoundEngine.PlaySound(SoundID.NPCHit36 with { Volume = 0.4f }, Player.Center);
                }
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (trumpCard > 0 && !escapeFail)
            {
                for (int inventoryItem = 0; inventoryItem < 50; inventoryItem++)
                {
                    Item item = Player.inventory[inventoryItem];
                    if (item.type == ModContent.ItemType<TrumpCard>())
                    {
                        item.stack--;

                        Player.statLife = Player.statLifeMax2;
                        barrierHealth = Player.statLifeMax2;
                        for (int i = -1; i < 5; i++)
                        {
                            Player.AddImmuneTime(i, 360);
                        }
                        Player.immune = true;
                        Player.immuneNoBlink = true;
                        deathEffectTimer += 120;
                        reviveDeathEffect = true;
                        SoundEngine.PlaySound(SoundID.PlayerKilled);
                        SoundEngine.PlaySound(new SoundStyle("TerRoguelike/Sounds/LossRevive"));
                        ZoomSystem.SetZoomAnimation(2.5f, 60);
                        return false;
                    }
                }
            }
            else
            {
                reviveDeathEffect = false;
            }

            if (!TerRoguelikeWorld.IsTerRoguelikeWorld)
                return true;

            if (damageSource.SourceNPCIndex > -1 && damageSource.SourceNPCIndex < Main.maxNPCs)
            {
                killerNPC = damageSource.SourceNPCIndex;
                killerNPCType = Main.npc[killerNPC].type;
            }
            if (damageSource.SourceProjectileLocalIndex > -1 && damageSource.SourceProjectileLocalIndex < Main.maxProjectiles)
            {
                TerRoguelikeGlobalProjectile modProj = Main.projectile[damageSource.SourceProjectileLocalIndex].ModProj();
                if (modProj.npcOwner > -1)
                {
                    killerNPC = modProj.npcOwner;
                    killerNPCType = modProj.npcOwnerType;
                }
                else
                {
                    killerProj = damageSource.SourceProjectileLocalIndex;
                    killerProjType = Main.projectile[killerProj].type;
                }
            }
            if (killerNPC == -1 && escapeFail)
            {
                killerNPC = 0;
                killerNPCType = ModContent.NPCType<TrueBrain>();
            }
            SoundEngine.PlaySound(new SoundStyle("TerRoguelike/Sounds/Loss"));
            ZoomSystem.SetZoomAnimation(2.5f, 60);
            deathEffectTimer += 120;
            return true;
        }
        #endregion

        #region Mechanical Functions
        public void ScaleableHeal(int healAmt)
        {
            healAmt = (int)(healAmt * healMultiplier);
            if (barrierSynthesizer > 0)
            {
                if (Player.statLife + healAmt > Player.statLifeMax2)
                {
                    int barrierHealAmt = (int)((Player.statLife + healAmt - Player.statLifeMax2) * barrierSynthesizer * 0.5f);
                    if (barrierHealAmt > 0)
                        AddBarrierHealth(barrierHealAmt);
                }
            }
            Player.Heal(healAmt);

            if (primevalRattle > 0 && healAmt >= 10)
                PrimevalRattleAdd();
        }
        public void AddBarrierHealth(int barrierGainAmt)
        {
            barrierHealth += barrierGainAmt;
            if (barrierHealth > Player.statLifeMax2)
                barrierHealth = Player.statLifeMax2;

            CombatText.NewText(Player.getRect(), Color.LightGoldenrodYellow, barrierGainAmt);

            if (primevalRattle > 0 && barrierGainAmt >= 10)
                PrimevalRattleAdd();
        }
        public void PrimevalRattleAdd()
        {
            primevalRattleStacks ??= [];

            primevalRattleStacks.Add(540);
            int potentialCap = 2 + 3 * primevalRattle;
            if (primevalRattleStacks.Count > potentialCap + 10)
            {
                int least = 100000;
                for (int i = 0; i < primevalRattleStacks.Count; i++)
                {
                    int check = primevalRattleStacks[i];
                    if (check < least)
                        least = check;
                }
                primevalRattleStacks.Remove(least);
            }
            SoundEngine.PlaySound(PrimevalRattleProc with { Volume = 0.24f, Pitch = -0.2f, PitchVariance = 0.13f, Variants = [2] }, Player.Center);
        }
        public void GiftBoxLogic(Vector2 position)
        {
            float commonWeight = 0.8f;
            float uncommonWeight = 0.18f * (float)giftBox;
            float rareWeight = 0.02f * (float)Math.Pow(giftBox, 2);
            float chance = Main.rand.NextFloat(commonWeight + uncommonWeight + rareWeight + float.Epsilon);
            int itemType;
            int itemTier;
            if (chance <= commonWeight)
            {
                itemType = ItemManager.GiveCommon(false);
                itemTier = 0;
            }
            else if (chance <= commonWeight + uncommonWeight)
            {
                itemType = ItemManager.GiveUncommon(false);
                itemTier = 1;
            }
            else
            {
                itemType = ItemManager.GiveRare(false);
                itemTier = 2;
            }

            SpawnManager.SpawnItem(itemType, position, itemTier, 105, 0.5f);
        }
        public void LunarCharmLogic(Vector2 position)
        {
            float commonWeight = 0.8f;
            float uncommonWeight = 0.18f + 0.18f * ((lunarCharm - 1) * 2);
            float rareWeight = 0.02f + 0.02f * (float)Math.Pow((lunarCharm - 1) * 2, 2);
            float chance = Main.rand.NextFloat(commonWeight + uncommonWeight + rareWeight + float.Epsilon);
            int itemType;
            int itemTier;
            if (chance <= commonWeight)
            {
                itemType = ItemManager.GiveCommon(false);
                itemTier = 0;
            }
            else if (chance <= commonWeight + uncommonWeight)
            {
                itemType = ItemManager.GiveUncommon(false);
                itemTier = 1;
            }
            else
            {
                itemType = ItemManager.GiveRare(false);
                itemTier = 2;
            }

            SpawnManager.SpawnItem(itemType, position, itemTier, 105, 0.5f);
        }
        public void SpawnRoguelikeItem(Vector2 position)
        {
            int chance = Main.rand.Next(1, 101);
            int itemType;
            int itemTier;

            if (chance <= 80)
            {
                itemType = ItemManager.GiveCommon(false);
                itemTier = 0;
            }
            else if (chance <= 98)
            {
                itemType = ItemManager.GiveUncommon(false);
                itemTier = 1;
            }
            else
            {
                itemType = ItemManager.GiveRare(false);
                itemTier = 2;
            }

            position = FindAirToPlayer(position);

            SpawnManager.SpawnItem(itemType, position, itemTier, 75, 0.5f);
        }
        public Vector2 FindAirToPlayer(Vector2 position)
        {
            Point tilePos = position.ToTileCoordinates();
            Tile tile = ParanoidTileRetrieval(tilePos.X, tilePos.Y);
            if (tile != null)
            {
                if (!tile.IsTileSolidGround(true))
                {
                    return position;
                }
            }

            float distanceReCheckMove = 22.627f;
            Vector2 direction = Player.Center - position;
            if (direction.Length() >= distanceReCheckMove)
            {
                position += direction.SafeNormalize(Vector2.UnitY) * distanceReCheckMove;
                return FindAirToPlayer(position);
            }

            return Player.Center;
        }

        public override void OnExtraJumpRefreshed(ExtraJump jump)
        {
            timesDoubleJumped = 0;
        }
        public override void OnEnterWorld()
        {
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                Main.BlackFadeIn = 255;
                Main.SetCameraLerp(1, 1);
                if (Player.armor[3].type == ItemID.CreativeWings)
                    Player.armor[3] = new Item();
                if (RoomSystem.RoomList != null && ModContent.GetInstance<TerRoguelikeConfig>().LoadEntireWorldUponEnteringWorld)
                {
                    for (int i = 0; i < RoomSystem.RoomList.Count; i++)
                    {
                        Room room = RoomSystem.RoomList[i];
                        Point topLeft = room.RoomPosition.ToPoint();
                        Point dimensions = room.RoomDimensions.ToPoint();
                        Rectangle frameRect = new Rectangle(topLeft.X, topLeft.Y, dimensions.X, dimensions.Y);
                        frameRect.Inflate(2000, 2000);
                        WorldGen.SectionTileFrameWithCheck(frameRect.X, frameRect.Y, frameRect.Width, frameRect.Height);
                    }
                }
            }
            escaped = false;
            DeathUI.itemsToDraw.Clear();
            CreditsUI.itemsToDraw.Clear();
            deadTime = 0;
            creditsViewTime = 0;
            escapeFail = false;
            barrierHealth = 0;
            playthroughTime.Restart();
            currentFloor = null;

        }
        #endregion

        #region Edit Starting Items
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            static Item createItem(int type)
            {
                Item i = new Item();
                i.SetDefaults(type);
                return i;
            }

            IEnumerable<Item> items = new List<Item>()
            {
                createItem(ItemManager.StarterRanged[rangedSelection].id),
                createItem(ItemManager.StarterMelee[meleeSelection].id),
            };

            return items;
        }

        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
        {
            itemsByMod["Terraria"].Clear();
        }
        #endregion

        #region Draw Effects
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (deathEffectTimer > 0) // FUCK ALL OF IT
            {
                drawInfo.hideEntirePlayer = true;
                drawInfo.colorArmorBody = Color.Transparent;
                drawInfo.colorArmorHead = Color.Transparent;
                drawInfo.colorArmorLegs = Color.Transparent;
                drawInfo.colorBodySkin = Color.Transparent;
                drawInfo.colorDisplayDollSkin = Color.Transparent;
                drawInfo.colorElectricity = Color.Transparent;
                drawInfo.colorEyes = Color.Transparent;
                drawInfo.colorEyeWhites = Color.Transparent;
                drawInfo.colorHair = Color.Transparent;
                drawInfo.colorHead = Color.Transparent;
                drawInfo.colorLegs = Color.Transparent;
                drawInfo.colorMount = Color.Transparent;
                drawInfo.colorPants = Color.Transparent;
                drawInfo.colorShirt = Color.Transparent;
                drawInfo.colorShoes = Color.Transparent;
                drawInfo.colorUnderShirt = Color.Transparent;
                drawInfo.armGlowColor = Color.Transparent;
                drawInfo.bodyGlowColor = Color.Transparent;
                drawInfo.headGlowColor = Color.Transparent;
                drawInfo.floatingTubeColor = Color.Transparent;
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (deathEffectTimer > 0 || Player.dead)
            {
                r = 0f;
                g = 0f;
                b = 0f;
                a = 0f;
                fullBright = false;
                return;
            }

            float closestNPCDistance = -1f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc == null || !npc.active || npc.friendly)
                    continue;

                if (closestNPCDistance == -1f)
                {
                    closestNPCDistance = Vector2.Distance(Player.Center, npc.ModNPC().ClosestPosition(npc.Center, Player.Center, npc));
                }
                else if (Vector2.Distance(Player.Center, npc.Center) < closestNPCDistance)
                {
                    closestNPCDistance = Vector2.Distance(Player.Center, npc.ModNPC().ClosestPosition(npc.Center, Player.Center, npc));
                }
            }
            if (closestNPCDistance == -1)
            {
                closestNPCDistance = 10000f;
            }

            if (bladeFlashTime > 0)
            {
                if (Player.HeldItem.type == ModContent.ItemType<AdaptiveSpear>())
                {
                    drawInfo.heldItem.color = Color.Lerp(Color.White, Color.Magenta, (float)bladeFlashTime / 23f);
                }
                else
                {
                    drawInfo.heldItem.color = Color.Lerp(Color.White, Color.Cyan, (float)bladeFlashTime / 23f);
                }
                
                bladeFlashTime--;
            }

            if (brazenNunchucks > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D telegraphBase = TexDict["InvisibleProj"];

                float scale = 1.64f;
                float opacity = MathHelper.Lerp(0.08f, 0f, (closestNPCDistance - 128f) / 128f);

                GameShaders.Misc["TerRoguelike:CircularGradientWithEdge"].UseOpacity(opacity);
                GameShaders.Misc["TerRoguelike:CircularGradientWithEdge"].UseColor(Color.Lerp(Color.Orange, Color.SandyBrown, 0.5f) * 0.85f);
                GameShaders.Misc["TerRoguelike:CircularGradientWithEdge"].UseSecondaryColor(Color.SandyBrown * 0.75f);
                GameShaders.Misc["TerRoguelike:CircularGradientWithEdge"].UseSaturation(scale);

                GameShaders.Misc["TerRoguelike:CircularGradientWithEdge"].Apply();

                Vector2 drawPosition = Player.Center - Main.screenPosition;
                Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.White, 0, telegraphBase.Size() / 2f, scale * 156f, 0, 0);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            }
            
            if (protectiveBubble > 0 && outOfDangerTime >= 400)
            {
                Main.spriteBatch.End();
                Effect shieldEffect = Filters.Scene["TerRoguelike:ProtectiveBubbleShield"].GetShader().Shader;
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                // If in vanity, the shield is always projected as if it's at full strength.
                float shieldStrength = 1f;

                // Noise scale also grows and shrinks, although out of sync with the shield
                float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                // Define shader parameters
                
                shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                // Shield opacity multiplier slightly changes, this is independent of current shield strength
                float baseShieldOpacity = 0.2f + 0.05f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * shieldStrength));
                shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                Color edgeColor;
                Color shieldColor;

                float opacity = MathHelper.Clamp(MathHelper.Lerp(0, 1f, (float)Math.Log2(Math.Pow((outOfDangerTime - 400) * 0.9f, 2)) + 6f) / 20f, 0f, 1f);
                Color blueTint = new Color(51, 102, 255);
                Color cyanTint = new Color(71, 202, 255);
                Color wulfGreen = new Color(100, 180, 220) * 0.8f;
                edgeColor = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen) * 0.5f * opacity;
                shieldColor = blueTint * 0.125f * opacity;

                // Define shader parameters for shield color
                shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                // Fetch shield noise overlay texture (this is the techy overlay fed to the shader)
                Vector2 pos = Player.MountedCenter + Player.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Texture2D tex = TexDict["OverheadWaves"];

                float scale = 0.115f * opacity;
                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            }

            if (ancientTwigCooldown > 0)
            {
                float completion = (1f - (ancientTwigCooldown / (float)ancientTwigSetRestoreRate)) * 2;
                float logCompletion = (float)Math.Log(completion + 1, 4);

                Main.spriteBatch.End();
                Effect shieldEffect = Filters.Scene["TerRoguelike:AncientTwigEffect"].GetShader().Shader;
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                // Noise scale also grows and shrinks, although out of sync with the shield
                float noiseScale = 1f;

                // Define shader parameters

                shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 1);
                shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                // Shield opacity multiplier slightly changes, this is independent of current shield strength
                float opacity = MathHelper.Clamp(MathHelper.Lerp(1f, -1f, completion), 0f, 1f);
                shieldEffect.Parameters["shieldOpacity"].SetValue(opacity);
                shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                Color edgeColor;
                Color shieldColor;


                edgeColor = Color.Goldenrod * 1f;
                shieldColor = Color.Yellow * 0.8f;

                // Define shader parameters for shield color
                shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector4());
                shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector4());

                // Fetch shield noise overlay texture (this is the techy overlay fed to the shader)
                Vector2 pos = Player.MountedCenter + Player.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Texture2D tex = TexDict["Crust"];

                float scale = MathHelper.Lerp(0, 0.55f, logCompletion);
                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);

                StartVanillaSpritebatch();
            }

            if (overclockerTime > 0)
            {
                r += (1f - r) * (overclockerTime / 360f);
                g -= g * (overclockerTime / 360f) * 0.5f;
                b -= b * (overclockerTime / 360f) * 0.5f;

                if (overclockerTime >= 180)
                {
                    float rate = overclockerTime == 360 ? 0.2f : 0.5f;
                    if (Main.GlobalTimeWrappedHourly % rate <= 0.017f)
                    {
                        int chosenSmoke = Main.rand.Next(3);
                        int goreID = chosenSmoke == 0 ? GoreID.Smoke1 : (chosenSmoke == 1 ? GoreID.Smoke2 : GoreID.Smoke3);
                        Gore smoke = Gore.NewGoreDirect(Player.GetSource_FromThis(), Player.Top + new Vector2(-16, -16), -Vector2.UnitY, goreID, 0.5f);
                        smoke.alpha = 150;
                    }
                }   
            }

            if (soulOfLena > 0)
            {
                bool hurtCheck = Player.immuneTime > 0 && soulOfLenaHurtVisual;
                if (hurtCheck)
                {
                    r = 0.4f;
                    g = 0.8f;
                    b = 1f;
                    a = 0.5f;
                    if (Player.immuneTime <= 1)
                    {
                        soulOfLenaHurtVisual = false;
                    }
                    Lighting.AddLight(Player.Center, 0f, 0.24f, 0.36f);
                }
                if (soulOfLenaUses < soulOfLena || soulOfLenaHurtVisual)
                {
                    Texture2D lenaTex = TexDict["Lena"];
                    Texture2D circleTex = TexDict["LenaGlow"];
                    int lenaFrame = hurtCheck ? 2 : (Main.GlobalTimeWrappedHourly % 1.1f >= 0.55f ? 1 : 0);
                    int frameHeight = lenaTex.Height / 3;
                    SpriteEffects spriteEffects = Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Color color = soulOfLenaHurtVisual && soulOfLenaUses == soulOfLena ? Color.White * MathHelper.Lerp(0, 1f, Player.immuneTime / 300f) : Color.White;

                    Main.EntitySpriteDraw(circleTex, lenaVisualPosition - Main.screenPosition, null, color.MultiplyRGB(Color.Cyan) * 0.2f, 0f, circleTex.Size() * 0.5f, 0.75f, SpriteEffects.None);
                    Main.EntitySpriteDraw(lenaTex, lenaVisualPosition - Main.screenPosition, new Rectangle(0, frameHeight * lenaFrame, lenaTex.Width, frameHeight), color, 0f, new Vector2(lenaTex.Width, frameHeight) * 0.5f, 1f, spriteEffects);
                }
                else
                    lenaVisualPosition = Vector2.Zero;
            }
            else
                lenaVisualPosition = Vector2.Zero;

            if (barbedLasso > 0)
            {
                if (barbedLassoTargets.Count > 0)
                {
                    for (int i = 0; i < barbedLassoTargets.Count; i++)
                    {
                        NPC npc = Main.npc[barbedLassoTargets[i]];

                        Vector2 distanceVector = npc.ModNPC().ClosestPosition(npc.Center, Player.Center, npc) - Player.Center;
                        for (int d = 0; d < 1; d++)
                        {
                            Vector2 dustPosition = distanceVector.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1f, distanceVector.Length());
                            Dust dust = Dust.NewDustPerfect(Player.MountedCenter + dustPosition, DustID.GreenTorch);
                            dust.noGravity = true;
                            dust.noLight = true;
                            dust.noLightEmittence = true;
                        }
                    }
                }
            }

            if (jetLeg > 0)
            {
                if (DashDirCache != 0 && DashTime > -6)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustDirect(Player.Bottom + new Vector2((-8 - (i * Player.velocity.X * 0.5f)) * DashDirCache, -10), 4, 8, DustID.Torch, SpeedX: (-4f - i) * DashDirCache);
                    }
                }
            }

            if (droneBuddy > 0)
            {
                Texture2D droneTex = TexDict["DroneBuddyMinion"];
                float fullAnimTime = Main.GlobalTimeWrappedHourly % 0.5f;
                int droneFrame = fullAnimTime % 0.25f <= 0.125f ? 1 : (fullAnimTime <= 0.25f ? 0 : 2);
                int faceFrame = droneBuddyState == 1 ? 5 : (Main.GlobalTimeWrappedHourly % 9f <= 0.4f ? 4 : 3);
                int frameHeight = droneTex.Height / 6;
                Color color = Color.White;
                SpriteEffects spriteEffects = droneBuddyState != 1 ? (droneBuddyVisualPosition.X > Player.Center.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None) : (Main.npc[droneTarget].Center.X >= droneBuddyVisualPosition.X ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                
                Main.EntitySpriteDraw(droneTex, droneBuddyVisualPosition - Main.screenPosition, new Rectangle(0, frameHeight * droneFrame, droneTex.Width, frameHeight - 1), color, droneSeenRot, new Vector2(droneTex.Width, frameHeight) * 0.5f, 1f, spriteEffects);
                Main.EntitySpriteDraw(droneTex, droneBuddyVisualPosition - Main.screenPosition, new Rectangle(0, frameHeight * faceFrame, droneTex.Width, frameHeight - 1), color, droneSeenRot, new Vector2(droneTex.Width, frameHeight) * 0.5f, 1f, spriteEffects);

                if (droneBuddyState == 2)
                {
                    float opacity = MathHelper.Clamp(MathHelper.Lerp(0f, 1f, (droneBuddyHealTime) / 30f), 0f, 1f);
                    Texture2D squareTex = TexDict["AdaptiveGunBullet"];
                    Vector2 projSpawnPos = droneBuddyVisualPosition + (new Vector2(0.4f * (droneBuddyVisualPosition.X > Player.Center.X ? -1 : 1), 1f).RotatedBy(droneSeenRot) * 11f * new Vector2(1.2f, 1));
                    for (int i = 0; i < 15; i++)
                    {
                        float randFloat = (i / 15f) + (Main.GlobalTimeWrappedHourly % 0.1f);
                        Vector2 pointOnLine = (Player.MountedCenter + new Vector2(0, Player.gfxOffY) - projSpawnPos) * randFloat;
                        pointOnLine.Y += (-1 * (float)Math.Pow(randFloat * 2f - 1, 2) + 1) * 32f;
                        pointOnLine += projSpawnPos;
                        if (pointOnLine.Distance(Player.MountedCenter) < 20f)
                            continue;

                        Main.EntitySpriteDraw(squareTex, pointOnLine - Main.screenPosition, null, Color.LimeGreen * opacity, Main.rand.NextFloatDirection(), squareTex.Size() * 0.5f, 1f, SpriteEffects.None);
                    }
                }
            }
            else
                droneBuddyVisualPosition = Vector2.Zero;

            if (allSeeingEye > 0)
            {
                if (allSeeingEyeTarget != -1)
                {
                    NPC npc = Main.npc[allSeeingEyeTarget];

                    Vector2 distanceVector = AimWorld() - Player.Center;
                    for (int d = 0; d < 2; d++)
                    {
                        Vector2 placement = distanceVector.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1f, distanceVector.Length());
                        Vector2 dustPosition = placement;
                        Dust dust = Dust.NewDustPerfect(Player.MountedCenter + dustPosition, DustID.GreenTorch, placement.Length() * (-placement.SafeNormalize(Vector2.UnitY)) * 0.05f + Player.velocity);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.noLightEmittence = true;
                    }
                }
            }

            if (symbioticFungus > 0)
            {
                DrawVisualFungi();
            }
            if (primevalRattleBoost || primevalStreaks.Count > 0)
            {
                DrawPrimevalVisuals();
            }

            if (brainSucklerTime > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D telegraphBase = TexDict["InvisibleProj"];

                float scale;
                float opacity;
                float interpolant = brainSucklerTime / 600f;
                scale = MathHelper.Clamp(MathHelper.Lerp(4f, 1.5f, interpolant * interpolant), 1.5f, 8f);
                opacity = MathHelper.Clamp(MathHelper.SmoothStep(0, 0.75f, interpolant * (1 + interpolant * 1.1f)), 0, 0.75f);
                
                GameShaders.Misc["TerRoguelike:CircularGradientOuter"].UseOpacity(opacity);
                GameShaders.Misc["TerRoguelike:CircularGradientOuter"].UseColor(Color.Black);
                GameShaders.Misc["TerRoguelike:CircularGradientOuter"].UseSecondaryColor(Color.Black);
                GameShaders.Misc["TerRoguelike:CircularGradientOuter"].UseSaturation(scale);
                GameShaders.Misc["TerRoguelike:CircularGradientOuter"].Apply();

                Vector2 drawPosition = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
                Main.EntitySpriteDraw(telegraphBase, drawPosition, null, Color.White, 0, telegraphBase.Size() / 2f, scale * 2000f, 0, 0);
                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            
            if (!Main.hideUI && escapeArrowTime > 0 && ModContent.GetInstance<TerRoguelikeConfig>().ObjectiveLocationArrow)
            {
                float opacity = MathHelper.Clamp(escapeArrowTime / 60f, 0, 1);
                int escapeTimer = TerRoguelikeWorld.escapeTimeSet - TerRoguelikeWorld.escapeTime;
                if (escapeTimer < 60)
                {
                    opacity *= escapeTimer / 60f;
                }

                float amplitude = (float)Math.Cos(escapeArrowTime / 60f * 3 + MathHelper.Pi);

                Texture2D escapeArrow = TexDict["BigArrow"];

                Vector2 playerToTargetVector = escapeArrowTarget - (Player.Center + Vector2.UnitY * Player.gfxOffY);
                float arrowRot = (playerToTargetVector).ToRotation();
                float distance = 240;
                float targetVectLength = playerToTargetVector.Length();
                if (targetVectLength < distance)
                    distance = targetVectLength;
                Vector2 arrowOffset = arrowRot.ToRotationVector2() * (distance + 16 * amplitude) / ZoomSystem.ScaleVector;
                arrowOffset += Vector2.UnitY * Player.gfxOffY;
                opacity *= 0.85f + 0.15f * amplitude;

                Color arrowColor = Color.DarkCyan;
                Color arrowOutlineColor = Color.Cyan;
                int frameHeight = escapeArrow.Height / 2;
                Rectangle arrowFrame = new Rectangle(0, 0, escapeArrow.Width, frameHeight - 2);
                Vector2 scale = new Vector2(1) / ZoomSystem.ScaleVector;
                Vector2 origin = arrowFrame.Size() * 0.5f;
                Main.EntitySpriteDraw(escapeArrow, Player.Center + arrowOffset - Main.screenPosition, arrowFrame, arrowColor * opacity * 0.7f, arrowRot, origin, scale, SpriteEffects.None);
                arrowFrame.Y += frameHeight;
                Main.EntitySpriteDraw(escapeArrow, Player.Center + arrowOffset - Main.screenPosition, arrowFrame, arrowOutlineColor * opacity * 0.9f, arrowRot, origin, scale, SpriteEffects.None);
            }

            if (ceremonialCrownStacks > 0)
            {
                var crownTex = TexDict["CeremonialCrownGems"];
                int vertiFrameCount = 3;
                int frameHeight = crownTex.Height / vertiFrameCount;
                int fullCycle = 12;
                float baseRot = Main.GlobalTimeWrappedHourly * MathHelper.Pi;
                float baseOutDist = 40;
                for (int i = 0; i < ceremonialCrownStacks; i++)
                {
                    int cycleCount = i / fullCycle;
                    int dir = cycleCount % 2 == 0 ? 1 : -1;
                    float outerRot = cycleCount * MathHelper.PiOver4;
                    Rectangle gemFrame = new Rectangle(0, i % vertiFrameCount * frameHeight, crownTex.Width, frameHeight - 2);
                    float extraRot = i / (float)fullCycle * MathHelper.TwoPi;
                    float outDist = baseOutDist + 15 * cycleCount;
                    float thisRot = extraRot + outerRot + baseRot * (1 - ((cycleCount + 1f) / (cycleCount + 2f))) * dir;
                    Vector2 drawPos = Player.Center + Vector2.UnitY * Player.gfxOffY + thisRot.ToRotationVector2() * outDist;
                    Main.EntitySpriteDraw(crownTex, drawPos - Main.screenPosition, gemFrame, Color.White * 0.75f, thisRot + MathHelper.PiOver2, gemFrame.Size() * 0.5f, 1f, SpriteEffects.None);
                }
            }

            return;

            if (evilEye > 0)
                EvilEyePlayerEffect();

            if (enchantingEye > 0)
                EnchantingEyePlayerEffect();
        }
        #endregion

        #region Item Drawing on Player
        public void EvilEyePlayerEffect()
        {
            int num = 0;
            num += Player.bodyFrame.Y / 56;
            if (num >= Main.OffsetsPlayerHeadgear.Length)
            {
                num = 0;
            }
            Vector2 vector = Main.OffsetsPlayerHeadgear[num];
            vector *= Player.Directions;
            Vector2 vector2 = new Vector2((float)(Player.width / 2), (float)(Player.height / 2)) + vector + (Player.MountedCenter - base.Player.Center);
            Player.sitting.GetSittingOffsetInfo(Player, out var posOffset, out var seatAdjustment);
            vector2 += posOffset + new Vector2(0f, seatAdjustment);
            if (Player.face == 19)
            {
                vector2.Y -= 5f * Player.gravDir;
            }
            if (Player.head == 276)
            {
                vector2.X += 2.5f * (float)Player.direction;
            }
            if (Player.mount.Active && Player.mount.Type == 52)
            {
                vector2.X += 14f * (float)Player.direction;
                vector2.Y -= 2f * Player.gravDir;
            }
            float y = -11.5f * Player.gravDir;
            int eyeDistance = Player.direction == 1 ? 7 : 3;
            Vector2 vector3 = new Vector2((float)(eyeDistance * Player.direction - ((Player.direction == 1) ? 1 : 0)), y) + Vector2.UnitY * Player.gfxOffY + vector2;
            Vector2 vector4 = new Vector2((float)(eyeDistance * Player.shadowDirection[1] - ((Player.direction == 1) ? 1 : 0)), y) + vector2;
            Vector2 vector5 = Vector2.Zero;
            if (Player.mount.Active && Player.mount.Cart)
            {
                int num2 = Math.Sign(Player.velocity.X);
                if (num2 == 0)
                {
                    num2 = Player.direction;
                }
                vector5 = Utils.RotatedBy(new Vector2(MathHelper.Lerp(0f, -8f, Player.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(Player.fullRotation / ((float)Math.PI / 4f)))), (double)Player.fullRotation, default(Vector2));
                if (num2 == Math.Sign(Player.fullRotation))
                {
                    vector5 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(Player.fullRotation / ((float)Math.PI / 4f)));
                }
            }
            if (Player.fullRotation != 0f)
            {
                vector3 = vector3.RotatedBy(Player.fullRotation, Player.fullRotationOrigin);
                vector4 = vector4.RotatedBy(Player.fullRotation, Player.fullRotationOrigin);
            }
            float num3 = 0f;
            Vector2 vector6 = Player.position + vector3 + vector5;
            Vector2 vector7 = Player.oldPosition + vector4 + vector5;
            vector7.Y -= num3 / 2f;
            vector6.Y -= num3 / 2f;
            float num4 = 0.58f;
            int num5 = (int)Vector2.Distance(vector6, vector7) / 3 + 1;
            if (Vector2.Distance(vector6, vector7) % 3f != 0f)
            {
                num5++;
            }
            for (float num6 = 1f; num6 <= (float)num5; num6 += 1f)
            {
                Dust[] dust = Main.dust;
                Vector2 center = base.Player.Center;
                Color newColor = default(Color);
                Dust obj = dust[Dust.NewDust(center, 0, 0, 182, 0f, 0f, 0, newColor)];
                obj.position = Vector2.Lerp(vector7, vector6, num6 / (float)num5);
                obj.noGravity = true;
                obj.velocity = Vector2.Zero;
                obj.scale = num4;
            }
        }
        public void EnchantingEyePlayerEffect()
        {
            int num = 0;
            num += Player.bodyFrame.Y / 56;
            if (num >= Main.OffsetsPlayerHeadgear.Length)
            {
                num = 0;
            }
            Vector2 vector = Main.OffsetsPlayerHeadgear[num];
            vector *= Player.Directions;
            Vector2 vector2 = new Vector2((float)(Player.width / 2), (float)(Player.height / 2)) + vector + (Player.MountedCenter - base.Player.Center);
            Player.sitting.GetSittingOffsetInfo(Player, out var posOffset, out var seatAdjustment);
            vector2 += posOffset + new Vector2(0f, seatAdjustment);
            if (Player.face == 19)
            {
                vector2.Y -= 5f * Player.gravDir;
            }
            if (Player.head == 276)
            {
                vector2.X += 2.5f * (float)Player.direction;
            }
            if (Player.mount.Active && Player.mount.Type == 52)
            {
                vector2.X += 14f * (float)Player.direction;
                vector2.Y -= 2f * Player.gravDir;
            }
            float y = -11.5f * Player.gravDir;
            int eyeDistance = Player.direction == 1 ? 3 : 7;
            Vector2 vector3 = new Vector2((float)(eyeDistance * Player.direction - ((Player.direction == 1) ? 1 : 0)), y) + Vector2.UnitY * Player.gfxOffY + vector2;
            Vector2 vector4 = new Vector2((float)(eyeDistance * Player.shadowDirection[1] - ((Player.direction == 1) ? 1 : 0)), y) + vector2;
            Vector2 vector5 = Vector2.Zero;
            if (Player.mount.Active && Player.mount.Cart)
            {
                int num2 = Math.Sign(Player.velocity.X);
                if (num2 == 0)
                {
                    num2 = Player.direction;
                }
                vector5 = Utils.RotatedBy(new Vector2(MathHelper.Lerp(0f, -8f, Player.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(Player.fullRotation / ((float)Math.PI / 4f)))), (double)Player.fullRotation, default(Vector2));
                if (num2 == Math.Sign(Player.fullRotation))
                {
                    vector5 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(Player.fullRotation / ((float)Math.PI / 4f)));
                }
            }
            if (Player.fullRotation != 0f)
            {
                vector3 = vector3.RotatedBy(Player.fullRotation, Player.fullRotationOrigin);
                vector4 = vector4.RotatedBy(Player.fullRotation, Player.fullRotationOrigin);
            }
            float num3 = 0f;
            Vector2 vector6 = Player.position + vector3 + vector5;
            Vector2 vector7 = Player.oldPosition + vector4 + vector5;
            vector7.Y -= num3 / 2f;
            vector6.Y -= num3 / 2f;
            float num4 = 0.6f;
            int num5 = (int)Vector2.Distance(vector6, vector7) / 3 + 1;
            if (Vector2.Distance(vector6, vector7) % 3f != 0f)
            {
                num5++;
            }
            for (float num6 = 1f; num6 <= (float)num5; num6 += 1f)
            {
                Dust[] dust = Main.dust;
                Vector2 center = base.Player.Center;
                Color newColor = default(Color);
                Dust obj = dust[Dust.NewDust(center, 0, 0, 180, 0f, 0f, 0, newColor)];
                obj.position = Vector2.Lerp(vector7, vector6, num6 / (float)num5);
                obj.noGravity = true;
                obj.velocity = Vector2.Zero;
                obj.scale = num4;
            }
        }
        #endregion

        #region Barrier Effect Drawing
        public class BarrierDrawLayer : PlayerDrawLayer
        {
            public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);

            public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
            {
                Player drawPlayer = drawInfo.drawPlayer;
                TerRoguelikePlayer modPlayer = drawPlayer.GetModPlayer<TerRoguelikePlayer>();
                if (drawInfo.shadow != 0f || drawPlayer.dead || (drawPlayer.immuneAlpha > 120) || modPlayer.deathEffectTimer > 0)
                    return false;

                return drawInfo.drawPlayer.GetModPlayer<TerRoguelikePlayer>().barrierHealth > 1f;
            }

            protected override void Draw(ref PlayerDrawSet drawInfo)
            {
                if (drawInfo.shadow != 0)
                    return;

                Player drawPlayer = drawInfo.drawPlayer;
                TerRoguelikePlayer modPlayer = drawPlayer.GetModPlayer<TerRoguelikePlayer>();

                List<DrawData> existingDrawData = drawInfo.DrawDataCache;
                for (int i = 0; i < 4; i++)
                {
                    float offsetScale = ((float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi)) * 0.15f + 0.75f;
                    float opacity = (0.13f + (0.23f * (0.5f + (modPlayer.barrierHealth / drawPlayer.statLifeMax2)))) * 0.3f;
                    List<DrawData> afterimages = new List<DrawData>();
                    for (int j = 0; j < existingDrawData.Count; j++)
                    {
                        var drawData = existingDrawData[j];
                        drawData.position = existingDrawData[j].position + (Vector2.UnitY * 6f * offsetScale).RotatedBy(MathHelper.PiOver2 * i);
                        drawData.color = Color.Yellow * opacity;
                        drawData.scale = new Vector2(1f);
                        afterimages.Add(drawData);
                        drawData.shader = 1;
                    }
                    drawInfo.DrawDataCache.InsertRange(0, afterimages);
                }
            }
        }
        #endregion

        #region Jet Leg Dash
        public void JetLegDashMovement()
        {
            Player.velocity.X = Player.maxRunSpeed * DashDirCache * 1.5f;
            for (int i = -1; i < 5; i++)
            {
                Player.AddImmuneTime(i, 1);
            }
            Player.immune = true;
            Player.immuneNoBlink = true;
        }
        #endregion

        #region Symbiotic Fungus Visuals

        public void DrawVisualFungi()
        {
            if (standingStillTime >= 45)
            {
                if (Main.GlobalTimeWrappedHourly % 0.33334 < 0.016667f)
                {
                    Vector2 position = Player.Center + (Vector2.UnitY * 12f) + Main.rand.NextVector2CircularEdge(24f, 12f);
                    visualFungi.Add(new VisualFungus(Main.rand.Next(150, 210), position, Main.rand.Next(0, Main.projFrames[ModContent.ProjectileType<HealingFungus>()]), Main.rand.NextBool() ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
                }
            }
            if (visualFungi.Count == 0)
            {
                return;
            }

            Texture2D fungTex = TexDict["HealingFungus"];
            int frameHeight = fungTex.Height / Main.projFrames[ModContent.ProjectileType<HealingFungus>()];
            for (int i = 0; i < visualFungi.Count; i++)
            {
                VisualFungus fungus = visualFungi[i];

                float opacity = 1f;

                if (fungus.Lifetime < 30)
                    opacity = MathHelper.Lerp(0f, 1f, fungus.Lifetime / 30f);
                else if (fungus.MaxLifetime - fungus.Lifetime < 30)
                    opacity = MathHelper.Lerp(0f, 1f, (fungus.MaxLifetime - fungus.Lifetime) / 30f);

                Main.EntitySpriteDraw(fungTex, fungus.Position - Main.screenPosition, new Rectangle(0, frameHeight * fungus.Type, fungTex.Width, frameHeight), Color.White * 0.5f * opacity, 0f, new Vector2(fungTex.Width * 0.5f, frameHeight * 0.5f), 1f, fungus.Effects);
                fungus.Position.Y -= 0.2f * (fungus.Lifetime / (float)fungus.MaxLifetime);

                fungus.Lifetime--;
            }
            visualFungi.RemoveAll(x => x.Lifetime <= 0);
        }
        public class VisualFungus
        {
            public VisualFungus(int maxLifetime, Vector2 position, int type, SpriteEffects effects)
            {
                MaxLifetime = maxLifetime;
                Lifetime = maxLifetime;
                Position = position;
                Type = type;
                Effects = effects;
            }
            public int MaxLifetime = 0;
            public int Lifetime = 0;
            public Vector2 Position = Vector2.Zero;
            public int Type = -1;
            public SpriteEffects Effects = SpriteEffects.None;
        }
        #endregion

        #region Primeval Visuals
        public void DrawPrimevalVisuals()
        {
            if (primevalRattleBoost)
            {
                if (Main.GlobalTimeWrappedHourly % 0.166667 < 0.016667f)
                {
                    Vector2 position = (Vector2.UnitY * 40f) + Main.rand.NextVector2CircularEdge(24f, 12f);
                    primevalStreaks.Add(new VisualFungus(Main.rand.Next(60, 90), position, Main.rand.Next(100), SpriteEffects.None));
                }
            }
            if (primevalStreaks.Count == 0)
            {
                return;
            }

            StartNonPremultipliedSpritebatch();
            Texture2D streakTex = TexDict["LerpLineGradient"];
            for (int i = 0; i < primevalStreaks.Count; i++)
            {
                var streak = primevalStreaks[i];

                float opacity = 1f;

                if (streak.Lifetime < 30)
                    opacity = MathHelper.Lerp(0f, 1f, streak.Lifetime / 30f);
                else if (streak.MaxLifetime - streak.Lifetime < 30)
                    opacity = MathHelper.Lerp(0f, 1f, (streak.MaxLifetime - streak.Lifetime) / 30f);

                Color streakColor = Color.Lerp(Color.Red, Color.Yellow, streak.Type * 0.008f);
                streakColor.A = (byte)(streakColor.A * opacity);
                for (int r = 0; r <= 1; r++)
                {
                    Main.EntitySpriteDraw(streakTex, Player.Center + streak.Position - Main.screenPosition + Player.gfxOffY * Vector2.UnitY, null, streakColor, MathHelper.Pi * r + MathHelper.PiOver2, streakTex.Size() * new Vector2(0, 0.5F), new Vector2(0.15f, 0.2f), streak.Effects);
                }
                streak.Position.Y -= 1f;

                streak.Lifetime--;
            }
            primevalStreaks.RemoveAll(x => x.Lifetime <= 0);
            StartAlphaBlendSpritebatch();
        }
        #endregion

        #region Death Effect
        public void DoDeathEffect()
        {

            var wantedTex = TextureAssets.Npc[NPCID.Ghost];
            if (wantedTex.State == AssetState.NotLoaded)
            {
                Main.Assets.Request<Texture2D>(TextureAssets.Npc[NPCID.Ghost].Name, AssetRequestMode.ImmediateLoad);
            }
            var ghostTex = wantedTex.Value;

            int frameCount = Main.npcFrameCount[NPCID.Ghost];
            int frameHeight = ghostTex.Height / frameCount;
            int frame = (int)((Main.GlobalTimeWrappedHourly / 0.15f) % frameCount);
            Vector2 offset;
            float opacity;
            if (reviveDeathEffect)
            {
                float posInterpolant = (float)-Math.Abs(Math.Pow((deathEffectTimer / 120f * 2f) - 1, 3)) + 1f;
                if (deathEffectTimer <= 60)
                    posInterpolant = (float)MathHelper.Clamp(posInterpolant * 2f, 0, 1f);
                offset = new Vector2(0, MathHelper.Lerp(0, -80f, posInterpolant));
                float opacityInterpolant = (deathEffectTimer) / 90f;
                if (deathEffectTimer <= 30)
                    opacityInterpolant = 1f - ((deathEffectTimer + 30) / 90f);
                opacity = MathHelper.Clamp(MathHelper.Lerp(0.3f, 1f, opacityInterpolant), 0f, 1f);
                if (deathEffectTimer == 20)
                    ZoomSystem.SetZoomAnimation(Main.GameZoomTarget, 20);
            }
            else
            {
                float posInterpolant = MathHelper.Clamp((float)-Math.Pow(deathEffectTimer / 120f, 3) + 1f, 0, 1f);
                offset = new Vector2(0, MathHelper.Lerp(0, -80f, posInterpolant));
                opacity = MathHelper.Clamp(MathHelper.Lerp(0f, 1f, (deathEffectTimer) / 60f), 0f, 1f);
            }

            StartAlphaBlendSpritebatch(false);
            Main.EntitySpriteDraw(ghostTex, Player.Center - Main.screenPosition + (offset), new Rectangle(0, frameHeight * frame, ghostTex.Width, frameHeight), Color.White * 0.5f * opacity, 0f, new Vector2(ghostTex.Width * 0.5f, (frameHeight * 0.5f)), 1f, SpriteEffects.None);
            Main.spriteBatch.End();

            if (!reviveDeathEffect)
                deathEffectTimer--;
            if (deathEffectTimer <= 0)
            {
                if (reviveDeathEffect)
                    SoundEngine.PlaySound(SoundID.NPCHit36 with { Volume = 0.4f });
                else if (!Player.dead)
                    ZoomSystem.SetZoomAnimation(Main.GameZoomTarget, 20);

                reviveDeathEffect = false;
                Player.immuneNoBlink = false;
            }
        }
        public override void OnRespawn()
        {
            ZoomSystem.SetZoomAnimation(Main.GameZoomTarget, 20);
            killerNPC = -1;
            killerProj = -1;
            DeathUI.itemsToDraw.Clear();
            deadTime = 0;
            creditsViewTime = 0;
        }
        #endregion

        public override void SaveData(TagCompound tag)
        {
            tag["isDeletableOnExit"] = isDeletableOnExit;
        }
        public override void LoadData(TagCompound tag)
        {
            isDeletableOnExit = TerRoguelikeMenu.prepareForRoguelikeGeneration ? tag.GetBool("isDeletableOnExit") : false;
        }
        public static void HealthUpIndicator(Player player)
        {
            string healthUp = Language.GetOrRegister("Mods.TerRoguelike.HealthUpAlert").Value;
            CombatText.NewText(player.getRect(), CombatText.HealLife, healthUp, true);
        }

        public Vector2 GetPositionRelativeToFrontHand(float length)
        {
            float rotation = Player.compositeFrontArm.rotation;

            float num = rotation + (float)Math.PI / 2f;
            Vector2 vector = new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * length;

            if (Player.direction == -1)
            {
                vector += new Vector2(4f, -2f);
                vector += new Vector2(0f, -3f).RotatedBy(rotation + (float)Math.PI / 2f);
            }
            else
            {
                vector += new Vector2(-4f, -2f);
                vector += new Vector2(0f, 3f).RotatedBy(rotation + (float)Math.PI / 2f);
            }

            return Player.MountedCenter + vector;
        }
    }
}
