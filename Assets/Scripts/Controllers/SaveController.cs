using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MiniJSON;

public class SaveController : MonoBehaviour 
{
	public int timeToSave = 1800;
	public int timeout = 5;

	public static bool loadFromSave;

	private static GameData _gameData;
	private static UserService _userService;

	#region Get / Set
	public static bool HasSavedData
	{
		get { return _gameData.hasSavedGame; }
	}

	public static bool HasWon
	{
		get { return _gameData.hasWon; }
	}

	public static List<ConstructionsSaved> ConstructionsSaved
	{
		get { return _gameData.constructions; }
	}

	public static List<CardsSaved> CardsSaved
	{
		get { return _gameData.cards; }
	}

	public static List<EffectSaved> EffectsSaved
	{
		get { return _gameData.effects; }
	}

	public static int Money
	{
		get { return _gameData.money; }
	}
	
	public static int Fame
	{
		get { return _gameData.fame; }
	}

	public static int Upkeep
	{
		get { return _gameData.upkeep; }
	}
	
	public static int FameReward
	{
		get { return _gameData.fameReward; }
	}

	public static int MoneyReward
	{
		get { return _gameData.moneyReward; }
	}

	public static int Week
	{
		get { return _gameData.week; }
	}
	
	public static int Month
	{
		get { return _gameData.month; }
	}

	public static ConflictCard.SpecialEffect ActiveConflictEffect
	{
		get { return _gameData.activeConflictEffect; }
	}

	public static float MoneyMultiplier
	{
		get { return _gameData.moneyMultiplier; }
	}

	public static float FameMultiplier
	{
		get { return _gameData.fameMultiplier; }
	}

	#endregion

	#region singleton
	private static SaveController instance;
	public static SaveController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<SaveController>();

