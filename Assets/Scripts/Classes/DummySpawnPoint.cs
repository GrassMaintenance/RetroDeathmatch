using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawnPoint : MonoBehaviour {
    [SerializeField] private GameObject graphics;
    
    void Start() {
        graphics.SetActive(false);
    }
}
