
using UnityEngine;
using UnityEngine.Experimental.U2D;

//
// Space War 1962 v2020.11.03
//
// created 2020.10.16
//


public class PlayerController : MonoBehaviour
{
    public static PlayerController playerController;

    // reference to audio controller script
    private AudioController audioController;

    public Transform playerShip;

    private Rigidbody2D playerShipRigidbody;

    public Transform weaponLauncher;
    public GameObject playerBullet;

    private float fireCounter;
    private float fireRate;

    private Animator thrusterAnimation;

    private float engineThrust;
    private float thrusterInput;

    // player start position
    private float randomPlayerPositionX;
    private float randomPlayerPositionY;

    private Vector2 playerSpawnPosition;

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
    private const float HYPER_DRIVE_EXIT_VELOCITY = 2f;
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
        playerController = this;

        audioController = AudioController.instance;
    }


    private void Start()
    {
        playerShipRigidbody = GetComponent<Rigidbody2D>();

        thrusterAnimation = GetComponentInChildren<Animator>();

        Initialise();
    }


    private void Update()
    {
        MissileController();

        HyperDriveController();
    }


    private void FixedUpdate()
    {
        ShipHandler();

        UpdateShipPosition(thrusterInput * (spaceshipAcceleration / engineThrust));
    }


    private void Initialise()
    {
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


    public void SpawnPlayer()
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

        playerSpawnPosition = new Vector2(randomPlayerPositionX, randomPlayerPositionY);

        // set parent transform to new position
        transform.position = playerSpawnPosition;

        if (!inHyperSpace)
        {
            // set parent transform rotation to child transform rotation
            transform.localEulerAngles = playerShip.localEulerAngles;

            // reset child transform rotation
            playerShip.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        GameController.gameController.playerDestroyed = false;

        gameObject.SetActive(true);
    }


    private void SetStartPosition()
    {
        if (GameController.gameController.playerDestroyed)
        {
            randomPlayerPositionX = -GameController.gameController.COORS_MAX / 108.21f;
            randomPlayerPositionY = -GameController.gameController.COORS_MAX / 108.21f;
        }

        else
        {
            randomPlayerPositionX = -GameController.gameController.QUADRANT / 100;
            randomPlayerPositionY = -GameController.gameController.QUADRANT / 100;
        }
    }


    private void SetRandomPosition()
    {
        randomPlayerPositionX = Random.Range(
            PlayerSpawnBoundaryController.playerSpawnBoundaries.leftSpawnBoundary.position.x,
            PlayerSpawnBoundaryController.playerSpawnBoundaries.rightSpawnBoundary.position.x);

        randomPlayerPositionY = Random.Range(
            PlayerSpawnBoundaryController.playerSpawnBoundaries.topSpawnBoundary.position.y,
            PlayerSpawnBoundaryController.playerSpawnBoundaries.bottomSpawnBoundary.position.y);
    }


    public void ShipControl()
    {
        if (GameController.gameController.playerDestroyed)
        {
            return;
        }


        if (!inHyperSpace)
        {
            thrusterInput = Input.GetAxis("Player 1 Vertical");
            rotationInput = Input.GetAxis("Player 1 Horizontal");


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


            if (Input.GetKeyDown(KeyCode.W))
            {
                FireMissile();
            }


            if (Input.GetKeyDown(KeyCode.Q))
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
        playerShipRigidbody.AddForce(transform.up * thrust);

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
                Instantiate(playerBullet, weaponLauncher.position, transform.rotation);

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
                SpawnPlayer();

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
        playerShip.gameObject.SetActive(false);

        currentShipVelocity = playerShipRigidbody.velocity;
        
        playerShipRigidbody.velocity = Vector2.zero;

        gameObject.GetComponent<GravityBody>().enabled = false;

        // engage hyperdrive
        GameController.gameController.hyperDrive.gameObject.SetActive(true);
    }


    // this routine handles a ship breaking out of hyperspace
    private void ExitHyperSpace()
    {
        GameController.gameController.hyperDrive.gameObject.SetActive(false);

        playerShip.gameObject.SetActive(true);

        playerShipRigidbody.velocity = currentShipVelocity * HYPER_DRIVE_EXIT_VELOCITY;
        
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
            DestroyPlayer();
        }

        inHyperSpace = false;
    }


    public void DestroyPlayer()
    {
        playerShipRigidbody.velocity = Vector2.zero;

        thrusterAnimation.SetBool("playerDestroyed", true);

        GameController.gameController.playerDestroyed = true;

        //gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collidingObject)
    {
        if (collidingObject.CompareTag("Gravity Well"))
        {
            if (!GameController.gameController.SUN_OFF)
            {
                if (GameController.gameController.SUN_KILLS)
                {
                    DestroyPlayer();
                }

                // otherwise . . .
                else
                {
                    playerShipRigidbody.velocity = Vector2.zero;

                    GameController.gameController.playerDestroyed = true;

                    SpawnPlayer();
                }

                return;
            }
        }
    }


} // end of class
