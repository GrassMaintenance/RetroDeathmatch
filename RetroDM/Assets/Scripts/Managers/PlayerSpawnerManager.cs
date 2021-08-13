using UnityEngine;

public class PlayerSpawnerManager : MonoBehaviour{

	public static PlayerSpawnerManager Instance;
	SpawnPoint[] spawnPoints;

	private void Awake() {
		Instance = this;
		spawnPoints = GetComponentsInChildren<SpawnPoint>();
	}

	public Transform GetSpawnpoint() {
		return spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform;
	}
}
