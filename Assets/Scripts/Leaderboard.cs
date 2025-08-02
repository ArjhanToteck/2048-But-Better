using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboard : MonoBehaviour
{
    const string server = "https://2048-but-better-server.arjhantoteck.repl.co/";

    public bool showLeaderboard = false;

    // Start is called before the first frame update
    void Start()
    {
		if (showLeaderboard)
		{
            try
            {
                StartCoroutine(GetText(server + "getLeaderboard", "", output => {
                    Score[] scores = JsonUtility.FromJson<ArrayWrapper>(output).array;

                    GetComponent<TMP_Text>().text = "";

                    for(int i = 0; i < scores.Length; i++)
                    {
                        GetComponent<TMP_Text>().text += $"{i + 1}. {scores[i].name}: {scores[i].score}\n";
                    }
                }));
            }
            catch { }
        }
    }

    public void CheckWorldRecord(Action<int> onMessageReceived)
	{
        try
        {
            StartCoroutine(GetText(server + "checkScore", JsonUtility.ToJson(new Score("", SaveData.savedGame.score, true)), output => {
                int rank = -1;
                Debug.Log(output);

                try
				{
                    rank = int.Parse(output);
				}
                catch
                {
                    onMessageReceived(-1);
                }

                onMessageReceived(rank);
            }));
        }
        catch
        {
            onMessageReceived(-1);
        }
    }

    public void SaveWorldrecord(string name)
    {
        try
        {
            StartCoroutine(GetText(server + "sendScore", JsonUtility.ToJson(new Score(name, SaveData.savedGame.score, true)), output => {}));
        }
        catch { }
    }

    public IEnumerator GetText(string url = server, string body = "", Action<string> onMessageReceived = null)
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, body);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            if(onMessageReceived != null) onMessageReceived(response);
        }
    }

    [Serializable]
    class Score
	{
        public string name;
        public long score;
        public bool set;

		public Score(string nameInput, long scoreInput, bool setInput)
		{
            name = nameInput;
            score = scoreInput;
            set = setInput;
		}
	}

    [Serializable]
    class ArrayWrapper
	{
        public Score[] array;
	}
}
