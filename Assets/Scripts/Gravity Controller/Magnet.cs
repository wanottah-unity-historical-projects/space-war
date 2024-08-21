
using UnityEngine;

//
// Space War 1962 v2020.10.27
//
// created 2020.10.27
//

public class Magnet : MonoBehaviour
{
    public GameObject magnet;

    private Rigidbody2D rb;

    public float gravitationalContant_G;

    public float earthMass;
    private float playerMass;



    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Initialise();
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        CreateMagnet();
    }


    private void Initialise()
    {
        //forceFactor = 1f;
        playerMass = rb.mass;
    }


    private void CreateMagnet()
    {
        Vector3 gravityDirection = (magnet.transform.position - transform.position).normalized;

        float distance = gravityDirection.magnitude;

        float gravitationalForce = gravitationalContant_G * (earthMass * playerMass) / Mathf.Pow(distance, 2);

        Vector3 gravity = gravityDirection * gravitationalForce;

        rb.AddForce(gravity); // gravityDirection * forceFactor);
    }


} // end of class
