using UnityEngine;
using System;
using System.Collections;

public class DragDropCard : UIDragDropItem 
{
	#region Action
	public static event Action<Card> OnDragStarted;
	public static event Action<Card> OnDropped;
	public static event Action<Card> OnDragMove;
	#endregion

	public static Card current;
	private Vector3 originalPosition;

	public LayerMask layerMask;

	protected override void OnDragDropStart ()
	{
		base.OnDragDropStart ();

		originalPosition = transform.localPosition;

		current = GetComponent<Card>();
		if(OnDragStarted != null)
			OnDragStarted(GetComponent<Card>());
	}

	protected override void OnDragDropEnd ()
	{
		base.OnDragDropEnd ();

		transform.localPosition = originalPosition;

		if(OnDropped != null)
			OnDropped(current);

		current = null;
	}

	protected override void OnDragDropMove (Vector2 delta)
	{
		base.OnDragDropMove (delta);

		if(OnDragMove != null)
			OnDragMove(current);
	}

	protected override void OnDragDropRelease (GameObject surface)
	{
		if (surface != null)
		{
			TrashCan trashCan = surface.GetComponent<TrashCan>();

			//dropped on discard pile
			if(trashCan != null)
				GetComponent<Card>().Discard();
		}
		base.OnDragDropRelease(surface);
	}
}
