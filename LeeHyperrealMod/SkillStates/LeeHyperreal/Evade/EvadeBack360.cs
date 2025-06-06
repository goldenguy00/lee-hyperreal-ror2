using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary;
using LeeHyperrealMod.Modules;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static LeeHyperrealMod.Content.Controllers.BulletController;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Evade
{
    internal class EvadeBack360 : BaseRootMotionMoverState
    {
        public static float baseDuration = 0.566f;
        public static float duration = 0.566f;

        public static string dodgeSoundString = "HenryRoll";
        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

        private Animator animator;

        public bool isLeftRoll;

        private float earlyExitFrac = 0.803f;

        private float movementMultiplier = 2f;

        private float disableInvincibility = 0.45f;

        public bool triggerBreakVFX = false;
        public bool playBreakSFX = false;
        public float playReloadSFXFrac = 0.5f;
        public bool hasPlayedReload = false;

        private float firingFrac = 0.42f;
        private bool hasFired = false;
        public static float procCoefficient = 1f;

        public OrbController orbController;
        public BulletController bulletController;
        public LeeHyperrealDomainController domainController;
        public LeeHyperrealUIController uiController;
        public float empoweredBulletMultiplier = 1f;


        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            duration = baseDuration / base.attackSpeedStat;

            uiController = gameObject.GetComponent<LeeHyperrealUIController>();
            animator.SetFloat("attack.playbackRate", base.attackSpeedStat);
            rmaMultiplier = movementMultiplier;
            bulletController = gameObject.GetComponent<BulletController>();
            orbController = gameObject.GetComponent<OrbController>();
            domainController = gameObject.GetComponent<LeeHyperrealDomainController>();
            base.characterBody.isSprinting = false;

            if (orbController)
            {
                orbController.isExecutingSkill = false;
            }

            if (bulletController.ConsumeEnhancedBullet(1))
            {
                empoweredBulletMultiplier = 2.0f;
                playBreakSFX = true;
                triggerBreakVFX = true;
            }

            if (domainController.GetDomainState())
            {
                BulletType type = bulletController.ConsumeColouredBullet();

                if (type != BulletType.NONE)
                {
                    //Grant a 3 ping.
                    orbController.Grant3Ping(type);
                }
            }

            //Receive the var from the previous state, run animation.

            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(Modules.Buffs.invincibilityBuff.buffIndex, baseDuration * disableInvincibility);
            }
            Ray aimRay = base.GetAimRay();
            base.characterDirection.forward = aimRay.direction;
            PlayDodgeAnimation();

            StartAimMode(duration, false);

            if (bulletController.snipeAerialPlatform)
            {
                bulletController.snipeAerialPlatform.GetComponent<DestroyPlatformOnDelay>().StartDestroying();

                bulletController.snipeAerialPlatform = null; // reset so Idle spawns another.
            }
        }

        public void PlaySwingEffect(float scale, GameObject effectPrefab, Vector3 startPos, Vector3 aimVector)
        {
            var effectData = new EffectData()
            {
                origin = startPos,
                rotation = Quaternion.LookRotation(aimVector),
                scale = scale
            };

            EffectManager.SpawnEffect(effectPrefab, effectData, true);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void UpdateMeleeRootMotion(float scale)
        {
            if (rma)
            {
                Vector3 a = rma.ExtractRootMotion();
                if (base.characterMotor)
                {
                    a.x *= Modules.StaticValues.ScaleMoveSpeed(moveSpeedStat);
                    a.z *= Modules.StaticValues.ScaleMoveSpeed(moveSpeedStat);

                    base.characterMotor.rootMotion = a * scale;
                }
            }
        }
        
        public override void Update()
        {
            base.Update();
            if (age >= duration * firingFrac && isAuthority && !hasFired)
            {
                uiController.TriggerFireCrosshair();
                hasFired = true;
                base.AddRecoil(-1f * Modules.StaticValues.snipeRecoil, -2f * Modules.StaticValues.snipeRecoil, -0.5f * Modules.StaticValues.snipeRecoil, 0.5f * Modules.StaticValues.snipeRecoil);

                if (base.inputBank.skill4.down || base.inputBank.skill2.down)
                {
                    Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
                }

                var modelTransform = GetModelTransform();
                var muzzleTransform = modelTransform.Find("Rifle").transform;
                var startPos = muzzleTransform.position;
                var startEffectPos = startPos;

                const float scale = 1.25f;
                var stupidOffset = scale == 1.25f ? 0.89f : 0.6f;
                startEffectPos.y -= stupidOffset;

                bool canAllowMovement = false;
                PlayerCharacterMasterController.CanSendBodyInput(characterBody.master.playerCharacterMasterController.networkUser, out var _, out var _, out var cameraRigController, out canAllowMovement);

                var endPos = cameraRigController.crosshairWorldPosition;
                var endEffectPos = endPos;
                endEffectPos.y -= stupidOffset;

                var aimVector = (endPos - startPos).normalized;
                var aimEffectVector = (endEffectPos - startEffectPos).normalized;

                if (isGrounded)
                {
                    PlaySwingEffect(scale, ParticleAssets.RetrieveParticleEffectFromSkin("snipeGround", characterBody), startEffectPos, aimEffectVector);
                }
                PlaySwingEffect(scale, ParticleAssets.RetrieveParticleEffectFromSkin("snipe", characterBody), startEffectPos, aimEffectVector);

                new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_atk_ex_3").Send(R2API.Networking.NetworkDestination.Clients);
                if (playBreakSFX)
                {
                    new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_atk_ex_3_break").Send(R2API.Networking.NetworkDestination.Clients);
                }

                new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimVector,
                    origin = startPos,
                    damage = Modules.StaticValues.snipeDamageCoefficient * this.damageStat * empoweredBulletMultiplier,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Secondary),
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = Modules.StaticValues.snipeRange,
                    force = Modules.StaticValues.snipeForce,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.75f,
                    sniper = false,
                    stopperMask = LayerIndex.world.mask,
                    weapon = null,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = triggerBreakVFX ? ParticleAssets.RetrieveParticleEffectFromSkin("snipeHitEnhanced", characterBody) : ParticleAssets.RetrieveParticleEffectFromSkin("snipeHit", characterBody),
                }.Fire();

                //characterBody.SetAimTimer(duration + 1f);
                if (!bulletController.snipeAerialPlatform && !isGrounded)
                {
                    ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
                    Transform baseTransform = childLocator.FindChild("BaseTransform");
                    bulletController.snipeAerialPlatform = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("snipeAerialFloor", characterBody), baseTransform.position, Quaternion.identity);
                }
            }

            if (age >= duration * playReloadSFXFrac && !hasPlayedReload)
            {
                hasPlayedReload = true;
                AkSoundEngine.PostEvent("Play_c_liRk4_atk_ex_3_reload", base.gameObject);
            }

            if (age >= duration * earlyExitFrac && isAuthority)
            {
                if (base.inputBank.skill1.down)
                {
                    //Fire Snipe
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new Snipe { }, EntityStates.InterruptPriority.Skill);
                        return;
                    }
                }

                if (base.inputBank.skill3.justPressed && skillLocator.utility.stock >= 1)
                {
                    skillLocator.utility.stock -= 1;
                    Vector3 result = Modules.StaticValues.CheckDirection(inputBank.moveVector, GetAimRay());
                    if (result == new Vector3(0, 0, 1))
                    {
                        base.outer.SetInterruptState(new Evade { unsetSnipe = true }, EntityStates.InterruptPriority.Frozen);
                        return;
                    }
                    if (result == new Vector3(0, 0, 0))
                    {
                        base.outer.SetInterruptState(new EvadeBack360 { }, EntityStates.InterruptPriority.Frozen);
                        return;
                    }
                    if (result == new Vector3(1, 0, 0))
                    {
                        if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                        {
                            base.outer.SetInterruptState(new EvadeSide { isLeftRoll = false }, EntityStates.InterruptPriority.Frozen);
                            return;
                        }
                    }
                    if (result == new Vector3(-1, 0, 0))
                    {
                        if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                        {
                            base.outer.SetInterruptState(new EvadeSide { isLeftRoll = true }, EntityStates.InterruptPriority.Frozen);
                            return;
                        }
                    }
                }

                if ((base.inputBank.skill4.justPressed) && base.isAuthority)
                {
                    Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
                }
            }
            if (age >= duration && isAuthority)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    base.outer.SetInterruptState(new IdleSnipe { }, EntityStates.InterruptPriority.Skill);
                    return;
                }
            }
        }

        public void PlayDodgeAnimation()
        {
            PlayAnimation("Body", "SnipeEvade180", "attack.playbackRate", duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (age >= duration * earlyExitFrac)
            {
                return InterruptPriority.Skill;
            }
            else
            {
                return InterruptPriority.Frozen;
            }
        }
    }
}
