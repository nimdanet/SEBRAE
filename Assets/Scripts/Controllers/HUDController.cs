using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
	public UISprite fameFill;
	public UILabel fameLabel;

	public UILabel moneyLabel;
	public UILabel upkeepLabel;
	public UILabel profitLabel;
	public UILabel fameProfitLabel;

	public TweenRotation pointer;

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

	void Awake()
	{
		GameController.OnFameChanged += FameUpdated;
		GameController.OnMoneyChanged += MoneyUpdated;
		GameController.OnUpkeepChanged += UpkeepUpdated;
		GameController.OnMoneyProfitChanged += MoneyProfitUpdated;
		GameController.OnFameProfitChanged += FameProfitUpdated;
	}

	void Start()
	{
		conflictDeck.SetActive(false);
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
			Debug.Log(string.Format("Too many cards in hand ({0})", GameController.MaxCardsInHand));
			return;
		}

		pointer.ResetToBeginning();

		pointer.from = new Vector3(0, 0, 90 - (90f * (GameController.Week - 1f)));
		pointer.to = new Vector3(0, 0, 90 - (90f * (GameController.Week - 0f)));

		pointer.PlayForward();
	}

	public void ShowConflictCard()
	{
		conflictDeck.SetActive(true);
	}

	public void HideConflictCard()
	{
		conflictDeck.SetActive(false);
	}
}
