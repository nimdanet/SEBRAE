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

	[HideInInspector]
	public GameCard originalCard;

	protected UILabel cardName;
	protected UISprite cardImage;
	protected UILabel rewardMoneyLabel;
	protected UILabel rewardFameLabel;
	protected UILabel costMoneyLabel;
	protected UILabel costFameLabel;
	protected UILabel cooldownLabel;

	void OnEnable()
	{

	}

	void OnDisable()
	{
		
	}

	protected virtual void Start()
	{
		cardName = transform.FindChild("Front").FindChild("Title").GetComponent<UILabel>();
		cardImage = transform.FindChild("Front").FindChild("Image").GetComponent<UISprite>();

		rewardMoneyLabel = transform.FindChild("Front").FindChild("Reward Money").FindChild("Label").GetComponent<UILabel>();
		rewardFameLabel = transform.FindChild("Front").FindChild("Reward Fama").FindChild("Label").GetComponent<UILabel>();
		costMoneyLabel = transform.FindChild("Front").FindChild("Cost").FindChild("money").GetComponent<UILabel>();
		costFameLabel = transform.FindChild("Front").FindChild("Cost").FindChild("fame").GetComponent<UILabel>();
		cooldownLabel = transform.FindChild("Front").FindChild("Cooldown").FindChild("Label").GetComponent<UILabel>();

		cardName.text = nome;
		rewardMoneyLabel.text = moneyReward.ToString();
		rewardFameLabel.text = fameReward.ToString();
		costMoneyLabel.text = cost.ToString();
		costFameLabel.text = minFame.ToString();
		cooldownLabel.text = cooldown.ToString();
	}

	public void Discard()
	{
		TrashCan.Discard(this);

		gameObject.SetActive(false);
	}
}
