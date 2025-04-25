using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10, typeof(GreenVFXServerAchievement))]
    class GreenVFXAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_GREEN_VFX_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_GREEN_VFX_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public List<ItemDef> HealingItems;

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);
            HealingItems = new List<ItemDef>();

            HealingItems.Add(RoR2Content.Items.FlatHealth);
            HealingItems.Add(RoR2Content.Items.Mushroom);
            HealingItems.Add(RoR2Content.Items.HealWhileSafe);
            HealingItems.Add(RoR2Content.Items.Medkit);
            HealingItems.Add(RoR2Content.Items.Tooth);
            HealingItems.Add(DLC1Content.Items.HealingPotion);
            HealingItems.Add(RoR2Content.Items.HealOnCrit);
            HealingItems.Add(RoR2Content.Items.Infusion);
            HealingItems.Add(RoR2Content.Items.Seed);
            HealingItems.Add(RoR2Content.Items.TPHealingNova);
            HealingItems.Add(DLC2Content.Items.ExtraStatsOnLevelUp);
            HealingItems.Add(RoR2Content.Items.Plant);
            HealingItems.Add(RoR2Content.Items.IncreaseHealing);
            HealingItems.Add(RoR2Content.Items.Pearl);
            HealingItems.Add(RoR2Content.Items.ShinyPearl);
            HealingItems.Add(RoR2Content.Items.Knurl);
            HealingItems.Add(RoR2Content.Items.RepeatHeal);
            HealingItems.Add(DLC1Content.Items.HalfSpeedDoubleHealth);
            HealingItems.Add(RoR2Content.Items.LunarUtilityReplacement);
            HealingItems.Add(RoR2Content.Items.ShieldOnly);
            HealingItems.Add(DLC1Content.Items.MushroomVoid);
        }

        public override bool CheckInventory(RunReport.PlayerInfo info)
        {
            CharacterMaster master = info.master;
            if (master)
            {
                Inventory inventory = master.inventory;

                if (inventory)
                {
                    int healItemCount = 0;

                    foreach (ItemDef item in HealingItems) 
                    {
                        healItemCount += inventory.GetItemCount(item);

                        if (healItemCount >= 15) 
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal class GreenVFXServerAchievement : BaseGachaServerAchievement
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
