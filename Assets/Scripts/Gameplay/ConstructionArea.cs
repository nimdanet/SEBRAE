using UnityEngine;
using System;
using System.Collections;

public class ConstructionArea : MonoBehaviour 
{
	#region Action
	public static event Action<Card> OnConstructed;
	public static event Action<Card> OnReady;
	public static event Action<Card> OnDiscarded;

	public static event Action<ConstructionArea> OnAreaSelected;
	#endregion

	public Color rightColor;
	public Color wrongColor;
	public Color hoverColor;
	public Color normalColor = Color.white;
	public Color inactiveColor = Color.gray;

	public Sprite blankSprite;
	public Sprite constructionSprite;

	[HideInInspector]
	public ConstructionCard constructionCard;

	private SpriteRenderer spriteArea;
	private Transform constructionHUD;
	private Transform hudCard;
	private UILabel countdown;

	private bool inactive;

	#region get / set
	public bool IsActive
	{
		get { return !inactive; }
	}
	#endregion

	void Start()
	{
		DragDropCard.OnDragStarted += CardDragged;
		DragDropCard.OnDropped += CardDropped;
		DragDropCard.OnDragMove += CardMoved;

		EffectCard.OnWaitingForSelect += ShowClickableAreas;
		ConstructionArea.OnAreaSelected += AreaSelected;

		spriteArea = GetComponent<SpriteRenderer>();
		spriteArea.sprite = blankSprite;
		Color c = spriteArea.color;
		c.a = 0;
		spriteArea.color = c;

		constructionHUD = GameObject.Find(gameObject.name.Replace("Area", "HUD")).transform;
		hudCard = constructionHUD.FindChild("Card");
		countdown = constructionHUD.FindChild("Countdown").GetComponent<UILabel>();

		countdown.enabled = false;
		hudCard.gameObject.SetActive(false);
		inactive = false;
	}

	public void OnCardFlipped()
	{
		TweenScale tween = UITweener.current as TweenScale;

		if(tween.direction == AnimationOrTween.Direction.Forward)
			tween.PlayReverse();
		else
			tween.PlayForward();
	}

	private void CardDragged(Card card)
	{
		ConstructionCard cCard = card as ConstructionCard;

		if(cCard == null) 
		{
			spriteArea.color = wrongColor;
			return;
		}

		if(GameController.activeCardEffect == EffectCard.EffectType.DescartaCompraCartas)
			spriteArea.color = wrongColor;
		else if(constructionCard != null)
		{
			if(constructionCard.constructionType == cCard.constructionType)
			{
				if(cCard.level == constructionCard.level + 1)
					spriteArea.color = rightColor;
				else
					spriteArea.color = wrongColor;
			}
			else
				spriteArea.color = wrongColor;

		}
		else
		{
			if(cCard.level > 1)
				spriteArea.color = wrongColor;
			else
				spriteArea.color = rightColor;
		}
		
	}

	private void CardDropped(Card card)
	{
		if(spriteArea.color == hoverColor)
		{
			if(GameController.CardsPlayedThisTurn >= GameController.MaxCardsPerTurn)
			{
				//TODO: feedback NO MORE CARDS!
				Popup.ShowOk(string.Format("Played {0} / {1} cards. Can't play more cards", GameController.CardsPlayedThisTurn, GameController.MaxCardsPerTurn));
			}
			else if(GameController.ConstructionCardsPlayedThisTurn >= GameController.MaxConstructionCardsPerTurn)
			{
				//TODO: feedback NO MORE CONSTRUCTION CARDS!
				Popup.ShowOk(string.Format("Played {0} / {1} construction cards. Can't play more cards", GameController.ConstructionCardsPlayedThisTurn, GameController.MaxConstructionCardsPerTurn));
			}
			else if(GameController.Fame < card.minFame)
			{
				//TODO: feedback NO FAME!
				Popup.ShowOk(string.Format("Não tem fama suficiente. ({0} / {1}) ", GameController.Fame, card.minFame));
			}
			else if(constructionCard != null && constructionCard.cooldown > 0)
			{
				Popup.ShowOk (string.Format("Você já tem uma construção em andamento nesse lugar.", constructionCard.cooldown));
			}
			/*else if(GameController.Money < card.cost)
			{
				//TODO: feedback NO MONEY!
				Debug.Log(string.Format("Money {0} Required Money {1} No MONEY", GameController.Money, card.cost));
			}*/
			else
			{
				Debug.Log("Construct!");

				if(constructionCard != null)
					Discard();

				//added card to gameplay
				constructionCard = card as ConstructionCard;
				card.transform.parent = hudCard;
				card.transform.localPosition = Vector3.zero;
				card.transform.localScale = Vector3.one;
				card.GetComponent<Collider>().enabled = false;
				card.transform.FindChild("Front").localPosition = Vector3.zero;
				card.transform.FindChild("Back").localPosition = Vector3.zero;
				card.placed = true;

				//inactivate blank card
				hudCard.FindChild("Blank").gameObject.SetActive(false);

				//change sprite and activate cooldown
				spriteArea.sprite = constructionSprite;
				countdown.enabled = true;
				countdown.text = constructionCard.cooldown.ToString();

				VerifyActiveConflict();

				GameController.OnWeekChanged += DecreaseCooldown;

				if(OnConstructed != null)
					OnConstructed(card);

				GameController.Money -= card.cost;
			}
		}

		spriteArea.color = (IsActive) ? normalColor : inactiveColor;

		if(spriteArea.sprite == blankSprite)
		{
			Color c = spriteArea.color;
			c.a = 0;
			spriteArea.color = c;
		}

	}

