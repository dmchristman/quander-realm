using UnityEngine;
using System.Collections;

namespace Qupcakery
{

    public class PatienceController : MonoBehaviour
    {
        [SerializeField]
        GameObject patienceBarFull;

        private void Start()
        {
            patienceBarFull.transform.localScale = new Vector2(1f, 1f);
        }

        public void OnPatienceUpdated(float remainingRatio)
        {
            patienceBarFull.transform.localScale = new Vector2(remainingRatio, 1f);
        }
    }
}
