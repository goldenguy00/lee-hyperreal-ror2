using EntityStates;
using LeeHyperrealMod.Content.Controllers;
using LeeHyperrealMod.Modules.Networking;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.SkillStates.LeeHyperreal.DomainShift;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Primary
{
    internal class Primary1 : BaseMeleeAttack
    {

        private float rollSpeed;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;

        public static float moveStartFrac = 0f;
        public static float moveEndFrac = 0.03f;
        private Ray aimRay;
        public RootMotionAccumulator rma;

        public static float heldButtonThreshold = 0.15f;
        public bool ifButtonLifted = false;

        private LeeHyperrealDomainController domainController;
        Transform baseTransform;
        /*
         
        internal bool enableParry;
        internal float parryLength;
        internal float parryTiming;
        internal bool parryTriggered;
        internal float parryPauseLength = 0.75f;
        internal ParryMonitor parryMonitor;
         */

        public override void OnEnter()
        {
            domainController = this.GetComponent<LeeHyperrealDomainController>();

            this.hitboxName = "ShortMelee";

            this.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary);
            this.damageCoefficient = StaticValues.primary1DamageCoefficient;
            this.procCoefficient = StaticValues.primary1ProcCoefficient;
            this.pushForce = StaticValues.primary1PushForce;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 2.23f;
            this.attackStartTime = 0.04f;
            this.attackEndTime = 0.15f;
            this.moveCancelEndTime = 0.17f;
            this.baseEarlyExitTime = 0.16f;
            this.bufferActiveTime = 0.10f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = StaticValues.primary1HitHopVelocity;

            this.swingSoundString = "";
            this.hitSoundString = "";
            this.muzzleString = "BaseTransform";
            this.swingScale = 1.25f;

            
            enableParry = true;
            parryLength = StaticValues.primary1ParryLength;
            parryTiming = StaticValues.primary1ParryTiming;
            parryPauseLength = StaticValues.primary1ParryPauseLength;
            parryProjectileTiming = StaticValues.primary1ParryProjectileTimingStart;
            parryProjectileTimingEnd = StaticValues.primary1ParryProjectileTimingEnd;

            base.OnEnter();
            InitMeleeRootMotion();

            this.swingEffectPrefab = ParticleAssets.RetrieveParticleEffectFromSkin("primary1Swing", characterBody);
            this.hitEffectPrefab = ParticleAssets.RetrieveParticleEffectFromSkin("primary1Hit", characterBody);

            ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
            baseTransform = childLocator.FindChild("BaseTransform");

            //Play Effect
            if (base.isGrounded) 
            {
                RoR2.EffectManager.SimpleEffect(
                    ParticleAssets.RetrieveParticleEffectFromSkin("primary1Floor", characterBody),
                    baseTransform.position,
                    Quaternion.LookRotation(new Vector3(GetAimRay().direction.x, 0f, GetAimRay().direction.z), Vector3.up),
                    true);
            }

            ParryTransform = childLocator.FindChild("FootL");
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

        public override void FixedUpdate() 
        {
            base.FixedUpdate();
            UpdateMeleeRootMotion(1.25f);
        }

        public override void Update()
        {

            if (!base.inputBank.skill1.down && base.isAuthority) 
            {
                ifButtonLifted = true;
            }

            if (!ifButtonLifted && base.isAuthority && base.stopwatch >= duration * heldButtonThreshold && domainController.DomainEntryAllowed()) 
            {
                //Cancel out into Domain shift skill state
                if (base.outer.state.GetMinimumInterruptPriority() != InterruptPriority.Death)
                {
                    base.outer.SetNextState(new DomainEnterState { shouldForceUpwards = true });
                    return;
                }
            }

            base.Update();
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("Body", "BufferEmpty");
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Body", "primary1", "attack.playbackRate", duration);
        }

        protected override void HitSoundCallback()
        {
            new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_nml_1").Send(R2API.Networking.NetworkDestination.Clients);
        }

        protected override void TriggerOrbIncrementor(int timesHit) 
        {
            if (orbController)
            {
                orbController.AddToIncrementor(Modules.StaticValues.flatAmountToGrantOnPrimaryHit * timesHit);
            }
        }

        protected override void SetNextState()
        {
            // Move to Primary2
            if (base.outer.state.GetMinimumInterruptPriority() != EntityStates.InterruptPriority.Death)
            {
                base.outer.SetNextState(new Primary2 { });
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
