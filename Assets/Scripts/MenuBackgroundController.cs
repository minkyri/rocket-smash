using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MenuBackgroundController : MonoBehaviour
{

    public NavMeshSurface surface2d;

    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private GameObject[] enemyPrefabs;

    [SerializeField]
    [Range(0,50)]
    private int numberOfRockets;

    [SerializeField]
    private float wallThickness;

    private void Start()
    {

        Fragment();
        surface2d.BuildNavMesh();
        SetupBackgroundBattle();

    }

    private void Fragment()
    {

        if (transform.childCount > 0)
        {

            List<GameObject> children = new List<GameObject>();

            foreach (Transform tr in transform) children.Add(tr.gameObject);

            foreach (GameObject child in children)
            {

                if (child.TryGetComponent<Explodable>(out Explodable sc))
                {

                    sc.extraPoints = Mathf.RoundToInt(5 * child.transform.localScale.x * child.transform.localScale.y);
                    sc.fragmentInEditor();

                }

            }

        }

    }

    private void SetupBackgroundBattle()
    {

        GenerateWalls();    
        GenerateRockets();

    }

    private void GenerateRockets()
    {

        var vertExtent = Camera.main.orthographicSize - (wallThickness);
        var horzExtent = (vertExtent * Screen.width / Screen.height) - (wallThickness);

        for (int i = 0; i < numberOfRockets; i++)
        {

            Vector2 pos = new Vector2(Random.Range(-horzExtent, horzExtent), Random.Range(-vertExtent, vertExtent));
            GameObject rocket = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], pos, Quaternion.identity, transform);
            rocket.transform.localScale = rocket.transform.localScale * Random.Range(0.7f, 1.3f);
            
            if(rocket.TryGetComponent<SpriteRenderer>(out SpriteRenderer rend))
            {

                Color.RGBToHSV(rend.color, out float h, out float s, out float v);
                h = Random.Range(0f,1f);
                Color newColour = Color.HSVToRGB(h, s, v);
                rend.color = newColour;

            }

        }

    }

    private void GenerateWalls()
    {

        var vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;

        Transform topWall = Instantiate(wallPrefab, new Vector3(0, vertExtent), Quaternion.Euler(0, 0, 0), transform).transform;
        Transform bottomWall = Instantiate(wallPrefab, new Vector3(0, -vertExtent), Quaternion.Euler(0, 0, 0), transform).transform;

        Transform rightWall = Instantiate(wallPrefab, new Vector3(horzExtent, 0), Quaternion.Euler(0, 0, 0), transform).transform;
        Transform leftWall = Instantiate(wallPrefab, new Vector3(-horzExtent, 0), Quaternion.Euler(0, 0, 0), transform).transform;

        topWall.localScale = new Vector3(horzExtent*2 + wallThickness, wallThickness);
        bottomWall.localScale = new Vector3(horzExtent*2 + wallThickness, wallThickness);
        rightWall.localScale = new Vector3(wallThickness, vertExtent*2 + wallThickness);
        leftWall.localScale = new Vector3(wallThickness, vertExtent*2 + wallThickness);

    }

}
