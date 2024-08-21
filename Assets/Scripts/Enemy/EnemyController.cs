
using UnityEngine;

//
// Space War 1962 v2020.11.03
//
// created 2020.10.16
//


public class EnemyController : MonoBehaviour
{
    public static EnemyController enemyController;

    private AudioController audioController;

    public Transform enemyShip;

    private Rigidbody2D enemyShipRigidbody;

    public Transform weaponLauncher;
    public GameObject enemyBullet;

    private float fireCounter;
    private float fireRate;

    private Animator thrusterAnimation;

    private float engineThrust;
    private float thrusterInput;

    private float movementSpeed;
    private int movementDirection;

    private float latestDirectionChangeTime;
    private float directionChangeTime;

    // enemy start position
    private float randomEnemyPositionX;
    private float randomEnemyPositionY;

    private Vector2 enemySpawnPosition;

    [HideInInspector] public float rotationInput;
    [HideInInspector] public float rotationSpeed;

    private bool playingThrusterSound;
    private bool playingRotateShipSound;


    // js code
    private const int TORPEDO_SUPPLY = 32;
    private float torpedoVelocity = 4f;
    private float torpedoReloadTime = 16f;
    private float torpedoLife = 96f;
    private const int FUEL_SUPPLY = 8192;
    ////private float angularAcceleration = 8 * GameController.BIN_RAD_COEF;
    private float spaceshipAcceleration = 1f;

    private float collisionRadius = 48f;
    private float collisionRadius2 = 24f;
    private float torpedoSpaceWarpage = 9f;

    private bool inHyperSpace;
    private float inHyperSpaceCounter;
    private const int HYPER_DRIVE_ENERGY = 8;
    private float hyperSpaceTimeBeforeBreakout = 3.2f;
    private float hyperspaceTimeInBreakout = 64f;
    private const float HYPER_DRIVE_RECHARGE_TIME = 128f;
    private int hyperspaceDisplacement = 9;
    private const float HYPER_DRIVE_EXIT_VELOCITY = 4f;
    private float hyperSpaceUncertainty = 16384f;
    private Vector2 currentShipVelocity;             // hyperspace: handler backup    (nh1, mh1)
    private int hyperSpaceJumps;             // hyperspace jumps remaining    (nh2, mh2)
    private float hyperDriveActiveCounter;             // hyperspace cooling            (nh3, mh3)
    private float hyperDriveOverload;             // hyperspace uncertainty        (nh4, mh4)

    private float angularMomentum;  //              (symbol: nom, pointer: mom)
    private float theta;            // rotation                      (nth, mth)
    private int fuel;             // amount of fuel                (nfu, mfu)
    private int torpedoes;        // torpedoes left                (ntr, mtr)



    private void Awake()
    {
        enemyController = this;

        audioController = AudioController.instance;
    }


    private void Start()
    {
        //Initialise();

        //movementDirection = GetRandomDirection();

        enemyShipRigidbody = GetComponent<Rigidbody2D>();

        thrusterAnimation = GetComponentInChildren<Animator>();

        Initialise();
    }


    private void Update()
    {
        //FireMissile();  
        
        MissileController();

        HyperDriveController();
    }


    private void FixedUpdate()
    {
        //MoveEnemyShips();

        ShipHandler();

        UpdateShipPosition(thrusterInput * (spaceshipAcceleration / engineThrust));
    }


    private void Initialise()
    {
        //movementSpeed = 0.8f;

        //directionChangeTime = 5f;
        //latestDirectionChangeTime = 0f;

        //fireCounter = 0f;

        engineThrust = 10f; // 0.5f;

        rotationSpeed = 2f;

        fireRate = 2f;
        fireCounter = 0f;

        playingThrusterSound = false;
        playingRotateShipSound = false;

        // js code
        //ss2.theta = 0;
        theta = 0f;

        //ss1.torpedoes = ss2.torpedoes = torpedoSupply;
        torpedoes = TORPEDO_SUPPLY;

        //ss1.fuel = ss2.fuel = fuelSupply;
        fuel = FUEL_SUPPLY;

        //ss1.hyp2 = ss2.hyp2 = hyperspaceShots;
        hyperSpaceJumps = HYPER_DRIVE_ENERGY;
    }


