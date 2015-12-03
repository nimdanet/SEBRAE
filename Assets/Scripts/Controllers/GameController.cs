using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour 
{
	#region Action
	public static event Action OnMoneyChanged;
	public static event Action OnFameChanged;
	public static event Action OnUpkeepChanged;
	public static event Action OnMoneyProfitChanged;
	public static event Action OnFameProfitChanged;

	public static event Action OnWeekChanged;
	public static event Action OnMonthChanged;
	#endregion

	public int initialMoney;
	public int initialFame;
	public int minFame;
	public int maxFame;
	public int initialCards;
	public int maxCardsPerTurn;
	public int maxConstructionCardsPerTurn;
	public int maxEffectsCardsPerTurn;
	public int cardsToDrawPerWeek;
	public int cardsToDrawPerMonth;
	public int maxCardsInHand;
	public FameDecrease[] fameDecrease;

	private static int currentFame;
	private static int currentMoney;
	private static int currentUpkeep;
	private static int currentMoneyProfit;
	private static int currentFameProfit;

	private static int currentWeek;
	private static int currentMonth;
	private static int constructionCardsPlayedThisTurn;
	private static int effectCardsPlayedThisTurn;

	private static float bonusMoney;
	private static float bonusFame;
	private static int cozinhaUpkeepBonus;
	#region conflict effects
	private static ConflictCard.SpecialEffect activeConflictEffect;
	public static float moneyMultiplier;
	public static float fameMultiplier;
	#endregion

	#region card effects
	public static EffectCard.EffectType activeCardEffect;
	public static float effectCardValue;
	#endregion

	private ConstructionArea[] constructionAreas;

	#region get / set
	public static ConstructionArea[] ConstructionAreas
	{
		get { return Instance.constructionAreas; }
	}

	public static int Money
	{
		get { return currentMoney; }

		set
		{
			currentMoney = value;

			if(OnMoneyChanged != null)
				OnMoneyChanged();
		}
	}

	public static int Fame
	{
		get { return currentFame; }
		
		set
		{
			currentFame = Mathf.Min(value, 100);
			
			if(OnFameChanged != null)
				OnFameChanged();
		}
	}

	public static int MoneyProfit
	{
		get { return currentMoneyProfit; }
		
		set
		{
			currentMoneyProfit = value;
			
			if(OnMoneyProfitChanged != null)
				OnMoneyProfitChanged();
		}
	}

	public static int FameProfit
	{
		get { return currentFameProfit; }
		
		set
		{
			currentFameProfit = value;
			
			if(OnFameProfitChanged != null)
				OnFameProfitChanged();
		}
	}

	public static int FameDecrease
	{
		get
		{
			int decrease = 0;

			foreach(FameDecrease fd in Instance.fameDecrease)
			{
				if(Money >= fd.money.min && Money <= fd.money.max)
				{
					decrease = fd.decrease;
					break;
				}
			}

			return decrease;
		}
	}

	public static int Upkeep
	{
		get { return currentUpkeep; }
		
		set
		{
			currentUpkeep = value;
			
			if(OnUpkeepChanged != null)
				OnUpkeepChanged();
		}
	}

	public static int Week
	{
		get { return currentWeek; }
		
		set
		{
			currentWeek = value;
			
			if(OnWeekChanged != null)
				OnWeekChanged();
		}
	}

	public static int Month
	{
		get { return currentMonth; }
		
		set
		{
			currentMonth = value;
			
			if(OnMonthChanged != null)
				OnMonthChanged();
		}
	}

	public static bool IsGameOver
	{
		get { return Fame <= MinFame || Fame >= MaxFame; }
	}

	public static int MaxFame
	{
		get { return Instance.maxFame; }
	}

	public static int MinFame
	{
		get { return Instance.minFame; }
	}

	public static int InitialCards
	{
		get { return Instance.initialCards; }
	}

	public static int CardsPlayedThisTurn
	{
		get { return constructionCardsPlayedThisTurn + effectCardsPlayedThisTurn; }
	}

	public static int ConstructionCardsPlayedThisTurn
	{
		get { return constructionCardsPlayedThisTurn; }
	}
	
	public static int EffectCardsPlayedThisTurn
	{
		get { return effectCardsPlayedThisTurn; }
	}

	public static int MaxCardsPerTurn
	{
		get { return Instance.maxCardsPerTurn; }
	}

	public static int MaxConstructionCardsPerTurn
	{
		get { return Instance.maxConstructionCardsPerTurn; }
	}

	public static int MaxEffectCardsPerTurn
	{
		get { return Instance.maxEffectsCardsPerTurn; }
	}

	public static int MaxCardsInHand
	{
		get { return Instance.maxCardsInHand; }
	}

	public static ConflictCard.SpecialEffect ActiveConflictEffect
	{
		get { return activeConflictEffect; }
	}

	#endregion

	#region singleton
	private static GameController instance;
	public static GameController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<GameController>();
			
			return instance;
		}
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		ConstructionArea.OnConstructed += NewConstruction;
		ConstructionArea.OnReady += UpdateParameters;
		ConstructionArea.OnDiscarded += UpdateParameters;

		EffectArea.OnPlayed += NewEffect;

		ConflictCard.OnActive += NewConflict;

		constructionAreas = GameObject.FindObjectsOfType<ConstructionArea>();

		Money = initialMoney;
		Fame = initialFame;

		currentMonth = 1;
		currentWeek = 1;
		bonusFame = 1;
		bonusMoney = 1;
		cozinhaUpkeepBonus = 0;

		activeCardEffect = EffectCard.EffectType.None;
		activeConflictEffect = ConflictCard.SpecialEffect.None;
		moneyMultiplier = 1;
		fameMultiplier = 1;
	}

	public void NextWeek()
	{
		Money -= Upkeep;
		
		constructionCardsPlayedThisTurn = 0;
		effectCardsPlayedThisTurn = 0;

		if(Week < 4)
		{
			Week++;
			DeckController.Instance.DrawCards(cardsToDrawPerWeek);
		}
		else
		{
			Week = 1;
			NextMonth();
		}

		Fame -= FameDecrease;

		VerifyEndGame();

		if(Week == 1 && !IsGameOver)
			ShowConflictCard();

		Debug.Log(string.Format("Passed to Week {0}", Week));
	}

	public void NextMonth()
	{
		Debug.Log(string.Format("Passed to Month {0}", Month));

		Money += MoneyProfit;
		Fame += FameProfit;

		//reset all conflict effects
		moneyMultiplier = 1;
		fameMultiplier = 1;
		
		UpdateParameters();

		DeckController.Instance.DrawCards(cardsToDrawPerMonth);

		Month++;
	}

	private void VerifyEndGame()
	{
		if(Fame <= MinFame)
			Popup.ShowBlank("GAME OVER");

		if(Fame >= MaxFame)
			Popup.ShowBlank("Winner, don't do drugs!!!");
	}

	private void ShowConflictCard()
	{
		DeckController.Instance.ShowConflictCard();
		HUDController.Instance.ShowConflictCard();
	}

	private void NewConflict(ConflictCard.SpecialEffect specialEffect)
	{
		activeConflictEffect = specialEffect;

		UpdateParameters();
	}

	private void UpdateParameters()
	{
		UpdateParameters(null);
	}

	private void UpdateParameters(Card c)
	{
		#region update construction bonus
		bonusFame = 1;
		bonusMoney = 1;
		cozinhaUpkeepBonus = 0;

		foreach(ConstructionArea cArea in constructionAreas)
		{
			if(cArea.constructionCard != null && cArea.IsActive && cArea.constructionCard.IsReady)
			{
				if(cArea.constructionCard.specialEffect == ConstructionCard.SpecialEffect.Lucro)
					bonusMoney += cArea.constructionCard.specialEffectValue;

				if(cArea.constructionCard.specialEffect == ConstructionCard.SpecialEffect.Fama)
					bonusFame += cArea.constructionCard.specialEffectValue;

				if(cArea.constructionCard.specialEffect == ConstructionCard.SpecialEffect.ManutencaoCozinha)
					cozinhaUpkeepBonus += (int)cArea.constructionCard.specialEffectValue;
			}
		}

		#endregion

		#region update monthly money reward
		int moneyProfit = 0;
		foreach(ConstructionArea cArea in constructionAreas)
		{
			if(cArea.constructionCard != null && cArea.IsActive && cArea.constructionCard.IsReady)
				moneyProfit += cArea.constructionCard.moneyReward;
		}

		MoneyProfit = Mathf.FloorToInt(moneyProfit * bonusMoney * moneyMultiplier);
		#endregion

		#region update monthly fame reward
		int fameProfit = 0;
		foreach(ConstructionArea cArea in constructionAreas)
		{
			if(cArea.constructionCard != null && cArea.IsActive && cArea.constructionCard.IsReady)
				fameProfit += cArea.constructionCard.fameReward;
		}

		FameProfit = Mathf.FloorToInt(fameProfit * bonusFame * fameMultiplier);
		#endregion

		#region update upkeep
		int upkeep = 0;
		foreach(ConstructionArea cArea in constructionAreas)
		{
			if(cArea.constructionCard != null)
			{
				if(cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Cozinha)
					upkeep += Mathf.Max(cArea.constructionCard.upkeep + cozinhaUpkeepBonus, 0);
				else
					upkeep += cArea.constructionCard.upkeep;
			}
		}
		
		Upkeep = upkeep;
		#endregion
	}

	private void NewConstruction(Card c)
	{
		constructionCardsPlayedThisTurn++;

		UpdateParameters();
	}

	private void NewEffect(Card c)
	{
		effectCardsPlayedThisTurn++;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log(
				string.Format("Money: {0} \n" +
				              "Fame: {1} \n" +
				              "Money Profit (month): {2} \n" +
				              "Fame Profit (month): {3} \n" +
			              	  "Fame Decrease: {4} \n" + 
				              "Upkeep (week): {5} \n" +
				              "Fame bonus: {6} \n" +
				              "Money bonus: {7} \n" +
				              "Cozinha upkeep bonus: {8} \n" +
				              "Current Week: {9} \n" +
				              "Current Month: {10} \n" +
				              "Cards played this turn: {11} / {12} \n" +
				              "Construction cards played this turn: {13} / {14} \n" +
				              "Effect cards played this turn: {15} / {16} \n" +
				              "Cards in hand: {17} / {18}",
				              Money,
				              Fame,
				              MoneyProfit,
				              FameProfit,
			              	  FameDecrease,
				              Upkeep,
			              	  ((bonusFame - 1) * 100f) + "%",
			              	  ((bonusMoney - 1) * 100f) + "%",
			              	  cozinhaUpkeepBonus,
				              Week,
				              Month,
				              CardsPlayedThisTurn, MaxCardsPerTurn,
				              ConstructionCardsPlayedThisTurn, MaxConstructionCardsPerTurn,
				              EffectCardsPlayedThisTurn, MaxEffectCardsPerTurn,
				              DeckController.CardsInHand, MaxCardsInHand
				              )
				);
		}
	}

}

[System.Serializable]
public class FameDecrease
{
	public MinMax money;
	public int decrease;
}

[System.Serializable]
public class MinMax
{
	public int max;
	public int min;
}
