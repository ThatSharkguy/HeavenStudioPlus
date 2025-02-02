using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_GardenDance
{
    public class Flower : MonoBehaviour
    {
        [SerializeField] Animator anim;
        private bool canBlink = true;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Random.Range(1, 600) == 1 && canBlink && !anim.IsPlayingAnimationNames("Blink"))
            {
                anim.DoScaledAnimationAsync("Blink", 0.5f, animLayer : 1);
            }
        }

        public void Bop()
        {
            anim.DoScaledAnimationAsync("Bop", 0.5f, animLayer: 0);
            anim.DoScaledAnimationAsync("IdleFace", 0.5f, animLayer: 1);
            canBlink = true;
        }
    }
}
