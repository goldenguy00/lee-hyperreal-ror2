﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.SkillStates.LeeHyperreal.Evade;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using LeeHyperrealMod.Modules;
using static LeeHyperrealMod.Content.Controllers.BulletController;
using UnityEngine.Networking;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary
{
    internal class Snipe : BaseSkillState
    {
        Animator animator;
        public float earlyExitFrac = 0.35f;
        public float firingFrac = 0.08f;
        public bool hasFired;
        public float duration = 1.3f;

        public static float damageCoefficient = Modules.StaticValues.snipeDamageCoefficient;
        public static float procCoefficient = 1f;
        public static float baseDuration = 1.3f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;

        public bool stockTaken;

        public OrbController orbController;
        public BulletController bulletController;
        public LeeHyperrealDomainController domainController;
        public LeeHyperrealUIController uiController;
        public float empoweredBulletMultiplier = 1f;

        public Vector3 velocity;

        public bool triggerBreakVFX = false;
        public bool playBreakSFX = false;
        public float playReloadSFXFrac = 0.475f;
        public bool hasPlayedReload = false;

        public override void OnEnter()
        {
            bulletController = gameObject.GetComponent<BulletController>();
            orbController = gameObject.GetComponent<OrbController>();
            domainController = gameObject.GetComponent<LeeHyperrealDomainController>();
            uiController = gameObject.GetComponent<LeeHyperrealUIController>();

            base.characterMotor.velocity = new Vector3(0, 0, 0);
            base.characterDirection.moveVector = new Vector3(0, 0, 0);

            base.characterBody.isSprinting = false;
            base.OnEnter();
            //Enter the snipe stance, move to IdleSnipe
            animator = this.GetModelAnimator();
            animator.SetFloat("attack.playbackRate", base.attackSpeedStat);

            duration = baseDuration / base.attackSpeedStat;

            if (NetworkServer.active) 
            {
                int itemCount = this.characterBody.inventory ? this.characterBody.inventory.GetItemCount(DLC2Content.Items.IncreasePrimaryDamage) : 0;
                if (itemCount > 0) 
                {
                    this.characterBody.AddIncreasePrimaryDamageStack();
                } 
            }



            if (bulletController.ConsumeEnhancedBullet(1))
            {
                empoweredBulletMultiplier = Modules.StaticValues.empoweredBulletMultiplier;
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
                    playBreakSFX = true;
                }
            }



            base.characterDirection.forward = Vector3.SmoothDamp(base.characterDirection.forward, base.inputBank.aimDirection, ref velocity, 0.1f, 300f, Time.deltaTime);


            //characterBody.SetAimTimer(duration + 1f);

            if (!bulletController.snipeAerialPlatform && !isGrounded)
            {
                ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
                Transform baseTransform = childLocator.FindChild("BaseTransform");
                bulletController.snipeAerialPlatform = Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("snipeAerialFloor", characterBody), baseTransform.position, Quaternion.identity);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!hasPlayedReload)
            {
                hasPlayedReload = true;
                AkSoundEngine.PostEvent("Play_c_liRk4_atk_ex_3_reload", base.gameObject);
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

        public override void Update()
        {
            base.Update();

            base.characterMotor.velocity = new Vector3(0, 0, 0);
            base.characterDirection.moveVector = new Vector3(0, 0, 0);

            //Check for dodging. Otherwise ignore.
            if (base.inputBank.skill3.justPressed && skillLocator.utility.stock >= 1 && !stockTaken)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    skillLocator.utility.stock -= 1;
                    stockTaken = true;
                    Vector3 result = Modules.StaticValues.CheckDirection(inputBank.moveVector, GetAimRay());
                    if (result == new Vector3(0, 0, 1))
                    {
                        base.outer.SetInterruptState(new Evade.Evade { unsetSnipe = true }, InterruptPriority.Frozen);
                        return;
                    }
                    if (result == new Vector3(0, 0, 0))
                    {
                        base.outer.SetInterruptState(new EvadeBack360 { }, InterruptPriority.Frozen);
                        return;
                    }
                    if (result == new Vector3(1, 0, 0))
                    {
                        base.outer.SetInterruptState(new EvadeSide { isLeftRoll = false }, InterruptPriority.Frozen);
                        return;
                    }
                    if (result == new Vector3(-1, 0, 0))
                    {
                        base.outer.SetInterruptState(new EvadeSide { isLeftRoll = true }, InterruptPriority.Frozen);
                        return;
                    }

                    return;
                }
            }

            if (base.inputBank.jump.down && characterBody.hasAuthority)
            {
                base.outer.SetInterruptState(new LeeHyperrealCharacterMain { forceJump = true }, InterruptPriority.Skill);
                return;
            }

            if (base.inputBank.sprint.justPressed && characterBody.hasAuthority)
            {
                this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                return;
            }

            if ((base.inputBank.skill4.justPressed || base.inputBank.skill2.justPressed) && isAuthority)
            {
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
            }

            if (age >= duration * playReloadSFXFrac && !hasPlayedReload)
            {
                hasPlayedReload = true;
                AkSoundEngine.PostEvent("Play_c_liRk4_atk_ex_3_reload", base.gameObject);
            }

            base.characterDirection.forward = Vector3.SmoothDamp(base.characterDirection.forward, base.inputBank.aimDirection, ref velocity, 0.1f, 300f, Time.deltaTime);
            if (age >= duration * firingFrac && !hasFired)
            {
                PlayAttackAnimation();

                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);

                if (base.isAuthority) 
                {
                    uiController.TriggerFireCrosshair();
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
                    PlaySwingEffect(scale, ParticleAssets.RetrieveParticleEffectFromSkin("snipeBulletCasing", characterBody), startEffectPos, aimEffectVector);

                    base.AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

                    new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimVector,
                        origin = startPos,
                        damage = damageCoefficient * this.damageStat * empoweredBulletMultiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Secondary),
                        falloffModel = BulletAttack.FalloffModel.None,
                        maxDistance = Modules.StaticValues.snipeRange,
                        force = Modules.StaticValues.snipeForce,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = base.RollCrit(),
                        owner = base.gameObject,
                        smartCollision = true,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = Modules.StaticValues.snipeProcCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = LayerIndex.world.mask,
                        weapon = null,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        hitCallback = snipeHitCallback,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = triggerBreakVFX ? ParticleAssets.RetrieveParticleEffectFromSkin("snipeHitEnhanced", characterBody) : ParticleAssets.RetrieveParticleEffectFromSkin("snipeHit", characterBody),
                    }.Fire();

                    new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_atk_ex_3").Send(R2API.Networking.NetworkDestination.Clients);
                    if (playBreakSFX)
                    {
                        new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_atk_ex_3_break").Send(R2API.Networking.NetworkDestination.Clients);
                    }
                }
            }

            if (age >= duration * earlyExitFrac && base.isAuthority)
            {
                if (inputBank.skill1.down)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new Snipe { }, InterruptPriority.PrioritySkill);
                        return;
                    }
                }


                //IF we're not in hold variant, then allow the transition if the skill is down.
                if (inputBank.skill2.down && !Modules.Config.allowSnipeButtonHold.Value && base.isAuthority)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new ExitSnipe(), InterruptPriority.PrioritySkill);
                    }
                }

                //IF we ARE in the hold variant, then allow the transition if the skill is not pressed.
                if (!base.inputBank.skill2.down && Modules.Config.allowSnipeButtonHold.Value && base.isAuthority)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new ExitSnipe(), InterruptPriority.PrioritySkill);
                    }
                }
            }

            if (age >= duration && base.isAuthority)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    base.outer.SetInterruptState(new IdleSnipe { }, InterruptPriority.PrioritySkill);
                    return;
                }
            }
        }

        private bool snipeHitCallback(BulletAttack bulletAttack, ref BulletAttack.BulletHit hitInfo)
        {
            if (orbController)
            {
                orbController.AddToIncrementor(Modules.StaticValues.flatAmountToGrantOnPrimaryHit);
            }
            return BulletAttack.defaultHitCallback(bulletAttack, ref hitInfo);
        }

        public void PlayAttackAnimation()
        {
            PlayAnimation("Body", "SnipeShot", "attack.playbackRate", duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}