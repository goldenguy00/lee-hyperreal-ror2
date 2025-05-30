﻿using LeeHyperrealMod.Content.Notifications;
using LeeHyperrealMod.Modules.Notifications;
using RoR2;
using RoR2.CharacterAI;
using LeeHyperrealMod.Modules;
using RoR2.UI;
using System;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RoR2.Skills.SkillFamily;
using System.Runtime.CompilerServices;

namespace LeeHyperrealMod.Content.Controllers
{
    internal class LeeHyperrealUIController : MonoBehaviour
    {
        [Serializable]
        public class ForcedUIReInitException : Exception
        {
            public ForcedUIReInitException() : base()
            { }

            public ForcedUIReInitException(string message) : base(message)
            { }

            public ForcedUIReInitException(string message, Exception innerException) : base(message, innerException)
            { }
        }

        private CharacterBody characterBody;
        private CharacterMaster characterMaster;

        private HUD RoRHUD;
        private Transform RoRHUDSpringCanvasTransform;
        private ChildLocator RoRHUDChildLoc;

        private OrbController orbController;
        private LeeHyperrealDomainController domainController;
        private LeeHyperrealPassive passiveController;
        private RectTransform OrbPositionComponent;

        public bool baseAIPresent;
        public bool enabledUI;

        public bool failedToInitialize;
        public const int MAX_FAIL_ATTEMPTS = 50;
        public int failAttempts = 0;

        public uint selectedSkin = 0;
        public float rgbOffset = 0f;

        #region Orb Variables
        private int maxShownOrbs = 8;
        private int startIndex = 1;
        private int endIndex = 8;
        private int orbAmountIndex = 0;
        GameObject orbUIObject;
        Transform orbUIObjectExtraParent;
        HGTextMeshProUGUI orbAmountLabel;
        List<Animator> orbAnimators;
        List<Image> orbImages;

        private int blueBracketIndex = 11;
        private int redBracketIndex = 12;
        private int yellowBracketIndex = 13;
        private float yAxisBracketFloat;

        //REGRET
        internal class BracketContainerProps 
        {
            public HGTextMeshProUGUI orbLabel;
            public Vector3 glyphSpeed;
            public Vector3 targetPosition;
            public GameObject bracketContainer;
            public Animator[] animators; // 1-3 wide brackets in order!

            public BracketContainerProps() { }
        }

        BracketContainerProps blueSimpleGlyph;
        BracketContainerProps redSimpleGlyph;
        BracketContainerProps yellowSimpleGlyph;

        GameObject emptyParent;

        public enum BracketType 
        {
            ONE,
            TWO,
            THREE,
        }
        #endregion

        #region Power Meter
        private int meterindex = 15;
        private GameObject powerMeterUIObject;
        private Transform powerMeterUIObjectExtraParent;
        private Transform powerMeterUIObjectBullet;
        private Animator meterAnimator;
        private bool isLerpingBetweenValues;
        private const float lerpSpeed = 10f;
        private float targetMeterAmount;
        private float currentMeterAmount;
        private Material powerBarFilledMaterial;
        #endregion

        #region Invincibility Layer
        private GameObject healthLayers;
        private GameObject layerInvincibilityHealthObject;
        private GameObject layerInvincibilityHazeObject;
        private Image invincibilityBorder;
        private GameObject layerInvincibilityHealthObjectHunk;
        private GameObject layerInvincibilityHazeObjectHunk;
        private Image invincibilityBorderHunk;
        private int healthIndex = 2;
        private int hazeIndex = 3;
        #endregion

        #region Ammo Management
        private int bulletIndex = 0; //Starts at 0 in the Bullet Parent section
        private int endBulletIndex = 4;
        private int parryPoweredIndex = 5; // Starts at 5 in the Bullet Parent section
        private int endParryPoweredIndex = 9; // Starts at 5 in the Bullet Parent section
        private int incomingParryBulletIndex = 10;
        private int extraParryPoweredIndex = 11; // Starts at 11 in the Bullet Parent section
        private int endExtraParryPoweredIndex = 25; // Starts at 11 in the Bullet Parent section
        private int incomingExtraParryPoweredIndex = 26; // Starts at 26 in the Bullet Parent section
        List<GameObject> bulletObjects;
        List<GameObject> parryBullets;
        List<GameObject> extraParryBullets;
        GameObject IncomingParryBullet;
        GameObject IncomingExtraParryBullet;
        BulletTriggerComponent trigger;

        public struct BulletState
        {
            public List<BulletController.BulletType> bulletTypes;
            public int parryBulletCount;
            public float bulletAnimationSpeed;
            public bool isColouredBulletMoving;
            public bool isEnhancedBulletMoving;

            public BulletState(int parryCount, List<BulletController.BulletType> types, float bulletAnimationSpeed, bool isColouredBulletMoving, bool isEnhancedBulletMoving)
            {
                bulletTypes = types;
                parryBulletCount = parryCount;
                this.bulletAnimationSpeed = bulletAnimationSpeed;
                this.isColouredBulletMoving = isColouredBulletMoving;
                this.isEnhancedBulletMoving = isEnhancedBulletMoving;
            }
        }
        private BulletState targetBulletState;
        #endregion

        #region Domain overlay
        private GameObject domainOverlayObject;
        private GameObject ceaseDomainOverlayObject;
        private bool spawnedEffect;
        #endregion

        #region Ultimate Indicator
        private GameObject ultimateIndicatorObject;
        private Animator ultimateIndicatorAnimator;
        #endregion

        #region Hold OK tag
        HGTextMeshProUGUI holdOKTag;
        #endregion

        #region Crosshair
        private GameObject crosshairObject;
        private Animator crosshairAnimator;
        public bool shouldOverrideAutoSprintingStateCrosshair;
        #endregion

        #region Skill Icons
        private List<Material> skillIconMaterials;
        #endregion

        private bool isInitialized = false;

        private HGTextMeshProUGUI CreateLabel(Transform parent, string name, string text, Vector2 position, float textScale)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            gameObject.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = gameObject.AddComponent<HGTextMeshProUGUI>();
            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = textScale;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = false;
            rectTransform.localPosition = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = position;
            return hgtextMeshProUGUI;
        }

        #region Unity MonoBehvaiour Functions
        public void Awake()
        {
            enabledUI = false;
            baseAIPresent = false;
            orbAnimators = new List<Animator>();
            orbImages = new List<Image>();
        }


        //Populate the internal variables
        public void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            orbController = GetComponent<OrbController>();
            domainController = GetComponent<LeeHyperrealDomainController>();
            passiveController = GetComponent<LeeHyperrealPassive>();
            characterMaster = characterBody.master;
            BaseAI baseAI = characterMaster.GetComponent<BaseAI>();
            baseAIPresent = baseAI;
            Hook();


            //For some reason on goboo's first spawn the master is just not there. However subsequent spawns work.
            // Disable the UI in this event.
            // Besides, there should never be a UI element related to a non-existant master on screen if the attached master/charbody does not exist.
            if (!characterMaster) baseAIPresent = true; // Disable UI Just in case.

            selectedSkin = characterBody.skinIndex;

            try
            {
                InitializeUI();
            }
            catch (NullReferenceException e)
            {
                Debug.Log($"Lee: Hyperreal - NRE on UI Initialization, trying again: {e}");
            }
            catch (ForcedUIReInitException) 
            {
                Debug.Log("Lee: Hyperreal - UI needs to Reinit!");
            }

