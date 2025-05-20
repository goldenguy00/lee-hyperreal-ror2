using EntityStates;
using UnityEngine;
using LeeHyperrealMod.SkillStates.LeeHyperreal.Evade;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.Content.Controllers;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Secondary
{
    internal class IdleSnipe : BaseSkillState
    {
        Animator animator;
        public float duration = 2.133f;
        Vector3 velocity = Vector3.zero;
        BulletController bulletController;
        public bool stocktaken;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = false;
            //Enter the snipe stance, move to IdleSnipe
            animator = this.GetModelAnimator();
            animator.SetFloat("attack.playbackRate", 1f);
            bulletController = gameObject.GetComponent<BulletController>();

            base.characterDirection.forward = Vector3.SmoothDamp(base.characterDirection.forward, base.inputBank.aimDirection, ref velocity, 0.1f, 100f, Time.deltaTime);
            base.characterMotor.velocity = Vector3.zero;

            PlayAttackAnimation();

            //characterBody.SetAimTimer(duration + 1f);
            if (!bulletController.snipeAerialPlatform && !isGrounded)
            {
                ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
                Transform baseTransform = childLocator.FindChild("BaseTransform");
                bulletController.snipeAerialPlatform = Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("snipeAerialFloor", characterBody), baseTransform.position, Quaternion.identity);
            }

            if (!base.inputBank.skill2.down && Modules.Config.allowSnipeButtonHold.Value && base.isAuthority)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != InterruptPriority.Death)
                {
                    base.outer.SetNextState(new ExitSnipe());
                    return;
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

            base.characterDirection.forward = Vector3.SmoothDamp(base.characterDirection.forward, base.inputBank.aimDirection, ref velocity, 0.1f, 300f, Time.deltaTime);
            base.characterDirection.moveVector = new Vector3(0, 0, 0);
            base.characterMotor.velocity = new Vector3(0, 0, 0);

            if (base.isAuthority)
            {
                //Check for dodging. Otherwise ignore.
                if (base.inputBank.skill1.down && !base.inputBank.skill4.down)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new Snipe { }, InterruptPriority.PrioritySkill);
                        return;
                    }
                }

                if (base.inputBank.jump.down && !base.inputBank.skill4.down)
                {
                    base.outer.SetInterruptState(new LeeHyperrealCharacterMain { forceJump = true }, InterruptPriority.Skill);
                    return;
                }

                if (base.inputBank.sprint.justPressed && characterBody.hasAuthority && !base.inputBank.skill4.down)
                {
                    this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                    return;
                }

                //Check for dodging. Otherwise ignore.
                if (base.inputBank.skill3.justPressed && skillLocator.utility.stock >= 1 && !stocktaken && !base.inputBank.skill4.down)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        skillLocator.utility.stock -= 1;
                        stocktaken = true;
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

                if (!base.inputBank.skill2.down && Modules.Config.allowSnipeButtonHold.Value && base.isAuthority && !base.inputBank.skill4.down)
                {
                    if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                    {
                        base.outer.SetInterruptState(new ExitSnipe(), InterruptPriority.PrioritySkill);
                    }
                }

                if ((base.inputBank.skill2.down || base.inputBank.skill4.down) && base.isAuthority)
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

        public void PlayAttackAnimation()
        {
            PlayAnimation("Body", "SnipeAim", "attack.playbackRate", duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}