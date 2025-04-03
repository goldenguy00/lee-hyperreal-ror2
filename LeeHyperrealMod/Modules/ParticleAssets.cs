using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LeeHyperrealMod.Content.Controllers;
using R2API;
using System;

namespace LeeHyperrealMod.Modules
{
    internal class ParticleAssets
    {
        internal static string DEFAULT_PARTICLE_VARIANT = "default";
        internal static string RED_PARTICLE_VARIANT = "red";
        internal static Color RED_PARTICLE_COLOR = new Color(1.0f, 0f, 0f);

        internal class ParticleVariant 
        {
            internal Dictionary<string, GameObject> colourVariants;

            internal ParticleVariant() 
            {
                colourVariants = new Dictionary<string, GameObject>();            
            }

            internal ParticleVariant(string name, GameObject obj) 
            {
                colourVariants = new Dictionary<string, GameObject>
                {
                    { name, obj }
                };
            }
        }

        /*
            On access, you should look for the key for that move you want
            In the case of P1, access "p1" and then choose a variant, these should be 
            defined here.

            Variants planned
                - "default", // This is the blue one.
                - "red",

            e.g to access the blue variant, call the following: particleDictionary["p1"].colourVariants["default"] -> returns the gameobject you want to use.
         */

        internal static Dictionary<string, ParticleVariant> particleDictionary;

        #region Materials
        internal static List<Material> materialStorage = new List<Material>();
        #endregion

        #region Yellow Orb
        public static GameObject yellowOrbSwing;
        public static GameObject yellowOrbSwingHit;
        public static GameObject yellowOrbKick;
        public static GameObject yellowOrbMultishot;
        public static GameObject yellowOrbMultishotHit;
        public static GameObject yellowOrbDomainBulletLeftovers;
        public static GameObject yellowOrbDomainClone;
        #endregion

        #region Snipe
        public static GameObject SnipeStart;
        public static GameObject Snipe;
        public static GameObject snipeHit;
        public static GameObject snipeHitEnhanced;
        public static GameObject snipeGround;
        public static GameObject snipeBulletCasing;
        public static GameObject snipeDodge;
        public static GameObject snipeAerialFloor;
        #endregion

        #region Parry
        public static GameObject normalParry;
        public static GameObject bigParry;
        #endregion

        #region Dodge
        public static GameObject dodgeForwards;
        public static GameObject dodgeBackwards;
        #endregion

        #region Ultimate Domain
        //FXR 42
        public static GameObject UltimateDomainFinisherEffect;
        public static GameObject UltimateDomainCEASEYOUREXISTANCE;
        public static GameObject DomainOverlayEffect;
        public static GameObject UltimateDomainBulletFinisher;
        public static GameObject UltimateDomainClone1;
        public static GameObject UltimateDomainClone2;
        public static GameObject UltimateDomainClone3;
        #endregion

        #region Domain/Transition Effects
        //FXR 41
        public static GameObject transitionEffectLee;
        public static GameObject transitionEffectHit;
        public static GameObject transitionEffectGround;
        public static GameObject domainFieldLoopEffect;
        public static GameObject domainFieldEndEffect;
        #endregion

        #region Ultimate non-domain 
        //FXR 51
        public static GameObject ultExplosion;
        public static GameObject ultGunEffect;
        public static GameObject ultTracerEffect;
        public static GameObject ultPreExplosionProjectile;
        public static GameObject ultShootingEffect;
        #endregion

        #region Display particles
        public static GameObject displayLandingEffect;
        #endregion

        #region Misc
        public static GameObject jumpEffect;
        public static GameObject customCrosshair;
        #endregion

        public static void Initialize() 
        {
            particleDictionary = new Dictionary<string, ParticleVariant>();

            UpdateAllBundleMaterials();
            CreateMaterialStorage(Modules.LeeHyperrealAssets.mainAssetBundle);
            PopulateAssets();
        }

        private static void UpdateAllBundleMaterials()
        {
            Material[] materials = Modules.LeeHyperrealAssets.mainAssetBundle.LoadAllAssets<Material>();

            foreach (Material material in materials)
            {
                if (material.shader.name.StartsWith("StubbedRoR2"))
                {
                    string newName;
                    nameConversion.TryGetValue(material.shader.name, out newName);
                    material.shader = RoR2.LegacyShaderAPI.Find(newName);
                }
            }
        }

