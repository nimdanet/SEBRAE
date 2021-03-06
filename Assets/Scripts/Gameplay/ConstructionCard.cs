﻿using UnityEngine;
using System.Collections;

public class ConstructionCard : Card 
{
	public enum ContructionType
	{
		Cozinha,
		Salao,
		Lobby,
		Dispensa,
		Administrativo,
		Banheiro,
	}

	public enum SpecialEffect
	{
		None,
		ManutencaoCozinha,
		Lucro,
		Fama,
	}
	
	public ContructionType constructionType;
	public int level;
	public int upkeep;
	public Sprite sprite;

	public SpecialEffect specialEffect;
	public float specialEffectValue;

	public AudioClip sound;

	protected UILabel upkeepLabel;

	#region get / set
	public bool IsReady
	{
		get { return cooldown <= 0; }
	}
	#endregion

	protected override void Start ()
	{
		base.Start ();

		cardType = Card.Type.Construction;

		cooldownLabel = transform.FindChild("Front").FindChild("Upkeep").FindChild("Label").GetComponent<UILabel>();

		cardName.text += " ";

		for(int i = 0; i < level; i++)
			cardName.text += "I";

		cooldownLabel.text = upkeep.ToString();

		int parameter = (specialEffect == SpecialEffect.ManutencaoCozinha) ? (int)Mathf.Abs (specialEffectValue) : (int)(specialEffectValue * 100);
		cardDescription.text = string.Format(Localization.Get(description), parameter);

	}

	public override string ToString ()
	{
		return string.Format ("{0} Level {1}", nome, level);
	}
}
