﻿using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using RoR2;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules;
using UnityEngine.UIElements.UIR;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Evade
{
    internal class Evade : BaseRootMotionMoverState
    {
        public static float baseDuration = 2.0f;
        public static float duration = 2.0f;
        public static float minimumDuration = 1.0f;
        public BulletController bulletController;
        public OrbController orbController;

        public static float dodgeFOV = EntityStates.Commando.DodgeState.dodgeFOV;

        private Animator animator;

        private bool isForwardRoll;

        private float earlyExitFrac = 0.28f;
        private float p3Cancel = 0.18f;
        private Vector3 forwardDirection;
        private Vector3 moveVector;

        private float movementMultiplier = 1.6f;

        private float disableInvincibility = 0.25f;
        public bool unsetSnipe = false;
        private float movementMultiplierPrimary3 = 2.0f;

        private Vector3 previousMovementVector;

        public Transform baseTransform;

        CharacterGravityParameters gravParams;
        CharacterGravityParameters oldGravParams;
        float turnOffGravityFrac = 0.13f;

        public override void OnEnter()
        {
            base.OnEnter();

            bulletController = gameObject.GetComponent<BulletController>();
            orbController = gameObject.GetComponent<OrbController>();

            if (orbController) 
            {
                orbController.isExecutingSkill = false;
            }

            if (bulletController && unsetSnipe) 
            {
                bulletController.UnsetSnipeStance();
                if (bulletController.snipeAerialPlatform) 
                {
                    bulletController.snipeAerialPlatform.GetComponent<DestroyPlatformOnDelay>().StartDestroying();
                    bulletController.snipeAerialPlatform = null; //unset so it doesn't follow the player.
                }
            }

            duration = baseDuration / attackSpeedStat;

            if (duration < minimumDuration) 
            {
                duration = minimumDuration;
            }

            previousMovementVector = characterMotor.velocity;

            animator = GetModelAnimator();
            forwardDirection = GetAimRay().direction;
            Vector3 backwardsDirection = forwardDirection * -1f;
            animator.SetFloat("attack.playbackRate", attackSpeedStat);
            moveVector = inputBank.moveVector;
            rmaMultiplier = movementMultiplier;
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(Modules.Buffs.invincibilityBuff.buffIndex, baseDuration * disableInvincibility);
            }

            ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
            baseTransform = childLocator.FindChild("BaseTransform");

            if (inputBank.moveVector == Vector3.zero)
            {
                EffectManager.SpawnEffect(
                    ParticleAssets.RetrieveParticleEffectFromSkin("dodgeBackwards", characterBody), 
                    new EffectData 
                    { 
                        origin = baseTransform.position,
                        scale = 1.25f,
                        rotation = Quaternion.LookRotation(GetAimRay().direction * -1f)
                    }, 
                    true);
                isForwardRoll = false;
                PlayAnimation();
                return;
            }

            //if (Vector3.Dot(backwardsDirection, inputBank.moveVector) >= 0.833f)
            //{
            //    isForwardRoll = false;
            //    PlayAnimation();
            //    return;
            //}

            if (base.isAuthority) 
            {
                EffectManager.SpawnEffect(
                    ParticleAssets.RetrieveParticleEffectFromSkin("dodgeForwards", characterBody),
                    new EffectData
                    {
                        origin = baseTransform.position,
                        scale = 1.25f,
                        rotation = Quaternion.LookRotation(inputBank.moveVector.normalized)
                    },
                    true);
            }
            isForwardRoll = true;
            PlayAnimation();

            this.characterDirection.forward = base.inputBank.moveVector;
            this.characterDirection.moveVector = base.inputBank.moveVector;

            oldGravParams = base.characterMotor.gravityParameters;
            gravParams = new CharacterGravityParameters();
            gravParams.environmentalAntiGravityGranterCount = 1;
            gravParams.channeledAntiGravityGranterCount = 1;

            characterMotor.gravityParameters = gravParams;
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

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
            float y = characterMotor.velocity.y;
            characterMotor.velocity = Vector3.zero;
            characterMotor.velocity.y = y;


            if (isForwardRoll)
            {
                characterDirection.moveVector = moveVector;
            }
            else
            {
                characterDirection.moveVector = forwardDirection;
            }

            if (age >= duration * p3Cancel && isAuthority)
            {
                if (!isForwardRoll)
                {
                    if (inputBank.skill1.down)
                    {
                        //Go to Primary 3.
                        if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                        {
                            outer.SetInterruptState(new Primary.Primary3 { xzMovementMultiplier = movementMultiplierPrimary3 * Modules.StaticValues.ScaleMoveSpeed(moveSpeedStat) }, InterruptPriority.PrioritySkill);
                            return;
                        }
                    }
                }
            }

            if (age <= duration * turnOffGravityFrac)
            {
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.gravityParameters = gravParams;
            }

            if (age >= duration * turnOffGravityFrac)
            {
                base.characterMotor.gravityParameters = oldGravParams;
            }

            if (age >= duration * earlyExitFrac && isAuthority)
            {
                if (!isForwardRoll)
                {
                    if (inputBank.skill1.down)
                    {
                        if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                        {
                            //Go to Primary 3.
                            outer.SetInterruptState(new Primary.Primary3 { xzMovementMultiplier = movementMultiplierPrimary3 * Modules.StaticValues.ScaleMoveSpeed(moveSpeedStat) }, InterruptPriority.PrioritySkill);
                            return;
                        }
                    }
                }
                if (inputBank.moveVector != Vector3.zero) 
                {
                    outer.SetInterruptState(new LeeHyperrealCharacterMain { }, InterruptPriority.Skill);
                    return;
                }
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
            }

            if (age >= duration && isAuthority)
            {
                outer.SetInterruptState(new LeeHyperrealCharacterMain { }, InterruptPriority.Skill);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            float y = base.characterMotor.velocity.y * 0.2f;
            base.characterMotor.velocity = new Vector3(previousMovementVector.x, y, previousMovementVector.z);
            base.characterBody.SetAimTimer(0);
            base.PlayAnimation("Body", "BufferEmpty");
            base.characterMotor.gravityParameters = oldGravParams;
        }

        public void PlayAnimation()
        {
            if (isForwardRoll)
            {
                PlayAnimation("Body", "evadeForward", "attack.playbackRate", duration);
                return;
            }
            PlayAnimation("Body", "evadeBack", "attack.playbackRate", duration);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(unsetSnipe);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            unsetSnipe = reader.ReadBoolean();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (age >= duration * earlyExitFrac)
            {
                return InterruptPriority.Skill;
            }
            else
            {
                return InterruptPriority.PrioritySkill;
            }
        }
    }
}
