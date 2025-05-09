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
        
        public void Start() 
        {
            // Replace everything with the notif text and image here as the prefab before was serialized. This is not so we need to do it ourselves
            animator = this.gameObject.GetComponent<Animator>();
            triggeredHide = false;

            Transform hyperLabel = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(3);
            Destroy(hyperLabel.gameObject.GetComponent<TextMeshPro>());
            if (!this.titleText)
            {
                this.titleText = CreateLabel(hyperLabel, "Hyper Effect", 24f);
                this.titleTMP = hyperLabel.GetComponent<HGTextMeshProUGUI>();
            }

            Transform descriptionLabel = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(2);
            Destroy(descriptionLabel.gameObject.GetComponent<TextMeshPro>());
            if (!descriptionText) 
            {
                this.descriptionText = CreateLabel(descriptionLabel, "", 16f);
            }
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
        }

        private LanguageTextMeshController CreateLabel(Transform parent, string text, float textScale)
        {
            LanguageTextMeshController controller = parent.gameObject.AddComponent<LanguageTextMeshController>();
            HGTextMeshProUGUI hgtextMeshProUGUI = parent.gameObject.AddComponent<HGTextMeshProUGUI>();
            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = textScale;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = false;

            controller.textMeshPro = hgtextMeshProUGUI;
            return controller;
        }
    }
}
