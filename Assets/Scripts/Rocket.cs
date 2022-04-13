using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    private float health = 100;

    [SerializeField]
    protected float rocketForce = 3;

    [SerializeField]
    private float explosionForce = 10;

    [SerializeField]
    private float explosionRadius = 2;

    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    protected float rotateSpeed = 20f;

    protected Explodable explodable;
    protected Rigidbody2D rb;

    private float minimumDamageThreshold = 1f;
    private ParticleSystem.EmissionModule em;

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        em = GetComponent<ParticleSystem>().emission;
        explodable = GetComponent<Explodable>();

    }

    private void Start()
    {
        
        em.enabled = true;

    }

    protected void Boost()
    {

        rb.AddForce(transform.right * rocketForce);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D colRb) || collision.gameObject.layer != 7) return;

        Vector3 normal = collision.GetContact(0).normal;
        Vector3 impulse = Extensions.ComputeTotalImpulse(collision);

        if (Vector3.Dot(normal, impulse) < 0f)
            impulse *= -1f;

        Vector3 myIncidentVelocity = (Vector3)rb.velocity - impulse / rb.mass;

        Vector3 otherIncidentVelocity = Vector3.zero;
        Rigidbody2D otherBody = collision.rigidbody;

        if (otherBody != null)
        {

            NavMeshAgent agent = collision.gameObject.GetComponent<NavMeshAgent>();
            if(agent != null)
            {

                otherIncidentVelocity = agent.velocity;

            }
            else
            {

                otherIncidentVelocity = otherBody.velocity;

            }

            if (!otherBody.isKinematic)
                otherIncidentVelocity += impulse / otherBody.mass;
        }

        float myApproach = Mathf.Max(0f, Vector3.Dot(myIncidentVelocity, normal));
        float otherApproach = Mathf.Max(0f, Vector3.Dot(otherIncidentVelocity, normal));

        float damage = Mathf.Max(0f, otherApproach - myApproach - minimumDamageThreshold);

        TakeDamage(damage);

    }
    protected void Death()
    {

        explodable.explode();

        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        ParticleSystem ps = explosion.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.EmissionModule em = ps.emission;
        ParticleSystem.MainModule mm = ps.main;
        mm.startSpeed = new ParticleSystem.MinMaxCurve(explosionForce/4, explosionForce/2);
        ParticleSystem.MinMaxCurve count = new ParticleSystem.MinMaxCurve(explosionForce * 0.3f, explosionForce * 0.5f);
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0, count);
        em.SetBurst(0, burst);
        ps.Play();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in colliders)
        {

            Rigidbody2D foundRb = col.GetComponent<Rigidbody2D>();
            if (foundRb != null)
            {

                Extensions.AddExplosionForce(foundRb, explosionForce, transform.position, explosionRadius);

            }

        }

        Destroy(gameObject);

    }
    protected void TakeDamage(float damage)
    {

        health -= damage;
        if(health < 0f)
        {

            Death();

        }

    }
    protected void PlayEmission()
    {
      
        em.rateOverDistance = rocketForce * 2;

    }
    protected void PauseEmission()
    {

        em.rateOverDistance = 0;

    }

}
