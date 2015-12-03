using UnityEngine;
using System;
using System.Collections;

public class Popup : MonoBehaviour 
{
	#region action
	private static event Action _okAction;
	public static event Action OnShow;
	public static event Action OnHide;
	#endregion

	#region get / set
	public static bool IsActive
	{
		get { return Instance.gameObject.activeInHierarchy; }
	}
	#endregion

	#region singleton
	private static Popup instance;
	public static Popup Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<Popup>();

			return instance;
		}
	}
	#endregion

	public UILabel textLabel;
	public GameObject okButton;

	// Use this for initialization
	void Start () 
	{
		instance = this;

		Hide();
	}

	public static void ShowBlank(string text)
	{
		HideAllButtons();

		Instance.textLabel.text = text;

		Show ();
	}

	public static void ShowOk(string text)
	{
		ShowOk(text, null);
	}

	public static void ShowOk(string text, Action okAction)
	{
		HideAllButtons();
		Instance.okButton.SetActive(true);

		Instance.textLabel.text = text;

		if(okAction != null)
			_okAction = okAction;

		Show ();
	}

	public void OkAction()
	{
		if(_okAction != null)
			_okAction();

		_okAction = null;

		Hide ();
	}

	private static void Show()
	{
		Instance.gameObject.SetActive(true);

		if(OnShow != null)
			OnShow();
	}

	public static void Hide()
	{
		Instance.gameObject.SetActive(false);

		if(OnHide != null)
			OnHide();
	}

	private static void HideAllButtons()
	{
		Instance.okButton.SetActive(false);
	}
}
