using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Threading;

namespace QueueBits
{
	public class Level14 : MonoBehaviour
	{
		enum Piece
		{
			Empty = 0,
			Blue = 1,
			Red = 2,
			Unknown = 3
		}

		[Range(3, 8)]
		public int numRows = 6;
		[Range(3, 8)]
		public int numColumns = 7;

		[Tooltip("How many pieces have to be connected to win.")]
		public int numPiecesToWin = 4;

		[Tooltip("Allow diagonally connected Pieces?")]
		public bool allowDiagonally = true;

		public float dropTime = 4f;

		// star system
		public bool starUpdated = false;
		public GameObject starFilled;
		public GameObject starEmpty;
		public GameObject Star1;
		public GameObject Star2;
		public GameObject Star3;

		// Gameobjects 
		public GameObject pieceRed;
		public GameObject pieceBlue;
		public GameObject pieceField;
		public GameObject finalColor;

		// Superposition Pieces
		public GameObject piece25;
		public GameObject piece50;
		public GameObject piece75;
		public GameObject piece25red_turn;
		public GameObject piece50red_turn;
		public GameObject piece75red_turn;
		public GameObject pieceSuperposition;

		//Piece Count Displays
		// public GameObject blueTitle;
		// public GameObject redTitle;

		//BLUE
		public GameObject pieceBlue100;
		public GameObject pieceBlue75;
		public GameObject pieceBlue50;
		public GameObject pieceBlue25;

		public int pieceBlue100Count = 7;
		public int pieceBlue75Count = 4;
		public int pieceBlue50Count = 5;
		public int pieceBlue25Count = 5;

		public GameObject pieceBlue100Text;
		public GameObject pieceBlue75Text;
		public GameObject pieceBlue50Text;
		public GameObject pieceBlue25Text;

		public GameObject pieceCounterText;

		//RED
		public GameObject pieceRed100;
		public GameObject pieceRed75;
		public GameObject pieceRed50;
		public GameObject pieceRed25;

		public int pieceRed100Count = 7;
		public int pieceRed75Count = 4;
		public int pieceRed50Count = 5;
		public int pieceRed25Count = 5;

		public GameObject pieceRed100Text;
		public GameObject pieceRed75Text;
		public GameObject pieceRed50Text;
		public GameObject pieceRed25Text;

		public int probability;

		Dictionary<int, (int, (int, int), (float, float), GameObject)> probDict = new Dictionary<int, (int, (int, int), (float, float), GameObject)>();

		Dictionary<int, int> redProbs = new Dictionary<int, int>();
		Dictionary<int, int> blueProbs = new Dictionary<int, int>();

		//int[] redProbs = { 25, 25, 25, 25, 25, 50, 50, 50, 50, 50, 75, 75, 75, 75, 100, 100, 100, 100, 100, 100, 100 };
		//int[] blueProbs = { 25, 25, 25, 25, 25, 50, 50, 50, 50, 50, 75, 75, 75, 75, 100, 100, 100, 100, 100, 100, 100 };

		// define prefilled board
		List<(Piece, int, int, int)> prefilledBoard = new List<(Piece piece, int col, int row, int prob)>();

		Dictionary<int, List<(Piece, int, int, int)>> prefilledBoardList = new Dictionary<int, List<(Piece, int, int, int)>>
		{
			{0, new List<(Piece piece, int col, int row, int prob)> //7.9
				{
					(Piece.Red, 3, 5, 50),
				(Piece.Blue, 2, 5, 75),
				(Piece.Red, 2, 4, 75),
				(Piece.Blue, 5, 5, 75),
				(Piece.Red, 4, 5, 50),
				(Piece.Blue, 3, 4, 50),
				(Piece.Red, 3, 3, 100),
				(Piece.Blue, 4, 4, 50),
				(Piece.Red, 0, 5, 100),
				(Piece.Blue, 3, 2, 100),
				(Piece.Red, 2, 3, 100),
				(Piece.Blue, 2, 2, 100),
				(Piece.Red, 4, 3, 75),
				(Piece.Blue, 0, 4, 100)
				}
			},
			{1, new List<(Piece piece, int col, int row, int prob)> //7.10
				{
					(Piece.Red, 3, 5, 100),
				(Piece.Blue, 3, 4, 100),
				(Piece.Red, 3, 3, 75),
				(Piece.Blue, 2, 5, 100),
				(Piece.Red, 2, 4, 100),
				(Piece.Blue, 4, 5, 100),
				(Piece.Red, 2, 3, 50),
				(Piece.Blue, 3, 2, 75),
				(Piece.Red, 3, 1, 100),
				(Piece.Blue, 4, 4, 75),
				(Piece.Red, 4, 3, 75),
				(Piece.Blue, 4, 2, 50),
				(Piece.Red, 5, 5, 75),
				(Piece.Blue, 2, 2, 75)
				}
			},
			{2, new List<(Piece piece, int col, int row, int prob)> //7.11
				{
				(Piece.Red, 3, 5, 100),
				(Piece.Blue, 3, 4, 100),
				(Piece.Red, 3, 3, 75),
				(Piece.Blue, 3, 2, 75),
				(Piece.Red, 3, 1, 100),
				(Piece.Blue, 4, 5, 50),
				(Piece.Red, 4, 4, 50),
				(Piece.Blue, 2, 5, 100),
				(Piece.Red, 4, 3, 50),
				(Piece.Blue, 5, 5, 75),
				(Piece.Red, 5, 4, 100),
				(Piece.Blue, 2, 4, 50),
				(Piece.Red, 0, 5, 75),
				(Piece.Blue, 0, 4, 100)
				}
			},
			{3, new List<(Piece piece, int col, int row, int prob)> //7.12
				{
					(Piece.Red, 3, 5, 100),
				(Piece.Blue, 3, 4, 75),
				(Piece.Red, 4, 5, 100),
				(Piece.Blue, 2, 5, 100),
				(Piece.Red, 3, 3, 50),
				(Piece.Blue, 4, 4, 50),
				(Piece.Red, 2, 4, 75),
				(Piece.Blue, 1, 5, 50),
				(Piece.Red, 4, 3, 50),
				(Piece.Blue, 2, 3, 100),
				(Piece.Red, 3, 2, 100),
				(Piece.Blue, 4, 2, 75),
				(Piece.Red, 2, 2, 75),
				(Piece.Blue, 3, 1, 100)
				}
			},
			{4, new List<(Piece piece, int col, int row, int prob)> //8.5
				{
					(Piece.Red, 3, 5, 75),
				(Piece.Blue, 4, 5, 75),
				(Piece.Red, 2, 5, 100),
				(Piece.Blue, 3, 4, 100),
				(Piece.Red, 2, 4, 100),
				(Piece.Blue, 3, 3, 50),
				(Piece.Red, 1, 5, 100),
				(Piece.Blue, 0, 5, 50),
				(Piece.Red, 3, 2, 75),
				(Piece.Blue, 2, 3, 75),
				(Piece.Red, 2, 2, 50),
				(Piece.Blue, 0, 4, 100),
				(Piece.Red, 4, 4, 50),
				(Piece.Blue, 3, 1, 100)
				}
			},
			{5, new List<(Piece piece, int col, int row, int prob)> //8.6
				{
					(Piece.Red, 4, 5, 50),
					(Piece.Blue, 1, 5, 100),
					(Piece.Red, 3, 5, 100),
					(Piece.Blue, 5, 5, 100),
					(Piece.Red, 1, 4, 75),
					(Piece.Blue, 4, 4, 50),
					(Piece.Red, 5, 4, 75),
					(Piece.Blue, 4, 3, 75),
					(Piece.Red, 4, 2, 75),
					(Piece.Blue, 3, 4, 75),
					(Piece.Red, 3, 3, 100),
					(Piece.Blue, 3, 2, 75),
					(Piece.Red, 1, 3, 50),
					(Piece.Blue, 3, 1, 50)
				}
			},
			{6, new List<(Piece piece, int col, int row, int prob)> //8.7
				{
					(Piece.Red, 2, 5, 75),
				(Piece.Blue, 3, 5, 50),
				(Piece.Red, 3, 4, 50),
				(Piece.Blue, 3, 3, 100),
				(Piece.Red, 3, 2, 75),
				(Piece.Blue, 5, 5, 100),
				(Piece.Red, 1, 5, 100),
				(Piece.Blue, 3, 1, 75),
				(Piece.Red, 6, 5, 50),
				(Piece.Blue, 5, 4, 75),
				(Piece.Red, 1, 4, 75),
				(Piece.Blue, 0, 5, 75),
				(Piece.Red, 2, 4, 100),
				(Piece.Blue, 0, 4, 50)
				}
			},
			{7, new List<(Piece piece, int col, int row, int prob)> //8.8
				{
					(Piece.Red, 3, 5, 75),
				(Piece.Blue, 5, 5, 50),
				(Piece.Red, 2, 5, 50),
				(Piece.Blue, 1, 5, 75),
				(Piece.Red, 5, 4, 50),
				(Piece.Blue, 3, 4, 100),
				(Piece.Red, 3, 3, 100),
				(Piece.Blue, 3, 2, 75),
				(Piece.Red, 2, 4, 100),
				(Piece.Blue, 2, 3, 50),
				(Piece.Red, 1, 4, 75),
				(Piece.Blue, 1, 3, 75),
				(Piece.Red, 5, 3, 75),
				(Piece.Blue, 3, 1, 100)
				}
			}
		};

