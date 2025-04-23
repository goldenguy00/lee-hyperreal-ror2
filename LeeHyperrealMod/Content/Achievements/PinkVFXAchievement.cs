using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10, typeof(PinkVFXServerAchievement))]
    class PinkVFXAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_PINK_VFX_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_PINK_VFX_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public LeeHyperrealDomainController domainController;

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);

            Hook();
        }

        private void Hook()
        {
            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
        }

        private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);

            if (self) 
            {
                if (self.baseNameToken == LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_NAME" && self.hasEffectiveAuthority) 
                {
                    if (!domainController)
                    {
                        domainController = self.gameObject.GetComponent<LeeHyperrealDomainController>();
                    }
                }
            }
        }

        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
        {
            if (self)
            {
                if (self.isChampion || self.isBoss)
                {
                    if (domainController) 
                    {
                        if (domainController.HasBuiltUpMaxStack) 
                        {
                            //Has died, and has the OnFire debuff? sounds good to me. Grant it!
                            base.Grant();
                        }
                    }
                }
            }
            orig(self);
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
            Unhook();
        }

        private void Unhook()
        {
            On.RoR2.CharacterBody.OnDeathStart -= CharacterBody_OnDeathStart;
        }

        public override bool CheckInventory(RunReport.PlayerInfo info)
        {
            // This is not going to be run
            return false;
        }

        internal class PinkVFXServerAchievement : BaseGachaServerAchievement
        {
            public override int Chance => 2;
            public override string RequiredChestType => "Chest1";

            internal override bool ChestValidator(GameObject purchasedObject)
            {
                return base.ChestValidator(purchasedObject);
            }

            internal override void CheckRoll(CharacterBody body)
            {
                base.CheckRoll(body);
                int rnd = UnityEngine.Random.Range(0, 101);

                if (rnd <= Chance)
                {
                    new AchievementGranterNetworkRequest(this.achievementDef.serverIndex.intValue, body.netId).Send(R2API.Networking.NetworkDestination.Clients);
                }
            }
        }
    }
}