        private static GameObject GetGameObjectFromBundle(string objectName) 
        {
            return Modules.LeeHyperrealAssets.mainAssetBundle.LoadAsset<GameObject>(objectName);
        }

        private static GameObject ModifyEffect(GameObject newEffect, string soundName, bool parentToTransform) 
        {
            return ModifyEffect(newEffect, soundName, parentToTransform, 1f);
        }

        private static GameObject ModifyEffect(GameObject newEffect, string soundName, bool parentToTransform, float duration, VFXAttributes.VFXPriority priority = VFXAttributes.VFXPriority.Always, bool shouldNotPool = false)
        {
            newEffect.AddComponent<DestroyOnTimer>().duration = duration;
            NetworkIdentity netid = newEffect.GetComponent<NetworkIdentity>();
            if (!netid)
            {
                netid = newEffect.AddComponent<NetworkIdentity>();
            }
            else 
            {
                //Reinit netid.
                UnityEngine.Object.Destroy(netid);
                netid = newEffect.AddComponent<NetworkIdentity>();
            }
            VFXAttributes attr = newEffect.GetComponent<VFXAttributes>();
            if (!attr) 
            {
                attr = newEffect.AddComponent<VFXAttributes>();
            }
            attr.vfxPriority = priority;
            attr.DoNotPool = shouldNotPool;
            EffectComponent effect = newEffect.GetComponent<EffectComponent>();
            if (!effect) 
            {
                effect = newEffect.AddComponent<EffectComponent>();// probably already added.
            }
            effect.applyScale = true;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
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

        public static void ModifyParticleSystemColorOnUberShader(Transform obj, Color color) 
        {
            ParticleSystemRenderer psr = obj.GetComponent<ParticleSystemRenderer>();
            psr.material.SetColor("_TintColor", color);
        }

        public static void ModifyXEffectOnPSR(GameObject obj, Color newColour) 
        {
            ParticleSystemRenderer[] psrs = obj.GetComponentsInChildren<ParticleSystemRenderer>();

            foreach (ParticleSystemRenderer psr in psrs) 
            {
                //Check if the material is Xeffect
                if (psr.material.shader.name == "Unlit/XEffect")
                {
                    //Get New Colour, figure out percentages for each and spread across colours in the same intensity
                    float oldR = psr.material.GetFloat("_EffectBrightnessR");
                    float oldG = psr.material.GetFloat("_EffectBrightnessG");
                    float oldB = psr.material.GetFloat("_EffectBrightnessB");

                    float totalToDistribute = oldR + oldG + oldB;

                    float totalColourVal = newColour.r + newColour.g + newColour.b;

                    float newR = (newColour.r / totalColourVal) * totalToDistribute;
                    float newG = (newColour.g / totalColourVal) * totalToDistribute;
                    float newB = (newColour.b / totalColourVal) * totalToDistribute;

                    // Modify the following properties:
                    psr.material.SetFloat("_EffectBrightnessR", newR);
                    psr.material.SetFloat("_EffectBrightnessG", newG);
                    psr.material.SetFloat("_EffectBrightnessB", newB);
                }
            }
        }

        #region Particle Effect Retrieval

        public static GameObject RetrieveParticleEffect(string particleName, string variant)
        {
            if (!particleDictionary.ContainsKey(particleName))
            {
                return null; // Failed!
            }

            return particleDictionary[particleName].colourVariants[variant];
        }

        public static GameObject RetrieveParticleEffectFromSkin(string particleName, CharacterBody body = null) 
        {
            //Checking if such a name exists for this particle.
            if (!particleDictionary.ContainsKey(particleName)) 
            {
                return null; // Failed!
            }

            ParticleVariant variants = particleDictionary[particleName];

            // Exit early, we don't know what variant they have selected and they explicitly used this one!
            if (!body) 
            {
                return variants.colourVariants[DEFAULT_PARTICLE_VARIANT];
            }

            //Get Skin index
            uint skinIndex = body.skinIndex;

            //Get related skin particle effect
            if (Modules.Survivors.LeeHyperreal.redVFXSkins.Contains((int)skinIndex)) 
            {
                if (variants.colourVariants.ContainsKey(RED_PARTICLE_VARIANT)) 
                {
                    return variants.colourVariants[RED_PARTICLE_VARIANT];
                }
            }

            //all else fails, just send them the default
            return variants.colourVariants[DEFAULT_PARTICLE_VARIANT];
        }

        private static void GenerateColorVariant(string name, Color color)
        {
            foreach (KeyValuePair<string, ParticleVariant> item in particleDictionary)
            {
                ParticleVariant pVar = item.Value;
                Dictionary<string, GameObject> pVarColourVar = item.Value.colourVariants;

                //Get default 
                GameObject defaultVariant = pVarColourVar[DEFAULT_PARTICLE_VARIANT];
                GameObject clone = PrefabAPI.InstantiateClone(defaultVariant, defaultVariant.name + "-" + name);

                //Modify with the new colour on XEffect, may need more functions to convert more later
                ModifyXEffectOnPSR(clone, color);

                //Get original sound from default
                EffectComponent effectComp = defaultVariant.GetComponent<EffectComponent>();
                string soundname = "";
                bool parentToTransform = true;
                if (effectComp)
                {
                    soundname = effectComp.soundName;
                    parentToTransform = effectComp.parentToReferencedTransform;
                }

                //Finally resetup the prefab
                ModifyEffect(clone, soundname, parentToTransform);

                //Register the new prefab under name
                pVarColourVar.Add(name, clone);
            }
        }

        #endregion

        public static void PopulateAssets() 
        {
            #region Primary
            PopulateAerialDomainAssets();
            PopulatePrimary1Assets();
            PopulatePrimary2Assets();
            PopulatePrimary3Assets();
            PopulatePrimary4Assets();
            PopulatePrimary5Assets();
            #endregion

            #region RedOrb
            PopulateRedOrbAssets();
            #endregion

            #region Blue Orb
            PopulateBlueOrbAssets();
            #endregion

            #region Yellow Orb
            PopulateYellowOrbAssets();
            #endregion

            #region Snipe
            PopulateSnipeAssets();
            #endregion

            #region Parry
            PopulateParryAssets();
            #endregion

            #region Dodge
            PopulateDodgeAssets();
            #endregion

            #region Domain/Transition Effects
            PopulateDomainTransitionAssets();
            #endregion

            #region Domain Ultimate
            PopulateDomainUltimateAssets();
            #endregion

            #region Ultimate Non-domain
            PopulateUltimateAssets();
            #endregion

            #region Display Particles
            PopulateDisplayParticleAssets();
            #endregion

            #region Misc
            PopulateMiscAssets();
            #endregion

            GenerateColorVariant(RED_PARTICLE_VARIANT, RED_PARTICLE_COLOR);
        }

        private static void PopulateMiscAssets()
        {
            jumpEffect = GetGameObjectFromBundle("Extra Jump Floor");
            EffectUnparenter effectUnparenter = jumpEffect.AddComponent<EffectUnparenter>();
            effectUnparenter.duration = 0.175f;
            jumpEffect = ModifyEffect(jumpEffect, "", true, 1.5f, VFXAttributes.VFXPriority.Always, true);


            customCrosshair = GetGameObjectFromBundle("Lee Crosshair");
        }

        private static void PopulateAerialDomainAssets()
        {
            ParticleVariant primaryAerialParticleVariant = new ParticleVariant(DEFAULT_PARTICLE_VARIANT, GetGameObjectFromBundle("DomainMidairLoop"));

            GameObject primaryAerialEffectEnd = GetGameObjectFromBundle("DomainMidairEnd");
            primaryAerialEffectEnd = ModifyEffect(primaryAerialEffectEnd, "Play_c_liRk4_skill_yellow_dilie", false, 2f);
            ParticleVariant primaryAerialEnd = new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primaryAerialEffectEnd);

            // Add to dictionary
            particleDictionary.Add("primaryAerialLoop", primaryAerialParticleVariant);
            particleDictionary.Add("primaryAerialEnd", primaryAerialEnd);
        }

