using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rocket : Entity
{

    [SerializeField]
    protected float rocketForce = 3;

    [SerializeField]
    protected float rotateSpeed = 20f;

    private ParticleSystem.EmissionModule em;
    private ParticleSystem.MainModule mainModule;   

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

    protected void PlayEmission()
    {

        em.rateOverDistance = rocketForce * 2;

    }
    protected void PauseEmission()
    {

        em.rateOverDistance = 0;

    }

}
