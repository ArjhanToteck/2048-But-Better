using System;

[Serializable]
class Score
{
	public string name;
	public long score;

	public Score(string nameInput, long scoreInput)
	{
		name = nameInput;
		score = scoreInput;
	}

	public class ScoreArray
	{
		public Score[] items;
	}
}
