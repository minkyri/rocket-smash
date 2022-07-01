using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTimer : MonoBehaviour
{

    [SerializeField]
    private float igniteHealthThreshold = 3f;

    [SerializeField]
    private float flameAmount = 1f;

    [SerializeField]
    private float explosionSpeed = 1f;

    [SerializeField]
    private Entity entity;

    [SerializeField]
    private GameObject flame;

    private bool startedExplosion = false;
    private ParticleSystem.EmissionModule flameEmissionModule;
    private ParticleSystem.MainModule flameMainModule;

    private void Start()
    {

        ParticleSystem flameSystem = Instantiate(flame, transform).GetComponent<ParticleSystem>();

        flameMainModule = flameSystem.main;
        flameEmissionModule = flameSystem.emission;

        flameEmissionModule.enabled = false;
        flameSystem.transform.localScale = entity.transform.localScale;
        flameMainModule.startSize = new ParticleSystem.MinMaxCurve(Mathf.Sqrt(0.10714f * transform.localScale.y * transform.localScale.x), Mathf.Sqrt(0.42857f * transform.localScale.y * transform.localScale.x));

    }

    private void Update()
    {

        if (!startedExplosion)
        {

            if (entity.GetMaxHealth() - entity.GetHealth() > igniteHealthThreshold)
            {

                startedExplosion = true;
                StartCoroutine(BombTimer());
                flameEmissionModule.enabled = true;

            }
            else
            {

                flameEmissionModule.enabled = false;

            }

        }

    }

    private IEnumerator BombTimer()
    {

        while(true)
        {

            if (entity.GetMaxHealth() - entity.GetHealth() < igniteHealthThreshold)
            {

                startedExplosion = false; 
                yield break;

            }

            entity.TakeDamage(Time.deltaTime * explosionSpeed);
            flameEmissionModule.rateOverTime = flameAmount / (entity.GetHealth() / 50);
            yield return null;

        }

    }

}
