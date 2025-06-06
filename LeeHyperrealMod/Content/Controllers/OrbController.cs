﻿//using ExtraSkillSlots;
using LeeHyperrealMod.Modules;
using LeeHyperrealMod.SkillStates.LeeHyperreal;
using LeeHyperrealMod.SkillStates.LeeHyperreal.RedOrb;
using LeeHyperrealMod.SkillStates.LeeHyperreal.YellowOrb;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static LeeHyperrealMod.Content.Controllers.BulletController;
using static LeeHyperrealMod.Content.Controllers.LeeHyperrealUIController;

namespace LeeHyperrealMod.Content.Controllers
{
    internal class OrbController : MonoBehaviour
    {
        // 16 orbs max
        // On fire or not
        // Orb Types

        internal enum OrbType : ushort

        {
            BLUE = 1,
            RED = 2,
            YELLOW = 3,
        }

        bool onFire = false;
        public List<OrbType> orbList;
        int OrbLimit = 16;
        float orbIncrementor = 0f;
        const bool autoIncrementOrbIncrementor = true;

        float orbUIStopwatch = 0f;
        float baseOrbGrantRate = Modules.StaticValues.LimitToGrantOrb;
        float orbGrantRate;

        float updateRate = 0.25f;

        LeeHyperrealUIController uiController;

        CharacterBody charBody;
        CharacterMaster characterMaster;
        bool baseAIPresent;

        EntityStateMachine[] stateMachines;

        InputContainer inputContainer;

        public bool isExecutingSkill = false;
        public bool isCheckingInput = false;

        public bool blueInputConsumed = false;
        public bool redInputConsumed = false;
        public bool yellowInputConsumed = false;

        public float isCheckingInputTimer = 0f;
        public float isExecutingInputTimer = 0f;

        public void Awake()
        {
            orbIncrementor = 0f;
            orbList = new List<OrbType>();
        }

