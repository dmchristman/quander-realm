using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Labyrinth
{
    public class Data : MonoBehaviour
    {
        public TTSaveData ttSaveData;
        public ResearchData researchData;

        public static Data Instance;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}