using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
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

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);
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

            internal override void CheckRoll(string chestName, CharacterBody body)
            {
                base.CheckRoll(chestName, body);
                if (chestName.Contains(RequiredChestType))
                {
                    int rnd = UnityEngine.Random.Range(0, 101);

                    Debug.Log("scarlet: " + rnd);

                    if (rnd <= Chance)
                    {
                        new AchievementGranterNetworkRequest(this.achievementDef.serverIndex.intValue, body.netId).Send(R2API.Networking.NetworkDestination.Clients);
                    }
                }
            }
        }
    }
}
