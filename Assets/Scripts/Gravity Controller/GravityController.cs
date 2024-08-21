
using UnityEngine;

//
// Space War 1962 v2020.11.01
//
// created 2020.10.31
//


public class GravityController : MonoBehaviour
{
    private float gravitationalConstant_G = 0.6674f; // 667.4f;

    private float gravityWellMass = -0.1f;

    private float gravityMassIncreaseTimer = 1f;



    private void Update()
    {
        //IncreaseGravityMass();
    }


    private void IncreaseGravityMass()
    {
        if (!GameController.gameController.inDemoMode)
        {
            gravityMassIncreaseTimer -= Time.deltaTime;

            if (gravityMassIncreaseTimer <= 0)
            {
                gravityWellMass -= 0.01f;

                //blackHole.localScale += new Vector3(0.01f, 0.01f, 0f);

                gravityMassIncreaseTimer = 1f;
            }
        }
    }


    public void CreateGravity(Transform gameObjectTransform)
    {
        Rigidbody2D gameObjectRigidbody = gameObjectTransform.GetComponent<Rigidbody2D>();

        Vector3 gravityDirection = (gameObjectTransform.position - transform.position).normalized;

        float gravityDistance = gravityDirection.magnitude;

        float gravitationalForce = gravitationalConstant_G * (gravityWellMass * gameObjectRigidbody.mass) / Mathf.Pow(gravityDistance, 2);

        Vector3 gravity = gravityDirection * gravitationalForce;

        gameObjectRigidbody.AddForce(gravity);
    }


} // end of class
