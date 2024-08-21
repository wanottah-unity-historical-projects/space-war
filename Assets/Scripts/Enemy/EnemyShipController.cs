
using UnityEngine;

//
// Space War 1962 v2020.10.28
//
// created 2020.10.16
//


public class EnemyShipController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.parent.GetComponent<EnemyController>().CollisionDetected(this);
    }


} // end of class
