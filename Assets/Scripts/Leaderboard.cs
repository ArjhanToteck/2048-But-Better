using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public partial class Leaderboard : MonoBehaviour
{
    const string server = "https://arjhantoteck.vercel.app/api/projects/2048ButBetterServer/";

    public bool showLeaderboard = false;

    // Start is called before the first frame update
    void Start()
    {
        if (showLeaderboard)
        {
            try
            {
                // get leaderboard from server
                StartCoroutine(GetText(server + "getLeaderboard", "", output =>
                {

                    // parse scores
                    Score[] scores = JsonUtility.FromJson<Score.ScoreArray>(output).items;
                    Debug.Log(scores);

                    // render scores
                    GetComponent<TMP_Text>().text = "";

                    for (int i = 0; i < scores.Length; i++)
                    {
                        Debug.Log($"{i + 1}. {scores[i].name}: {scores[i].score}\n");
                        GetComponent<TMP_Text>().text += $"{i + 1}. {scores[i].name}: {scores[i].score}\n";
                    }
                }));
            }
            catch
            {
                Debug.LogError("Error getting leaderboard from server.");
            }
        }
    }

    public void CheckWorldRecord(Action<int> callback)
    {
        // score of 0 skipped
        if (SaveData.savedGame.score <= 0)
        {
            Debug.Log("nah");
            callback?.Invoke(-1);
        }

        try
        {
            // check rank of current score
            StartCoroutine(GetText(server + "getRank", JsonUtility.ToJson(new Score("", SaveData.savedGame.score)), output =>
            {
                // default of -1 means not on leaderboard at all
                int rank = -1;
                Debug.Log(output);

                try
                {
                    rank = int.Parse(output);
                }
                catch
                {
                    Debug.LogError("Error checking score.");
                }

                callback?.Invoke(rank);
            }));
        }
        catch
        {
            // return default
            callback?.Invoke(-1);
        }
    }

    public void SaveWorldrecord(string name)
    {
        try
        {
            StartCoroutine(GetText(server + "sendScore", JsonUtility.ToJson(new Score(name, SaveData.savedGame.score)), output => { }));
        }
        catch
        {
            Debug.LogError("Error sending record to server.");
        }
    }

    public IEnumerator GetText(string url = server, string body = "", Action<string> callback = null)
    {
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();

        if (body != "")
        {
            // convert string body to raw JSON bytes
            byte[] jsonBytes = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.SetRequestHeader("Content-Type", "application/json");
        }

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            callback?.Invoke(response);
        }
    }
}
