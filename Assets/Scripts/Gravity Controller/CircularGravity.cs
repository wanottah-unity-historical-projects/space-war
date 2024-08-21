
using UnityEngine;

//
// Space War 1962 v2020.10.27
//
// created 2020.10.26
//


public class CircularGravity : MonoBehaviour
{
    private Rigidbody2D player;

    public Transform earth;

    public float gravitationalConstant_G;

    public float massOfEarth;

    private float massOfPlayer;



    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        Initialise();
    }


    private void FixedUpdate()
    {
        ApplyGravity();
    }


    private void Initialise()
    {
        //gravitationalConstant_G = 6.674f;

        massOfPlayer = player.mass;


    }


    private void ApplyGravity()
    {
        Vector3 direction = earth.position - transform.position;

        float distanceFromPlayerToEarth = direction.magnitude;

        float gravitationalForce = gravitationalConstant_G * (massOfEarth * massOfPlayer) / Mathf.Pow(distanceFromPlayerToEarth, 2);

        Vector3 gravity = direction.normalized * gravitationalForce;

        player.AddForce(gravity);
    }


} // end of class
