using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UserService : MonoBehaviour 
{
	public string userName;
	public string userEmail;
	public string urlServidor;
	public int highscore;

	public void Awake() 
	{
		//Application.ExternalCall("enviarDadosJogo", "Login Service");
	}

	public void SetUserData(string info) 
	{
		string[] dados = info.Split(';');
		userName = dados[0];
		userEmail = dados[1];
		urlServidor = dados[2];
	}

	public void SetHighScore(int score)
	{
		highscore = score;
	}

	public void CallSendScore()
	{
		StartCoroutine("sendScore");
	}

	public IEnumerator sendScore() 
	{
		string json = "{\"gameName\": \"RESTORAMA\", \"highscore\": "+ highscore + ",\"metaData\": \"\", \"user\": {\"userName\": \"" + userName + "\", \"userEmail\": \"" + userEmail + "\"} }";

		Dictionary<string, string> hash = new Dictionary<string, string>();
		hash["Content-Type"] = "application/json";
		byte[] pData = Encoding.ASCII.GetBytes(json.ToCharArray());
		WWW w = new WWW(urlServidor + "/rest/game/sendhighscore", pData, hash);
		while (!w.isDone) {
			yield return null;
		}
		if (w.error != null || w.text != null) {
			Application.ExternalCall("sendScoreCallback", w.error != null ? w.error : w.text);
		}
		Debug.Log(w.error);
	}
}