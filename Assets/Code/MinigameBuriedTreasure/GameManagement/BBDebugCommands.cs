using UnityEngine;
using Wrapper;

namespace BlackBox
{
    public class BBDebugCommands : MonoBehaviour
    {
        private void OnEnable()
        {
            Events.BBGotoLevel += GotoLevel;
            Events.BBToggleDebug += ToggleDebug;
            Events.BBClearMarkers += ClearMarkers;
        }

        private void OnDisable()
        {
            Events.BBGotoLevel -= GotoLevel;
            Events.BBToggleDebug -= ToggleDebug;
            Events.BBClearMarkers -= ClearMarkers;
        }

        private void GotoLevel(string levelID)
        {
            BBEvents.GotoLevel?.Invoke(levelID);
        }

        private void ToggleDebug()
        {
            #if (UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN) && !UNITY_EDITOR
                BBEvents.ToggleDebug?.Invoke();
            #else
                Debug.LogWarning("Command only works for ios and android, use the 'D' key to toggle Debug Mode");
            #endif
        }

        private void ClearMarkers()
        {
            BBEvents.ClearMarkers?.Invoke();
        }
    }
}