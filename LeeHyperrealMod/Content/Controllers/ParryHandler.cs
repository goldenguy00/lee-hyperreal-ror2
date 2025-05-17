using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.Modules;
using R2API.Networking;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using R2API.Networking.Interfaces;
using EntityStates;
using System.Linq;
using UnityEngine.Events;

namespace LeeHyperrealMod.Content.Controllers
{
    class ParryHandler
    {
        //Should be instantiated normally and run through the life cycle explicitly
        public ParryMonitor monitor;
        public CharacterDirection characterDirection;
        public CharacterBody body;
        public CharacterMotor motor;
        public Animator animator;
        public InputBankTest inputBank;
        public BulletController bulletController;

        //Length
        public float duration;
        private float stopwatch;

        // trigger
        private bool parryTriggered;
        private bool isInParryFreeze;
        private bool resetComboOnParry;

        // Parry attributes
        public float parryTiming;
        public float parryLength;
        public float parryProjectileTimingStart;
        public float parryPauseLength = 0.75f;
        public float parryProjectileTimingEnd;
        public bool parryAutoTransitionWithoutInput;
        public Transform ParryTransform = null;

        // Hit Pause
        private BaseState.HitStopCachedState hitStopCachedState;
        private bool inHitPause;
        public float hitStopDuration = 0.012f;
        private float hitPauseTimer;
        private Vector3 storedVelocity;
        private string animatorPauseParameter = "attack.playbackRate";
        private bool isEnemyParryFrozen;

        // Locking Vars
        private bool lockIntoParryState;

        //Action
        public delegate void ParryDelegate();
        public ParryDelegate onParry;

        public ParryHandler(ParryMonitor monitor, 
            CharacterDirection characterDirection, 
            CharacterBody body, 
            CharacterMotor motor, 
            Animator animator, 
            InputBankTest inputBank,
            BulletController bulletController) 
        {
            this.monitor = monitor;
            this.characterDirection = characterDirection;
            this.body = body;
            this.motor = motor;
            this.animator = animator;
            this.inputBank = inputBank;
            this.bulletController = bulletController;
        }

        public void FixedUpdate() 
        {
            if (lockIntoParryState) 
            {
                //Do nothing else.
                return;
            }

            // Handle State Transition
            if (body.hasEffectiveAuthority && resetComboOnParry)
            {
                //AutoTransition after out of hitpause.
                if (parryAutoTransitionWithoutInput && !inHitPause) 
                {
                    lockIntoParryState = true;
                    onParry();
                    return;
                }

                //Otherwise handle this on tap.
                if (inputBank.skill1.down && !inputBank.skill1.hasPressBeenClaimed)
                {
                    inputBank.skill1.hasPressBeenClaimed = true;
                    lockIntoParryState = true;
                    onParry();
                    return;
                }
            }

            // Handle Deflect
            if (stopwatch >= duration * parryProjectileTimingStart && body.hasEffectiveAuthority && stopwatch <= duration * parryProjectileTimingEnd)
            {
                Deflect(); 
            }
            
            //Handle the Buff application
            if (stopwatch >= duration * parryTiming && body.hasEffectiveAuthority && !parryTriggered)
            {
                parryTriggered = true;
                body.ApplyBuff(Modules.Buffs.parryBuff.buffIndex, 1, parryLength);
            }

            //If parried through networkrequest, handle it now!
            if (body.hasEffectiveAuthority && monitor.GetPauseTrigger()) 
            {
                //Set Parry pause
                isInParryFreeze = true;
                resetComboOnParry = true;
                monitor.SetPauseTrigger(false);

                HandleParry();
            }

            //Handle Hit Pause on Parry
            if (hitPauseTimer <= 0f && inHitPause)
            {
                ConsumeHitStopCachedState(hitStopCachedState, motor, animator);
                inHitPause = false;
                isInParryFreeze = false;
                motor.velocity = storedVelocity;
            }

            if (!inHitPause)
            {
                stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (motor) motor.velocity = Vector3.zero;
                if (animator) animator.SetFloat("attack.playbackRate", 0f);
                this.hitPauseTimer -= Time.fixedDeltaTime;
            }
        }

        public bool GetParryFreezeState() 
        {
            return isInParryFreeze;
        }

