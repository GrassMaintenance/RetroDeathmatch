using Photon.Pun;
using System;
using System.Collections.Generic;

public class WeaponSwitch : MonoBehaviourPunCallbacks {
	public static WeaponSwitch Instance;
	public event Action<int, int> OnWeaponSwitch;
	private PlayerControls controls;
	private int currentWeapon = 0;
	private List<Gun> guns = new List<Gun>();
	private PhotonView PV;

	private void Awake() {
		controls = new PlayerControls();
		Instance = this;
		PV = GetComponent<PhotonView>();
		for(int i = 0; i < transform.childCount; i++) {
			Gun gun = transform.GetChild(i).GetComponent<Gun>();
			guns.Add(gun);
		}		
	}

	public override void OnEnable() => controls.Enable();

	public override void OnDisable() => controls.Disable();

    private void Update() {
		if (!PV.IsMine) { return; }
		float mouseScrollValue = controls.Controls.WeaponSwitch.ReadValue<float>();
		GetWeapon(mouseScrollValue);
	}

	private void GetWeapon(float mouseScrollValue) {
		if (mouseScrollValue > 0) {
			if (currentWeapon >= transform.childCount - 1) {
				currentWeapon = 0;
			} else {
				currentWeapon++;
			}
		} else if (mouseScrollValue < 0) {
			if (currentWeapon <= 0) {
				currentWeapon = transform.childCount - 1;
			} else {
				currentWeapon--;
			}
		}

		PV.RPC("SwitchWeapon", RpcTarget.All, currentWeapon);
	}

	[PunRPC]
	public void SwitchWeapon(int currentWeapon) {
		for(int i = 0; i < guns.Count; i++) { 
			if (currentWeapon == i) {
				guns[i].gameObject.SetActive(true);
			} else {
				guns[i].gameObject.SetActive(false);
			}
		}
		OnWeaponSwitch?.Invoke(guns[currentWeapon].clip, guns[currentWeapon].reserveAmmo);
	}
}
