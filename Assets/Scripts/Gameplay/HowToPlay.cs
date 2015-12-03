using UnityEngine;
using System;
using System.Collections;

public class HowToPlay : MonoBehaviour 
{
	#region Action
	public static event Action OnOpen;
	public static event Action OnClose;
	#endregion

	public TweenScale tween;

	#region singleton
	private static HowToPlay instance;
	public static HowToPlay Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<HowToPlay>();

			return instance;
		}
	}
	#endregion

	// Use this for initialization
	void Start () 
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

	public void StartGame()
	{
		if(!GameController.IsGameStarted)
		{
			if(tween.direction == AnimationOrTween.Direction.Reverse)
				GameController.Instance.StartGame();
		}
	}
}
