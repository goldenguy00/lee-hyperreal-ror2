using EntityStates;
using RoR2.CharacterAI;
using LeeHyperrealMod.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace LeeHyperrealMod.SkillStates
{
    public class Freeze : BaseSkillState
    {
        Animator animator;
        internal float duration;
        BaseAI[] aiComponents;
        internal float previousAttackSpeedStat;
        private Animator modelAnimator;
        private TemporaryOverlay temporaryOverlay;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelAnimator = base.GetModelAnimator();
            if (this.modelAnimator)
            {
                this.modelAnimator.enabled = false;
            }
            if (base.rigidbody && !base.rigidbody.isKinematic)
            {
                base.rigidbody.velocity = Vector3.zero;
                if (base.rigidbodyMotor)
                {
                    base.rigidbodyMotor.moveVector = Vector3.zero;
                }
            }
            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            if (NetworkServer.active) 
            {
                characterBody.AddTimedBuff(Modules.Buffs.glitchEffectBuff, duration);
            }
        }
        public override void OnExit()
        {
            if (this.modelAnimator)
            {
                this.modelAnimator.enabled = true;
            }
            CharacterModel model = this.GetModelTransform().GetComponent<CharacterModel>();
            if (model) 
            {
                model.forceUpdate = true;
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            attackSpeedStat = 0f;


            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(duration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            duration = reader.ReadSingle();
        }
    }
}