using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10, typeof(VioletVFXServerAchievement))]
    class VioletVFXAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_VIOLET_VFX_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_VIOLET_VFX_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }

        public override bool CheckInventory(RunReport.PlayerInfo info)
        {
            //avoiding inventory check
            return false;
        }


        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                bool eclipseDifficultyCheck = (difficultyDef.nameToken == "ECLIPSE_8_NAME");

                if (difficultyDef != null && eclipseDifficultyCheck)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }


        internal class VioletVFXServerAchievement : BaseGachaServerAchievement
        {
            public override int Chance => 1;
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
