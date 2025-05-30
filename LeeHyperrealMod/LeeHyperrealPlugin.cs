﻿using BepInEx;
using BepInEx.Bootstrap;
using LeeHyperrealMod.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using EmotesAPI;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using ShaderSwapper;
using System.Collections.ObjectModel;
using R2API;
using System;
using MonoMod.RuntimeDetour;
using RoR2.UI;
using System.Runtime.CompilerServices;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace LeeHyperrealMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.prefab", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.language", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.networking", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.unlockable", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.bepis.r2api.damagetype", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.KingEnderBrine.ExtraSkillSlots", BepInDependency.DependencyFlags.HardDependency)]

    [BepInDependency("com.johnedwa.RTAutoSprintEx", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("bubbet.riskui", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TheTimeSweeper.BetterHudLite", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]


    public class LeeHyperrealPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.PopcornFactory.LeeHyperrealMod";
        public const string MODNAME = "LeeHyperrealMod";
        public const string MODVERSION = "2.1.0";
        
        public const string DEVELOPER_PREFIX = "POPCORN";

        public static LeeHyperrealPlugin instance;
        public static PluginInfo PInfo { get; private set; }
        public static bool isControllerCheck = false;
        public static bool isRiskUIInstalled = false;
        public static bool isBetterHudInstalled = false;
        public static bool isHunkHudInstalled => Chainloader.PluginInfos.ContainsKey("com.public_ParticleSystem.HunkHud");
        public static bool isAutoSprintActive = false;
        private static Hook AddBankAfterAKSoundEngineInit;

        private void Awake()
        {
            instance = this;
            PInfo = Info;
            Log.Init(Logger);

            if (Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ExtraSkillSlots"))
            {
                LeeHyperrealPlugin.isControllerCheck = true;
            }

            Modules.LeeHyperrealAssets.Initialize(); // load assets and read config
            base.StartCoroutine(Modules.LeeHyperrealAssets.mainAssetBundle.UpgradeStubbedShadersAsync());
            Modules.ParticleAssets.Initialize();
            Modules.Config.ReadConfig();
            Modules.Damage.SetupModdedDamageTypes();

            if (Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                Modules.Config.SetupRiskOfOptions();
                Modules.Config.OnChangeHooks();
            }
            if (Chainloader.PluginInfos.ContainsKey("bubbet.riskui"))
            {
                isRiskUIInstalled = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.BetterHudLite")) 
            {
                isBetterHudInstalled = true;
            }
            if (Chainloader.PluginInfos.ContainsKey("com.johnedwa.RTAutoSprintEx")) 
            {
                isAutoSprintActive = true;
            }

            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            Modules.LeeHyperrealAssets.LatePopulateAssets();
            Modules.StaticValues.AddBlacklistStates();
            Modules.Unlockables.Initialize();
            // survivor initialization
            new LeeHyperreal().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();


            SetupNetworkMessages();
            Hook();
        }

        private void Start() 
        {
            AddBankAfterAKSoundEngineInit = new Hook(
                typeof(AkSoundEngineInitialization).GetMethodCached(nameof(AkSoundEngineInitialization.InitializeSoundEngine)),
                typeof(LeeHyperrealPlugin).GetMethodCached(nameof(AddBank)));

        }

        private void SetupNetworkMessages()
        {
            NetworkingAPI.RegisterMessageType<PerformForceNetworkRequest>();
            NetworkingAPI.RegisterMessageType<SetFreezeOnBodyRequest>();
            NetworkingAPI.RegisterMessageType<SetPauseTriggerNetworkRequest>();
            NetworkingAPI.RegisterMessageType<PlaySoundNetworkRequest>();
            NetworkingAPI.RegisterMessageType<UltimateObjectSpawnNetworkRequest>();
            NetworkingAPI.RegisterMessageType<SetDomainUltimateNetworkRequest>();
            NetworkingAPI.RegisterMessageType<ForceSpawnStateNetworkRequest>();
            NetworkingAPI.RegisterMessageType<AchievementGranterNetworkRequest>();
        }

        private static bool AddBank(Func<AkSoundEngineInitialization, bool> orig, AkSoundEngineInitialization self)
        {
            var res = orig(self);

            //Load Soundbanks in.
            Modules.LeeHyperrealAssets.LoadSoundbank();
            if (AkSoundEngine.IsInitialized())
            {
                AkSoundEngine.SetRTPCValue("Volume_Lee_Voice", Modules.Config.voiceVolume.Value);
            }

            return res;
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterModel.Start += CharacterModel_Start;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            On.RoR2.CharacterModel.UpdateOverlayStates += CharacterModel_UpdateOverlayStates;
            On.RoR2.CharacterSpeech.BrotherSpeechDriver.OnBodyKill += BrotherSpeechDriver_OnBodyKill;
            On.RoR2.CharacterSpeech.BrotherSpeechDriver.DoInitialSightResponse += BrotherSpeechDriver_DoInitialSightResponse;
            On.RoR2.UI.LoadoutPanelController.Row.FromSkillSlot += Row_FromSkillSlot;
            On.RoR2.UI.CharacterSelectController.Awake += CharacterSelectController_Awake;

            //On.RoR2.CharacterBody.Update += CharacterBody_Update;

            if (Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            }

            if (isAutoSprintActive) 
            {
                SetupAutoSprintBlacklist();
            }
        }

        private void CharacterSelectController_Awake(On.RoR2.UI.CharacterSelectController.orig_Awake orig, CharacterSelectController self)
        {
            //Add the Lee menu listener controller.
            orig(self);

            LeeHyperrealCharacterMenuListener listener = self.GetComponent<LeeHyperrealCharacterMenuListener>();

            if (!listener) 
            {
                listener = self.gameObject.AddComponent<LeeHyperrealCharacterMenuListener>();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SetupAutoSprintBlacklist() 
        {
            SendMessage("RT_SprintDisableMessage", "LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary.Snipe");
            SendMessage("RT_SprintDisableMessage", "LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary.IdleSnipe");
            SendMessage("RT_SprintDisableMessage", "LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary.ExitSnipe");
            SendMessage("RT_SprintDisableMessage", "LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary.EnterSnipe");
        }

        private LoadoutPanelController.Row Row_FromSkillSlot(On.RoR2.UI.LoadoutPanelController.Row.orig_FromSkillSlot orig, LoadoutPanelController owner, BodyIndex bodyIndex, int skillSlotIndex, GenericSkill skillSlot)
        {
            LoadoutPanelController.Row row = orig(owner, bodyIndex, skillSlotIndex, skillSlot);
            string newToken = "";
            if ((skillSlot.skillFamily as ScriptableObject).name.Contains("Lee: Hyperreal - Orbs and Ammo")) 
            {
                newToken = DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_PASSIVE_ORB_AND_AMMO_NAME";
            }
            if ((skillSlot.skillFamily as ScriptableObject).name.Contains("Lee: Hyperreal - Hypermatrix")) 
            {
                newToken = DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_PASSIVE_DOMAIN_NAME";    
            }
            if ((skillSlot.skillFamily as ScriptableObject).name.Contains("Lee:_Hyperreal_-_VFX_Color"))
            {
                newToken = DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_PASSIVE_VFX_COLOR_NAME";
            }
            if (newToken != "") 
            {
                Transform label = row.rowPanelTransform.Find("SlotLabel") ?? row.rowPanelTransform.Find("LabelContainer").Find("SlotLabel");
                if (label)
                {
                    label.GetComponent<LanguageTextMeshController>().token = newToken;
                }
            }


            return row;
        }

        private void BaseMainMenuScreen_Awake(On.RoR2.UI.MainMenu.BaseMainMenuScreen.orig_Awake orig, RoR2.UI.MainMenu.BaseMainMenuScreen self)
        {
            orig(self);
            Modules.StaticValues.AddNotificationItemPairs();
        }

        private void BrotherSpeechDriver_DoInitialSightResponse(On.RoR2.CharacterSpeech.BrotherSpeechDriver.orig_DoInitialSightResponse orig, RoR2.CharacterSpeech.BrotherSpeechDriver self)
        {
            bool leeFound = false;

            ReadOnlyCollection<CharacterBody> characterBodies = CharacterBody.readOnlyInstancesList;
            for (int i = 0; i < characterBodies.Count; i++)
            {
                BodyIndex bodyIndex = characterBodies[i].bodyIndex;
                leeFound |= (bodyIndex == BodyCatalog.FindBodyIndex("LeeHyperrealBody"));
            }

            if (leeFound)
            {
                RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool = new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[]
                {
                    new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo
                    {
                        duration = 1f,
                        maxWait = 4f,
                        mustPlay = true,
                        priority = 0f,
                        token = "LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ENTRY"
                    },
                };

                self.SendReponseFromPool(responsePool);
            }

            orig(self);
        }

        private void BrotherSpeechDriver_OnBodyKill(On.RoR2.CharacterSpeech.BrotherSpeechDriver.orig_OnBodyKill orig, RoR2.CharacterSpeech.BrotherSpeechDriver self, DamageReport damageReport)
        {
            if (damageReport.victimBody)
            {
                if (damageReport.victimBodyIndex == BodyCatalog.FindBodyIndex("LeeHyperrealBody"))
                {
                    RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool = new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[]
                    {
                    new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo
                    {
                        duration = 1f,
                        maxWait = 4f,
                        mustPlay = true,
                        priority = 0f,
                        token = "LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ON_DEATH_1"
                    },
                    new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo
                    {
                        duration = 1f,
                        maxWait = 4f,
                        mustPlay = true,
                        priority = 0f,
                        token = "LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ON_DEATH_2"
                    },
                    new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo
                    {
                        duration = 1f,
                        maxWait = 4f,
                        mustPlay = true,
                        priority = 0f,
                        token = "LEE_HYPERREAL_BODY_BROTHER_RESPONSE_ON_DEATH_3"
                    }
                    };

                    self.SendReponseFromPool(responsePool);
                }
            }

            orig(self, damageReport);
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {

            DamageType HealthCostShrineDamageType = DamageType.NonLethal | DamageType.BypassArmor;
            float damageWouldReceive = damageInfo.damage;
            
            if (self)
            {
                if (self.body)
                {
                    if (self.body.HasBuff(Modules.Buffs.fallDamageNegateBuff)) 
                    {
                        DamageType tempDamageType = DamageType.FallDamage | DamageType.NonLethal;
                        DamageType frailtyDamageType = DamageType.FallDamage | DamageType.BypassOneShotProtection;

                        if (damageInfo.damageType.damageType == tempDamageType || damageInfo.damageType.damageType == frailtyDamageType)
                        {
                            damageInfo.rejected = true;

                            damageInfo.damage = 0;
                        }
                    }

                    if (self.body.HasBuff(Modules.Buffs.invincibilityBuff) && damageInfo.damageType.damageType != HealthCostShrineDamageType) 
                    {
                        damageInfo.rejected = true;
                        damageInfo.damage = 0f;
                    }


                    if (self.body.baseNameToken == DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_NAME" && damageInfo.attacker) 
                    {   
                        if (self.body.HasBuff(Modules.Buffs.parryBuff)) 
                        {
                            bool doBigParry = false;
                            if (damageInfo.damage > self.fullHealth * Modules.StaticValues.bigParryHealthFrac) 
                            {
                                doBigParry = true;
                            }


                            if (damageInfo.damageType.damageType == HealthCostShrineDamageType)
                            {
                                damageInfo.damage = damageWouldReceive * 0.5f;
                                damageInfo.rejected = false;
                            }
                            else 
                            {
                                //Reject damage, return to ~~monke~~ pause in state.
                                damageInfo.rejected = true;
                                damageInfo.damage = 0f;
                            }

                            new SetPauseTriggerNetworkRequest(self.body.master.netId, true, doBigParry).Send(NetworkDestination.Clients);

                            //Stun the attacker
                            if (damageInfo.attacker && !damageInfo.HasModdedDamageType(Modules.Damage.leeHyperrealParryDamage)) 
                            {
                                HealthComponent attackerHealthComp = damageInfo.attacker.GetComponent<HealthComponent>();
                                if (attackerHealthComp) 
                                {
                                    DamageInfo stunInfo = new DamageInfo
                                    {
                                        damage = damageWouldReceive * 0.25f,
                                        attacker = self.body.gameObject,
                                        crit = self.body.RollCrit(),
                                        damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, DamageSource.Primary),
                                        damageColorIndex = DamageColorIndex.Default
                                    };

                                    DamageAPI.AddModdedDamageType(stunInfo, Modules.Damage.leeHyperrealParryDamage);
                                    attackerHealthComp.TakeDamage(stunInfo);
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo);
        }

        private void CharacterModel_Start(On.RoR2.CharacterModel.orig_Start orig, CharacterModel self)
        {
            orig(self);
            if (self.gameObject.name.Contains("LeeHyperrealDisplay"))
            {
                LeeHyperrealDisplayController displayHandler = self.gameObject.GetComponent<LeeHyperrealDisplayController>();
                if (!displayHandler)
                {
                    displayHandler = self.gameObject.AddComponent<LeeHyperrealDisplayController>();
                }
            }
        }

        private void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        {
            orig();
            foreach (var item in SurvivorCatalog.allSurvivorDefs)
            {
                if (item.bodyPrefab.name == "LeeHyperrealBody")
                {
                    CustomEmotesAPI.ImportArmature(item.bodyPrefab, Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<GameObject>("humanoidLeeHyperreal"));
                    item.bodyPrefab.GetComponentInChildren<BoneMapper>().scale = 1.05f;
                    CustomEmotesAPI.CreateNameTokenSpritePair(DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_NAME", Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<Sprite>("LeeEmotesIcon"));
                }
            }
        }

        private bool CharacterModel_UpdateOverlayStates(On.RoR2.CharacterModel.orig_UpdateOverlayStates orig, CharacterModel self) 
        {
            bool result = orig(self);

            if (!result) 
            {
                //Fuck it, force update until it is no more
                if (self && self.body && self.body.HasBuff(Modules.Buffs.glitchEffectBuff)) 
                {
                    return true;
                }
            }

            return result;
        }

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);

            if (self && self.body && self.materialsDirty) 
            {
                this.overlayFunction(Modules.LeeHyperrealAssets.glitchMaterial, self.body.HasBuff(Modules.Buffs.glitchEffectBuff), self);
            }
        }



        private void overlayFunction(Material overlayMaterial, bool condition, CharacterModel model)
        {
            if (model.activeOverlayCount >= CharacterModel.maxOverlays)
            {
                return;
            }
            if (condition)
            {
                Material[] array = model.currentOverlays;
                int num = model.activeOverlayCount;
                model.activeOverlayCount = num + 1;
                array[num] = overlayMaterial;
            }
        }
    }
}