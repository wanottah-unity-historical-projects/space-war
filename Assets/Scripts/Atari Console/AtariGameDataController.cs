
using UnityEngine;

//
// Atari Video Game Data Controller v2020.10.27
//
// created 17-12-2019
//


public class AtariGameDataController : MonoBehaviour
{
    // reference to atari game data script
    private AtariGameData atariGameData;


    private void Awake()
    {
        atariGameData = GetComponent<AtariGameData>();
    }


    public void SelectGame(string GAME_TITLE)
    {
        switch (GAME_TITLE)
        {
            case AtariGameData.SPACEWAR:

                atariGameData.SpaceWar();

                break;
        }
    }


} // end of class
