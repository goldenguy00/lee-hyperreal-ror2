using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10, typeof(YellowVFXServerAchievement))]
    class YellowVFXAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_YELLOW_VFX_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_YELLOW_VFX_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);
        }

        public override bool CheckInventory(RunReport.PlayerInfo info) 
        {
            CharacterMaster master = info.master;
            if (master) 
            {
                Inventory inventory = master.inventory;

                if (inventory)
                {
                    int yellowItemCount = inventory.GetTotalItemCountOfTier(ItemTier.Boss);

                    if (yellowItemCount >= 5) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal class YellowVFXServerAchievement : BaseGachaServerAchievement
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
                    if (Modules.Config.playLootBoxSound.Value) 
                    {
                        new PlaySoundNetworkRequest(body.netId, "Play_Loot_Box").Send(R2API.Networking.NetworkDestination.Clients);
                    }

                    new AchievementGranterNetworkRequest(this.achievementDef.serverIndex.intValue, body.netId).Send(R2API.Networking.NetworkDestination.Clients);
                }
            }
        }
    }
}
