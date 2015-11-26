using UnityEngine;
using System;
using System.Collections;

public class EffectArea : MonoBehaviour 
{
	#region Action
	public static event Action<Card> OnPlayed;
	public static event Action<Card> OnReady;
	public static event Action<Card> OnDestroyed;
	#endregion

	public Color rightColor;
	public Color wrongColor;
	public Color hoverColor;
	public Color normalColor = Color.white;

	public EffectCard effectCard;

	private UI2DSprite spriteArea;
	private Transform hudCard;
	private UILabel countdown;

	// Use this for initialization
	void Start () 
	{
		DragDropCard.OnDragStarted += CardDragged;
		DragDropCard.OnDropped += CardDropped;
		DragDropCard.OnDragMove += CardMoved;

		spriteArea = transform.FindChild("Sprite").GetComponent<UI2DSprite>();
		hudCard = transform;//GameObject.Find(gameObject.name.Replace("Area", "HUD")).transform;
		countdown = hudCard.FindChild("Countdown").GetComponentInChildren<UILabel>();
		countdown.enabled = false;
	}

	private void CardDragged(Card card)
	{
		EffectCard eCard = card as EffectCard;
		
		if(eCard == null) 
		{
			spriteArea.color = wrongColor;
			return;
		}
		
		if(effectCard != null)
			spriteArea.color = wrongColor;
		else
			spriteArea.color = rightColor;
		
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
			else if(GameController.EffectCardsPlayedThisTurn >= GameController.MaxEffectCardsPerTurn)
			{
				//TODO: feedback NO MORE CONSTRUCTION CARDS!
				Debug.Log(string.Format("Played {0} / {1} effect cards. Can't play more cards", GameController.ConstructionCardsPlayedThisTurn, GameController.MaxConstructionCardsPerTurn));
			}
			else if(GameController.Fame < card.minFame)
			{
				//TODO: feedback NO FAME!
				Debug.Log(string.Format("Fame {0} Required Fame {1} No FAME", GameController.Fame, card.minFame));
			}
			
			/*else if(GameController.Money < card.cost && card.cost != 0)
			{
				//TODO: feedback NO MONEY!
				Debug.Log(string.Format("Money {0} Required Money {1} No MONEY", GameController.Money, card.cost));
			}*/
			else
			{
				Debug.Log("New Effect!");
				
				//added card to gameplay
				effectCard = card as EffectCard;
				card.transform.parent = hudCard.FindChild("Card");
				card.transform.localPosition = Vector3.zero;
				card.transform.localScale = Vector3.one;
				
				//change sprite and activate cooldown
				countdown.enabled = true;
				countdown.text = effectCard.cooldown.ToString();
				
				GameController.OnWeekChanged += DecreaseCooldown;

				effectCard.OnPlayed();

				if(OnPlayed != null)
					OnPlayed(card);
				
				GameController.Money -= card.cost;

				if(effectCard.cooldown == 0)
					UnlockEffect();
			}
		}

		spriteArea.color = normalColor;
	}
	
	private void DecreaseCooldown()
	{
		effectCard.cooldown--;
		countdown.text = effectCard.cooldown.ToString();
		
		if(effectCard.cooldown <= 0)
			UnlockEffect();
	}

	private void UnlockEffect()
	{
		GameController.OnWeekChanged -= DecreaseCooldown;
		countdown.enabled = false;
		
		effectCard.OnReady();
		
		if(OnReady != null)
			OnReady(effectCard);
		
		effectCard = null;
	}
	
	private void CardMoved(Card card)
	{
		if(spriteArea.color == wrongColor) return;

		Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;
		Physics.Raycast(ray, out hit);

		if(hit.collider != null)
		{
			if(hit.collider == GetComponent<Collider>())
				spriteArea.color = hoverColor;
			else
				spriteArea.color = rightColor;
		}
		else
		{
			spriteArea.color = rightColor;
		}
	}
}
