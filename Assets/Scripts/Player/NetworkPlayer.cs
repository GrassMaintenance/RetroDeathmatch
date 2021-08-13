using UnityEngine;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviourPunCallbacks, IPunObservable {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	PhotonView PV;

	private void Awake() {
		PV = GetComponent<PhotonView>();
	}

	private void Update() {
		if (PV.IsMine) return;
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){ 
		if (stream.IsWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		} else {
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
		}
	}
}
