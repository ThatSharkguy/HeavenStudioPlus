using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;
using HeavenStudio.InputSystem;

using Jukebox;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    /// Minigame loaders handle the setup of your minigame.
    /// Here, you designate the game prefab, define entities, and mark what AssetBundle to load

    /// Names of minigame loaders follow a specific naming convention of `PlatformcodeNameLoader`, where:
    /// `Platformcode` is a three-leter platform code with the minigame's origin
    /// `Name` is a short internal name
    /// `Loader` is the string "Loader"

    /// Platform codes are as follows:
    /// Agb: Gameboy Advance    ("Advance Gameboy")
    /// Ntr: Nintendo DS        ("Nitro")
    /// Rvl: Nintendo Wii       ("Revolution")
    /// Ctr: Nintendo 3DS       ("Centrair")
    /// Mob: Mobile
    /// Pco: PC / Other

    /// Fill in the loader class label, "*prefab name*", and "*Display Name*" with the relevant information
    /// For help, feel free to reach out to us on our discord, in the #development channel.
    public static class PcoGardenLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("gardenDance", "Garden Dance", "92cbe8", false, false, new List<GameAction>()
            {
                new GameAction("bop", "Bop")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out GardenDance instance)) {
                            instance.Bop(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["auto"], eventCaller.currentEntity["toggle"]);
                        }
                    },
                    resizable = true,
                    parameters = new List<Param>()
                    {
                        new Param("toggle", GardenDance.WhoBops.Both, "Bop", "Set the characters to bop for the duration of this event."),
                        new Param("auto", GardenDance.WhoBops.None, "Bop (Auto)", "Set the characters to automatically bop until another Bop event is reached."),
                    }
                },
                new GameAction("dance", "Dancing")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out GardenDance instance)) {
                            instance.Dance(eventCaller.currentEntity.beat, eventCaller.currentEntity.length);
                        }
                    },
                    resizable = true,
                    defaultLength = 4f
                },
                new GameAction("pose", "Pose")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out GardenDance instance)) {
                            instance.Pose(eventCaller.currentEntity.beat);
                        }
                    },
                    defaultLength = 4f
                },
                new GameAction("triplet", "Triplet Cue")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out GardenDance instance)) {
                            instance.Triplet(eventCaller.currentEntity.beat);
                        }
                    },
                    defaultLength = 4f
                },
                new GameAction("bird", "Mr. Bird")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out GardenDance instance)) {
                            instance.Bird(eventCaller.currentEntity.beat, eventCaller.currentEntity.length);
                        }
                    },
                    resizable = true,
                    defaultLength = 4f
                },
                new GameAction("sun", "Sun Animation")
                {
                    function = delegate {
                        if (eventCaller.gameManager.minigameObj.TryGetComponent(out GardenDance instance)) {
                            instance.Sun(eventCaller.currentEntity.beat, eventCaller.currentEntity.length, eventCaller.currentEntity["whichAnim"], eventCaller.currentEntity["instant"]);
                        }
                    },
                    resizable = true,
                    defaultLength = 4f,
                    parameters = new List<Param>()
                    {
                        new Param("whichAnim", GardenDance.SunAnimation.Enter, "Animation", "Chooses what animation the sun should play."),
                        new Param("instant", false, "Instant", "Toggle if the animation should be played instantly.")
                    }
                },
            }
            //new List<string>() { "pco", "keep" },
            //"pcogarden", "en",
            //new List<string>() {},
            //chronologicalSortKey: 20240530
            );
        }
    }
}

namespace HeavenStudio.Games
{
    /// This class handles the minigame logic.
    /// Minigame inherits directly from MonoBehaviour, and adds Heaven Studio specific methods to override.
    using Scripts_GardenDance;
    public class GardenDance : Minigame
    {
        [SerializeField] Flower flowerPlayer;
        [SerializeField] List<Flower> flowers = new List<Flower>();
        [SerializeField] Animator sunAnim;
        private bool flowerBop;

