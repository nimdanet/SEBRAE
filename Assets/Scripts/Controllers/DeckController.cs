using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DeckController : MonoBehaviour 
{
	#region action
	public static event Action StartDrawCards;
	public static event Action FinishDrawCards;
	#endregion

	#region singleton
	private static DeckController instance;
	public static DeckController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<DeckController>();

			return instance;
		}
	}
	#endregion

	#region Get / Set
	public static int CardsInHand
	{
		get { return cardsInHand.Count; }
	}
	#endregion

	public List<GameConstructionCard> constructionCards;
	public List<GameEffectCard> effectCards;
	public List<GameConflictCard> conflictCards;
	private static List<GameCard> deck;
	private static List<GameConflictCard> conflictDeck;
	private static List<Card> cardsInHand;
	public static List<GameCard> discardedPile;
	public static List<GameConflictCard> discardedConflictPile;

	[Header("For Developer")]
	public Transform hand;
	public GameObject constructionCardDefault;
	public GameObject effectCardDefault;
	public Transform conflictCardHolder;
	public GameObject goodConflictDefault;
	public GameObject badConflictDefault;
	public float maxCardSpace = 95f;

	[Header("Card Tween")]
	public Transform spawnPosition;
	public Transform waypoint1;
	public float timeDrawing;

	void Start()
	{
		ConstructionArea.OnConstructed += NewConstruction;
		TrashCan.OnDiscarded += CardDiscarded;
		EffectArea.OnPlayed += NewEffect;

		Build();
	}

	private void Build()
	{
		discardedPile = new List<GameCard>();
		deck = new List<GameCard>();
		cardsInHand = new List<Card>();
		conflictDeck = new List<GameConflictCard>();
		discardedConflictPile = new List<GameConflictCard>();

		foreach(GameConstructionCard cardParameters in constructionCards)
		{
			for(byte i = 0; i < cardParameters.qnty; i++)
				deck.Add(cardParameters);
		}

		foreach(GameEffectCard cardParameters in effectCards)
		{
			for(byte i = 0; i < cardParameters.qnty; i++)
				deck.Add(cardParameters);
		}

		#region CONFLICT DECK
		foreach(GameConflictCard cardParameters in conflictCards)
		{
			for(byte i = 0; i < cardParameters.qnty; i++)
				conflictDeck.Add(cardParameters);
		}
		#endregion


		ShuffleDeck();
		ShuffleConflictDeck();
		//DrawCards(GameController.InitialCards);
	}

	public void ShuffleDeck()
	{
		for (byte i = 0; i < deck.Count; i++) 
		{
			GameCard temp = deck[i];
			int randomIndex = UnityEngine.Random.Range(i, deck.Count);
			deck[i] = deck[randomIndex];
			deck[randomIndex] = temp;
		}
	}

	public void ShuffleConflictDeck()
	{
		for (byte i = 0; i < conflictDeck.Count; i++) 
		{
			GameConflictCard temp = conflictDeck[i];
			int randomIndex = UnityEngine.Random.Range(i, conflictDeck.Count);
			conflictDeck[i] = conflictDeck[randomIndex];
			conflictDeck[randomIndex] = temp;
		}
	}

	public void DrawCards(int cardsToDraw)
	{
		StartCoroutine(DrawCards(cardsToDraw, timeDrawing * 0.45f));
	}

	private IEnumerator DrawCards(int cardsToDraw, float timeBetweenDraw)
	{
		if(StartDrawCards != null)
			StartDrawCards();

		if(deck.Count == 0)
			RecicleCards();

		for(byte i = 0; i < cardsToDraw; i++)
		{
			GameCard gameCard = deck[0] as GameCard;
			
			GameObject card = null;
			
			if(gameCard.GetType() == typeof(GameConstructionCard))
				card = constructionCardDefault;
			else if(gameCard.GetType() == typeof(GameEffectCard))
				card = effectCardDefault;
			
			card = Instantiate(card) as GameObject;
			Card cardComponent = card.GetComponent<Card>();
			
			cardComponent.originalCard = gameCard;
			
			cardComponent.nome = deck[0].nome;
			cardComponent.description = deck[0].description;
			cardComponent.image = deck[0].image;
			cardComponent.cost = deck[0].cost;
			cardComponent.minFame = deck[0].minFame;
			cardComponent.moneyReward = deck[0].moneyReward;
			cardComponent.fameReward = deck[0].fameReward;
			cardComponent.cooldown = deck[0].cooldown;
			
			if(gameCard.GetType() == typeof(GameConstructionCard))
			{
				ConstructionCard constructionCardComponent = cardComponent as ConstructionCard;
				GameConstructionCard gameConstructionCard = deck[0] as GameConstructionCard;
				
				constructionCardComponent.specialEffect = gameConstructionCard.specialEffect;
				constructionCardComponent.specialEffectValue = gameConstructionCard.specialEffectValue;
				constructionCardComponent.constructionType = gameConstructionCard.constructionType;
				constructionCardComponent.upkeep = gameConstructionCard.upkeep;
				constructionCardComponent.sprite = gameConstructionCard.sprite;
				constructionCardComponent.level = gameConstructionCard.level;
			}
			else if(gameCard.GetType() == typeof(GameEffectCard))
			{
				EffectCard effectCardComponent = cardComponent as EffectCard;
				GameEffectCard gameEffectCard = deck[0] as GameEffectCard;
				
				effectCardComponent.specialEffect = gameEffectCard.specialEffect;
				effectCardComponent.specialEffectValue = gameEffectCard.specialEffectValue;
				effectCardComponent.effectType = gameEffectCard.effectType;
			}
			
			cardsInHand.Add(cardComponent);
			card.transform.parent = hand.transform;
			card.transform.localPosition = spawnPosition.localPosition;
			card.transform.localScale = Vector3.one;
			
			deck.RemoveAt(0);

			ArrangeDepths();
			//ArrangeHand();

			yield return new WaitForSeconds(timeBetweenDraw);
		}

		bool cardsFinishedTweening = false;
		while(!cardsFinishedTweening)
		{
			cardsFinishedTweening = true;
			//wait for last card to stop tweening
			foreach(Card card in cardsInHand)
			{
				if(card.IsTweening)
				{
					cardsFinishedTweening = false;
					break;
				}
			}
			yield return null;
		}

		if(FinishDrawCards != null)
			FinishDrawCards();

		Debug.Log("Cards Left: " + deck.Count);
	}

	public Vector3 GetPlacePosition(Card card)
	{
		Vector3 pos = Vector3.zero;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(cardsInHand[i] == card)
			{
				BoxCollider collider = hand.GetComponent<BoxCollider>();
				float cellWidth = Mathf.Min(collider.size.x / (Mathf.Max(i, 1)), maxCardSpace);
				float initialPosition = -((cellWidth * (i + 1)) / 2);

				pos = cardsInHand[i].transform.localPosition;
				pos.x = initialPosition + (cellWidth * i);
				pos.y = 0;

			}
		}

		return pos;
	}

	public void ShowConflictCard()
	{
		if(conflictDeck.Count == 0)
			RecicleConflictCards();

		GameConflictCard gameCard = conflictDeck[0] as GameConflictCard;
		
		GameObject card = null;
		
		if(gameCard.type == ConflictCard.ConflictType.Good)
			card = goodConflictDefault;
		else if(gameCard.type == ConflictCard.ConflictType.Bad)
			card = badConflictDefault;
		
		card = Instantiate(card) as GameObject;
		ConflictCard cardComponent = card.GetComponent<ConflictCard>();
		
		cardComponent.nome = conflictDeck[0].nome;
		cardComponent.description = conflictDeck[0].description;
		cardComponent.image = conflictDeck[0].image;
		cardComponent.conflictType = conflictDeck[0].type;
		cardComponent.specialEffect = conflictDeck[0].specialEffect;
		cardComponent.specialEffectValue = conflictDeck[0].specialEffectValue;
		cardComponent.specialEffectValue2 = conflictDeck[0].specialEffectValue2;

		card.transform.parent = conflictCardHolder.transform;
		card.transform.localScale = Vector3.one;

		discardedConflictPile.Add(conflictDeck[0]);
		conflictDeck.RemoveAt(0);

		Debug.Log("Conflict Cards Left: " + conflictDeck.Count);
		Debug.Log("Next Conflict Card: " + conflictDeck[0].ToString());
	}

	private void RecicleCards()
	{
		Debug.Log("Out of cards. Recicling...");
		deck = new List<GameCard>(discardedPile);
		discardedPile.Clear();

		ShuffleDeck();
	}

	private void RecicleConflictCards()
	{
		Debug.Log("Out of conflict cards. Recicling...");
		conflictDeck = new List<GameConflictCard>(discardedConflictPile);
		discardedConflictPile.Clear();
		
		ShuffleConflictDeck();
	}

	private void NewConstruction(Card card)
	{
		cardsInHand.Remove(card);

		ArrangeHand();
	}

	private void NewEffect(Card card)
	{
		cardsInHand.Remove(card);
		
		ArrangeHand();
	}

	private void CardDiscarded(Card card)
	{
		discardedPile.Add(card.originalCard);
		cardsInHand.Remove(card);

		ArrangeHand();
	}

	private void ArrangeDepths()
	{
		for(byte i = 0; i < cardsInHand.Count; i++)
			cardsInHand[i].Depth = 10 + i;
	}

	public void ArrangeHand()
	{
		if(CardsInHand == 0) return;

		int realCardsInHand = 0;
		foreach(Card card in cardsInHand)
		{
			if(!card.IsTweening)
				realCardsInHand++;
		}

		BoxCollider collider = hand.GetComponent<BoxCollider>();
		float cellWidth = Mathf.Min(collider.size.x / (Mathf.Max(realCardsInHand - 1, 1)), maxCardSpace);
		float initialPosition = -((cellWidth * realCardsInHand) / 2);
		int realIndex = 0;

		for(byte i = 0; i < cardsInHand.Count; i++)
		{
			cardsInHand[i].Depth = 10 + i;

			if(cardsInHand[i].IsTweening) continue;

			Vector3 pos = cardsInHand[i].transform.localPosition;
			pos.x = initialPosition + (cellWidth * realIndex);
			pos.y = 0;
			cardsInHand[i].transform.localPosition = pos;

			realIndex++;
		}
	}

	public void DiscardHand()
	{
		for(int i = cardsInHand.Count - 1; i >= 0; i--)
			cardsInHand[i].Discard();

		cardsInHand.Clear();
	}
}

[System.Serializable]
public class GameCard
{
	public string nome;
	public int qnty;
	public string description;
	public Texture2D image;

	public int cost;
	public int minFame;
	public int moneyReward;
	public int fameReward;
	public int cooldown;
}

[System.Serializable]
public class GameConstructionCard : GameCard
{
	public ConstructionCard.SpecialEffect specialEffect;
	public float specialEffectValue;

	public ConstructionCard.ContructionType constructionType;
	public int upkeep;
	public Sprite sprite;
	public int level;
}

[System.Serializable]
public class GameEffectCard : GameCard
{
	public EffectCard.SpecialEffect specialEffect;
	public int specialEffectValue;

	public EffectCard.EffectType effectType;
}

[System.Serializable]
public class GameConflictCard
{
	public string nome;
	public int qnty;
	public string description;
	public Texture2D image;

	public ConflictCard.ConflictType type;
	public ConflictCard.SpecialEffect specialEffect;
	public float specialEffectValue;
	public float specialEffectValue2;

	public override string ToString ()
	{
		return string.Format("Type: {0} \n" +
		                     "Special Effect: {1} \n" +
		                     "Special Effect Value: {2}",
		                     type,
		                     specialEffect,
		                     specialEffectValue);
	}
}