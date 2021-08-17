using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : Gun {

    private bool isFiring;

    protected override void Start() {
        controls.Controls.Shoot.performed += _ => isFiring = true;
        base.Start();
    }

    protected override void Update() {
        if(isFiring && !isReloading) {
            base.Shoot();
            Debug.Log("Fire!");
            isFiring = false;
        }
    }
}
