using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{

    public bool generateWalls = true;
    public float horizontalSize = 18;
    public float verticalSize = 8;
    [SerializeField] private float thickness = 0.75f;
    public Color themeColor;
    public string description;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject gameFloorPrefab;

    Transform floor;

    private void Awake()
    {

        floor = Instantiate(gameFloorPrefab, Vector3.zero, Quaternion.identity, transform).transform;
        floor.localScale = new Vector3(horizontalSize, verticalSize);

    }

    private void Start()
    {

        if(generateWalls) GenerateWalls();
        ColourObjects();
        SetCameraOnPlayer();

    }

    private void SetCameraOnPlayer()
    {

        foreach (Transform tr in transform)
        {

            if (tr.gameObject.tag == "Player")Camera.main.transform.position = tr.transform.position;

        }

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

    private void ColourObjects()
    {

        Color.RGBToHSV(themeColor, out float h, out float s, out float v);
        Color wallColour;
        Color floorColour;

        if (s == 0)
        {

            wallColour = Color.white;
            floorColour = Color.HSVToRGB(0, 0, 0.1f);

        }
        else
        {

            wallColour = Color.HSVToRGB(h, 0.7f, 1);
            floorColour = Color.HSVToRGB(h, 0.5f, 0.1f);

        }

        foreach (Transform tr in transform)
        {

            tr.TryGetComponent<SpriteRenderer>(out SpriteRenderer rend);
            if(
                
                rend != null &&
                tr.gameObject.tag == "Wall"
                
            )rend.color = wallColour;

        }

        floor.GetComponent<SpriteRenderer>().color = floorColour;
        Camera.main.backgroundColor = floorColour;

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireCube(transform.position, new Vector3(horizontalSize, verticalSize, 1));

    }

}
