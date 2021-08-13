using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPunCallbacks, IDamageable {
    public static PlayerController Instance;
    public event Action<int> OnHealthChange;
    public static float sensitivity;
    private Animator animator;
    private bool isGrounded;
    private float gravity = -9.81f;
    private float groundDistance = 0.4f;
    [SerializeField] private float jumpHeight = 3f;
    private float speed = 6;
    private float xRotate = 0;
    private PlayerControls controls;
    private int health = 100;
    private PhotonView PV;
    private PlayerManager playerManager;
    private Vector3 velocity;
    [Header("Components")]
    [SerializeField] private List<Gun> guns = new List<Gun>();
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject cameraHolder, hand;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;

    private void Awake() {
        Instance = this;
        controls = new PlayerControls();
        PV = GetComponent<PhotonView>();
        Settings.Instance.OnMouseSensitivityChanged += UpdateMouseSensitivity;
    }

    private void Start() {
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        OnHealthChange?.Invoke(health);
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        controls.Controls.Jump.performed += _ => Jump();

        if(PV.IsMine) {
            for(int i = 0; i < hand.transform.childCount; i++) {
                guns.Add(hand.transform.GetChild(i).GetComponent<Gun>());
            }
        } else {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<Canvas>().gameObject);
            animator.enabled = false;
        }
    }

    public override void OnEnable() => controls.Enable();

    public override void OnDisable() => controls.Disable();

    private void Update() {
        if(!PV.IsMine || Cursor.lockState == CursorLockMode.None) { return; }
        if(transform.position.y <= -50) { Die(); }
        MovePlayer();
        RotatePlayer();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void UpdateMouseSensitivity(object sender, EventArgs e) {
        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
    }

    private void MovePlayer() {
        //Player movement
        float x = controls.Controls.WASD.ReadValue<Vector2>().x;
        float z = controls.Controls.WASD.ReadValue<Vector2>().y;
        Vector3 finalVector = transform.right * x + transform.forward * z;
        controller.Move(finalVector.normalized * Time.deltaTime * speed);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if(isGrounded && velocity.y < 0) {
            velocity.y = -2;
        }
    }

    private void Jump() {
        if(isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

    private void RotatePlayer() {
        //Camera and player rotation
        float mouseX = controls.Controls.MouseLook.ReadValue<Vector2>().x * sensitivity * Time.deltaTime;
        float mouseY = controls.Controls.MouseLook.ReadValue<Vector2>().y * sensitivity * Time.deltaTime;

        xRotate -= mouseY;
        xRotate = Mathf.Clamp(xRotate, -90, 90);
        cameraHolder.transform.localRotation = Quaternion.Euler(xRotate, 0, 0);
        controller.transform.Rotate(Vector3.up * mouseX);
    }

    public void AddHealth(object sender, EventArgs e) {
        if(health < 100) {
            health += 25;
            health = Mathf.Clamp(health, 0, 100);
            PlayerGUI.Instance.UpdateHealth(health);
        }
    }

    public void TakeDamage(float damage) {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage) {
        if(!PV.IsMine) { return; }
        health -= Convert.ToInt32(damage);
        health = Mathf.Clamp(health, 0, 100);
        PlayerGUI.Instance.UpdateHealth(health);
        if(health == 0) { Die(); }
    }

    private void Die() {
        playerManager.Die();
    }
}
