using RoR2;
using RoR2.Achievements;

namespace LeeHyperrealMod.Content.Achievements
{
    internal abstract class BaseGachaServerAchievement : BaseServerAchievement
    {

        public static int lastOpenedItem = 0;
        public abstract string RequiredChestType { get; }
        public abstract int Chance { get; }

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
        private CostTypeDef.PayCostResults CostTypeDef_PayCost(On.RoR2.CostTypeDef.orig_PayCost orig, CostTypeDef self, int cost, Interactor activator, UnityEngine.GameObject purchasedObject, Xoroshiro128Plus rng, ItemIndex avoidedItemIndex)
        {
            CostTypeDef.PayCostResults result = orig(self, cost, activator, purchasedObject, rng, avoidedItemIndex);
            bool allowRoll = false;

            //check if the body matches us
            // Check purchased object
            //Roll the dice and see if you earn it
            if (activator.GetComponent<CharacterBody>().baseNameToken != LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_NAME")
            {
                // Exit
                return result;
            }


            if (lastOpenedItem == 0)
            {
                lastOpenedItem = purchasedObject.GetInstanceID();
                allowRoll = true;
            }

            if (lastOpenedItem != purchasedObject.GetInstanceID() && !allowRoll)
            {
                lastOpenedItem = purchasedObject.GetInstanceID();
                allowRoll = true;
            }

            if (allowRoll)
            {
                //Roll the dice and see what you get.
                CheckRoll(purchasedObject.name);
            }

            return result;
        }

        internal virtual void CheckRoll(string chestName)
        {
            // Implement above.
        }
    }
}
