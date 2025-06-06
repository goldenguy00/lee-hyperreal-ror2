﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.SkillStates.LeeHyperreal.Evade;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary
{
    internal class EnterSnipe : BaseSkillState
    {
        LeeHyperrealUIController uiController;
        BulletController bulletController;
        Animator animator;
        public float baseDuration = 2.133f;
        public float duration;
        public float earlyExitFrac = 0.22f;
        public bool consumedStocks = false;
        Vector3 velocity;
        GameObject platform;
        public string muzzleString = "BaseTransform";

        public override void OnEnter()
        {
            base.OnEnter();
            uiController = gameObject.GetComponent<LeeHyperrealUIController>();
            bulletController = gameObject.GetComponent<BulletController>();
            base.characterBody.isSprinting = false;
            base.characterMotor.velocity = new Vector3(0, 0, 0);
            base.characterDirection.moveVector = new Vector3(0, 0, 0);

            duration = baseDuration / base.attackSpeedStat;

            //Override the M1 skill with snipe.
            bulletController.SetSnipeStance();

            //Enter the snipe stance, move to IdleSnipe
            animator = this.GetModelAnimator();
            animator.SetFloat("attack.playbackRate", base.attackSpeedStat);
            PlayAttackAnimation();

            //Set direction
            base.characterDirection.forward = Vector3.SmoothDamp(base.characterDirection.forward, base.inputBank.aimDirection, ref velocity, 0.1f, 100f, Time.deltaTime);

            base.characterMotor.velocity = Vector3.zero;

            //characterBody.SetAimTimer(duration);
            ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
            Transform baseTransform = childLocator.FindChild("BaseTransform");
            if (!isGrounded)
            {
                bulletController.snipeAerialPlatform = Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("snipeAerialFloor", characterBody), baseTransform.position, Quaternion.identity);
            }

            Util.PlaySound("Play_c_liRk4_atk_ex_3_xuli", base.gameObject);
            if (base.isAuthority) 
            {
                PlaySwingEffect(1.25f, ParticleAssets.RetrieveParticleEffectFromSkin("snipeStart", characterBody), true);
            }
        }

        protected virtual void PlaySwingEffect(float scale, GameObject effectPrefab, bool aimRot = true)
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
                        Vector3 aimRotation = GetAimRay().direction;
                        EffectData effectData = new EffectData
                        {
                            origin = transform.position,
                            scale = scale,
                            rotation = Quaternion.LookRotation(new Vector3(aimRotation.x, 0f, aimRotation.z), Vector3.up),
                        };
                        if (aimRot)
                        {
                            effectData.rotation = Quaternion.LookRotation(GetAimRay().direction, Vector3.up);
                        }
                        //effectData.SetChildLocatorTransformReference(gameObject, childIndex);
                        EffectManager.SpawnEffect(effectPrefab, effectData, true);
                    }
                }
            }
            //EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void PlayAttackAnimation() 
        {
            PlayAnimation("Body", "SnipeEntry", "attack.playbackRate", duration);
        }

        public override void Update() 
        {
            base.Update();
            base.characterDirection.forward = Vector3.SmoothDamp(base.characterDirection.forward, base.inputBank.aimDirection, ref velocity, 0.1f, 100f, Time.deltaTime);
            base.characterDirection.moveVector = Vector3.zero;
            base.characterMotor.velocity = new Vector3(0, 0, 0);

            if (age >= duration * earlyExitFrac && base.isAuthority) 
            {
                if (base.inputBank.jump.down) 
                {
                    base.outer.SetInterruptState(new LeeHyperrealCharacterMain { forceJump = true }, InterruptPriority.Skill);
                    return;
                }

                if (base.inputBank.sprint.justPressed && characterBody.hasAuthority)
                {
                    this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                    return;
                }

                if (base.inputBank.skill1.down) 
                {
                    base.outer.SetInterruptState(new Snipe { }, InterruptPriority.PrioritySkill);
                    return;
                }

                if (base.inputBank.skill3.down && skillLocator.utility.stock >= 1 && !consumedStocks)
                {
                    skillLocator.utility.stock -= 1;
                    consumedStocks = true;
                    Vector3 result = Modules.StaticValues.CheckDirection(inputBank.moveVector, GetAimRay());

                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        inputBank.skill3.hasPressBeenClaimed = true;
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

                if (!base.inputBank.skill2.down && Modules.Config.allowSnipeButtonHold.Value && base.isAuthority)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new ExitSnipe(), InterruptPriority.Skill);
                    }
                }

                if ((base.inputBank.skill2.justPressed || base.inputBank.skill3.justPressed || base.inputBank.skill4.justPressed) && base.isAuthority)
                {
                    Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