        private static void PopulateDisplayParticleAssets()
        {
            displayLandingEffect = GetGameObjectFromBundle("fxr4liangborn1");
            displayLandingEffect.AddComponent<DestroyOnTimer>().duration = 5f;
        }

        private static void PopulateUltimateAssets()
        {
            //ultGunEffect = GetGameObjectFromBundle("");
            //ultGunEffect = ModifyEffect(ultGunEffect, "", true);

            ultTracerEffect = GetGameObjectFromBundle("fxr4liangatk51xuli");
            ultTracerEffect = ModifyEffect(ultTracerEffect, "", true, 6f);

            ultShootingEffect = GetGameObjectFromBundle("Cannon Ult Shot Extra Prefab");
            ultShootingEffect = ModifyEffect(ultShootingEffect, "", true, 6f);

            ultPreExplosionProjectile = GetGameObjectFromBundle("fxr4liangatk51");
            ultPreExplosionProjectile = ModifyEffect(ultPreExplosionProjectile, "Play_c_liRk4_skill_ultimate_blast", true, 5f);

            ultExplosion = GetGameObjectFromBundle("fxr4liangatk51zha");
            ultExplosion = ModifyEffect(ultExplosion, "", true, 5f);
        }

        private static void PopulateDomainUltimateAssets()
        {
            UltimateDomainFinisherEffect = GetGameObjectFromBundle("fxr4liangatk42suiping");
            UltimateDomainFinisherEffect.AddComponent<DestroyOnTimer>().duration = 2f;

            UltimateDomainCEASEYOUREXISTANCE = GetGameObjectFromBundle("Cease");
            UltimateDomainCEASEYOUREXISTANCE.AddComponent<DestroyOnTimer>().duration = 2f;

            DomainOverlayEffect = GetGameObjectFromBundle("fxr4liangatk51pingmu"); // Control manually.

            UltimateDomainBulletFinisher = GetGameObjectFromBundle("fxr4liangatk42zhongjie01");
            UltimateDomainBulletFinisher = ModifyEffect(UltimateDomainBulletFinisher, "", true, 10f);

            UltimateDomainClone1 = GetGameObjectFromBundle("DomainUltClone1");
            UltimateDomainClone1 = ModifyEffect(UltimateDomainClone1, "", true, 2f, VFXAttributes.VFXPriority.Always);

            UltimateDomainClone2 = GetGameObjectFromBundle("DomainUltClone2");
            UltimateDomainClone2 = ModifyEffect(UltimateDomainClone2, "", true, 2f, VFXAttributes.VFXPriority.Always);

            UltimateDomainClone3 = GetGameObjectFromBundle("DomainUltClone3");
            UltimateDomainClone3 = ModifyEffect(UltimateDomainClone3, "", true, 2f, VFXAttributes.VFXPriority.Always);
        }

