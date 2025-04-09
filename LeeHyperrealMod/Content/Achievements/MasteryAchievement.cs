using RoR2;

namespace LeeHyperrealMod.Modules.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10)]
    internal class MasteryAchievement: BaseMasteryUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_MASTERYUNLOCKABLE_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_MASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        //public const string prerequisiteAchievementIdentifier = ArsonistUnlockable.identifier;

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(RequiredCharacterBody);
        }

    }
}