using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TrashCan : MonoBehaviour 
{
	#region action 
	public static event Action<Card> OnDiscarded;
	#endregion

	public static void Discard(Card card)
	{
		if(OnDiscarded != null)
			OnDiscarded(card);

		Destroy(card.gameObject);
	}
}
