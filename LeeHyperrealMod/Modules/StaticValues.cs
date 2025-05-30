﻿using RoR2.Projectile;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using EntityStates;
using TMPro;
using System.Collections.Generic;

namespace LeeHyperrealMod.Modules
{
    internal static class StaticValues
    {        
        internal static Color bodyColor = new Color(0.4f, 1f, 1f);

        internal static float redBaseHueOffset = 0.6f;
        internal static float orangeBaseHueOffset = 0.539f;
        internal static float yellowBaseHueOffset = 0.478f;
        internal static float greenBaseHueOffset = 0.319f;
        internal static float lightBlueBaseHueOffset = 0.132f;
        internal static float violetBaseHueOffset = -0.126f;
        internal static float pinkBaseHueOffset = -0.271f;
        internal static float defaultBaseHueOffset = 0f;

        internal static List<string> prefix = new List<string> { "Rift", "Void", "Phantom", "Dimension", "Dimensional", "Nether", "Chrono", "Echo", "Reality", "Quantum", "Portal", "Multiverse" };
        internal static List<string> suffix = new List<string> { "Hunter", "Striker", "Roamer", "Vanguard", "Seeker", "Mercenary", "Ranger", "Drifter", "Walker" };

        #region Character Stats
        internal const float baseMoveSpeed = 7f;
        internal const float maxAttackSpeedScaling = 1.35f; // The maximum speedup allowed on moves.
        internal const float inputMaxAttackSpeedOnBody = 2.5f; // The maximum input attack speed that scales with the aforementioned value.

        internal const float maxMoveSpeedScaling = 2.5f; // The maximum movement speed scaling allowed on moves
        internal const float inputMaxMoveSpeedOnBody = 25f; // The maximum input move speed that scales with the aforementioned value.
        internal const float requiredMoveSpeedToSupersprint = 20f;
        #endregion

        #region Primary 1
        internal const float primary1DamageCoefficient = 1f;
        internal const float primary1ProcCoefficient = 1f;
        internal const float primary1PushForce = 300f;
        internal const float primary1HitHopVelocity = 4f;
        internal const float primary1ParryLength = 0.15f; //How long the parry window is openf or
        internal const float primary1ParryTiming = 0.0f; // A percentage of the duration in which the parry starts
        internal const float primary1ParryPauseLength = 0.2f; // How long YOU are stuck in hit pause when a parry is triggered
        internal const float primary1ParryProjectileTimingStart = 0.00f; // Percentage of the duration of the move where a projectile parry can be triggered
        internal const float primary1ParryProjectileTimingEnd = 0.1f; // Percentage of the duration of the move when a projectile parry can be triggered. In this case it's ranging from duration * start to duration * end, this value should never be higher than 1f
        #endregion

        #region Primary 2
        internal const float primary2DamageCoefficient = 0.7f;
        internal const float primary2ProcCoefficient = 1f;
        internal const float primary2PushForce = 300f;
        internal const float primary2HitHopVelocity = 4f;
        #endregion

        #region Primary 3
        internal const float primary3DamageCoefficient = 2.25f;
        internal const float primary3ProcCoefficient = 1f;
        internal const float primary3PushForce = 300f;
        internal const float primary3HitHopVelocity = 4f;
        internal const float primary3ParryLength = 0.55f; //How long the parry window is openf or
        internal const float primary3ParryTiming = 0.03f; // A percentage of the duration in which the parry starts
        internal const float primary3ParryPauseLength = 0.2f; // How long YOU are stuck in hit pause when a parry is triggered
        internal const float primary3ParryProjectileTimingStart = 0.03f; // Percentage of the duration of the move where a projectile parry can be triggered
        internal const float primary3ParryProjectileTimingEnd = 0.193f; // Percentage of the duration of the move when a projectile parry can be triggered. In this case it's ranging from 5% to 10% of the move, this value should never be higher than 1f
        #endregion

        #region Primary 4
        internal const float primary4DamageCoefficient = 0.8f; // This is per tick!
        internal const float primary4ProcCoefficient = 1f;
        internal const float primary4BasePulseRate = 0.2f;
        internal const float primary4BlastRadius = 25f;
        #endregion

