using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Rocket
{

    [SerializeField]
    private float awareRadius = 10f;

    [SerializeField]
    private float wanderRadius = 5f;

    [SerializeField]
    private float wanderTimer = 1f;

    private float timer;

    Transform target;
    NavMeshAgent agent;

    Vector3 lastVelocity;
    Vector3 acceleration;

    private void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;

        lastVelocity = Vector3.zero;

        timer = wanderTimer;

        PlayEmission();

    }

    private void FixedUpdate()
    {

        if (target == null)
		{

			if (GameObject.FindGameObjectWithTag("Player") != null) target = GameObject.FindGameObjectWithTag("Player").transform;

		}

        timer -= Time.fixedDeltaTime;

        if (
            target != null &&
            !Extensions.CheckIfPathPossible(agent, target.position) && 
            Vector2.Distance(transform.position, target.position) <= awareRadius
        )
        {

            agent.acceleration = rocketForce;
            agent.SetDestination(target.position);

        }
        else if(timer <= 0f)
        {

            agent.acceleration = rocketForce / 6;
            agent.SetDestination(Extensions.RandomNavSphere(transform.position, wanderRadius, 7));
            timer = wanderTimer;

        }

        rb.MovePosition(agent.nextPosition);


        acceleration = (agent.velocity - lastVelocity) / Time.deltaTime;

        float angle = Mathf.Atan2(acceleration.y, acceleration.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle), Time.fixedDeltaTime * rotateSpeed);

        lastVelocity = agent.velocity;

    }

}
