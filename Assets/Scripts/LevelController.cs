using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
        GameObject level = GameObject.FindGameObjectWithTag("Level");

        List<GameObject> children = new List<GameObject>();
        foreach (Transform tr in level.transform) children.Add(tr.gameObject);

        foreach (GameObject child in children)
        {

            if (child.TryGetComponent<Explodable>(out Explodable sc))
            {

                sc.fragmentInEditor();

            }

        }

        surface2d.BuildNavMesh();

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
    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireCube(transform.position, new Vector3(horizontalSize, verticalSize, 1));

    }

}
