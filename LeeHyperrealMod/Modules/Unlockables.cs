using LeeHyperrealMod.Modules.Achievements;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.Modules
{
    internal class Unlockables
    {
        public static UnlockableDef masteryUnlockableDef;

        public static void Initialize() 
        {
            masteryUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                MasteryAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(MasteryAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("ProspectorSkinRedIcon")
            );
        }
    }
}
