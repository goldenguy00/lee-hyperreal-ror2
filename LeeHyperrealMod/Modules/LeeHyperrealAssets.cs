﻿using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using System;
using LeeHyperrealMod.Content.Controllers;
using EntityStates;
using UnityEngine.AddressableAssets;
using LeeHyperrealMod.Content.Notifications;
using R2API.Utils;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine.UI;
using FontStyles = UnityEngine.TextCore.Text.FontStyles;

namespace LeeHyperrealMod.Modules
{
    internal static class LeeHyperrealAssets
    {
        #region Other Assets
        // particle effects
        internal static GameObject uiObject;
        internal static Material blueOrbMat;
        internal static Material yellowOrbMat;
        internal static Material redOrbMat;
        internal static Material UIFadeMat;
        internal static Sprite scarletSprite;
        internal static Sprite prospectorSprite;
        internal static Sprite comradeSprite;
        internal static Sprite leeIconSprite;

        internal static GameObject ultimateCameraObject;
        internal static GameObject domainUltimateCameraObject;

        internal static GameObject ultimateExplosionObject;

        // UI objects
        internal static GameObject powerMeterObject;
        internal static GameObject healthPrefabs;
        internal static GameObject healthPrefabsHunkHud;
        internal static GameObject orbsUIObject;
        internal static GameObject spinnyIconUIObject;
        internal static GameObject leenotificationBoxPrefab;

        //Material
        internal static Material glitchMaterial;

        //Lee "Survivor Pod"
        internal static GameObject leeSurvivorPod;

        //Lee Custom Notification Panel stolen from HUNK
        internal static GameObject customNotificationPrefab;
        #endregion

        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        // CHANGE THIS
        private const string assetbundleName = "leehyperrealassets";
        //change this to your project's name if/when you've renamed it
        private const string csProjName = "LeeHyperrealMod";

        public static string AssetBundlePath
        {
            get
            {
                //This returns the path to your assetbundle assuming said bundle is on the same folder as your DLL. If you have your bundle in a folder, you can uncomment the statement below this one.
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(LeeHyperrealPlugin.PInfo.Location), assetbundleName);
                //return Path.Combine(Path.GetDirectoryName(MainClass.PInfo.Location), assetBundleFolder, myBundle);
            }
        }

