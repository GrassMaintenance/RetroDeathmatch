using TMPro;
using Photon.Pun;
using UnityEngine;

public class PlayerGUI : MonoBehaviourPunCallbacks
{
	public static PlayerGUI Instance;
	[SerializeField] private GameObject panel;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private TMP_Text ammoText;
	[SerializeField] private TMP_Text healthText;

	private void Awake()
	{
		Instance = this;
		Gun.Instance.OnGunUpdate += UpdateAmmo;
		playerController.OnHealthChange += UpdateHealth;
		WeaponSwitch.Instance.OnWeaponSwitch += UpdateAmmo;
	}

	public void UpdateAmmo(int clip, int reserveAmmo)
	{
		ammoText.text = $"{clip} | {reserveAmmo}";
	}

	public void UpdateHealth(int health)
	{
		healthText.text = health.ToString();
	}
}
