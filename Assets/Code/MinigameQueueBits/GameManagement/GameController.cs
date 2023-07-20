using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//using System;
//using MySql.Data.MySqlClient;
//using Data;

namespace QueueBits
{
	public class GameController : MonoBehaviour 
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

		// Gameobjects 
		public GameObject pieceRed;
		public GameObject pieceBlue;
		public GameObject pieceField;
		public GameObject finalColor;

		public GameObject pieceSuperposition;
		public GameObject piece25;
		public GameObject piece50;
		public GameObject piece75;
		public GameObject playerTurnText;

		//Piece Count Displays
		public GameObject blueTitle;
		public GameObject redTitle;

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

		Dictionary<int, (int, (int, int))> probDict = new Dictionary<int, (int, (int, int))>();

		int[] redProbs = { 25, 25, 25, 25, 25, 50, 50, 50, 50, 50, 75, 75, 75, 75, 100, 100, 100, 100, 100, 100, 100 };
		int[] blueProbs = { 25, 25, 25, 25, 25, 50, 50, 50, 50, 50, 75, 75, 75, 75, 100, 100, 100, 100, 100, 100, 100 };

		int bturns = 0;
		int rturns = 0;

		public GameObject winningText;

		public GameObject playerTurnObject;

		public string playerWonText = "Yellow Won!";
		public string playerLoseText = "Red Won!";
		public string drawText = "Draw!";

		public GameObject probText;

		public GameObject btnPlayAgain;
		bool btnPlayAgainTouching = false;
		Color btnPlayAgainOrigColor;
		Color btnPlayAgainHoverColor = new Color(255, 143,4);

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

		// Use this for initialization
		void Start () 
		{
			int max = Mathf.Max (numRows, numColumns);

			if(numPiecesToWin > max)
				numPiecesToWin = max;

			CreateField ();

			isPlayersTurn = System.Convert.ToBoolean(Random.Range (0, 2));
			playerTurnText.GetComponent<TextMesh>().text = isPlayersTurn ? "Yellow's Turn" : "Red's Turn";

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

			btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
		}