		// Select Tokens
		GameObject SelectTokenText;
		GameObject choice50;
		GameObject choice50text;
		GameObject choice75;
		GameObject choice75text;
		GameObject choice100;
		GameObject choice100text;
		bool SelectMenuGenerated = false;
		int choice;

		public GameObject winningText;

		public GameObject resultBG;
		public bool resultBGshown = false;
		public string playerWonText = "You Won!";
		public string playerLoseText = "Red Won!";
		public string drawText = "Draw!";

		public GameObject probText;
		public GameObject starText;

		// public GameObject btnPlayAgain;
		// bool btnPlayAgainTouching = false;
		// Color btnPlayAgainOrigColor;
		// Color btnPlayAgainHoverColor = new Color(255, 143, 4);

		GameObject gameObjectField;

		// temporary gameobject, holds the piece at mouse position until the mouse has clicked
		GameObject gameObjectTurn;

		/// <summary>
		/// The Game field.
		/// 0 = Empty
		/// 1 = Blue
		/// 2 = Red
		/// </summary>
		int[,] field;
		int[,] probField;
		(int, int)[] dropOrder = new (int, int)[42];
		GameObject[] pieces = new GameObject[42];

		int probCounter = 0;
		int numSuperpositionPieces;
		bool revealingProbs = false;
		bool choosingReveal = false;
		bool revealAuto = false;
		bool revealManual = false;

		public GameObject playerTurnText;
		public GameObject playerTurnObject;

		bool isPlayersTurn = true;
		bool isLoading = true;
		bool isDropping = false;
		bool mouseButtonPressed = false;

		bool gameOver = false;
		bool isCheckingForWinner = false;

		// dialogue
		bool dialoguePhase = false;

		//Shivani Puli Data Collection
		Data mydata = new Data();
		int turn = 0;
		int revealturn = 0;

		string state = "000000000000000000000000000000000000000000";
		int[] colPointers = { 5, 5, 5, 5, 5, 5, 5 };
		HashSet<(int, int)> visited = new HashSet<(int, int)>();


		// Use this for initialization
		void Start()
		{
			ShowStarSystem();

			// dialogue
			if (GameManager.saveData.dialogueSystem[14])
			{
				dialoguePhase = true;
				Wrapper.Events.StartDialogueSequence?.Invoke("QB_Level14");
				GameManager.saveData.dialogueSystem[14] = false;
				GameManager.Save();
				Wrapper.Events.DialogueSequenceEnded += updateDialoguePhase;
			}

			int board_num = Random.Range(0, prefilledBoardList.Keys.Count);
			prefilledBoard = prefilledBoardList[board_num];

			//Shivani Puli Data Collection
			mydata.level = 14;
			mydata.userID = Wrapper.Events.GetPlayerResearchCode?.Invoke();
			mydata.prefilledBoard = board_num;
			mydata.placement_order = new int[numColumns * numRows];
			mydata.superposition = new int[numColumns * numRows];
			mydata.reveal_order = new int[numColumns * numRows];
			mydata.outcome = new int[numColumns * numRows];
			for (int i = 0; i < numColumns * numRows; i++)
			{
				mydata.placement_order[i] = 0;
				mydata.superposition[i] = 0;
				mydata.reveal_order[i] = 0;
				mydata.outcome[i] = 0;
			}

			foreach ((Piece pi, int c, int r, int pr) in prefilledBoard)
			{
				turn++;
				int index = r * numColumns + c;
				mydata.placement_order[index] = turn;
				mydata.superposition[index] = pr;
				if (pr == 100)
					if (pi == Piece.Blue)//if Yellow
					{
						playMove(c, "1");
						if (pr == 100)
							mydata.outcome[index] = 1;
					}
					else
					{
						playMove(c, "2");
						if (pr == 100)
							mydata.outcome[index] = 2;
					}
			}
			//Data collection

			redProbs.Add(50, 6);
			redProbs.Add(75, 4);
			redProbs.Add(100, 4);

			blueProbs.Add(50, 6);
			blueProbs.Add(75, 4);
			blueProbs.Add(100, 4);

			int max = Mathf.Max(numRows, numColumns);

			if (numPiecesToWin > max)
				numPiecesToWin = max;

			CreateField();

			GameObject levelText = Instantiate(probText, new Vector3(numColumns - 5f, -7f, -1), Quaternion.identity) as GameObject;
			levelText.GetComponent<TextMesh>().text = "Level 14";
			levelText.SetActive(true);

			isPlayersTurn = false;
			//isPlayersTurn = System.Convert.ToBoolean(Random.Range(0, 2));
			playerTurnText.GetComponent<TextMesh>().text = isPlayersTurn ? "Your Turn" : "Red's Turn";


			if (isPlayersTurn)
			{
				playerTurnObject = Instantiate(pieceBlue, new Vector3(numColumns - 1.75f, -6.3f, -1), Quaternion.identity) as GameObject;
				playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
			}
			else
			{
				playerTurnObject = Instantiate(pieceRed, new Vector3(numColumns - 2.25f, -6.3f, -1), Quaternion.identity) as GameObject;
				playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
			}

			// btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
		}

