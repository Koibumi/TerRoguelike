﻿using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TerRoguelike.Managers;
using TerRoguelike.Particles;
using TerRoguelike.Projectiles;
using TerRoguelike.Systems;
using TerRoguelike.Utilities;
using TerRoguelike.World;
using static Terraria.GameContent.PlayerEyeHelper;
using static TerRoguelike.Managers.TextureManager;
using static TerRoguelike.Schematics.SchematicManager;
using static TerRoguelike.Systems.MusicSystem;
using static TerRoguelike.Systems.RoomSystem;
using static TerRoguelike.Utilities.TerRoguelikeUtils;
using static TerRoguelike.Systems.EnemyHealthBarSystem;
using System.Diagnostics;

namespace TerRoguelike.NPCs.Enemy.Boss
{
    public class MoonLord : BaseRoguelikeNPC
    {
        public Entity target;
        public Vector2 spawnPos;
        public bool ableToHit = true;
        public bool canBeHit = true;
        public override int modNPCID => ModContent.NPCType<MoonLord>();
        public int handType = ModContent.NPCType<MoonLordHand>();
        public int headType = ModContent.NPCType<MoonLordHead>();
        public override List<int> associatedFloors => new List<int>() { FloorDict["Lunar"] };
        public override int CombatStyle => -1;
        public int coreCurrentFrame = 0;
        public int mouthCurrentFrame = 0;
        public int headEyeCurrentFrame = 0;
        public int leftHandCurrentFrame = 0;
        public int rightHandCurrentFrame = 0;
        public int emptyEyeCurrentFrame = 0;
        public Rectangle coreFrame;
        public Rectangle mouthFrame;
        public Rectangle headEyeFrame;
        public Rectangle leftHandFrame;
        public Rectangle rightHandFrame;
        public Rectangle emptyEyeFrame;
        Vector2 headPos;
        Vector2 leftHandPos;
        Vector2 rightHandPos;
        Vector2 headEyeVector;
        Vector2 leftEyeVector;
        Vector2 rightEyeVector;

        public Texture2D coreTex, coreCrackTex, emptyEyeTex, innerEyeTex, lowerArmTex, upperArmTex, mouthTex, sideEyeTex, topEyeTex, topEyeOverlayTex, headTex, handTex, bodyTex;
        public int leftHandWho = -1; // yes, technically moon lord's "Left" is not the same as the left for the viewer, and vice versa for right hand. I do not care. internally it will be based on viewer perspective.
        public int rightHandWho = -1;
        public int headWho = -1;

        public int deadTime = 0;
        public int cutsceneDuration = 120;
        public int deathCutsceneDuration = 120;

        public static Attack None = new Attack(0, 0, 180);
        public static Attack PhantSpin = new Attack(1, 30, 180);
        public static Attack PhantBolt = new Attack(2, 30, 180);
        public static Attack PhantSphere = new Attack(3, 30, 180);
        public static Attack Tentacle = new Attack(4, 30, 180);
        public static Attack Deathray = new Attack(5, 30, 180);
        public static Attack PhantSpawn = new Attack(6, 30, 180);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 60;
            NPC.height = 88;
            NPC.aiStyle = -1;
            NPC.damage = 36;
            NPC.lifeMax = 25000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0f;
            modNPC.drawCenter = new Vector2(0, 0);
            modNPC.IgnoreRoomWallCollision = true;
            modNPC.OverrideIgniteVisual = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.behindTiles = true;
            coreTex = TexDict["MoonLordCore"];
            coreCrackTex = TexDict["MoonLordCoreCracks"];
            emptyEyeTex = TexDict["MoonLordEmptyEye"];
            innerEyeTex = TexDict["MoonLordInnerEye"];
            lowerArmTex = TexDict["MoonLordLowerArm"];
            upperArmTex = TexDict["MoonLordUpperArm"];
            mouthTex = TexDict["MoonLordMouth"];
            sideEyeTex = TexDict["MoonLordSideEye"];
            topEyeTex = TexDict["MoonLordTopEye"];
            topEyeOverlayTex = TexDict["MoonLordTopEyeOverlay"];
            headTex = TexDict["MoonLordHead"];
            handTex = TexDict["MoonLordHand"];
            bodyTex = TexDict["MoonLordBodyHalf"];
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.localAI[0] = -(cutsceneDuration + 30);
            NPC.direction = -1;
            NPC.spriteDirection = -1;
            spawnPos = NPC.Center;
            NPC.ai[2] = None.Id;
            ableToHit = false;
            rightEyeVector = leftEyeVector = headEyeVector = Vector2.Zero;

