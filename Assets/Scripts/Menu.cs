using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public Button continueButton;

	// Start is called before the first frame update
	void Start()
	{
		if (!!continueButton && SaveData.savedGame.moves > 0)
		{
			continueButton.interactable = true;
		}
	}

	public void Home()
	{
		SceneManager.LoadScene("Menu");
	}

	public void Continue()
	{
		SceneManager.LoadScene("Game");
	}

	public void NewGame()
	{
		SaveData.savedGame = new SaveData.GameSave(new long[4, 4], 0);
		SceneManager.LoadScene("Game");
	}

	public void Credits()
	{
		SceneManager.LoadScene("Credits");
	}

	public void Leaderboard()
	{
		SceneManager.LoadScene("Leaderboard");
	}

	public void Exit()
	{
		Application.Quit();
	}
}
