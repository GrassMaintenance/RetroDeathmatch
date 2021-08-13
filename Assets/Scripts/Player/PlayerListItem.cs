using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
	[SerializeField] TMP_Text playerName;
	Player player;

	public void Setup(Player player)
	{
		this.player = player;
		playerName.text = player.NickName;
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (player == otherPlayer) Destroy(gameObject);
	}

	public override void OnLeftRoom()
	{
		Destroy(gameObject);
	}
}
