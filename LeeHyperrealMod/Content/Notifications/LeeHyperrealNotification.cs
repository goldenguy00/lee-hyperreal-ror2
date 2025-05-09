using LeeHyperrealMod.Content.Controllers;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeeHyperrealMod.Content.Notifications
{
    //Shamelessly stolen from HUNK: thank you for making my job easier ROB
    internal class LeeHyperrealNotification : GenericNotification
    {
        public Animator animator;
        public bool triggeredHide;

        public Material portraitOutline;
        public Material descriptionOutline;
        public Material descriptionInner;

        public void Start() 
        {
            // Replace everything with the notif text and image here as the prefab before was serialized. This is not so we need to do it ourselves
            animator = this.gameObject.GetComponent<Animator>();
            triggeredHide = false;

            // GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0) fck


        }

        public void CheckNotificationVisibility(float t) 
        {

            if (!animator) 
            {
                animator = this.gameObject.GetComponent<Animator>();
            }
            if (t >= 1f && !triggeredHide && animator) 
            {
                triggeredHide = true;
                animator.SetTrigger("Outro");
            }

            if (t > 1.5f && triggeredHide) 
            {
                Destroy(this.gameObject);
            }
        }

        public void SetText(string title, string desc, CharacterMaster master)
        {
            this.titleText.token = title;
            this.descriptionText.token = desc;

            bool iconSet = false;
            if (master) 
            {
                if (master.GetBody()) 
                {
                    if (master.GetBody().portraitIcon) 
                    {
                        iconSet = true;

                        this.iconImage.texture = master.GetBody().portraitIcon;
                    }
                }
            }

            if (!iconSet) 
            {
                this.iconImage.texture = LeeHyperrealMod.Modules.Survivors.LeeHyperreal.staticBodyPrefab.GetComponent<CharacterBody>().portraitIcon;
            }
            
            this.titleTMP.color = Modules.StaticValues.bodyColor;


            //Get Body and then skill loc
            CharacterBody body = master.GetBody();

            if (body) 
            {
                Color color = Modules.ParticleAssets.RetrieveParticleColor(body);
                portraitOutline = this.gameObject.transform.Find("LeeNotificationExtraParent/LeeNotificationExtraParent2/CharacterPortraitbackground/CharacterPortrait/CharacterPortraitOutline").gameObject.GetComponent<Image>().material;
                descriptionOutline = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>().material;
                descriptionInner = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>().material;
                portraitOutline.SetColor("_GlowColor", color);
                descriptionOutline.SetColor("_GlowColor", color);
                descriptionInner.SetColor("_glowcolor", color);
            }
        }
    }
}