			return instance;
		}
	}
	#endregion

	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}
		
		DontDestroyOnLoad (gameObject);

		StartCoroutine(Save(timeToSave));
	}

	// Use this for initialization
	void Start () 
	{
		_gameData = GetComponent<GameData>();
		_userService = GetComponentInChildren<UserService>();
	}

	public static void SetUser(string name, string email)
	{
		_userService.SetUserData(name + ";" + email + ";" + _userService.urlServidor);
	}

	private static void UpdateValues()
	{
		_gameData.hasSavedGame = true;
		_gameData.money = GameController.Money;
		_gameData.fame = GameController.Fame;
		_gameData.cardsInHand = DeckController.CardsInHand;
		_gameData.week = GameController.Week;
		_gameData.month = GameController.Month;
		_gameData.upkeep = GameController.Upkeep;
		_gameData.fameReward = GameController.FameProfit;
		_gameData.moneyReward = GameController.MoneyProfit;
		_gameData.hasWon = GameController.alreadyWon;
		_gameData.activeConflictEffect = GameController.ActiveConflictEffect;
		_gameData.moneyMultiplier = GameController.moneyMultiplier;
		_gameData.fameMultiplier = GameController.fameMultiplier;

		_gameData.constructions = new List<ConstructionsSaved>();
		foreach(ConstructionArea cArea in GameController.ConstructionAreas)
		{
			if(cArea.constructionCard == null) continue;

			ConstructionsSaved cSaved = new ConstructionsSaved();
			cSaved.name = cArea.constructionCard.nome;
			cSaved.level = cArea.constructionCard.level;
			cSaved.cooldown = cArea.constructionCard.cooldown;
			cSaved.row = cArea.row;
			cSaved.column = cArea.column;

			_gameData.constructions.Add(cSaved);
		}

		_gameData.constructionsActive = _gameData.constructions.Count;

		_gameData.effects = new List<EffectSaved>();
		foreach(EffectArea eArea in GameController.EffectAreas)
		{
			if(eArea.effectCard == null) continue;

			EffectSaved eSaved = new EffectSaved();
			eSaved.name = eArea.effectCard.nome;
			eSaved.cooldown = eArea.effectCard.cooldown;
			eSaved.position = eArea.position;

			_gameData.effects.Add(eSaved);
		}
		_gameData.effectsActive = _gameData.effects.Count;

		_gameData.cards = new List<CardsSaved>();
		foreach(Card card in DeckController.cardsInHand)
		{
			CardsSaved cardSaved = new CardsSaved();
			cardSaved.name = card.nome;

			if(card.cardType == Card.Type.Construction)
			{
				ConstructionCard constructionCard = (ConstructionCard)card;
				cardSaved.level = constructionCard.level;
			}

			_gameData.cards.Add(cardSaved);
		}

	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.S))
			StartCoroutine(Save ());
		
		if(Input.GetKeyDown(KeyCode.L))
			StartCoroutine(Load ());

		if(Input.GetKeyDown(KeyCode.E))
			Erase ();
	}

	private static IEnumerator Save(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		if(Application.loadedLevelName == "Gameplay")
			Instance.StartCoroutine(Save ());

		Instance.StartCoroutine(Save (Instance.timeToSave));
	}

	public static IEnumerator Save()
	{
		UpdateValues();

		string metadata = "";

		#region local save
		try 
		{
			//Get a binary formatter
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			//Create an in memory stream
			MemoryStream memoryStram = new MemoryStream();
			//Save the scores
			binaryFormatter.Serialize(memoryStram, _gameData);
			//Add it to player prefs
			metadata = Convert.ToBase64String(memoryStram.GetBuffer());
			PlayerPrefs.SetString("metadata", metadata);
			//Debug.Log(Data Saved);
		}
		catch (Exception ex)
		{
			Debug.LogError("SaveDataException :"+ex.ToString());
		}

		PlayerPrefs.Save();
		#endregion

		#region server save
		string json = "{\"gameName\": \"RESTORAMA\", \"highscore\": "+ _userService.highscore + ",\"metaData\": " + metadata + ", \"user\": {\"userName\": \"" + _userService.userName + "\", \"userEmail\": \"" + _userService.userEmail + "\"} }";
		Debug.Log("Save json: " + json);

		Dictionary<string, string> hash = new Dictionary<string, string>();
		hash["Content-Type"] = "application/json";
		byte[] pData = Encoding.ASCII.GetBytes(json.ToCharArray());
		WWW w = new WWW(_userService.urlServidor + "/rest/game/sendhighscore", pData, hash);
		float time = 0;
		while (!w.isDone && time < Instance.timeout) 
		{
			time += Time.deltaTime;
			yield return null;
		}

		if(time > Instance.timeout)
		{
			w.Dispose();
			Debug.Log("Error while saving: timeout");
		}
		else
		{
			if (w.error != null || w.text != null) 
			{
				//Application.ExternalCall("sendScoreCallback", w.error != null ? w.error : w.text);
			}

			if(w.error != null)
				Debug.Log("Error while saving: " + w.error);
			else
				Debug.Log("Game saved succesfully");
		}
		#endregion
	}

	public static IEnumerator Load()
	{
		string data = "";

		#region server save
		string json = "{\"gameName\": \"RESTORAMA\", \"user\": {\"userName\": \"" + _userService.userName + "\", \"userEmail\": \"" + _userService.userEmail + "\"} }";
		Debug.Log("Load json: " + json);

		Dictionary<string, string> hash = new Dictionary<string, string>();
		hash["Content-Type"] = "application/json";
		byte[] pData = Encoding.ASCII.GetBytes(json.ToCharArray());
		WWW w = new WWW(_userService.urlServidor + "/rest/game/gethighscore", pData, hash);
		Debug.Log(w.url);
		float time = 0;
		while (!w.isDone && time < Instance.timeout) 
		{
			time += Time.deltaTime;
			yield return null;
		}

		if(time > Instance.timeout)
		{
			w.Dispose();
			Debug.Log("Error while loading: timeout");
		}
		else
		{
			if (w.error != null || w.text != null) 
			{
				//Application.ExternalCall("sendScoreCallback", w.error != null ? w.error : w.text);
			}
			
			if(w.error != null)
				Debug.Log("Error while loading: " + w.error);
			else
			{
				Debug.Log("Game successful loaded");
				Dictionary<string, object> loadedData = MiniJSON.Json.Deserialize(w.text) as Dictionary<string, object>; 
				foreach(KeyValuePair<string, object> kPair in loadedData)
					Debug.Log(kPair.Key + ": " + kPair.Value);

				data = loadedData["metadata"].ToString();
			}
		}
		#endregion

		if(String.IsNullOrEmpty(data))
		{
			Debug.Log("Loading locally...");
		 	data = PlayerPrefs.GetString("metadata", "null");
		}

		//If not blank then load it
		if(!data.Equals("null"))
		{
			try 
			{
				//Binary formatter for loading back
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				//Create a memory stream with the data
				MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(data));
				//Load back the scores
				GameData loadedData = (GameData)binaryFormatter.Deserialize(memoryStream);

				_gameData.hasSavedGame = loadedData.hasSavedGame;
				_gameData.fame = loadedData.fame;
				_gameData.money = loadedData.money;
				_gameData.upkeep = loadedData.upkeep;
				_gameData.moneyReward = loadedData.moneyReward;
				_gameData.fameReward = loadedData.fameReward;
				_gameData.week = loadedData.week;
				_gameData.month = loadedData.month;
				_gameData.cardsInHand = loadedData.cardsInHand;
				_gameData.constructionsActive = loadedData.constructionsActive;
				_gameData.constructions = loadedData.constructions;
				_gameData.effectsActive = loadedData.effectsActive;
				_gameData.effects = loadedData.effects;
				_gameData.cards = loadedData.cards;
				_gameData.hasWon = loadedData.hasWon;
				_gameData.activeConflictEffect = loadedData.activeConflictEffect;
				_gameData.moneyMultiplier = loadedData.moneyMultiplier;
				_gameData.fameMultiplier = loadedData.fameMultiplier;
			} 
			catch (Exception ex) 
			{
				Debug.LogError("MetaData read error :"+ ex.ToString());
			}
		}
	}

	public static void Erase()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}

	void OnApplicationQuit()
	{
		StartCoroutine(Save ());
	}
}