        internal void TriggerHitPause(float duration)
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = motor.velocity;
                hitStopCachedState = CreateHitStopCachedState(motor, animator, animatorPauseParameter);
                hitPauseTimer = duration;
                inHitPause = true;
            }
        }

        protected Ray GetAimRay()
        {
            if (inputBank)
            {
                return new Ray(inputBank.aimOrigin, inputBank.aimDirection);
            }
            return new Ray(body.gameObject.transform.position, body.gameObject.transform.forward);
        }

        private BaseState.HitStopCachedState CreateHitStopCachedState(CharacterMotor characterMotor, Animator animator, string playbackRateAnimationParameter)
        {
            BaseState.HitStopCachedState hitStopCachedState = default(BaseState.HitStopCachedState);
            hitStopCachedState.characterVelocity = new Vector3(characterMotor.velocity.x, Mathf.Max(0f, characterMotor.velocity.y), characterMotor.velocity.z);
            hitStopCachedState.playbackName = playbackRateAnimationParameter;
            hitStopCachedState.playbackRate = animator.GetFloat(hitStopCachedState.playbackName);
            return hitStopCachedState;
        }

        private void ConsumeHitStopCachedState(BaseState.HitStopCachedState hitStopCachedState, CharacterMotor characterMotor, Animator animator)
        {
            characterMotor.velocity = hitStopCachedState.characterVelocity;
            animator.SetFloat(hitStopCachedState.playbackName, hitStopCachedState.playbackRate);
        }

        public void TriggerEnemyFreeze()
        {
            if (!isEnemyParryFrozen)
            {
                //set the parryFreeze so we don't need to freeze/unfreeze everything every frame.
                isEnemyParryFrozen = true;

                BullseyeSearch search = new BullseyeSearch
                {
                    teamMaskFilter = TeamMask.GetEnemyTeams(body.teamComponent.teamIndex),
                    filterByLoS = false,
                    searchOrigin = body.corePosition,
                    searchDirection = UnityEngine.Random.onUnitSphere,
                    sortMode = BullseyeSearch.SortMode.Distance,
                    maxDistanceFilter = 100f,
                    maxAngleFilter = 360f
                };

                search.RefreshCandidates();
                search.FilterOutGameObject(body.gameObject);

                List<HurtBox> target = search.GetResults().ToList<HurtBox>();
                foreach (HurtBox singularTarget in target)
                {
                    if (singularTarget.healthComponent && singularTarget.healthComponent.body)
                    {
                        //stop time for all enemies within this radius

                        new SetFreezeOnBodyRequest(singularTarget.healthComponent.body.masterObjectId, StaticValues.bigParryFreezeDuration).Send(NetworkDestination.Clients);
                    }
                }
            }

        }

        private void HandleParry()
        {
            Vector3 position = body.gameObject.transform.position + (characterDirection.forward + Vector3.up * 1f) * 2f;

            if (monitor.ShouldDoBigParry) 
            {
                bulletController.GrantEnhancedBullet(Modules.StaticValues.enhancedBulletGrantOnDamageParryBig);
                monitor.ShouldDoBigParry = false;
                //Determine if it's big pause or not.
                TriggerHitPause(Modules.StaticValues.bigParryLeeFreezeDuration);
                TriggerEnemyFreeze();
                new PlaySoundNetworkRequest(body.netId, "Play_Big_parry").Send(NetworkDestination.Clients);
                if (ParryTransform)
                {
                    position = ParryTransform.position;
                }
                EffectManager.SpawnEffect(ParticleAssets.RetrieveParticleEffectFromSkin("bigParry", body),
                    new EffectData
                    {
                        origin = position,
                        scale = 2f
                    }, true);

                return;
            }

            TriggerHitPause(parryPauseLength);

            bulletController.GrantEnhancedBullet(Modules.StaticValues.enhancedBulletGrantOnDamageParry);
            new PlaySoundNetworkRequest(body.netId, "Play_Normal_parry").Send(NetworkDestination.Clients);
            
            if (ParryTransform)
            {
                position = ParryTransform.position;
            }
            EffectManager.SpawnEffect(ParticleAssets.RetrieveParticleEffectFromSkin("normalParry", body),
                new EffectData
                {
                    origin = position,
                    scale = 2f,
                    rotation = Quaternion.LookRotation(GetAimRay().direction.normalized, Vector3.up)
                },
                true);
        }

        private void Deflect()
        {
            Vector3 parryPosition = body.gameObject.transform.position + (characterDirection.forward + Vector3.up * 1f) * 2f;
            if (ParryTransform)
            {
                parryPosition = ParryTransform.position;
            }
            Collider[] array = Physics.OverlapSphere(parryPosition, Modules.StaticValues.parryProjectileRadius, LayerIndex.projectile.mask);

            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc)
                {
                    if (pc.owner != body.gameObject)
                    {
                        Ray aimRay = GetAimRay();
                        resetComboOnParry = true;
                        pc.owner = body.gameObject;

                        FireProjectileInfo info = new FireProjectileInfo()
                        {
                            projectilePrefab = pc.gameObject,
                            position = pc.gameObject.transform.position,
                            rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                            owner = body.gameObject,
                            damage = body.damage * Modules.StaticValues.parryProjectileDamageMultiplier,
                            force = 200f,
                            crit = body.RollCrit(),
                            damageColorIndex = DamageColorIndex.Default,
                            target = null,
                            speedOverride = 120f,
                            fuseOverride = -1f
                        };
                        ProjectileManager.instance.FireProjectile(info);

                        new PlaySoundNetworkRequest(body.netId, "Play_Normal_parry").Send(NetworkDestination.Clients);
                        EffectManager.SpawnEffect(ParticleAssets.RetrieveParticleEffectFromSkin("normalParry", body),
                            new EffectData
                            {
                                origin = parryPosition,
                                scale = 2f,
                                rotation = Quaternion.LookRotation(GetAimRay().direction.normalized, Vector3.up)
                            },
                            true);

                        bulletController.GrantEnhancedBullet(Modules.StaticValues.enhancedBulletGrantOnProjectileParry);
                        UnityEngine.Object.Destroy(pc.gameObject);
                    }
                }
            }
        }
    }
}
