using RoR2;
using RoR2.Achievements;
using System;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    internal abstract class BaseGachaServerAchievement : BaseServerAchievement
    {

        public static int lastOpenedLarge = 0;
        public static int lastOpenedLegendary = 0;
        public static int lastOpenedSmall = 0;

        public abstract string RequiredChestType { get; }
        public abstract int Chance { get; }

        public enum ChestType 
        {
            
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Hook();
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Unhook();
        }

        private void Unhook()
        {
            On.RoR2.CostTypeDef.PayCost -= CostTypeDef_PayCost;
        }

        private void Hook()
        {
            On.RoR2.CostTypeDef.PayCost += CostTypeDef_PayCost;
        }

        //Check to prevent multiple rolls on different achievements.
        internal virtual CostTypeDef.PayCostResults CostTypeDef_PayCost(On.RoR2.CostTypeDef.orig_PayCost orig, CostTypeDef self, int cost, Interactor activator, UnityEngine.GameObject purchasedObject, Xoroshiro128Plus rng, ItemIndex avoidedItemIndex)
        {
            CostTypeDef.PayCostResults result = orig(self, cost, activator, purchasedObject, rng, avoidedItemIndex);
            bool allowRoll = false;
            CharacterBody body = activator.GetComponent<CharacterBody>();

            //check if the body matches us
            // Check purchased object
            //Roll the dice and see if you earn it
            if (body.baseNameToken != LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_NAME")
            {
                // Exit
                return result;
            }

            allowRoll = ChestValidator(purchasedObject);

            if (allowRoll)
            {
                //Roll the dice and see what you get.
                CheckRoll(body);
            }

            return result;
        }

        internal virtual bool ChestValidator(GameObject purchasedObject)
        {
            if (purchasedObject.name.Contains("Chest1"))
            {
                if (lastOpenedSmall == 0)
                {
                    lastOpenedSmall = purchasedObject.GetInstanceID();
                    return true;
                }

                if (lastOpenedSmall != purchasedObject.GetInstanceID())
                {
                    lastOpenedSmall = purchasedObject.GetInstanceID();
                    return true;
                }
            }

            if (purchasedObject.name.Contains("Chest2"))
            {
                if (lastOpenedLarge == 0)
                {
                    lastOpenedLarge = purchasedObject.GetInstanceID();
                    return true;
                }

                if (lastOpenedLarge != purchasedObject.GetInstanceID())
                {
                    lastOpenedLarge = purchasedObject.GetInstanceID();
                    return true;
                }
            }

            if (purchasedObject.name.Contains("GoldChest"))
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

        internal virtual void CheckRoll(CharacterBody body)
        {
            // Implement above.
        }
    }
}
