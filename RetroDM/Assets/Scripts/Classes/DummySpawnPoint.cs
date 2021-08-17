using System;
using UnityEngine;

public class DummySpawnPoint : MonoBehaviour {
    GameObject dummy;
    [SerializeField] private GameObject graphics;
    [SerializeField] private GameObject Enemy;


    void Start() {
        graphics.SetActive(false);
        //SpawnDummy(this, EventArgs.Empty);
        //Enemy.Instance.OnDeath += SpawnDummy;
    }


    private void SpawnDummy(object sender, EventArgs e) {
        Timer.SetTimer(3f, () => dummy = Instantiate(Enemy, transform.position, transform.rotation));
    }
}
