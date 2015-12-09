using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
public class GameData : MonoBehaviour, ISerializable
{
	public bool hasSavedGame;
	public int week;
	public int month;
	public int fame;
	public int money;
	public int upkeep;
	public int moneyReward;
	public int fameReward;
	public bool hasWon;

	public ConflictCard.SpecialEffect activeConflictEffect;
	public float moneyMultiplier;
	public float fameMultiplier;

	public int cardsInHand;
	public int constructionsActive;
	public int effectsActive;

	public List<ConstructionsSaved> constructions;
	public List<CardsSaved> cards;
	public List<EffectSaved> effects;

	protected GameData(SerializationInfo info, StreamingContext context)
	{
		this.hasSavedGame = info.GetBoolean("hasSavedGame");
		this.fame = info.GetInt32("fame");
		this.money = info.GetInt32("money");
		this.upkeep = info.GetInt32("upkeep");
		this.moneyReward = info.GetInt32("moneyReward");
		this.fameReward = info.GetInt32("fameReward");
		this.week = info.GetInt32("week");
		this.month = info.GetInt32("month");
		this.hasWon = info.GetBoolean("hasWon");
		this.cardsInHand = info.GetInt32("cardsInHand");
		this.constructionsActive = info.GetInt32("constructionsActive");
		this.effectsActive = info.GetInt32("effectsActive");
		this.activeConflictEffect = (ConflictCard.SpecialEffect)info.GetInt32("activeConflictEffect");
		this.moneyMultiplier = info.GetSingle("moneyMultiplier");
		this.fameMultiplier = info.GetSingle("fameMultiplier");

		this.constructions = new List<ConstructionsSaved>();
		for(int i = 0; i < constructionsActive; i++)
		{
			ConstructionsSaved cSaved = new ConstructionsSaved();
			cSaved.name = info.GetString("construction" + i + "_name");
			cSaved.level = info.GetInt32("construction" + i + "_level");
			cSaved.cooldown = info.GetInt32("construction" + i + "_cooldown");
			cSaved.row = info.GetInt32("construction" + i + "_row");
			cSaved.column = info.GetInt32("construction" + i + "_column");
			constructions.Add(cSaved);
		}

		this.cards = new List<CardsSaved>();
		for(int i = 0; i < cardsInHand; i++)
		{
			CardsSaved cardSaved = new CardsSaved();
			cardSaved.name = info.GetString("card" + i + "_name");
			cardSaved.level = info.GetInt32("card" + i + "_level");
			cards.Add(cardSaved);
		}

		this.effects = new List<EffectSaved>();
		for(int i = 0; i < effectsActive; i++)
		{
			EffectSaved effectSaved = new EffectSaved();
			effectSaved.name = info.GetString("effect" + i + "_name");
			effectSaved.cooldown = info.GetInt32("effect" + i + "_cooldown");
			effectSaved.position = info.GetInt32("effect" + i + "_position");
			effects.Add(effectSaved);
		}
	}
	
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("hasSavedGame", hasSavedGame);
		info.AddValue("fame", fame);
		info.AddValue("money", money);
		info.AddValue("upkeep", upkeep);
		info.AddValue("moneyReward", moneyReward);
		info.AddValue("fameReward", fameReward);
		info.AddValue("week", week);
		info.AddValue("month", month);
		info.AddValue("cardsInHand", cardsInHand);
		info.AddValue("constructionsActive", constructionsActive);
		info.AddValue("effectsActive", effectsActive);
		info.AddValue("activeConflictEffect", (int)activeConflictEffect);
		info.AddValue("moneyMultiplier", moneyMultiplier);
		info.AddValue("fameMultiplier", fameMultiplier);
		info.AddValue("hasWon", hasWon);

		for(int i = 0; i < constructions.Count; i++)
		{
			info.AddValue("construction" + i + "_name", constructions[i].name);
			info.AddValue("construction" + i + "_level", constructions[i].level);
			info.AddValue("construction" + i + "_cooldown", constructions[i].cooldown);
			info.AddValue("construction" + i + "_row", constructions[i].row);
			info.AddValue("construction" + i + "_column", constructions[i].column);
		}

		for(int i = 0; i < cards.Count; i++)
		{
			info.AddValue("card" + i + "_name", cards[i].name);
			info.AddValue("card" + i + "_level", cards[i].level);
		}

		Debug.Log("effects.Count: " + effects.Count);
		for(int i = 0; i < effects.Count; i++)
		{
			info.AddValue("effect" + i + "_name", effects[i].name);
			info.AddValue("effect" + i + "_cooldown", effects[i].cooldown);
			info.AddValue("effect" + i + "_position", effects[i].position);
		}
	}
}

[System.Serializable]
public class ConstructionsSaved
{
	public string name;
	public int level;
	public int cooldown;
	public int row;
	public int column;
}

[System.Serializable]
public class CardsSaved
{
	public string name;
	public int level;
}

[System.Serializable]
public class EffectSaved
{
	public string name;
	public int cooldown;
	public int position;
}