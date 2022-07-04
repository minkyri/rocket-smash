using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowEnemyController : EnemyController
{

    [SerializeField]
    private GameObject explosiveBarrel;

    [SerializeField]
    private float checkForEnemyRadius = 5;

    [SerializeField]
    private float bombingDistance = 4;

    [SerializeField]
    private float bombTime = 0.6f;

    [SerializeField]
    private float bombCooldownTime = 2f;

    [SerializeField]
    private int bombsDroppedBeforeCooldown = 3;

    private float bombCounter;
    private float cooldownCounter;

    private int bombsDropped;

    private bool canBomb;

    protected override void Start()
    {

        base.Start();
        bombCounter = bombTime;
        cooldownCounter = bombCooldownTime;
        bombsDropped = 0;
        canBomb = true;

    }

    protected override void Update()
    {

        if (canBomb)
        {

            bombCounter -= Time.deltaTime;

            bool enemyInRange = false;
            if(GetTargetTag() != "Enemy")
            {

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, checkForEnemyRadius);
                foreach (Collider2D col in colliders)
                {

                    if (col.gameObject.tag == "Enemy" && col.gameObject != gameObject)
                    {

                        enemyInRange = true;

                    }

                }

            }

            if (target != null && Vector2.Distance(transform.position, target.position) < bombingDistance && bombCounter <= 0 && !enemyInRange)
            {

                bombCounter = bombTime;

                Transform bomb = Instantiate(explosiveBarrel, transform.position + (-transform.right * transform.localScale.x / 2), transform.rotation, transform.parent).transform;

                //bomb.localScale = ((bomb.localScale.magnitude / transform.localScale.magnitude) * bomb.localScale) * 0.75f;

                float sideLength = Mathf.Sqrt(0.4412214222f * transform.localScale.y * transform.localScale.x);
                bomb.localScale = Vector2.one * sideLength; 

                if (bomb.gameObject.TryGetComponent<Explodable>(out Explodable sc))
                {

                    sc.extraPoints = Mathf.RoundToInt(5 * bomb.localScale.x * bomb.localScale.y);
                    sc.fragmentInEditor();

                }
                if (bomb.childCount > 0)
                {

                    LevelController.FragmentChildren(bomb.gameObject);

                }
                if (bomb.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D bombRb))
                {

                    bombRb.AddTorque(Random.Range(-rocketForce, rocketForce));
                    bombRb.AddForce(-transform.right * rocketForce * Random.Range(0.1f, 0.2f), ForceMode2D.Impulse);

                }
                if (bomb.gameObject.TryGetComponent<Entity>(out Entity entity))
                {

                    entity.AutoSetExplosionStats();
                    entity.health = 35.36f * bomb.localScale.magnitude;

                }
                if (bomb.gameObject.TryGetComponent<ExplosionTimer>(out ExplosionTimer bombTimer))
                {

                    bombTimer.SetExplosionSpeed(10f);
                    bombTimer.Ignite();

                }

                bombsDropped ++;

                if(bombsDropped >= bombsDroppedBeforeCooldown)
                {

                    canBomb = false;
                    bombCounter = bombTime;
                    bombsDropped = 0;

                }

            }

        }
        else
        {

            cooldownCounter -= Time.deltaTime;

            if(cooldownCounter <= 0)
            {

                cooldownCounter = bombCooldownTime;
                canBomb = true;

            }

        }

    }

}