            for (int i = -1; i <= 1; i += 2)
            {
                Vector2 handSpawnPos = new Vector2(800 * i, -40) + NPC.Center;
                int whoAmI = NPC.NewNPC(NPC.GetSource_FromThis(), (int)handSpawnPos.X, (int)handSpawnPos.Y, handType);
                NPC hand = Main.npc[whoAmI];
                hand.direction = hand.spriteDirection = i;
                if (i == -1)
                {
                    leftHandWho = whoAmI;
                    leftHandPos = handSpawnPos;
                }
                else
                {
                    rightHandWho = whoAmI;
                    rightHandPos = handSpawnPos;
                }
                    
            }
            Vector2 headSpawnPos = new Vector2(0, -700) + NPC.Center;
            headWho = NPC.NewNPC(NPC.GetSource_FromThis(), (int)headSpawnPos.X, (int)headSpawnPos.Y, headType);
            headPos = headSpawnPos;
        }
        public override void PostAI()
        {
            leftHandPos = NPC.Center + new Vector2(-400, -60);
            rightHandPos = NPC.Center + new Vector2(400, -60);
            //rightHandPos = Main.MouseWorld;
            headPos = NPC.Center + new Vector2(0, -395); // do NOT touch this one

            NPC leftHand = leftHandWho >= 0 ? Main.npc[leftHandWho] : null;
            NPC rightHand = rightHandWho >= 0 ? Main.npc[rightHandWho] : null;
            NPC head = headWho >= 0 ? Main.npc[headWho] : null;

            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            canBeHit = false;
            bool enableHitBox = true;

            if (leftHand != null)
            {
                if (leftHand.type != handType)
                {
                    leftHandWho = -1;
                }
                else if (leftHand.life > 1)
                {
                    enableHitBox = false;
                    leftHand.Center = leftHandPos;
                    InnerEyePositionUpdate(ref leftEyeVector, leftHandPos);
                }
            }
            if (rightHand != null)
            {
                if (rightHand.type != handType)
                {
                    rightHandWho = -1;
                }
                else if(rightHand.life > 1)
                {
                    enableHitBox = false;
                    rightHand.Center = rightHandPos;
                    InnerEyePositionUpdate(ref rightEyeVector, rightHandPos);
                }
            }
            if (head != null)
            {
                if (head.type != headType)
                {
                    headWho = -1;
                }
                else if (head.life > 1)
                {
                    enableHitBox = false;
                    head.Center = headPos;
                    InnerEyePositionUpdate(ref headEyeVector, headPos);
                }
            }
            
            if (enableHitBox && deadTime == 0)
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
                canBeHit = true;
            }