        public void Start()
        {
            charBody = gameObject.GetComponent<CharacterBody>();
            uiController = gameObject.GetComponent<LeeHyperrealUIController>();
            if (LeeHyperrealPlugin.isControllerCheck)
            {
                InitializeInputBank();
            }
            stateMachines = charBody.gameObject.GetComponents<EntityStateMachine>();

            characterMaster = charBody.master;
            BaseAI baseAI = characterMaster.GetComponent<BaseAI>();
            baseAIPresent = baseAI;


            //For some reason on goboo's first spawn the master is just not there. However subsequent spawns work.
            // Disable the UI in this event.
            // Besides, there should never be a UI element related to a non-existant master on screen if the attached master/charbody does not exist.
            if (!characterMaster) baseAIPresent = true; // Disable UI Just in case.

            RecalcUpdateRate();
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InitializeInputBank()
        {
            inputContainer = new InputContainer(gameObject, this);
        }

        public void Hook()
        {

        }

        public void Unhook()
        {

        }

        public void RecalcUpdateRate() 
        {
            int alienHeadCount = charBody.inventory.GetItemCount(RoR2Content.Items.AlienHead);
            int purityCount = charBody.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
            //Flat difference for purity, percentage taken for alien head.

            float cooldownScale = 1f;
            for (int i = 0; i < alienHeadCount; i++)
            {
                cooldownScale *= 0.75f;
            }

            float flatCooldownReduction = 0f;
            if (purityCount > 0)
            {
                flatCooldownReduction += 2f + 1f * (float)(purityCount - 1);
            }

            orbGrantRate = Mathf.Min(this.baseOrbGrantRate, Mathf.Max(Modules.StaticValues.MinimumOrbGrantSpeed, this.baseOrbGrantRate * cooldownScale - flatCooldownReduction));
        }

        public void UpdateBracketPositionFromIntArr(int[] ints, OrbType orbType) 
        {
            int str = 0;
            for (int i = 0; i < ints.Length; i++)
            {
                if (ints[i] != -1) 
                {
                    str++;
                }
            }

            if (str != 0)
            {
                BracketType bracketType = BracketType.ONE;
                if (str == 1)
                {
                    bracketType = BracketType.ONE;
                }
                else if (str == 2)
                {
                    bracketType = BracketType.TWO;
                }
                else if (str == 3)
                {
                    bracketType = BracketType.THREE;
                }

                uiController.SetBracketOnOrb(ints[0], bracketType, orbType);
                return;
            }
            else 
            {
                //disable bracket.
                uiController.DisableBracket(orbType);
            }            
        }

        public void Update()
        {
            //Check input
            if (charBody.hasEffectiveAuthority && !PauseManager.isPaused && !baseAIPresent)
            {

                //If the orb list is greater than 8, it means that we don't need to update the glyph indicators cause there's nothing
                // Modifying the position.
                if (Modules.Config.isSimple.Value)
                {
                    //Check move validity of Blue, red and yellow, update the brackets accordingly.
                    UpdateBracketPositionFromIntArr(CheckMoveValidity(OrbType.BLUE), OrbType.BLUE);
                    UpdateBracketPositionFromIntArr(CheckMoveValidity(OrbType.RED), OrbType.RED);
                    UpdateBracketPositionFromIntArr(CheckMoveValidity(OrbType.YELLOW), OrbType.YELLOW);
                }
                else if(!Modules.Config.isSimple.Value) 
                {
                    uiController.DisableBracket(OrbType.BLUE);
                    uiController.DisableBracket(OrbType.RED);
                    uiController.DisableBracket(OrbType.YELLOW);
                }

                if (!isExecutingSkill && !isCheckingInput)
                {
                    bool triggeredSomething = false;
                    if (Modules.Config.isSimple.Value)
                    {
                         triggeredSomething = CheckSimpleInput();
                    }
                    else
                    {
                        CheckNonSimpleInput();
                    }

                    if (!triggeredSomething)
                    {
                        #region Controller Check
                        if (LeeHyperrealPlugin.isControllerCheck)
                        {
                            inputContainer.ExtraSkillSlotControllerInputCheck();
                        }
                        #endregion
                    }
                }

                //Failsafe in case this flag gets set, which happens often enough actually.
                if (isCheckingInput) 
                {
                    isCheckingInputTimer += Time.deltaTime;
                    if (isCheckingInputTimer >= 3f) 
                    {
                        isCheckingInput = false;
                        isCheckingInputTimer = 0f;
                    }
                }
                if (!isCheckingInput) 
                {
                    isCheckingInputTimer = 0f;
                }
                if (isExecutingSkill)
                {
                    isExecutingInputTimer += Time.deltaTime;
                    if (isExecutingInputTimer >= 3f)
                    {
                        isExecutingSkill = false;
                        isExecutingInputTimer = 0f;
                    }
                }
                if (!isExecutingSkill)
                {
                    isExecutingInputTimer = 0f;
                }



                if (uiController)
                {
                    orbUIStopwatch += Time.deltaTime;
                    if (orbUIStopwatch >= updateRate)
                    { 
                        orbUIStopwatch = 0f;
                        uiController.UpdateOrbList(orbList);
                        uiController.UpdateOrbAmount(orbList.Count, OrbLimit);
                    }
                }
            }
        }

        private bool CheckSimpleInput()
        {
            bool triggeredSomething = false;
            if (UnityEngine.Input.GetKeyDown(Modules.Config.blueOrbTrigger.Value.MainKey))
            {
                //Debug.Log($"blue key down: {UnityEngine.Input.GetKeyDown(Modules.Config.blueOrbTrigger.Value.MainKey)}");
                ConsumeOrbsSimple(OrbType.BLUE);
                triggeredSomething = true;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.redOrbTrigger.Value.MainKey))
            {
                ConsumeOrbsSimple(OrbType.RED);
                triggeredSomething = true;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.yellowOrbTrigger.Value.MainKey))
            {
                ConsumeOrbsSimple(OrbType.YELLOW);
                triggeredSomething = true;
            }

            return triggeredSomething;
        }

