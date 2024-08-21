
using UnityEngine;

//
// Space War v2020.10.28
//
// created 2020.10.16
//


public class EnemyScoreController : MonoBehaviour
{
    public static EnemyScoreController enemyScoreController;

    public SpriteRenderer[] enemyScoreDigit;


    private void Awake()
    {
        enemyScoreController = this;
    }


    public void UpdateEnemyScoreDisplay()
    {
        enemyScoreDigit[1].sprite = GameController.gameController.number[GameController.gameController.enemyScore % 16];
        enemyScoreDigit[0].sprite = GameController.gameController.number[GameController.gameController.enemyScore / 16];
    }


} // end of class
