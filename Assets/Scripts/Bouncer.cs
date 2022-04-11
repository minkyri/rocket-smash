using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{

    private Rigidbody2D rb;
    private Vector3 lastFrameVelocity;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {

        lastFrameVelocity = rb.velocity;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Bounce(collision.contacts[0].normal);

    }

    private void Bounce(Vector3 collisionNormal)
    {

        float speed = lastFrameVelocity.magnitude;
        Vector3 direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);

        rb.velocity = direction * speed;

    }

}