        private bool sunBop;
        private bool sunActive = true;
        private double sunBeat;
        private float sunLength;
        private string sunAnimation = "";

        public enum WhoBops
        {
            Flowers = 0,
            Sun = 1,
            Both = 2,
            None = 3
        }
        public enum SunAnimation
        {
            Enter = 0, 
            Exit = 1
        }
        
        private void Awake()
        {

        }

        private void Update()
        {
            if (sunAnimation != "")
            {
                float normalizedBeat = Conductor.instance.GetPositionFromBeat(sunBeat, sunLength);
                if (normalizedBeat > 1) sunAnimation = "";
                sunAnim.DoNormalizedAnimation(sunAnimation, normalizedBeat, 0);
            }
        }

        public override void OnLateBeatPulse(double beat)
        {
            if (flowerBop) DoBop(0);
            if (sunBop) DoBop(1);
        }

        public void Bop(double beat, float length, int auto, int bop)
        {
            flowerBop = auto == (int)WhoBops.Flowers || auto == (int)WhoBops.Both;
            sunBop = auto == (int)WhoBops.Sun || auto == (int)WhoBops.Both;
            for (int i = 0; i < length; i++)
            {
                BeatAction.New(this, new List<BeatAction.Action>()
                {
                    new BeatAction.Action(beat + i, delegate
                    {
                        DoBop(bop);
                    })
                });
            }
        }

        private void DoBop(int bop)
        {
            switch (bop)
            {
                case 0:
                    foreach (var flower in flowers)
                    {
                        flower.Bop();
                    }
                    flowerPlayer.Bop();
                    break;
                case 1:
                    if (sunActive) sunAnim.DoScaledAnimationAsync("Bop", 0.5f);
                    break;
                case 2:
                    foreach (var flower in flowers)
                    {
                        flower.Bop();
                    }
                    flowerPlayer.Bop();
                    if (sunActive) sunAnim.DoScaledAnimationAsync("Bop", 0.5f);
                    break;
                default:
                    break;
            }
        }

        public void Dance(double beat, float length)
        {
            for (int i = 0; i < length; i++)
            {
                ScheduleInput(beat - 1, 1 + i, InputAction_BasicPress, DanceHit, DanceMiss, null);
                foreach (var flower in flowers)
                {
                    BeatAction.New(this, new List<BeatAction.Action>()
                    {
                        new BeatAction.Action(beat + i, delegate { flower.Dance(true); }),
                        new BeatAction.Action(beat + i, delegate { SoundByte.PlayOneShotGame("gardenDance/dance"); })
                    });
                }
                
            }
        }

        private void DanceHit(PlayerActionEvent caller, float state)
        {
            if (state >= 1f || state <= -1f)
            {
                flowerPlayer.Dance(true, true);
                return;
            }
            flowerPlayer.Dance(true, false);
            SoundByte.PlayOneShotGame("gardenDance/dance");
        }

        private void DanceMiss(PlayerActionEvent caller)
        {
            
        }

        public void Pose(double beat)
        {

        }

        public void Triplet(double beat)
        {

        }

        public void Bird(double beat, float length)
        {

        }

        public void Sun(double beat, float length, int anim, bool instant)
        {
            if (!instant) 
            {
                sunBeat = beat;
                sunLength = length;
                if (anim == 0)
                {
                    sunAnimation = "Enter";
                    sunActive = true;
                }
                else
                {
                    sunAnimation = "Leave";
                    sunActive = false;
                }
            }
            else
            {
                if (anim == 0)
                {
                    sunAnim.DoScaledAnimationAsync("Idle", 0.5f);
                    sunActive = true;
                }
                else
                {
                    sunAnim.DoScaledAnimationAsync("Hide", 0.5f);
                    sunActive = false;
                }
            }
        }

    }
}