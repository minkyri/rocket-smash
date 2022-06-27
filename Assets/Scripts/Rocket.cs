using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    protected float health = 100;
    private float maxHealth;

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
    private ParticleSystem.MainModule mainModule;   

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        em = GetComponent<ParticleSystem>().emission;
        mainModule = GetComponent<ParticleSystem>().main;
        explodable = GetComponent<Explodable>();

    }

    protected virtual void Start()
    {
        
        em.enabled = true;
        mainModule.startSize = new ParticleSystem.MinMaxCurve((0.1f / 0.35f) * transform.localScale.y, (0.3f / 0.35f) * transform.localScale.y);
        maxHealth = health;

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

        if (

            !collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D colRb) || 
            collision.gameObject.layer == 8 ||
            (gameObject.tag == "Enemy" && collision.gameObject.tag == "Enemy")

        ) return;

        Vector3 normal = collision.GetContact(0).normal;
        Vector3 impulse = Extensions.ComputeTotalImpulse(collision);

        if (Vector3.Dot(normal, impulse) < 0f)
            impulse *= -1f;

        Vector3 myIncidentVelocity = (Vector3)rb.velocity - impulse / rb.mass;

        Vector3 otherIncidentVelocity = Vector3.zero;
        var otherBody = collision.rigidbody;
        if (otherBody != null)
        {
            otherIncidentVelocity = otherBody.velocity;
            if (!otherBody.isKinematic)
                otherIncidentVelocity += impulse / otherBody.mass;
        }

        // Compute how fast each one was moving along the collision normal,
        // Or zero if we were moving against the normal.
        float myApproach = Mathf.Max(0f, Vector3.Dot(myIncidentVelocity, normal));
        float otherApproach = Mathf.Max(0f, Vector3.Dot(otherIncidentVelocity, normal));

        float myMomentum = Mathf.Abs(Vector3.Dot(myIncidentVelocity, normal)) * rb.mass;
        float otherMomenum = Mathf.Abs(Vector3.Dot(otherIncidentVelocity, normal)) * otherBody.mass;

        //print(gameObject.name + " momenum = " + myMomentum);
        //print(gameObject.name + " other momenum = " + otherMomenum);

        if (myMomentum < otherMomenum)
        {

            float damage = Mathf.Max(0f, otherApproach - myApproach - minimumDamageThreshold);

            TakeDamage(damage);

        }

    }
    protected virtual void Death()
    {

        explodable.explode();

        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        ParticleSystem ps = explosion.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.EmissionModule em = ps.emission;
        ParticleSystem.MainModule mm = ps.main;
        mm.startSpeed = new ParticleSystem.MinMaxCurve(explosionForce/4, explosionForce/2);
        mm.startSize = new ParticleSystem.MinMaxCurve((0.08f/0.21f) * transform.localScale.y * transform.localScale.x, (0.2f / 0.21f) * transform.localScale.y * transform.localScale.x);
        ParticleSystem.MinMaxCurve count = new ParticleSystem.MinMaxCurve(explosionForce * 0.5f, explosionForce * 0.8f);
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

    public float GetHealth() { return health; }
    public float GetMaxHealth() { return maxHealth; }

}
