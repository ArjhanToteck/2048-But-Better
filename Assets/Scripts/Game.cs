using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	[Header("GridData")]
	public int width;
	public int height;

	public GameObject[,] grid;


	[Header("GameObjects")]
	public GameObject floor;
	public Transform tilesParent;
	public GameObject tilePrefab;
	public TMP_Text scoreText;
	public TMP_Text highScoreText;
	public GameObject gameOverPanel;
	public GameObject worldRecordPanel;
	public AudioManager audioManager;
	public SwipeManager swipeManager;

	[Header("GameData")]
	public bool gameOver = false;
	public bool boardFull = false;

	public int moves = 0;

	private long score;
	public long Score
	{
		get
		{
			return score;
		}

		set
		{
			score = value;

			// checks if score is better than current best
			if (score > SaveData.highScore)
			{
				highScoreText.text = "Best: " + score;
			}
		}
	}

	private void Awake()
	{
		// loads settings
		width = SaveData.savedGame.gridSize[0];
		height = SaveData.savedGame.gridSize[1];

		highScoreText.text = "Best: " + SaveData.highScore;

		// creates grid with dimensions from loaded settings
		grid = new GameObject[height, width];
	}

	void Start()
	{
		LoadGame();

		if (moves == 0) RandomTile();
	}

	void LoadGame()
	{
		moves = SaveData.savedGame.moves;
		score = SaveData.savedGame.score;

		floor.transform.localScale = new Vector3(width, 0.1f, height);

		for (int y = height - 1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				if (SaveData.savedGame.grid[y, x] != 0)
				{
					// creates new tile
					GameObject newTile = Instantiate(tilePrefab, tilesParent, true);

					// spawns tile in random location that was found to be empty
					newTile.GetComponent<Tile>().position = new Vector2Int(x, y);
					newTile.transform.position = new Vector3((x + 0.5f) - width / 2f, 0, (y + 0.5f) - height / 2f);
					newTile.transform.localScale = Vector3.zero;

					newTile.GetComponent<Tile>().UpdateValue(false, (int)SaveData.savedGame.grid[y, x], false);

					// sets tile in local grid
					grid[y, x] = newTile;
				}
			}
		}

		AddToScore(0);
	}

	// Update is called once per frame
	void Update()
	{
		if (!gameOver)
		{
			bool tileMoved = false;
			bool keyPressed = false;

			// up
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || swipeManager.swipeDirection == SwipeManager.SwipeDirection.Up)
			{
				keyPressed = true;

				// marks all tiles as not recently merged
				MarkAllTilesUnmerged();

				// loops through rows
				for (int y = height - 1; y >= 0; y--)
				{
					// loops through columns
					for (int x = 0; x < width; x++)
					{
						// checks if tile isn't empty
						if (grid[y, x] != null)
						{
							// checks if tile above exists and is of same value and neither was recently merged
							if (y + 1 < height && grid[y, x].GetComponent<Tile>().value == grid[y + 1, x].GetComponent<Tile>().value && !grid[y, x].GetComponent<Tile>().recentlyMerged && !grid[y + 1, x].GetComponent<Tile>().recentlyMerged)
							{
								tileMoved = true;

								// marks tile above as recently merged
								grid[y + 1, x].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile up (and then to crash into tile below it)
								grid[y, x].GetComponent<Tile>().MoveUp(0, grid[y + 1, x]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[y, x] = null;

								// no need to check below tile if on bottom row
								if (y == 0) continue;
							}
							else
							{
								// continues as rest of code applies only to empty tiles
								continue;
							}
						}
						;

						// loops through tiles below
						int movedTiles = 0;

						for (int i = y - 1; i >= 0; i--)
						{
							// continues if empty tile
							if (grid[i, x] == null) continue;

							tileMoved = true;

							int targetY = y - movedTiles;

							// moves tile up to where it's supposed to go on grid
							grid[targetY, x] = grid[i, x];
							grid[i, x] = null;

							// checks if there is tile above and value of tile above is equal and neither tiles were recently merged
							if (targetY + 1 < height && grid[targetY + 1, x].GetComponent<Tile>().value == grid[targetY, x].GetComponent<Tile>().value && !grid[targetY, x].GetComponent<Tile>().recentlyMerged && !grid[targetY + 1, x].GetComponent<Tile>().recentlyMerged)
							{
								// marks tile above as recently merged
								grid[targetY + 1, x].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile down (and then to crash into tile below it)
								grid[targetY, x].GetComponent<Tile>().MoveUp(targetY - i, grid[targetY + 1, x]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[targetY, x] = null;
							}
							else
							{
								// triggers animation to move tile down with nothing under to collide with
								grid[targetY, x].GetComponent<Tile>().MoveUp(targetY - i);
								movedTiles++;
							}
						}
					}
				}
			}
			// down
			else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || swipeManager.swipeDirection == SwipeManager.SwipeDirection.Down)
			{
				keyPressed = true;

				// marks all tiles as not recently merged
				MarkAllTilesUnmerged();

				// loops through rows
				for (int y = 0; y < height; y++)
				{
					// loops through columns
					for (int x = 0; x < width; x++)
					{
						// checks if tile isn't empty
						if (grid[y, x] != null)
						{
							// checks if tile below exists and is of same value and neither tiles were recently merged
							if (y > 0 && grid[y, x].GetComponent<Tile>().value == grid[y - 1, x].GetComponent<Tile>().value && !grid[y, x].GetComponent<Tile>().recentlyMerged && !grid[y - 1, x].GetComponent<Tile>().recentlyMerged)
							{
								tileMoved = true;

								// marks tile below as recently merged
								grid[y - 1, x].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile down (and then to crash into tile below it)
								grid[y, x].GetComponent<Tile>().MoveDown(0, grid[y - 1, x]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[y, x] = null;

								// no need to check above tile if on top row
								if (y == height - 1) continue;
							}
							else
							{
								// continues as rest of code applies only to empty tiles
								continue;
							}
						}
						;

						// loops through tiles above
						int movedTiles = 0;

						for (int i = y + 1; i < height; i++)
						{
							// continues if empty tile
							if (grid[i, x] == null) continue;

							tileMoved = true;

							int targetY = y + movedTiles;

							// moves tile down to where it's supposed to go on grid
							grid[targetY, x] = grid[i, x];
							grid[i, x] = null;

							// checks if there is tile underneath and value of tile underneath is equal and neither tiles were recently merged
							if (targetY - 1 >= 0 && grid[targetY - 1, x].GetComponent<Tile>().value == grid[targetY, x].GetComponent<Tile>().value && !grid[targetY, x].GetComponent<Tile>().recentlyMerged && !grid[targetY - 1, x].GetComponent<Tile>().recentlyMerged)
							{
								// marks tile below as recently merged
								grid[targetY - 1, x].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile down (and then to crash into tile below it)
								grid[targetY, x].GetComponent<Tile>().MoveDown(i - targetY, grid[targetY - 1, x]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[targetY, x] = null;
							}
							else
							{
								// triggers animation to move tile down with nothing under to collide with
								grid[targetY, x].GetComponent<Tile>().MoveDown(i - targetY);
								movedTiles++;
							}
						}
					}
				}
			}
			// left
			else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || swipeManager.swipeDirection == SwipeManager.SwipeDirection.Left)
			{
				keyPressed = true;

				// marks all tiles as not recently merged
				MarkAllTilesUnmerged();

				// loops through rows
				for (int y = 0; y < height; y++)
				{
					// loops through columns
					for (int x = 0; x < width; x++)
					{
						// checks if tile isn't empty
						if (grid[y, x] != null)
						{
							// checks if tile to left exists and is of same value and neither tiles were recently merged
							if (x > 0 && grid[y, x].GetComponent<Tile>().value == grid[y, x - 1].GetComponent<Tile>().value && !grid[y, x].GetComponent<Tile>().recentlyMerged && !grid[y, x - 1].GetComponent<Tile>().recentlyMerged)
							{
								tileMoved = true;

								// marks tile to left as recently merged
								grid[y, x - 1].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile to left (and then to crash into tile left of it)
								grid[y, x].GetComponent<Tile>().MoveLeft(0, grid[y, x - 1]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[y, x] = null;

								// no need to check right tile if on last column to right
								if (x == width - 1) continue;
							}
							else
							{
								// continues as rest of code applies only to empty tiles
								continue;
							}
						}
						;

						// loops through tiles to right
						int movedTiles = 0;

						for (int i = x + 1; i < height; i++)
						{
							// continues if empty tile
							if (grid[y, i] == null) continue;

							tileMoved = true;

							int targetX = x + movedTiles;

							// moves tile down to where it's supposed to go on grid
							grid[y, targetX] = grid[y, i];
							grid[y, i] = null;

							// checks if there is tile underneath and value of tile underneath is equal and neither tiles were recently merged
							if (targetX - 1 >= 0 && grid[y, targetX - 1].GetComponent<Tile>().value == grid[y, targetX].GetComponent<Tile>().value && !grid[y, targetX].GetComponent<Tile>().recentlyMerged && !grid[y, targetX - 1].GetComponent<Tile>().recentlyMerged)
							{
								// marks tile to left as recently merged
								grid[y, targetX - 1].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile down (and then to crash into tile below it)
								grid[y, targetX].GetComponent<Tile>().MoveLeft(i - targetX, grid[y, targetX - 1]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[y, targetX] = null;
							}
							else
							{
								// triggers animation to move tile down with nothing under to collide with
								grid[y, targetX].GetComponent<Tile>().MoveLeft(i - targetX);
								movedTiles++;
							}
						}
					}
				}
			}
			// right
			else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || swipeManager.swipeDirection == SwipeManager.SwipeDirection.Right)
			{
				keyPressed = true;

				// marks all tiles as not recently merged
				MarkAllTilesUnmerged();

				// loops through rows
				for (int y = 0; y < height; y++)
				{
					// loops through columns
					for (int x = width - 1; x >= 0; x--)
					{
						// checks if tile isn't empty
						if (grid[y, x] != null)
						{
							// checks if tile above exists and is of same value and neither was recently merged
							if (x + 1 < width && grid[y, x + 1] != null && grid[y, x].GetComponent<Tile>().value == grid[y, x + 1].GetComponent<Tile>().value && !grid[y, x].GetComponent<Tile>().recentlyMerged && !grid[y, x + 1].GetComponent<Tile>().recentlyMerged)
							{
								tileMoved = true;

								// marks tile above as recently merged
								grid[y, x + 1].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile up (and then to crash into tile below it)
								grid[y, x].GetComponent<Tile>().MoveRight(0, grid[y, x + 1]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[y, x] = null;

								// no need to check below tile if on bottom row
								if (x == 0) continue;
							}
							else
							{
								// continues as rest of code applies only to empty tiles
								continue;
							}
						}
						;

						// loops through tiles below
						int movedTiles = 0;

						for (int i = x - 1; i >= 0; i--)
						{
							// continues if empty tile
							if (grid[y, i] == null) continue;

							tileMoved = true;

							int targetX = x - movedTiles;

							// moves tile up to where it's supposed to go on grid
							grid[y, targetX] = grid[y, i];
							grid[y, i] = null;

							// checks if there is tile above and value of tile above is equal and neither tiles were recently merged
							if (targetX + 1 < width && grid[y, targetX + 1] != null && grid[y, targetX + 1].GetComponent<Tile>().value == grid[y, targetX].GetComponent<Tile>().value && !grid[y, targetX].GetComponent<Tile>().recentlyMerged && !grid[y, targetX + 1].GetComponent<Tile>().recentlyMerged)
							{
								// marks tile above as recently merged
								grid[y, targetX + 1].GetComponent<Tile>().recentlyMerged = true;

								// triggers animation to move tile down (and then to crash into tile below it)
								grid[y, targetX].GetComponent<Tile>().MoveRight(targetX - i, grid[y, targetX + 1]);

								// removes tile from grid (since it will be merged with the underneath tile)
								grid[y, targetX] = null;
							}
							else
							{
								// triggers animation to move tile down with nothing under to collide with
								grid[y, targetX].GetComponent<Tile>().MoveRight(targetX - i);
								movedTiles++;
							}
						}
					}
				}
			}

			if (keyPressed) CheckForGameOver();

			if (tileMoved)
			{
				moves++;
				RandomTile();
				audioManager.ClickSound();
			}
		}
	}

	/*void StopAllMovements()
    {
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// checks if an cell has a tile
				if (grid[y, x] != null)
				{
					// ends the tile's movement
					grid[y, x].GetComponent<Tile>().stopMoving = true;
					break;
				}
			}
		}
	}*/

	void RandomTile()
	{
		// doesn't do anything if game over
		if (gameOver) return;

		CheckForGameOver();

		if (!boardFull)
		{
			Vector2Int randomCoords;

			// repeats until it finds a tile that is empty
			do
			{
				// gets random coordinates
				randomCoords = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
			}
			while (grid[randomCoords.y, randomCoords.x] != null);

			// creates new tile
			GameObject newTile = Instantiate(tilePrefab, tilesParent, true);

			// spawns tile in random location that was found to be empty
			newTile.GetComponent<Tile>().position = randomCoords;
			newTile.transform.position = new Vector3((randomCoords.x + 0.5f) - width / 2f, 0, (randomCoords.y + 0.5f) - height / 2f);
			newTile.transform.localScale = Vector3.zero;

			// adds tile to grid
			grid[randomCoords.y, randomCoords.x] = newTile;
		}

		if (!gameOver) CheckForGameOver();
	}

	void CheckForGameOver()
	{
		// checks if space is left on board for any more tiles
		bool spaceLeft = false;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				// checks if an empty cell was found
				if (grid[y, x] == null)
				{
					// declares there is space left and exits loop
					spaceLeft = true;
					break;
				}
			}

			if (spaceLeft) break;
		}

		boardFull = !spaceLeft;

		// checks if no empty tiles left
		if (!spaceLeft)
		{
			bool possibleMoves = false;

			// loops through tiles
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					long value = grid[y, x].GetComponent<Tile>().value;

					// checks if an equally valued tile is adjacent to current tile
					if ((y + 1 < height && grid[y + 1, x].GetComponent<Tile>().value == value) || (y > 0 && grid[y - 1, x].GetComponent<Tile>().value == value) || (x + 1 < width && grid[y, x + 1].GetComponent<Tile>().value == value) || (x > 0 && grid[y, x - 1].GetComponent<Tile>().value == value))
					{
						// declares there is space left and exits loop
						possibleMoves = true;
						break;
					}
				}

				if (possibleMoves) break;
			}

			Debug.Log(possibleMoves);
			if (!possibleMoves)
			{
				GameOver();
				Debug.Log("game over");
			}
		}
	}

	void MarkAllTilesUnmerged()
	{
		for (int y = height - 1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				if (grid[y, x] != null) grid[y, x].GetComponent<Tile>().recentlyMerged = false;
			}
		}
	}

	SaveData.GameSave GetGameSave()
	{
		long[,] numberGrid = new long[height, width];

		for (int y = height - 1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				numberGrid[y, x] = grid[y, x] == null ? 0 : grid[y, x].GetComponent<Tile>().value;
			}
		}

		return new SaveData.GameSave(numberGrid, score, moves);
	}

	void GameOver()
	{
		SaveData.savedGame.score = score;
		Debug.Log(score);

		// marks game as finished
		gameOver = true;

		List<SaveData.GameSave> bestGamesList = new List<SaveData.GameSave>(SaveData.bestGames);

		// loops through best games in reverse order
		for (int i = 9; i >= 0; i--)
		{
			// checks if either no game or lower score in current game
			if (SaveData.bestGames[i] == null || SaveData.bestGames[i].score < score)
			{
				// inserts game
				bestGamesList.Insert(i, GetGameSave());

				// removes worst game in list
				bestGamesList.RemoveAt(0);

				// saves best game data
				SaveData.bestGames = bestGamesList.ToArray();

				break;
			}
		}

		// checks for world record
		GetComponent<Leaderboard>().CheckWorldRecord(rank =>
		{
			Debug.Log(rank);

			if (rank != -1)
			{
				worldRecordPanel.SetActive(true);
				worldRecordPanel.transform.Find("Description").GetComponent<TMP_Text>().text = $"You got a world record at #{rank} in the world! Enter your name to get on the leaderboard.";

				worldRecordPanel.transform.Find("Skip").GetComponent<Button>().onClick.AddListener(ShowGameOverPanelDefault);
				worldRecordPanel.transform.Find("Continue").GetComponent<Button>().onClick.AddListener(SaveScore);

				void SaveScore()
				{
					GetComponent<Leaderboard>().SaveWorldrecord(worldRecordPanel.transform.Find("Input").GetComponent<TMP_InputField>().text.Substring(0, Mathf.Min(20, worldRecordPanel.transform.Find("Input").GetComponent<TMP_InputField>().text.Length)));
					ShowGameOverPanel($"You ranked #{rank} in the world. Congratulations!");
				}
			}
			else
			{
				ShowGameOverPanel();
			}

			void ShowGameOverPanelDefault()
			{
				ShowGameOverPanel();
			}

			void ShowGameOverPanel(string customText = null)
			{
				// shows game over panel
				gameOverPanel.SetActive(true);
				gameOverPanel.transform.Find("Score").GetComponent<TMP_Text>().text = $"Score: {score}\nBest: {SaveData.highScore}";

				if (customText != null)
				{
					gameOverPanel.transform.Find("Description").GetComponent<TMP_Text>().text = customText;
				}
				else if (SaveData.savedGame.score == SaveData.highScore)
				{
					gameOverPanel.transform.Find("Description").GetComponent<TMP_Text>().text = "You were so close to beating your high score! Better luck next time.";
				}
				else if (SaveData.savedGame.score > SaveData.highScore)
				{
					gameOverPanel.transform.Find("Description").GetComponent<TMP_Text>().text = "You beat your high score!";
				}
				else
				{
					gameOverPanel.transform.Find("Description").GetComponent<TMP_Text>().text = "You didn't beat your high score, but it's alright. I'm still proud of you.";
				}

				// removes saved game
				SaveData.savedGame = null;
			}
		});


	}

	public void AddToScore(long value)
	{
		score += value;
		scoreText.text = "Score: " + Score;
	}

	string BoardToText()
	{
		string output = "";

		for (int y = height - 1; y >= 0; y--)
		{
			for (int x = 0; x < width; x++)
			{
				output += grid[y, x] == null ? 0 : grid[y, x].GetComponent<Tile>().value;
				output += "|";
			}

			output += "\n_______\n";
		}

		return output;
	}

	public void Home()
	{
		SceneManager.LoadScene("Menu");
	}

	void OnSceneUnload()
	{
		SaveData.savedGame = GetGameSave();
		SaveData.Save();
	}

	void OnDestroy()
	{
		if (!gameObject.scene.isLoaded)
		{
			OnSceneUnload();
		}
	}

	void OnApplicationQuit()
	{
		OnSceneUnload();
	}
}
