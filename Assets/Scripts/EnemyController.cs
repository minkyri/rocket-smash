using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Rocket
{


    //GameObject target;
    //private void FixedUpdate()
    //{

    //    if (target == null)
    //    {

    //        target = GameObject.FindGameObjectWithTag("Player");

    //    }
    //    if (target != null)
    //    {

    //        Vector2 targetPosition = target.transform.position - transform.position;
    //        float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
    //        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);

    //        PlayEmission();
    //        Boost();

    //    }

    //}

    Transform target;
    private void FixedUpdate()
    {

        if (target == null)
        {

            target = GameObject.FindGameObjectWithTag("Player").transform;

        }
        if (target != null)
        {

            Vector2[] points = Pathfinding.RequestPath(transform.position, target.position);



            Vector2 targetPosition = target.position - transform.position;
            float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);

            PlayEmission();
            Boost();

        }

    }

}
