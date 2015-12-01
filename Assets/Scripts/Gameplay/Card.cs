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

	void OnEnable()
	{

	}

	void OnDisable()
	{
		
	}

	protected virtual void Start()
	{
		panel = GetComponent<UIPanel>();

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
		costFameLabel.text = (minFame == -5) ? "--" : minFame.ToString();
		cooldownLabel.text = cooldown.ToString();

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
			panel.depth += 10;
		else
			panel.depth -= 10;
	}
}