        #region Primary 5
        internal const float primary5DamageCoefficient = 6f;
        internal const float primary5ProcCoefficient = 1f;
        internal const float primary5PushForce = 300f;
        internal const float primary5HitHopVelocity = 4f;
        #endregion

        #region Primary Aerial
        internal const float primaryAerialSlamRadius = 5f;
        internal const float primaryAerialMaxDamageMultiplier = 3f;
        internal const float primaryAerialDamageCoefficient = 4f;
        internal const float primaryAerialProcCoefficient = 1f;
        internal const float primaryAerialSlamSpeed = 125f;
        
        internal const float primaryAerialParryLength = 0.1f; //How long the parry window is openf or
        internal const float primaryAerialParryTiming = 0.0f; // A percentage of the duration in which the parry starts
        internal const float primaryAerialParryPauseLength = 0.05f; // How long YOU are stuck in hit pause when a parry is triggered
        internal const float primaryAerialProjectileTimingStart = 0.0f; // Percentage of the duration of the move where a projectile parry can be triggered
        internal const float primaryAerialProjectileTimingEnd = 0.1f; // Percentage of the duration of the move when a projectile parry can be triggered. In this case it's ranging from duration * start to duration * end, this value should never be higher than 1f
        #endregion

        #region Domain Shift
        internal const float forceUpwardsMagnitude = 9f;
        #endregion

        #region Parry Stuff
        internal const float bigParryFreezeRadius = 20f;
        internal const float bigParryFreezeDuration = 1.5f;
        internal const float bigParryLeeFreezeDuration = 0.6f;
        internal const float bigParryHealthFrac = 0.2f;
        internal const float parryProjectileRadius = 3f;
        internal const float parryProjectileDamageMultiplier = 5f;
        internal const int enhancedBulletGrantOnProjectileParry = 1;
        internal const int enhancedBulletGrantOnDamageParry = 1;
        internal const int enhancedBulletGrantOnDamageParryBig = 3;
        #endregion

        #region Orb Controller Values
        internal const float flatIncreaseOrbIncrementor = 0.5f;
        internal const float LimitToGrantOrb = 5f; // Amount that determines when to give orb, smaller = faster.
        internal const float MinimumOrbGrantSpeed = 0.2f; // Cap on limit to grant orb. 0f = basically 1 every frame.
        internal const float flatAmountToGrantOnPrimaryHit = 0.35f;
        internal const float yAxisPositionBrackets = -135f;
        internal const float yAxisPositionBracketsRiskUI = -140f;
        #endregion

        #region Blue Orb
        internal const float blueOrbCoefficient = 2.25f;
        internal const float blueOrbBlastRadius = 8f;
        internal const float blueOrbTripleMultiplier = 4f;
        internal const float blueOrbShotCoefficient = 3f;
        internal const float blueOrbProcCoefficient = 1f;
        #endregion

        #region Domain
        internal const float domainShiftBlastRadius = 18f;
        internal const float domainShiftCoefficient = 8f;
        internal const float domainShiftBulletDamageCoefficient = 4.5f;
        internal const float energyRechargeSpeed = 1f; // Unit per second
        internal const float energyConsumptionSpeed = 7f; // Unit per second
        internal const float energyReturnedPer3ping = 35f; 
        internal const int maxIntuitionStocks = 4;
        #endregion

        #region Yellow Orb
        internal const float yellowOrbDamageCoefficient = 0.75f;
        internal const int yellowOrbBaseHitAmount = 3;
        internal const float yellowOrbProcCoefficient = 1f;
        internal const float yellowOrbTripleMultiplier = 4f;
        internal const float yellowOrbBlastRadius = 8f;

        internal const float yellowOrbFinisherDamageCoefficient = 8f;
        internal const float yellowOrbFinisherProcCoefficient = 1f;
        internal const float yellowOrbFinisherPushForce = 300f;

        internal const float yellowOrbDomainDamageCoefficient = 0.45f;
        internal const float yellowOrbDomainProcCoefficient = 1f;
        internal const float yellowOrbDomainBlastForce = 200f;
        internal const float yellowOrbDomainBlastRadius = 20f;
        internal const int yellowOrbDomainFireCount = 9;
        internal const float yellowOrbDomainTripleMultiplier = 4f;

