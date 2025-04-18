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

            internal override void CheckRoll(string chestName)
            {
                Debug.Log("rolling for Scarlet");
                if (chestName.Contains(RequiredChestType))
                {
                    //Medium Chest! Roll to see the result
                    int rnd = UnityEngine.Random.Range(0, 101);

                    Debug.Log("rnd: " + rnd + " Chance: " + Chance);

                    if (rnd <= Chance)
                    {
                        base.Grant();   
                    }
                }
            }

            /*
             		public void RpcGrantAchievement(ServerAchievementIndex serverAchievementIndex)
		{
			LocalUser localUser = this.networkUser.localUser;
			if (localUser != null)
			{
				UserAchievementManager userAchievementManager = AchievementManager.GetUserAchievementManager(localUser);
				if (userAchievementManager == null)
				{
					return;
				}
				userAchievementManager.HandleServerAchievementCompleted(serverAchievementIndex);
			}
		}
             */
        }
    }
}
