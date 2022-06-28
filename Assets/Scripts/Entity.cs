using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{

    [SerializeField]
    protected float health = 100;
    private float maxHealth;

    [SerializeField]
    private bool explodeFragmentsOnly;

    [SerializeField]
    private float explosionForce = 10;

    [SerializeField]
    private float explosionRadius = 2;

    [SerializeField]
    private GameObject explosionPrefab;

    private float minimumDamageThreshold = 1f;

    private Rigidbody2D lastCollisionRb;
    private Vector2 lastCollisionRbVelocity;
    private Vector2 lastFrameVelocity;

    protected Explodable explodable;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        explodable = GetComponent<Explodable>();

    }

    protected virtual void Start()
    {

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
        lastCollisionRb = otherBody;

        if (otherBody != null)
        {
            otherIncidentVelocity = otherBody.velocity;
            if (!otherBody.isKinematic)
                otherIncidentVelocity += impulse / otherBody.mass;
        }

        lastCollisionRbVelocity = otherIncidentVelocity;
        lastFrameVelocity = myIncidentVelocity;

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

        Vector2 collisionVelocity = ((lastCollisionRb.mass * lastCollisionRbVelocity) - (rb.mass * lastFrameVelocity)) / rb.mass;
        explodable.explode();
        explodable.SetFragmentVelocity(collisionVelocity);

        if (explosionPrefab != null)
        {

            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            ParticleSystem ps = explosion.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.EmissionModule em = ps.emission;
            ParticleSystem.MainModule mm = ps.main;
            mm.startSpeed = new ParticleSystem.MinMaxCurve(explosionForce / 4, explosionForce / 2);
            mm.startSize = new ParticleSystem.MinMaxCurve((0.08f / 0.21f) * transform.localScale.y * transform.localScale.x, (0.2f / 0.21f) * transform.localScale.y * transform.localScale.x);
            ParticleSystem.MinMaxCurve count = new ParticleSystem.MinMaxCurve(explosionForce * 0.5f, explosionForce * 0.8f);
            ParticleSystem.Burst burst = new ParticleSystem.Burst(0, count);
            em.SetBurst(0, burst);
            ps.Play();

        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in colliders)
        {

            Rigidbody2D foundRb = col.GetComponent<Rigidbody2D>();
            if (foundRb != null)
            {

                if(explodeFragmentsOnly)
                {

                    if(foundRb.gameObject.TryGetComponent<Fragment>(out Fragment frag)) Extensions.AddExplosionForce(foundRb, explosionForce, transform.position, explosionRadius);

                }
                else
                {

                    Extensions.AddExplosionForce(foundRb, explosionForce, transform.position, explosionRadius);

                }

            }

        }

        Destroy(gameObject);

    }

    protected void TakeDamage(float damage)
    {

        health -= damage;
        if (health < 0f)
        {

            Death();

        }

    }

    public float GetHealth() { return health; }
    public float GetMaxHealth() { return maxHealth; }

}
