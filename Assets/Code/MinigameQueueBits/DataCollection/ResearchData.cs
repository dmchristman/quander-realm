
using System;
using System.Collections.Generic;
using UnityEngine;
//research data
namespace QueueBits
{
	[System.Serializable]
	public class ResearchData
	{
		public string Username = Wrapper.Events.GetPlayerResearchCode?.Invoke();
		public string SaveData = string.Empty;

		public void UpdateResearchData(Data data)
		{
			SaveData = JsonUtility.ToJson(data);
		}
	}
}