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
    private bool startedFlashing = false;
    private ParticleSystem.EmissionModule flameEmissionModule;
    private ParticleSystem.MainModule flameMainModule;
    private SpriteRenderer rend;

    private void Awake()
    {

        rend = GetComponent<SpriteRenderer>();

    }

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

        if (!startedExplosion && !startedFlashing)
        {

            if (entity.GetMaxHealth() - entity.GetHealth() > igniteHealthThreshold)
            {

                startedExplosion = true;
                startedFlashing = true;
                StartCoroutine(Flaming());
                StartCoroutine(Flashing());
                flameEmissionModule.enabled = true;

            }
            else
            {

                flameEmissionModule.enabled = false;

            }

        }

    }

    private IEnumerator Flaming()
    {

        while (entity.GetMaxHealth() - entity.GetHealth() > igniteHealthThreshold)
        {

            entity.TakeDamage(Time.deltaTime * explosionSpeed);
            flameEmissionModule.rateOverTime = flameAmount / (entity.GetHealth() / 50);

            yield return null;

        }

        startedExplosion = false;
        yield break;

    }

    private IEnumerator Flashing()
    {

        Color startColour = rend.color;
        Color.RGBToHSV(startColour, out float h, out float s, out float v);

        //h = h * 360;
        //if (h + 50 > 360)
        //{

        //    h = (h + 50) - 360;

        //}
        //h = h / 360;

        //Color bombColour = Color.HSVToRGB(h, s, v);
        Color bombColour = Color.HSVToRGB(0, 0, 1);

        while(entity.GetMaxHealth() - entity.GetHealth() > igniteHealthThreshold)
        {

            yield return StartCoroutine(LerpToColour(rend.color, bombColour, 0.1f));

            if(entity.GetHealth() > 0.1f * entity.GetMaxHealth())
            {

                yield return StartCoroutine(LerpToColour(rend.color, startColour, 0.1f));
                yield return new WaitForSeconds(entity.GetHealth() / 50);

            }

        }

        yield return StartCoroutine(LerpToColour(rend.color, startColour, 1f));
        startedFlashing = false;

    }
    private IEnumerator LerpToColour(Color startColour, Color endColour, float time)
    {

        float timeElapsed = 0;
        while (timeElapsed < time)
        {

            rend.color = Color.Lerp(startColour, endColour, timeElapsed/time);
            timeElapsed += Time.deltaTime;
            yield return null;

        }

    }

}
