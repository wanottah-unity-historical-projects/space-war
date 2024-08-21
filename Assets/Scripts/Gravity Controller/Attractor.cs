
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Space War 1962 v2020.10.26
//
// created 2020.10.26
//


public class Attractor : MonoBehaviour
{
    public Rigidbody2D rb;


    private void FixedUpdate()
    {
        
    }


    private void Attract(Attractor objectToAttract)
    {
        Rigidbody2D rigidbodyToAttract = objectToAttract.rb;

        Vector3 direction = rb.position - rigidbodyToAttract.position;

        float distance = direction.magnitude;

        float forceMagnitude = (rb.mass * rigidbodyToAttract.mass) / Mathf.Pow(distance, 2);

        Vector3 force = direction.normalized * forceMagnitude;

        rigidbodyToAttract.AddForce(force);
    }

} // end of class
