﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.SkillStates.LeeHyperreal.DomainShift;
using R2API.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class PrimaryAerialLoop : BaseSkillState
    {
        public Vector3 velocity;
        public LeeHyperrealDomainController domainController;

        public float heldStopwatch;
        public static float heldLengthToTrigger = 0.5f;

        public float initialAirTime;

        public override void OnEnter()
        {
            base.OnEnter();
            domainController = gameObject.GetComponent<LeeHyperrealDomainController>();
            //Do nothing until you hit the ground.
            if (base.isAuthority && isGrounded)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Send instantly to end state
                    base.outer.SetInterruptState(new PrimaryAerialSlam { airTime = fixedAge }, InterruptPriority.Skill);
                    return;
                }
            }

            if (base.isAuthority)
            {
                base.characterBody.ApplyBuff(Modules.Buffs.fallDamageNegateBuff.buffIndex, 1);
            }

            base.characterMotor.velocity = Vector3.SmoothDamp(base.characterMotor.velocity, ((Vector3.down).normalized * Modules.StaticValues.primaryAerialSlamSpeed), ref velocity, 0.2f);
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
                heldStopwatch = 0f;
            }

            if (base.inputBank.skill1.down && base.isAuthority)
            {
                heldStopwatch = Time.deltaTime;
            }

            if (base.isAuthority && heldStopwatch >= heldLengthToTrigger && domainController.DomainEntryAllowed())
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
                    base.outer.SetInterruptState(new PrimaryAerialSlam { airTime = fixedAge + initialAirTime }, InterruptPriority.PrioritySkill);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //Increase y vel until grounded
            base.characterMotor.velocity = Vector3.SmoothDamp(base.characterMotor.velocity, ((Vector3.down).normalized * Modules.StaticValues.primaryAerialSlamSpeed), ref velocity, 0.1f);


            //Only transition on grounded.
            if (base.isAuthority && isGrounded)
            {
                if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
                {
                    //Send instantly to end state
                    base.outer.SetInterruptState(new PrimaryAerialSlam { airTime = fixedAge + initialAirTime }, InterruptPriority.PrioritySkill);
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(initialAirTime);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            initialAirTime = reader.ReadSingle();
        }
    }
}
