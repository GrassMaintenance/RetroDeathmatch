using Photon.Pun;
using System;
using UnityEngine;

public class Gun : MonoBehaviourPunCallbacks {
    [HideInInspector] public static Gun Instance;
    [HideInInspector] public int clip, reserveAmmo;
    [HideInInspector] public Action<int, int> OnGunUpdate;
    protected Animator animator;
    protected bool isReloading;
    protected float fireRate, nextFire;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] protected float bloom, RPM, reloadTime;
    [SerializeField] protected int clipCount, clipSize, damage;
    protected GameObject bulletHolePrefab, bloodParticlePrefab, impactParticlePrefab;
    protected RaycastHit hit;
    [SerializeField] private AudioClip[] sounds;
    protected PhotonView PV;
    protected PlayerController player;
    protected PlayerControls controls;
    private AudioSource audioSource;
    private Camera playerCamera;
    private GameObject particlePrefab;
    private Vector3 shootDirection;

    private void Awake() {
        //Get all components and prefabs
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        bulletHolePrefab = Resources.Load("Prefabs/BulletHole") as GameObject;
        bloodParticlePrefab = Resources.Load("Particles/BloodParticle") as GameObject;
        impactParticlePrefab = Resources.Load("Particles/ImpactParticle") as GameObject;
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        player = transform.root.GetComponent<PlayerController>();
        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        PV = GetComponent<PhotonView>();

        //Set this class as an instance
        Instance = this;

        //Setup player input
        controls = new PlayerControls();
        controls.Controls.Reload.performed += _ => Timer.SetTimer(reloadTime, ReloadWeapon);
    }

    protected virtual void Start() {
        clip = clipSize;
        fireRate = 60 / RPM;
        reserveAmmo = clip * clipCount;
        UpdateGun(clip, reserveAmmo);
    }

    protected new void OnEnable() {
        isReloading = false;
        animator.SetBool("isFiring", false);
        UpdateGun(clip, reserveAmmo);
        controls.Enable();
    }

    protected new void OnDisable() => controls.Disable();

    protected void Update() {
        if(!PV.IsMine) { return; }
        if(Cursor.lockState == CursorLockMode.Locked) {
            GetInput();
        }
    }

    protected virtual void GetInput() {
        //Check for mouse click
        bool isFiring = Convert.ToBoolean(controls.Controls.Shoot.ReadValue<float>());
        if(isFiring && !isReloading) {
            PV.RPC("Shoot", RpcTarget.All);
        } else {
            animator.SetBool("isFiring", false);
        }
    }

    [PunRPC]
    public virtual void Shoot() {
        if(clip > 0 && Time.time > nextFire) {
            muzzleFlash.Play();
            animator.SetBool("isFiring", true);
            clip--;
            nextFire = Time.time + fireRate;

            if(!PV.IsMine) {
                AudioSource.PlayClipAtPoint(sounds[0], transform.position);
            } else {
                audioSource.PlayOneShot(sounds[0]);
            }

            if(PV.IsMine) {
                float xSpread = UnityEngine.Random.Range(-bloom, bloom);
                float ySpread = UnityEngine.Random.Range(-bloom, bloom);
                shootDirection = playerCamera.transform.forward + new Vector3(xSpread, ySpread, xSpread);
                UpdateGun(clip, reserveAmmo);
            }

            if(Physics.Raycast(playerCamera.transform.position, shootDirection, out hit, 100)) {
                bool playerTag = hit.transform.tag == "Player";
                PV.RPC("SpawnParticles", RpcTarget.All, hit.point, hit.normal, playerTag);
                hit.collider.GetComponent<IDamageable>()?.TakeDamage(damage);
                if(hit.collider.GetComponent<Rigidbody>() != null)
                    hit.collider.GetComponent<Rigidbody>().AddForce(-hit.normal * 250);                
            }
        }

        if(clip == 0) ReloadWeapon();
    }

    [PunRPC]
    public void SpawnParticles(Vector3 hitPosition, Vector3 hitNormal, bool playerTag) {
        particlePrefab = playerTag ? bloodParticlePrefab : impactParticlePrefab;
        GameObject bulletHole = Instantiate(bulletHolePrefab, hitPosition + hitNormal * 0.0001f, Quaternion.LookRotation(-hitNormal));
        GameObject particleSystem = Instantiate(particlePrefab, hitPosition + hitNormal * 0.0001f, Quaternion.LookRotation(hitNormal));
        Destroy(particleSystem, 2f);
        Destroy(bulletHole, 2f);
    }

    protected virtual void ReloadWeapon() {
        isReloading = true;
        Timer.SetTimer(reloadTime + 0.25f, () => {
            int shotsFired = clipSize - clip;
            if(reserveAmmo > 0) {
                if(shotsFired > reserveAmmo) {
                    clip = reserveAmmo;
                    reserveAmmo = 0;
                } else {
                    reserveAmmo -= shotsFired;
                    clip = clipSize;
                }
            }
            UpdateGun(clip, reserveAmmo);
            isReloading = false;
        });
    }

    public void AddAmmo(object sender, EventArgs e) {
        if(reserveAmmo < clipSize * clipCount) {
            reserveAmmo += clipSize;
            if(reserveAmmo > clipSize * clipCount) {
                reserveAmmo = clipSize * clipCount;
            }
        }
        UpdateGun(clip, reserveAmmo);
    }

    public void UpdateGun(int clip, int reserveAmmo) {
        OnGunUpdate?.Invoke(clip, reserveAmmo);
    }
}
