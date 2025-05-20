using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class PrimaryEntry : BaseSkillState
    {
        public override void OnEnter() 
        {
            base.OnEnter();

            //Decide where to start the move:

            LeeHyperrealDomainController domainController = base.gameObject.GetComponent<LeeHyperrealDomainController>();

            if (base.isAuthority)
            {
                if (!characterMotor.isGrounded)
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
                    base.outer.SetInterruptState(new Primary1 { }, InterruptPriority.Skill);
                    return;
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
        }
    }
}
