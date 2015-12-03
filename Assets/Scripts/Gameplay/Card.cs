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
	private bool lastColliderStatus;
	private int originalDepth;

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
	#endregion

	void OnDestroy()
	{
		Debug.Log("OnDestroy");
		Popup.OnShow -= LockInteraction;
		ConflictCard.OnShow -= LockInteraction;
		Popup.OnHide -= UnlockInteraction;
		ConflictCard.OnHide -= UnlockInteraction;
	}

	protected virtual void Start()
	{
		Popup.OnShow += LockInteraction;
		ConflictCard.OnShow += LockInteraction;
		Popup.OnHide += UnlockInteraction;
		ConflictCard.OnHide += UnlockInteraction;

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

		fullCooldown = cooldown;
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
		Debug.Log("Lock: " + nome);
		Panel.depth = originalDepth;
		lastColliderStatus = GetComponent<Collider>().enabled;
		GetComponent<Collider>().enabled = false;

		if(GameController.activeCardEffect == EffectCard.EffectType.DescartaCompraCartas)
			UnlockInteraction();
	}

	private void UnlockInteraction()
	{
		Debug.Log("Unlock: " + nome);
		GetComponent<Collider>().enabled = lastColliderStatus;
	}
}
