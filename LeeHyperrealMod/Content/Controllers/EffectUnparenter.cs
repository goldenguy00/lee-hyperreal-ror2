using UnityEngine;

namespace LeeHyperrealMod.Content.Controllers
{
    internal class EffectUnparenter : MonoBehaviour
    {

        public float duration;
        float stopwatch;
        bool hasUnparented;

        public void OnEnable() 
        {
            stopwatch = 0f;
            hasUnparented = false;
        }

        public void Update()
        {
            stopwatch += Time.deltaTime;

            if (stopwatch >= duration && !hasUnparented) 
            {
                //Unparent
                hasUnparented = true;
                this.gameObject.transform.SetParent(null, true);
            }
        }
    }
}
