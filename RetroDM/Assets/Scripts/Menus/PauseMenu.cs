using Photon.Pun;
using System;
using UnityEngine;

public class PauseMenu : MonoBehaviourPunCallbacks
{
	private bool isEnabled = false;
	[SerializeField] private GameObject panel;

	private void Start()
	{
		panel.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			OpenPauseMenu(this, EventArgs.Empty);
		}
	}

	private void OpenPauseMenu(object sender, EventArgs e)
	{
		isEnabled = !isEnabled;
		if (isEnabled)
		{
			panel.SetActive(isEnabled);
			Cursor.lockState = CursorLockMode.None;
		} 
		else
		{
			panel.SetActive(isEnabled);
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	public void Resume()
	{
		isEnabled = false;
		Cursor.lockState = CursorLockMode.Locked;
		panel.SetActive(isEnabled);
	}

	public void OpenSettings()
	{
		Debug.Log("Opened settings.");
	}

	public void LeaveRoom()
	{
		Destroy(GameObject.Find("RoomManager"));
		Destroy(GameObject.Find("AmmoPickup"));
		Destroy(GameObject.Find("HealthPickup"));
		PhotonNetwork.Disconnect();
		PhotonNetwork.ConnectUsingSettings();
		PhotonNetwork.LoadLevel(0);
	}
}