        private static void PopulateDomainTransitionAssets()
        {
            transitionEffectLee = GetGameObjectFromBundle("fxr4liangatk41");
            transitionEffectLee = ModifyEffect(transitionEffectLee, "", true);

            transitionEffectGround = GetGameObjectFromBundle("fxr4liangatk41bao");
            transitionEffectGround = ModifyEffect(transitionEffectGround, "", true);

            transitionEffectHit = GetGameObjectFromBundle("fxr4liangatk41hit");
            //AddLightIntensityCurveWithCurve(
            //    transitionEffectHit.transform.GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.18f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false,
            //    },
            //    "fxr4liangatk41hit"
            //);
            transitionEffectHit = ModifyEffect(transitionEffectHit, "", true);

            domainFieldLoopEffect = GetGameObjectFromBundle("fxr4liangatk41loop");
            domainFieldEndEffect = GetGameObjectFromBundle("fxr4liangatk41out");
        }

        private static void PopulateDodgeAssets()
        {
            dodgeForwards = GetGameObjectFromBundle("fxr4liangmove01");
            dodgeForwards = ModifyEffect(dodgeForwards, "Play_c_liRk4_act_flash_1", true);

            dodgeBackwards = GetGameObjectFromBundle("fxr4liangmove02");
            dodgeBackwards = ModifyEffect(dodgeBackwards, "Play_c_liRk4_act_flash_2", true);
        }

        private static void PopulateParryAssets()
        {
            normalParry = GetGameObjectFromBundle("Normal Parry");
            normalParry = ModifyEffect(normalParry, "", true);

            bigParry = GetGameObjectFromBundle("Big Parry");
            bigParry = ModifyEffect(bigParry, "", true);
        }

