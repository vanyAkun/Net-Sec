using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviour, IPunObservable //net sync
{
    PhotonView photonView;//net operations of the object
    public float movementSpeed = 10f;
    Rigidbody rigidbody;
    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    float nextFire;
    public AudioClip playerShootingAudio;
    public GameObject bulletFiringEffect;
    [HideInInspector]
    public int health = 100;
    public Slider healthBar;
    [SerializeField]
    public GameObject mainCamera; // Reference to the main camera

    void Start()
    {
        {
            mainCamera = GameObject.Find("Main Camera"); 
        }
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)//sync health
    {
        if (stream.IsWriting)//send player health
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();//receives and updates player health
            healthBar.value = health;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)//this checks if the photonview is owned by the local player so it controls only the local player
            return;

        Move();
        if (Input.GetKey(KeyCode.Space))
            photonView.RPC("Fire", RpcTarget.AllViaServer);//remote procedural call//processed by the server first helps consistency and sync.basically whatever is done by the player to be sync  around

        UpdateCameraPosition(); 
    }

    void Move()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return;

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var rotation = Quaternion.LookRotation(new Vector3(horizontalInput, 0, verticalInput));
        transform.rotation = rotation;

        Vector3 movementDir = transform.forward * Time.deltaTime * movementSpeed;
        rigidbody.MovePosition(rigidbody.position + movementDir);
    }

    [PunRPC]//remote call via photon. difference with rpc?? Movement occurs on very frame and using [PunRPC] for such frequent updates could lead to lag
    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);

            bullet.GetComponent<MultiplayerBulletController>()?.InitializeBullet(transform.rotation * Vector3.forward, photonView.Owner);

            AudioManager.Instance.Play3D(playerShootingAudio, transform.position);

            VFXManager.instance.PlayVFX(bulletFiringEffect, bulletPosition.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Multiplayer Bullet"))
        {
            MultiplayerBulletController bullet = collision.gameObject.GetComponent<MultiplayerBulletController>();
            TakeDamage(bullet);
        }
    }

    void TakeDamage(MultiplayerBulletController bullet)
    {
        health -= bullet.damage;
        Debug.Log("Player taking damage: " + bullet.damage);
        healthBar.value = health;

        if (health <= 0)
        {
            bullet.owner.AddScore(1);
            PlayerDied();
        }
    }


    void PlayerDied()
    {
        Debug.Log("Player has died!");
        health = 100;
        healthBar.value = health;
    }

    void UpdateCameraPosition()
    {
      
        Vector3 offset = new Vector3(0f, 13f, -9.87f); 
        mainCamera.transform.position = transform.position + offset;
        mainCamera.transform.LookAt(transform.position);
    }
}