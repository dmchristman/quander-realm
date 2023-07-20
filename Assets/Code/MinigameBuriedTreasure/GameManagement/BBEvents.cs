using System;
using UnityEngine;
using UnityEngine.Events;

namespace BlackBox
{
    public static class BBEvents
    {
        /// Debug
        public static Action ToggleDebug;
        public static Func<bool> IsDebug;
        public static Action<string> GotoLevel;
        public static Action ClearMarkers;

        /// Tutorial
        public static Action<BBSaveData, Level> ShowTutorial;

        /// Level Select
        public static Action<string> PlayLevel;
        public static Action<bool> OpenLevelSelect;
        public static Action CloseLevel;

        /// Ray and Markers
        public static Action<Vector3Int, Dir> FireRay;
        public static Action<Marker, Dir, Vector3Int, bool> MarkUnits;
        public static Action<Dir, Vector3Int, Dir, Vector3Int, int> MarkDetourUnits;
        public static Action<string, Dir, Vector3Int> ToggleLinkedHighlight;
        public static Action<Dir, Vector3Int> TestLinkHovered;
        public static Action DisableMolly;
        public static Action SendMollyIn;

        /// Interaction Delays
        public static Func<bool> IsInteractionDelayed;
        public static Action<bool> DelayInteraction;
        public static Action<Action> DelayReaction;

        /// Flags/Lanterns
        public static Action<Vector3Int, bool> ToggleFlag;
        public static Action<GameObject> ReturnLanternHome;
        public static Action<bool> ToggleLanternHeld;
        public static Func<Transform> GetFrontMount;

        /// Energy Bar
        public static Action DecrementEnergy;
        public static Action IndicateEmptyMeter;
        public static Action InitEnergyBar;
        public static Func<int> GetNumEnergyUnits;

        /// Level Submission
        public static Action CheckWinState;
        public static Action CheckWolfieReady;
        public static Action<bool> ToggleWolfieButton;
        public static Action<int> UpdateHUDWolfieLives;
        public static Action<WinState> UpdateEndPanel;
        public static Action StartNextLevel;
        public static Action RestartLevel;
        public static Action QuitBlackBox;
        public static Action CompleteBlackBox;
        public static Func<int> LanternPlacedCount;
    }
}
