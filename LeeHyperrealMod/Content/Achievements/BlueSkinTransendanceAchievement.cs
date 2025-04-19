using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10, typeof(BlueSkinTransendanceServerAchievement))]
    class BlueSkinTransendanceAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_BLUESKIN_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_BLUESKIN_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public override void OnInstall()
        {
            base.OnInstall();
            base.SetServerTracked(true);
        }

        internal class BlueSkinTransendanceServerAchievement : BaseGachaServerAchievement
        {
            public override int Chance => 5;
            public override string RequiredChestType => "Chest2";

            internal override void CheckRoll(string chestName, CharacterBody body)
            {
                Debug.Log("rolling for Blue");
                if (chestName.Contains(RequiredChestType))
                {
                    //Medium Chest! Roll to see the result
                    int rnd = UnityEngine.Random.Range(0, 101);

                    Debug.Log("rnd: " + rnd + " Chance: " + Chance);

                    if (rnd <= Chance)
                    {
                        new AchievementGranterNetworkRequest(this.achievementDef.serverIndex.intValue, body.netId).Send(R2API.Networking.NetworkDestination.Clients);
                    }
                }
            }
        }
    }
}
