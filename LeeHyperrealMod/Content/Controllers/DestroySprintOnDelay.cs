using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LeeHyperrealMod.Content.Controllers
{
    internal class DestroySprintOnDelay : MonoBehaviour
    {
        public float delay = 1.1f;
        public bool startDestroy = false;
        public float timer = 0f;

        public Animator animator;

        public void Start() 
        {
            animator = gameObject.GetComponent<Animator>();
        }

        public void Update()
        {
            if (startDestroy)
            {
                timer += Time.deltaTime;
                if (timer >= delay) 
                {
                    TriggerDestroy();
                }
            }
        }

        public void StartDestroying() 
        {
            startDestroy = true;
            if (animator)
            {
                animator.SetTrigger("Outro");
            }
        }

        public void TriggerDestroy() 
        {
            Destroy(this.gameObject);
        }
    }
}
