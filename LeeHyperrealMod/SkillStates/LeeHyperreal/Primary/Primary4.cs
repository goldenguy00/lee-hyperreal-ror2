﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.SkillStates.LeeHyperreal.DomainShift;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class Primary4 : BaseSkillState
    {
        private float rollSpeed;
        private Vector3 forwardDirection;
        private Vector3 previousPosition;

        public static float initialSpeedCoefficient = 2.2f;
        public static float finalSpeedCoefficient = 0f;

        public static float moveStartFrac = 0.05f;
        public static float moveEndFrac = 0.47f;
        public static float shootRadius = StaticValues.primary4BlastRadius;
        public static float basePulseRate = StaticValues.primary4BasePulseRate;
        public static float damageCoefficient = StaticValues.primary4DamageCoefficient;
        public float pulseRate;
        private Ray aimRay;

        private static List<Tuple<float, float>> timings; // When he's invis
        private int currentIndex;

        private bool bufferNextMove;
        private float bufferActiveTime;
        private float earlyExitTime;
        public float duration;
        public float baseDuration = 3f;
        private float hitPauseTimer;
        private BlastAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        internal float stopwatch;
        internal float moveCancelEndTime = 0.7f;
        public RootMotionAccumulator rma;
        public OrbController orbController;
        private int hitConfirmResult;

        public static float heldButtonThreshold = 0.46f;
        public bool ifButtonLifted = false;

        private LeeHyperrealDomainController domainController;

        public float defaultMovementMultiplier = 0.3f;
        public float inputMovementMultiplier = 2f;
        public float movementMultiplier;
        public Transform baseTransform;

        public float waitSwingTimer = 0.066f;
        public bool playedSwing = false;

        public WeaponModelHandler weaponModelHandler;

        public override void OnEnter()
        {
            base.OnEnter();
            weaponModelHandler = base.gameObject.GetComponent<WeaponModelHandler>();
            orbController = base.gameObject.GetComponent<OrbController>();
            domainController = this.GetComponent<LeeHyperrealDomainController>();
            duration = baseDuration / Modules.StaticValues.ScaleAttackSpeed(attackSpeedStat);
            pulseRate = basePulseRate / this.attackSpeedStat;
            earlyExitTime = 0.48f;

            bufferActiveTime = 0.42f;
            aimRay = base.GetAimRay();
            PlayAttackAnimation();

            // Setup Blastattack
            attack = new BlastAttack
            {
                attacker = base.gameObject,
                inflictor = null,
                teamIndex = base.GetTeam(),
                position = base.gameObject.transform.position,
                radius = shootRadius,
                falloffModel = BlastAttack.FalloffModel.None,
                baseDamage = this.damageStat * damageCoefficient,
                baseForce = 0f,
                bonusForce = Vector3.zero,
                crit = this.RollCrit(),
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary),
                losType = BlastAttack.LoSType.NearestHit,
                canRejectForce = false,
                procChainMask = new ProcChainMask(),
                procCoefficient = Modules.StaticValues.primary4ProcCoefficient,
                impactEffect = EffectCatalog.FindEffectIndexFromPrefab(ParticleAssets.RetrieveParticleEffectFromSkin("primary4Hit", characterBody)),
                attackerFiltering = AttackerFiltering.NeverHitSelf
        };

            stopwatch = 0f;
            InitMeleeRootMotion();
            movementMultiplier = defaultMovementMultiplier;

            if (base.isAuthority) 
            {
                PlaySwingEffect("BaseTransform", 1.25f, ParticleAssets.RetrieveParticleEffectFromSkin("primary4AfterImage", characterBody));
            }

            //Iterate through list of tuples when timer has exceeded, set model off
            currentIndex = 0;
            timings = new List<Tuple<float, float>>();
            timings.Add(new Tuple<float, float>(0f, 0.03f));
            timings.Add(new Tuple<float, float>(0.08f, 0.115f));
            timings.Add(new Tuple<float, float>(0.16f, 0.195f));
            timings.Add(new Tuple<float, float>(0.22f, 0.255f));
            timings.Add(new Tuple<float, float>(0.27f, 0.305f));
            timings.Add(new Tuple<float, float>(0.35f, 0.385f));
            timings.Add(new Tuple<float, float>(0.41f, 0.445f));
        }

        public void PlaySwingEffect(string muzzleString, float swingScale, GameObject effectPrefab) 
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
                        EffectManager.SpawnEffect(effectPrefab, effectData, true);
                    }
                }
            }
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
                    a.x *= Modules.StaticValues.ScaleMoveSpeed(moveSpeedStat);
                    a.z *= Modules.StaticValues.ScaleMoveSpeed(moveSpeedStat);

                    base.characterMotor.rootMotion = a * scale;
                }
            }
        }

        public override void Update()
        {
            if ((base.inputBank.skill3.justPressed || base.inputBank.skill4.justPressed) && base.isAuthority)
            {
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
                return;
            }

            if (currentIndex < timings.Count)
            {
                //If timer is in index range, then keep invis
                if (base.age >= duration * timings[currentIndex].Item1 && base.age <= duration * timings[currentIndex].Item2)
                {
                    //Keep invis
                    if (weaponModelHandler)
                    {
                        weaponModelHandler.SetStateForModelAndSubmachine(false);
                    }
                }

                if (base.age >= duration * timings[currentIndex].Item2)
                {
                    //increment the index, become visible again.
                    currentIndex += 1;
                    if (weaponModelHandler)
                    {
                        weaponModelHandler.SetStateForModelAndSubmachine(true);
                    }
                }
            }
            else 
            {
                // Become visible again
                if (weaponModelHandler)
                {
                    weaponModelHandler.SetStateForModelAndSubmachine(true);
                }
            }


            if (base.isAuthority && this.age >= duration * bufferActiveTime && base.isAuthority)
            {
                if (inputBank.skill1.down)
                {
                    bufferNextMove = true;
                }
            }

            if (this.age >= waitSwingTimer * duration && !playedSwing && base.isAuthority) 
            {
                playedSwing = true;
                new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_atk_nml_4").Send(R2API.Networking.NetworkDestination.Clients);

                PlaySwingEffect("BaseTransform", 1f, ParticleAssets.RetrieveParticleEffectFromSkin("primary4Swing", characterBody));
            }

            if (base.isAuthority && this.age <= duration * earlyExitTime) 
            {
                if (inputBank.moveVector != Vector3.zero)
                {
                    this.characterDirection.moveVector = base.inputBank.moveVector;
                    this.characterDirection.forward = base.inputBank.moveVector;
                    movementMultiplier = inputMovementMultiplier;
                }
                else 
                {
                    this.characterDirection.moveVector = base.inputBank.aimDirection;
                    this.characterDirection.forward = base.inputBank.aimDirection;
                    movementMultiplier = defaultMovementMultiplier;
                }
            }

            if (!base.inputBank.skill1.down)
            {
                ifButtonLifted = true;
            }

            if (!ifButtonLifted && base.isAuthority && base.age >= duration * heldButtonThreshold && domainController.DomainEntryAllowed())
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Cancel out into Domain shift skill state
                    base.outer.SetInterruptState(new DomainEnterState { shouldForceUpwards = true }, InterruptPriority.Frozen);
                }
            }

            base.Update();

            stopwatch += Time.deltaTime;

            if (this.age >= (this.duration * moveStartFrac) && this.age <= (this.duration * moveEndFrac)) 
            {
                if (stopwatch >= pulseRate)
                {
                    stopwatch = 0f;

                    if (base.isAuthority)
                    {
                        FireAttack();
                    }
                }
            }

            if (this.age >= (this.duration * this.earlyExitTime) && base.isAuthority)
            {
                //Check this first.
                if (base.inputBank.skill1.down || bufferNextMove)
                {
                    this.SetNextState();
                    return;
                }

                Modules.BodyInputCheckHelper.CheckForOtherInputs(base.skillLocator, isAuthority, base.inputBank);
            }

            if (this.age >= (this.duration * this.moveCancelEndTime) && base.isAuthority) 
            {
                if (inputBank.moveVector != new Vector3(0, 0, 0)) 
                {
                    this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                    return;
                }
            }

            if (this.age >= this.duration && base.isAuthority)
            {
                this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                return;
            }

        }


        // Client sided function. Don't call on server.
        internal void FireAttack() 
        {
            attack.position = this.gameObject.transform.position;
            BlastAttack.Result result = attack.Fire();
            UpdateMeleeRootMotion(movementMultiplier);

            if (result.hitCount > 0) 
            {
                new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_nml_4").Send(R2API.Networking.NetworkDestination.Clients);
                if (orbController)
                {
                    orbController.AddToIncrementor(Modules.StaticValues.flatAmountToGrantOnPrimaryHit * result.hitCount * basePulseRate);
                }

            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
            this.characterBody.SetAimTimer(0);
            base.PlayAnimation("Body", "BufferEmpty");

            // Become visible again
            if (weaponModelHandler)
            {
                weaponModelHandler.SetStateForModelAndSubmachine(true);
            }
        }


        protected void PlayAttackAnimation()
        {
            base.PlayAnimation("Body", "primary4", "attack.playbackRate", duration);
        }

        protected void SetNextState()
        {
            // Move to Primary5
            if (!base.isGrounded)
            {
                if (domainController && domainController.GetDomainState())
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new PrimaryDomainAerialStart { }, InterruptPriority.Skill);
                        return;
                    }
                }

                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    base.outer.SetInterruptState(new PrimaryAerialStart { }, InterruptPriority.Skill);
                    return;
                }
            }

            if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
            {
                base.outer.SetInterruptState(new Primary5 { }, InterruptPriority.Skill);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
