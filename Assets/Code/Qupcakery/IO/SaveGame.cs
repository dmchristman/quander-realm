using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/*
 * Saves a game to database
 */
namespace Qupcakery
{
    public class SaveGame : MonoBehaviour
    {
        public static SaveGame Instance;

        void Awake()
        {
            Instance = this;
        }

        public int Save()
        {
            // Save research data
            Data.researchData.UpdateResearchData(GameManagement.Instance.game.gameStat);
            Wrapper.Events.SaveMinigameResearchData?.Invoke(Wrapper.Game.Qupcakes,
                Data.researchData);

            // Save game data
            Data.gameData.UpdateGameData(GameManagement.Instance.game.gameStat);
            Wrapper.Events.UpdateMinigameSaveData?.Invoke(Wrapper.Game.Qupcakes,
                Data.gameData);
            return 0;
        }
    }
}
