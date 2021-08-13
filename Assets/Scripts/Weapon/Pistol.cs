public class Pistol : Gun {

    protected override void Start() {
        //Check if player pressed fire button
        controls.Controls.Shoot.performed += _ => GetInput();
    }

    protected override void GetInput() {
        if (!isReloading) {
            PV.RPC("Shoot", Photon.Pun.RpcTarget.All);
        }
    }
}