        internal static uint _soundBankId;
        internal static string soundbankName = "LeeHyperrealSoundBank.bnk";
        internal static string soundbankFolder = "Soundbanks";
        public static string SoundBankDirectory
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(LeeHyperrealPlugin.PInfo.Location), "");
            }
        }


        internal static void Initialize()
        {
            LoadAssetBundle();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            Log.Info("Adding Assetbundle");
            try
            {
                if (mainAssetBundle == null)
                {
                    mainAssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
                    //using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.{assetbundleName}"))
                    //{
                    //    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    //}
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }
        }

        internal static void LoadSoundbank()
        {
            //using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{csProjName}.LeeHyperrealSoundBank.bnk"))
            //{
            //    byte[] array = new byte[manifestResourceStream2.Length];
            //    manifestResourceStream2.Read(array, 0, array.Length);
            //SoundAPI.SoundBanks.Add(array);
            //}

            var akResult = AkSoundEngine.AddBasePath(SoundBankDirectory);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank base path : {SoundBankDirectory}");
            }
            else
            {
                Log.Error(
                    $"Error adding base path : {SoundBankDirectory} " +
                    $"Error code : {akResult}");
            }

            akResult = AkSoundEngine.LoadBank(soundbankName, out _soundBankId);
            if (akResult == AKRESULT.AK_Success)
            {
                Log.Info($"Added bank : {soundbankName}");
            }
            else
            {
                Log.Error(
                    $"Error loading bank : {soundbankName} " +
                    $"Error code : {akResult}");
            }
        }

        internal static void LatePopulateAssets() 
        {
            BuffPassengerWhileSeated buffPass = leeSurvivorPod.GetComponent<BuffPassengerWhileSeated>();
            VehicleSeat vehicleSeat = leeSurvivorPod.GetComponent<VehicleSeat>();
            buffPass.buff = Modules.Buffs.invincibilityBuff;
            buffPass.vehicleSeat = vehicleSeat;

            Modules.Content.AddNetworkObjectPrefabs(leeSurvivorPod);
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Log.Error("There is no AssetBundle to load assets from.");
                return;
            }

            uiObject = mainAssetBundle.LoadAsset<GameObject>("LeeHyperrealUI");
            powerMeterObject = mainAssetBundle.LoadAsset<GameObject>("Power Bar Empty");
            healthPrefabs = mainAssetBundle.LoadAsset<GameObject>("LeeHealth");
            healthPrefabsHunkHud = mainAssetBundle.LoadAsset<GameObject>("LeeHealthHunkHud");
            orbsUIObject = mainAssetBundle.LoadAsset<GameObject>("Orbs");
            spinnyIconUIObject = mainAssetBundle.LoadAsset<GameObject>("Ult Base");

            prospectorSprite = mainAssetBundle.LoadAsset<Sprite>("ProspectorCharacterIcon");
            comradeSprite = mainAssetBundle.LoadAsset<Sprite>("ComradeCharacterIcon");
            leeIconSprite = mainAssetBundle.LoadAsset<Sprite>("LeeCharacterIcon");
            scarletSprite = mainAssetBundle.LoadAsset<Sprite>("ScarletRedeemerCharacterIcon");

            UIFadeMat = mainAssetBundle.LoadAsset<Material>("UIFadeMat");

            blueOrbMat = mainAssetBundle.LoadAsset<Material>("UI Blue Orb");
            yellowOrbMat = mainAssetBundle.LoadAsset<Material>("UI Yellow Orb");
            redOrbMat = mainAssetBundle.LoadAsset<Material>("UI Red Orb");

            ultimateCameraObject = mainAssetBundle.LoadAsset<GameObject>("Camera Ultimate Root");
            domainUltimateCameraObject = mainAssetBundle.LoadAsset<GameObject>("Domain Ultimate Camera");

            ultimateExplosionObject = mainAssetBundle.LoadAsset<GameObject>("fxr4liangatk51sound"); // LMAO empty object let's use it 
            ultimateExplosionObject.AddComponent<UltimateOrbExplosion>();

            glitchMaterial = mainAssetBundle.LoadAsset<Material>("fxr4liang010011mdcanying703");

            leeSurvivorPod = mainAssetBundle.LoadAsset<GameObject>("stageIntroPrefab");

            //Time to setup the prefab:
            NetworkIdentity netID = leeSurvivorPod.AddComponent<NetworkIdentity>();
            netID.SetDynamicAssetId(NetworkHash128.Parse(HashString("Lee Hyperreal Survivor Pod GameObject")));
            SurvivorPodController podController = leeSurvivorPod.AddComponent<SurvivorPodController>();
            podController.cameraBone = leeSurvivorPod.transform.GetChild(4);

            ////Setup the ESM/NSM
            // This is added when SurvivorPodController is added
            EntityStateMachine podESM = leeSurvivorPod.GetComponent<EntityStateMachine>();
            podESM.customName = "Main";
            podESM.initialStateType = new SerializableEntityStateType(typeof(SkillStates.LeeHyperrealSurvivorPod.Idle));
            podESM.mainStateType = new SerializableEntityStateType(typeof(SkillStates.LeeHyperrealSurvivorPod.Idle));
            NetworkStateMachine podNSM = leeSurvivorPod.AddComponent<NetworkStateMachine>();
            EntityStateMachine[] stateMachines = new EntityStateMachine[1];
            stateMachines[0] = podESM;
            podNSM.stateMachines = stateMachines;

            leeSurvivorPod.AddComponent<EntityLocator>();
            VehicleSeat vehicleSeat = leeSurvivorPod.GetComponent<VehicleSeat>(); // Added with SurvivorPodController
            vehicleSeat.seatPosition = leeSurvivorPod.transform;
            vehicleSeat.exitVehicleContextString = $"{LeeHyperrealPlugin.DEVELOPER_PREFIX}_LEE_HYPERREAL_BODY_SURVIVOR_POD_EXIT";
            vehicleSeat.passengerState = new SerializableEntityStateType(typeof(EntityStates.Idle));

            leeSurvivorPod.AddComponent<BuffPassengerWhileSeated>();

            leenotificationBoxPrefab = mainAssetBundle.LoadAsset<GameObject>("LeeNotification");
            LeeHyperrealNotification leeNotification = leenotificationBoxPrefab.AddComponent<LeeHyperrealNotification>();
            customNotificationPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/NotificationPanel2.prefab").WaitForCompletion(), "LeeHyperrealNotification", false);
            GenericNotification genericComponent = LeeHyperrealAssets.customNotificationPrefab.GetComponent<GenericNotification>();


            Transform hyperLabel = leenotificationBoxPrefab.transform.GetChild(0).GetChild(0).GetChild(3);
            UnityEngine.Object.DestroyImmediate(hyperLabel.gameObject.GetComponent<TextMeshProUGUI>());
            Transform descriptionLabel = leenotificationBoxPrefab.transform.GetChild(0).GetChild(0).GetChild(2);
            UnityEngine.Object.DestroyImmediate(descriptionLabel.gameObject.GetComponent<TextMeshProUGUI>());
            UnityEngine.Object.DestroyImmediate(leeNotification.titleText);
            UnityEngine.Object.DestroyImmediate(leeNotification.descriptionText);

            leeNotification.titleText = CreateLabel(hyperLabel, "Hyper Effect", 36f);
            leeNotification.titleTMP = hyperLabel.GetComponent<HGTextMeshProUGUI>();
            leeNotification.titleTMP.color = StaticValues.bodyColor;
            leeNotification.descriptionText = CreateLabel(descriptionLabel, "Demo Text", 20f);
            leeNotification.titleText.token = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_ITEM_EFFECT_TITLE";
            leeNotification.descriptionText.token = LeeHyperrealPlugin.DEVELOPER_PREFIX + "_LEE_HYPERREAL_BODY_ITEM_EFFECT_TITLE";
            leeNotification.iconImage = leenotificationBoxPrefab.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<RawImage>();
            leeNotification.previousIconImage = genericComponent.previousIconImage;
            leeNotification.canvasGroup = genericComponent.canvasGroup;
            leeNotification.fadeOutT = genericComponent.fadeOutT;
        }

        private static LanguageTextMeshController CreateLabel(Transform parent, string text, float textScale)
        {
            LanguageTextMeshController controller = parent.gameObject.AddComponent<LanguageTextMeshController>();
            HGTextMeshProUGUI hgtextMeshProUGUI = parent.gameObject.AddComponent<HGTextMeshProUGUI>();
            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = textScale;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = true;
            hgtextMeshProUGUI.autoSizeTextContainer = true;

            controller.textMeshPro = hgtextMeshProUGUI;
            return controller;
        }

        private static string HashString(string str) 
        {
            MD5 md5 = MD5.Create();
            byte[] computedHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            md5.Dispose();

            return computedHash.ToString();    
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                if (i) 
                {
                    if (!i.gameObject.transform.parent.gameObject.name.Contains("Clone") && !i.gameObject.transform.parent.gameObject.name.Contains("xuanzhuan"))
                    {
                        i?.material?.SetHopooMaterial();
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }


        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the BodyName in your code matches the prefab name of the object in Unity\nFor Example, if your prefab in unity is 'mdlHenry', then your BodyName must be 'Henry'");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            if (!newEffect)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}