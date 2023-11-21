using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float movementSpeed = 10f;

    Rigidbody rigidbody;

    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    public Text deathText;
    float nextFire;


    public AudioClip playerShootingAudio;

    public GameObject bulletFiringEffect;

    [HideInInspector]
    public int health = 100;

    public Slider healthBar;

   public delegate void PlayerKilled();
   public static event PlayerKilled OnPlayerKilled;

    public int respawnCount = 0;
    private Vector3 initialPosition;


    // Start is called before the first frame update
    void Start()
    {
          rigidbody = GetComponent<Rigidbody>();
        initialPosition = transform.position; // Saves initial position for respawning
        ResetHealth(); // Set initial health
     
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        if (Input.GetKey(KeyCode.Space))
            Fire();
    }

    void Move()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return;

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var rotation = Quaternion.LookRotation(new Vector3(horizontalInput,0,verticalInput));
        transform.rotation = rotation;

        Vector3 movementDir = transform.forward * Time.deltaTime * movementSpeed;
        rigidbody.MovePosition(rigidbody.position + movementDir);
    }


    void Fire() 
    {
        if (Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);

            bullet.GetComponent<BulletController>()?.InitializeBullet(transform.rotation * Vector3.forward);

            AudioManager.Instance.Play3D(playerShootingAudio, transform.position);

            VFXManager.instance.PlayVFX(bulletFiringEffect, bulletPosition.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = collision.gameObject.GetComponent<BulletController>();
            TakeDamage(bullet.damage);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health / 100f;

        if (health <= 0)
        {
            Respawn();
        }
    
    }
    private void Respawn()
    {
        respawnCount++;
      
        deathText.text = respawnCount.ToString();
       
        Debug.Log("Respawn count: " + respawnCount);
        transform.position = initialPosition;
        ResetHealth();
        //gameObject.SetActive(true);

       
    }
    private void ResetHealth()
    {
        health = 100; // Reset health to full
        healthBar.value = 1; // Reset health bar UI
    }

  

}
