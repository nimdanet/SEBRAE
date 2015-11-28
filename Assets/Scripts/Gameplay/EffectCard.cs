using UnityEngine;
using System.Collections;

public class EffectCard : Card 
{
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
	}

	public enum SpecialEffect
	{
		None,
		Emprestimo,
	}

	public EffectType effectType;
	public SpecialEffect specialEffect;
	public int specialEffectValue;

	protected override void Start ()
	{
		base.Start ();

		cardDescription.text = GetDescription();

		if(specialEffect == SpecialEffect.Emprestimo)
			rewardMoneyLabel.text = specialEffectValue.ToString();
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

		Discard();
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

		return s;
	}
}
