using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSpawnController : MonoBehaviour 
{
	public List<GameObject> characters;

	public Transform waypoints;

	private RandomBetweenTwoConst spawnTime;

	// Use this for initialization
	void Start () 
	{
		spawnTime = new RandomBetweenTwoConst();
		spawnTime.min = 5f;
		spawnTime.max = 10f;

		SpawnCharacter();
	}

	private IEnumerator SpawnCharacter(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		SpawnCharacter();
	}

	private void SpawnCharacter()
	{
		GameObject characterToSpawn = characters[Random.Range(0, characters.Count)];
		int waypointNumber = Random.Range(1, (int)(waypoints.childCount / 2) + 1);

		GameObject character = Instantiate(characterToSpawn) as GameObject;
		character.GetComponent<Character>().waypoint = waypointNumber;

		StartCoroutine(SpawnCharacter(spawnTime.Random()));
	}
}

[System.Serializable]
public class RandomBetweenTwoConst
{
	public float min;
	public float max;

	public float Random()
	{
		return UnityEngine.Random.Range(min, max);
	}
}