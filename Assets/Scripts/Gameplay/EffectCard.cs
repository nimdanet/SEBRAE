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
	}

	public enum SpecialEffect
	{
		None,
	}

	public EffectType effectType;
	public SpecialEffect specialEffect;
	public int specialEffectValue;

	private UILabel cardDescription;

	protected override void Start ()
	{
		base.Start ();

		cardDescription = transform.FindChild("Front").FindChild("Description").GetComponent<UILabel>();

		cardDescription.text = GetDescription();
	}

	public void OnPlayed()
	{

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

		return s;
	}
}
