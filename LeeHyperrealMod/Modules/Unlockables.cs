using LeeHyperrealMod.Content.Achievements;
using LeeHyperrealMod.Modules.Achievements;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.Modules
{
    internal class Unlockables
    {
        public static UnlockableDef masteryUnlockableDef;
        public static UnlockableDef blueSkinUnlockableDef;

        public static void Initialize() 
        {
            masteryUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                MasteryAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(MasteryAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("ProspectorSkinRedIcon")
            );

            blueSkinUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                BlueSkinTransendanceAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(BlueSkinTransendanceAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("texCloneSkin")
            );
        }
    }
}
