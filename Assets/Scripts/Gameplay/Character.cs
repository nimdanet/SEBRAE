using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	public float vel = 2f;
	public float tempoVisita = 8f;

	[HideInInspector]
	public int waypoint;

	private Transform waypointFrom;
	private Transform waypointTo;

	private Animator myAnimator;
	private Rigidbody2D myRigidbody;
	private SpriteRenderer mySpriteRenderer;

	// Use this for initialization
	void Start () 
	{
		myAnimator = GetComponent<Animator>();
		myRigidbody = GetComponent<Rigidbody2D>();
		mySpriteRenderer = GetComponent<SpriteRenderer>();

		StartCoroutine(FollowWaypoint());
	}

	private IEnumerator FollowWaypoint()
	{
		while(waypoint == 0)
			yield return null;

		mySpriteRenderer.sortingOrder = waypoint;

		waypointFrom = GameObject.Find("Waypoint Begin " + waypoint).transform;
		waypointTo = (waypoint == 1) ? GameObject.Find("Waypoint Restaurante").transform : GameObject.Find("Waypoint Finish " + waypoint).transform;

		transform.position = waypointFrom.position;

		if(waypointFrom.position.y > waypointTo.position.y)
			myAnimator.SetInteger("State", 0);
		else
			myAnimator.SetInteger("State", 1);

		float angle = Mathf.Atan2(waypointTo.position.y - waypointFrom.position.y, waypointTo.position.x - waypointFrom.position.x);
		myRigidbody.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * vel;

		while(Vector3.Distance(transform.position, waypointTo.position) > 0.1f)
			yield return null;

		Debug.Log(waypoint);
		if(waypoint != 1)
		{
			Destroy(gameObject);
			yield break;
		}

		Debug.Log("Entrou!");
		myRigidbody.velocity = Vector2.zero;
		myAnimator.SetInteger("State", 2);

		//fade out
		while(mySpriteRenderer.color.a > 0)
		{
			Color c = mySpriteRenderer.color;
			c.a -= Time.deltaTime;
			mySpriteRenderer.color = c;
			yield return null;
		}

		yield return new WaitForSeconds(tempoVisita);

		myAnimator.SetInteger("State", 3);
		//fade in
		while(mySpriteRenderer.color.a < 1)
		{
			Color c = mySpriteRenderer.color;
			c.a += Time.deltaTime;
			mySpriteRenderer.color = c;
			yield return null;
		}

		waypointTo = GameObject.Find("Waypoint Finish " + waypoint).transform;

		angle = Mathf.Atan2(waypointTo.position.y - waypointFrom.position.y, waypointTo.position.x - waypointFrom.position.x);
		myRigidbody.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * vel;

		myAnimator.SetInteger("State", 0);

		while(Vector3.Distance(transform.position, waypointTo.position) > 0.1f)
			yield return null;

		Destroy(gameObject);
	}
}
