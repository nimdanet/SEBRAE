using UnityEngine;
using System;
using System.Collections;

public class ConstructionArea : MonoBehaviour 
{
	#region Action
	public static event Action<Card> OnConstructed;
	public static event Action<Card> OnReady;
	public static event Action<Card> OnDestroyed;
	#endregion

	public Color rightColor;
	public Color wrongColor;
	public Color hoverColor;
	public Color normalColor = Color.white;

	public Sprite blankSprite;
	public Sprite constructionSprite;

	public ConstructionCard constructionCard;

	private SpriteRenderer spriteArea;
	private Transform hudCard;
	private UILabel countdown;

	void Start()
	{
		DragDropCard.OnDragStarted += CardDragged;
		DragDropCard.OnDropped += CardDropped;
		DragDropCard.OnDragMove += CardMoved;

		spriteArea = GetComponent<SpriteRenderer>();
		spriteArea.sprite = blankSprite;

		hudCard = GameObject.Find(gameObject.name.Replace("Area", "HUD")).transform;
		countdown = hudCard.FindChild("Countdown").GetComponent<UILabel>();
		countdown.enabled = false;
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

		if(constructionCard != null)
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
				Debug.Log(string.Format("Played {0} / {1} cards. Can't play more cards", GameController.CardsPlayedThisTurn, GameController.MaxCardsPerTurn));
			}
			else if(GameController.ConstructionCardsPlayedThisTurn >= GameController.MaxConstructionCardsPerTurn)
			{
				//TODO: feedback NO MORE CONSTRUCTION CARDS!
				Debug.Log(string.Format("Played {0} / {1} construction cards. Can't play more cards", GameController.ConstructionCardsPlayedThisTurn, GameController.MaxConstructionCardsPerTurn));
			}
			else if(GameController.Fame < card.minFame)
			{
				//TODO: feedback NO FAME!
				Debug.Log(string.Format("Fame {0} Required Fame {1} No FAME", GameController.Fame, card.minFame));
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
				{
					GameController.OnWeekChanged -= DecreaseCooldown;

					constructionCard.Discard();

					if(OnDestroyed != null)
						OnDestroyed(constructionCard);

					constructionCard = null;
				}

				//added card to gameplay
				constructionCard = card as ConstructionCard;
				card.transform.parent = hudCard.FindChild("Card");
				card.transform.localPosition = Vector3.zero;
				card.transform.localScale = Vector3.one;

				//inactivate blank card
				hudCard.FindChild("Card").FindChild("Blank").gameObject.SetActive(false);

				//change sprite and activate cooldown
				spriteArea.sprite = constructionSprite;
				countdown.enabled = true;
				countdown.text = constructionCard.cooldown.ToString();

				GameController.OnWeekChanged += DecreaseCooldown;

				if(OnConstructed != null)
					OnConstructed(card);

				GameController.Money -= card.cost;
			}
		}

		spriteArea.color = normalColor;
	}

	private void DecreaseCooldown()
	{
		constructionCard.cooldown--;
		countdown.text = constructionCard.cooldown.ToString();

		if(constructionCard.cooldown == 0)
		{
			GameController.OnWeekChanged -= DecreaseCooldown;
			countdown.enabled = false;
			//TODO: uncomment this when all sprite areas were filled
			//spriteArea.sprite = constructionCard.sprite;

			if(OnReady != null)
				OnReady(constructionCard);
		}
	}

	private void CardMoved(Card card)
	{

	}

	void OnMouseOver() 
	{
		if(DragDropCard.current != null && spriteArea.color != wrongColor)
			spriteArea.color = hoverColor;
	}

	void OnMouseExit() 
	{
		if(DragDropCard.current != null && spriteArea.color != wrongColor)
			spriteArea.color = rightColor;
	}


}
