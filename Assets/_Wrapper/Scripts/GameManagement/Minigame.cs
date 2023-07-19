using UnityEngine;

namespace Wrapper
{
    [CreateAssetMenu(fileName = "Minigame", menuName = "Minigame")]
    public class Minigame : ScriptableObject
    {
        public string StartScene;
        public new string name;
        public Game gameValue;
    }
}