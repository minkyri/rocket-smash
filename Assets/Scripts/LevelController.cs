using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelController : MonoBehaviour
{


    

    public float horizontalSize = 18;
    public float verticalSize = 8;
    [SerializeField] private float thickness = 0.75f;
    [SerializeField] private NavMeshSurface surface2d;
    [SerializeField] private GameObject gameFloorPrefab;
    [SerializeField] private GameObject wallPrefab;
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
        SpawnObjects();

        //surface2d.BuildNavMeshAsync();
        surface2d.BuildNavMesh();

        SpawnRockets();

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

        Transform floor = Instantiate(gameFloorPrefab, Vector3.zero, Quaternion.identity, transform).transform;
        floor.localScale = new Vector3(horizontalSize, verticalSize);

    }
    private void SpawnObjects()
    {

        

    }
    private void SpawnRockets()
    {

        GameObject[] rockets = new GameObject[]
        {

            Instantiate(playerPrefab, new Vector3(-25, 0), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(24.57f, 3.87f), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(-25.96f, -4.37f), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(7.95f, 2.77f), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(0f, 0f), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(-6.34f, 2.34f), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(4.11000013f,4.88999987f,0), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(2.08999991f,-6.25f,0), Quaternion.identity, transform),
            Instantiate(enemyPrefab, new Vector3(23.6599998f,-6.71000004f,0), Quaternion.identity, transform)


        };

        foreach(GameObject rocket in rockets)
        {

            if(rocket.TryGetComponent<Explodable>(out Explodable sc))
            {

                sc.fragmentInEditor();

            }
            

        }

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireCube(transform.position, new Vector3(horizontalSize, verticalSize, 1));

    }

}
