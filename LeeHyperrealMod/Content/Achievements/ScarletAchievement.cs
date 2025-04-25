using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10, typeof(ScarletServerAchievement))]
    class ScarletAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_SCARLET_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_SCARLET_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public List<ItemDef> bannedItems;

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);

            bannedItems = new List<ItemDef>();

            bannedItems.Add(RoR2Content.Items.BleedOnHit);
            bannedItems.Add(RoR2Content.Items.BleedOnHitAndExplode);
            bannedItems.Add(DLC1Content.Items.FragileDamageBonus);
            bannedItems.Add(DLC1Content.Items.FragileDamageBonusConsumed);
            bannedItems.Add(DLC2Content.Items.TriggerEnemyDebuffs);
            bannedItems.Add(DLC2Content.Items.TeleportOnLowHealthConsumed);
        }

        public override bool CheckInventory(RunReport.PlayerInfo info)
        {
            CharacterMaster master = info.master;
            if (master)
            {
                Inventory inventory = master.inventory;

                if (inventory)
                {
                    int bannedCount = 0;

                    foreach (ItemDef item in bannedItems) 
                    {
                        bannedCount += inventory.GetItemCount(item);
                        if (bannedCount > 0) 
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }


        internal class ScarletServerAchievement : BaseGachaServerAchievement
        {
            public override int Chance => 5;
            public override string RequiredChestType => "GoldChest";

            internal override bool ChestValidator(GameObject purchasedObject)
            {
                if (purchasedObject.name.Contains(RequiredChestType))
                {
                    if (lastOpenedLegendary == 0)
                    {
                        lastOpenedLegendary = purchasedObject.GetInstanceID();
                        return true;
                    }

                    if (lastOpenedLegendary != purchasedObject.GetInstanceID())
                    {
                        lastOpenedLegendary = purchasedObject.GetInstanceID();
                        return true;
                    }
                }

                return false;
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
