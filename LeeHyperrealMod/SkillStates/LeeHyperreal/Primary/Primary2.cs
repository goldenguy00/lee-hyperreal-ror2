﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.SkillStates.LeeHyperreal.DomainShift;
using R2API.Networking.Interfaces;
using RoR2;
using System.Collections.Generic;
using LeeHyperrealMod.Modules;
using UnityEngine;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class Primary2 : BaseMeleeAttack
    {
        public static float initialSpeedCoefficient = 3f;
        public static float finalSpeedCoefficient = 0f;

        public static float moveStartFrac = 0.1f;
        public static float moveEndFrac = 0.15f;

        public RootMotionAccumulator rma;

        public static float heldButtonThreshold = 0.25f;
        public bool ifButtonLifted = false;

        public float attack2StartFrac = 0.125f;
        public float attack2EndFrac = 0.150f;
        public bool hasFired2 = false;

        public float attack3StartFrac = 0.14f;
        public float attack3EndFrac = 0.170f;
        public bool hasFired3 = false;

        public float attack4StartFrac = 0.216f;
        public float attack4EndFrac = 0.25f;
        public bool hasFired4 = false;

        public float attack5StartFrac = 0.231f;
        public float attack5EndFrac = 0.27f;
        public bool hasFired5 = false;

        private LeeHyperrealDomainController domainController;

        public override void OnEnter()
        {
            domainController = this.GetComponent<LeeHyperrealDomainController>();
            this.hitboxName = "Primary2";

            this.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary);
            this.damageCoefficient = StaticValues.primary2DamageCoefficient;
            this.procCoefficient = StaticValues.primary2ProcCoefficient;
            this.pushForce = StaticValues.primary2PushForce;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 2.366f;
            this.attackStartTime = 0.067f;
            this.attackEndTime = 0.1f;
            this.moveCancelEndTime = 0.3f;
            this.baseEarlyExitTime = 0.225f;

            this.bufferActiveTime = 0.15f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = StaticValues.primary2HitHopVelocity;

            this.swingSoundString = "HenrySwordSwing";
            this.hitSoundString = "Play_c_liRk4_imp_nml_2_2";
            this.muzzleString = "BaseTransform";
            this.swingEffectPrefab = null;
            base.OnEnter();

            this.hitEffectPrefab = ParticleAssets.RetrieveParticleEffectFromSkin("primary2hit1", characterBody);
            InitMeleeRootMotion();

            this.swingScale = 1.25f;

            //Play the effect after setting the muzzle string, it'll be spawned in a random place if you don't
            if (base.isAuthority)
            {
                PlayExtraSwingEffect(ParticleAssets.RetrieveParticleEffectFromSkin("primary2Shot", characterBody));
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
                    base.characterMotor.rootMotion = a * scale;
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

            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateMeleeRootMotion(1.5f);
            if (base.stopwatch >= duration * attack2StartFrac && base.stopwatch <= duration * attack2EndFrac && base.isAuthority && !hasFired2)
            {
                FireSecondAttack();
            }
            if (base.stopwatch >= duration * attack3StartFrac && base.stopwatch <= duration * attack3EndFrac && base.isAuthority && !hasFired3)
            {
                FireThirdAttack();
            }
            if (base.stopwatch >= duration * attack4StartFrac && base.stopwatch <= duration * attack4EndFrac && base.isAuthority && !hasFired4)
            {
                FireFourthAttack();
            }
            if (base.stopwatch >= duration * attack5StartFrac && base.stopwatch <= duration * attack5EndFrac && base.isAuthority && !hasFired5)
            {
                FireFifthAttack();
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
        internal void FireThirdAttack()
        {
            if (!hasFired3)
            {
                this.hasFired3 = true;

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

        internal void FireFourthAttack()
        {
            if (!hasFired4)
            {
                this.hasFired4 = true;

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

        internal void FireFifthAttack()
        {
            if (!hasFired5)
            {
                this.hasFired5 = true;

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
            base.OnExit();
            base.PlayAnimation("Body", "BufferEmpty");
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Body", "primary2", "attack.playbackRate", duration);
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
            new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_nml_2_2").Send(R2API.Networking.NetworkDestination.Clients);
        }

        protected override void SetNextState()
        {
            // Move to Primary3
            if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
            {
                base.outer.SetInterruptState(new Primary3 { }, InterruptPriority.Skill);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
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
    }
}
