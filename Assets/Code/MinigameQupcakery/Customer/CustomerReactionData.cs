using System;
using UnityEngine;

namespace Qupcakery
{
    [CreateAssetMenu]
    public class CustomerReactionData : ScriptableObject
    {
        public float WaitTimeAfterReceivingBox;
        public float WaitTimeAfterOpeningBox;
        public float WaitTimeAfterSpawningExpression;
    }
}
