
using UnityEngine;
using UnityEngine.UI;

//
// Computer Space 1971 v2020.11.03
// 
// Port of Atari's 1971 video game
// by Atari
// 
// created 2020.10.16
//


public class GameController : MonoBehaviour
{
    public static GameController gameController;
    
    // reference to audio controller script
    private AudioController audioController;

    // reference to player and enemy controller scripts
    public PlayerController playerController;
    public EnemyController enemyController;

    // reference to player and enemy scores
    public PlayerScoreController playerScoreController;
    public EnemyScoreController enemyScoreController;

    // reference to timer controller script
    public TimerController timerController;

    public Transform hyperDrive;

    public StarfieldController starFieldController;

    public Sprite[] number;

    private Camera mainCamera;

    public const int DIGITS = 2;

    public const int NUMBER_OF_ENEMIES = 2;

    public Text insertCoinsText;
    public Text coinsInsertedText;

    public Text gameOverText;
    public Text pressStartText;

    // player scores
    [HideInInspector] public int playerScore;
    [HideInInspector] public int enemyScore;

    // game credits
    [HideInInspector] public int gameCredits;


    // game mode
    [HideInInspector] public bool canPlay;
    [HideInInspector] public bool inPlayMode;
    [HideInInspector] public bool inDemoMode;
    [HideInInspector] public bool inPawzMode;

    [HideInInspector] public bool gameOver;

    [HideInInspector] public bool playerDestroyed;
    [HideInInspector] public bool enemyDestroyed;

    [HideInInspector] public bool playingSaucerHumSound;

    private bool screenWrappingX;
    private bool screenWrappingY;


    // respawn timers
    private float playerRespawnTimer;
    private bool playerRespawnTimerCanCount;
    private bool playerRespawnTimerDoOnce;

    private float enemyRespawnTimer;
    private bool enemyRespawnTimerCanCount;
    private bool enemyRespawnTimerDoOnce;

    public const float RESPAWN_DELAY = 3f;


    // js code
    public float SCREEN_WIDTH; // = 1980;
    public float COORS_MAX; // = SCREEN_WIDTH / 2;
    public float QUADRANT; // = COORS_MAX / 2;

    public bool ANGULAR_MOMENTUM = false;  // sense switch 1
    public bool LOW_GRAVITY = false;  // sense switch 2
    public bool SINGLE_SHOTS = false;  // sense switch 3
    public bool NO_BACKGROUND = false;  // sense switch 4
    public bool SUN_KILLS = true;  // sense switch 5
    public bool SUN_OFF = false;  // sense switch 6

    ////public bool TEST_WORD_CONTROLS = false;  // use testword controls
    ////public bool HALT_ON_SCORES = false;  // halt on scores/matches (call resume() to continue)
    ////public int FPS = 22;   // fps (original alternates between 19 and 25)
 
    ////public const float BIN_RAD_COEF = Mathf.PI / 51472;
    ////public const float TAU = Mathf.PI * 2;

    public const float CLOCKWISE = -1f;
    public const float COUNTER_CLOCKWISE = 1f;

    ////public float starCaptureRadius = 0.05f; // 1f;


    public const int PLAYER_ONE = 1;
    public const int PLAYER_TWO = 2;
    public const int PLAYER_THREE = 3;
    public const int PLAYER_FOUR = 4;

    // colours
    public const int WHITE = 255;
    public const int RED = 255;
    public const int GREEN = 255;
    public const int BLUE = 255;

    public const int START_SCORE = 0;
    public const int PLAYER_WINNING_SCORE = 15;
    public const int ENEMY_WINNING_SCORE = 7;
    public const int GAMEOVER_SCORE = 99;

    public const int INSERT_COINS = 0;
    public const int ONE_PLAYER_COINS = 1;
    public const int MAXIMUM_COINS = 99;

    // console initialisation
    private const string GAME_TITLE = "SPACE WAR";
    private const int TV_MODE = AtariConsoleController.BW_TV;



    private void Awake()
    {
        gameController = this;

        // set reference to audio source component
        audioController = AudioController.instance;
    }


    void Start()
    {
        GetCameraSize();

        CreateStarfield();

        Initialise();
    }


    // =============================================================================
    // check for player input
    // =============================================================================
    void Update()
    {
        KeyboardInput();

        ControllerInput();

        RunPlayerRespawnTimer();

        RunEnemyRespawnTimer();

        PlaySaucerHumSound();

        //timerController.UpdateTimerDisplay();
    }


