using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using QueueBits;

// for research data
//		DO NOT push to firebase and mix with non-research data

public class saveData : MonoBehaviour
{
	// Use this for initialization
	public static int Save(Data mydata)
	{
		ResearchData researchData = new ResearchData();
		researchData.UpdateResearchData(mydata);

		Wrapper.Events.SaveMinigameResearchData?.Invoke(Wrapper.Game.QueueBits, researchData);
		Debug.Log("Research Data");
		Debug.Log(researchData.SaveData);
		return 0;
	}

}