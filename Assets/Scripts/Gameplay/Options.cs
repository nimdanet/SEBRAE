using UnityEngine;
using System;
using System.Collections;

public class Options : MonoBehaviour 
{
	#region Action
	public static event Action OnOpen;
	public static event Action OnClose;
	#endregion
	
	public TweenScale tween;
	
	#region singleton
	private static Options instance;
	public static Options Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<Options>();
			
			return instance;
		}
	}
	#endregion

	void Start()
	{
		tween.transform.localScale = Vector3.zero;
	}
	
	public void OpenClose()
	{
		if(tween.direction == AnimationOrTween.Direction.Forward)
			Close();
		else
			Open();
	}
	
	public void Open()
	{
		tween.PlayForward();
		
		if(OnOpen != null)
			OnOpen();
	}
	
	public void Close()
	{
		tween.PlayReverse();
		
		if(OnClose != null)
			OnClose();
	}

	public void SaveGame()
	{
		StartCoroutine(SaveController.Save());

		Popup.ShowOk(Localization.Get("SALVO"));
	}

	public void ExitGame()
	{
		Application.LoadLevel("Login");
	}
}
