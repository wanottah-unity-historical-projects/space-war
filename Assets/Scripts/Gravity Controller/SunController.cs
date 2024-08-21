
using UnityEngine;

//
// Space War 1962 v2020.11.01
//
// created 2020.11.01
//


public class SunController : MonoBehaviour
{
    public GameObject[] flares;

    private int flare;

    private float mainTimer;

    private float timer;
    private bool canCount;
    private bool doOnce;

    private const float SEQUENCE_TIMER =  0.04f;



    private void Start()
    {
        Initialise();

        ShowFlare();
    }


    private void Update()
    {
        RunTimer(flare);
    }


    private void Initialise()
    {
        mainTimer = SEQUENCE_TIMER;

        canCount = false;

        doOnce = false;

        timer = mainTimer;
    }


    private void ShowFlare()
    {
        flare = Random.Range(0, flares.Length); // += LedDirection();

        flares[flare].gameObject.SetActive(true);

        StartTimer();
    }


    private void ClearFlare(int flare)
    {
        flares[flare].gameObject.SetActive(false);
    }


    private void StartTimer()
    {
        timer = mainTimer;

        canCount = true;

        doOnce = false;
    }


    private void RunTimer(int flare)
    {
        if (canCount && timer >= 0.0f)
        {
            timer -= Time.deltaTime;
        }

        else if (!doOnce && timer <= 0.0f)
        {
            canCount = false;

            doOnce = true;

            timer = 0.0f;

            ClearFlare(flare);

            ShowFlare();
        }
    }


} // end of class