		/// <summary>
		/// Creates the field.
		/// </summary>
		void CreateField()
		{
			winningText.SetActive(false);
			btnPlayAgain.SetActive(true);
			playerTurnText.SetActive(true);
			playerTurnText.GetComponent<Renderer>().sortingOrder = 4;

			isLoading = true;

			gameObjectField = GameObject.Find ("Field");
			if(gameObjectField != null)
			{
				DestroyImmediate(gameObjectField);
			}
			gameObjectField = new GameObject("Field");

			// create an empty field and instantiate the cells
			field = new int[numColumns, numRows];
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					field[x, y] = (int)Piece.Empty;
					GameObject g = Instantiate(pieceField, new Vector3(x, -y, -1), Quaternion.identity) as GameObject;
					g.transform.parent = gameObjectField.transform;
				}
			}

			isLoading = false;
			gameOver = false;

			// center camera
			Camera.main.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f), Camera.main.transform.position.z);

			winningText.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) + 1, winningText.transform.position.z);

			//btnPlayAgain.transform.position = new Vector3(
			//	(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) - 1, btnPlayAgain.transform.position.z);

			playerTurnText.transform.position = new Vector3(
			(numColumns - 1) / 2.0f, -6, playerTurnText.transform.position.z);

			//Piece Count Displays
			blueTitle.transform.position = new Vector3(-3, 1, 0);
			blueTitle.GetComponent<Renderer>().sortingOrder = 10;
			blueTitle.SetActive(true);

			redTitle.transform.position = new Vector3(6.75f, 1, 0);
			redTitle.GetComponent<Renderer>().sortingOrder = 10;
			redTitle.SetActive(true);

			pieceBlue100 = Instantiate(pieceBlue, new Vector3(-1, 0, -1), Quaternion.identity) as GameObject;
			pieceBlue100.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceBlue75 = Instantiate(piece75, new Vector3(-1, -1, -1), Quaternion.identity) as GameObject;
			pieceBlue75.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceBlue50 = Instantiate(piece50, new Vector3(-1, -2, -1), Quaternion.identity) as GameObject;
			pieceBlue50.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceBlue25 = Instantiate(piece25, new Vector3(-1, -3, -1), Quaternion.identity) as GameObject;
			pieceBlue25.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			//Piece Count Texts - BLUE
			pieceBlue100Text = Instantiate(pieceCounterText, new Vector3(-1.75f, 0, -1), Quaternion.identity) as GameObject;
			pieceBlue100Text.SetActive(true);

			pieceBlue75Text = Instantiate(pieceCounterText, new Vector3(-1.75f, -1, -1), Quaternion.identity) as GameObject;
			pieceBlue75Text.SetActive(true);

			pieceBlue50Text = Instantiate(pieceCounterText, new Vector3(-1.75f, -2, -1), Quaternion.identity) as GameObject;
			pieceBlue50Text.SetActive(true);

			pieceBlue25Text = Instantiate(pieceCounterText, new Vector3(-1.75f, -3, -1), Quaternion.identity) as GameObject;
			pieceBlue25Text.SetActive(true);

			//Piece Count Displays - RED
			pieceRed100 = Instantiate(pieceRed, new Vector3(7, 0, -1), Quaternion.identity) as GameObject;
			pieceRed100.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceRed75 = Instantiate(piece25, new Vector3(7, -1, -1), Quaternion.identity) as GameObject;
			pieceRed75.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceRed50 = Instantiate(piece50, new Vector3(7, -2, -1), Quaternion.identity) as GameObject;
			pieceRed50.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			pieceRed25 = Instantiate(piece75, new Vector3(7, -3, -1), Quaternion.identity) as GameObject;
			pieceRed25.transform.localScale -= new Vector3(0.5f, 0.5f, 0);

			//Piece Count Texts - RED
			pieceRed100Text = Instantiate(pieceCounterText, new Vector3(7.5f, 0, -1), Quaternion.identity) as GameObject;
			pieceRed100Text.SetActive(true);

			pieceRed75Text = Instantiate(pieceCounterText, new Vector3(7.5f, -1, -1), Quaternion.identity) as GameObject;
			pieceRed75Text.SetActive(true);

			pieceRed50Text = Instantiate(pieceCounterText, new Vector3(7.5f, -2, -1), Quaternion.identity) as GameObject;
			pieceRed50Text.SetActive(true);

			pieceRed25Text = Instantiate(pieceCounterText, new Vector3(7.5f, -3, -1), Quaternion.identity) as GameObject;
			pieceRed25Text.SetActive(true);

			pieceCounterText.SetActive(false);
		}

		/// <summary>
		/// Spawns a piece at mouse position above the first row
		/// </summary>
		/// <returns>The piece.</returns>
		(GameObject, int, GameObject) SpawnPiece()
		{
			Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int prob = 0;

			if (isPlayersTurn)
			{
				int i = Random.Range(0, blueProbs.Length - bturns);
				prob = blueProbs[i];

				// delete probability from player's list
				for (int k = 0; k < blueProbs.Length - bturns - 1; k++)
				{
					if (k >= i)
					{
						blueProbs[k] = blueProbs[k + 1];
					}
				}
				bturns++;

				if (prob == 100)
				{
					pieceSuperposition = pieceBlue;
					pieceBlue100Count--;
					pieceBlue100Text.GetComponent<TextMesh>().text = pieceBlue100Count.ToString();
				}
				else if (prob == 75)
				{
					pieceSuperposition = piece75;
					pieceBlue75Count--;
					pieceBlue75Text.GetComponent<TextMesh>().text = pieceBlue75Count.ToString();
				}
				else if (prob == 50)
				{
					pieceSuperposition = piece50;
					pieceBlue50Count--;
					pieceBlue50Text.GetComponent<TextMesh>().text = pieceBlue50Count.ToString();
				}
				else
				{
					pieceSuperposition = piece25;
					pieceBlue25Count--;
					pieceBlue25Text.GetComponent<TextMesh>().text = pieceBlue25Count.ToString();
				}

				probText.GetComponent<TextMesh>().text = prob.ToString() + "% YELLOW";
			}
			else
			{
				int i = Random.Range(0, redProbs.Length - rturns);
				prob = redProbs[i];

				// delete probability from player's list
				for (int k = 0; k < redProbs.Length - rturns - 1; k++)
				{
					if (k >= i)
					{
						redProbs[k] = redProbs[k + 1];
					}
				}
				rturns++;
				if (prob == 100)
				{
					pieceSuperposition = pieceRed;
					pieceRed100Count--;
					pieceRed100Text.GetComponent<TextMesh>().text = pieceRed100Count.ToString();
				}
				else if (prob == 75)
				{
					pieceSuperposition = piece25;
					pieceRed75Count--;
					pieceRed75Text.GetComponent<TextMesh>().text = pieceRed75Count.ToString();
				}
				else if (prob == 50)
				{
					pieceSuperposition = piece50;
					pieceRed50Count--;
					pieceRed50Text.GetComponent<TextMesh>().text = pieceRed50Count.ToString();
				}
				else
				{
					pieceSuperposition = piece75;
					pieceRed25Count--;
					pieceRed25Text.GetComponent<TextMesh>().text = pieceRed25Count.ToString();
				}

				probText.GetComponent<TextMesh>().text = prob.ToString() + "% RED";
			}
			GameObject g = Instantiate(pieceSuperposition,
					new Vector3(
					Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
					gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
					Quaternion.identity) as GameObject;

			probText.transform.position = new Vector3(spawnPos.x + 1, gameObjectField.transform.position.y + 1, 0);
			probText.transform.parent = g.transform;

			probText.SetActive(true);

			return (g, prob, probText);
		}

		// Update is called once per frame
		void Update () 
		{
			if(isLoading)
				return;

			if(isCheckingForWinner)
				return;

			if(gameOver)
			{
				winningText.SetActive(true);
				btnPlayAgain.SetActive(true);

				playerTurnText.SetActive(false);
				playerTurnObject.SetActive(false);

				// fix play again button
				btnPlayAgain.transform.position = new Vector3(
	(numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
				btnPlayAgain.GetComponent<TextMesh>().color = Color.white;
				btnPlayAgain.GetComponent<TextMesh>().text = "EXIT TO MENU";
				btnPlayAgain.GetComponent<TextMesh>().fontSize = 70;

				UpdatePlayAgainButton();

				return;
			}

			UpdatePlayAgainButton();

			if (isPlayersTurn)
			{
				if(gameObjectTurn == null)
				{
					(gameObjectTurn, probability, probText) = SpawnPiece();
				}
				else
				{
					// update the objects position
					Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					gameObjectTurn.transform.position = new Vector3(
						Mathf.Clamp(pos.x, 0, numColumns-1), 
						gameObjectField.transform.position.y + 1, 0);

					// click the left mouse button to drop the piece into the selected column
					if(Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping)
					{
						mouseButtonPressed= true;

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
				if(gameObjectTurn == null)
				{
					(gameObjectTurn, probability, probText) = SpawnPiece();
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
		}

		/// <summary>
		/// This method searches for a empty cell and lets 
		/// the object fall down into this cell
		/// </summary>
		/// <param name="gObject">Game Object.</param>
		IEnumerator dropPiece(GameObject gObject, GameObject probText, int probability)
		{
			
			isDropping = true;

			Vector3 startPosition = gObject.transform.position;
			Vector3 endPosition = new Vector3();

			// round to a grid cell
			int x = Mathf.RoundToInt(startPosition.x);
			startPosition = new Vector3(x, startPosition.y, startPosition.z);

			// is there a free cell in the selected column?
			bool foundFreeCell = false;
			(int, int) tempLocation = (-1, -1);

			for (int i = numRows-1; i >= 0; i--)
			{
				if(field[x, i] == 0)
				{
					foundFreeCell = true;
					if (isPlayersTurn)
                    {
						int p = Random.Range(1, 101);
						if (p < probability)
                        {
							Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
							finalColor = Instantiate(
								pieceBlue, // is players turn = spawn blue, else spawn red
                                new Vector3(Mathf.Clamp(pos.x, 0, numColumns - 1),
								gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
								Quaternion.identity) as GameObject;
							field[x, i] = (int)Piece.Blue;
						}
						else
                        {
							Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
							finalColor = Instantiate(
								pieceRed, // is players turn = spawn blue, else spawn red
								new Vector3(Mathf.Clamp(pos.x, 0, numColumns - 1),
								gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
								Quaternion.identity) as GameObject;
							field[x, i] = (int)Piece.Red;
						}
					}
                    else
                    {
						int p = Random.Range(1, 101);
						if (p < probability)
						{
							Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
							finalColor = Instantiate(
								pieceRed, // is players turn = spawn blue, else spawn red
								new Vector3(Mathf.Clamp(pos.x, 0, numColumns - 1),
								gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
								Quaternion.identity) as GameObject;
							field[x, i] = (int)Piece.Red;
						}
						else
						{
							Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
							finalColor = Instantiate(
								pieceBlue, // is players turn = spawn blue, else spawn red
								new Vector3(Mathf.Clamp(pos.x, 0, numColumns - 1),
								gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
								Quaternion.identity) as GameObject;
							field[x, i] = (int)Piece.Blue;
						}
					}
					
					endPosition = new Vector3(x, i * -1, startPosition.z);

					break;
				}
			}

			if(foundFreeCell)
			{
				// Instantiate a new Piece, disable the temporary
				GameObject g = Instantiate (finalColor) as GameObject;
				gameObjectTurn.GetComponent<Renderer>().enabled = false;
				finalColor.GetComponent<Renderer>().enabled = false;

				//GameObject p = Instantiate(probText) as GameObject;
				//p.transform.parent = g.transform;
				//p.SetActive(true);

				float distance = Vector3.Distance(startPosition, endPosition);

				float t = 0;
				while(t < 1)
				{
					t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

					g.transform.position = Vector3.Lerp (startPosition, endPosition, t);
					yield return null;
				}

				g.transform.parent = gameObjectField.transform;

				//if (isPlayersTurn)
				//	probDict.Add(g.transform.GetInstanceID(), (probability, tempLocation));
				//else
				//	probDict.Add(g.transform.GetInstanceID(), (100 - probability, tempLocation));

				// remove the temporary gameobject
				DestroyImmediate(gameObjectTurn);

				// run coroutine to check if someone has won
				StartCoroutine(Won());

				// wait until winning check is done
				while(isCheckingForWinner)
					yield return null;

				isPlayersTurn = !isPlayersTurn;
				playerTurnText.GetComponent<TextMesh>().text = isPlayersTurn ? "Yellow's Turn" : "Red's Turn";

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

			bool blueWon = false;

			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					//if somebody won, gameOver = true;
					int color = field[x, y];
					if (color != 0)
                    {
						//check up
						if (y >= 3 && field[x, y - 1] == color && field[x, y - 2] == color && field[x, y - 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}

						//check down
						if (y <= numRows - 4 && field[x, y + 1] == color && field[x, y + 2] == color && field[x, y + 3] == color)
                        {
							if (color == 1)
								blueWon = true;
							gameOver = true;
                        }

						//check left
						if (x >= 3 && field[x - 1, y] == color && field[x - 2, y] == color && field[x - 3, y] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}

						//check right
						if (x <= numColumns - 4 && field[x + 1, y] == color && field[x + 2, y] == color && field[x + 3, y] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}

						//check upper left diagonal
						if (y >= 3 && x >= 3 && field[x - 1, y - 1] == color && field[x - 2, y - 2] == color && field[x - 3, y - 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}

						// check upper right diagonal
						if (y >= 3 && x <= numColumns - 4 && field[x + 1, y - 1] == color && field[x + 2, y - 2] == color && field[x + 3, y - 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}

						// check lower left diagonal
						if (x >= 3 && y <= numRows - 4 && field[x - 1, y + 1] == color && field[x - 2, y + 2] == color && field[x - 3, y + 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}

						//check lower right diagonal
						if (x <= numColumns - 4 && y <= numRows - 4 && field[x + 1, y + 1] == color && field[x + 2, y + 2] == color && field[x + 3, y + 3] == color)
						{
							if (color == 1)
								blueWon = true;
							gameOver = true;
						}
					}
					yield return null;
				}

				yield return null;
			}

			// if Game Over update the winning text to show who has won
			if(gameOver == true)
			{
				winningText.GetComponent<TextMesh>().text = blueWon ? playerWonText : playerLoseText;
			}
			else 
			{
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if(!FieldContainsEmptyCell())
				{
					gameOver = true;
					winningText.GetComponent<TextMesh>().text = drawText;
				}
			}

			isCheckingForWinner = false;

			yield return 0;
		}

		void UpdatePlayAgainButton()
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
		}

		/// <summary>
		/// check if the field contains an empty cell
		/// </summary>
		/// <returns><c>true</c>, if it contains empty cell, <c>false</c> otherwise.</returns>
		bool FieldContainsEmptyCell()
		{
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					if(field[x, y] == (int)Piece.Empty)
						return true;
				}
			}
			return false;
		}
	}
}
