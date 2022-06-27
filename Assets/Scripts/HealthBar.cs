using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5;

    [SerializeField]
    private float colourChangeSpeed = 5;

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Gradient gradient;

    private Slider slider;
    private Rocket rocket;

    private void Awake()
    {

        slider = GetComponent<Slider>();

    }

    private void Start()
    {

        FindPlayer();
        slider.minValue = 0;

    }

    private void Update()
    {

        float health;

        if (rocket == null)
        {

            FindPlayer();

        }
        if (rocket != null)
        {

            slider.maxValue = rocket.GetMaxHealth();
            health = rocket.GetHealth();

            if (healthBar != null)
            {

                float t = slider.value / slider.maxValue;
                if (t >= 0 && t <= 1) healthBar.color = Color.Lerp(healthBar.color, gradient.Evaluate(t), Time.deltaTime * colourChangeSpeed);

            }

        }
        else health = 0;

        slider.value = Mathf.Lerp(slider.value, health, Time.deltaTime * moveSpeed);

    }

    private void FindPlayer()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {

            player.TryGetComponent<Rocket>(out rocket);

        }

    }

}
