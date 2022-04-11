using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float overshootDistance = 1f;
    [SerializeField] private float followSpeed = 5;
    [SerializeField] private Transform target;

    private LevelController levelController;

    float minX;
    float maxX;
    float minY;
    float maxY;

    private void Awake()
    {

        levelController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelController>();

    }

    private void Update()
    {

        try
        {

            target = GameObject.FindGameObjectWithTag("Player").transform;

        }
        catch (System.NullReferenceException)
        {

            return;

        }

        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        //Debug.Log("V: " + vertExtent.ToString() + "     H: " + horzExtent.ToString());

        if (horzExtent > levelController.horizontalSize / 2.0f)
        {

            minX = 0;
            maxX = 0;

        }
        else
        {

            minX = (horzExtent - levelController.horizontalSize / 2.0f) - overshootDistance;
            maxX = (levelController.horizontalSize / 2.0f - horzExtent) + overshootDistance;

        }
        if (vertExtent > levelController.verticalSize / 2.0f)
        {

            minY = 0;
            maxY = 0;

        }
        else
        {

            minY = (vertExtent - levelController.verticalSize / 2.0f) - overshootDistance;
            maxY = (levelController.verticalSize / 2.0f - vertExtent) + overshootDistance;

        }

        Vector3 targetPosition = new Vector3(

            Mathf.Clamp(target.position.x, minX, maxX),
            Mathf.Clamp(target.position.y, minY, maxY)


        ) + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        //transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * followSpeed);

    }

}