        internal const float yellowOrbFinisherParryLength = 0.75f; //How long the parry window is openf or
        internal const float yellowOrbFinisherParryTiming = 0.0f; // A percentage of the duration in which the parry starts
        internal const float yellowOrbFinisherParryPauseLength = 0.2f; // How long YOU are stuck in hit pause when a parry is triggered
        internal const float yellowOrbFinisherParryProjectileTimingStart = 0.0f; // Percentage of the duration of the move where a projectile parry can be triggered
        internal const float yellowOrbFinisherParryProjectileTimingEnd = 0.5f; // Percentage of the duration of the move when a projectile parry can be triggered. In this case it's ranging from 0% to 50% of the move, this value should never be higher than 1f
        #endregion

        #region Red Orb
        internal const float redOrbDamageCoefficient = 1f;
        internal const int redOrbBaseHitAmount = 3; // Amount of shots at 1 attack speed
        internal const float redOrbProcCoefficient = 1f;
        internal const float redOrbBulletRange = 256f;
        internal const float redOrbBulletForce = 800f;
        internal const float redOrbTripleMultiplier = 4f;

        internal const float redOrbFinisherDamageCoefficient = 4f;
        internal const float redOrbFinisherProcCoefficient = 1f;
        internal const float redOrbFinisherBulletRange = 256f;
        internal const float redOrbFinisherBulletForce = 800f;

        internal const float redOrbDomainDamageCoefficient = 1.55f;
        internal const float redOrbDomainBlastRadius = 10f;
        internal const int redOrbDomainFireCount = 3;
        internal const float redOrbDomainProcCoefficient = 1f;
        internal const float redOrbDomainBlastForce = 1000f;
        internal const float redOrbDomainBaseFireInterval = 0.2f;
        internal const float redOrbDomainTripleMultiplier = 4f;
        #endregion

        #region Ultimate
        internal const float ultimateDamageCoefficient = 65f;
        internal const float ultimateProcCoefficient = 1f;
        internal const float ultimateBlastRadius = 22f;
        internal const float ultimateFreezeDuration = 6.9f;
        internal const int ultimateFinalBlastHitCount = 12;

        internal const float ultimateDomainMiniDamageCoefficient = 2f;
        internal const float ultimateDomainDamageCoefficient = 15f;
        internal const float ultimateDomainBlastRadius = 40f;
        internal const int ultimateDomainFireCount = 3;
        internal const float ultimateDomainDuration = 4f;
        #endregion

        #region Invincibility Health
        internal static Color blueInvincibility = new Color(c(104), c(244), c(255), c(255));
        internal static Color parryInvincibility = new Color(c(255), c(139), c(232), c(255));
        #endregion

        #region Snipe
        internal const float snipeProcCoefficient = 1f;
        internal const float snipeRange = 1000f;
        internal const float snipeDamageCoefficient = 4f;
        internal const float snipeForce = 100f;
        internal const float snipeRecoil = 4f;
        internal const float empoweredBulletMultiplier = 2f;
        #endregion

        #region Custom Item Notifications
        public class CustomItemEffect(string titleToken, string descToken)
        {
            public string titleToken = titleToken;
            public string descToken = descToken;
        }

        internal static Dictionary<ItemIndex, CustomItemEffect> itemKeyValueNotificationPairs = [];
        internal static Dictionary<EquipmentIndex, CustomItemEffect> equipmentKeyValueNotificationPairs = [];

