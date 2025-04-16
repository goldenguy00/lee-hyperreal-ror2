using RoR2;
using RoR2.Achievements;
using System;
using UnityEngine;


namespace LeeHyperrealMod.Content.Achievements
{
    abstract class BaseGachaUnlockable : BaseAchievement
    {
        public abstract string RequiredCharacterBody { get; }
        public abstract float RequiredDifficultyCoefficient { get; }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Run.onClientGameOverGlobal += OnClientGameOverGlobal;
        }
        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= OnClientGameOverGlobal;
            base.OnBodyRequirementBroken();
        }
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
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

        private CostTypeDef.PayCostResults CostTypeDef_PayCost(On.RoR2.CostTypeDef.orig_PayCost orig, CostTypeDef self, int cost, Interactor activator, UnityEngine.GameObject purchasedObject, Xoroshiro128Plus rng, ItemIndex avoidedItemIndex)
        {
            CostTypeDef.PayCostResults result =  orig(self, cost, activator, purchasedObject, rng, avoidedItemIndex);

            Debug.Log($"Body: {activator.GetComponent<CharacterBody>().baseNameToken}");
            Debug.Log($"Purchased: {purchasedObject.name}");

            //check if the body matches us
            // Check purchased object
            //Roll the dice and see if you earn it

            return result;
        }

        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (!runReport.gameEnding)
            {
                return;
            }
            if (runReport.gameEnding.isWin)
            {
                DifficultyIndex difficultyIndex = runReport.ruleBook.FindDifficulty();
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
                if (difficultyDef != null)
                {

                    bool isDifficulty = difficultyDef.countsAsHardMode && difficultyDef.scalingValue >= RequiredDifficultyCoefficient;
                    bool isInferno = difficultyDef.nameToken == "INFERNO_NAME";
                    bool isEclipse = difficultyIndex >= DifficultyIndex.Eclipse1 && difficultyIndex <= DifficultyIndex.Eclipse8;

                    if (isDifficulty || isInferno || isEclipse)
                    {
                        base.Grant();
                    }
                }
            }
        }
    }
}
