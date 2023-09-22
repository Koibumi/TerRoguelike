﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using TerRoguelike.World;

namespace TerRoguelike.Projectiles
{
    public class TerRoguelikeGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public ProcChainBools procChainBools = new ProcChainBools();
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (TerRoguelikeWorld.IsTerRoguelikeWorld)
            {
                modifiers.DamageVariationScale *= 0;
            }

            if (procChainBools.critPreviously)
                modifiers.SetCrit();
            else if (!procChainBools.originalHit)
            {
                modifiers.DisableCrit();
            }
        }
    }
    public class ProcChainBools
    {
        public bool originalHit = true;
        public bool critPreviously = false;
        public bool clinglyGrenadePreviously = false;
    }
}
