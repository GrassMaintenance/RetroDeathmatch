using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	[SerializeField] GameObject graphics;
	private void Start() {
		graphics.SetActive(false);
	}
}
