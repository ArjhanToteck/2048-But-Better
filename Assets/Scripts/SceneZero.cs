using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneZero : MonoBehaviour
{
	void Start()
	{
		// pings server to wake up if needed
		GetComponent<Leaderboard>().GetText();

		// loads SaveData from file
		SaveData.Load();

		// loads next scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}