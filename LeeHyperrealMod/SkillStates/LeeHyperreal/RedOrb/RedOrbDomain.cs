﻿using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using LeeHyperrealMod.SkillStates.BaseStates;
using System;
using LeeHyperrealMod.Modules;
using UnityEngine.UIElements.UIR;
using LeeHyperrealMod.Content.Controllers;
using System.ComponentModel;
using System.Collections.Generic;
using LeeHyperrealMod.Modules.Networking;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.RedOrb
{
    internal class RedOrbDomain : BaseRootMotionMoverState
    {

        public float start = 0;
        public float earlyEnd = 0.35f;
        public float fireTime = 0.17f;
        public float baseFireInterval = Modules.StaticValues.redOrbDomainBaseFireInterval;
        public float fireInterval;
        public float duration = 4f;
        public int moveStrength; //1-3

        public float fireStopwatch;
        public int fireCount = 0;

        internal BlastAttack blastAttack;
        internal OrbController orbController;

        internal bool isStrong;
        internal float procCoefficient = Modules.StaticValues.redOrbDomainProcCoefficient;
        internal float damageCoefficient = Modules.StaticValues.redOrbDomainDamageCoefficient;

        Transform baseTransform;
        internal Vector3 positionToSpawnClone;
        internal bool spawnedClone;

        CharacterGravityParameters gravParams;
        CharacterGravityParameters oldGravParams;
        float turnOffGravityFrac = 0.298f;
        float gravOffTimer = 0.18f;

        private float movementMultiplier = 1f;

        internal Transform boxGunTransform;
        internal bool effectPlayed;

        internal bool hasHit = false;
        internal bool hasHit2 = false;
        internal bool hasHit3 = false;

        float invincibilityOnFrac = 0.05f;
        float invincibilityOffFrac = 0.2f;
        bool invincibilityApplied = false;
        LeeHyperrealDomainController domainController;

        bool hasUnsetOrbController;

        public override void OnEnter()
        {
            base.OnEnter();
            effectPlayed = false;
            domainController = gameObject.GetComponent<LeeHyperrealDomainController>();
            orbController = gameObject.GetComponent<OrbController>();
            if (orbController)
            {
                orbController.isExecutingSkill = true;
            }

            rma = InitMeleeRootMotion();
            rmaMultiplier = movementMultiplier;
            fireInterval = baseFireInterval / attackSpeedStat;

            oldGravParams = base.characterMotor.gravityParameters;
            gravParams = new CharacterGravityParameters();
            gravParams.environmentalAntiGravityGranterCount = 1;
            gravParams.channeledAntiGravityGranterCount = 1;

            characterMotor.gravityParameters = gravParams;

            Ray aimRay = base.GetAimRay();
            characterMotor.velocity.y = 0f;

            ChildLocator childLocator = base.GetModelTransform().GetComponent<ChildLocator>();

            boxGunTransform = childLocator.FindChild("GunCasePos");
            baseTransform = childLocator.FindChild("BaseTransform");
            positionToSpawnClone = baseTransform.position;

            PlayAttackAnimation();

            blastAttack = new BlastAttack
            {
                attacker = gameObject,
                inflictor = null,
                teamIndex = TeamIndex.Player,
                position = boxGunTransform.position,
                radius = Modules.StaticValues.redOrbDomainBlastRadius,
                falloffModel = BlastAttack.FalloffModel.None,
                baseDamage = damageStat * Modules.StaticValues.redOrbDomainDamageCoefficient * (moveStrength >= 3 ? Modules.StaticValues.redOrbDomainTripleMultiplier : 1),
                baseForce = Modules.StaticValues.redOrbDomainBlastForce,
                bonusForce = Vector3.zero,
                crit = RollCrit(),
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.NoneSpecified),
                losType = BlastAttack.LoSType.None,
                canRejectForce = false,
                procChainMask = new ProcChainMask(),
                procCoefficient = procCoefficient,
                attackerFiltering = AttackerFiltering.NeverHitSelf
            };

            if (domainController)
            {
                GameObject domainClone = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin(Helpers.RetrieveClonePrefab(characterBody), characterBody), baseTransform.position, Quaternion.LookRotation(characterDirection.forward));
                LeeHyperrealCloneController cloneController = domainClone.GetComponent<LeeHyperrealCloneController>();
                cloneController.animationTrigger = "redDomain";
                domainController.redCloneObjects.Add(domainClone);
            }

        }

        protected void PlayAttackAnimation()
        {
            PlayAnimation("Body", "RedOrbDomain", "attack.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            characterMotor.gravityParameters = oldGravParams;
            PlayAnimation("Body", "BufferEmpty");
            if (orbController && !hasUnsetOrbController)
            {
                orbController.isExecutingSkill = false;
            }

            if (NetworkServer.active)
            {
                //Set Invincibility cause fuck you.
                characterBody.ClearTimedBuffs(Modules.Buffs.invincibilityBuff.buffIndex);
            }
        }

        public override void Update()
        {
            base.Update();

            if (age >= duration * invincibilityOnFrac && NetworkServer.active && !invincibilityApplied)
            {
                invincibilityApplied = true;
                float buffDuration = (duration * invincibilityOffFrac) - (duration * invincibilityOnFrac);
                characterBody.AddTimedBuff(Modules.Buffs.invincibilityBuff.buffIndex, buffDuration);
            }

            if ((base.inputBank.skill3.down || base.inputBank.skill4.down) && base.isAuthority)
            {
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
                return;
            }
            //Able to be cancelled after this.
            if (age >= duration * earlyEnd && base.isAuthority)
            {
                if (orbController && !hasUnsetOrbController)
                {
                    hasUnsetOrbController = true;
                    orbController.isExecutingSkill = false;
                }
                if (inputBank.moveVector != Vector3.zero) 
                {
                    this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                    return;
                }
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
            }

            if(age >= duration * gravOffTimer && base.isAuthority)
            {
                base.characterMotor.gravityParameters = oldGravParams;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration * fireTime && !spawnedClone) 
            {
                if (domainController)
                {
                    spawnedClone = true;
                    GameObject domainClone = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin(Helpers.RetrieveClonePrefab(characterBody), characterBody), positionToSpawnClone, Quaternion.LookRotation(characterDirection.forward));
                    LeeHyperrealCloneController cloneController = domainClone.GetComponent<LeeHyperrealCloneController>();
                    cloneController.animationTrigger = "redDomainStart";
                    domainController.redCloneObjects.Add(domainClone);
                }
            }

            if (fixedAge >= duration * fireTime && isAuthority)
            {
                if (!effectPlayed) 
                {
                    effectPlayed = true;
                    EffectManager.SimpleEffect(ParticleAssets.RetrieveParticleEffectFromSkin("redOrbDomainFloorImpact", characterBody), boxGunTransform.position, Quaternion.identity, true);
                }
                if(fireStopwatch <= 0f && fireCount < StaticValues.redOrbDomainFireCount)
                {
                    fireStopwatch = fireInterval;
                    blastAttack.position = boxGunTransform.position;
                    BlastAttack.Result result = blastAttack.Fire();
                    if (result.hitCount > 0) 
                    {
                        OnHitEnemyAuthority(result.hitPoints);
                    }
                    fireCount++;
                }
                else
                {
                    fireStopwatch -= Time.fixedDeltaTime;
                }

                if (fixedAge >= duration * fireTime * 1.2f && !hasHit)
                {
                    hasHit = true;
                    new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_ex_2_1").Send(R2API.Networking.NetworkDestination.Clients);
                }
                if (fixedAge >= duration * fireTime * 1.5f && !hasHit2)
                {
                    hasHit2 = true;
                    new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_ex_2_2").Send(R2API.Networking.NetworkDestination.Clients);
                }
                if (fixedAge >= duration * fireTime * 1.8f && !hasHit3)
                {
                    hasHit3 = true;
                    new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_ex_2_1").Send(R2API.Networking.NetworkDestination.Clients);
                }
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public void OnHitEnemyAuthority(BlastAttack.HitPoint[] hitPoints)
        {
            //Do something on hit.
            foreach (BlastAttack.HitPoint point in hitPoints) 
            {
                hasHit = true;
                hasHit2 = true;
                EffectManager.SimpleEffect(ParticleAssets.RetrieveParticleEffectFromSkin("redOrbDomainHit", characterBody), point.hitPosition, Quaternion.identity, true);
            }
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (fixedAge >= duration * earlyEnd)
            {
                return InterruptPriority.Skill;
            }

            return InterruptPriority.Frozen;
        }
    }
}
