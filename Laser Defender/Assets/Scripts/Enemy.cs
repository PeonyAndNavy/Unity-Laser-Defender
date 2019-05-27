using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip shootingSound;
    [SerializeField] [Range(0, 1)] float shootingSoundVolume = 0.75f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = UnityEngine.Random.Range(
            minTimeBetweenShots, 
            maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            AudioSource.PlayClipAtPoint(
                shootingSound, 
                Camera.main.transform.position,
                shootingSoundVolume);
            shotCounter = UnityEngine.Random.Range(
                minTimeBetweenShots, 
                maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
                projectile,
                transform.position,
                Quaternion.identity
                ) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity
            = new Vector2(0, -projectileSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer
            = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; } //protecting against null
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject); 
        AudioSource.PlayClipAtPoint(
            deathSound, 
            Camera.main.transform.position,
            deathSoundVolume);
        GameObject explosion = Instantiate(deathVFX,
            transform.position,
            Quaternion.identity) as GameObject;
        Destroy(explosion, durationOfExplosion);
    }
}
