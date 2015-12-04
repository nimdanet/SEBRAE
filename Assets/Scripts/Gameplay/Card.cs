using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour 
{
	public enum Type
	{
		Construction,
		Effect,
	}

	public string nome;
	public string description;
	public Texture2D image;

	protected Type cardType;
	public int cost;
	public int minFame;
	public int moneyReward;
	public int fameReward;
	public int cooldown;
	private int fullCooldown;

	[HideInInspector]
	public GameCard originalCard;

	protected UILabel cardName;
	protected UILabel cardDescription;
	protected UITexture cardImage;
	protected UILabel rewardMoneyLabel;
	protected UILabel rewardFameLabel;
	protected UILabel costMoneyLabel;
	protected UILabel costFameLabel;
	protected UILabel cooldownLabel;

	private UIPanel panel;
	private int originalDepth;
	private TweenPosition tweenPosition;
	private TweenScale tweenScale;
	private bool isTweening;

	[HideInInspector]
	public bool placed;

	#region get / set
	public int Depth
	{
		set
		{
			originalDepth = value;
			Panel.depth = value;
		}
	}

	private UIPanel Panel
	{
		get
		{
			if(panel == null)
				panel = GetComponent<UIPanel>();

			return panel;
		}
	}

	public bool IsTweening
	{
		get { return isTweening; }
	}
	#endregion

	void OnDestroy()
	{
		Popup.OnShow -= LockInteraction;
		Popup.OnHide -= UnlockInteraction;
		ConflictCard.OnShow -= LockInteraction;
		ConflictCard.OnHide -= UnlockInteraction;
		DeckController.StartDrawCards -= LockInteraction;
		DeckController.FinishDrawCards -= UnlockInteraction;
		HUDController.OnPassWeek -= LockInteraction;
		HowToPlay.OnOpen -= LockInteraction;
		HowToPlay.OnClose -= UnlockInteraction;
	}

	protected virtual void Start()
	{
		Popup.OnShow += LockInteraction;
		Popup.OnHide += UnlockInteraction;
		ConflictCard.OnShow += LockInteraction;
		ConflictCard.OnHide += UnlockInteraction;
		DeckController.StartDrawCards += LockInteraction;
		DeckController.FinishDrawCards += UnlockInteraction;
		HUDController.OnPassWeek += LockInteraction;
		HowToPlay.OnOpen += LockInteraction;
		HowToPlay.OnClose += UnlockInteraction;

		tweenPosition = GetComponent<TweenPosition>();
		tweenScale = GetComponent<TweenScale>();

		cardName = transform.FindChild("Front").FindChild("Title").FindChild ("Label").GetComponent<UILabel>();
		cardDescription = transform.FindChild("Front").FindChild("Description").FindChild ("Label").GetComponent<UILabel>();
		cardImage = transform.FindChild("Front").FindChild("Image").GetComponent<UITexture>();

		rewardMoneyLabel = transform.FindChild("Front").FindChild("Reward Money").FindChild("Label").GetComponent<UILabel>();
		rewardFameLabel = transform.FindChild("Front").FindChild("Reward Fama").FindChild("Label").GetComponent<UILabel>();
		costMoneyLabel = transform.FindChild("Front").FindChild("Cost").FindChild("money").GetComponent<UILabel>();
		costFameLabel = transform.FindChild("Front").FindChild("Cost").FindChild("fame").GetComponent<UILabel>();
		cooldownLabel = transform.FindChild("Front").FindChild("Cooldown").FindChild("Label").GetComponent<UILabel>();

		cardName.text = nome;
		cardDescription.text = description;
		if(image != null)
			cardImage.mainTexture = image;
		rewardMoneyLabel.text = moneyReward.ToString();
		rewardFameLabel.text = fameReward.ToString();
		costMoneyLabel.text = cost.ToString();
		costFameLabel.text = (minFame < 0) ? "--" : minFame.ToString();
		cooldownLabel.text = cooldown.ToString();

		originalDepth = Panel.depth;
		placed = false;

		fullCooldown = cooldown;

		StartCoroutine(DoTween());
	}

	private IEnumerator DoTween()
	{
		isTweening = true;
		LockInteraction();

		tweenPosition.ResetToBeginning();
		tweenPosition.duration = DeckController.Instance.timeDrawing * 0.2f;
		tweenPosition.from = DeckController.Instance.spawnPosition.localPosition;
		tweenPosition.to = DeckController.Instance.waypoint1.localPosition;
		tweenPosition.PlayForward();

		tweenScale.ResetToBeginning();
		tweenScale.duration = DeckController.Instance.timeDrawing * 0.2f;
		tweenScale.from = Vector3.one;
		tweenScale.to = Vector3.one * 1.5f;
		tweenScale.PlayForward();

		while(tweenPosition.isActiveAndEnabled)
			yield return null;

		tweenPosition.ResetToBeginning();
		tweenPosition.duration = DeckController.Instance.timeDrawing * 0.6f;
		tweenPosition.from = tweenPosition.to;
		tweenPosition.to = tweenPosition.from + new Vector3(200f, 0);
		tweenPosition.PlayForward();

		while(tweenPosition.isActiveAndEnabled)
			yield return null;

		tweenPosition.ResetToBeginning();
		tweenPosition.duration = DeckController.Instance.timeDrawing * 0.2f;
		tweenPosition.from = tweenPosition.to;
		tweenPosition.to = DeckController.Instance.GetPlacePosition(this);
		tweenPosition.PlayForward();

		tweenScale.ResetToBeginning();
		tweenScale.duration = DeckController.Instance.timeDrawing * 0.2f;
		tweenScale.from = Vector3.one * 1.5f;
		tweenScale.to = Vector3.one;
		tweenScale.PlayForward();
		
		while(tweenPosition.isActiveAndEnabled)
			yield return null;

		isTweening = false;

		DeckController.Instance.ArrangeHand();
	}

	public void ResetCooldown()
	{
		cooldown = fullCooldown;
	}

	public void ResetCooldown(int weeks)
	{
		cooldown = weeks;
	}

	public void Discard()
	{
		TrashCan.Discard(this);

		//gameObject.SetActive(false);
	}

	void OnHover(bool isOver)
	{
		if(isOver)
		{
			originalDepth = Panel.depth;
			Panel.depth += 10;
		}
		else
			Panel.depth = originalDepth;
	}

	private void LockInteraction()
	{
		//Debug.Log("Lock: " + nome);
		Panel.depth = originalDepth;
		GetComponent<Collider>().enabled = false;

		if(GameController.activeCardEffect == EffectCard.EffectType.DescartaCompraCartas)
			UnlockInteraction();
	}

	private void UnlockInteraction()
	{
		Debug.Log("Unlock: " + nome);
		GetComponent<Collider>().enabled = !placed;
	}
}
