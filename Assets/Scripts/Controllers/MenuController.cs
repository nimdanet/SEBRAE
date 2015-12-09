using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour 
{
	public UILabel nameLabel;
	public UILabel emailLabel;

	public UILabel errorLabel;

	public GameObject loginButton;
	public GameObject jogarButton;
	public GameObject continueButton;

	void Start()
	{
		errorLabel.gameObject.SetActive(false);

		foreach(UIButton button in loginButton.GetComponents<UIButton>())
			button.isEnabled = true;

		foreach(UIButton button in jogarButton.GetComponents<UIButton>())
			button.isEnabled = false;

		foreach(UIButton button in continueButton.GetComponents<UIButton>())
			button.isEnabled = false;

		SoundController.PlayMusic(SoundController.Music.Menu);
	}

	public void Login()
	{
		if(nameLabel.text == "")
		{
			errorLabel.gameObject.SetActive(true);
			errorLabel.text = Localization.Get("PREENCHER_NOME");
			return;
		}

		if(emailLabel.text == "")
		{
			errorLabel.gameObject.SetActive(true);
			errorLabel.text = Localization.Get("PREENCHER_EMAIL");
			return;
		}

		errorLabel.gameObject.SetActive(true);
		errorLabel.color = Color.green;
		errorLabel.text = Localization.Get("CARREGANDO") + "...";

		SaveController.SetUser(nameLabel.text, emailLabel.text);

		StartCoroutine(LoadGame());
	}

	private IEnumerator LoadGame()
	{
		foreach(UIButton button in loginButton.GetComponents<UIButton>())
			button.isEnabled = false;

		yield return StartCoroutine(SaveController.Load());

		errorLabel.text = Localization.Get("CARREGADO");

		CheckSavedGame();
	}

	public void Jogar()
	{
		SaveController.loadFromSave = false;
		Application.LoadLevel("Gameplay");
	}

	public void Continuar()
	{
		SaveController.loadFromSave = true;
		Application.LoadLevel("Gameplay");
	}

	private void CheckSavedGame()
	{
		foreach(UIButton button in loginButton.GetComponents<UIButton>())
			button.isEnabled = false;
		
		foreach(UIButton button in jogarButton.GetComponents<UIButton>())
			button.isEnabled = true;
		
		foreach(UIButton button in continueButton.GetComponents<UIButton>())
			button.isEnabled = SaveController.HasSavedData;
	}
}
