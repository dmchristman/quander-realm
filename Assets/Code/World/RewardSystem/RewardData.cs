using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wrapper
{
    public class RewardData : MonoBehaviour
    {
        public RewardSaveObject RewardSaveObj = new RewardSaveObject();

        public static RewardData Instance;

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