    public void SpawnEnemy()
    {
        // set random position
        if (inHyperSpace)
        {
            SetRandomPosition();
        }

        else
        {
            // set start position
            SetStartPosition();
        }

        enemySpawnPosition = new Vector2(randomEnemyPositionX, randomEnemyPositionY);

        // set parent transform to new position
        transform.position = enemySpawnPosition;

        if (!inHyperSpace)
        {
            // set parent transform rotation to child transform rotation
            transform.localEulerAngles = enemyShip.localEulerAngles;

            // reset child transform rotation
            enemyShip.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        GameController.gameController.enemyDestroyed = false;

        gameObject.SetActive(true);
    }


    private void SetStartPosition()
    {
        if (GameController.gameController.enemyDestroyed)
        {
            randomEnemyPositionX = GameController.gameController.COORS_MAX / 108.21f;
            randomEnemyPositionY = GameController.gameController.COORS_MAX / 108.21f;
        }

        else
        {
            randomEnemyPositionX = GameController.gameController.QUADRANT / 100;
            randomEnemyPositionY = GameController.gameController.QUADRANT / 100;
        }
    }


    private void SetRandomPosition()
    {
        randomEnemyPositionX = Random.Range(
            EnemySpawnBoundaryController.enemySpawnBoundaries.leftSpawnBoundary.position.x,
            EnemySpawnBoundaryController.enemySpawnBoundaries.rightSpawnBoundary.position.x);

        randomEnemyPositionY = Random.Range(
            EnemySpawnBoundaryController.enemySpawnBoundaries.topSpawnBoundary.position.y,
            EnemySpawnBoundaryController.enemySpawnBoundaries.bottomSpawnBoundary.position.y);
    }


    private int GetRandomDirection()
    {
        return Random.Range(0, 9);
    }


    private float GetRandomFireRate()
    {
        return Random.Range(3f, 4f);
    }


    /*
    private void MoveEnemyShips()
    {
        MoveEnemy();
    }


    private void MoveEnemy()
    {
        if (Time.time - latestDirectionChangeTime > directionChangeTime)
        {
            latestDirectionChangeTime = Time.time;

            movementDirection = GetRandomDirection();
        }

        switch (movementDirection)
        {
            case 0: enemyRigidbody.velocity = new Vector2(0f, 0f); break;

            case 1: enemyRigidbody.velocity = new Vector2(-movementSpeed, 0f); break;

            case 2: enemyRigidbody.velocity = new Vector2(movementSpeed, 0f); break;

            case 3: enemyRigidbody.velocity = new Vector2(0f, movementSpeed); break;

            case 4: enemyRigidbody.velocity = new Vector2(0f, -movementSpeed); break;

            case 5: enemyRigidbody.velocity = new Vector2(-movementSpeed, movementSpeed); break;

            case 6: enemyRigidbody.velocity = new Vector2(movementSpeed, movementSpeed); break;

            case 7: enemyRigidbody.velocity = new Vector2(-movementSpeed, -movementSpeed); break;

            case 8: enemyRigidbody.velocity = new Vector2(movementSpeed, -movementSpeed); break;
        }

        GameController.gameController.ScreenWrap(enemyShip);
    }


    private void FireMissile()
    {
        if (!GameController.gameController.inDemoMode)
        {
            if (fireCounter > 0f)
            {
                fireCounter -= Time.deltaTime;

                if (fireCounter <= 0f)
                {
                    fireCounter = GetRandomFireRate();

                    Instantiate(enemyBullet, weaponLauncher.position, transform.rotation);
                }
            }

            else
            {
                fireCounter = GetRandomFireRate();
            }
        }
    }
    */


    public void ShipControl()
    {
        if (GameController.gameController.enemyDestroyed)
        {
            return;
        }


        if (!inHyperSpace)
        {
            thrusterInput = Input.GetAxis("Player 2 Vertical");
            rotationInput = Input.GetAxis("Player 2 Horizontal");


            if (thrusterInput == 0f)
            {
                audioController.StopAudioClip("Thrusters Engaged");

                playingThrusterSound = false;
            }

            if (thrusterInput > 0f)
            {
                if (!playingThrusterSound)
                {
                    //thrusterAnimation.Play("Thrusters Engaged");

                    audioController.PlayAudioClip("Thrusters Engaged");

                    playingThrusterSound = true;

                }
            }


            if (rotationInput == 0f)
            {
                //audioController.StopAudioClip("Rotate Ship");

                playingRotateShipSound = false;
            }

            if (rotationInput > 0f)
            {
                if (!playingRotateShipSound)
                {
                    //audioController.PlayAudioClip("Rotate Ship");

                    playingRotateShipSound = true;
                }
            }

            if (rotationInput < 0f)
            {
                if (!playingRotateShipSound)
                {
                    //audioController.PlayAudioClip("Rotate Ship");

                    playingRotateShipSound = true;
                }
            }


            if (Input.GetKeyDown(KeyCode.I))
            {
                FireMissile();
            }


            if (Input.GetKeyDown(KeyCode.O))
            {
                ActivateHyperDrive();
            }
        }
    }


    private void ShipHandler()
    {
        if (!inHyperSpace)
        {
            if (thrusterInput > 0f)
            {
                // js code
                if (fuel > 0)
                {
                    fuel -= 1;

                    if (fuel <= 0)
                    {
                        fuel = 0;
                    }
                }
            }


            if (rotationInput > 0f)
            {
                RotateShip(GameController.CLOCKWISE);
            }


            if (rotationInput < 0f)
            {
                RotateShip(GameController.COUNTER_CLOCKWISE);
            }
        }
    }


    private void UpdateShipPosition(float thrust)
    {
        enemyShipRigidbody.AddForce(transform.up * thrust);

        GameController.gameController.ScreenWrap(transform);
    }


    private void RotateShip(float rotationDirection)
    {
        transform.Rotate(0f, 0f, rotationDirection * rotationSpeed);
    }


    private void MissileController()
    {
        fireCounter -= Time.deltaTime;
    }


    private void FireMissile()
    {
        if (torpedoes > 0)
        {
            if (fireCounter <= 0f)
            {
                Instantiate(enemyBullet, weaponLauncher.position, transform.rotation);

                torpedoes -= 1;

                if (torpedoes <= 0)
                {
                    torpedoes = 0;
                }

                fireCounter = fireRate;
            }
        }
    }


    private void HyperDriveController()
    {
        hyperDriveActiveCounter -= Time.deltaTime;


        if (inHyperSpace)
        {
            inHyperSpaceCounter -= Time.deltaTime;


            if (inHyperSpaceCounter <= 0)
            {
                SpawnEnemy();

                ExitHyperSpace();
            }
        }
    }


    // hyperspace
    private void ActivateHyperDrive()
    {
        if (hyperDriveActiveCounter <= 0)
        {
            if (hyperSpaceJumps > 0)
            {
                inHyperSpace = true;

                inHyperSpaceCounter = hyperSpaceTimeBeforeBreakout;

                EnterHyperSpace();
            }
        }
    }


    // this routine handles a non-colliding ship invisibly in hyperspace
    private void EnterHyperSpace()
    {
        enemyShip.gameObject.SetActive(false);

        currentShipVelocity = enemyShipRigidbody.velocity;

        enemyShipRigidbody.velocity = Vector2.zero;

        gameObject.GetComponent<GravityBody>().enabled = false;

        // engage hyperdrive
        GameController.gameController.hyperDrive.gameObject.SetActive(true);
    }


    // this routine handles a ship breaking out of hyperspace
    private void ExitHyperSpace()
    {
        GameController.gameController.hyperDrive.gameObject.SetActive(false);

        enemyShip.gameObject.SetActive(true);

        enemyShipRigidbody.velocity = currentShipVelocity * HYPER_DRIVE_EXIT_VELOCITY;

        gameObject.GetComponent<GravityBody>().enabled = true;

        if (hyperSpaceJumps > 0)
        {
            // decrement remaining jumps
            hyperSpaceJumps -= 1;
        }

        hyperDriveActiveCounter = HYPER_DRIVE_RECHARGE_TIME;

        // now check, if we break on re-entry (Mark One Hyperfield Generators ...)
        hyperDriveOverload += hyperSpaceUncertainty;

        int r = ((1 << 20) | 0) & 0x1FFFF; // (Random.Range(0, 2) * (1 << 20)) | 0; // & 0x1FFFF; // 17-bits random
        Debug.Log("r = " + r);
        if (hyperDriveOverload >= r)
        {
            DestroyEnemy();
        }

        inHyperSpace = false;
    }


    public void CollisionDetected(EnemyShipController enemy)
    {
        if (!GameController.gameController.playerDestroyed)
        {
            if (enemy.gameObject.CompareTag("Enemy0") || enemy.gameObject.CompareTag("Enemy1"))
            {
                string collidingObject = enemy.gameObject.tag;

                GameController.gameController.playerDestroyed = true;

                DestroyEnemy(); // (collidingObject);

                /*
                if (!GameController.gameController.gameOver)
                {
                    GameController.gameController.UpdatePlayerScore();
                    GameController.gameController.UpdateEnemyScore();
                }
                */
            }
        }
    }


    /*
    public void DestroyEnemy(string collidingObject)
    {
        if (collidingObject == "Enemy")
        {
            enemyShip.gameObject.SetActive(false);
        }

        audioController.StopAudioClip("Thrusters Engaged");
        audioController.StopAudioClip("Rotate Ship");
        audioController.StopAudioClip("Fire Player Bullet");
        audioController.StopAudioClip("Fire Enemy Bullet");

        audioController.PlayAudioClip("Explosion");

        if (collidingObject == "Enemy")
        {
            GameController.gameController.enemyDestroyed = true;
        }
    }
    */


    public void DestroyEnemy()
    {
        enemyShipRigidbody.velocity = Vector2.zero;

        thrusterAnimation.SetBool("enemyDestroyed", true);

        GameController.gameController.enemyDestroyed = true;

        //gameObject.SetActive(false);
    }


} // end of class
