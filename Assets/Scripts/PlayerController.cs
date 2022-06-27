using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Rocket
{

    private void FixedUpdate()
    {

        if (rb == null) return;

        rb.angularVelocity = 0;
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle), Time.deltaTime * rotateSpeed);

        if (Input.GetButton("Fire1"))
        {

            rb.AddForce(transform.right * rocketForce);
            PlayEmission();

        }
        else
        {

            PauseEmission();

        }

    }

    protected override void Death()
    {

        GameController.instance.levelController.playerDead = true;
        base.Death();

    }

}