        private static void PopulateSnipeAssets()
        {
            SnipeStart = GetGameObjectFromBundle("fxr4liangatk23");
            SnipeStart = ModifyEffect(SnipeStart, "", true);

            Snipe = GetGameObjectFromBundle("fxr4liangatk24");
            //AddLightIntensityCurveWithCurve(
            //    Snipe.transform.GetChild(0).GetChild(1).gameObject,
            //    new LightIntensityProps 
            //    {
            //        timeMax = 0.5f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false,
            //    },
            //    "fxr4liangatk24-lightSC1"
            //    );
            //AddLightIntensityCurveWithCurve(
            //    Snipe.transform.GetChild(1).GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.4f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false,
            //    },
            //    "fxr4liangatk24-spjere"
            //    );
            //AddLightIntensityCurveWithCurve(
            //    Snipe.transform.GetChild(2).GetChild(11).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.45f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false,
            //    },
            //    "fxr4liangatk24-lightSC2"
            //    );

            Snipe = ModifyEffect(Snipe, "", false, 1.25f, VFXAttributes.VFXPriority.Medium);

            snipeHitEnhanced = GetGameObjectFromBundle("fxr4liangatk24bao");
            snipeHitEnhanced = ModifyEffect(snipeHitEnhanced, "Play_c_liRk4_atk_ex_3_break", true, 2f);

            snipeHit = GetGameObjectFromBundle("fxr4liangatk24hit");
            //AddLightIntensityCurveWithCurve(
            //    snipeHit.transform.GetChild(0).GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.15f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false,
            //    },
            //    "fxr4liangatk24hit-lightSC"
            //    );
            //AddLightIntensityCurveWithCurve(
            //    snipeHit.transform.GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.18f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false,
            //    },
            //    "fxr4liangatk24hit-spjere"
            //    );
            snipeHit = ModifyEffect(snipeHit, "Play_c_liRk4_imp_ex_3_1", true, 2f);

            snipeGround = GetGameObjectFromBundle("fxr4liangatk24ground");
            snipeGround = ModifyEffect(snipeGround, "", true, 2f);

            snipeBulletCasing = GetGameObjectFromBundle("fxr4liangatk24bulletcasing");
            snipeBulletCasing = ModifyEffect(snipeBulletCasing, "", true, 2f, VFXAttributes.VFXPriority.Low);

            snipeDodge = GetGameObjectFromBundle("fxr4liangatk28");
            snipeDodge = ModifyEffect(snipeDodge, "Play_c_liRk4_act_flash_3", true);

            snipeAerialFloor = GetGameObjectFromBundle("Snipe Floor");
            snipeAerialFloor.AddComponent<DestroyPlatformOnDelay>();
        }

        private static void PopulateYellowOrbAssets()
        {
            yellowOrbSwing = GetGameObjectFromBundle("fxr4liangatk34dilie");
            yellowOrbSwing = ModifyEffect(yellowOrbSwing, "Play_c_liRk4_skill_yellow", true);

            yellowOrbSwingHit = GetGameObjectFromBundle("fxr4liangatk34hit");
            //AddLightIntensityCurveWithCurve(
            //    yellowOrbSwingHit.transform.GetChild(0).GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.18f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false

            //    },
            //    "fxr4liangatk34hit-spjere"
            //    );
            //AddLightIntensityCurveWithCurve(
            //    yellowOrbSwingHit.transform.GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.12f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false

            //    },
            //    "fxr4liangatk34hit-lightSC"
            //    );
            yellowOrbSwingHit = ModifyEffect(yellowOrbSwingHit, "Play_c_liRk4_imp_yellow_1", true);

            yellowOrbKick = GetGameObjectFromBundle("fxr4liangatk32");
            yellowOrbKick = ModifyEffect(yellowOrbKick, "Play_c_liRk4_skill_yellow_fire", true);

            yellowOrbMultishot = GetGameObjectFromBundle("fxr4liangatk35");
            //AddLightIntensityCurveWithCurve(
            //    yellowOrbMultishot.transform.GetChild(8).GetChild(3).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 0.15f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false
            //    },
            //    "fxr4liangatk35"
            //    );
            yellowOrbMultishot = ModifyEffect(yellowOrbMultishot, "", true);

            yellowOrbMultishotHit = GetGameObjectFromBundle("fxr4liangatk35hit");
            //AddLightIntensityCurveWithCurve(
            //    yellowOrbMultishotHit.transform.GetChild(0).GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 1.6f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false
            //    },
            //    "fxr4liangatk35hit-lightSC"
            //    );
            //AddLightIntensityCurveWithCurve(
            //    yellowOrbMultishotHit.transform.GetChild(1).gameObject,
            //    new LightIntensityProps
            //    {
            //        timeMax = 1.6f,
            //        loop = false,
            //        randomStart = false,
            //        enableNegativeLights = false
            //    },
            //    "fxr4liangatk35hit-spjere"
            //    );
            yellowOrbMultishotHit = ModifyEffect(yellowOrbMultishotHit, "", true);

            yellowOrbDomainBulletLeftovers = GetGameObjectFromBundle("fxr4liangatk35dandao");
            yellowOrbDomainBulletLeftovers.AddComponent<DestroyOnTimer>().duration = 150f;

            yellowOrbDomainClone = GetGameObjectFromBundle("YellowOrbDomainClone");
            LeeHyperrealCloneFlicker flicker = yellowOrbDomainClone.AddComponent<LeeHyperrealCloneFlicker>();
        }