            if (Modules.Config.loreMode.Value)
            {
                characterBody.portraitIcon = characterBody.skinIndex switch
                {
                    0 => Modules.LeeHyperrealAssets.prospectorSprite.texture,
                    1 => Modules.LeeHyperrealAssets.comradeSprite.texture,
                    4 => Modules.LeeHyperrealAssets.scarletSprite.texture,
                    _ => Modules.LeeHyperrealAssets.leeIconSprite.texture,
                };
            }
            else
            {
                characterBody.portraitIcon = characterBody.skinIndex switch
                {
                    2 => Modules.LeeHyperrealAssets.scarletSprite.texture,
                    3 => Modules.LeeHyperrealAssets.prospectorSprite.texture,
                    4 => Modules.LeeHyperrealAssets.comradeSprite.texture,
                    _ => Modules.LeeHyperrealAssets.leeIconSprite.texture,
                };
            }
        }

        public float ResolveColor()
        {
            if (passiveController)
            {
                switch (passiveController.GetVFXPassive())
                {
                    case LeeHyperrealPassive.VFXPassive.RED:
                        return Modules.StaticValues.redBaseHueOffset;
                    case LeeHyperrealPassive.VFXPassive.ORANGE:
                        return Modules.StaticValues.orangeBaseHueOffset;
                    case LeeHyperrealPassive.VFXPassive.YELLOW:
                        return Modules.StaticValues.yellowBaseHueOffset;
                    case LeeHyperrealPassive.VFXPassive.GREEN:
                        return Modules.StaticValues.greenBaseHueOffset;
                    case LeeHyperrealPassive.VFXPassive.BLUE:
                        return Modules.StaticValues.defaultBaseHueOffset; // Don't set anything, it's the default blue.
                    case LeeHyperrealPassive.VFXPassive.LIGHTBLUE:
                        return Modules.StaticValues.lightBlueBaseHueOffset;
                    case LeeHyperrealPassive.VFXPassive.VIOLET:
                        return Modules.StaticValues.violetBaseHueOffset;
                    case LeeHyperrealPassive.VFXPassive.PINK:
                        return Modules.StaticValues.pinkBaseHueOffset;
                }
            }

            //Do skin resolution
            if (IsRedSkin())
            {
                return Modules.StaticValues.redBaseHueOffset;
            }

            if (IsOrangeSkin())
            {
                return Modules.StaticValues.orangeBaseHueOffset;
            }

            if (IsLightBlueSkin()) 
            {
                return Modules.StaticValues.lightBlueBaseHueOffset;
            }


            // I GIVE UP, LEAVE IT BLUEEEE
            return Modules.StaticValues.defaultBaseHueOffset;
        }

        public bool IsRedSkin() 
        {
            return LeeHyperrealMod.Modules.Survivors.LeeHyperreal.redVFXSkins.Contains((int)selectedSkin);
        }

        public bool IsOrangeSkin() 
        {
            return LeeHyperrealMod.Modules.Survivors.LeeHyperreal.orangeVFXSkins.Contains((int)selectedSkin);
        }

        public bool IsLightBlueSkin()
        {
            return LeeHyperrealMod.Modules.Survivors.LeeHyperreal.lightBlueVFXSkins.Contains((int)selectedSkin);
        }

        public void InitializeUI() 
        {
            if (!isInitialized && !baseAIPresent)
            {
                InitializeRoRHUD();
                //Initialize stuff that's custom.
                InitializePowerMeter();
                InitializeHealthLayer();
                InitializeBulletUI();
                InitializeUltimateIndicator();
                InitializeOrbAnimatorArray();
                InitializeHoldOKTag();
                //Now we need to initialize everything inside the canvas to variables we can control.
                InitializeOrbAmountLabel();
                InitializeOrbBrackets();
                InitializeCrosshair();
                InitializeSkillIconMaterial();

                if (orbController)
                {
                    UpdateOrbList(orbController.orbList);
                }
                isInitialized = true;
            }
        }

        private void InitializeRoRHUD()
        {
            if (RoRHUD)
            {
                // Get this transform for easier reference.
                RoRHUDSpringCanvasTransform = RoRHUDChildLoc.FindChild("SpringCanvas") ?? RoRHUD.mainUIPanel.transform.Find("SpringCanvas");

                var notificationArea = RoRHUDChildLoc.FindChild("NotificationArea")?.gameObject ?? RoRHUD.mainContainer.transform.Find("NotificationArea").gameObject;

                var leeUINotifController = notificationArea.GetComponent<LeeHyperrealUINotificationController>() 
                                        ?? notificationArea.AddComponent<LeeHyperrealUINotificationController>();

                var queue = RoRHUD.targetMaster.GetComponent<LeeHyperrealNotificationQueue>()
                         ?? RoRHUD.targetMaster.gameObject.AddComponent<LeeHyperrealNotificationQueue>();

                leeUINotifController.hud = RoRHUD;
                leeUINotifController.genericNotificationPrefab = Modules.LeeHyperrealAssets.leenotificationBoxPrefab;
                leeUINotifController.notificationQueue = queue;
                return;    
            }
            throw new NullReferenceException();
        }

        public void Update()
        {
            if (characterBody.hasEffectiveAuthority)
            {
                if (!isInitialized && !failedToInitialize) 
                {
                    try
                    {
                        InitializeUI();
                    }
                    catch (NullReferenceException e)
                    {
                        Debug.Log($"Lee: Hyperreal - NRE on UI Initialization, trying again: {e}");
                        failAttempts++;

                        if (failAttempts >= MAX_FAIL_ATTEMPTS) 
                        {
                            failedToInitialize = true;
                            Debug.Log($"Lee: Hyperreal UI failed to initialize after more than {MAX_FAIL_ATTEMPTS} attempts. Stopping attempts to initialize.");
                        }
                    }
                    catch (ForcedUIReInitException)
                    {
                        Debug.Log("Lee: Hyperreal - UI needs to Reinit!");
                    }

                    return;
                }

                if (!baseAIPresent && isInitialized) 
                {
                    UpdateHealthUIObject();
                    UpdateMeterLevel();
                    SetAnimatorMeterValue();
                    HandleBulletUIChange();
                    UpdateGlyphPositions();
                    UpdateSprintingCrosshairState();
                    HandleRGBMode();
                    LateUpdatePositions();
                }
            }
        }

        private void HandleRGBMode()
        {
            if (Modules.Config.rgbMode.Value) 
            {
                if (domainController.GetDomainState() || domainController.DomainEntryAllowed())
                {
                    rgbOffset += Time.deltaTime * Modules.Config.rgbPulseSpeed.Value;
                    rgbOffset = rgbOffset % 360f;

                    float offsetOffset = 0f;

                    if (powerBarFilledMaterial)
                    {
                        SetCustomUIMaterial(powerBarFilledMaterial, rgbOffset + offsetOffset, -2f);
                    }
                    
                    foreach (Material mat in skillIconMaterials)
                    {
                        offsetOffset += 10f;
                        SetCustomUIMaterial(mat, (rgbOffset + offsetOffset) % 360f, -2f);
                    }
                }
                else 
                {
                    if (powerBarFilledMaterial)
                    {
                        SetCustomUIMaterial(powerBarFilledMaterial, 0f, -2f);
                    }

                    foreach (Material mat in skillIconMaterials)
                    {
                        SetCustomUIMaterial(mat, 0f, -2f);
                    }
                }
            }
        }

        private void LateUpdatePositions()
        {
            if (!orbUIObject || !RoRHUDSpringCanvasTransform) 
            {
                return;
            }

            if (LeeHyperrealPlugin.isHunkHudInstalled)
            {
                return;
            }

            if (LeeHyperrealPlugin.isRiskUIInstalled)
            {
                orbUIObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                orbUIObject.transform.localPosition = new Vector3(185f, 287.8038f, 0f);
            }
            else // DEFAULT UI 
            {
                if (RoRHUD) 
                {
                    //Move Chatbox up a little bit to not collide with the energy bar.
                    RoRHUDSpringCanvasTransform.GetChild(0).GetChild(0).position = new Vector3(-11.4084f, - 4.3756f, 12.6537f);
                }
            }
        }

        public void FixedUpdate()
        {
            if (characterBody.hasEffectiveAuthority)
            {
                if (!enabledUI && !baseAIPresent)
                {
                    //canvasObject.SetActive(true);

                    if (!spawnedEffect && Camera.main) 
                    {
                        domainOverlayObject = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("domainOverlayEffect", characterBody), new Vector3(0, 0, -0.1f), Quaternion.identity, Camera.main.transform);
                        domainOverlayObject.SetActive(false);
                        spawnedEffect = true;
                    }
                }
            }
        }

        public void OnDestroy()
        {
            //Destroy(canvasObject);
            Destroy(domainOverlayObject);
            Destroy(orbUIObject);
            Destroy(emptyParent);
            Destroy(powerMeterUIObject);
            Destroy(healthLayers);
            Destroy(ultimateIndicatorObject);
            Destroy(crosshairObject);
            UnsetSkillIconMat();
            Unhook();
        }
        #endregion

        #region Invincible Health layer
        public void InitializeHealthLayer()
        {
            if (RoRHUD && !healthLayers)
            {
                if (LeeHyperrealPlugin.isHunkHudInstalled)
                {
                    InitHunkHUDHealthLayer();
                }
                else if (LeeHyperrealPlugin.isRiskUIInstalled)
                {
                    healthLayers = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.healthPrefabs, RoRHUDSpringCanvasTransform.Find("BottomLeftCluster/BarRoots/Healthbar"));
                    healthLayers.transform.rotation = Quaternion.identity;
                    healthLayers.transform.localScale = new Vector3(0.7891f, 0.4f, 1f);
                    healthLayers.transform.position = new Vector3(-9.7021f, -4.8843f, 12.6537f);
                    healthLayers.transform.localPosition = new Vector3(2.3027f, 0.7998f, 0.0003f);
                }
                else if (LeeHyperrealPlugin.isBetterHudInstalled)
                {
                    healthLayers = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.healthPrefabs, RoRHUDSpringCanvasTransform.Find("BottomCenterCluster/BarRoots/HealthbarRoot"));
                }
                else 
                {
                    healthLayers = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.healthPrefabs, RoRHUDSpringCanvasTransform.Find("BottomLeftCluster/BarRoots/HealthbarRoot"));
                }
            }
            layerInvincibilityHealthObject = healthLayers.transform.GetChild(0).gameObject;
            layerInvincibilityHazeObject = healthLayers.transform.GetChild(1).gameObject;
            invincibilityBorder = layerInvincibilityHealthObject.transform.GetChild(0).gameObject.GetComponent<Image>();
            if (LeeHyperrealPlugin.isHunkHudInstalled)
            {
                layerInvincibilityHealthObjectHunk = healthLayers.transform.GetChild(2).gameObject;
                layerInvincibilityHazeObjectHunk = healthLayers.transform.GetChild(2).GetChild(1).gameObject;
                invincibilityBorderHunk = healthLayers.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InitHunkHUDHealthLayer() 
        {
            healthLayers = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.healthPrefabsHunkHud, HunkHud.Components.UI.CustomHealthBar.instance.transform);
        }

        public void SetActiveHealthUIObject(bool state, Color color)
        {
            if (layerInvincibilityHazeObject)
            {
                layerInvincibilityHazeObject.SetActive(state);
                if (layerInvincibilityHazeObjectHunk)
                    layerInvincibilityHazeObjectHunk.SetActive(state);
            }

            if (layerInvincibilityHealthObject)
            {
                layerInvincibilityHealthObject.SetActive(state);
                if (layerInvincibilityHealthObjectHunk)
                    layerInvincibilityHealthObjectHunk.SetActive(state);
            }

            if (invincibilityBorder) 
            {
                invincibilityBorder.color = color;
                if (invincibilityBorderHunk)
                    invincibilityBorderHunk.color = color;
            }
        }

        public void UpdateHealthUIObject() 
        {
            if (!characterBody) 
            {
                return;
            }

            if (characterBody.HasBuff(Modules.Buffs.parryBuff.buffIndex)) 
            {
                SetActiveHealthUIObject(true, Modules.StaticValues.parryInvincibility);
                return;
            }
            SetActiveHealthUIObject(characterBody.HasBuff(Modules.Buffs.invincibilityBuff) || characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility), Modules.StaticValues.blueInvincibility);
        }
        #endregion

        #region Power Meter Functions
        private void InitializePowerMeter()
        {
            if (RoRHUD && !powerMeterUIObject) 
            {
                var bottomLeft = RoRHUDChildLoc.FindChild("BottomLeftCluster") ?? RoRHUDSpringCanvasTransform.Find("BottomLeftCluster");
                if (LeeHyperrealPlugin.isHunkHudInstalled)
                {
                    powerMeterUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.powerMeterObject, bottomLeft);
                    powerMeterUIObjectExtraParent = powerMeterUIObject.transform.GetChild(0);
                    OrbPositionComponent = powerMeterUIObjectExtraParent.GetComponent<RectTransform>();
                    OrbPositionComponent.anchoredPosition = new Vector3(170f, 255f, 0f);
                    
                }
                else if (LeeHyperrealPlugin.isRiskUIInstalled)
                {
                    powerMeterUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.powerMeterObject, bottomLeft);
                    powerMeterUIObject.transform.localScale = new Vector3(1, 1, 1);
                    powerMeterUIObject.transform.localRotation = Quaternion.identity;
                    powerMeterUIObject.transform.localPosition = new Vector3(200f, 220f, -42f);
                    powerMeterUIObjectBullet = powerMeterUIObject.transform.GetChild(1);
                    powerMeterUIObjectBullet.transform.localPosition = new Vector3(-30f, -4f, 11f);

                }
                else if (LeeHyperrealPlugin.isBetterHudInstalled)
                {
                    var bottomCenter = RoRHUDChildLoc.FindChild("BottomCenterCluster") ?? RoRHUDSpringCanvasTransform.Find("BottomCenterCluster");
                    powerMeterUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.powerMeterObject, bottomCenter);
                    powerMeterUIObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    powerMeterUIObject.transform.localRotation = Quaternion.identity;
                    powerMeterUIObject.transform.localPosition = new Vector3(-500f, 100.709f, - 47.7458f);
                }
                else
                {
                    powerMeterUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.powerMeterObject, bottomLeft);
                }
            }

            meterAnimator = powerMeterUIObject.GetComponent<Animator>();
            powerBarFilledMaterial = powerMeterUIObject.transform.GetChild(0).Find("Power Bar Filled").GetComponent<Image>().material;

            //Setup red variant

            SetCustomUIMaterial(powerBarFilledMaterial, 0f, ResolveColor());
        }

        private void SetCustomUIMaterial(Material mat, float hueShiftDeg, float baseHueOffset) 
        {
            if (baseHueOffset >= -1.1f) 
            {
                mat.SetFloat("_BaseHueOffset", baseHueOffset);
            }

            mat.SetFloat("_HueShiftDegrees", hueShiftDeg);
        }

        public void SetMeterLevel(float percentageFill)
        {
            isLerpingBetweenValues = true;

            if (percentageFill >= 1f)
            {
                targetMeterAmount = 0.999f;
                return;
            }

            if (percentageFill < 0f)
            {
                targetMeterAmount = 0f;
                return;
            }

            targetMeterAmount = percentageFill;

        }

        public void UpdateMeterLevel()
        {
            if (isLerpingBetweenValues)
            {
                if (currentMeterAmount <= targetMeterAmount)
                {
                    currentMeterAmount += Time.deltaTime * lerpSpeed;
                    if (currentMeterAmount >= targetMeterAmount)
                    {
                        currentMeterAmount = targetMeterAmount;
                        isLerpingBetweenValues = false;
                    }
                }
                if (currentMeterAmount >= targetMeterAmount)
                {
                    currentMeterAmount -= Time.deltaTime * lerpSpeed;
                    if (currentMeterAmount <= targetMeterAmount)
                    {
                        currentMeterAmount = targetMeterAmount;
                        isLerpingBetweenValues = false;
                    }
                }
            }
        }

        private void SetAnimatorMeterValue()
        {
            if (meterAnimator) 
            {
                meterAnimator.SetFloat("bar fill", currentMeterAmount);
            }
        }

        #endregion

        #region Orb Functions

        private void InitializeOrbBrackets()
        {
            if (blueSimpleGlyph == null)
            {
                blueSimpleGlyph = new BracketContainerProps();
            }
            if (redSimpleGlyph == null)
            {
                redSimpleGlyph = new BracketContainerProps();
            }
            if (yellowSimpleGlyph == null)
            {
                yellowSimpleGlyph = new BracketContainerProps();
            }

            if (!blueSimpleGlyph.bracketContainer) { blueSimpleGlyph.bracketContainer = orbUIObject.transform.GetChild(0).GetChild(blueBracketIndex).gameObject; }
            if (!redSimpleGlyph.bracketContainer) { redSimpleGlyph.bracketContainer = orbUIObject.transform.GetChild(0).GetChild(redBracketIndex).gameObject; }
            if (!yellowSimpleGlyph.bracketContainer) { yellowSimpleGlyph.bracketContainer = orbUIObject.transform.GetChild(0).GetChild(yellowBracketIndex).gameObject; }

            //Go through each object and destroy the text label and replace it.

            Transform blueLabel = blueSimpleGlyph.bracketContainer.transform.GetChild(0);
            Destroy(blueLabel.gameObject.GetComponent<Text>());
            blueSimpleGlyph.orbLabel = CreateLabel(blueLabel, "Hotkey", DeAlphaizeString(Modules.Config.blueOrbTrigger.Value.ToString()), Vector2.zero, 24f);

            Transform redlabel = redSimpleGlyph.bracketContainer.transform.GetChild(0);
            Destroy(redlabel.gameObject.GetComponent<Text>());
            redSimpleGlyph.orbLabel = CreateLabel(redlabel, "Hotkey", DeAlphaizeString(Modules.Config.redOrbTrigger.Value.ToString()), Vector2.zero, 24f);

            Transform yellowLabel = yellowSimpleGlyph.bracketContainer.transform.GetChild(0);
            Destroy(yellowLabel.gameObject.GetComponent<Text>());
            yellowSimpleGlyph.orbLabel = CreateLabel(yellowLabel, "Hotkey", DeAlphaizeString(Modules.Config.yellowOrbTrigger.Value.ToString()), Vector2.zero, 24f);

            //Get Animators for glyphs 
            blueSimpleGlyph.animators = new Animator[3];
            for (int i = 1; i < blueSimpleGlyph.bracketContainer.transform.childCount; i++)
            {
                blueSimpleGlyph.animators[i - 1] = blueSimpleGlyph.bracketContainer.transform.GetChild(i).GetChild(0).GetComponent<Animator>();
            }
            redSimpleGlyph.animators = new Animator[3];
            for (int i = 1; i < redSimpleGlyph.bracketContainer.transform.childCount; i++)
            {
                redSimpleGlyph.animators[i - 1] = redSimpleGlyph.bracketContainer.transform.GetChild(i).GetChild(0).GetComponent<Animator>();
            }
            yellowSimpleGlyph.animators = new Animator[3];
            for (int i = 1; i < yellowSimpleGlyph.bracketContainer.transform.childCount; i++)
            {
                yellowSimpleGlyph.animators[i - 1] = yellowSimpleGlyph.bracketContainer.transform.GetChild(i).GetChild(0).GetComponent<Animator>();
            }

            if (LeeHyperrealPlugin.isRiskUIInstalled)
            {
                yAxisBracketFloat = Modules.StaticValues.yAxisPositionBracketsRiskUI;
            }
            else 
            {
                yAxisBracketFloat = Modules.StaticValues.yAxisPositionBrackets;
            }

            blueSimpleGlyph.targetPosition = new Vector3(0, yAxisBracketFloat, 0f);
            redSimpleGlyph.targetPosition = new Vector3(0, yAxisBracketFloat, 0f);
            yellowSimpleGlyph.targetPosition = new Vector3(0, yAxisBracketFloat, 0f);
        }

        private void ExitAllSimpleBrackets() 
        {
            for (int i = 0; i < 3; i++) 
            {
                blueSimpleGlyph.animators[i].SetTrigger("Exit");
                redSimpleGlyph.animators[i].SetTrigger("Exit");
                yellowSimpleGlyph.animators[i].SetTrigger("Exit");
            }
        }

        private void SetColorForBrackets() 
        {
            for (int i = 0; i < 3; i++)
            {
                //Modify all animators.
                blueSimpleGlyph.animators[i].SetTrigger("Blue");
                redSimpleGlyph.animators[i].SetTrigger("Red");
                yellowSimpleGlyph.animators[i].SetTrigger("Yellow");
            }
        }

        private void UpdateGlyphPositions()
        {
            if (blueSimpleGlyph == null || redSimpleGlyph == null || yellowSimpleGlyph == null) 
            {
                return;
            }

            if (blueSimpleGlyph.bracketContainer)
            {
                blueSimpleGlyph.bracketContainer.transform.localPosition = Vector3.SmoothDamp(blueSimpleGlyph.bracketContainer.transform.localPosition, blueSimpleGlyph.targetPosition, ref blueSimpleGlyph.glyphSpeed, 0.15f);
            }
            if (redSimpleGlyph.bracketContainer)
            {
                redSimpleGlyph.bracketContainer.transform.localPosition = Vector3.SmoothDamp(redSimpleGlyph.bracketContainer.transform.localPosition, redSimpleGlyph.targetPosition, ref redSimpleGlyph.glyphSpeed, 0.15f);
            }
            if (yellowSimpleGlyph.bracketContainer) 
            {
                yellowSimpleGlyph.bracketContainer.transform.localPosition = Vector3.SmoothDamp(yellowSimpleGlyph.bracketContainer.transform.localPosition, yellowSimpleGlyph.targetPosition, ref yellowSimpleGlyph.glyphSpeed, 0.15f);

            }
        }

        private string DeAlphaizeString(string input) 
        {
            return input.Replace("Alpha", "");
        }

        private void InitializeOrbAmountLabel()
        {
            Transform labeltransform = orbUIObject.transform.GetChild(0).GetChild(orbAmountIndex);
            Destroy(labeltransform.gameObject.GetComponent<Text>());
            if (!orbAmountLabel) 
            {
                orbAmountLabel = CreateLabel(labeltransform, "Orb Amount", "0 / 16", Vector2.zero, 24f);
            }
        }

        public void UpdateOrbAmount(int amount, int max) 
        {
            if (orbAmountLabel) 
            {
                orbAmountLabel.SetText($"{amount} / {max}");
            }   
        }

        public void DisableBracket(OrbController.OrbType orbType) 
        {
            if (blueSimpleGlyph == null || redSimpleGlyph == null || yellowSimpleGlyph == null)
            {
                return;
            }

            switch (orbType)
            {
                case OrbController.OrbType.BLUE:
                    for (int i = 0; i < 3; i++)
                    {
                        blueSimpleGlyph.animators[i].SetTrigger("Exit");
                    }
                    blueSimpleGlyph.orbLabel.gameObject.SetActive(false);
                    break;
                case OrbController.OrbType.RED:
                    for (int i = 0; i < 3; i++)
                    {
                        redSimpleGlyph.animators[i].SetTrigger("Exit");
                    }
                    redSimpleGlyph.orbLabel.gameObject.SetActive(false);
                    break;
                case OrbController.OrbType.YELLOW:
                    for (int i = 0; i < 3; i++)
                    {
                        yellowSimpleGlyph.animators[i].SetTrigger("Exit");
                    }
                    yellowSimpleGlyph.orbLabel.gameObject.SetActive(false);
                    break;
            }
        }

        public void SetBracketOnOrb(int position, BracketType bracketType, OrbController.OrbType orbType)
        {
            //position is zero-indexed!
            //This function takes a position and sets the bracket 
            // If the selection is a 3 ping but positions on the 7th index, then we do nothing.

            bool exitCondition = (bracketType == BracketType.THREE && position >= 6)
                    || (bracketType == BracketType.TWO && position >= 7)
                    || (position > 7);
            //Check Bracket type and position.
            if (exitCondition) 
            {
                //NO
                return;
            }

            if (blueSimpleGlyph == null || redSimpleGlyph == null || yellowSimpleGlyph == null) 
            {
                return;
            }

            if (!blueSimpleGlyph.bracketContainer || !redSimpleGlyph.bracketContainer || !yellowSimpleGlyph.bracketContainer)
            {
                return;
            }

            //Assuming the situation of RR BBB Y BB
            // if position 2 is given, get the 3rd object, B, extend by bracketType, and then position in the center object (object at position 2 + 1)
            // If position 0 is given, get the first object, R, get pos + 1 and half distance, put the bracket at that position.
            // If position 5 is given, get the object and place underneath it.
            switch (orbType) 
            {
                case OrbController.OrbType.BLUE:
                    blueSimpleGlyph.targetPosition = HandleBracketTransition(blueSimpleGlyph, position, bracketType);
                    blueSimpleGlyph.orbLabel.gameObject.SetActive(true);
                    break;
                case OrbController.OrbType.RED:
                    redSimpleGlyph.targetPosition = HandleBracketTransition(redSimpleGlyph, position, bracketType);
                    redSimpleGlyph.orbLabel.gameObject.SetActive(true);
                    break;
                case OrbController.OrbType.YELLOW:
                    yellowSimpleGlyph.targetPosition = HandleBracketTransition(yellowSimpleGlyph, position, bracketType);
                    yellowSimpleGlyph.orbLabel.gameObject.SetActive(true);
                    break;
            }           
        }

        public Vector3 HandleBracketTransition(BracketContainerProps bracketContainerProps, int position, BracketType bracketType) 
        {
            if (!bracketContainerProps.bracketContainer) 
            {
                return new Vector3();
            }

            SetColorForBrackets();

            return bracketType switch
            {
                BracketType.ONE => HandleBracketOne(bracketContainerProps, position),
                BracketType.TWO => HandleBracketTwo(bracketContainerProps, position),
                BracketType.THREE => HandleBracketThree(bracketContainerProps, position),
                _ => new Vector3(),
            };
        }

        public Vector3 HandleBracketOne(BracketContainerProps bracketContainerProps, int position)
        {
            //Get the one orb bracket, set the rest to exit.
            bracketContainerProps.animators[0].SetTrigger("Enter");
            bracketContainerProps.animators[1].SetTrigger("Exit");
            bracketContainerProps.animators[2].SetTrigger("Exit");

            //Set right under the orb.
            Vector3 newPosition = orbAnimators[position].transform.localPosition;
            newPosition.y = yAxisBracketFloat;
            return newPosition;
        }

        public Vector3 HandleBracketTwo(BracketContainerProps bracketContainerProps, int position)
        {
            //Get the two orb bracket
            bracketContainerProps.animators[0].SetTrigger("Exit");
            bracketContainerProps.animators[1].SetTrigger("Enter");
            bracketContainerProps.animators[2].SetTrigger("Exit");

            //Set in between position and position + 1
            Vector3 newPosition = orbAnimators[position].transform.localPosition + orbAnimators[position + 1].transform.localPosition;
            newPosition = newPosition / 2f;
            newPosition.y = yAxisBracketFloat;
            return newPosition;
        }

        public Vector3 HandleBracketThree(BracketContainerProps bracketContainerProps, int position)
        {
            //Get the three orb bracket
            bracketContainerProps.animators[0].SetTrigger("Exit");
            bracketContainerProps.animators[1].SetTrigger("Exit");
            bracketContainerProps.animators[2].SetTrigger("Enter");

            //Set in between position and position + 1
            Vector3 newPosition = orbAnimators[position + 1].transform.localPosition;
            newPosition.y = yAxisBracketFloat;

            return newPosition;
        }


        private void InitializeOrbAnimatorArray()
        {
            if (RoRHUD && !orbUIObject) 
            {
                if (LeeHyperrealPlugin.isHunkHudInstalled)
                {
                    orbUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.orbsUIObject, RoRHUDSpringCanvasTransform.Find("BottomCenterCluster"));
                    orbUIObjectExtraParent = orbUIObject.transform.GetChild(0);
                    OrbPositionComponent = orbUIObjectExtraParent.GetComponent<RectTransform>();
                    OrbPositionComponent.anchoredPosition3D = new Vector3(60f, 150f, 0f);
                    OrbPositionComponent.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                }
                else if (LeeHyperrealPlugin.isRiskUIInstalled)
                {
                    emptyParent = UnityEngine.GameObject.Instantiate(new GameObject("Empty Orb Parent"), RoRHUDSpringCanvasTransform.Find("BottomLeftCluster"));
                    orbUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.orbsUIObject, RoRHUDSpringCanvasTransform.Find("BottomLeftCluster"));
                    orbUIObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    orbUIObject.transform.position = new Vector3(-9.5764f, -3.2462f, 12.6537f);
                    orbUIObject.transform.localPosition = new Vector3(185f, 287.8038f, 0f);
                    orbUIObject.transform.SetParent(emptyParent.transform);
                }
                else if (LeeHyperrealPlugin.isBetterHudInstalled)
                {
                    orbUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.orbsUIObject, RoRHUDSpringCanvasTransform.Find("BottomCenterCluster"));
                    orbUIObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    orbUIObject.transform.localRotation = Quaternion.identity;
                    orbUIObject.transform.localPosition = new Vector3(-425f, 78.2634f, 0f);
                }
                else 
                {
                    orbUIObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.orbsUIObject, RoRHUDSpringCanvasTransform.Find("BottomCenterCluster"));
                    
                }
            }

            // blegh not modular at all
            for (int i = startIndex; i <= endIndex; i++)
            {
                orbAnimators.Add(orbUIObject.transform.GetChild(0).GetChild(i).GetComponent<Animator>());
                orbImages.Add(orbUIObject.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>());
            }

            UpdateOrbList(new List<OrbController.OrbType>());// Update with empty list.
        }

        //Index is 0 indexed.
        public void PingSpecificOrb(int index)
        {
            if (!isInitialized || failedToInitialize) 
            {
                return;
            }
            try
            {
                orbAnimators[index].SetTrigger("isPinged");
            }
            catch (IndexOutOfRangeException e)
            {
                //do nothing really.
                //Mask the error.
                Debug.Log(e);
            }
        }

        public void UpdateOrbList(List<OrbController.OrbType> orbsList)
        {
            if (!isInitialized || failedToInitialize) 
            {
                return;
            }

            if (orbsList.Count == 0)
            {
                //Clear everything
                for (int i = 0; i < orbAnimators.Count; i++)
                {
                    orbAnimators[i].SetBool("spawned", false);
                    orbAnimators[i].SetBool("hidden", true);
                }
                return;
            }

            // Go through the list
            // Determine what orbs should show using material setting.

            int maxOrbCount = Mathf.Min(maxShownOrbs, orbsList.Count);

            for (int i = 0; i < maxShownOrbs; i++)
            {
                if (i < maxOrbCount)
                {
                    orbImages[i].material = SelectOrbMaterial(orbsList[i]);
                    orbAnimators[i].SetBool("spawned", true);
                    orbAnimators[i].SetBool("hidden", false);
                }
                else
                {
                    orbAnimators[i].SetBool("hidden", true);
                    orbAnimators[i].SetBool("spawned", false);
                }
            }

            //if (orbsList.Count < maxShownOrbs) 
            //{
            //    //Go through all the orbs outside orbList.Count and silent clear.
            //    int diff = maxShownOrbs - orbsList.Count;

            //    for (int i = maxShownOrbs - 1; i > orbsList.Count - 1; i--) 
            //    {
            //        orbAnimators[i].SetTrigger("Silent Clear");                    
            //    }
            //}
        }

        public Material SelectOrbMaterial(OrbController.OrbType orb) => orb switch
        {
            OrbController.OrbType.RED => Modules.LeeHyperrealAssets.redOrbMat,
            OrbController.OrbType.YELLOW => Modules.LeeHyperrealAssets.yellowOrbMat,
            OrbController.OrbType.BLUE => Modules.LeeHyperrealAssets.blueOrbMat,
            _ => null,
        };

        #endregion

        #region Bullet UI Functions

        private void HandleBulletUIChange()
        {
            //The UI constantly checks if we should transition to the next state.
            //The issue lies in the fact that there is no gap between transitions, so now we have a situation where the bullet is consumed but never
            // triggers an update.
            // We need a way to trigger the update by checking the previous state of the UI. If the UI is outdated, force an update on the UI to show what it should actually look like.

            if (!IncomingExtraParryBullet) 
            {
                return;
            }

            if (!IncomingParryBullet) 
            {
                return;
            }

            if (targetBulletState.parryBulletCount > 19)
            {
                IncomingExtraParryBullet.SetActive(true);
            }
            else 
            {
                if (IncomingExtraParryBullet) 
                {
                    IncomingExtraParryBullet.SetActive(false);
                }
            }
            if (targetBulletState.parryBulletCount > 4)
            {
                IncomingParryBullet.SetActive(true);
            }
            else
            {
                IncomingParryBullet.SetActive(false);
            }
        }

        internal void UpdateBulletStateTarget(BulletState state) 
        {
            targetBulletState = state;

            SetFiringSpeed(state.bulletAnimationSpeed);
        }

        internal void AdvanceBulletState(BulletState state) 
        {
            targetBulletState = state;

            //Check if the state is mid animation.


            SetFiringSpeed(state.bulletAnimationSpeed);
            if (state.isColouredBulletMoving)
            {
                TriggerBulletFire();
            }
            if (state.isEnhancedBulletMoving) 
            {
                TriggerParryBulletReload();
            }
        }

        public void InitializeBulletUI()
        {
            Transform powerMeter = powerMeterUIObject.transform;

            //Add the Script to the animator part of the thing we want to monitor.
            trigger = powerMeter.gameObject.AddComponent<BulletTriggerComponent>();
            trigger.body = this.characterBody;
            trigger.uiController = this;

            //Initialize Bullet objects first
            bulletObjects = new List<GameObject>();
            for (int i = bulletIndex; i <= endBulletIndex; i++) 
            {
                bulletObjects.Add(powerMeter.GetChild(0).GetChild(1).GetChild(i).gameObject);
            }

            parryBullets = new List<GameObject>();
            for (int i = parryPoweredIndex; i <= endParryPoweredIndex; i++) 
            {
                parryBullets.Add(powerMeter.GetChild(0).GetChild(1).GetChild(i).gameObject);
            }

            extraParryBullets = new List<GameObject>();
            for (int i = extraParryPoweredIndex; i <= endExtraParryPoweredIndex; i++) 
            {
                extraParryBullets.Add(powerMeter.GetChild(0).GetChild(1).GetChild(i).gameObject);
            }

            IncomingExtraParryBullet = powerMeter.GetChild(0).GetChild(1).GetChild(incomingExtraParryPoweredIndex).gameObject;
            IncomingParryBullet = powerMeter.GetChild(0).GetChild(1).GetChild(incomingParryBulletIndex).gameObject;

            //Disable all UI elements as there are no bullets.
            foreach (GameObject bullet in bulletObjects) 
            {
                bullet.SetActive(false);
            }
            foreach (GameObject bullet in parryBullets)
            {
                bullet.SetActive(false);
            }
            foreach (GameObject bullet in extraParryBullets)
            {
                bullet.SetActive(false);
            }
            IncomingExtraParryBullet.SetActive(false);
            IncomingParryBullet.SetActive(false);
        }

        public void TriggerBulletFire() 
        {
            //Triggers the event on the animator.
            if (meterAnimator) 
            {
                meterAnimator.SetTrigger("Fire Bullet");
            }
        }

        public void TriggerParryBulletReload() 
        {
            if (meterAnimator)
            {
                meterAnimator.SetTrigger("Fire Enhance Bullet");
            }
        }

        public void SetFiringSpeed(float firingSpeed) 
        {
            if (meterAnimator)
            {
                meterAnimator.SetFloat("firing speed", firingSpeed);
            }
        }

        public void SetBulletStates(List<BulletController.BulletType> bulletTypes) 
        {                
            if (baseAIPresent || !isInitialized || failedToInitialize)
            {
                return;
            }

            if (meterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Fire Bullet"))
            {
                return;
            }
            //Set Bullet count based on bullet input.
            if (bulletTypes.Count <= 5)
            {
                for (int i = 0; i < bulletTypes.Count; i++)
                {
                    SetBulletType(bulletObjects[i], bulletTypes[i]);
                }
            }
            else 
            {
                for (int i = 0; i < 5; i++) 
                {
                    SetBulletType(bulletObjects[i], bulletTypes[i]);
                }
            }

            //Disable the bullets that should not be seen.
            for (int i = 0; i < bulletObjects.Count; i++) 
            {
                bulletObjects[i].SetActive(i < bulletTypes.Count);
            }
        }

        public void SetBulletType(GameObject bullet, BulletController.BulletType type) 
        {
            //Order of children is red blue yellow
            switch (type) 
            {
                case BulletController.BulletType.RED:
                    bullet.SetActive(true);
                    bullet.transform.GetChild(0).gameObject.SetActive(true);
                    bullet.transform.GetChild(1).gameObject.SetActive(false);
                    bullet.transform.GetChild(2).gameObject.SetActive(false);
                    break;
                case BulletController.BulletType.BLUE:
                    bullet.SetActive(true);
                    bullet.transform.GetChild(0).gameObject.SetActive(false);
                    bullet.transform.GetChild(1).gameObject.SetActive(true);
                    bullet.transform.GetChild(2).gameObject.SetActive(false);
                    break;
                case BulletController.BulletType.YELLOW:
                    bullet.SetActive(true);
                    bullet.transform.GetChild(0).gameObject.SetActive(false);
                    bullet.transform.GetChild(1).gameObject.SetActive(false);
                    bullet.transform.GetChild(2).gameObject.SetActive(true);
                    break;
            }
        }

        public void SetEnhancedBulletState(int bulletCount)
        {
            if (baseAIPresent || !isInitialized || failedToInitialize) 
            {
                return;
            }
            //if (meterAnimator.GetCurrentAnimatorStateInfo(2).IsName("Fire Enhanced Ammo"))
            //{
            //    Chat.AddMessage("Bruh");
            //    return;
            //}
            if (bulletCount < 6) 
            {
                //Only enable the first 5 bullets.
                foreach (GameObject extraParryObject in extraParryBullets) 
                {
                    extraParryObject.SetActive(false);
                }

                for (int i = 0; i < parryBullets.Count; i++) 
                {
                    parryBullets[i].SetActive( i < bulletCount );
                }
                return;
            }

            //Set the bullets up with the extended stuff.
            foreach (GameObject parryBullet in parryBullets) 
            {
                parryBullet.SetActive(true);
            }

            for (int i = 0; i < extraParryBullets.Count; i++) 
            {
                extraParryBullets[i].SetActive(i < (bulletCount - 5) );
            }
        }

        public void TriggerUpdateOnEnhanced() 
        {
            SetEnhancedBulletState(targetBulletState.parryBulletCount);
        }

        public void TriggerUpdateOnColour()
        {
            SetBulletStates(targetBulletState.bulletTypes);
        }

        #endregion

        #region Domain overlay
        public void DomainOverlayEnable(bool state) 
        {
            if (domainOverlayObject) 
            {
                domainOverlayObject.SetActive(state);
            }
        }
        #endregion

        #region Ultimate Indicator
        public void InitializeUltimateIndicator() 
        {
            if (RoRHUD && !ultimateIndicatorObject)
            {
                if (LeeHyperrealPlugin.isRiskUIInstalled)
                {
                    ultimateIndicatorObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.spinnyIconUIObject, RoRHUDSpringCanvasTransform.Find("BottomRightCluster/Scaler"));
                    ultimateIndicatorObject.transform.position = new Vector3(14.04f, -7.2146f, 12.6244f);
                    ultimateIndicatorObject.transform.localPosition = new Vector3(199.0623f, -97.8483f, -2.9584f);
                    ultimateIndicatorObject.transform.rotation = Quaternion.identity;
                    ultimateIndicatorObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                }
                else if (LeeHyperrealPlugin.isBetterHudInstalled)
                {
                    ultimateIndicatorObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.spinnyIconUIObject, RoRHUDSpringCanvasTransform.Find("BottomCenterCluster/Scaler"));
                    ultimateIndicatorObject.transform.localPosition = new Vector3(850.0032f, -375.0046f, 0f);
                    ultimateIndicatorObject.transform.rotation = Quaternion.identity;
                    ultimateIndicatorObject.transform.localScale = new Vector3(1.4674f, 1.4674f, 1.4674f);
                }
                else 
                {
                    ultimateIndicatorObject = UnityEngine.GameObject.Instantiate(Modules.LeeHyperrealAssets.spinnyIconUIObject, RoRHUDSpringCanvasTransform.Find("BottomRightCluster/Scaler"));
                }
            }

            ultimateIndicatorAnimator = ultimateIndicatorObject.GetComponent<Animator>();
        }

        public void SetIntuitionStacks(int value) 
        {
            //Take int and apply it to a float between 0 -> 0.99
            float fraction = (float)value / (float)Modules.StaticValues.maxIntuitionStocks;

            if (fraction >= 0.999f) 
            {
                fraction = 0.99f;
            }

            if (fraction <= 0f) 
            {
                fraction = 0f;
            }

            if (ultimateIndicatorAnimator) 
            {
                ultimateIndicatorAnimator.SetFloat("Anschauung", fraction);
            }
        }

        public void TriggerUltDomain() 
        {
            if (ultimateIndicatorAnimator)
            {
                ultimateIndicatorAnimator.ResetTrigger("Ult Normal Ready");
                ultimateIndicatorAnimator.ResetTrigger("Ult Domain");
                ultimateIndicatorAnimator.SetTrigger("Ult Domain");
            }
        }

        public void TriggerUlt()
        {
            if (ultimateIndicatorAnimator)
            {
                ultimateIndicatorAnimator.ResetTrigger("Ult Normal Ready");
                ultimateIndicatorAnimator.ResetTrigger("Ult Domain");
                ultimateIndicatorAnimator.SetTrigger("Ult Normal Ready");
            }
        }
        
        public void TriggerNone()
        {
            if (ultimateIndicatorAnimator)
            {
                ultimateIndicatorAnimator.SetTrigger("None");
            }
        }

        public void TriggerTappedUltIcon()
        {
            if (ultimateIndicatorAnimator)
            {
                ultimateIndicatorAnimator.SetTrigger("Tapped");
            }
        }
        #endregion

        #region Hold
        public void InitializeHoldOKTag() 
        {
            if (RoRHUD && !holdOKTag)
            {
                if (LeeHyperrealPlugin.isRiskUIInstalled)
                {
                    Transform rootObject = RoRHUDSpringCanvasTransform.Find("BottomRightCluster/Scaler/SkillIconContainer/Skill1Root/BottomContainer");
                    Transform stockText = rootObject.transform.GetChild(0);
                    holdOKTag = CreateLabel(rootObject, "Hold OK Tag", "HOLD OK!", new Vector2(stockText.transform.position.x - 7.5f, stockText.transform.position.y + 20f), 13f);
                    holdOKTag.color = Modules.StaticValues.blueInvincibility;
                }
                else if(LeeHyperrealPlugin.isBetterHudInstalled)
                {
                    Transform rootObject = RoRHUDSpringCanvasTransform.Find("BottomCenterCluster/Scaler/Skill1Root/Skill1StockRoot");
                    Transform stockText = rootObject.transform.GetChild(0);
                    holdOKTag = CreateLabel(rootObject, "Hold OK Tag", "HOLD OK!", new Vector2(stockText.transform.position.x, stockText.transform.position.y + 18f), 12f);
                    holdOKTag.color = Modules.StaticValues.blueInvincibility;
                }
                else 
                {
                    Transform rootObject = RoRHUDSpringCanvasTransform.Find("BottomRightCluster/Scaler/Skill1Root/Skill1StockRoot");
                    Transform stockText = rootObject.transform.GetChild(0);
                    holdOKTag = CreateLabel(rootObject, "Hold OK Tag", "HOLD OK!", new Vector2(stockText.transform.position.x - 7.5f, stockText.transform.position.y + 18f), 12f);
                    holdOKTag.color = Modules.StaticValues.blueInvincibility;
                }
            }
        }

        public void SetHoldTagState(bool state) 
        {
            if (holdOKTag) 
            {
                holdOKTag.gameObject.SetActive(state);
            }
        }
        #endregion

        #region Crosshair
        public void InitializeCrosshair() 
        {
            if (!crosshairObject) 
            {
                var crosshairCanvas = RoRHUDChildLoc.FindChild("CrosshairCanvas") ?? RoRHUD.mainUIPanel.transform.Find("CrosshairCanvas");
                if (crosshairCanvas)
                {
                    crosshairObject = GameObject.Instantiate(Modules.ParticleAssets.customCrosshair, crosshairCanvas);
                }
            }

            crosshairObject.transform.localScale = new Vector3(Modules.Config.crosshairSize.Value, Modules.Config.crosshairSize.Value, 0f);
            crosshairAnimator = crosshairObject.GetComponent<Animator>();
        }

        public void ReinitCrosshair() 
        {
            Destroy(crosshairObject);
            InitializeCrosshair();
        }

        public void TriggerFireCrosshair() 
        {
            if (crosshairAnimator) 
            {
                crosshairAnimator.SetTrigger("Fire");
            }
        }

        public void SetSnipeStateCrosshair(bool state) 
        {
            if (crosshairAnimator) 
            {
                crosshairAnimator.SetBool("isSnipe", state);
            }
        }

        public void SetSprintingStateCrosshair(bool state) 
        {
            if (crosshairAnimator) 
            {
                crosshairAnimator.SetBool("isSprinting", state);
            }
        }

        public void UpdateSprintingCrosshairState() 
        {
            if (shouldOverrideAutoSprintingStateCrosshair) 
            {
                return;
            }
            
            SetSprintingStateCrosshair(characterBody.isSprinting);
        }
        #endregion

        #region Skill Icon Colour
        private void InitializeSkillIconMaterial()
        {
            //Set Material to our own custom one, steal the image and apply it to the material image.
            foreach (SkillIcon icon in RoRHUD.skillIcons)
            {
                Image iconImage = icon.iconImage;
                // Skill icons have not been properly initialized, throw this shit at the game
                if (iconImage.sprite.name == "texBanditSkill3Icon")
                {
                    throw new ForcedUIReInitException();
                }

                iconImage.material = new Material(Modules.LeeHyperrealAssets.UIFadeMat);
                iconImage.material.SetTexture("_IconTexture", iconImage.sprite.texture);

                //Depending on the skin, set the color

                //Setup red variant
                SetCustomUIMaterial(iconImage.material, 0f, ResolveColor());
                skillIconMaterials.Add(iconImage.material);
            }
        }

        private void UnsetSkillIconMat()
        {
            if (RoRHUD)
            {
                foreach (SkillIcon icon in RoRHUD.skillIcons)
                {
                    if (icon?.iconImage)
                        icon.iconImage.material = icon.iconImage.defaultMaterial;
                }
            }
        }
        #endregion

        public void SetRORUIActiveState(bool state)
        {
            if (RoRHUD) 
            {
                RoRHUD.gameObject.SetActive(state);
            }
        }

        #region Hook
        public void Hook()
        {
            //On.RoR2.CameraRigController.Update += CameraRigController_Update;
            On.RoR2.UI.HUD.Update += HUD_Update;
            On.RoR2.UI.NotificationUIController.ShowCurrentNotification += NotificationUIController_ShowCurrentNotification;
            Modules.Config.blueOrbTrigger.SettingChanged += SimpleKeyChanged;
            Modules.Config.redOrbTrigger.SettingChanged += SimpleKeyChanged;
            Modules.Config.yellowOrbTrigger.SettingChanged += SimpleKeyChanged;
            Modules.Config.crosshairSize.SettingChanged += CrosshairSize_SettingChanged;
        }

        private void NotificationUIController_ShowCurrentNotification(On.RoR2.UI.NotificationUIController.orig_ShowCurrentNotification orig, NotificationUIController self, CharacterMasterNotificationQueue notificationQueue)
        {
            if (notificationQueue)
            {
                var currentNotification = notificationQueue.GetCurrentNotification();
                var notifQueue = notificationQueue.GetComponent<LeeHyperrealNotificationQueue>();

                if (currentNotification != null && notifQueue != null)
                {
                    if (currentNotification.data is ItemDef key)
                    {
                        if (Modules.StaticValues.itemKeyValueNotificationPairs.TryGetValue(key.itemIndex, out var customEffect))
                        {
                            LeeHyperrealNotificationQueue.PushNotification(notificationQueue.gameObject.GetComponent<CharacterMaster>(), customEffect);
                        }
                    }
                    else if (currentNotification.data is EquipmentDef equipmentDef) 
                    {
                        if (Modules.StaticValues.equipmentKeyValueNotificationPairs.TryGetValue(equipmentDef.equipmentIndex, out var customEffect)) 
                        {
                            LeeHyperrealNotificationQueue.PushNotification(notificationQueue.gameObject.GetComponent<CharacterMaster>(), customEffect);
                        }
                    }
                }
            }

            orig(self, notificationQueue);
        }

        private void CrosshairSize_SettingChanged(object sender, EventArgs e)
        {
            if (crosshairObject) 
            {
                crosshairObject.transform.localScale = new Vector3(Modules.Config.crosshairSize.Value, Modules.Config.crosshairSize.Value, 0f);
            }
        }

        private void SimpleKeyChanged(object sender, EventArgs e)
        {
            if (blueSimpleGlyph != null) 
            {
                blueSimpleGlyph.orbLabel.text = DeAlphaizeString(Modules.Config.blueOrbTrigger.Value.ToString());
            }

            if (redSimpleGlyph != null)
            {
                redSimpleGlyph.orbLabel.text = DeAlphaizeString(Modules.Config.redOrbTrigger.Value.ToString());
            }

            if (yellowSimpleGlyph != null)
            {
                yellowSimpleGlyph.orbLabel.text = DeAlphaizeString(Modules.Config.yellowOrbTrigger.Value.ToString());
            }
        }

        public void Unhook()
        {
            //On.RoR2.CameraRigController.Update -= CameraRigController_Update;
            On.RoR2.UI.HUD.Update -= HUD_Update;
            On.RoR2.UI.NotificationUIController.ShowCurrentNotification -= NotificationUIController_ShowCurrentNotification;
            Modules.Config.blueOrbTrigger.SettingChanged -= SimpleKeyChanged;
            Modules.Config.redOrbTrigger.SettingChanged -= SimpleKeyChanged;
            Modules.Config.yellowOrbTrigger.SettingChanged -= SimpleKeyChanged;
        }

        private void HUD_Update(On.RoR2.UI.HUD.orig_Update orig, HUD self)
        {
            orig(self);

            if (RoRHUD != self)
            {
                RoRHUD = self;
                RoRHUDChildLoc = self.GetComponent<ChildLocator>();
            }
        }
        #endregion


    }
}
