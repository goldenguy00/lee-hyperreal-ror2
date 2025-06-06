﻿using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using LeeHyperrealMod.SkillStates.BaseStates;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.Modules.Networking;
using R2API.Networking;
using System.Collections.Generic;
using System.Linq;
using R2API.Networking.Interfaces;
using LeeHyperrealMod.Content.Controllers;

namespace LeeHyperrealMod.SkillStates.LeeHyperreal.Ultimate
{
    internal class UltimateDomain : BaseRootMotionMoverState
    {
        public LeeHyperrealDomainController docon;
        private UltimateCameraController ultimateCameraController;
        private BulletController bulletController;
        private OrbController orbController;

        public float start = 0;
        public float earlyEnd = 0.65f;
        public float effectPlay = 0.55f;
        public bool hasPlayedEffect = false;
        public float fireTime = 0.2f;
        public float fireEndTime = 0.31f;
        public double fireInterval;
        public float finalInterval = 0.2f;
        public float duration = StaticValues.ultimateDomainDuration;
        public double fireStopwatch;
        public float finalStopwatch;
        public bool preFinalBlastTriggered = false;
        public bool finalBlastTriggered = false;

        public int domainHitCount;

        internal BlastAttack blastAttack;

        internal bool isStrong;
        internal float procCoefficient = Modules.StaticValues.redOrbProcCoefficient;
        internal float damageCoefficient = Modules.StaticValues.redOrbDomainDamageCoefficient;

        private float movementMultiplier = 1.5f;
        private int fireCount;
        private int sightStacks; //annschauung

        private bool setCease = false;
        private float playCeaseFrac = 0.188f;
        private bool hasCeased = false;

        CharacterGravityParameters gravParams;
        CharacterGravityParameters oldGravParams;
        public List<GameObject> domainClones;
        public override void OnEnter()
        {
            base.OnEnter();
            characterBody.isSprinting = false;
            docon = gameObject.GetComponent<LeeHyperrealDomainController>();
            ultimateCameraController = gameObject.GetComponent<UltimateCameraController>();
            bulletController = gameObject.GetComponent<BulletController>();
            orbController = gameObject.GetComponent<OrbController>();
            domainClones = new List<GameObject>();

            if (orbController)
            {
                orbController.isExecutingSkill = true;
            }

            if (bulletController.inSnipeStance && isAuthority)
            {
                bulletController.UnsetSnipeStance();
            }

            sightStacks = docon.GetIntuitionStacks();

            domainHitCount = (sightStacks * StaticValues.ultimateDomainFireCount);

            if (domainHitCount == 0) 
            {
                domainHitCount = StaticValues.ultimateDomainFireCount;
            }

            fireInterval = (duration * fireEndTime - duration * fireTime) / (float)domainHitCount;

            if (sightStacks == 0) 
            {
                sightStacks = 1;
            }
            docon.SetFlagForAchievement();
            docon.DisableDomain(false);
            docon.SetTapped();
            docon.DisableLoopEffect();

            characterMotor.velocity.y = 0f;
            oldGravParams = base.characterMotor.gravityParameters;
            gravParams = new CharacterGravityParameters();
            gravParams.environmentalAntiGravityGranterCount = 1;
            gravParams.channeledAntiGravityGranterCount = 1;

            characterMotor.gravityParameters = gravParams;

            if (base.isAuthority && Modules.Config.ceaseChance.Value != 0f) 
            {
                //Roll Cease chance
                float rand = UnityEngine.Random.Range(0f, 100f);
                if (rand <= Modules.Config.ceaseChance.Value) 
                {
                    setCease = true;
                }
            }


            if (isAuthority)
            {
                new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_skill_ex_ultimate").Send(NetworkDestination.Clients);

                bool loreModeCheck = Modules.Config.loreMode.Value && Modules.Survivors.LeeHyperreal.voiceDisabledSkins.Contains((int)characterBody.skinIndex);

                if (Modules.Config.voiceEnabled.Value && !setCease && !loreModeCheck)
                {
                    if (Modules.Config.voiceLanguageOption.Value == Modules.Config.VoiceLanguage.ENG)
                    {
                        new PlaySoundNetworkRequest(characterBody.netId, "Play_Lee_Domain_Ult_Voice_EN").Send(NetworkDestination.Clients);
                    }
                    else
                    {
                        new PlaySoundNetworkRequest(characterBody.netId, "Play_Lee_Domain_Ult_Voice_JP").Send(NetworkDestination.Clients);
                    }
                }
            }


            rma = InitMeleeRootMotion();
            rmaMultiplier = movementMultiplier;

            Ray aimRay = base.GetAimRay();

            blastAttack = new BlastAttack
            {
                attacker = gameObject,
                inflictor = null,
                teamIndex = base.GetTeam(),
                position = gameObject.transform.position,
                radius = Modules.StaticValues.ultimateDomainBlastRadius,
                falloffModel = BlastAttack.FalloffModel.None,
                baseDamage = damageStat * Modules.StaticValues.ultimateDomainMiniDamageCoefficient, //multiply by anschauung stacks //no do not
                baseForce = 0f,
                bonusForce = Vector3.zero,
                crit = RollCrit(),
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Special),
                losType = BlastAttack.LoSType.None,
                canRejectForce = false,
                procChainMask = new ProcChainMask(),
                procCoefficient = procCoefficient,
                attackerFiltering = AttackerFiltering.NeverHitSelf
            };