	private void VerifyActiveConflict()
	{
		if((GameController.ActiveConflictEffect == ConflictCard.SpecialEffect.Cozinhas && 
		    constructionCard.constructionType == ConstructionCard.ContructionType.Cozinha) ||
		   (GameController.ActiveConflictEffect == ConflictCard.SpecialEffect.Saloes && 
			constructionCard.constructionType == ConstructionCard.ContructionType.Salao) ||
		   (GameController.ActiveConflictEffect == ConflictCard.SpecialEffect.DispensaLobby && 
		 	(constructionCard.constructionType == ConstructionCard.ContructionType.Dispensa || 
		 	constructionCard.constructionType == ConstructionCard.ContructionType.Lobby)) ||
		   (GameController.ActiveConflictEffect == ConflictCard.SpecialEffect.AdministrativoBanheiro && 
		 	(constructionCard.constructionType == ConstructionCard.ContructionType.Administrativo || 
		 	constructionCard.constructionType == ConstructionCard.ContructionType.Banheiro)))
		{
			Inactivate();
		}
	}

	private void DecreaseCooldown()
	{
		constructionCard.cooldown--;
		countdown.text = constructionCard.cooldown.ToString();

		if(constructionCard.cooldown == 0)
			ConstructionComplete();
	}

	private void ConstructionComplete()
	{
		GameController.OnWeekChanged -= DecreaseCooldown;
		countdown.enabled = false;

		spriteArea.sprite = constructionCard.sprite;
		
		if(OnReady != null)
			OnReady(constructionCard);
	}

	public void Discard()
	{
		GameController.OnWeekChanged -= DecreaseCooldown;
		
		constructionCard.Discard();
		
		if(OnDiscarded != null)
			OnDiscarded(constructionCard);
		
		constructionCard = null;

		spriteArea.sprite = blankSprite;
		Color c = spriteArea.color;
		c.a = 0;
		spriteArea.color = c;
		countdown.enabled = false;

		//reactivate blank card
		hudCard.FindChild("Blank").gameObject.SetActive(true);
	}

	public void ZeraCooldown()
	{
		Debug.Log(string.Format("Zera Cooldown da {0} {1}", constructionCard.constructionType, constructionCard.level));
		constructionCard.cooldown = 0;

		ConstructionComplete();
	}

	public void RestoreCooldown()
	{
		Debug.Log(string.Format("Volta Cooldown da {0} {1}", constructionCard.constructionType, constructionCard.level));
		constructionCard.ResetCooldown();

		spriteArea.sprite = constructionSprite;
		countdown.enabled = true;
		countdown.text = constructionCard.cooldown.ToString();

		GameController.OnWeekChanged -= DecreaseCooldown;
		GameController.OnWeekChanged += DecreaseCooldown;
	}

	public void DiminuiCooldown(int value)
	{
		while(value > 0)
		{
			DecreaseCooldown();
			value--;
		}
	}

	public void Inactivate()
	{
		Debug.Log("Inactivate!");

		inactive = true;
		spriteArea.color = inactiveColor;

		GameController.OnMonthChanged += Reactivate;
	}

	private void Reactivate()
	{
		inactive = false;
		spriteArea.color = normalColor;

		GameController.OnMonthChanged -= Reactivate;
	}

	private void CardMoved(Card card)
	{

	}

	private void ShowClickableAreas()
	{
		Debug.Log(gameObject.name);
		Debug.Log(constructionCard);
		if(GameController.activeCardEffect == EffectCard.EffectType.DestroiConstrucao)
		{
			if(constructionCard != null)
				spriteArea.color = rightColor;
			else
				spriteArea.color = wrongColor;
		}
		else if(GameController.activeCardEffect == EffectCard.EffectType.DiminuiCooldown)
		{
			if(constructionCard != null)
			{
				Debug.Log(constructionCard.cooldown);
				if(constructionCard.cooldown > 0)
					spriteArea.color = rightColor;
				else
					spriteArea.color = wrongColor;
			}
			else
				spriteArea.color = wrongColor;
		}
	}

	private void AreaSelected(ConstructionArea cArea)
	{
		if(constructionCard == null)
			spriteArea.color = normalColor;
		else
			spriteArea.color = (IsActive) ? normalColor : inactiveColor;

		if(spriteArea.sprite == blankSprite)
		{
			Color c = spriteArea.color;
			c.a = 0;
			spriteArea.color = c;
		}
	}

	void OnMouseOver() 
	{
		if(DragDropCard.current != null && spriteArea.color != wrongColor)
			spriteArea.color = hoverColor;

		if(GameController.activeCardEffect == EffectCard.EffectType.DestroiConstrucao || GameController.activeCardEffect == EffectCard.EffectType.DiminuiCooldown)
		{
			if(spriteArea.color != wrongColor)
				spriteArea.color = hoverColor;
		}

		if(constructionCard != null && (spriteArea.color == normalColor || spriteArea.color == inactiveColor))
			hudCard.gameObject.SetActive(true);
	}

	void OnMouseExit() 
	{
		if(DragDropCard.current != null && spriteArea.color != wrongColor)
			spriteArea.color = rightColor;

		if(GameController.activeCardEffect == EffectCard.EffectType.DestroiConstrucao || GameController.activeCardEffect == EffectCard.EffectType.DiminuiCooldown)
		{
			if(spriteArea.color != wrongColor)
				spriteArea.color = rightColor;
		}

		if(constructionCard != null && (spriteArea.color == normalColor || spriteArea.color == inactiveColor))
			hudCard.gameObject.SetActive(false);
	}

	void OnMouseDown()
	{
		Debug.Log("Selected : " + gameObject.name);

		if(OnAreaSelected != null)
			OnAreaSelected(this);
	}
}
