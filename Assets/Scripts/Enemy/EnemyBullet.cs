
using UnityEngine;

//
// Space War 1962 v2020.11.03
//
// created 2020.10.16
//


public class EnemyBullet : MonoBehaviour
{
    private AudioController audioController;

    private Rigidbody2D bulletRigidbody;

    public GameObject bulletEcho;

    private float bulletSpeed;
    private float timeBetweenSpawns;
    private float startTimeBetweenSpawns;

    private float bulletLifeSpan;
    private float soundLifeSpan;

    public bool playingFireBulletSound;

    //private Vector3 bulletDirection;



    private void Awake()
    {
        audioController = AudioController.instance;
    }


    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody2D>();

        Initialise();
    }


    void FixedUpdate()
    {
        MoveBullet();
    }


    private void Initialise()
    {
        bulletSpeed = 2.5f;

        bulletLifeSpan = 2f;
        
        soundLifeSpan = bulletLifeSpan;
        
        playingFireBulletSound = false;

        //bulletDirection = PlayerController.playerController.transform.position - transform.position;

        //bulletDirection.Normalize();

        timeBetweenSpawns = 0f;

        startTimeBetweenSpawns = 0.001f;
    }


    private void MoveBullet()
    {
        if (!playingFireBulletSound)
        {
            //audioController.PlayAudioClip("Fire Player Bullet");

            playingFireBulletSound = true;
        }

        soundLifeSpan -= Time.deltaTime;

        bulletRigidbody.velocity = transform.up * bulletSpeed;

        //transform.position += bulletDirection * bulletSpeed * Time.deltaTime;

        GameController.gameController.ScreenWrap(transform);

        SpawnBulletEcho();

        Destroy(gameObject, bulletLifeSpan);

        if (soundLifeSpan <= 0f)
        {
            //audioController.StopAudioClip("Fire Player Bullet");

            playingFireBulletSound = false;
        }
    }


    private void SpawnBulletEcho()
    {
        if (timeBetweenSpawns <= 0f)
        {
            GameObject echoInstance = Instantiate(bulletEcho, transform.position, Quaternion.identity);

            Destroy(echoInstance, 0.2f);

            timeBetweenSpawns = startTimeBetweenSpawns;
        }

        else
        {
            timeBetweenSpawns -= Time.deltaTime;
        }
    }


    /*
    private void PlayFireBulletSound()
    {
        if (soundLifeSpan <= 0f)
        {
            audioController.StopAudioClip("Fire Enemy Bullet");

            playingFireBulletSound = false;

            return;
        }

        if (!playingFireBulletSound)
        {
            audioController.PlayAudioClip("Fire Enemy Bullet");

            playingFireBulletSound = true;
        }
    }
    */


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);

        if (collision.gameObject.CompareTag("Player"))
        {
            if (!GameController.gameController.playerDestroyed)
            {
                audioController.StopAudioClip("Fire Enemy Bullet");
                audioController.StopAudioClip("Rotate Ship");
                audioController.StopAudioClip("Thrusters Engaged");

                audioController.PlayAudioClip("Explosion");

                GameController.gameController.playerDestroyed = true;

                if (!GameController.gameController.gameOver)
                {
                    GameController.gameController.UpdateEnemyScore();
                }
            }
        }
    }


} // end of class