            base.GetModelAnimator().SetFloat("attack.playbackRate", 1f);
            base.characterDirection.forward = inputBank.aimDirection;

            Freeze();
            PlayAttackAnimation();

            ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
            Transform baseTransform = childLocator.FindChild("BaseTransform");
            if (base.isAuthority)
            {
                if (Modules.Config.enableUltimateCamera.Value)
                {
                    ultimateCameraController.TriggerDomainUlt();
                }

                Vector3 targetDir = Camera.main.transform.position - baseTransform.position;
                EffectData effectData = new EffectData
                {
                    origin = gameObject.transform.position,
                    rotation = Quaternion.LookRotation(targetDir.normalized, Vector3.up),
                    scale = 1.25f,
                };
                EffectManager.SpawnEffect(ParticleAssets.RetrieveParticleEffectFromSkin("ultimateDomainBulletFinisher", characterBody), effectData, true);
            }

            GameObject domainClone = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin(Helpers.RetrieveClonePrefab(characterBody), characterBody), baseTransform);
            LeeHyperrealCloneController cloneController = domainClone.GetComponent<LeeHyperrealCloneController>();
            cloneController.animationTrigger = "ultDomain1";
            cloneController.shouldFadeout = true;

            GameObject domainClone2 = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin(Helpers.RetrieveClonePrefab(characterBody), characterBody), baseTransform);
            LeeHyperrealCloneController cloneController2 = domainClone2.GetComponent<LeeHyperrealCloneController>();
            cloneController2.animationTrigger = "ultDomain2";
            cloneController2.shouldFadeout = true;

            GameObject domainClone3 = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin(Helpers.RetrieveClonePrefab(characterBody), characterBody), baseTransform);
            LeeHyperrealCloneController cloneController3 = domainClone3.GetComponent<LeeHyperrealCloneController>();
            cloneController3.animationTrigger = "ultDomain3";
            cloneController3.shouldFadeout = true;

            domainClones.Add(domainClone);
            domainClones.Add(domainClone2);
            domainClones.Add(domainClone3);

            if (NetworkServer.active)
            {
                //Set Invincibility cause fuck you.
                characterBody.SetBuffCount(Modules.Buffs.invincibilityBuff.buffIndex, 1);
            }
        }

        public void Freeze()
        {
            BullseyeSearch search = new BullseyeSearch
            {

                teamMaskFilter = TeamMask.GetEnemyTeams(characterBody.teamComponent.teamIndex),
                filterByLoS = false,
                searchOrigin = characterBody.corePosition,
                searchDirection = UnityEngine.Random.onUnitSphere,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = 200f,
                maxAngleFilter = 360f
            };

            search.RefreshCandidates();
            search.FilterOutGameObject(characterBody.gameObject);

            List<HurtBox> target = search.GetResults().ToList<HurtBox>();
            foreach (HurtBox singularTarget in target)
            {
                if (singularTarget.healthComponent && singularTarget.healthComponent.body)
                {
                    //stop time for all enemies within this radius

                    //Chat.AddMessage("freeze enemy");
                    new SetFreezeOnBodyRequest(singularTarget.healthComponent.body.masterObjectId, StaticValues.ultimateDomainDuration).Send(NetworkDestination.Clients);
                }
            }
        }
        protected void PlayAttackAnimation()
        {
            PlayAnimation("Body", "UltimateDomain", "attack.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (base.isAuthority)
            {
                if (Modules.Config.enableUltimateCamera.Value)
                {
                    ultimateCameraController.UnsetUltimate();
                }

                skillLocator.special.UnsetSkillOverride(skillLocator.special, LeeHyperrealMod.Modules.Survivors.LeeHyperreal.domainUltimateSkill, RoR2.GenericSkill.SkillOverridePriority.Contextual);
                skillLocator.special.rechargeStopwatch = docon.ultCooldownBeforeSwitch;
                skillLocator.special.stock = docon.ultStockBeforeSwitch;
            }
            PlayCrossfade("Body", "IdleEmptyHand", 0.05f);

            docon.JustEnded = true;
            
            if (NetworkServer.active)
            {
                //Set Invincibility cause fuck you.
                characterBody.SetBuffCount(Modules.Buffs.invincibilityBuff.buffIndex, 0);
            }

            if (orbController)
            {
                orbController.isExecutingSkill = false;
            }

            characterMotor.gravityParameters = oldGravParams;

            if (bulletController.snipeAerialPlatform) 
            {
                bulletController.snipeAerialPlatform.GetComponent<DestroyPlatformOnDelay>().StartDestroying();
                bulletController.snipeAerialPlatform = null;
            }

            foreach (GameObject obj in domainClones)
            {
                Destroy(obj);
            }

            docon.SetFlagForAchievement();
        }

        public override void Update()
        {
            base.Update();

            if (!base.isGrounded) 
            {
                if (!bulletController.snipeAerialPlatform) 
                {
                    ChildLocator childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
                    Transform baseTransform = childLocator.FindChild("BaseTransform");
                    bulletController.snipeAerialPlatform = UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("snipeAerialFloor", characterBody), baseTransform.position, Quaternion.identity);
                }
            }

            if (age >= duration * earlyEnd && base.isAuthority)
            {
                Modules.BodyInputCheckHelper.CheckForOtherInputs(skillLocator, isAuthority, inputBank);
            }

            if (age >= duration * playCeaseFrac && !hasCeased && base.isAuthority && setCease) 
            {
                //Play cease effect.
                hasCeased = true;

                new PlaySoundNetworkRequest(characterBody.netId, "Play_cease_your_existance_NOW").Send(NetworkDestination.Clients);
                UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("ultimateDomainCEASEYOUREXISTANCE", characterBody), Camera.main.transform);
            }

            if (age >= duration * effectPlay && !hasPlayedEffect && base.isAuthority && !setCease) 
            {
                hasPlayedEffect = true;
                UnityEngine.Object.Instantiate(ParticleAssets.RetrieveParticleEffectFromSkin("ultimateDomainFinisherEffect", characterBody), Camera.main.transform);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //Able to be cancelled after this.
            if (fixedAge >= duration * fireTime && fixedAge <= duration * (fireEndTime + 0.1f) && base.isAuthority)
            {
                // Take the time between start and end, space each hit accordingly and smash all the hits within a small timeframe.
                if (fireStopwatch <= 0f && fireCount < domainHitCount)
                {
                    //mini hits
                    fireStopwatch = fireInterval;
                    fireCount++;
                    BlastAttack.Result result = blastAttack.Fire();

                    if (result.hitCount > 0)
                    {
                        new PlaySoundNetworkRequest(characterBody.netId, "Play_c_liRk4_imp_ex_3_2").Send(R2API.Networking.NetworkDestination.Clients);

                    }
                }
                else if (fireStopwatch > 0f && fireCount < domainHitCount)
                {
                    fireStopwatch -= Time.fixedDeltaTime;
                }
            }

            if (fixedAge >= duration * effectPlay && base.isAuthority) 
            {
                if (!finalBlastTriggered) 
                {
                    finalBlastTriggered = true;
                    blastAttack.baseDamage = damageStat * Modules.StaticValues.ultimateDomainDamageCoefficient;
                    blastAttack.bonusForce = Vector3.up;
                    blastAttack.baseForce = 1000f;
                    blastAttack.Fire();
                }
            }


            if (fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public void OnHitEnemyAuthority()
        {
            //Do something on hit.
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
