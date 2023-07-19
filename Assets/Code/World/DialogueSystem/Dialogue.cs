using UnityEngine;
using Filament.Content;

namespace Wrapper
{
    public class Dialogue : ContentAsset
    {
        [SerializeField, ContentValue("SequenceID")]
        public string sequenceID;

        [SerializeField, ContentValue("Number")]
        public int num;
        
        [SerializeField, ContentValue("Text")]
        public string text;
        
        [SerializeField, ContentValue("Speaker")]
        public Character speaker;
        
        [SerializeField, ContentValue("Speaker Expression")]
        public Expression speakerExpression;

        [SerializeField, ContentValue("Listener")]
        public Character listener;

        [SerializeField, ContentValue("Listener Expression")]
        public Expression listenerExpression;

        [SerializeField, ContentValue("Context Image Path")]
        public string contextImagePath;
    }
}
