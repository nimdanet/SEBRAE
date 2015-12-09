using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HUDController : MonoBehaviour 
{
	#region Action
	public static event Action OnPassWeek;
	#endregion

	public UISprite fameFill;
	public UILabel fameLabel;

	public UILabel moneyLabel;
	public UILabel upkeepLabel;
	public UILabel profitLabel;
	public UILabel fameProfitLabel;

	public UILabel proximoMesLabel;
	public UILabel currentMesLabel;
	public Transform pagina;
	private List<UISprite> weeks;

	public GameObject conflictDeck;

	#region singleton
	private static HUDController instance;
	public static HUDController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<HUDController>();
	
			return instance;
		}
	}
	#endregion

	void OnDestroy()
	{
		GameController.OnFameChanged -= FameUpdated;
		GameController.OnMoneyChanged -= MoneyUpdated;
		GameController.OnUpkeepChanged -= UpkeepUpdated;
		GameController.OnMoneyProfitChanged -= MoneyProfitUpdated;
		GameController.OnFameProfitChanged -= FameProfitUpdated;
		
		GameController.OnGameLoaded -= GameLoaded;
	}

	void Awake()
	{
		GameController.OnFameChanged += FameUpdated;
		GameController.OnMoneyChanged += MoneyUpdated;
		GameController.OnUpkeepChanged += UpkeepUpdated;
		GameController.OnMoneyProfitChanged += MoneyProfitUpdated;
		GameController.OnFameProfitChanged += FameProfitUpdated;

		GameController.OnGameLoaded += GameLoaded;

		weeks = new List<UISprite>();
		for(int i = 1; i <= 4; i++)
		{
			UISprite week = pagina.FindChild("semana" + i).GetComponent<UISprite>();
			week.fillAmount = 0;
			weeks.Add(week);
		}
	}

	void Start()
	{
		proximoMesLabel.text = Localization.Get("MES_CALENDARIO") + " " + (GameController.Month + 1);
		currentMesLabel.text = Localization.Get("MES_CALENDARIO") + " " + GameController.Month;

		conflictDeck.SetActive(false);
	}

	private void GameLoaded()
	{
		proximoMesLabel.text = Localization.Get("MES_CALENDARIO") + " " + (GameController.Month + 1);
		currentMesLabel.text = Localization.Get("MES_CALENDARIO") + " " + GameController.Month;

		Debug.Log(GameController.Week);
		for(int i = 0; i < weeks.Count; i++)
		{
			UISprite w = weeks[i];
			Debug.Log(i < (GameController.Week - 1));
			w.fillAmount = (i < GameController.Week - 1) ? 1 : 0;
		}
	}

	private void FameUpdated()
	{
		fameFill.fillAmount = Mathf.Max((float)(GameController.Fame) / (float)(GameController.MaxFame), 0f);
		fameLabel.text = GameController.Fame.ToString();
	}

	private void MoneyUpdated()
	{
		moneyLabel.text = "x " + GameController.Money.ToString();
	}

	private void UpkeepUpdated()
	{
		upkeepLabel.text = "x " + GameController.Upkeep.ToString();
	}

	private void MoneyProfitUpdated()
	{
		profitLabel.text = "x " + GameController.MoneyProfit.ToString();
	}

	private void FameProfitUpdated()
	{
		fameProfitLabel.text = "x " + GameController.FameProfit.ToString();
	}

	public void PassWeek()
	{
		if(DeckController.CardsInHand > GameController.MaxCardsInHand)
		{
			Popup.ShowOk(string.Format(Localization.Get("LIMITE_MAO"), GameController.MaxCardsInHand));
			return;
		}

		if(GameController.activeCardEffect != EffectCard.EffectType.None)
			return;

		if(Popup.IsActive || ConflictCard.IsActive)
			return;

		StartCoroutine(RiskWeek());

		if(OnPassWeek != null)
			OnPassWeek();
	}

	public IEnumerator RiskWeek()
	{
		UISprite week = weeks[GameController.Week - 1];

		while(week.fillAmount < 1)
		{
			week.fillAmount += Time.deltaTime;
			yield return null;
		}

		GameController.Instance.NextWeek();

		if(GameController.Week > 4)
		{
			GameController.Instance.NextMonth();

			TweenPosition tween = pagina.GetComponent<TweenPosition>();
			tween.ResetToBeginning();
			tween.PlayForward();

			yield return new WaitForSeconds(tween.duration);

			foreach(UISprite w in weeks)
				w.fillAmount = 0;

			pagina.transform.localPosition = tween.from;
			currentMesLabel.text = Localization.Get("MES_CALENDARIO") + " " + (GameController.Month - 0);
			proximoMesLabel.text = Localization.Get("MES_CALENDARIO") + " " + (GameController.Month + 1);
		}
	}

	public void ShowConflictCard()
	{
		conflictDeck.SetActive(true);
	}

	public void HideConflictCard()
	{
		conflictDeck.SetActive(false);
	}

	public void Save()
	{
		SaveController.Save();
	}

	public void Load()
	{
		SaveController.Load();
	}
}
