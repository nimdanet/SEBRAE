using UnityEngine;
using System;
using System.Collections;

public class ConflictCard : MonoBehaviour 
{
	#region Action
	public static event Action<SpecialEffect> OnActive;
	public static event Action OnShow;
	public static event Action OnHide;
	#endregion

	public enum ConflictType
	{
		Good,
		Bad,
	}

	public enum SpecialEffect
	{
		Gold,
		Fame,
		ProfitGold,
		ProfitFama,
		ProfitAll,
		Saloes,
		Cozinhas,
		AdministrativoBanheiro,
		DispensaLobby,
		DescartaCartas,
		Destroy,
		VoltaCooldown,
		CompletaCartas,
		ZeraCooldown,
		None,
	}

	#region get / set
	public static bool IsActive
	{
		get { return active; }
	}
	#endregion

	public string nome;
	public string description;
	public Texture2D image;

	public ConflictType conflictType;
	public SpecialEffect specialEffect;
	public float specialEffectValue;

	private static bool active;


	void Start()
	{
		UILabel cardName = transform.FindChild("Front").FindChild("Title").GetComponent<UILabel>();
		UISprite cardImage = transform.FindChild("Front").FindChild("Image").GetComponent<UISprite>();
		UILabel cardDescription = transform.FindChild("Front").FindChild("Description").GetComponent<UILabel>();

		cardName.text = nome;
		cardDescription.text = string.Format(description, Mathf.Abs(specialEffectValue));

		if(image != null)
			cardImage.mainTexture = image;

		active = true;

		if(OnShow != null)
			OnShow();
	}

	public void AtivaEfeito()
	{
		Debug.Log("Conflict card effect!: " + specialEffect);

		switch(specialEffect)
		{
			case SpecialEffect.Gold:
				GameController.Money += (int)specialEffectValue;
			break;

			case SpecialEffect.Fame:
				GameController.Fame += (int)specialEffectValue;
			break;

			case SpecialEffect.ProfitGold:
				GameController.moneyMultiplier = specialEffectValue;
			break;

			case SpecialEffect.ProfitFama:
				GameController.fameMultiplier = specialEffectValue;
			break;

			case SpecialEffect.ProfitAll:
				GameController.moneyMultiplier = specialEffectValue;
				GameController.fameMultiplier = specialEffectValue;
			break;

			case SpecialEffect.Cozinhas:
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard == null) continue;

					if(cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Cozinha)
						cArea.Inactivate();
				}
			break;

			case SpecialEffect.Saloes:
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard == null) continue;

					if(cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Salao)
						cArea.Inactivate();
				}
			break;

			case SpecialEffect.AdministrativoBanheiro:
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard == null) continue;

					if(cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Banheiro ||
				   	   cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Administrativo)
						cArea.Inactivate();
				}
			break;

			case SpecialEffect.DispensaLobby:
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard == null) continue;

					if(cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Dispensa ||
				   	   cArea.constructionCard.constructionType == ConstructionCard.ContructionType.Lobby)
						cArea.Inactivate();
				}
			break;

			case SpecialEffect.Destroy:
				//verify if there is any construction in game
				bool hasConstruction = false;
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard != null)
					{
						 hasConstruction = true;
						break;
					}
				}

				if(hasConstruction)
				{
					ConstructionArea cArea = null;
					do
					{
						int rnd = UnityEngine.Random.Range(0, GameController.ConstructionAreas.Length);
						cArea = GameController.ConstructionAreas[rnd];
					}
					while(cArea.constructionCard == null);

					cArea.Discard();
				}
			break;

			case SpecialEffect.DescartaCartas:
				DeckController.Instance.DiscardHand();
			break;

			case SpecialEffect.CompletaCartas:
				DeckController.Instance.DrawCards(GameController.Instance.maxCardsInHand - DeckController.CardsInHand);
			break;

			case SpecialEffect.ZeraCooldown:
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard != null && cArea.constructionCard.cooldown > 0)
						cArea.ZeraCooldown();
				}
			break;

			case SpecialEffect.VoltaCooldown:
				//verify if there is any construction in game
				hasConstruction = false;
				foreach(ConstructionArea cArea in GameController.ConstructionAreas)
				{
					if(cArea.constructionCard != null)
					{
						 hasConstruction = true;
						break;
					}
				}

				if(hasConstruction)
				{
					ConstructionArea cArea = null;
					do
					{
						int rnd = UnityEngine.Random.Range(0, GameController.ConstructionAreas.Length);
						cArea = GameController.ConstructionAreas[rnd];
					}
					while(cArea.constructionCard == null);

					cArea.RestoreCooldown();
				}
			break;
		}

		if(OnActive != null)
			OnActive(specialEffect);

		HUDController.Instance.HideConflictCard();

		active = false;

		if(OnHide != null)
			OnHide();

		Destroy(gameObject);
	}
}
