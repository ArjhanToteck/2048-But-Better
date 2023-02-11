using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveData
{
	public static readonly string path = Application.persistentDataPath + "/saveData.gameSetup";
	public static GameSave savedGame = new GameSave(new long[4, 4], 0);
	public static GameSave[] bestGames = new GameSave[10];
	public static Settings settings = new Settings();

	[System.Serializable]
	public class Settings
	{
		public static float tileSpeed = 1;
	}

	public static long highScore
	{
		get
		{
			return bestGames[9] == null ? 0: bestGames[9].score;
		}
	}

	[System.Serializable]
	public class GameSave
	{
		public int[] gridSize;

		public long[,] grid;
		public long score;
		public int moves;

		public GameSave(long[,] gridInput, long scoreInput = 0, int movesInput = 0)
		{
			score = scoreInput;
			grid = gridInput;
			gridSize = new int[] { grid.GetLength(1), grid.GetLength(0) };
			moves = movesInput;
		}
	}

	public static void Load()
	{
		BinaryFormatter formatter = new BinaryFormatter();

		if (File.Exists(path))
		{
			FileStream stream = new FileStream(path, FileMode.Open);
			SaveDataSerializable data = formatter.Deserialize(stream) as SaveDataSerializable;
			stream.Close();

			savedGame = data.savedGame;
			bestGames = data.bestGames;
			settings = data.settings;
		}
		else
		{
			Save();
		}
	}

	public static void Save()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(path, FileMode.Create);

		formatter.Serialize(stream, new SaveDataSerializable());
		stream.Close();
	}
}

[System.Serializable]
public class SaveDataSerializable
{
	public SaveData.GameSave savedGame = SaveData.savedGame;
	public SaveData.GameSave[] bestGames = SaveData.bestGames;
	public SaveData.Settings settings = SaveData.settings;
}