            void InnerEyePositionUpdate(ref Vector2 eyeVector, Vector2 basePosition)
            {
                bool eyeCenter = target == null;

                if (eyeCenter)
                {
                    eyeVector = Vector2.Lerp(eyeVector, Vector2.Zero, 0.2f);
                }
                else
                {
                    Vector2 targetPos = target != null ? target.Center : spawnPos + new Vector2(0, -80);
                    float maxEyeOffset = 16;

                    Vector2 targetVect = targetPos - basePosition;
                    if (targetVect.Length() > maxEyeOffset)
                        targetVect = targetVect.SafeNormalize(Vector2.UnitY) * maxEyeOffset;
                    eyeVector = Vector2.Lerp(eyeVector, targetVect, 0.2f);
                }
            }
        }
        public override void AI()
        {
            NPC.frameCounter += 0.2d;
            if (deadTime > 0)
            {
                CheckDead();
                return;
            }
            if (modNPC.isRoomNPC && NPC.localAI[0] == -(cutsceneDuration + 30))
            {
                SetBossTrack(TempleGolemTheme);
            }

            ableToHit = false;
            canBeHit = true;

            if (NPC.localAI[0] < 0)
            {
                target = modNPC.GetTarget(NPC);

                if (NPC.localAI[0] == -cutsceneDuration)
                {
                    CutsceneSystem.SetCutscene(NPC.Center, cutsceneDuration, 30, 30, 2.5f);
                }
                NPC.localAI[0]++;

                if (NPC.localAI[0] == -30)
                {
                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;
                    NPC.ai[1] = 0;
                    enemyHealthBar = new EnemyHealthBar([NPC.whoAmI, headWho, leftHandWho, rightHandWho], NPC.FullName);
                }
            }
            else
            {
                NPC.localAI[0]++;
                BossAI();
            }
        }
        public void BossAI()
        {
            target = modNPC.GetTarget(NPC);
            NPC.ai[1]++;

            if (NPC.ai[0] == None.Id)
            {
                if (NPC.ai[1] >= None.Duration)
                {
                    ChooseAttack();
                }
                else
                {

                }
            }

            if (NPC.ai[0] == PhantSpin.Id)
            {                
                if (NPC.ai[1] >= PhantSpin.Duration)
                {
                    NPC.ai[0] = None.Id;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = PhantSpin.Id;
                }
            }
            else if (NPC.ai[0] == PhantBolt.Id)
            {
                if (NPC.ai[1] >= PhantBolt.Duration)
                {
                    NPC.ai[0] = None.Id;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = PhantBolt.Id;
                }
            }
            else if (NPC.ai[0] == PhantSphere.Id)
            {
                if (NPC.ai[1] >= PhantSphere.Duration)
                {
                    NPC.ai[0] = None.Id;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = PhantSphere.Id;
                }
            }
            else if (NPC.ai[0] == Tentacle.Id)
            {
                if (NPC.ai[1] >= Tentacle.Duration)
                {
                    NPC.ai[0] = None.Id;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = Tentacle.Id;
                }
            }
            else if (NPC.ai[0] == Deathray.Id)
            {
                if (NPC.ai[1] >= Deathray.Duration)
                {
                    NPC.ai[0] = None.Id;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = Deathray.Id;
                }
            }
            else if (NPC.ai[0] == PhantSpawn.Id)
            {
                if (NPC.ai[1] >= PhantSpawn.Duration)
                {
                    NPC.ai[0] = None.Id;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = PhantSpawn.Id;
                }
            }
        }
        public void ChooseAttack()
        {
            NPC.ai[1] = 0;
            int chosenAttack = 0;

            List<Attack> potentialAttacks = new List<Attack>() { PhantSpin, PhantBolt, PhantSphere, Tentacle, Deathray, PhantSpawn };
            potentialAttacks.RemoveAll(x => x.Id == (int)NPC.ai[2]);

            int totalWeight = 0;
            for (int i = 0; i < potentialAttacks.Count; i++)
            {
                totalWeight += potentialAttacks[i].Weight;
            }
            int chosenRandom = Main.rand.Next(totalWeight);

            for (int i = potentialAttacks.Count - 1; i >= 0; i--)
            {
                Attack attack = potentialAttacks[i];
                chosenRandom -= attack.Weight;
                if (chosenRandom < 0)
                {
                    chosenAttack = attack.Id;
                    break;
                }
            }

            NPC.ai[0] = chosenAttack;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return ableToHit;
        }
        public override bool CanHitNPC(NPC target)
        {
            return ableToHit;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return canBeHit ? null : false;
        }
        public override bool CheckDead()
        {
            if (deadTime >= deathCutsceneDuration - 30)
            {
                return true;
            }

            NPC.ai[0] = None.Id;
            NPC.ai[1] = 1;

            modNPC.OverrideIgniteVisual = true;
            NPC.life = 1;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.active = true;
            ableToHit = false;
            canBeHit = false;

            if (deadTime == 0)
            {
                enemyHealthBar.ForceEnd(0);
                NPC.velocity = Vector2.Zero;
                NPC.rotation = 0;
                modNPC.ignitedStacks.Clear();

                if (modNPC.isRoomNPC)
                {
                    ActiveBossTheme.endFlag = true;
                    Room room = RoomList[modNPC.sourceRoomListID];
                    room.bossDead = true;
                    ClearChildren();
                }
                CutsceneSystem.SetCutscene(NPC.Center, deathCutsceneDuration, 30, 30, 2.5f);
            }

            void ClearChildren()
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == NPC.whoAmI)
                        continue;

                    NPC childNPC = Main.npc[i];
                    if (childNPC == null)
                        continue;
                    if (!childNPC.active)
                        continue;

                    TerRoguelikeGlobalNPC modChildNPC = childNPC.ModNPC();
                    if (modChildNPC == null)
                        continue;
                    if (modChildNPC.isRoomNPC && modChildNPC.sourceRoomListID == modNPC.sourceRoomListID)
                    {
                        childNPC.StrikeInstantKill();
                        childNPC.active = false;
                    }
                }
            }
            deadTime++;

            if (deadTime >= deathCutsceneDuration - 30)
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
                NPC.StrikeInstantKill();
            }

            return deadTime >= cutsceneDuration - 30;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; (double)i < hit.Damage * 0.015d; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Vortex, hit.HitDirection, -1f);
                    Main.dust[d].noLight = true;
                    Main.dust[d].noLightEmittence = true;
                }
            }
        }
        public override void OnKill()
        {
            
        }
        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            boundingBox = NPC.Hitbox;
        }
        public override void FindFrame(int frameHeight)
        {
            bool leftHandAlive = leftHandWho >= 0 && Main.npc[leftHandWho].life > 1;
            bool rightHandAlive = rightHandWho >= 0 && Main.npc[rightHandWho].life > 1;
            bool headAlive = headWho >= 0 && Main.npc[headWho].life > 1;

            int showcaseFrame = (int)(NPC.frameCounter * 0.65d) % 20;
            if (showcaseFrame > 6)
                showcaseFrame = 0;
            else
            {
                switch (showcaseFrame)
                {
                    default:
                    case 0:
                    case 6:
                        showcaseFrame = 0;
                        break;
                    case 1:
                    case 5:
                        showcaseFrame = 1;
                        break;
                    case 2:
                    case 4:
                        showcaseFrame = 2;
                        break;
                    case 3:
                        showcaseFrame = 3;
                        break;
                }
            }

            frameHeight = coreTex.Height / 5;
            coreCurrentFrame = leftHandAlive || rightHandAlive || headAlive ? 0 : (int)NPC.frameCounter % 4 + 1;
            coreFrame = new Rectangle(0, coreCurrentFrame * frameHeight, coreTex.Width, frameHeight - 2);

            frameHeight = mouthTex.Height / 3;
            mouthCurrentFrame = 0;
            mouthFrame = new Rectangle(0, mouthCurrentFrame * frameHeight, mouthTex.Width, frameHeight - 2);

            frameHeight = topEyeOverlayTex.Height / 4;
            headEyeCurrentFrame = showcaseFrame;
            headEyeFrame = new Rectangle(0, headEyeCurrentFrame * frameHeight, topEyeOverlayTex.Width, frameHeight - 2);

            frameHeight = handTex.Height / 4;
            leftHandCurrentFrame = showcaseFrame;
            leftHandFrame = new Rectangle(0, leftHandCurrentFrame * frameHeight, handTex.Width, frameHeight - 2);

            rightHandCurrentFrame = showcaseFrame;
            rightHandFrame = new Rectangle(0, rightHandCurrentFrame * frameHeight, handTex.Width, frameHeight - 2);

            frameHeight = emptyEyeTex.Height / 4;
            emptyEyeCurrentFrame = (int)NPC.frameCounter % 4;
            emptyEyeFrame = new Rectangle(0, emptyEyeCurrentFrame * frameHeight, emptyEyeTex.Width, frameHeight - 2);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool leftHandAlive = leftHandWho >= 0 && Main.npc[leftHandWho].life > 1;
            bool rightHandAlive = rightHandWho >= 0 && Main.npc[rightHandWho].life > 1;
            bool headAlive = headWho >= 0 && Main.npc[headWho].life > 1;

            Vector2 bodyDrawPos = NPC.Center + new Vector2(0, 43);
            Vector2 shoulderAnchor = new Vector2(220, -98);
            float upperArmLength = upperArmTex.Height * 0.8f;
            float lowerArmLength = lowerArmTex.Height * 1f;
            float upperArmLengthRatio = 1 / ((upperArmLength + lowerArmLength) / upperArmLength);

            Vector2 leftUpperArmOrigin = upperArmTex.Size() * new Vector2(0.4477f, 0.17f);
            Vector2 rightUpperArmOrigin = upperArmTex.Size() * new Vector2(1 - 0.4477f, 0.17f);
            Vector2 lowerArmOrigin = lowerArmTex.Size() * new Vector2(0.5f, 0.9f);

            Vector2 leftShoulderPos = bodyDrawPos + shoulderAnchor * new Vector2(-1, 1);
            Vector2 rightShoulderPos = bodyDrawPos + shoulderAnchor * new Vector2(1, 1);

            Vector2 leftHandBottomPos = leftHandPos + new Vector2(0, 32);
            Vector2 rightHandBottomPos = rightHandPos + new Vector2(0, 32);
            Vector2 leftShoulderHandVect = leftHandBottomPos - leftShoulderPos;
            Vector2 rightShoulderHandVect = rightHandBottomPos - rightShoulderPos;

            float leftUpperArmRot = (float)Math.Asin((leftShoulderHandVect * upperArmLengthRatio).Length() / upperArmLength) + leftShoulderHandVect.ToRotation() - MathHelper.Pi;
            Vector2 leftElbowPos = (leftUpperArmRot + MathHelper.PiOver2).ToRotationVector2() * upperArmLength + leftShoulderPos;
            float rightUpperArmRot = (float)Math.Asin((rightShoulderHandVect * upperArmLengthRatio).Length() / upperArmLength) * -1 + rightShoulderHandVect.ToRotation();
            Vector2 rightElbowPos = (rightUpperArmRot + MathHelper.PiOver2).ToRotationVector2() * upperArmLength + rightShoulderPos;

            float leftLowerArmRot = (leftHandBottomPos - leftElbowPos).ToRotation() + MathHelper.PiOver2;
            float rightLowerArmRot = (rightHandBottomPos - rightElbowPos).ToRotation() + MathHelper.PiOver2;


            List<StoredDraw> draws = [];
            draws.Add(new StoredDraw(upperArmTex, leftShoulderPos, null, Color.White, leftUpperArmRot, leftUpperArmOrigin, NPC.scale, SpriteEffects.None));
            draws.Add(new StoredDraw(upperArmTex, rightShoulderPos, null, Color.White, rightUpperArmRot, rightUpperArmOrigin, NPC.scale, SpriteEffects.FlipHorizontally));

            for (int i = -1; i <= 1; i += 2)
            {
                draws.Add(new StoredDraw(bodyTex, bodyDrawPos, null, Color.White, 0, bodyTex.Size() * new Vector2(i == -1 ? 1 : 0, 0.5f), NPC.scale, i == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
            }
            draws.Add(new StoredDraw(coreCrackTex, NPC.Center + new Vector2(2, -11), null, Color.White, 0, coreCrackTex.Size() * 0.5f, NPC.scale, SpriteEffects.None));
            draws.Add(new StoredDraw(coreTex, NPC.Center + new Vector2(-1, 0), coreFrame, Color.White, 0, coreFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None));

            draws.Add(new StoredDraw(lowerArmTex, leftElbowPos, null, Color.White, leftLowerArmRot, lowerArmOrigin, NPC.scale, SpriteEffects.None));
            draws.Add(new StoredDraw(lowerArmTex, rightElbowPos, null, Color.White, rightLowerArmRot, lowerArmOrigin, NPC.scale, SpriteEffects.FlipHorizontally));

            draws.Add(new StoredDraw(emptyEyeTex, leftHandPos + new Vector2(0, -2), emptyEyeFrame, Color.White, 0, emptyEyeFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None));
            if (leftHandAlive)
            {
                draws.Add(new StoredDraw(sideEyeTex, leftHandPos, null, Color.White, 0, sideEyeTex.Size() * 0.5f, NPC.scale, SpriteEffects.None));
                draws.Add(new StoredDraw(innerEyeTex, leftHandPos + leftEyeVector, null, Color.White, 0, innerEyeTex.Size() * 0.5f, NPC.scale, SpriteEffects.None));
            }
            draws.Add(new StoredDraw(handTex, leftHandPos + new Vector2(2, -49), leftHandFrame, Color.White, 0, leftHandFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None));

            draws.Add(new StoredDraw(emptyEyeTex, rightHandPos + new Vector2(0, -2), emptyEyeFrame, Color.White, 0, emptyEyeFrame.Size() * 0.5f, NPC.scale, SpriteEffects.FlipHorizontally));
            if (rightHandAlive)
            {
                draws.Add(new StoredDraw(sideEyeTex, rightHandPos, null, Color.White, 0, sideEyeTex.Size() * 0.5f, NPC.scale, SpriteEffects.FlipHorizontally));
                draws.Add(new StoredDraw(innerEyeTex, rightHandPos + rightEyeVector, null, Color.White, 0, innerEyeTex.Size() * 0.5f, NPC.scale, SpriteEffects.None));
            }
            draws.Add(new StoredDraw(handTex, rightHandPos + new Vector2(-2, -49), rightHandFrame, Color.White, 0, rightHandFrame.Size() * 0.5f, NPC.scale, SpriteEffects.FlipHorizontally));

            draws.Add(new StoredDraw(headTex, headPos + new Vector2(0, 4), null, Color.White, 0, headTex.Size() * new Vector2(0.5f, 0.25f), NPC.scale, SpriteEffects.None));
            draws.Add(new StoredDraw(mouthTex, headPos + new Vector2(1, 212), mouthFrame, Color.White, 0, mouthFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None));

            draws.Add(new StoredDraw(emptyEyeTex, headPos, emptyEyeFrame, Color.White, 0, emptyEyeFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None));
            if (headAlive)
            {
                draws.Add(new StoredDraw(topEyeTex, headPos, null, Color.White, 0, topEyeTex.Size() * 0.5f, NPC.scale, SpriteEffects.None));
                draws.Add(new StoredDraw(innerEyeTex, headPos + headEyeVector, null, Color.White, 0, innerEyeTex.Size() * 0.5f, NPC.scale, SpriteEffects.None));
                draws.Add(new StoredDraw(topEyeOverlayTex, headPos + new Vector2(0, 4), headEyeFrame, Color.White, 0, headEyeFrame.Size() * 0.5f, NPC.scale, SpriteEffects.None));
            }

            Vector2 drawOff = -Main.screenPosition;

            if (modNPC.ignitedStacks.Count > 0 || (leftHandAlive && Main.npc[leftHandWho].ModNPC().ignitedStacks.Count > 0) || (rightHandAlive && Main.npc[rightHandWho].ModNPC().ignitedStacks.Count > 0) || (headAlive && Main.npc[headWho].ModNPC().ignitedStacks.Count > 0))
            {
                StartAlphaBlendSpritebatch();

                Color color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextFloat(0.4f, 0.6f + float.Epsilon) + 0.2f + (0.2f * (float)Math.Cos((Main.GlobalTimeWrappedHourly * 20f)))) * 0.8f;
                Vector3 colorHSL = Main.rgbToHsl(color);
                GameShaders.Misc["TerRoguelike:BasicTint"].UseOpacity(1f);
                GameShaders.Misc["TerRoguelike:BasicTint"].UseColor(Main.hslToRgb(1 - colorHSL.X, colorHSL.Y, colorHSL.Z));
                GameShaders.Misc["TerRoguelike:BasicTint"].Apply();

                for (int i = 0; i < draws.Count; i++)
                {
                    var draw = draws[i];
                    if (draw.texture.Width < 100) // every small texture here is covered up by a bigger texture. no point in wasting time drawing ignite textures for things that would have no effect
                        continue;
                    for (int j = 0; j < 8; j++)
                    {
                        draw.Draw(drawOff + Vector2.UnitX.RotatedBy(j * MathHelper.PiOver4 + draw.rotation) * 2);
                    }
                }

                StartVanillaSpritebatch();
            }

            for (int i = 0; i < draws.Count; i++)
            {
                draws[i].Draw(drawOff);
            }
            return false;
        }
    }
}