        private void CheckNonSimpleInput()
        {
            int SelectedIndex = -1;
            bool isAltPressed = UnityEngine.Input.GetKey(Modules.Config.orbAltTrigger.Value.MainKey);
            if (UnityEngine.Input.GetKeyDown(Modules.Config.orb1Trigger.Value.MainKey))
            {
                SelectedIndex = 1;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb2Trigger.Value.MainKey))
            {
                SelectedIndex = 2;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb3Trigger.Value.MainKey))
            {
                SelectedIndex = 3;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb4Trigger.Value.MainKey))
            {
                SelectedIndex = 4;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb5Trigger.Value.MainKey) && !Modules.Config.useOrbAltTrigger.Value)
            {
                SelectedIndex = 5;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb6Trigger.Value.MainKey) && !Modules.Config.useOrbAltTrigger.Value)
            {
                SelectedIndex = 6;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb7Trigger.Value.MainKey) && !Modules.Config.useOrbAltTrigger.Value)
            {
                SelectedIndex = 7;
            }
            else if (UnityEngine.Input.GetKeyDown(Modules.Config.orb8Trigger.Value.MainKey) && !Modules.Config.useOrbAltTrigger.Value)
            {
                SelectedIndex = 8;
            }

            if (isAltPressed && SelectedIndex != -1 && Modules.Config.useOrbAltTrigger.Value)
            {
                SelectedIndex += 4;
            }

            if (SelectedIndex != -1)
            {
                ConsumeOrbsFromSlot(SelectedIndex);
            }
        }


        // Checks if move is valid determined by an int. 
        // If value is greater than 0, an orb was found. 
        // Values returned will be the index in the list it was found at. 
        // If specified orb was not found, 0, 0, 0 will be returned. 
        // If there are no orbs, 0, 0 ,0 will be returned.
        public int[] CheckMoveValidity(OrbType type)
        {

            int objLimit = orbList.Count >= 8 ? 8 : orbList.Count;

            int[] result = { -1, -1, -1};
            int indexPtr = 0;

            // No Orbs to spend!
            if (objLimit == 0)
            {
                return result;
            }

            bool found = false;

            //Only check the first 8.
            for (int i = 0; i < objLimit; i++) 
            {
                if (orbList[i] == type)
                {
                    found = true;
                    result[indexPtr] = i;
                    indexPtr++;
                }

                // Previously found, but new one doesn't match the current one. end of sequence.
                if (found && orbList[i] != type) 
                {
                    return result;
                }

                if (indexPtr == 3) 
                {
                    return result;
                }
            }



            return result;
        }

        //
        public int ConsumeOrbsSimple(OrbType type) 
        {
            // Uses the CheckMoveValidity() and attempts to remove.     

            isCheckingInput = true;
            int[] moveValidity = CheckMoveValidity(type);
            int[] failedCheck = { -1, -1, -1 };

            if (failedCheck[0] == moveValidity[0] && failedCheck[1] == moveValidity[1] && failedCheck[2] == moveValidity[2]) 
            {
                // Disallowed from running.
                isCheckingInput = false;
                return 0;
            }

            // Inverse the array since we remove from the end of the list first.
            int strength = 0;
            for( int i = moveValidity.Length - 1; i > -1; i-- )
            {
                if (moveValidity[i] != -1) 
                {
                    orbList.RemoveAt(moveValidity[i]);
                    strength++;
                    if (uiController) 
                    {
                        uiController.PingSpecificOrb(moveValidity[i]); 
                    }


                }
            }


            if (uiController) 
            {
                uiController.UpdateOrbList(orbList);
            }

            TriggerOrbState(strength, type);
            //Success.
            isCheckingInput = false;
            return strength;
        }

        public void ConsumeOrbsFromSlot(int slotToConsumeFrom) 
        {
            //Check the first 8 orbs
            //Group into colours
            //check against the index (it'll be from 1 - 8)
            // match against the group that the index matches.  
            // count strength
            // Trigger state associated with that.

            isCheckingInput = true;
            List<Tuple<OrbType, Nullable<int>>> typeList = new List<Tuple<OrbType, Nullable<int>>>();

            //Build a list of types and group them.
            for (int i = 0; i < orbList.Count; i++)
            {
                if (i == 0)
                {
                    typeList.Add(new Tuple<OrbType, Nullable<int>>(orbList[i], 1));
                }
                else 
                {
                    //Type if matches the previous type.
                    if (typeList[typeList.Count - 1].Item1 == orbList[i]) 
                    {
                        // BRUGH LMAOOO
                        //Update the type and add to the strength count.
                        typeList[typeList.Count - 1] = new Tuple<OrbType, Nullable<int>>(typeList[typeList.Count - 1].Item1, typeList[typeList.Count - 1].Item2 + 1);
                    } 
                    else 
                    {
                        // Type if not matched.
                        //Create a new entry in type list.
                        typeList.Add(new Tuple<OrbType, Nullable<int>>(orbList[i], 1));
                    }
                }
            }


            //Check the list for the slot to consume from.
            //Increment by adding the strength of the previous type, and checking if the selected index is within that range.
            Nullable<int> slotPos = 0;
            bool found = false;
            for (int i = 0; i < typeList.Count; i++) 
            {
                slotPos += typeList[i].Item2;

                if (slotPos >= slotToConsumeFrom && !found)
                {
                    //Found the state we need to trigger off.   
                    found = true;

                    int startingIndex = 0;
                    int[] indexes = new int[3];
                    //Finding the damn indexes to remove.
                    for (int j = 0; j < typeList.Count; j++) 
                    {
                        if (j == i) 
                        {
                            //Indexing BRUHSIAUHSD I HATE ALL OF THIS LOGIC>
                            indexes[0] = startingIndex;
                            indexes[1] = typeList[i].Item2 >= 2 ? startingIndex + 1 : -1;
                            indexes[2] = typeList[i].Item2 >= 3 ? startingIndex + 2 : -1;

                            //End 
                            j = typeList.Count - 1;
                        }

                        startingIndex += (int)typeList[j].Item2;
                    }

                    //Debug.Log($"Non-simple: executed {typeList[i].Item1} : {typeList[i].Item2} ");

                    ClearSuggestedOrbs(indexes);
                    TriggerOrbState((int)typeList[i].Item2, typeList[i].Item1);
                }
            }

            isCheckingInput = false;
        }

        private void ClearSuggestedOrbs(int[] indexes)
        {
            for (int i = indexes.Length - 1; i > -1; i--)
            {
                if (indexes[i] != -1)
                {
                    orbList.RemoveAt(indexes[i]);
                    if (uiController)
                    {
                        uiController.PingSpecificOrb(indexes[i]);
                    }
                }
            }
        }

        public void TriggerOrbState(int strength, OrbType orbType) 
        {
            

            //Force this to true, preventing other states from running on top of this.
            isExecutingSkill = true;

            EntityStateMachine bodyMachine = null;
            
            foreach (EntityStateMachine esm in stateMachines) 
            {
                if (esm.customName == "Body") 
                {
                    bodyMachine = esm;
                }
            }

            if (bodyMachine) 
            {
                // Set Interrupt state to Frozen to override shit entirely
                switch (orbType)
                {
                    case OrbType.BLUE:
                        bodyMachine.SetInterruptState(new BlueOrb { moveStrength = strength}, EntityStates.InterruptPriority.Frozen);
                        break;
                    case OrbType.YELLOW:
                        bodyMachine.SetInterruptState(new YellowOrbEntry { moveStrength = strength }, EntityStates.InterruptPriority.Frozen);
                        break;
                    case OrbType.RED:
                        bodyMachine.SetInterruptState(new RedOrbEntry { moveStrength = strength }, EntityStates.InterruptPriority.Frozen);
                        break;
                    default:
                        break;
                }
            }
        }


        //Logic
        public void FixedUpdate()
        {
            if (charBody.hasEffectiveAuthority && !PauseManager.isPaused && !baseAIPresent)
            {
                orbIncrementor += Modules.StaticValues.flatIncreaseOrbIncrementor * Time.fixedDeltaTime;

                if (orbIncrementor >= orbGrantRate)
                {
                    orbIncrementor = 0f;
                    GrantOrb();
                }

                RecalcUpdateRate();
            }

            //string output = "";

            //foreach (OrbType type in orbList)
            //{
            //    switch (type)
            //    {
            //        case OrbType.BLUE:
            //            output += "B";
            //            break;
            //        case OrbType.RED:
            //            output += "R";
            //            break;
            //        case OrbType.YELLOW:
            //            output += "Y";
            //            break;
            //    }
            //}

            //Chat.AddMessage($"{orbIncrementor}");
            //Chat.AddMessage($"{output}");
        }

        public void GrantOrb()
        {
            //Disallow more than the limit specified.
            if (orbList.Count >= OrbLimit) 
            {
                return;
            }

            int chosen = UnityEngine.Random.Range(1, 4);

            OrbType chosenOrbType = (OrbType)chosen;

            orbList.Add(chosenOrbType);

            if (uiController)
            {
                uiController.UpdateOrbList(orbList);
            }
        }

        public void Grant3Ping(BulletType type) 
        {
            switch (type) 
            {
                case BulletType.BLUE:
                    orbList.Add(OrbType.BLUE);
                    orbList.Add(OrbType.BLUE);
                    orbList.Add(OrbType.BLUE);
                    break;
                case BulletType.RED:
                    orbList.Add(OrbType.RED);
                    orbList.Add(OrbType.RED);
                    orbList.Add(OrbType.RED);
                    break;
                case BulletType.YELLOW:
                    orbList.Add(OrbType.YELLOW);
                    orbList.Add(OrbType.YELLOW);
                    orbList.Add(OrbType.YELLOW);
                    break;
            }
        }

        public void AddToIncrementor(float amount) 
        {
            orbIncrementor += amount;
        }
    }
}
