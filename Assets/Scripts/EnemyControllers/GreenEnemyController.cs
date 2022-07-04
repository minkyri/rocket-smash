using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemyController : EnemyController
{

    private float startingRocketForce;

    private float lastFrameDistanceToTarget;

    protected override void Start()
    {

        base.Start();

        startingRocketForce = rocketForce;

    }

    protected override void FixedUpdate()
    {

        if(target != null)
        {

            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget < lastFrameDistanceToTarget)
            {


                RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(transform.position, target.position - transform.position, distanceToTarget);

                bool hitObstacle = false;
                foreach (RaycastHit2D hit in raycastHit2Ds)
                {

                    if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Obstacle") hitObstacle = true;

                }

                if (!hitObstacle)
                {

                    rocketForce = startingRocketForce * 2f;

                }

            }
            else
            {

                rocketForce = startingRocketForce;

            }

            lastFrameDistanceToTarget = distanceToTarget;

        }
        else
        {

            rocketForce = startingRocketForce;

        }

        base.FixedUpdate();

    }

}
