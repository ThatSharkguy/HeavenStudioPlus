using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HeavenStudio.Games.Scripts_WizardsWaltz
{
    public class Plant : MonoBehaviour
    {
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public double createBeat;

        private WizardsWaltz game;
        private bool passed = false;

        public int order = 0;

        public void Init(bool spawnedInactive) 
        {
            game = WizardsWaltz.instance;
            spriteRenderer.sortingOrder = order;
            animator.Play("Appear", 0, spawnedInactive ? 1 : 0);
        }

        public void StartInput(double beat, float length)
        {
            game.ScheduleInput(beat, length, WizardsWaltz.InputAction_Press, Just, Miss, Out);
        }

        private void Update()
        {
            if (!passed && Conductor.instance.songPositionInBeats > createBeat + game.beatInterval)
            {
                StartCoroutine(FadeOut());
                passed = true;
            }
        }

        public void Bloom()
        {
            animator.Play("Hit", 0, 0);
        }

        public void IdlePlant()
        {
            animator.Play("IdlePlant", 0, 0);
        }

        public void IdleFlower()
        {
            animator.Play("IdleFlower", 0, 0);
        }

        public void Eat()
        {
            animator.Play("Eat", 0, 0);
        }

        public void EatLoop()
        {
            animator.Play("EatLoop", 0, 0);
        }

        public void Ace()
        {
            game.wizard.Magic(this, true);
        }

        public void NearMiss()
        {
            game.wizard.Magic(this, false);
        }

        private void Just(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f) {
                NearMiss();
                return; 
            }
            Ace();
        }

        private void Miss(PlayerActionEvent caller) 
        {
            // this is where perfect challenge breaks
        }

        private void Out(PlayerActionEvent caller) {}

        public IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(Conductor.instance.secPerBeat * game.beatInterval / 2f);
            Destroy(gameObject);
            game.currentPlants.Remove(this);
        }
    }
}