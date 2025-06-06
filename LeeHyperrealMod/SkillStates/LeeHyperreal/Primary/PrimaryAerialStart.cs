﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.SkillStates.LeeHyperreal.DomainShift;
using RoR2;
using LeeHyperrealMod.Modules;
using UnityEngine;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class PrimaryAerialStart : BaseRootMotionMoverState
    {
        LeeHyperrealDomainController domainController;
        private float duration = 0.6f;
        private float stopwatch = 0f;

        public static float heldButtonThreshold = 0.3f;
        public bool ifButtonLifted = false;

        public ParryHandler parryHandler;

        public override void OnEnter()
        {
            base.OnEnter();

            parryHandler = new ParryHandler(gameObject.GetComponent<ParryMonitor>(),
                characterDirection,
                characterBody,
                characterMotor,
                GetModelAnimator(),
                inputBank,
                gameObject.GetComponent<BulletController>());

            parryHandler.onParry += SetNextStateOnParry;
            parryHandler.duration = duration;
            parryHandler.parryTiming = Modules.StaticValues.primaryAerialParryTiming;
            parryHandler.parryLength = Modules.StaticValues.primaryAerialParryLength;
            parryHandler.parryProjectileTimingStart = Modules.StaticValues.primaryAerialProjectileTimingStart;
            parryHandler.parryProjectileTimingEnd = Modules.StaticValues.primaryAerialProjectileTimingEnd;
            parryHandler.parryPauseLength = Modules.StaticValues.primaryAerialParryPauseLength;
            parryHandler.parryAutoTransitionWithoutInput = true;

            ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();

            parryHandler.ParryTransform = childLocator.FindChild("WeaponCase");

            //Play animation for going straight down. There will be a switch to change to domain variant in this state.
            //There are no attacks on this until you hit the ground.
            domainController = gameObject.GetComponent<LeeHyperrealDomainController>();

            //Continue with straight down attack
            base.PlayAnimation("Body", "Midair Attack Start", "attack.playbackRate", duration);

            Util.PlaySound("Play_c_liRk4_atk_nml_5_xuli", base.gameObject);
            //Automatically leads into Midair Attack Loop
            characterMotor.velocity.y = 0f;        /*reset current Velocity at start.*/

            if (base.isAuthority)
            {
                PlaySwing("BaseTransform", 1.25f, ParticleAssets.RetrieveParticleEffectFromSkin("primary5Swing", characterBody));
            }
        }

        public void SetNextStateOnParry()
        {
            if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
            {
                this.outer.SetInterruptState(new PrimaryAerialLoop(), InterruptPriority.Skill);
                return;

            }
        }


        public void PlaySwing(string muzzleString, float swingScale, GameObject effectPrefab)
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


        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();

            if (!base.inputBank.skill1.down && base.isAuthority)
            {
                ifButtonLifted = true;
            }

            if (!ifButtonLifted && base.isAuthority && stopwatch >= duration * heldButtonThreshold && domainController.DomainEntryAllowed())
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Cancel out into Domain shift skill state
                    base.outer.SetInterruptState(new DomainEnterState { shouldForceUpwards = true }, InterruptPriority.Frozen);
                    return;
                }
            }

            if (base.isAuthority && isGrounded)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Send instantly to end state
                    base.outer.SetInterruptState(new PrimaryAerialSlam { airTime = fixedAge }, InterruptPriority.Skill);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            parryHandler.FixedUpdate();

            if (!parryHandler.GetParryFreezeState()) 
            {
                stopwatch += Time.fixedDeltaTime;
            }


            if (base.isAuthority && isGrounded)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Send instantly to end state
                    base.outer.SetInterruptState(new PrimaryAerialSlam { airTime = fixedAge }, InterruptPriority.Frozen);
                    return;
                }
            }

            if (stopwatch >= duration && base.isAuthority)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Send to loop state.
                    base.outer.SetInterruptState(new PrimaryAerialLoop { initialAirTime = fixedAge }, InterruptPriority.Skill);
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
