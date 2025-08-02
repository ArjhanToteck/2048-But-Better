using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public void Classic()
	{
		SaveData.savedGame = new SaveData.GameSave(new long[4, 4], 0);

		OpenGame();
	}

	public void SixBySix()
	{
		SaveData.savedGame = new SaveData.GameSave(new long[6, 6], 0);

		OpenGame();
	}

	public void TimesTwo()
	{
		SaveData.savedGame = new SaveData.GameSave(new long[6, 6], 0);

		OpenGame();
	}

	void OpenGame()
	{
		SceneManager.LoadScene("Game");
	}
}