		// dialogue
		void updateDialoguePhase()
		{
			dialoguePhase = false;
			Wrapper.Events.DialogueSequenceEnded -= updateDialoguePhase;
		}

		/// <summary>
		/// Creates the field.
		/// </summary>
		void CreateField()
		{

			winningText.SetActive(false);
			// btnPlayAgain.SetActive(false);

			playerTurnText.SetActive(true);
			playerTurnText.GetComponent<Renderer>().sortingOrder = 4;

			isLoading = true;

			gameObjectField = GameObject.Find("Field");
			if (gameObjectField != null)
			{
				DestroyImmediate(gameObjectField);
			}
			gameObjectField = new GameObject("Field");

			// create an empty field and instantiate the cells
			field = new int[numColumns, numRows];
			probField = new int[numColumns, numRows];

			// initialize the wood board image
			GameObject g = Instantiate(pieceField, new Vector3(3, -2.8f, 1), Quaternion.identity) as GameObject;
			g.transform.parent = gameObjectField.transform;

			// initialize field for pieces
			for (int x = 0; x < numColumns; x++)
			{
				for (int y = 0; y < numRows; y++)
				{
					field[x, y] = (int)Piece.Empty;
					probField[x, y] = -1;
				}
			}

			// initialize prefilled board
			for (int i = 0; i < prefilledBoard.Count; i++)
			{
				probCounter++;
				if (prefilledBoard[i].Item4 == 100)
				{
					field[prefilledBoard[i].Item2, prefilledBoard[i].Item3] = (int)prefilledBoard[i].Item1;
					if (prefilledBoard[i].Item1 == Piece.Blue)
					{
						GameObject obj = Instantiate(pieceBlue, new Vector3(prefilledBoard[i].Item2, -prefilledBoard[i].Item3, 0), Quaternion.identity) as GameObject;
					}
					else
					{
						GameObject obj = Instantiate(pieceRed, new Vector3(prefilledBoard[i].Item2, -prefilledBoard[i].Item3, 0), Quaternion.identity) as GameObject;
					}
				}
				else
				{
					field[prefilledBoard[i].Item2, prefilledBoard[i].Item3] = (int)Piece.Unknown;
					GameObject obj;
					if (prefilledBoard[i].Item1 == Piece.Blue)
					{
						probField[prefilledBoard[i].Item2, prefilledBoard[i].Item3] = prefilledBoard[i].Item4;
						if (prefilledBoard[i].Item4 == 75)
						{
							obj = Instantiate(piece75, new Vector3(prefilledBoard[i].Item2, -prefilledBoard[i].Item3, 0), Quaternion.identity) as GameObject;
						}
						else
						{
							obj = Instantiate(piece50, new Vector3(prefilledBoard[i].Item2, -prefilledBoard[i].Item3, 0), Quaternion.identity) as GameObject;
						}
					}
					else
					{
						probField[prefilledBoard[i].Item2, prefilledBoard[i].Item3] = 100 - prefilledBoard[i].Item4;
						if (prefilledBoard[i].Item4 == 75)
						{
							obj = Instantiate(piece25red_turn, new Vector3(prefilledBoard[i].Item2, -prefilledBoard[i].Item3, 0), Quaternion.identity) as GameObject;
						}
						else
						{
							obj = Instantiate(piece50red_turn, new Vector3(prefilledBoard[i].Item2, -prefilledBoard[i].Item3, 0), Quaternion.identity) as GameObject;
						}
					}
					(int, int) tempLocation = (prefilledBoard[i].Item2, prefilledBoard[i].Item3);
					(float, float) piecePos = (obj.transform.position.x, obj.transform.position.y);
					probDict.Add(obj.transform.GetInstanceID(), (probability, tempLocation, piecePos, obj));

					Color c = obj.GetComponent<MeshRenderer>().material.color;
					c.a = 0.5f;
					obj.GetComponent<MeshRenderer>().material.color = c;

					dropOrder[numSuperpositionPieces] = (prefilledBoard[i].Item2, prefilledBoard[i].Item3);
					pieces[numSuperpositionPieces] = obj;
					numSuperpositionPieces++;
				}
			}

			isLoading = false;
			gameOver = false;

			// center camera
			Camera.main.transform.position = new Vector3(
				(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f), Camera.main.transform.position.z);

			winningText.transform.position = new Vector3(
				(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) + 0.2f, winningText.transform.position.z);

			//btnPlayAgain.transform.position = new Vector3(
			//	(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnPlayAgain.transform.position.z);

			playerTurnText.transform.position = new Vector3(
				(numColumns - 1) / 2.0f, -6.3f, playerTurnText.transform.position.z);

			//Piece Count Displays
			// blueTitle.transform.position = new Vector3(-4, 0, 0);
			// blueTitle.GetComponent<Renderer>().sortingOrder = 10;
			// blueTitle.SetActive(true);

			// redTitle.transform.position = new Vector3(7.75f, 0, 0);
			// redTitle.GetComponent<Renderer>().sortingOrder = 10;
			// redTitle.SetActive(true);

			pieceBlue100 = Instantiate(pieceBlue, new Vector3(-2, -1, -1), Quaternion.identity) as GameObject;
			pieceBlue100.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceBlue75 = Instantiate(piece75, new Vector3(-2, -2, -1), Quaternion.identity) as GameObject;
			pieceBlue75.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceBlue50 = Instantiate(piece50, new Vector3(-2, -3, -1), Quaternion.identity) as GameObject;
			pieceBlue50.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			//Piece Count Texts - BLUE
			pieceBlue100Text = Instantiate(pieceCounterText, new Vector3(-2.75f, -1, -1), Quaternion.identity) as GameObject;
			pieceBlue100Text.GetComponent<TextMesh>().text = blueProbs[100].ToString();
			pieceBlue100Text.SetActive(true);

			pieceBlue75Text = Instantiate(pieceCounterText, new Vector3(-2.75f, -2, -1), Quaternion.identity) as GameObject;
			pieceBlue75Text.GetComponent<TextMesh>().text = blueProbs[75].ToString();
			pieceBlue75Text.SetActive(true);

			pieceBlue50Text = Instantiate(pieceCounterText, new Vector3(-2.75f, -3, -1), Quaternion.identity) as GameObject;
			pieceBlue50Text.GetComponent<TextMesh>().text = blueProbs[50].ToString();
			pieceBlue50Text.SetActive(true);

			//Piece Count Displays - RED
			pieceRed100 = Instantiate(pieceRed, new Vector3(8, -1, -1), Quaternion.identity) as GameObject;
			pieceRed100.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceRed75 = Instantiate(piece25red_turn, new Vector3(8, -2, -1), Quaternion.identity) as GameObject;
			pieceRed75.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceRed50 = Instantiate(piece50red_turn, new Vector3(8, -3, -1), Quaternion.identity) as GameObject;
			pieceRed50.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			//Piece Count Texts - RED
			pieceRed100Text = Instantiate(pieceCounterText, new Vector3(8.5f, -1, -1), Quaternion.identity) as GameObject;
			pieceRed100Text.GetComponent<TextMesh>().text = redProbs[100].ToString();
			pieceRed100Text.SetActive(true);

			pieceRed75Text = Instantiate(pieceCounterText, new Vector3(8.5f, -2, -1), Quaternion.identity) as GameObject;
			pieceRed75Text.GetComponent<TextMesh>().text = redProbs[75].ToString();
			pieceRed75Text.SetActive(true);

			pieceRed50Text = Instantiate(pieceCounterText, new Vector3(8.5f, -3, -1), Quaternion.identity) as GameObject;
			pieceRed50Text.GetComponent<TextMesh>().text = redProbs[50].ToString();
			pieceRed50Text.SetActive(true);

			pieceCounterText.SetActive(false);
		}

