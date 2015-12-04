using UnityEngine;
using System.Collections;

public class Neon : MonoBehaviour 
{
	public float timeBlink;

	public UILabel apagado;
	public UILabel aceso;
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		yield return new WaitForSeconds(timeBlink);

		aceso.enabled = !aceso.enabled;

		StartCoroutine(Blink());
	}
}
