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
        public static UnlockableDef scarletUnlockableDef;
        public static UnlockableDef yellowVFXUnlockableDef;
        public static UnlockableDef greenVFXUnlockableDef;
        public static UnlockableDef pinkVFXUnlockableDef;
        public static UnlockableDef violetVFXUnlockableDef;

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

            scarletUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                ScarletAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(ScarletAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("texScarletSkin")
            );

            yellowVFXUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                YellowVFXAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(YellowVFXAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("VFXIconYellow")
            );

            greenVFXUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                GreenVFXAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(GreenVFXAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("VFXIconGreen")
            );

            pinkVFXUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                PinkVFXAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(PinkVFXAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("VFXIconPink")
            );

            violetVFXUnlockableDef = Modules.ContentPacks.CreateAndAddUnlockbleDef(
                VioletVFXAchievement.unlockableidentifier,
                Modules.Helpers.GetAchievementNameToken(VioletVFXAchievement.identifier),
                Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("VFXIconViolet")
            );
        }
    }
}
