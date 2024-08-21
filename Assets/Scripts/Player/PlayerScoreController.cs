
using UnityEngine;

//
// Computer Space 1971 v2020.11.02
//
// created 2020.10.16
//


public class PlayerScoreController : MonoBehaviour
{
    public static PlayerScoreController playerScoreController;

    public SpriteRenderer[] led;

    public Sprite ledOn;
    public Sprite ledOff;

    public SpriteRenderer[] playerScoreDigit;
        
    private string binaryString;



    private void Awake()
    {
        playerScoreController = this;
    }


    public void UpdatePlayerScoreDisplay()
    {
        playerScoreDigit[1].sprite = GameController.gameController.number[GameController.gameController.playerScore % 10];
        playerScoreDigit[0].sprite = GameController.gameController.number[GameController.gameController.playerScore / 10];

        if (GameController.gameController.playerScore > 0)
        {
            ConvertToBinary(GameController.gameController.playerScore);

            IlluminateLEDs();
        }
    }


    private void ConvertToBinary(int score)
    {
        int quot;

        string binaryBits = "";

        binaryString = "";

        while (score >= 1)
        {
            quot = score / 2;

            binaryBits += (score % 2).ToString();

            score = quot;
        }

        for (int bit = binaryBits.Length - 1; bit >= 0; bit--)
        {
            binaryString += binaryBits[bit];
        }
    }


    private void IlluminateLEDs()
    {
        string ledPanel = "000000000000000000";

        string leds = ledPanel.Substring(0, (ledPanel.Length - 1) - (binaryString.Length - 1)) + binaryString;
        
        for (int bit = 0; bit < leds.Length; bit++)
        {
            if (leds.Substring(bit, 1) == "0")
            {
                led[bit].sprite = ledOff;
            }

            if (leds.Substring(bit, 1) == "1")
            {
                led[bit].sprite = ledOn;
            }
        }
    }


} // end of class
