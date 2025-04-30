﻿using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeeHyperrealMod.Modules
{
    internal static class Helpers
    {
        internal const string agilePrefix = "<style=cIsUtility>Agile.</style> ";

        internal static string ScepterDescription(string desc)
        {
            return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
        }

        internal static string SubDesc(string desc) 
        {
            return $"<style=cSub>{desc}</style>";
        }

        internal static string UtilDesc(string desc)
        {
            return $"<style=cIsUtility>{desc}</style>";
        }

        internal static string DmgDesc(string desc)
        {
            return $"<style=cIsDamage>{desc}</style>";
        }

        internal static string Stack(string desc)
        {
            return $"<style=cStack>{desc}</style>";
        }

        internal static string Keyword(string desc)
        {
            return $"<style=cKeywordName>{desc}</style>";
        }

        internal static string Mono(string desc)
        {
            return $"<style=cMono>{desc}</style>";
        }

        internal static string UserSetting(string desc)
        {
            return $"<style=cUserSetting>{desc}</style>";
        }

        internal static string Lee(string desc, string altDesc) 
        {
            return Lee(desc, altDesc, true);
        }

        internal static string Lee(string desc, string altDesc, bool isColoured) 
        {

            if (Modules.Config.loreMode.Value)
            {
                if (isColoured) 
                {
                    return altDesc;
                }
                return $"<color=#{ColorUtility.ToHtmlStringRGB(Modules.StaticValues.bodyColor)}>{altDesc}</color>";
            }
            if (isColoured)
            {
                return desc;
            }
            return $"<color=#{ColorUtility.ToHtmlStringRGB(Modules.StaticValues.bodyColor)}>{desc}</color>";
        }

        internal static string BlueOrb() 
        {
            return $"<color=#4e69ff>Blue</color>";
        }

        internal static string RedOrb()
        {
            return $"<color=#FF0000>Red</color>";
        }

        internal static string YellowOrb()
        {
            return $"<color=#faf400>Yellow</color>";
        }

        public static T[] Append<T>(ref T[] array, List<T> list)
        {
            var orig = array.Length;
            var added = list.Count;
            Array.Resize<T>(ref array, orig + added);
            list.CopyTo(array, orig);
            return array;
        }

        /// <summary>
        /// gets langauge token from achievement's registered identifier
        /// </summary>
        public static string GetAchievementNameToken(string identifier)
        {
            return $"ACHIEVEMENT_{identifier.ToUpperInvariant()}_NAME";
        }
        public static Func<T[], T[]> AppendDel<T>(List<T> list) => (r) => Append(ref r, list);

        public static string RetrieveClonePrefab(CharacterBody body) 
        {
            if (Modules.Config.loreMode.Value)
            {
                switch (body.skinIndex)
                {
                    case 0:
                        return "prospectorMasterClone";
                    case 1:
                        return "comradeMasterClone";
                    case 4:
                        return "scarletMasterClone";
                    default:
                        return "leeMasterClone";
                }
            }
            else
            {
                switch (body.skinIndex)
                {
                    case 2:
                        return "scarletMasterClone";
                    case 3:
                        return "prospectorMasterClone";
                    case 4:
                        return "comradeMasterClone";
                    default:
                        return "leeMasterClone";
                }
            }
        }
        public static void PlaySwingEffect(string muzzleString, float swingScale, GameObject effectPrefab, GameObject gameObject)
        {
            ModelLocator component = gameObject.GetComponent<ModelLocator>();
            if (component && component.modelTransform)
            {
                ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                if (component2)
                {
                    int childIndex = component2.FindChildIndex(muzzleString);
                    Transform transform = component2.FindChild(childIndex);
                    if (transform)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = transform.position,
                            scale = swingScale,
                        };
                        effectData.SetChildLocatorTransformReference(gameObject, childIndex);
                        EffectManager.SpawnEffect(effectPrefab, effectData, true);
                    }
                }
            }
        }
    }
}