using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour
{

    private Rigidbody2D rb;
    private MeshRenderer mr;
    private float timeToFade = 5;
    private float timer;

    private void Awake()
    {

        gameObject.layer = 8;
        rb = GetComponent<Rigidbody2D>();
        mr = GetComponent<MeshRenderer>();
        rb.useAutoMass = true;
        rb.gravityScale = 0;
        rb.angularDrag = 0;
        rb.drag = 1;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Extrapolate;


    }
    private void Start()
    {

        timer = timeToFade;

    }

    private void Update()
    {
        
        timer -= Time.deltaTime;
        if(timer <= 0)
        {

            Destroy(gameObject);

        }
        else
        {

            mr.material.color = new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, timer/timeToFade);

        }

    }

}
