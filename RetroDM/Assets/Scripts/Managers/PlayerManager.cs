using Photon.Pun;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
	private PhotonView PV;
	private GameObject controller;

	private void Awake() {
		PV = GetComponent<PhotonView>();
	}

	private void Start() {
		if (PV.IsMine) {
			CreateController();
		}
	}

	private void CreateController() {
		Transform spawnpoint = PlayerSpawnerManager.Instance.GetSpawnpoint();
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
	}

	public void Die() {
		PhotonNetwork.Destroy(controller);
		CreateController();
	}
}
