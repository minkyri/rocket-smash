using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Rocket
{
    
    [SerializeField]
    private float rotateSpeed = 20;
    private Vector2 mouseDir;

    private void FixedUpdate()
    {

        if (rb == null) return;

        rb.angularVelocity = 0;
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle), Time.deltaTime * rotateSpeed);

        if (Input.GetButton("Fire1"))
        {

            Boost();
            PlayEmission();

        }
        else
        {

            PauseEmission();

        }

    }

}
