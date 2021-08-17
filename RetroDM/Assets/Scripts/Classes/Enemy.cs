using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable{
    private float health = 100;
    public static Enemy Instance;
    public event EventHandler OnDeath;
    [SerializeField] GameObject character;
    [SerializeField] GameObject characterPieces;

    private void Awake() {
        Instance = this;
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if(health <= 0) {
            Destroy(gameObject);
            characterPieces = Instantiate(characterPieces);
            Timer.SetTimer(3, () => {
                Destroy(characterPieces);
                OnDeath?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
