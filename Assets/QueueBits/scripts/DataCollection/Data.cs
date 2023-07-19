
using System;
using System.Collections.Generic;
using UnityEngine;
//research data
namespace QueueBits
{
	[System.Serializable]
	public class Data
	{
		//public Wrapper.Game gameID = Wrapper.Game.QueueBits;
		public string userID;
		public int level;
		public int winner; //0 = tie 1 = player 2 = AI 
						   //maps position index to value
		public int prefilledBoard;
		public int[] placement_order; //even turns = player, odd turns = AI
		public int[] superposition;
		public int[] reveal_order; //even turns = player, odd turns = AI
		public int[] outcome; //1 = player 2 = AI
	}
}