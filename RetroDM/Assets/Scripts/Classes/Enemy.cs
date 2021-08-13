using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable{
    private float health = 100;
    [SerializeField] GameObject character;
    [SerializeField] GameObject characterPieces;

    public void TakeDamage(float damage) {
        health -= damage;

        if(health <= 0) {
            Destroy(character);
            Instantiate(characterPieces, transform.position, transform.rotation);
        }
    }
}
