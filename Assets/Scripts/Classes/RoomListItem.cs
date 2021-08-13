using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomListItem : MonoBehaviour
{
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] TMP_Text playerCountText;
	RoomInfo info;

	public void Setup(RoomInfo info)
	{
		this.info = info;
		roomNameText.text = info.Name;
		playerCountText.text = info.PlayerCount + "/" + info.MaxPlayers;
	}

	public void OnClick()
	{
		Launcher.Instance.JoinRoom(info);
	}
}