    private void GetCameraSize()
    {
        mainCamera = Camera.main;

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        SCREEN_WIDTH = 1024; // cameraWidth;
        COORS_MAX = SCREEN_WIDTH / 2;
        QUADRANT = COORS_MAX / 2;
    }


    private void CreateStarfield()
    {
        starFieldController.CreateStarfield();
    }


    private void KeyboardInput()
    {
        /*
        if (!inPawzMode)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                AtariConsoleController.atariConsoleController.GamePawzSwitch();
            }
        }
        */
    }


    private void ControllerInput()
    {
        if (!inPawzMode && !inDemoMode)
        {
            PlayerController.playerController.ShipControl();

            EnemyController.enemyController.ShipControl();
        }
    }


    private void PlaySaucerHumSound()
    {
        if (!inDemoMode)
        {
            if (!playingSaucerHumSound)
            {
                //audioController.PlayAudioClip("Saucer Hum");

                playingSaucerHumSound = true;
            }
        }

        else
        {
            //audioController.StopAudioClip("Saucer Hum");
            //audioController.StopAudioClip("Fire Enemy Bullet");
            //audioController.StopAudioClip("Fire Player Bullet");
            //audioController.StopAudioClip("Rotate Ship");
            //audioController.StopAudioClip("Thrusters Engaged");
        }
    }


    private void Initialise()
    {
        InitialiseCabinet();

        InitialiseConsoleSystem();

        //audioController.PlayAudioClip("Music");

        StartDemoMode();
    }


    private void InitialiseCabinet()
    {
        gameCredits = INSERT_COINS;

        UpdateGameCreditsText();

        canPlay = false;

        gameOver = true;
    }


    public void InitialiseDifficultySwitchSettings()
    {
        //leftDifficultyASpriteWidth = DIFFICULTY_A_WIDTH;

        //leftDifficultyASpriteHeight = DIFFICULTY_A_HEIGHT;

        //leftDifficultyBSpriteWidth = DIFFICULTY_B_WIDTH;

        //leftDifficultyBSpriteHeight = DIFFICULTY_B_HEIGHT;

        //rightDifficultyASpriteWidth = DIFFICULTY_A_WIDTH;

        //rightDifficultyASpriteHeight = DIFFICULTY_A_HEIGHT;

        //rightDifficultyBSpriteWidth = DIFFICULTY_B_WIDTH;

        //rightDifficultyBSpriteHeight = DIFFICULTY_B_HEIGHT;
    }


    private void InitialiseConsoleSystem()
    {
        AtariConsoleController.atariConsoleController.initialisingConsoleSystem = true;

        AtariConsoleController.atariConsoleController.InitialiseConsole(GAME_TITLE, TV_MODE);
    }


    private void SetAtariConsoleMode(int consoleMode)
    {
        AtariConsoleController.atariConsoleController.consoleMode = consoleMode;

        AtariConsoleController.atariConsoleController.SetConsoleMode(consoleMode);
    }


    public void SetTvMode(int tvMode)
    {
        switch (tvMode)
        {
            case AtariConsoleController.BW_TV:

                SetClassicMode();

                break;

            case AtariConsoleController.COLOUR_TV:

                SetColourMode();

                break;
        }
    }


    private void SetClassicMode()
    {
        SetPlayer1Colour(WHITE, WHITE, WHITE);

        SetPlayer2Colour(WHITE, WHITE, WHITE);

        SetPlayer3Colour(WHITE, WHITE, WHITE);

        //SetPlayer4Colour(WHITE, WHITE, WHITE);

        //SetBallColour(tvMode, PLAYER_ONE);
    }


    private void SetColourMode()
    {
        SetPlayer1Colour(RED, GREEN, 0);

        SetPlayer2Colour(0, GREEN, 0);

        SetPlayer3Colour(0, GREEN, BLUE);

        //SetPlayer4Colour(RED, GREEN, 0);

        //SetBallColour(tvMode, PLAYER_ONE);
    }


    private void SetPlayer1Colour(int r, int g, int b)
    {
        // yellow
        playerController.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);