		int index(int r, int c)
		{
			int i = r * 7 + c;
			return i;
		}

		(int, int) reverse_index(int i)
		{
			int r = i / 7;
			int c = i % 7;
			return (r, c);
		}

		int evaluateState()
		{
			int score = 0;
			visited.Clear();

			for (int r = 0; r < 6; r++)
			{
				for (int c = 0; c < 7; c++)
				{
					int i = index(r, c);
					if (state.Substring(i, 1).Equals("2")) // && !visited.Contains((r,c)) -- adding this causes missed connections, but speed?
					{
						score += evaluatePosition(r, c, "2");
					}
					else if (state.Substring(i, 1).Equals("1"))
					{
						score -= evaluatePosition(r, c, "1");
					}
				}
			}
			return score;
		}

		int evaluatePosition(int r, int c, string color)
		{
			int i = index(r, c);
			(int, int) pos;

			int[] num_connected = new int[4];
			int hasCenter = 0;

			// look right
			int r_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				pos = reverse_index(i);
				visited.Add(pos);

				(int row, int col) = pos;
				if (col >= 2 && col <= 4)
					hasCenter = 1;

				r_counter++;
				i++;
				if ((i % 7) == 0)
					i = state.Length;
			}
			r_counter = Mathf.Min(4, r_counter); // if 5 or more connected, goes to 4.
			num_connected[r_counter - 1]++;

			// look down
			i = index(r, c);
			int d_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				pos = reverse_index(i);
				visited.Add(pos);

				(int row, int col) = pos;
				if (col >= 2 && col <= 4)
					hasCenter = 1;

				d_counter++;
				i += 7;
			}
			d_counter = Mathf.Min(4, d_counter);
			num_connected[d_counter - 1]++;

			// look diagonal right-down
			i = index(r, c);
			int rd_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				pos = reverse_index(i);
				visited.Add(pos);

				(int row, int col) = pos;
				if (col >= 2 && col <= 4)
					hasCenter = 1;

