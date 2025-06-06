﻿using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.SkillStates.BaseStates;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.YellowOrb
{
    internal class YellowOrbFinisher : BaseMeleeAttack
    {
        internal OverlapAttack attack;

        internal HitBoxGroup hitBoxGroup;

        internal int attackAmount;
        internal float partialAttack;

        internal float attackStart = 0.154f;
        internal float attackEnd = 0.24f;
        internal float exitEarlyFrac = 0.26f;

        internal RootMotionAccumulator rma;
        internal OrbController orbController;
        private Transform baseTransform;

        internal float playSoundFrac = 0.12f;
        internal bool hasPlayedSound = false;
        bool hasUnsetOrbController;
        public override void OnEnter()
        {

            this.hitboxName = "ShortMelee";
            base.OnEnter();
            duration = 1.13f;

            this.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.NoneSpecified);
            this.damageCoefficient = Modules.StaticValues.yellowOrbFinisherDamageCoefficient;
            this.procCoefficient = Modules.StaticValues.yellowOrbFinisherProcCoefficient;
            this.pushForce = Modules.StaticValues.yellowOrbFinisherPushForce;
            this.bonusForce = Vector3.up;
            this.baseDuration = 2.366f;
            this.attackStartTime = attackStart;
            this.attackEndTime = attackEnd;
            this.baseEarlyExitTime = exitEarlyFrac;
            this.hitStopDuration = 0.05f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "HenrySwordSwing";
            this.hitSoundString = "";
            this.muzzleString = "BaseTransform";

            enableParry = true;
            parryLength = Modules.StaticValues.yellowOrbFinisherParryLength;
            parryTiming = Modules.StaticValues.yellowOrbFinisherParryTiming;
            parryPauseLength = Modules.StaticValues.yellowOrbFinisherParryPauseLength;
            parryProjectileTiming = Modules.StaticValues.yellowOrbFinisherParryProjectileTimingStart;
            parryProjectileTimingEnd = Modules.StaticValues.yellowOrbFinisherParryProjectileTimingEnd;

            base.OnEnter();

            this.swingEffectPrefab = Modules.ParticleAssets.RetrieveParticleEffectFromSkin("yellowOrbKick", characterBody);
            this.hitEffectPrefab = Modules.ParticleAssets.RetrieveParticleEffectFromSkin("yellowOrbSwingHit", characterBody);

            InitMeleeRootMotion();

            orbController = gameObject.GetComponent<OrbController>();
            if (orbController)
            {
                orbController.isExecutingSkill = true;
            }
            characterMotor.velocity.y = 0f;

            //ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
            //baseTransform = childLocator.FindChild("BaseTransform");

            ////Play Effect
            //EffectData effectData = new EffectData
            //{
            //    origin = baseTransform.position,
            //    rotation = Quaternion.LookRotation(characterDirection.forward),
            //    scale = 1.25f,
            //};
            //EffectManager.SpawnEffect(Modules.ParticleAssets.yellowOrbKick, effectData, true);
        }
        
        protected override void PlayAttackAnimation()
        {
            PlayAnimation("Body", "yellowOrb2", "attack.playbackRate", duration);
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

        public override void OnExit()
        {
            base.OnExit();
            if (orbController && !hasUnsetOrbController)
            {
                orbController.isExecutingSkill = false;
            }
            PlayAnimation("Body", "BufferEmpty");
        }

        public override void Update()
        {
            base.Update();
            if ((base.inputBank.skill3.down || base.inputBank.skill4.down) && base.isAuthority)
            {
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
                return;
            }
            UpdateMeleeRootMotion(1f);

            if (base.age >= duration * exitEarlyFrac && base.isAuthority) 
            {
                if (orbController && !hasUnsetOrbController)
                {
                    orbController.isExecutingSkill = false;
                    hasUnsetOrbController = true;
                }
                if (base.inputBank.moveVector != Vector3.zero) 
                {
                    this.outer.SetInterruptState(new LeeHyperrealCharacterMain(), InterruptPriority.Skill);
                    return;
                }
                Modules.BodyInputCheckHelper.CheckForOtherInputs(base.skillLocator, isAuthority, base.inputBank);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration * playSoundFrac && base.isAuthority && !hasPlayedSound) 
            {
                hasPlayedSound = true;
                new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_skill_yellow_dilie").Send(R2API.Networking.NetworkDestination.Clients);
                new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_skill_yellow_fire").Send(R2API.Networking.NetworkDestination.Clients);
            }

            if (fixedAge >= duration && base.isAuthority) 
            {
                base.outer.SetNextStateToMain();
                return;
            }
        }

        protected override void HitSoundCallback()
        {
            new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_yellow_2").Send(R2API.Networking.NetworkDestination.Clients);
        }

        protected override void SetNextState()
        {
            return;
        }

        protected override void OnHitEnemyAuthority()
        {
            //Do something on hit.
        }

    }
}
