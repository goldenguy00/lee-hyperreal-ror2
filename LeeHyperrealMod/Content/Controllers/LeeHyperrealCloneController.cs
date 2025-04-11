using RoR2;
using UnityEngine;

namespace LeeHyperrealMod.Content.Controllers
{
    internal class LeeHyperrealCloneController : MonoBehaviour
    {
        public bool triggered = false;
        public bool shouldFadeout = false;
        public string animationTrigger = "";
        public Animator animator;

        public void Start()
        {
            animator = GetComponent<Animator>();
            //Check the animator for the flicker animation
            //Choose random point in animation to play from

            animator.SetBool("Flicker", true);

            animator.Play("Flicker", animator.GetLayerIndex("Flicker"), UnityEngine.Random.Range(0f, 1.0f));
            animator.Play("FlickerMaterial", animator.GetLayerIndex("MaterialChange"), UnityEngine.Random.Range(0f, 1.0f));
        }

        public void Update() 
        {
            if (!triggered && animationTrigger != "") 
            {
                animator.SetTrigger(animationTrigger);
                triggered = true;
            }

            if (shouldFadeout) 
            {
                animator.SetBool("FadeoutFlicker", true);
                animator.SetBool("Flicker", false);
            }
        }
    }
}