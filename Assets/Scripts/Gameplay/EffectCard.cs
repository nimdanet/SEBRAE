using UnityEngine;
using System;
using System.Collections;

public class EffectCard : Card 
{
	#region Action
	public static event Action OnWaitingForSelect;
	#endregion

	public enum EffectType
	{
		InvestimentoBaixo,
		InvestimentoAlto,
		MarketingDigitalFraco,
		MarketingDigitalForte,
		MarketingTradicionalFraco,
		MarketingTradicionalForte,
		IngredientesDeQualidade,
		IngredientesDeAltaQualidade,
		UmDiaBom,
		UmDiaExcelente,
		SaiuNoJornal,
		EmprestimoBancarioPequeno,
		EmprestimoBancarioMedio,
		EmprestimoBancarioGrande,
		CompraCartas,
		DescartaCompraCartas,
		DestroiConstrucao,
		DiminuiCooldown,
		None,
	}

	public enum SpecialEffect
	{
		None,
		Emprestimo,
	}

	public EffectType effectType;
	public SpecialEffect specialEffect;
	public int specialEffectValue;

	private string errmsg;

	#region get / set
	public string ErrorMessage
	{
		get { return errmsg; }
	}
	#endregion

	protected override void Start ()
	{
		base.Start ();

		cardDescription.text = GetDescription();

		if(specialEffect == SpecialEffect.Emprestimo)
			rewardMoneyLabel.text = specialEffectValue.ToString();
	}

	public bool CanBePlayed()
	{
		if(effectType == EffectType.DescartaCompraCartas && DeckController.CardsInHand - 1 < specialEffectValue)
		{
			errmsg = "Você não tem cartas suficientes na mão para descartar";
			return false;
		}
		if(effectType == EffectType.DestroiConstrucao)
		{
			bool hasConstruction = false;
			foreach(ConstructionArea cArea in GameController.ConstructionAreas)
			{
				if(cArea.constructionCard != null)
					hasConstruction = true;
			}

			if(!hasConstruction)
				errmsg = "Você não possui nenhuma construção para destruir.";

			return hasConstruction;
		}

		if(effectType == EffectType.DiminuiCooldown)
		{
			bool hasConstruction = false;
			foreach(ConstructionArea cArea in GameController.ConstructionAreas)
			{
				if(cArea.constructionCard != null && cArea.constructionCard.cooldown > 0)
					hasConstruction = true;
			}
			
			if(!hasConstruction)
				errmsg = "Você não possui nenhuma construção com cooldown maior que 0 para diminuir.";
			
			return hasConstruction;
		}

		return true;
	}

	public void OnPlayed()
	{
		if(specialEffect == SpecialEffect.Emprestimo)
			GameController.Money += specialEffectValue;
	}

	public void OnReady()
	{
		GameController.Money += moneyReward;
		GameController.Fame += fameReward;

		if(effectType == EffectType.CompraCartas)
			DeckController.Instance.DrawCards(specialEffectValue);
		else if(effectType == EffectType.DescartaCompraCartas)
		{
			GameController.activeCardEffect = effectType;
			GameController.effectCardValue = specialEffectValue;
			TrashCan.OnDiscarded += CardDiscarded;
			Popup.ShowBlank("Descarte " + GameController.effectCardValue + " cartas");
			return;
		}
		else if(effectType == EffectType.DestroiConstrucao || effectType == EffectType.DiminuiCooldown)
		{
			GameController.activeCardEffect = effectType;
			GameController.effectCardValue = specialEffectValue;

			ConstructionArea.OnAreaSelected += AreaSelected;

			if(OnWaitingForSelect != null)
				OnWaitingForSelect();

			return;
		}
		Discard();
	}

	public void AreaSelected(ConstructionArea cArea)
	{
		if(effectType == EffectType.DestroiConstrucao)
			cArea.Discard();

		else if(effectType == EffectType.DiminuiCooldown)
			cArea.DiminuiCooldown(specialEffectValue);

		GameController.activeCardEffect = EffectType.None;

		ConstructionArea.OnAreaSelected -= AreaSelected;

		Discard();
	}

	private void CardDiscarded(Card c)
	{
		GameController.effectCardValue--;

		Popup.ShowBlank("Descarte " + GameController.effectCardValue + " carta(s)");

		if(GameController.effectCardValue == 0)
		{
			GameController.activeCardEffect = EffectType.None;
			TrashCan.OnDiscarded -= CardDiscarded;

			DeckController.Instance.DrawCards(specialEffectValue);

			Popup.Hide();

			Discard();
		}
	}

	private string GetDescription()
	{
		string s = "";

		if(effectType == EffectType.MarketingTradicionalForte || effectType == EffectType.IngredientesDeQualidade || 
		   effectType == EffectType.IngredientesDeAltaQualidade)
			s = string.Format(description, fameReward, moneyReward, cooldown);

		else if(effectType == EffectType.InvestimentoBaixo || effectType == EffectType.InvestimentoAlto ||
		        effectType == EffectType.UmDiaBom || effectType == EffectType.UmDiaExcelente)
			s = string.Format(description, moneyReward, cooldown);

		else if(effectType == EffectType.MarketingDigitalFraco || effectType == EffectType.MarketingDigitalForte || 
		        effectType == EffectType.MarketingTradicionalFraco || effectType == EffectType.SaiuNoJornal)
			s = string.Format(description, fameReward, cooldown);

		else if(effectType == EffectType.EmprestimoBancarioPequeno || effectType == EffectType.EmprestimoBancarioMedio ||
		        effectType == EffectType.EmprestimoBancarioGrande)
			s = string.Format(description, specialEffectValue, cooldown, Mathf.Abs(moneyReward));

		else if(effectType == EffectType.DescartaCompraCartas)
			s = string.Format(description, specialEffectValue, specialEffectValue);

		else if(effectType == EffectType.CompraCartas || effectType == EffectType.DiminuiCooldown)
			s = string.Format(description, specialEffectValue);

		else
			s = description;

		return s;
	}
}
