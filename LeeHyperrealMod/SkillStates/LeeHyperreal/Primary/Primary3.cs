﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.SkillStates.LeeHyperreal.DomainShift;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using LeeHyperrealMod.Modules;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class Primary3 : BaseMeleeAttack
    {

        private float rollSpeed;
        private Vector3 forwardDirection;
        private Vector3 previousPosition;

        public static float initialSpeedCoefficient = 4f;
        public static float finalSpeedCoefficient = 0f;

        public static float moveStartFrac = 0f;
        public static float moveEndFrac = 0.18f;
        private Ray aimRay;

        public RootMotionAccumulator rma;


        public static float heldButtonThreshold = 0.21f;
        public bool ifButtonLifted = false;

        private LeeHyperrealDomainController domainController;

        CharacterGravityParameters gravParams;
        CharacterGravityParameters oldGravParams;
        float turnOffGravityFrac = 0.11f;

        private float playSlamSFXFrac = 0.2f;
        private bool soundPlayed = false;

        private float playInitialWeaponImpactEffect = 0.07f;
        private bool effectPlayed = false;

        private float attack2StartFrac = 0.2f;
        private float attack2EndFrac = 0.35f;
        private bool hasFired2 = false;

        public override void OnEnter()
        {
            domainController = this.GetComponent<LeeHyperrealDomainController>();
            this.hitboxName = "AOEMelee";

            this.damageType = new DamageTypeCombo(DamageType.Stun1s, DamageTypeExtended.Generic, DamageSource.Primary);
            this.damageCoefficient = StaticValues.primary3DamageCoefficient;
            this.procCoefficient = StaticValues.primary3ProcCoefficient;
            this.pushForce = StaticValues.primary3PushForce;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 3f;
            this.attackStartTime = 0.08f;
            this.attackEndTime = 0.11f;
            this.baseEarlyExitTime = 0.225f;
            this.bufferActiveTime = 0.15f;
            this.moveCancelEndTime = 0.673f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = Modules.StaticValues.primary3HitHopVelocity;

            this.swingSoundString = "";
            this.hitSoundString = "";
            this.muzzleString = "BaseTransform";
            this.swingEffectPrefab = null;

            enableParry = true;
            parryLength = StaticValues.primary3ParryLength;
            parryTiming = StaticValues.primary3ParryTiming;
            parryPauseLength = StaticValues.primary3ParryPauseLength;
            parryProjectileTiming = StaticValues.primary3ParryProjectileTimingStart;
            parryProjectileTimingEnd = StaticValues.primary3ParryProjectileTimingEnd;

            base.OnEnter();

            this.hitEffectPrefab = ParticleAssets.RetrieveParticleEffectFromSkin("primary3Hit", characterBody);

            InitMeleeRootMotion();

            oldGravParams = base.characterMotor.gravityParameters;
            gravParams = new CharacterGravityParameters();
            gravParams.environmentalAntiGravityGranterCount = 1;
            gravParams.channeledAntiGravityGranterCount = 1;

            characterMotor.gravityParameters = gravParams;
        }

        public RootMotionAccumulator InitMeleeRootMotion()
        {
            rma = base.GetModelRootMotionAccumulator();
            if (rma)
            {
                rma.ExtractRootMotion();
            }
            if (base.characterDirection)
            {
                base.characterDirection.forward = base.inputBank.aimDirection;
            }
            if (base.characterMotor)
            {
                base.characterMotor.moveDirection = Vector3.zero;
            }
            return rma;
        }

        // Token: 0x060003CA RID: 970 RVA: 0x0000F924 File Offset: 0x0000DB24
        public void UpdateMeleeRootMotion(float scale)
        {
            if (rma)
            {
                Vector3 a = rma.ExtractRootMotion();
                if (base.characterMotor)
                {
                    base.characterMotor.rootMotion = new Vector3(a.x * xzMovementMultiplier, a.y, a.z * xzMovementMultiplier) * scale;
                }
            }
        }
        public override void Update()
        {
            if (!base.inputBank.skill1.down && base.isAuthority)
            {
                ifButtonLifted = true;
            }

            if (!ifButtonLifted && base.isAuthority && base.stopwatch >= duration * heldButtonThreshold && domainController.DomainEntryAllowed())
            {
                //Cancel out into Domain shift skill state
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    base.outer.SetInterruptState(new DomainEnterState { shouldForceUpwards = true }, InterruptPriority.Frozen);
                }
            }

            if (base.stopwatch >= duration * playInitialWeaponImpactEffect && !effectPlayed && base.isAuthority) 
            {
                effectPlayed = true;
                PlayExtraSwingEffect(ParticleAssets.RetrieveParticleEffectFromSkin("primary3Swing1", characterBody));
            }

            if (base.stopwatch >= duration * playSlamSFXFrac && !soundPlayed && base.isAuthority) 
            {
                soundPlayed = true;
                PlayExtraSwingEffect(ParticleAssets.RetrieveParticleEffectFromSkin("primary3Swing2", characterBody));
            }

            if (base.stopwatch <= duration * turnOffGravityFrac) 
            {
                base.characterMotor.Motor.ForceUnground();
            }

            if (base.stopwatch >= duration * turnOffGravityFrac) 
            {
                base.characterMotor.gravityParameters = oldGravParams;
                animator.SetBool("isGrounded", base.isGrounded);
            }

            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateMeleeRootMotion(1.6f);
            if (base.isAuthority && stopwatch >= duration * attack2StartFrac && stopwatch <= duration * attack2EndFrac) 
            {
                FireSecondAttack();
            }
        }

        internal void FireSecondAttack()
        {
            if (!hasFired2)
            {
                this.hasFired2 = true;

                if (base.isAuthority)
                {
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                    int maxNumHit = 0;
                    List<HurtBox> result = new List<HurtBox>();
                    for (int i = 0; i < attackAmount; i++)
                    {
                        // Create Attack, fire it, do the on hit enemy authority.
                        this.attack = new OverlapAttack();
                        this.attack.damageType = this.damageType;
                        this.attack.attacker = base.gameObject;
                        this.attack.inflictor = base.gameObject;
                        this.attack.teamIndex = base.GetTeam();
                        this.attack.damage = this.damageCoefficient * this.damageStat;
                        this.attack.procCoefficient = this.procCoefficient;
                        this.attack.hitEffectPrefab = this.hitEffectPrefab;
                        this.attack.forceVector = this.bonusForce / attackAmount;
                        this.attack.pushAwayForce = this.pushForce / attackAmount;
                        this.attack.hitBoxGroup = hitBoxGroup;
                        this.attack.isCrit = base.RollCrit();
                        this.attack.impactSound = this.impactSound;
                        if (this.attack != null)
                        {
                            this.attack.Fire(result);
                            if (result.Count > maxNumHit)
                            {
                                maxNumHit = result.Count;
                            }
                        }
                    }
                    if (partialAttack > 0.0f)
                    {
                        // Create Attack, fire it, do the on hit enemy authority, partaial damage on final 
                        this.attack = new OverlapAttack();
                        this.attack.damageType = this.damageType;
                        this.attack.attacker = base.gameObject;
                        this.attack.inflictor = base.gameObject;
                        this.attack.teamIndex = base.GetTeam();
                        this.attack.damage = this.damageCoefficient * this.damageStat * partialAttack;
                        this.attack.procCoefficient = this.procCoefficient * partialAttack;
                        this.attack.hitEffectPrefab = this.hitEffectPrefab;
                        this.attack.forceVector = this.bonusForce * partialAttack / attackAmount;
                        this.attack.pushAwayForce = this.pushForce * partialAttack / attackAmount;
                        this.attack.hitBoxGroup = hitBoxGroup;
                        this.attack.isCrit = base.RollCrit();
                        this.attack.impactSound = this.impactSound;
                        if (this.attack != null)
                        {
                            this.attack.Fire(result);
                            if (result.Count > maxNumHit)
                            {
                                maxNumHit = result.Count;
                            }
                        }
                    }

                    if (maxNumHit > 0)
                    {
                        HitSoundCallback();
                        this.OnHitEnemyAuthority();
                    }
                }
            }
        }
        public override void OnExit()
        {
            animator.SetBool("isGrounded", base.isGrounded);
            base.PlayAnimation("Body", "BufferEmpty");
            base.characterMotor.gravityParameters = oldGravParams;
            base.OnExit();
        }

        protected void PlayExtraSwingEffect(GameObject effect)
        {
            ModelLocator component = gameObject.GetComponent<ModelLocator>();
            if (component && component.modelTransform)
            {
                ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                if (component2)
                {
                    int childIndex = component2.FindChildIndex(muzzleString);
                    Transform transform = component2.FindChild(childIndex);
                    if (transform)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = transform.position,
                            scale = swingScale,
                        };
                        effectData.SetChildLocatorTransformReference(gameObject, childIndex);
                        EffectManager.SpawnEffect(effect, effectData, true);
                    }
                }
            }
        }

        public override void SetNextStateOnParry()
        {
            if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
            {
                this.outer.SetInterruptState(new Primary3 { xzMovementMultiplier = 0 }, InterruptPriority.Frozen);
                return;

            }
        }

        protected override void TriggerOrbIncrementor(int timesHit)
        {
            if (orbController)
            {
                orbController.AddToIncrementor(Modules.StaticValues.flatAmountToGrantOnPrimaryHit * timesHit);
            }
        }

        protected override void HitSoundCallback()
        {
            new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_nml_3_2").Send(R2API.Networking.NetworkDestination.Clients);
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Body", "primary3", "attack.playbackRate", duration);
        }

        protected override void SetNextState()
        {
            // Move to Primary4

            if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
            {
                base.outer.SetInterruptState(new Primary4 { }, InterruptPriority.Skill);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    
        
    }
}
