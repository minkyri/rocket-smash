using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float overshootDistance = 1f;
    [SerializeField] private float followSpeed = 5;
    [SerializeField] private float maxOrthographicSize = 10f;
    private float minOrthographicSize;
    [SerializeField] private float zoomSpeed = 5;
    [SerializeField] private float deathPanDampening = 2f; 
    private Transform target;

    private Rigidbody2D targetRb;
    private Vector2 lastTargetVelocity;

    private bool deathPanning = false;

    float minX;
    float maxX;
    float minY;
    float maxY;

    private void Start()
    {

        minOrthographicSize = Camera.main.orthographicSize;

    }

    private void Update()
    {

        if (target == null)
        {

            if (GameObject.FindGameObjectWithTag("Player") != null)
            {

                target = GameObject.FindGameObjectWithTag("Player").transform;
                gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb);
                if(rb != null)Destroy(rb);

                target.gameObject.TryGetComponent(out targetRb);

                deathPanning = false;

            }
            else if(!deathPanning)
            {

                deathPanning = true;

                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.drag = deathPanDampening;
                rb.freezeRotation = true;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                rb.velocity = lastTargetVelocity;

            }

        }

        if(target != null)
        {

            float vertExtent = Camera.main.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            float horizontalSize = 0;
            float verticalSize = 0;

            if(GameController.instance.levelController != null)
            {

                Vector2 size = GameController.instance.levelController.GetLevelDimensions();
                horizontalSize = size.x;
                verticalSize = size.y;

            }

            //Debug.Log("V: " + vertExtent.ToString() + "     H: " + horzExtent.ToString());

            if (horzExtent > horizontalSize / 2.0f)
            {

                minX = 0;
                maxX = 0;

            }
            else
            {

                minX = (horzExtent - horizontalSize / 2.0f) - overshootDistance;
                maxX = (horizontalSize / 2.0f - horzExtent) + overshootDistance;

            }
            if (vertExtent > verticalSize / 2.0f)
            {

                minY = 0;
                maxY = 0;

            }
            else
            {

                minY = (vertExtent - verticalSize / 2.0f) - overshootDistance;
                maxY = (verticalSize / 2.0f - vertExtent) + overshootDistance;

            }

            Vector3 targetPosition = new Vector3(

                Mathf.Clamp(target.position.x, minX, maxX),
                Mathf.Clamp(target.position.y, minY, maxY)


            ) + offset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
            if (targetRb != null)
            {

                lastTargetVelocity = targetRb.velocity;

            }
            else
            {

                lastTargetVelocity = Vector3.zero;

            }
            //transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * followSpeed);

            if (!(horzExtent > horizontalSize / 2.0f && vertExtent > verticalSize / 2.0f))
            {

                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Mathf.Clamp(targetRb.velocity.magnitude, minOrthographicSize, maxOrthographicSize), Time.deltaTime * zoomSpeed);

            }       

        }

    }

}