        [SystemInitializer([typeof(ItemCatalog)])]
        internal static void AddNotificationItemPairs()
        {
            string prefix = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_";
            itemKeyValueNotificationPairs[RoR2Content.Items.SecondarySkillMagazine.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_BACKUPMAG_DESC");
            itemKeyValueNotificationPairs[RoR2Content.Items.AlienHead.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_ALIEN_HEAD_DESC");
            itemKeyValueNotificationPairs[RoR2Content.Items.LunarBadLuck.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_PURITY_DESC");
            itemKeyValueNotificationPairs[RoR2Content.Items.Syringe.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_ATTACK_SPEED_DESC");
            itemKeyValueNotificationPairs[DLC1Content.Items.AttackSpeedAndMoveSpeed.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_ATTACK_SPEED_DESC");
            itemKeyValueNotificationPairs[RoR2Content.Items.EnergizedOnEquipmentUse.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_ATTACK_SPEED_DESC");
            itemKeyValueNotificationPairs[RoR2Content.Items.AttackSpeedOnCrit.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_ATTACK_SPEED_DESC");
            itemKeyValueNotificationPairs[DLC2Content.Items.IncreasePrimaryDamage.itemIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_LUMINOUS_SHOT_DESC");
        }

        [SystemInitializer([typeof(EquipmentCatalog)])]
        internal static void AddNotificationEquipmentPairs()
        {
            string prefix = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_";
            equipmentKeyValueNotificationPairs[RoR2Content.Equipment.TeamWarCry.equipmentIndex] = new CustomItemEffect($"{prefix}ITEM_EFFECT_TITLE", $"{prefix}ITEM_EFFECT_ATTACK_SPEED_DESC");
        }
        #endregion

        #region Blacklisted states to not freeze
        //If the ESM is playing one of these states, don't freeze the esm.
        internal static List<Type> BLACKLIST_STATES = new List<Type>();
        public static void AddBlacklistStates()
        {
            BLACKLIST_STATES.Add(typeof(EntityStates.BrotherMonster.SpellChannelState));
            BLACKLIST_STATES.Add(typeof(EntityStates.BrotherMonster.SpellChannelEnterState));
            BLACKLIST_STATES.Add(typeof(EntityStates.BrotherMonster.SpellChannelExitState));
        }
        #endregion

        #region Static functions
        public static float c(int val) 
        {
            return (float)val / 255f;
        }

        public static Vector3 CheckDirection(Vector3 moveVector, Ray aimRay)
        {
            Vector3 outputVector = new Vector3();
            if (moveVector == Vector3.zero)
            {
                outputVector = new Vector3(0, 0, 0); // Dodge backwards.

                return outputVector;
            }

            Vector3 direction = new Vector3(aimRay.direction.x, 0, aimRay.direction.z);
            Vector3 rotatedVector = Quaternion.AngleAxis(90, Vector3.up) * direction;

            if (Vector3.Dot(moveVector, aimRay.direction) >= 0.833f) 
            {
                outputVector = new Vector3(0, 0, 1);
                return outputVector;
            }

            if (Vector3.Dot(direction, moveVector) <= -0.833f)
            {
                outputVector = new Vector3(0, 0, 0); // dodge backwards
                return outputVector;
            }

            //Should be rotated 90 degrees
            if (Vector3.Dot(rotatedVector, moveVector) >= 0.833f)
            {
                // It's in the right direction
                outputVector = new Vector3(1, 0, 0);
                return outputVector;
            }
            else if( Vector3.Dot(rotatedVector, moveVector) <= -0.833f)
            {
                outputVector = new Vector3(-1, 0, 0);
                return outputVector;
            }

            //Cases where strict input failed.
            float dotProduct = Vector3.Dot(rotatedVector, moveVector);

            if (dotProduct >= 0f)
            {
                // Right prioritised
                return new Vector3(1, 0, 0);
            }
            else if (dotProduct < 0f) 
            {
                return new Vector3(-1, 0, 0);
            }


            return outputVector;
        }

        public static float ScaleAttackSpeed(float inputAttackSpeed) 
        {
            float rangeBeforeConversion = Mathf.InverseLerp(1f, inputMaxAttackSpeedOnBody, inputAttackSpeed);
            return Mathf.Lerp(1f, maxAttackSpeedScaling, rangeBeforeConversion);     
        }

        public static float ScaleMoveSpeed(float inputMoveSpeed) 
        {
            float rangeBeforeConversion = Mathf.InverseLerp(1f, inputMaxMoveSpeedOnBody, inputMoveSpeed);
            return Mathf.Lerp(1f, maxMoveSpeedScaling, rangeBeforeConversion);
        }
        #endregion
    }
}