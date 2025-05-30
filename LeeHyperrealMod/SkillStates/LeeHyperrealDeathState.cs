﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LeeHyperrealMod.SkillStates
{
    internal class LeeHyperrealDeathState : GenericCharacterDeath
    {
        internal float triggerRagdollFrac = 0.97f;
        internal bool triggeredRagdoll = false;
        internal float duration = 3.66f;
        LeeHyperrealDomainController domainController;
        OrbController orbController;

        public override void OnEnter()
        {
            base.OnEnter();
            domainController = gameObject.GetComponent<LeeHyperrealDomainController>();
            orbController = gameObject.GetComponent<OrbController>();
            base.PlayAnimation("Death", "FullBody, Override", "attack.playbackRate", duration);
            
            if (domainController.GetDomainState()) 
            {
                domainController.DisableDomain(false);
            }

            orbController.isExecutingSkill = true;

            bool loreModeCheck = Modules.Config.loreMode.Value && Modules.Survivors.LeeHyperreal.voiceDisabledSkins.Contains((int)characterBody.skinIndex);

            if (!isGrounded || loreModeCheck)
            {
                TriggerRagdoll(true);
            }
            else 
            {

                if (Modules.Config.voiceEnabled.Value && !loreModeCheck)
                {
                    if (Modules.Config.voiceLanguageOption.Value == Modules.Config.VoiceLanguage.ENG)
                    {
                        Util.PlaySound("Play_Lee_Death_Voice_EN", gameObject);
                    }
                    else
                    {
                        Util.PlaySound("Play_Lee_Death_Voice_JP", gameObject);
                    }
                }
            }
        }

        public override bool shouldAutoDestroy
        {
            get
            {
                return false;
            }
        }

        public void TriggerRagdoll(bool useForce)
        {
            triggeredRagdoll = true;
            Vector3 vector = Vector3.up * 5f;
            if (base.characterMotor)
            {
                vector += base.characterMotor.velocity;
                base.characterMotor.enabled = false;
            }
            if (base.cachedModelTransform)
            {
                RagdollController ragdollController = base.cachedModelTransform.GetComponent<RagdollController>();
                if (ragdollController)
                {
                    ragdollController.BeginRagdoll(useForce ? vector : Vector3.zero);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration * triggerRagdollFrac)
            {
                if (!triggeredRagdoll) 
                {
                    TriggerRagdoll(false);
                }
            }

            if (base.fixedAge > duration) 
            {
                if (!triggeredRagdoll) 
                {
                    TriggerRagdoll(false);
                }
            }

            if (NetworkServer.active && base.fixedAge > 8f)
            {
                EntityState.Destroy(base.gameObject);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