				rd_counter++;
				i += 8;
				if ((i % 7) == 0)
					i = state.Length;
			}
			rd_counter = Mathf.Min(4, rd_counter);
			num_connected[rd_counter - 1]++;

			// look diagonal left-down
			i = index(r, c);
			int ld_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				pos = reverse_index(i);
				visited.Add(pos);

				(int row, int col) = pos;
				if (col >= 2 && col <= 4)
					hasCenter = 1;

				ld_counter++;
				i += 6;
				if ((i % 7) == 6)
					i = state.Length;
			}
			ld_counter = Mathf.Min(4, ld_counter);
			num_connected[ld_counter - 1]++;

			int score = 100 * num_connected[3] + 20 * num_connected[2] + 3 * num_connected[1] + 10 * hasCenter;
			return score;
		}

		bool isWin(int r, int c, string color)
		{
			int i = index(r, c);
			// look right
			int r_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				r_counter++;
				i++;
				if ((i % 7) == 0)
					i = state.Length;
				if (color == "1" && i < state.Length && mydata.superposition[i] != 100)
					i = state.Length;
			}

			i = index(r, c);
			while (i >= 0 && state.Substring(i, 1).Equals(color))
			{
				r_counter++;
				i--;
				if ((i % 7) == 6)
					i = -1;
				if (color == "1" && i >= 0 && mydata.superposition[i] != 100)
					i = -1;
			}
			r_counter--; // center val counted twice
			if (r_counter >= 4)
				return true;

			// look down
			i = index(r, c);
			int d_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				d_counter++;
				i += 7;
				if (color == "1" && i < state.Length && mydata.superposition[i] != 100)
				{
					i = state.Length;
				}
			}
			//no need to look up for vertical case
			if (d_counter >= 4)
				return true;

			// look diagonal right-down
			i = index(r, c);
			int rd_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				rd_counter++;
				i += 8;
				if ((i % 7) == 0)
					i = state.Length;
				if (color == "1" && i < state.Length && mydata.superposition[i] != 100)
					i = state.Length;
			}
			i = index(r, c);
			while (i >= 0 && state.Substring(i, 1).Equals(color))
			{
				rd_counter++;
				i -= 8;
				if ((i % 7) == 6)
					i = -1;
				if (color == "1" && i >= 0 && mydata.superposition[i] != 100)
					i = -1;
			}
			rd_counter--;
			if (rd_counter >= 4)
				return true;

			// look diagonal left-down
			i = index(r, c);
			int ld_counter = 0;
			while (i < state.Length && state.Substring(i, 1).Equals(color))
			{
				ld_counter++;
				i += 6;
				if ((i % 7) == 6)
					i = state.Length;
				if (color == "1" && i < state.Length && mydata.superposition[i] != 100)
					i = state.Length;
			}
			i = index(r, c);
			while (i >= 0 && state.Substring(i, 1).Equals(color))
			{
				ld_counter++;
				i -= 6;
				if ((i % 7) == 0)
					i = -1;
				if (color == "1" && i >= 0 && mydata.superposition[i] != 100)
					i = -1;
			}
			ld_counter--;
			if (ld_counter >= 4)
				return true;

			return false;
		}

		List<int> getMoves(int[] cols)
		{
			List<int> possCols = new List<int>();
			for (int i = 0; i < cols.Length; i++)
			{
				if (cols[i] != -1)
					possCols.Add(i);
			}
			return possCols;
		}

		void playMove(int column, string color)
		{
			int r = colPointers[column];
			colPointers[column] -= 1;

			int i = index(r, column);

			if (i == 0)
				state = color + state.Substring(1); //prevents substring with length 0 error
			else
				state = state.Substring(0, i) + color + state.Substring(i + 1);
		}

		void reverseMove(int column)
		{
			colPointers[column] += 1;
			int r = colPointers[column];

			int i = index(r, column);

			if (i == 0)
				state = "0" + state.Substring(1); //prevents substring with length 0 error
			else
			{
				state = state.Substring(0, i) + "0" + state.Substring(i + 1);
			}

		}

		int findBestMove(int[] cols)
		{
			int bestVal = int.MinValue;
			int bestMove = -1;

			List<int> moves = getMoves(cols);

			//check for all 100% yellow win
			foreach (int column in moves)
			{
				playMove(column, "1");
				if (isWin(colPointers[column] + 1, column, "1"))
				{
					reverseMove(column);
					return column;
				}
				reverseMove(column);
			}

			foreach (int column in moves)
			{
				playMove(column, "2");
				if(isWin(colPointers[column]+1,column,"2"))
                {
					reverseMove(column);
					return column;
                }
				int value = minimax(0, 3, false);
				if (value > bestVal)
				{
					bestVal = value;
					bestMove = column;
				}
				reverseMove(column);
			}
			return bestMove;
		}

		int minimax(int depth, int maxDepth, bool isMaximizing)
		{
			List<int> moves = getMoves(colPointers);

			if (moves.Count == 0 || depth == maxDepth)
				return evaluateState();

			if (isMaximizing)
			{
				int bestVal = int.MinValue;
				foreach (int column in moves)
				{
					playMove(column, "2");
					int value = minimax(depth + 1, maxDepth, !isMaximizing);
					bestVal = Mathf.Max(bestVal, value);
					reverseMove(column);
				}
				return bestVal;
			}

			else
			{
				int bestVal = int.MaxValue;
				foreach (int column in moves)
				{
					playMove(column, "1");
					int value = minimax(depth + 1, maxDepth, !isMaximizing);
					bestVal = Mathf.Min(bestVal, value);
					reverseMove(column);
				}
				return bestVal;
			}

		}

		/// <summary>
		/// Gets all the possible moves.
		/// </summary>
		/// <returns>The possible moves.</returns>
		public List<int> GetPossibleMoves()
		{
			List<int> possibleMoves = new List<int>();
			for (int x = 0; x < numColumns; x++)
			{
				if (field[x, 0] == 0)
				{
					possibleMoves.Add(x);
				}
			}
			return possibleMoves;
		}

		/// <summary>
		/// Spawns a piece at mouse position above the first row
		/// </summary>
		/// <returns>The piece.</returns>
		(GameObject, int, GameObject) SpawnPiece()
		{
			// PROBABILITY
			Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int prob = 0;

			if (isPlayersTurn)
			{
				prob = choice;
				int freq = blueProbs[prob];

				// delete probability from player's list
				blueProbs[prob] -= 1;

				if (prob == 100)
				{
					pieceSuperposition = pieceBlue;
					pieceBlue100Text.GetComponent<TextMesh>().text = blueProbs[100].ToString();
				}
				else if (prob == 75)
				{
					pieceSuperposition = piece75;
					pieceBlue75Text.GetComponent<TextMesh>().text = blueProbs[75].ToString();
				}
				else if (prob == 50)
				{
					pieceSuperposition = piece50;
					pieceBlue50Text.GetComponent<TextMesh>().text = blueProbs[50].ToString();
				}
				else
				{
					pieceSuperposition = piece25;
					pieceBlue25Text.GetComponent<TextMesh>().text = blueProbs[25].ToString();
				}

				if (blueProbs[prob] == 0)
				{
					blueProbs.Remove(prob);
				}

				probText.GetComponent<TextMesh>().text = prob.ToString() + "% YELLOW";
				// probText.GetComponent<Renderer>().sortingOrder = 11;
			}
			else
			{
				int i = Random.Range(0, redProbs.Keys.Count);
				List<int> keyList = new List<int>(redProbs.Keys);
				prob = keyList[i];
				int freq = redProbs[prob];

				// delete probability from player's list
				redProbs[prob] -= 1;

				if (prob == 100)
				{
					pieceSuperposition = pieceRed;
					pieceRed100Text.GetComponent<TextMesh>().text = redProbs[100].ToString();
				}
				else if (prob == 75)
				{
					pieceSuperposition = piece25red_turn;
					pieceRed75Text.GetComponent<TextMesh>().text = redProbs[75].ToString();
				}
				else if (prob == 50)
				{
					pieceSuperposition = piece50red_turn;
					pieceRed50Text.GetComponent<TextMesh>().text = redProbs[50].ToString();
				}
				else
				{
					pieceSuperposition = piece75red_turn;
					pieceRed25Text.GetComponent<TextMesh>().text = redProbs[25].ToString();
				}

				if (redProbs[prob] == 0)
				{
					redProbs.Remove(prob);
				}

				probText.GetComponent<TextMesh>().text = prob.ToString() + "% RED";
				// probText.GetComponent<Renderer>().sortingOrder = 11;
			}

			List<int> moves = GetPossibleMoves();

			for (int i = 0; i < numColumns; i++)
			{
				for (int j = 0; j < numRows; j++)
				{
					if (field[i, j] != 0)
					{
						colPointers[i] = j - 1;
						break;
					}
				}
			}

			if (moves.Count > 0)
			{
				int column = findBestMove(colPointers);
				spawnPos = new Vector3(column, 0, 0);
			}

			GameObject g = Instantiate(pieceSuperposition,
					new Vector3(
					Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
					gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
					Quaternion.identity) as GameObject;

			probText.transform.position = new Vector3(1.5f, 2, -1);
			// probText.transform.parent = g.transform;

			probText.SetActive(true);

			return (g, prob, probText);
		}

		// Update is called once per frame
		void Update()
		{
			if (isLoading)
				return;

			// if (dialoguePhase)
			// 	return;

			if (revealingProbs)
			{
				revealAuto = false;
				revealProbabilitiesThroughClick();
				return;
			}

			if (isCheckingForWinner)
				return;

			if (gameOver)
			{
	// 			winningText.SetActive(true);
	// 			btnPlayAgain.SetActive(false);

	// 			// fix play again button
	// 			btnPlayAgain.transform.position = new Vector3(
	// (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
	// 			btnPlayAgain.GetComponent<TextMesh>().color = Color.white;
	// 			btnPlayAgain.GetComponent<TextMesh>().text = "EXIT TO MENU";
	// 			btnPlayAgain.GetComponent<TextMesh>().fontSize = 70;
				// UpdatePlayAgainButton();

				playerTurnText.SetActive(false);
				playerTurnObject.SetActive(false);

				return;
			}

			// UpdatePlayAgainButton();

			if (isPlayersTurn)
			{
				if (gameObjectTurn == null)
				{
					if (!SelectMenuGenerated)
					{
						SelectTokenText = Instantiate(pieceCounterText, new Vector3(1.5f, 2, -1), Quaternion.identity) as GameObject;
						SelectTokenText.GetComponent<TextMesh>().text = "SELECT TOKEN";
						SelectTokenText.GetComponent<TextMesh>().color = Color.white;
						SelectTokenText.SetActive(true);

						if (blueProbs.ContainsKey(100) && blueProbs[100] > 0)
						{
							choice100 = Instantiate(pieceBlue, new Vector3(-1f, 1, -1), Quaternion.identity) as GameObject;
							choice100text = Instantiate(probText, new Vector3(-0.4f, 1, -1), Quaternion.identity) as GameObject;
							choice100text.GetComponent<TextMesh>().text = "100% YELLOW";
							choice100text.transform.localScale = new Vector3(0.04f, 0.04f, 0.05f);
							choice100text.SetActive(true);
						}
						if (blueProbs.ContainsKey(75) && blueProbs[75] > 0)
						{
							choice75 = Instantiate(piece75, new Vector3(3, 1, -1), Quaternion.identity) as GameObject;
							choice75text = Instantiate(probText, new Vector3(3.5f, 1, -1), Quaternion.identity) as GameObject;
							choice75text.GetComponent<TextMesh>().text = "75% YELLOW";
							choice75text.transform.localScale = new Vector3(0.04f, 0.04f, 0.05f);
							choice75text.SetActive(true);
						}
						if (blueProbs.ContainsKey(50) && blueProbs[50] > 0)
						{
							choice50 = Instantiate(piece50, new Vector3(6.6f, 1, -1), Quaternion.identity) as GameObject;
							choice50text = Instantiate(probText, new Vector3(7.15f, 1, -1), Quaternion.identity) as GameObject;
							choice50text.GetComponent<TextMesh>().text = "50% YELLOW";
							choice50text.transform.localScale = new Vector3(0.04f, 0.04f, 0.05f);
							choice50text.SetActive(true);
						}
						SelectMenuGenerated = true;
					}

					if (Input.GetMouseButtonDown(0))
					{
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit hit;

						if (Physics.Raycast(ray, out hit))
						{
							GameObject piece = hit.transform.gameObject;
							int clickedObjectID = piece.GetInstanceID();
							if (blueProbs.ContainsKey(100) && blueProbs[100] > 0 && clickedObjectID == choice100.GetInstanceID())
							{
								choice = 100;
								SelectTokenText.SetActive(false);
								DestroyImmediate(choice100);
								choice100text.SetActive(false);
								if (blueProbs.ContainsKey(75) && blueProbs[75] > 0)
								{
									DestroyImmediate(choice75);
									choice75text.SetActive(false);
								}
								if (blueProbs.ContainsKey(50) && blueProbs[50] > 0)
								{
									DestroyImmediate(choice50);
									choice50text.SetActive(false);
								}
								(gameObjectTurn, probability, probText) = SpawnPiece();
								SelectMenuGenerated = false;
							}
							if (blueProbs.ContainsKey(75) && blueProbs[75] > 0 && clickedObjectID == choice75.GetInstanceID())
							{
								choice = 75;
								SelectTokenText.SetActive(false);
								if (blueProbs.ContainsKey(100) && blueProbs[100] > 0)
								{
									DestroyImmediate(choice100);
									choice100text.SetActive(false);
								}
								DestroyImmediate(choice75);
								choice75text.SetActive(false);
								if (blueProbs.ContainsKey(50) && blueProbs[50] > 0)
								{
									DestroyImmediate(choice50);
									choice50text.SetActive(false);
								}
								(gameObjectTurn, probability, probText) = SpawnPiece();
								SelectMenuGenerated = false;
							}
							if (blueProbs.ContainsKey(50) && blueProbs[50] > 0 && clickedObjectID == choice50.GetInstanceID())
							{
								choice = 50;
								SelectTokenText.SetActive(false);
								if (blueProbs.ContainsKey(100) && blueProbs[100] > 0)
								{
									DestroyImmediate(choice100);
									choice100text.SetActive(false);
								}
								if (blueProbs.ContainsKey(75) && blueProbs[75] > 0)
								{
									DestroyImmediate(choice75);
									choice75text.SetActive(false);
								}
								DestroyImmediate(choice50);
								choice50text.SetActive(false);
								(gameObjectTurn, probability, probText) = SpawnPiece();
								SelectMenuGenerated = false;
							}
						}
					}
				}
				else
				{
					// update the objects position
					Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					gameObjectTurn.transform.position = new Vector3(
						Mathf.Clamp(pos.x, 0, numColumns - 1),
						gameObjectField.transform.position.y + 1, 0);

					// click the left mouse button to drop the piece into the selected column
					if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping)
					{
						mouseButtonPressed = true;

						probText.transform.parent = null;
						probText.SetActive(false);

						StartCoroutine(dropPiece(gameObjectTurn, probText, probability));
					}
					else
					{
						mouseButtonPressed = false;
					}
				}
			}
			else
			{
				if (gameObjectTurn == null)
				{
					(gameObjectTurn, probability, probText) = SpawnPiece();
				}
				else
				{
					if (!isDropping)
					{
						probText.transform.parent = null;
						probText.SetActive(false);
						//Thread.Sleep(1000);
						StartCoroutine(dropPiece(gameObjectTurn, probText, probability));
					}
				}
			}
		}

		/// <summary>
		/// This method searches for a empty cell and lets 
		/// the object fall down into this cell
		/// </summary>
		/// <param name="gObject">Game Object.</param>
		IEnumerator dropPiece(GameObject gObject, GameObject probText, int probability)
		{

			isDropping = true;
			probText.SetActive(false);

			Vector3 startPosition = gObject.transform.position;
			Vector3 endPosition = new Vector3();

			// round to a grid cell
			int x = Mathf.RoundToInt(startPosition.x);
			startPosition = new Vector3(x, startPosition.y, startPosition.z);

			// is there a free cell in the selected column?
			bool foundFreeCell = false;
			(int, int) tempLocation = (-1, -1);

			for (int i = numRows - 1; i >= 0; i--)
			{
				if (field[x, i] == 0)
				{
					//Shivani Puli Data Collection
					turn++;
					int index = i * numColumns + x;
					mydata.placement_order[index] = turn;
					mydata.superposition[index] = probability;
					foundFreeCell = true;
					if (isPlayersTurn)
					{
						probField[x, i] = probability; //probability of being a blue piece
						playMove(x, "1");
						if (probability == 100)
						{
							field[x, i] = 1;
							mydata.outcome[index] = 1;
						}
						else
							field[x, i] = 3;
					}
					else
					{
						probField[x, i] = 100 - probability; //probability of being a blue piece
						playMove(x, "2");
						if (probability == 100)
						{
							field[x, i] = 2;
							mydata.outcome[index] = 2;
						}
						else
							field[x, i] = 3;
					}

					tempLocation = (x, i);

					if (probability != 100)
					{
						dropOrder[numSuperpositionPieces] = (x, i);
					}
					endPosition = new Vector3(x, i * -1, startPosition.z);

					break;
				}
			}

			if (foundFreeCell)
			{
				// Instantiate a new Piece, disable the temporary
				GameObject g = Instantiate(gObject) as GameObject;
				gameObjectTurn.GetComponent<Renderer>().enabled = false;

				GameObject p = Instantiate(probText) as GameObject;
				p.transform.parent = g.transform;

				float distance = Vector3.Distance(startPosition, endPosition);

				if (probability != 100)
				{
					Color c = g.GetComponent<MeshRenderer>().material.color;
					c.a = 0.5f;
					g.GetComponent<MeshRenderer>().material.color = c;
				}

				float t = 0;
				while (t < 1)
				{
					t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

					g.transform.position = Vector3.Lerp(startPosition, endPosition, t);
					yield return null;
				}

				g.transform.parent = gameObjectField.transform;

				if (probability != 100)
				{
					(float, float) piecePos = (g.transform.position.x, g.transform.position.y);
					if (isPlayersTurn)
						probDict.Add(g.transform.GetInstanceID(), (probability, tempLocation, piecePos, g));
					else
						probDict.Add(g.transform.GetInstanceID(), (100 - probability, tempLocation, piecePos, g));

					pieces[numSuperpositionPieces] = g;
					numSuperpositionPieces++;
				}

				// remove the temporary gameobject
				DestroyImmediate(gameObjectTurn);

				probCounter++;

				//if (probCounter == 42)
				//            {
				//	StartCoroutine(revealProbabilities());
				//            }

				StartCoroutine(Won());

				while (isCheckingForWinner)
					yield return null;

				if (probCounter == 42)
				{
					choosingReveal = true;
					revealingProbs = true;
				}

				//StartCoroutine(Won());

				//while (isCheckingForWinner)
				//	yield return null;

				isPlayersTurn = !isPlayersTurn;
				playerTurnText.GetComponent<TextMesh>().text = isPlayersTurn ? "Your Turn" : "Red's Turn";

				DestroyImmediate(playerTurnObject);

				if (isPlayersTurn)
				{
					playerTurnObject = Instantiate(pieceBlue, new Vector3(numColumns - 1.75f, -6.3f, -1), Quaternion.identity) as GameObject;
					playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
				}
				else
				{
					playerTurnObject = Instantiate(pieceRed, new Vector3(numColumns - 2.25f, -6.3f, -1), Quaternion.identity) as GameObject;
					playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

				}
			}
			else
			{
				probText.SetActive(true);
				probText.transform.position = new Vector3(1.5f, 2, -1);
			}

			isDropping = false;

			yield return 0;
		}

		void revealProbabilitiesThroughClick()
		{
			if (isPlayersTurn)
			{
				if (Input.GetMouseButtonDown(0))
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;

					if (Physics.Raycast(ray, out hit))
					{
						GameObject piece = hit.transform.gameObject;
						int clickedObjectID = piece.GetInstanceID();

						if (probDict.ContainsKey(clickedObjectID - 2))
						{
							(int probability, (int coord_x, int coord_y), (float x, float y), GameObject token) = probDict[clickedObjectID - 2];
							int p = Random.Range(1, 101);
							if (p < probability)
							{
								Vector3 pos = piece.transform.position;
								finalColor = Instantiate(
									pieceBlue,
									new Vector3(pos.x, pos.y, 0),
									Quaternion.identity) as GameObject;
								DestroyImmediate(piece);
								//Data Collection
								int index = coord_y * numColumns + coord_x;
								revealturn++;
								mydata.reveal_order[index] = revealturn;
								mydata.outcome[index] = 1;
								field[coord_x, coord_y] = 1;
							}
							else
							{
								Vector3 pos = piece.transform.position;
								finalColor = Instantiate(
									pieceRed,
									new Vector3(pos.x, pos.y, 0),
									Quaternion.identity) as GameObject;
								DestroyImmediate(piece);
								//Data Collection
								int index = coord_y * numColumns + coord_x;
								revealturn++;
								mydata.reveal_order[index] = revealturn;
								mydata.outcome[index] = 2;
								field[coord_x, coord_y] = 2;
							}
							isPlayersTurn = !isPlayersTurn;
							playerTurnText.GetComponent<TextMesh>().text = isPlayersTurn ? "Your Turn" : "Red's Turn";

							DestroyImmediate(playerTurnObject);

							if (isPlayersTurn)
							{
								playerTurnObject = Instantiate(pieceBlue, new Vector3(numColumns - 1.75f, -6, 1), Quaternion.identity) as GameObject;
								playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
							}
							else
							{
								playerTurnObject = Instantiate(pieceRed, new Vector3(numColumns - 2.25f, -6, 1), Quaternion.identity) as GameObject;
								playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
							}
							probDict.Remove(clickedObjectID - 2);

						}
						StartCoroutine(Won());
					}
				}
			}
			else
			{
				Thread.Sleep(1000);
				int chosenObjectID = probDict.ElementAt(Random.Range(0, probDict.Count)).Key;
				(int probability, (int coord_x, int coord_y), (float x, float y), GameObject token) = probDict[chosenObjectID];
				int p = Random.Range(1, 101);
				if (p < probability)
				{
					finalColor = Instantiate(pieceBlue, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
					DestroyImmediate(token);
					//Data Collection
					int index = coord_y * numColumns + coord_x;
					revealturn++;
					mydata.reveal_order[index] = revealturn;
					mydata.outcome[index] = 1;
					field[coord_x, coord_y] = 1;
				}
				else
				{
					finalColor = Instantiate(pieceRed, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
					DestroyImmediate(token);
					//Data Collection
					int index = coord_y * numColumns + coord_x;
					revealturn++;
					mydata.reveal_order[index] = revealturn;
					mydata.outcome[index] = 2;
					field[coord_x, coord_y] = 2;
				}
				// Debug.Log((coord_x, coord_y));

				isPlayersTurn = !isPlayersTurn;
				playerTurnText.GetComponent<TextMesh>().text = isPlayersTurn ? "Your Turn" : "Red's Turn";

				DestroyImmediate(playerTurnObject);

				if (isPlayersTurn)
				{
					playerTurnObject = Instantiate(pieceBlue, new Vector3(numColumns - 1.75f, -6, 1), Quaternion.identity) as GameObject;
					playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
				}
				else
				{
					playerTurnObject = Instantiate(pieceRed, new Vector3(numColumns - 2.25f, -6, 1), Quaternion.identity) as GameObject;
					playerTurnObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
				}
				probDict.Remove(chosenObjectID);

				StartCoroutine(Won());
			}

			if (gameOver)
			{
				revealingProbs = false;
			}

		}

		IEnumerator revealProbabilities()
		{
			revealingProbs = true;
			int x, y;
			GameObject piece;
			for (int i = 0; i < numSuperpositionPieces; i++)
			{
				piece = pieces[i];
				(x, y) = dropOrder[i];
				int probability = probField[x, y];
				int p = Random.Range(1, 101);
				if (p < probability)
				{
					Vector3 pos = piece.transform.position;
					finalColor = Instantiate(
						pieceBlue,
						new Vector3(pos.x, pos.y, 0),
						Quaternion.identity) as GameObject;
					DestroyImmediate(piece);
					field[x, y] = 1;
				}
				else
				{
					Vector3 pos = piece.transform.position;
					finalColor = Instantiate(
						pieceRed,
						new Vector3(pos.x, pos.y, 0),
						Quaternion.identity) as GameObject;
					DestroyImmediate(piece);
					field[x, y] = 2;
				}

				StartCoroutine(Won());

				while (isCheckingForWinner)
					yield return null;

				if (gameOver)
					break;


				yield return new WaitForSeconds(1);
			}

			revealingProbs = false;
			yield return 0;
		}
		/// <summary>
		/// Check for Winner
		/// </summary>
		IEnumerator Won()
		{
			isCheckingForWinner = true;

			bool blueWon = false;

			//string output = "";
			//for (int x = 0; x < numColumns; x++)
			//         {
			//	string newRow = "";
			//	for (int y = 0; y < numRows; y++)
			//	{
			//		newRow += field[x, y].ToString();
			//	}
			//	output += newRow + "\n";
			//}

			//Debug.Log(output);

			for (int x = 0; x < numColumns; x++)
			{
				for (int y = 0; y < numRows; y++)
				{
					//if somebody won, gameOver = true;
					int color = field[x, y];
					if (color != 3 && color != 0)
					{
						//check up
						if (y >= 3 && field[x, y - 1] == color && field[x, y - 2] == color && field[x, y - 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						//check down
						if (y <= numRows - 4 && field[x, y + 1] == color && field[x, y + 2] == color && field[x, y + 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						//check left
						if (x >= 3 && field[x - 1, y] == color && field[x - 2, y] == color && field[x - 3, y] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						//check right
						if (x <= numColumns - 4 && field[x + 1, y] == color && field[x + 2, y] == color && field[x + 3, y] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						//check upper left diagonal
						if (y >= 3 && x >= 3 && field[x - 1, y - 1] == color && field[x - 2, y - 2] == color && field[x - 3, y - 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						// check upper right diagonal
						if (y >= 3 && x <= numColumns - 4 && field[x + 1, y - 1] == color && field[x + 2, y - 2] == color && field[x + 3, y - 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						// check lower left diagonal
						if (x >= 3 && y <= numRows - 4 && field[x - 1, y + 1] == color && field[x - 2, y + 2] == color && field[x - 3, y + 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}

						//check lower right diagonal
						if (x <= numColumns - 4 && y <= numRows - 4 && field[x + 1, y + 1] == color && field[x + 2, y + 2] == color && field[x + 3, y + 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
							revealingProbs = false;
						}
					}
					yield return null;
				}

				yield return null;
			}

			// if Game Over update the winning text to show who has won
			if (gameOver == true)
			{
				//Data Collection
				if (blueWon)
					mydata.winner = 1;
				else
					mydata.winner = 2;
				saveData.Save(mydata);

				// star system
				if (!starUpdated)
				{
					starUpdated = true;
					if (GameManager.saveData.starSystem[14] + 1 <= 3)
					{
						GameManager.saveData.starSystem[14] = GameManager.saveData.starSystem[14] + 1;
						GameManager.Save();
					}
				}
				// StarSystem
				DestroyImmediate(Star1);
				DestroyImmediate(Star2);
				DestroyImmediate(Star3);
				ShowStarSystem();

				if (!resultBGshown)
				{
					GameObject bg = Instantiate(resultBG, new Vector3(3, -2.5f, -1), Quaternion.identity) as GameObject;
					resultBGshown = true;
				}
				winningText.GetComponent<TextMesh>().text = blueWon ? playerWonText : playerLoseText;
				GameObject star = Instantiate(starText, new Vector3(-0.7f, -3.5f, -1), Quaternion.identity) as GameObject;

				// Reward System
				if (GameManager.rewardSystem[14])
				{
					Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.QueueBits, 14);
				}
			}
			else
			{
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if (!FieldContainsUnknownCell())
				{
					mydata.winner = 0;
					saveData.Save(mydata);

					// star system
					if (!starUpdated)
					{
						starUpdated = true;
						if (GameManager.saveData.starSystem[14] + 1 <= 3)
						{
							GameManager.saveData.starSystem[14] = GameManager.saveData.starSystem[14] + 1;
							GameManager.Save();
						}
					}
					// StarSystem
					DestroyImmediate(Star1);
					DestroyImmediate(Star2);
					DestroyImmediate(Star3);
					ShowStarSystem();

					if (!resultBGshown)
					{
						GameObject bg = Instantiate(resultBG, new Vector3(3, -2.5f, -1), Quaternion.identity) as GameObject;
						resultBGshown = true;
					}
					gameOver = true;
					winningText.GetComponent<TextMesh>().text = drawText;
					GameObject star = Instantiate(starText, new Vector3(-0.7f, -3.5f, -1), Quaternion.identity) as GameObject;

					// Reward System
					if (GameManager.rewardSystem[14])
					{
						Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.QueueBits, 14);
					}
				}
			}

			isCheckingForWinner = false;

			yield return 0;
		}

		void ShowStarSystem()
		{
			// Star System
			if (GameManager.saveData.starSystem[14] == 0)
			{
				Star1 = Instantiate(starEmpty, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starEmpty, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starEmpty, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
			else if (GameManager.saveData.starSystem[14] == 1)
			{
				Star1 = Instantiate(starFilled, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starEmpty, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starEmpty, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
			else if (GameManager.saveData.starSystem[14] == 2)
			{
				Star1 = Instantiate(starFilled, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starFilled, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starEmpty, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
			else if (GameManager.saveData.starSystem[14] == 3)
			{
				Star1 = Instantiate(starFilled, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starFilled, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starFilled, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
		}

		/* void UpdatePlayAgainButton()
		{
			RaycastHit hit;
			//ray shooting out of the camera from where the mouse is
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name)
			{
				btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainHoverColor;
				//check if the left mouse has been pressed down this frame
				if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnPlayAgainTouching == false)
				{
					btnPlayAgainTouching = true;

					//CreateField();
					Application.LoadLevel(0);
				}
			}
			else
			{
				btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainOrigColor;
			}

			if (Input.touchCount == 0)
			{
				btnPlayAgainTouching = false;
			}
		} */

		/// <summary>
		/// check if the field contains an empty cell
		/// </summary>
		/// <returns><c>true</c>, if it contains empty cell, <c>false</c> otherwise.</returns>
		bool FieldContainsUnknownCell()
		{
			for (int x = 0; x < numColumns; x++)
			{
				for (int y = 0; y < numRows; y++)
				{
					if (field[x, y] == 3 || field[x, y] == 0)
						return true;
				}
			}
			return false;
		}
	}
}
