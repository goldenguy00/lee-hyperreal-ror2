﻿using R2API;
using System;
using static LeeHyperrealMod.Modules.Helpers;
using static LeeHyperrealMod.Modules.StaticValues;
using static LeeHyperrealMod.Modules.Config;

namespace LeeHyperrealMod.Modules
{
    internal static class Tokens
    {
        internal static string GenerateName() 
        {
            Random rnd = new Random();
            int prefixIndex = rnd.Next(Modules.StaticValues.prefix.Count);
            string prefix = Modules.StaticValues.prefix[prefixIndex];

            int suffixIndex = rnd.Next(Modules.StaticValues.suffix.Count);
            string suffix = Modules.StaticValues.suffix[suffixIndex];
            
            return $"{prefix} {suffix}";
        }

        internal static void AddTokens()
        {
            string generatedName = GenerateName();

            #region Lee: Hyperreal
            string prefix = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_";

            string desc = $"{Lee("Lee: Hyperreal", generatedName)} is a highly technical grounded character with hard to master gameplay. He entirely focuses on damage to the detriment of his survivability." +
                Environment.NewLine + Environment.NewLine + $"<!> {UtilDesc("[3-ping]s")} do {DmgDesc("4 times")} the damage compared to 1 or 2-pings of the same color so always try to manipulate the orb list to do {UtilDesc("[3-ping]s")}." +
                Environment.NewLine + Environment.NewLine + $"<!> On a successful parry with {UtilDesc("[Armament Barrage]")}, you can instantly parry again. With decent timing, you can reduce/entirely negate damage." +
                $" Projectiles parried will be reflected with their damage multiplied. Hitting enemies with this ability will regen orbs faster." +
                Environment.NewLine + Environment.NewLine + $"<!> Entering the {UtilDesc("[Hypermatrix]")} is your time to shine. Delete those bosses but watch out for the end-lag on orb skills in Hypermatrix." +
                Environment.NewLine + Environment.NewLine + $"<!> Try to keep your ammo counters filled. Get {UtilDesc("[Enhanced Bullets]")} from parrying, and {UtilDesc("[Coloured Bullets]")} from {UtilDesc("[3-ping]s")} " +
                Environment.NewLine + Environment.NewLine + $"<!> {UtilDesc("[Snipe Stance]")} is your highest damaging tool, even more so with {UtilDesc("[Enhanced Bullets]")}. However, you’re vulnerable in this stance, so use this skill wisely." +
                Environment.NewLine + Environment.NewLine + $"<!> {UtilDesc("[End of Time]")} can decimate a huge radius of enemies with its unlimited range and huge radius. But be mindful of its long cooldown.";

            string lore = "\"The tests are going well?\"" +
                Environment.NewLine + Environment.NewLine + "Crisp footsteps from expensive shoes sounded too loud in the medically sterile chambers." +
                Environment.NewLine + Environment.NewLine + "\"Yes, sir. Did you feel that just now?\"" +
                Environment.NewLine + Environment.NewLine + "The deep hum of energy that swept through the disinfected floors vibrated against the shoes, the expensive pair and the worn-down pair." +
                Environment.NewLine + Environment.NewLine + "\"That was a weapons test, sir. Handheld weapon.\"" +
                Environment.NewLine + Environment.NewLine + "That put a falter in the footsteps. \"Handheld?\"" +
                Environment.NewLine + Environment.NewLine + "\"Yes sir. It’s come a long way since you were here last.\"" +
                Environment.NewLine + Environment.NewLine + "\"Hrm. Well, the weapons won’t mean much if the subject is another defect. Are we close?\"" +
                Environment.NewLine + Environment.NewLine + "\"Yes sir, right this way.\"" +
                Environment.NewLine + Environment.NewLine + "The shabby, chemical-stained and malfunction-burnt shoes pulled ahead, and fumbling hands reached for a card he’d been gifted for the day. " +
                "They knew he’d be a nervous wreck, but it fed their guest’s ego, which meant he had a happier visit." +
                Environment.NewLine + Environment.NewLine + "A single swipe, a retinal scan he’d just been given access to, and they were in a room neither of them had seen before." +
                Environment.NewLine + Environment.NewLine + "\"It’s… is it an it? Or a he?\"" +
                Environment.NewLine + Environment.NewLine + "Words from a memorized infographic filled his head. \"It’s both, sir. The biosynthesis worked perfectly, unlike previous tests. It’s a perfect combination.\"" +
                Environment.NewLine + Environment.NewLine + "\"I see. So it is organic?\"" +
                Environment.NewLine + Environment.NewLine + "\"It’s cybernetic, but you are correct. And it wasn’t easy, sir. Like growing a square watermelon with natural selection.\"" +
                Environment.NewLine + Environment.NewLine + "\"Yes yes, I get your point. The funds will be transferred on my return.\"" +
                Environment.NewLine + Environment.NewLine + "\"Thank you, sir.\"" +
                Environment.NewLine + Environment.NewLine + "The expensive shoes turned to leave, but were stopped by a sudden thought. \"Say… you’ve tested it, right?\"" +
                Environment.NewLine + Environment.NewLine + "A nervous swallow, an unexpected question. \"Hm? Oh, uh— no, I guess we haven’t.\"" +
                Environment.NewLine + Environment.NewLine + "\"Hm. Well fix that, or else I’ll have to view it as a liability. You don’t want to repeat the last mistake, do you?\"" +
                Environment.NewLine + Environment.NewLine + "The bitter words made the shabby shoes falter, and an access card trembled inside an acid-burned hand. \"No, Mr. Wade. No we don’t. We’ll send you footage of the live testing, for your decision.\"" +
                Environment.NewLine + Environment.NewLine + "An hour later, a large case was stowed in the loading bay, and a UES high priority boarding card let a pair of shabby shoes and acrid burns onto the Safe Travels.";

            string outro = "..and so he left, still no closer to his true reality.";
            string outroFailure = "..and so he vanished, with a mission unfulfilled.";


            LanguageAPI.Add(prefix + "NAME", $"{Lee("Lee: Hyperreal", generatedName, Modules.Config.isBlueName.Value)}");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Hypermatrix Traverser");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);
            LanguageAPI.Add(prefix + "SURVIVOR_POD_EXIT", "Traverse Dimension");

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Lee: Hyperreal");
            LanguageAPI.Add(prefix + "ALT_SKIN_NAME", "Lee: Real");
            LanguageAPI.Add(prefix + "SCARLET_SKIN_NAME", "Lee: Scarlet Redeemer");
            LanguageAPI.Add(prefix + "PROSPECTOR_SKIN_NAME", $"{generatedName}: Prospector");
            LanguageAPI.Add(prefix + "PROSPECTOR_ALT_SKIN_NAME", $"{generatedName}: Comrade");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_ORB_AND_AMMO_NAME", "Orb and Ammo System");
            LanguageAPI.Add(prefix + "PASSIVE_ORB_AND_AMMO_DESCRIPTION", "" +
                $"{Lee("Lee: Hyperreal", generatedName)} uses {UtilDesc("[Orbs]")} that allow the execution of unique skills. " +
                $"His {UtilDesc("[Ammo Counter]")} grants unique effects for {UtilDesc("[Snipe Stance]")}.");

            LanguageAPI.Add(prefix + "PASSIVE_DOMAIN_NAME", "Hypermatrix System");
            LanguageAPI.Add(prefix + "PASSIVE_DOMAIN_DESCRIPTION", "" +
                $"{Lee("Lee: Hyperreal", generatedName)} has access to the {UtilDesc("[Hypermatrix]")}, by holding the Primary button" +
                $" down with a full {UtilDesc("[Power Gauge]")}." +
                $" {Lee("Lee: Hyperreal", generatedName)} gains more hits in place of attack speed, however Snipe Stance scales normally.");

            LanguageAPI.Add(prefix + "PASSIVE_VFX_COLOR_NAME", "VFX Color");

            LanguageAPI.Add(prefix + "PASSIVE_DEFAULT_VFX_NAME", "Default VFX");
            LanguageAPI.Add(prefix + "PASSIVE_DEFAULT_VFX_DESCRIPTION", "Sets the VFX colour to the default based on the skin choice");
            LanguageAPI.Add(prefix + "PASSIVE_RED_VFX_NAME", "Red VFX");
            LanguageAPI.Add(prefix + "PASSIVE_RED_VFX_DESCRIPTION", "Sets the VFX colour to Red");
            LanguageAPI.Add(prefix + "PASSIVE_ORANGE_VFX_NAME", "Orange VFX");
            LanguageAPI.Add(prefix + "PASSIVE_ORANGE_VFX_DESCRIPTION", "Sets the VFX colour to Orange");
            LanguageAPI.Add(prefix + "PASSIVE_YELLOW_VFX_NAME", "Yellow VFX");
            LanguageAPI.Add(prefix + "PASSIVE_YELLOW_VFX_DESCRIPTION", "Sets the VFX colour to Yellow");
            LanguageAPI.Add(prefix + "PASSIVE_GREEN_VFX_NAME", "Green VFX");
            LanguageAPI.Add(prefix + "PASSIVE_GREEN_VFX_DESCRIPTION", "Sets the VFX colour to Green");
            LanguageAPI.Add(prefix + "PASSIVE_BLUE_VFX_NAME", "Blue VFX");
            LanguageAPI.Add(prefix + "PASSIVE_BLUE_VFX_DESCRIPTION", "Sets the VFX colour to Blue");
            LanguageAPI.Add(prefix + "PASSIVE_LIGHTBLUE_VFX_NAME", "Light Blue VFX");
            LanguageAPI.Add(prefix + "PASSIVE_LIGHTBLUE_VFX_DESCRIPTION", "Sets the VFX colour to Light Blue");
            LanguageAPI.Add(prefix + "PASSIVE_VIOLET_VFX_NAME", "Violet VFX");
            LanguageAPI.Add(prefix + "PASSIVE_VIOLET_VFX_DESCRIPTION", "Sets the VFX colour to Violet");
            LanguageAPI.Add(prefix + "PASSIVE_PINK_VFX_NAME", "Pink VFX");
            LanguageAPI.Add(prefix + "PASSIVE_PINK_VFX_DESCRIPTION", "Sets the VFX colour to Pink");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_NAME", "Armament Barrage");
            LanguageAPI.Add(prefix + "PRIMARY_DESCRIPTION", "Launch a 5-string-combo attack. On hit 1 and 3, " +
                $"perform a {UtilDesc("[Parry]")} active for a short time. " +
                $"In the air, slam down, dealing {DmgDesc($"{primaryAerialDamageCoefficient * 100}% damage")}, increasing up to {DmgDesc($"{primaryAerialMaxDamageMultiplier}x times,")} based on distance travelled. " +
                $"Hold down at anytime with a full {UtilDesc("[Power Gauge]")} to enter the {UtilDesc("[Hypermatrix]")}");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SNIPE_NAME", "Snipe");
            LanguageAPI.Add(prefix + "SECONDARY_SNIPE_DESCRIPTION", "" +
                $"Shoot a bullet for {DmgDesc($"{snipeDamageCoefficient * 100f}% damage")}.");
            LanguageAPI.Add(prefix + "EXIT_SNIPE_NAME", "Exit Snipe Stance");
            LanguageAPI.Add(prefix + "EXIT_SNIPE_DESCRIPTION", "" +
                $"Exit {UtilDesc("[Snipe Stance]")}, allowing you to move again.");
            LanguageAPI.Add(prefix + "ENTER_SNIPE_NAME", "Snipe Stance");
            LanguageAPI.Add(prefix + "ENTER_SNIPE_DESCRIPTION", "" +
                $"Enter {UtilDesc("[Snipe Stance]")}, locking you in place, allowing you to Snipe using Primary" +
                $" for {DmgDesc($"{snipeDamageCoefficient * 100f}% damage")}.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "DASH_NAME", "Reality Travel");
            LanguageAPI.Add(prefix + "DASH_DESCRIPTION", "" +
                $"{UtilDesc("Dodge")}, turning invincible for a short time.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "ULTIMATE_NAME", "End of Time");
            LanguageAPI.Add(prefix + "ULTIMATE_DESCRIPTION", "" +
                $"Turn invincible, pulling out a cannon that blasts enemies in a wide radius for {DmgDesc($"{ultimateDamageCoefficient * 100f}% damage")} after a short windup. " +
                $"Move changes while in the {UtilDesc("[Hypermatrix]")}.");

            LanguageAPI.Add(prefix + "ULTIMATE_DOMAIN_NAME", "Collapsing Realm");
            LanguageAPI.Add(prefix + "ULTIMATE_DOMAIN_DESCRIPTION", "" +
                $"Collapse the {UtilDesc("[Hypermatrix]")}, turning invincible for a short time and dealing {DmgDesc($"{ultimateDomainDamageCoefficient * 100f}% damage")} in your wake." +
                $"Each 3-Ping during hypermatrix adds 3 hits of 200% damage to {UtilDesc("[Collapsing Realm]")}");
            #endregion

            #region Item Effects
            LanguageAPI.Add(prefix + "ITEM_EFFECT_TITLE", "Hyper effect");
            LanguageAPI.Add(prefix + "ITEM_EFFECT_BACKUPMAG_DESC", $"{Lee("Lee: Hyperreal", generatedName)} gains {UtilDesc("+1")} {Stack("(+1 per stack)")} Enhanced bullet(s) on each successful {UtilDesc("[Parry]")}");
            LanguageAPI.Add(prefix + "ITEM_EFFECT_ALIEN_HEAD_DESC", $"{Lee("Lee: Hyperreal", generatedName)}'s orbs charge {UtilDesc("25% faster")}.");
            LanguageAPI.Add(prefix + "ITEM_EFFECT_PURITY_DESC", $"{Lee("Lee: Hyperreal", generatedName)}'s orb recharge rate is reduced by {UtilDesc("2")} {Stack("(+1 per stack)")} seconds");
            LanguageAPI.Add(prefix + "ITEM_EFFECT_ATTACK_SPEED_DESC", $"{Lee("Lee: Hyperreal", generatedName)}'s primary gains more {UtilDesc("hits")} instead of attack speed. E.g. 250% attack speed = 2 hits and a half damage hit.");
            LanguageAPI.Add(prefix + "ITEM_EFFECT_LUMINOUS_SHOT_DESC", $"{Lee("Lee: Hyperreal", generatedName)}'s Snipe Stance shots give {UtilDesc("1 Stack")} per shot instead of using Secondary");
            #endregion

            #region Keywords
            LanguageAPI.Add(prefix + "KEYWORD_ORBS",
                $"{Keyword("Orb System")}" +
                Environment.NewLine +
                $"{Lee("Lee: Hyperreal", generatedName)} uses 3 different coloured Orbs to use extra skills." +
                Environment.NewLine +
                $"In Simple Mode, activate {BlueOrb()}, {RedOrb()} and {YellowOrb()} orbs using the keys " +
                $"{UserSetting($"{blueOrbTrigger.Value}, {redOrbTrigger.Value}, {yellowOrbTrigger.Value}")} respectively, to use the first group of coloured orbs going from left to right. " +
                Environment.NewLine +
                $"Using 3 adjacent orbs of the same color, known as a {UtilDesc($"[3-ping]")}, " +
                $"will increase the orb's skill damage by {DmgDesc($"{yellowOrbTripleMultiplier}x times")}, while also granting extra attacks for {RedOrb()} and {YellowOrb()} Orbs.");
            LanguageAPI.Add(prefix + "KEYWORD_AMMO",
                $"{Keyword("Ammo System")}" +
                Environment.NewLine + $"" +
                $"{Lee("Lee: Hyperreal", generatedName)} can store and use a variety of {UtilDesc("Ammo")}, indicated above the {UtilDesc("[Power Gauge]")}." +
                Environment.NewLine +
                $"Using a {UtilDesc("[3-ping]")} will store a {UtilDesc("[Coloured Bullet]")}, which can be used in the {UtilDesc("[Hypermatrix]")} to add 3 Orbs of that shots colour back into your {UtilDesc("[Orb System]")} by Sniping during it." +
                Environment.NewLine +
                $"Upon a successful {UtilDesc("[Parry]")}, {UtilDesc("[Enhancedd Bullet]'s")} are granted, increasing the damage of your next shot in {UtilDesc("[Snipe Stance]")} by {DmgDesc($"{empoweredBulletMultiplier}x times.  ")}");
            LanguageAPI.Add(prefix + "KEYWORD_SNIPE_STANCE",
                $"{Keyword("Snipe Stance")}" +
                Environment.NewLine + $"Enter a stance which immobilises you, changing your {UserSetting("Primary Skill")} to a Sniping shot that deals {DmgDesc($"{snipeDamageCoefficient * 100f}% Damage")} per shot." +
                Environment.NewLine + $"Using {UserSetting("Utility Skill")} without any directional input allows you to dodge backwards while firing a bullet that deals {DmgDesc($"{snipeDamageCoefficient * 100f}% Damage")}. Using this skill otherwise cancels the stance.");
            LanguageAPI.Add(prefix + "KEYWORD_POWER_GAUGE",
                $"{Keyword("Power Gauge")}" +
                Environment.NewLine + $"The Power Gauge situated above the Health gauge determines when you can enter the {UtilDesc("[Hypermatrix]")}, and how long left before exiting. Hold the {UserSetting("Primary Skill")} to enter the {UtilDesc("[Hypermatrix]")}.");
            LanguageAPI.Add(prefix + "KEYWORD_PARRY",
                $"{Keyword("Parry")}" +
                Environment.NewLine + $"Upon a successful parry, stun the enemy that dealt the damage if they can be stunned. Any projectile parried will be shot back in the direction you aim at. A successful parry resets {UtilDesc("[Armament Barrage]")}, starting from the move used to parry again.");
            LanguageAPI.Add(prefix + "KEYWORD_DOMAIN",
                $"{Keyword("Hypermatrix System")}" +
                Environment.NewLine +
                $"Above your Health is the {UtilDesc("[Hypermatrix System Gauge]")}. When the gauge is full, holding {UserSetting("Primary Skill")} at any time in {UtilDesc("[Armament Barrage]")} will transition him into the {UtilDesc("[Hypermatrix]")}. " +
                Environment.NewLine +
                $"While in this state, {RedOrb()} and {YellowOrb()} orbs change entirely, and your Special becomes {UtilDesc("[Collapsing Realm]")}." +
                Environment.NewLine +
                $"If the gauge runs out while in this state, {UtilDesc("[Collapsing Realm]")} will trigger automatically, exiting you out of the {UtilDesc("[Hypermatrix]")}.");

            LanguageAPI.Add(prefix + "KEYWORD_DOMAIN_ULT",
                $"{Keyword("Collapsing Realm")}" +
                Environment.NewLine + 
                $"Collapse the {UtilDesc("[Hypermatrix]")}, turning invincible for a short time and dealing {DmgDesc($"{ultimateDomainDamageCoefficient * 100f}% damage")} in your wake. add 3 hits of {DmgDesc($"{ultimateDomainMiniDamageCoefficient * 100f}% damage every time you use a {UtilDesc($"[3-ping]")} in hypermatrix. max of 4 times for 12 hits extra.")}");

            #endregion

            #region Mithrix Response
            LanguageAPI.Add("LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ENTRY", "How did you get here?");
            LanguageAPI.Add("LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ON_DEATH_1", "Pray in your own dimension.");
            LanguageAPI.Add("LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ON_DEATH_2", "Another reality won't save you from me.");
            LanguageAPI.Add("LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ON_DEATH_3", "I'll crush every copy of you.");
            #endregion

            #region Achievements
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Mastery");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "BLUESKIN_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Real");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "BLUESKIN_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, beat the game on Monsoon with Transcendence, or Roll for a 5% chance from a Medium chest or higher tier.");
            LanguageAPI.Add(prefix + "BLUESKIN_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Real");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "SCARLET_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Scarlet");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "SCARLET_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, beat the game on Monsoon with no watches and bleed items, or Roll for a 5% chance from a Legendary chest.");
            LanguageAPI.Add(prefix + "SCARLET_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Scarlet");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "YELLOW_VFX_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Yellow VFX");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "YELLOW_VFX_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, beat the game on Monsoon while holding 5 or more Boss Tier Items, or Roll any chest for a 2% chance.");
            LanguageAPI.Add(prefix + "YELLOW_VFX_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Yellow VFX");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "GREEN_VFX_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Green VFX");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "GREEN_VFX_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, beat the game on Monsoon while holding 15 or more Healing items (RoR2 items only), or Roll any chest for a 2% chance.");
            LanguageAPI.Add(prefix + "GREEN_VFX_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Green VFX");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "PINK_VFX_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Pink VFX");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "PINK_VFX_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, kill the Teleporter boss with End of Time at max strength, or Roll any chest for a 2% chance.");
            LanguageAPI.Add(prefix + "PINK_VFX_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Pink VFX");

            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "VIOLET_VFX_ACHIEVEMENT_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Violet VFX");
            LanguageAPI.Add("ACHIEVEMENT_" + prefix + "VIOLET_VFX_ACHIEVEMENT_DESCRIPTION", $"As {Lee("Lee: Hyperreal", generatedName)}, beat the game on Eclipse 8, or Roll for a 1% chance from a Legendary chest.");
            LanguageAPI.Add(prefix + "VIOLET_VFX_UNLOCKABLE_NAME", $"{Lee("Lee: Hyperreal", generatedName)}: Violet VFX");
            #endregion
            #endregion
        }
    }
}