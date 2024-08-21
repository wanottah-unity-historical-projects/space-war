
using UnityEngine;

//
// Space War 1962 v2020.10.30
//
// created 2020.10.28
//


public class AccretionDisk : MonoBehaviour
{
    private float rotationSpeed = 0.02f;


    private void FixedUpdate()
    {
        Rotate();
    }


    public void Rotate()
    {
        transform.Rotate(0f, 0f, GameController.CLOCKWISE * rotationSpeed);
    }


} // end of class
