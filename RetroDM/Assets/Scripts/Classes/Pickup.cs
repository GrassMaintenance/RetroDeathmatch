using Photon.Pun;
using System;
using UnityEngine;

public class Pickup : MonoBehaviourPunCallbacks {
	[SerializeField] private AudioClip ammoPickupClip;
	[SerializeField] private AudioClip healthPickupClip;
	[SerializeField] private float amplitude = 0.25f;
	private PhotonView PV;


	private void Update() {
		transform.Translate(Vector3.up * Mathf.Sin(Time.time) * Time.deltaTime * amplitude, Space.World);
		transform.Rotate(Vector3.up * Time.deltaTime * 15, Space.World);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.transform.CompareTag("Player")) {
			PlayerController playerController = other.GetComponent<PlayerController>();
			Gun gun = playerController.GetComponentInChildren<Gun>();
			switch (transform.name) {
				case "HealthPickup":
					playerController.AddHealth(this, EventArgs.Empty);
					AudioSource.PlayClipAtPoint(healthPickupClip, transform.position);
					break;
				case "AmmoPickup":
					gun.AddAmmo(this, EventArgs.Empty);
					AudioSource.PlayClipAtPoint(ammoPickupClip, transform.position);
					break;
			}
			gameObject.SetActive(false);
			Timer.SetTimer(3f, () => RespawnItem());
		}
	}

	private void RespawnItem() {
		gameObject.SetActive(true);
		gameObject.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
	}
}
