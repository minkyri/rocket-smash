using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rocket : Entity
{

    [SerializeField]
    protected float rocketForce = 2f;

    [SerializeField]
    protected float rotateSpeed = 10f;

    private ParticleSystem.EmissionModule em;
    private ParticleSystem.MainModule mainModule;

    private bool particlePlaying = false;

    protected override void Awake()
    {
        
        base.Awake();
        em = GetComponent<ParticleSystem>().emission;
        mainModule = GetComponent<ParticleSystem>().main;

    }

    protected override void Start()
    {
        
        base.Start();
        em.enabled = true;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(Mathf.Sqrt(0.10714f * transform.localScale.y * transform.localScale.x), Mathf.Sqrt(0.42857f * transform.localScale.y * transform.localScale.x));

    }

    protected virtual void Update()
    {

        if (particlePlaying)
        {

            em.rateOverDistance = rocketForce * 3;

        }

    }

    protected void PlayEmission()
    {

        particlePlaying = true;
        em.rateOverDistance = rocketForce * 2;

    }
    protected void PauseEmission()
    {

        particlePlaying = false;
        em.rateOverDistance = 0;

    }

}
