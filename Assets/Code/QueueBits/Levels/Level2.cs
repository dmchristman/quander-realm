using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace QueueBits
{
	public class Level2 : MonoBehaviour
	{
		enum Piece
		{
			Empty = 0,
			Blue = 1,
			Red = 2
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

		public GameObject winningText;
		public GameObject resultBG;
		public string playerWonText = "You Won!";
		public string playerLoseText = "Red Won!";
		public string drawText = "Draw!";

		// public GameObject probText;
		// public GameObject starText;

		/* public GameObject btnPlayAgain;
		bool btnPlayAgainTouching = false;
		Color btnPlayAgainOrigColor;
		Color btnPlayAgainHoverColor = new Color(255, 143, 4);

		public GameObject btnNextLevel;
		bool btnNextLevelTouching = false; */

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

		bool isPlayersTurn = true;
		bool isLoading = true;
		bool isDropping = false;
		bool mouseButtonPressed = false;

		bool gameOver = false;
		bool isCheckingForWinner = false;

		string state = "000000000000000000000000000000000000000000";
		int[] colPointers = { 5, 5, 5, 5, 5, 5, 5 };
		HashSet<(int, int)> visited = new HashSet<(int, int)>();

		// dialogue
		bool dialoguePhase = false;

		// Shivani Puli Data Collection
		int turn = 0;
		Data mydata = new Data();



		// Use this for initialization
		void Start()
		{
			ShowStarSystem();

			// dialogue
			if (GameManager.saveData.dialogueSystem[2])
			{
				dialoguePhase = true;
				Wrapper.Events.StartDialogueSequence?.Invoke("QB_Level2");
				GameManager.saveData.dialogueSystem[2] = false;
				GameManager.Save();
				Wrapper.Events.DialogueSequenceEnded += updateDialoguePhase;
			}

			//Shivani Puli Data Collection
			mydata.level = 2;
			mydata.userID = Wrapper.Events.GetPlayerResearchCode?.Invoke();
			mydata.prefilledBoard = -1;
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


			int max = Mathf.Max(numRows, numColumns);

			if (numPiecesToWin > max)
				numPiecesToWin = max;

			CreateField();

			// GameObject levelText = Instantiate(probText, new Vector3(numColumns - 5f, -7f, -1), Quaternion.identity) as GameObject;
			// levelText.GetComponent<TextMesh>().text = "Level 2";

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
			// btnNextLevel.SetActive(false);

			isLoading = true;

			gameObjectField = GameObject.Find("Field");
			if (gameObjectField != null)
			{
				DestroyImmediate(gameObjectField);
			}
			gameObjectField = new GameObject("Field");

			// create an empty field and instantiate the cells
			field = new int[numColumns, numRows];

			GameObject g = Instantiate(pieceField, new Vector3(3, -2.8f, 1), Quaternion.identity) as GameObject;
			g.transform.parent = gameObjectField.transform;

			for (int x = 0; x < numColumns; x++)
			{
				for (int y = 0; y < numRows; y++)
				{
					field[x, y] = (int)Piece.Empty;
				}
			}

			isLoading = false;
			gameOver = false;

			// center camera
			Camera.main.transform.position = new Vector3(
				(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f), Camera.main.transform.position.z);

			winningText.transform.position = new Vector3(
				(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) + 0.2f, winningText.transform.position.z);

	// 		btnNextLevel.transform.position = new Vector3(
	// (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnNextLevel.transform.position.z);

			//btnPlayAgain.transform.position = new Vector3(
			//	(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) - 2, btnPlayAgain.transform.position.z);
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

		void printState()
		{
			string p_state = "";
			for (int r = 0; r < 6; r++)
			{
				int i = index(r, 0);
				p_state += state.Substring(i, 7) + "\n";
			}
			Debug.Log(p_state);
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
			while(i < state.Length && state.Substring(i, 1).Equals(color))
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
			foreach (int column in moves)
			{
				playMove(column, "2");
				int value = minimax(0, 3, false); //changed to test
				Debug.Log("Column " + column + ": " + value);
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
		/// Spawns a piece at mouse position above the first row
		/// </summary>
		/// <returns>The piece.</returns>
		GameObject SpawnPiece()
		{
			Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			if (!isPlayersTurn)
			{
				List<int> moves = GetPossibleMoves();

				if (moves.Count > 0)
				{
					int column = findBestMove(colPointers);

					//Shivani Puli changed for data collection
					int r = colPointers[column];
					int index = r * numColumns + column;
					turn++;
					mydata.placement_order[index] = turn;//turn count
					mydata.superposition[index] = 100;
					mydata.reveal_order[index] = turn;
					mydata.outcome[index] = 2;

					playMove(column, "2");

					spawnPos = new Vector3(column, 0, 0);
				}

			}

			GameObject g = Instantiate(
					isPlayersTurn ? pieceBlue : pieceRed, // is players turn = spawn blue, else spawn red
					new Vector3(
					Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
					gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
					Quaternion.identity) as GameObject;

			return g;
		}

		void ShowStarSystem()
		{
			// Star System
			if (GameManager.saveData.starSystem[2] == 0)
			{
				Star1 = Instantiate(starEmpty, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starEmpty, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starEmpty, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
			else if (GameManager.saveData.starSystem[2] == 1)
			{
				Star1 = Instantiate(starFilled, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starEmpty, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starEmpty, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
			else if (GameManager.saveData.starSystem[2] == 2)
			{
				Star1 = Instantiate(starFilled, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starFilled, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starEmpty, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
			else if (GameManager.saveData.starSystem[2] == 3)
			{
				Star1 = Instantiate(starFilled, new Vector3(-3.3f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star2 = Instantiate(starFilled, new Vector3(-2.4f, -6.9f, 1), Quaternion.identity) as GameObject;
				Star3 = Instantiate(starFilled, new Vector3(-1.5f, -6.9f, 1), Quaternion.identity) as GameObject;
			}
		}

		void UpdatePlayAgainButton()
		{
			RaycastHit hit;
			//ray shooting out of the camera from where the mouse is
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			/* if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name)
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
			} */
		}

		void UpdateNextLevelButton()
		{
			RaycastHit hit;
			//ray shooting out of the camera from where the mouse is
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			/* if (Physics.Raycast(ray, out hit) && hit.collider.name == btnNextLevel.name)
			{
				//check if the left mouse has been pressed down this frame
				if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnNextLevelTouching == false)
				{
					btnNextLevelTouching = true;

					//CreateField();
					SceneManager.LoadScene("level2");
				}
			}

			if (Input.touchCount == 0)
			{
				btnNextLevelTouching = false;
			} */
		}

		// Update is called once per frame
		void Update()
		{
			if (isLoading)
				return;

			if (dialoguePhase)
				return;

			if (isCheckingForWinner)
				return;

			if (gameOver)
			{
				winningText.SetActive(true);
				/* btnPlayAgain.SetActive(false);
				btnNextLevel.SetActive(false);

				// fix play again button
				btnPlayAgain.transform.position = new Vector3(
	(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
				btnPlayAgain.GetComponent<TextMesh>().color = Color.white;
				btnPlayAgain.GetComponent<TextMesh>().text = "EXIT TO MENU";
				btnPlayAgain.GetComponent<TextMesh>().fontSize = 70; */

				UpdatePlayAgainButton();
				UpdateNextLevelButton();

				return;
			}

			if (isPlayersTurn)
			{
				if (gameObjectTurn == null)
				{
					gameObjectTurn = SpawnPiece();
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

						Vector3 startPosition = gameObjectTurn.transform.position;
						int column = Mathf.RoundToInt(startPosition.x);

						List<int> moves = GetPossibleMoves();
						if (moves.Contains(column))
						{
							//Shivani Puli Data Collection
							int r = colPointers[column];
							int index = r * numColumns + column;
							turn++;
							mydata.placement_order[index] = turn;//turn count
							mydata.superposition[index] = 100;
							mydata.reveal_order[index] = turn;
							mydata.outcome[index] = 1;


							playMove(column, "1");
							StartCoroutine(dropPiece(gameObjectTurn));
						}
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
					gameObjectTurn = SpawnPiece();
				}
				else
				{
					if (!isDropping)
						StartCoroutine(dropPiece(gameObjectTurn));
				}
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
				for (int y = numRows - 1; y >= 0; y--)
				{
					if (field[x, y] == (int)Piece.Empty)
					{
						possibleMoves.Add(x);
						break;
					}
				}
			}
			return possibleMoves;
		}

		/// <summary>
		/// This method searches for a empty cell and lets 
		/// the object fall down into this cell
		/// </summary>
		/// <param name="gObject">Game Object.</param>
		IEnumerator dropPiece(GameObject gObject)
		{
			isDropping = true;

			Vector3 startPosition = gObject.transform.position;
			Vector3 endPosition = new Vector3();

			// round to a grid cell
			int x = Mathf.RoundToInt(startPosition.x);
			startPosition = new Vector3(x, startPosition.y, startPosition.z);

			// is there a free cell in the selected column?
			bool foundFreeCell = false;
			for (int i = numRows - 1; i >= 0; i--)
			{
				if (field[x, i] == 0)
				{
					foundFreeCell = true;
					field[x, i] = isPlayersTurn ? (int)Piece.Blue : (int)Piece.Red;
					endPosition = new Vector3(x, i * -1, startPosition.z);

					break;
				}
			}

			if (foundFreeCell)
			{
				// Instantiate a new Piece, disable the temporary
				GameObject g = Instantiate(gObject) as GameObject;
				gameObjectTurn.GetComponent<Renderer>().enabled = false;

				float distance = Vector3.Distance(startPosition, endPosition);

				float t = 0;
				while (t < 1)
				{
					t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

					g.transform.position = Vector3.Lerp(startPosition, endPosition, t);
					yield return null;
				}

				g.transform.parent = gameObjectField.transform;

				// remove the temporary gameobject
				DestroyImmediate(gameObjectTurn);

				// run coroutine to check if someone has won
				StartCoroutine(Won());

				// wait until winning check is done
				while (isCheckingForWinner)
					yield return null;

				isPlayersTurn = !isPlayersTurn;
			}

			isDropping = false;

			yield return 0;
		}

		/// <summary>
		/// Check for Winner
		/// </summary>
		IEnumerator Won()
		{
			isCheckingForWinner = true;

			for (int x = 0; x < numColumns; x++)
			{
				for (int y = 0; y < numRows; y++)
				{
					// Get the Laymask to Raycast against, if its Players turn only include
					// Layermask Blue otherwise Layermask Red
					int layermask = isPlayersTurn ? (1 << 8) : (1 << 9);

					// If its Players turn ignore red as Starting piece and wise versa
					if (field[x, y] != (isPlayersTurn ? (int)Piece.Blue : (int)Piece.Red))
					{
						continue;
					}

					// shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
					RaycastHit[] hitsHorz = Physics.RaycastAll(
						new Vector3(x, y * -1, 0),
						Vector3.right,
						numPiecesToWin - 1,
						layermask);

					// return true (won) if enough hits
					if (hitsHorz.Length == numPiecesToWin - 1)
					{
						gameOver = true;
						break;
					}

					// shoot a ray up to test vertically
					RaycastHit[] hitsVert = Physics.RaycastAll(
						new Vector3(x, y * -1, 0),
						Vector3.up,
						numPiecesToWin - 1,
						layermask);

					if (hitsVert.Length == numPiecesToWin - 1)
					{
						gameOver = true;
						break;
					}

					// test diagonally
					if (allowDiagonally)
					{
						// calculate the length of the ray to shoot diagonally
						float length = Vector2.Distance(new Vector2(0, 0), new Vector2(numPiecesToWin - 1, numPiecesToWin - 1));

						RaycastHit[] hitsDiaLeft = Physics.RaycastAll(
							new Vector3(x, y * -1, 0),
							new Vector3(-1, 1),
							length,
							layermask);

						if (hitsDiaLeft.Length == numPiecesToWin - 1)
						{
							gameOver = true;
							break;
						}

						RaycastHit[] hitsDiaRight = Physics.RaycastAll(
							new Vector3(x, y * -1, 0),
							new Vector3(1, 1),
							length,
							layermask);

						if (hitsDiaRight.Length == numPiecesToWin - 1)
						{
							gameOver = true;
							break;
						}
					}

					yield return null;
				}

				yield return null;
			}

			// if Game Over update the winning text to show who has won
			if (gameOver == true)
			{
				// star system
				if (!starUpdated)
				{
					starUpdated = true;
					if (GameManager.saveData.starSystem[2] + 1 <= 3)
					{
						GameManager.saveData.starSystem[2] = GameManager.saveData.starSystem[2] + 1;
						GameManager.Save();
					}
				}

				// StarSystem
				DestroyImmediate(Star1);
				DestroyImmediate(Star2);
				DestroyImmediate(Star3);
				ShowStarSystem();

				GameObject bg = Instantiate(resultBG, new Vector3(3, -2.5f, -1), Quaternion.identity) as GameObject;
				winningText.GetComponent<TextMesh>().text = isPlayersTurn ? playerWonText : playerLoseText;
				// GameObject star = Instantiate(starText, new Vector3(-0.7f, -3.5f, -1), Quaternion.identity) as GameObject;

				// Shivani Puli Data Collection -> store winner
				if (isPlayersTurn)
					mydata.winner = 1;
				else
					mydata.winner = 2;
				saveData.Save(mydata);


				// Reward System
				if (GameManager.rewardSystem[2])
				{
					Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.QueueBits, 2);
				}
			}
			else
			{
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if (!FieldContainsEmptyCell())
				{
					//Shivani Puli Data Collection
					mydata.winner = 0;
					saveData.Save(mydata);


					// star system
					if (!starUpdated)
					{
						starUpdated = true;
						if (GameManager.saveData.starSystem[2] + 1 <= 3)
						{
							GameManager.saveData.starSystem[2] = GameManager.saveData.starSystem[2] + 1;
							GameManager.Save();
						}
					}
					// StarSystem
					DestroyImmediate(Star1);
					DestroyImmediate(Star2);
					DestroyImmediate(Star3);
					ShowStarSystem();

					GameObject bg = Instantiate(resultBG, new Vector3(3, -2.5f, -1), Quaternion.identity) as GameObject;
					gameOver = true;
					winningText.GetComponent<TextMesh>().text = drawText;
					// GameObject star = Instantiate(starText, new Vector3(-0.7f, -3.5f, -1), Quaternion.identity) as GameObject;

					// Reward System
					if (GameManager.rewardSystem[2])
					{
						Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.QueueBits, 2);
					}
				}
			}

			isCheckingForWinner = false;

			yield return 0;
		}

		/// <summary>
		/// check if the field contains an empty cell
		/// </summary>
		/// <returns><c>true</c>, if it contains empty cell, <c>false</c> otherwise.</returns>
		bool FieldContainsEmptyCell()
		{
			for (int x = 0; x < numColumns; x++)
			{
				for (int y = 0; y < numRows; y++)
				{
					if (field[x, y] == (int)Piece.Empty)
						return true;
				}
			}
			return false;
		}
	}
}