        private static void PopulateBlueOrbAssets()
        {
            GameObject blueOrbShot = GetGameObjectFromBundle("fxr4liangatk20");
            ModifyEffect(blueOrbShot, "Play_c_liRk4_skill_blue", true);

            GameObject blueOrbHit = GetGameObjectFromBundle("fxr4liangatk20hit");
            ModifyEffect(blueOrbHit, "Play_c_liRk4_imp_blue", true);

            GameObject blueOrbGroundHit = GetGameObjectFromBundle("fxr4liangatk20bao");
            ModifyEffect(blueOrbGroundHit, "Play_c_liRk4_skill_blue_dilie", true, 3f);

            particleDictionary.Add("blueOrbShot", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, blueOrbShot));
            particleDictionary.Add("blueOrbHit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, blueOrbHit));
            particleDictionary.Add("blueOrbGroundHit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, blueOrbGroundHit));
        }

        private static void PopulateRedOrbAssets()
        {
            GameObject redOrbSwing = GetGameObjectFromBundle("fxr4liangatk10");
            ModifyEffect(redOrbSwing, "Play_c_liRk4_skill_red", true);

            GameObject redOrbHit = GetGameObjectFromBundle("fxr4liangatk10hit02");
            ModifyEffect(redOrbHit, "Play_c_liRk4_imp_red_1", false);

            GameObject redOrbPingSwing = GetGameObjectFromBundle("fxr4liangatk11");
            ModifyEffect(redOrbPingSwing, "", false);

            GameObject redOrbPingGround = GetGameObjectFromBundle("fxr4liangatk11dilie");
            ModifyEffect(redOrbPingGround, "", false);

            GameObject redOrbDomainFloorImpact = GetGameObjectFromBundle("fxr4liangatk14dilie");
            ModifyEffect(redOrbDomainFloorImpact, "Play_c_liRk4_atk_ex_2", false);

            GameObject redOrbDomainHit = GetGameObjectFromBundle("fxr4liangatk14fshit01");
            ModifyEffect(redOrbDomainHit, "Play_c_liRk4_imp_ex_2_2", false);

            GameObject redOrbDomainClone = GetGameObjectFromBundle("RedOrbDomainClone");
            redOrbDomainClone.AddComponent<LeeHyperrealCloneFlicker>();

            GameObject redOrbDomainCloneStart = GetGameObjectFromBundle("RedOrbDomainCloneStart");
            redOrbDomainCloneStart.AddComponent<LeeHyperrealCloneFlicker>();

            particleDictionary.Add("redOrbSwing", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbSwing));
            particleDictionary.Add("redOrbHit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbHit));
            particleDictionary.Add("redOrbPingSwing", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbPingSwing));
            particleDictionary.Add("redOrbPingGround", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbPingGround));
            particleDictionary.Add("redOrbDomainFloorImpact", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbDomainFloorImpact));
            particleDictionary.Add("redOrbDomainHit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbDomainHit));
            particleDictionary.Add("redOrbDomainClone", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbDomainClone));
            particleDictionary.Add("redOrbDomainCloneStart", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, redOrbDomainCloneStart));
        }

        private static void PopulatePrimary5Assets()
        {
            GameObject primary5Swing = GetGameObjectFromBundle("fxr4liangatk05");
            ModifyEffect(primary5Swing, "Play_c_liRk4_atk_nml_5_jump", true, 1.5f);

            GameObject primary5Floor = GetGameObjectFromBundle("fxr4liangatk05dilie");
            ModifyEffect(primary5Floor, "Play_c_liRk4_atk_nml_5_dilie", false, 1.5f);

            particleDictionary.Add("primary5Swing", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary5Swing));
            particleDictionary.Add("primary5Floor", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary5Floor));
        }

        private static void PopulatePrimary4Assets()
        {
            GameObject primary4Swing = GetGameObjectFromBundle("fxr4liangatk04");
            ModifyEffect(primary4Swing, "", true, 2f);

            GameObject primary4AfterImage = GetGameObjectFromBundle("fxr4liangatk04canying");
            ModifyEffect(primary4AfterImage, "", true, 2f);

            GameObject primary4Hit = GetGameObjectFromBundle("fxr4liangatk04hit");
            ModifyEffect(primary4Hit, "", true);

            particleDictionary.Add("primary4Swing", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary4Swing));
            particleDictionary.Add("primary4AfterImage", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary4AfterImage));
            particleDictionary.Add("primary4Hit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary4Hit));
        }

        public static void PopulatePrimary3Assets() 
        {
            GameObject primary3Swing1 = GetGameObjectFromBundle("fxr4liangatk03dilie1");
            ModifyEffect(primary3Swing1, "Play_c_liRk4_atk_nml_3_dilie_1", true);

            GameObject primary3Swing2 = GetGameObjectFromBundle("fxr4liangatk03dilie2");
            ModifyEffect(primary3Swing2, "Play_c_liRk4_atk_nml_3_dilie_2", true);

            GameObject primary3hit = GetGameObjectFromBundle("fxr4liangatk03hit01");
            ModifyEffect(primary3hit, "", false, 1.5f);

            particleDictionary.Add("primary3Swing1", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary3Swing1));
            particleDictionary.Add("primary3Swing2", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary3Swing2));
            particleDictionary.Add("primary3Hit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary3hit));
        }

        public static void PopulatePrimary2Assets()
        {
            GameObject primary2Shot = GetGameObjectFromBundle("fxr4liangatk02");
            ModifyEffect(primary2Shot, "Play_c_liRk4_atk_nml_2", true);

            GameObject primary2hit1 = GetGameObjectFromBundle("fxr4liangatk02hit01");
            ModifyEffect(primary2hit1, "", true);

            GameObject primary2hit2 = GetGameObjectFromBundle("fxr4liangatk02hit02");
            ModifyEffect(primary2hit2, "", true);

            particleDictionary.Add("primary2Shot", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary2Shot));
            particleDictionary.Add("primary2hit1", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary2hit1));
            particleDictionary.Add("primary2hit2", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary2hit2));
        }

        public static void PopulatePrimary1Assets() 
        {
            GameObject primary1Swing = GetGameObjectFromBundle("fxr4liangatk01");
            ModifyEffect(primary1Swing, "Play_c_liRk4_atk_nml_1", true);

            GameObject primary1Hit = GetGameObjectFromBundle("fxr4liangatk01hit");
            ModifyEffect(primary1Hit, "", true);

            GameObject primary1Floor = GetGameObjectFromBundle("fxr4liangatk01dilie");
            ModifyEffect(primary1Floor, "", true);


            particleDictionary.Add("primary1Swing", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary1Swing));
            particleDictionary.Add("primary1Hit", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary1Hit));
            particleDictionary.Add("primary1Floor", new ParticleVariant(DEFAULT_PARTICLE_VARIANT, primary1Floor));
        }

        private static void CreateMaterialStorage(AssetBundle inAssetBundle)
        {
            Material[] tempArray = inAssetBundle.LoadAllAssets<Material>();

            materialStorage.AddRange(tempArray);
        }


        private static Dictionary<string, string> nameConversion = new Dictionary<string, string>()
        {
            ["StubbedShader/Deferred/Standard"] = "Hopoo Games/Deferred/Standard",
            ["StubbedShader/UI/Default Overbrighten"] = "Hopoo Games/UI/Default Overbrighten",
            ["stubbed_Hopoo Games/FX/Cloud Remap Proxy"] = "Hopoo Games/FX/Cloud Remap",
            ["StubbedRoR2/Base/Shaders/HGDistortion"] = "Hopoo Games/FX/Distortion"
        };
    }
}
