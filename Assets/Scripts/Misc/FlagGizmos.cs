using UnityEngine;
using System.Collections;

public class FlagGizmos : MonoBehaviour 
{
	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, 0.2f);
	}
}
