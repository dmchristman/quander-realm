using Filament.Content;
using UnityEngine;

namespace Wrapper
{
    public class RewardAsset : ContentAsset
    {
        [SerializeField, ContentValue("Game")]
        public Game game;

        [SerializeField, ContentValue("Level")]
        public int level;

        [SerializeField, ContentValue("Reward ID")]
        public string rewardID;

        [SerializeField, ContentValue("Card Type")]
        public CardType cardType;

        [SerializeField, ContentValue("Sticker Title")]
        public string title;

        [SerializeField, ContentValue("Back Text")]
        public string backText;

        [SerializeField, ContentValue("Front Flavor Text")]
        public string flavorText;

        [SerializeField, ContentValue("Front Image Path")]
        public string imagePath;
    }
}
