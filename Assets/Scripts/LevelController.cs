using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [SerializeField]
    private GameObject wallPrefab;

    public float horizontalSize = 18;
    public float verticalSize = 8;
    [SerializeField] private float thickness = 0.75f;
    [SerializeField] private GameObject dynamicObjectPrefab;
    [SerializeField] private GameObject staticObjectPrefab;
    [SerializeField] private GameObject kinematicObjectPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    private void Start()
    {
        
        GenerateLevel();

    }

    private void GenerateLevel()
    {

        if (wallPrefab == null)
        {

            Debug.Log("Wall Prefab has not been assigned!");
            return;

        }

        GenerateWalls();
        //SpawnObjects();
        //SpawnRockets();

    }

    private void GenerateWalls()
    {

        Transform topWall = Instantiate(wallPrefab, new Vector3(0, verticalSize / 2), Quaternion.Euler(0, 0, 0), transform).transform;
        Transform bottomWall = Instantiate(wallPrefab, new Vector3(0, -verticalSize / 2), Quaternion.Euler(0, 0, 0), transform).transform;

        Transform rightWall = Instantiate(wallPrefab, new Vector3(horizontalSize / 2, 0), Quaternion.Euler(0, 0, 0), transform).transform;
        Transform leftWall = Instantiate(wallPrefab, new Vector3(-horizontalSize / 2, 0), Quaternion.Euler(0, 0, 0), transform).transform;

        topWall.localScale = new Vector3(horizontalSize + thickness, thickness);
        bottomWall.localScale = new Vector3(horizontalSize + thickness, thickness);
        rightWall.localScale = new Vector3(thickness, verticalSize + thickness);
        leftWall.localScale = new Vector3(thickness, verticalSize + thickness);

    }
    private void SpawnObjects()
    {

        

    }
    private void SpawnRockets()
    {

        GameObject[] rockets = new GameObject[]
        {

            Instantiate(playerPrefab, new Vector3(0, 0), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(10, 0), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(-11, 3), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(2, -8), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(7, 6), Quaternion.identity, transform)

        };

        foreach(GameObject rocket in rockets)
        {

            if(rocket.TryGetComponent<Explodable>(out Explodable sc))
            {

                sc.fragmentInEditor();

            }
            

        }

    }

}
