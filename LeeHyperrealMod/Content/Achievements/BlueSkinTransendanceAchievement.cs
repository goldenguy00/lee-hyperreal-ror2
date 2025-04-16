using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeeHyperrealMod.Content.Achievements
{
    [RegisterAchievement(identifier, unlockableidentifier, null, 10)]
    class BlueSkinTransendanceAchievement : BaseGachaUnlockable
    {
        public const string identifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_BLUESKIN_ACHIEVEMENT";
        public const string unlockableidentifier = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_BLUESKIN_ACHIEVEMENT_ID";

        public override string RequiredCharacterBody => "LeeHyperrealBody";

        public override float RequiredDifficultyCoefficient => 3;
    }
}
