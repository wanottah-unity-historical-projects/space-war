
using UnityEngine;

//
// Space War 1962 v2020.10.31
//
// created 2020.10.31
//


public class GravityBody : MonoBehaviour
{
    public GravityController gravityController;


    private void FixedUpdate()
    {
        gravityController.CreateGravity(transform);
    }


} // end of class