        playerScoreController.playerScoreDigit[0].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);
        playerScoreController.playerScoreDigit[1].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);
    }


    private void SetPlayer2Colour(int r, int g, int b)
    {
        // green
        enemyController.enemyShip.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);

        enemyScoreController.enemyScoreDigit[0].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);
        enemyScoreController.enemyScoreDigit[1].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);
    }


    private void SetPlayer3Colour(int r, int g, int b)
    {
        // cyan
        //enemyController.enemyShip[1].gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(r, g, b);
    }


    private void SetPlayer4Colour(int r, int g, int b)
    {
        // yellow
        //player4SpriteController.gameObject.GetComponent<SpriteRenderer>().material.color = new Color(r, g, b);

        //player4Goal.gameObject.GetComponent<SpriteRenderer>().material.color = new Color(r, g, b);

        //player4ScoreCounter1.gameObject.GetComponent<SpriteRenderer>().material.color = new Color(r, g, b);
        //player4ScoreCounter2.gameObject.GetComponent<SpriteRenderer>().material.color = new Color(r, g, b);
        //player4ScoreCounter3.gameObject.GetComponent<SpriteRenderer>().material.color = new Color(r, g, b);
        //player4ScoreCounter4.gameObject.GetComponent<SpriteRenderer>().material.color = new Color(r, g, b);
    }


    #region DIFFICULTY SWITCHES
    public void SetLeftDifficultyA()
    {
        // only one enemy ship fires
    }


    public void SetLeftDifficultyB()
    {
        // both enemy ships fire
    }


    public void SetRightDifficultyA()
    {
        // enemy moves together
    }


    public void SetRightDifficultyB()
    {
        // enemy moves independently
    }
    #endregion


    public void SetPawzMode()
    {
        SetAtariConsoleMode(AtariConsoleController.CONSOLE_VISIBLE);
    }


    public void SetPlayMode()
    {
        SetAtariConsoleMode(AtariConsoleController.CONSOLE_HIDDEN);
    }


    // start demo mode
    public void StartDemoMode()
    {
        gameOverText.gameObject.SetActive(true);

        // start demo mode
        inDemoMode = true;
        inPlayMode = false;

        playingSaucerHumSound = false;

        AtariConsoleController.atariConsoleController.SetPawzModeSwitches();

        // show atari console
        SetAtariConsoleMode(AtariConsoleController.CONSOLE_VISIBLE);

        // check if there are any credits
        if (gameCredits == INSERT_COINS)
        {
            insertCoinsText.gameObject.SetActive(true);
        }

        AtariConsoleController.atariConsoleController.SetGameSelection();
    }


    // start one player game
    public void StartOnePlayerGame()
    {
        InitialiseGame();
    }

    // start two player game
    public void StartTwoPlayerGame()
    {
        //InitialiseGameMode();
    }

    // start three player game
    public void StartThreePlayerGame()
    {
        //InitialiseGameMode();
    }

    // start four player game
    public void StartFourPlayerGame()
    {
        //InitialiseGameMode();
    }


    // initialise game mode
    private void InitialiseGame()
    {
        gameCredits -= 1;

        UpdateGameCreditsText();

        if (gameCredits == INSERT_COINS)
        {
            canPlay = false;

            AtariConsoleController.atariConsoleController.gameNumberSelected = AtariConsoleController.NO_GAME_SELECTED;

            AtariConsoleController.atariConsoleController.SetGameSelection();
        }

        pressStartText.gameObject.SetActive(false);

        inPlayMode = true;
        inDemoMode = false;

        gameOver = false;

        AtariConsoleController.atariConsoleController.SetPawzModeSwitches();

        // hide atari console
        SetAtariConsoleMode(AtariConsoleController.CONSOLE_HIDDEN);

        InitialiseScore();

        // reset player and enemy
        playerDestroyed = false;
        enemyDestroyed = false;

        playerRespawnTimer = 0f;
        enemyRespawnTimer = 0f;

        playerController.SpawnPlayer();
        enemyController.SpawnEnemy();

        timerController.ResetTimer();
    }


    private void InitialiseScore()
    {
        playerScore = START_SCORE;

        enemyScore = START_SCORE;

        UpdateScoreText();
    }


    public void UpdatePlayerScore()
    {
        playerScore += 1;

        UpdateScoreText();

        StartEnemyRespawnTimer();
    }


    #region ENEMY RESPAWN
    private void StartEnemyRespawnTimer()
    {
        enemyRespawnTimer = RESPAWN_DELAY;

        enemyRespawnTimerCanCount = true;

        enemyRespawnTimerDoOnce = false;
    }


    private void RunEnemyRespawnTimer()
    {
        if (!inDemoMode)
        {
            if (enemyRespawnTimerCanCount && enemyRespawnTimer >= 0.0f)
            {
                enemyRespawnTimer -= Time.deltaTime;
            }

            else if (!enemyRespawnTimerDoOnce && enemyRespawnTimer <= 0.0f)
            {
                enemyRespawnTimerCanCount = false;

                enemyRespawnTimerDoOnce = true;

                enemyRespawnTimer = 0.0f;

                if (playerScore > 0)
                {
                    enemyController.SpawnEnemy();
                }
            }
        }
    }
    #endregion


    public void UpdateEnemyScore()
    {
        enemyScore += 1;

        UpdateScoreText();

        if (playerDestroyed)
        {
            StartPlayerRespawnTimer();
        }
    }


    #region PLAYER RESPAWN
    private void StartPlayerRespawnTimer()
    {
        playerRespawnTimer = RESPAWN_DELAY;

        playerRespawnTimerCanCount = true;

        playerRespawnTimerDoOnce = false;
    }


    private void RunPlayerRespawnTimer()
    {
        if (!inDemoMode)
        {
            if (playerRespawnTimerCanCount && playerRespawnTimer >= 0.0f)
            {
                playerRespawnTimer -= Time.deltaTime;
            }

            else if (!playerRespawnTimerDoOnce && playerRespawnTimer <= 0.0f)
            {
                playerRespawnTimerCanCount = false;

                playerRespawnTimerDoOnce = true;

                playerRespawnTimer = 0.0f;

                if (enemyScore > 0)
                {
                    playerController.SpawnPlayer();
                }
            }
        }
    }
    #endregion


    // when the game is over
    public void GameOver()
    {
        hyperDrive.gameObject.SetActive(false);

        StartDemoMode();
    }


    // update the player's scores
    private void UpdateScoreText()
    {
        PlayerScoreController.playerScoreController.UpdatePlayerScoreDisplay();

        EnemyScoreController.enemyScoreController.UpdateEnemyScoreDisplay();
    }


    public void UpdateGameCreditsText()
    {
        coinsInsertedText.text = gameCredits.ToString("00");
    }


    #region SCREEN WRAP
    public void ScreenWrap(Transform gameOjectTransform)
    {
        bool objectIsVisible = WithinScreenBoundary(gameOjectTransform);

        if (objectIsVisible)
        {
            screenWrappingX = false;
            screenWrappingY = false;

            return;
        }


        if (screenWrappingX && screenWrappingY)
        {
            return;
        }


        Vector2 gameObjectPosition = gameOjectTransform.position;


        if (gameObjectPosition.y > ScreenBoundaryController.screenBoundaries.topScreenBoundary.position.y)
        {
            gameObjectPosition.y = ScreenBoundaryController.screenBoundaries.bottomScreenBoundary.position.y;

            screenWrappingY = true;
        }

        if (gameObjectPosition.y < ScreenBoundaryController.screenBoundaries.bottomScreenBoundary.position.y)
        {
            gameObjectPosition.y = ScreenBoundaryController.screenBoundaries.topScreenBoundary.position.y;

            screenWrappingY = true;
        }


        if (gameObjectPosition.x > ScreenBoundaryController.screenBoundaries.rightScreenBoundary.position.x)
        {
            gameObjectPosition.x = ScreenBoundaryController.screenBoundaries.leftScreenBoundary.position.x;

            screenWrappingX = true;
        }

        if (gameObjectPosition.x < ScreenBoundaryController.screenBoundaries.leftScreenBoundary.position.x)
        {
            gameObjectPosition.x = ScreenBoundaryController.screenBoundaries.rightScreenBoundary.position.x;

            screenWrappingX = true;
        }


        gameOjectTransform.position = gameObjectPosition;
    }


    private bool WithinScreenBoundary(Transform gameObjectTransform)
    {
        if (gameObjectTransform.position.y < ScreenBoundaryController.screenBoundaries.topScreenBoundary.position.y &&
            gameObjectTransform.position.y > ScreenBoundaryController.screenBoundaries.bottomScreenBoundary.position.y &&
            gameObjectTransform.position.x > ScreenBoundaryController.screenBoundaries.leftScreenBoundary.position.x &&
            gameObjectTransform.position.x < ScreenBoundaryController.screenBoundaries.rightScreenBoundary.position.x)
        {
            return true;
        }

        return false;
    }
    #endregion


} // end of class
