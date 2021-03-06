using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{

    public NavMeshSurface surface2d;

    [SerializeField] private bool generateLevels = true;
    [SerializeField] private int lives = 3;
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float messageDuration;
    [SerializeField] private float timeBeforeLevelExit;
    [SerializeField] private GameObject[] hudElements;
    [SerializeField] private GameObject versionPanel;
    [SerializeField] private RawImage fadeScreen;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI livesText;
    [HideInInspector] public GameObject level;
    [HideInInspector] public bool isTransitioning = true;
    [HideInInspector] public bool playerDead = false;

    private Color originalBackgroundColour;
    private int levelCount;
    private int enemyCount;
    private bool canProgressToNextLevel;
    private Coroutine checkEnemyCount;

    private void Start()
    {

        if (generateLevels)
        {

            canProgressToNextLevel = false;
            levelCount = 0;

            if (transform.childCount > 0)
            {

                foreach (Transform tr in transform)
                {

                    Destroy(tr.gameObject);

                }

            }

            SetMessageSettings();

            HideHUD(true);
            originalBackgroundColour = Camera.main.backgroundColor;
            Camera.main.backgroundColor = fadeScreen.color;
            StartCoroutine(GenerateLevel());

        }
        else
        {

            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0);
            stageText.color = new Color(stageText.color.r, stageText.color.g, stageText.color.b, 0);
            descriptionText.color = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, 0);
            level = transform.GetChild(0).gameObject;
            //level = Instantiate(levelPrefabs[0], Vector3.zero, Quaternion.identity, transform);
            Fragment();
            surface2d.BuildNavMesh();

        }

    }

    private void HideHUD(bool hide)
    {

        foreach (GameObject go in hudElements)
        {

            go.SetActive(!hide);

        }

    }

    private IEnumerator GenerateLevel()
    {

        //print("generating level");

        if(levelCount == 0)
        {

            yield return StartCoroutine(Fade(true));

        }

        playerDead = false;

        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1);
        stageText.color = new Color(stageText.color.r, stageText.color.g, stageText.color.b, 1);
        descriptionText.color = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, 1);

        livesText.text = "X" + lives.ToString();

        if (checkEnemyCount != null) StopCoroutine(checkEnemyCount);

        //yield return new WaitForSeconds(messageDuration);

        float timeElapsed = 0;
        while(timeElapsed < messageDuration)
        {

            timeElapsed += Time.deltaTime;
            if (Input.anyKey)
            {

                timeElapsed = messageDuration;

            }

            yield return null;

        }

        if (levelCount > levelPrefabs.Length - 1 || levelPrefabs.Length == 0) yield break; //completed all levels

        level = Instantiate(levelPrefabs[levelCount], Vector3.zero, Quaternion.identity, transform);

        Fragment();
        surface2d.BuildNavMesh();

        levelCount++;

        checkEnemyCount = StartCoroutine(CheckEnemyCount());

        HideHUD(false);
        Camera.main.backgroundColor = originalBackgroundColour;
        yield return StartCoroutine(Fade(false));
        isTransitioning = false;

    }
    private IEnumerator GameOverScreen()
    {

        Camera.main.backgroundColor = fadeScreen.color;
        HideHUD(true);
        //yield return new WaitForSeconds(messageDuration);
        while(!Input.anyKeyDown)
        {

            yield return null;

        }

        yield return StartCoroutine(Fade(false));
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield break;

    }

    private void Fragment()
    {

        if (level == null) return;
        if (level.transform.childCount > 0)
        {

            List<GameObject> children = new List<GameObject>();

            foreach (Transform tr in level.transform) children.Add(tr.gameObject);

            foreach (GameObject child in children)
            {

                if (child.TryGetComponent<Explodable>(out Explodable sc))
                {

                    sc.extraPoints = Mathf.RoundToInt(5 * child.transform.localScale.x * child.transform.localScale.y);
                    sc.fragmentInEditor();

                }
                if(child.transform.childCount > 0)
                {

                    FragmentChildren(child);

                }

            }

        }

    }

    public static void FragmentChildren(GameObject obj)
    {

        foreach (Transform tr in obj.transform)
        {

            if (tr.gameObject.TryGetComponent<Explodable>(out Explodable sc))
            {

                sc.extraPoints = Mathf.RoundToInt(5 * obj.transform.localScale.x * obj.transform.localScale.y);
                sc.fragmentInEditor();

            }
            if (tr.childCount > 0)
            {

                FragmentChildren(tr.gameObject);

            }

        }

    }

    private void Update()
    {

        if (canProgressToNextLevel)
        {

            if (!playerDead)
            {

                StartCoroutine(GenerateLevel());

            }
            else if (playerDead && lives > 0)
            {

                StartCoroutine(PlayerDeadDoubleMessage());

            }
            else
            {

                StartCoroutine(GameOverScreen());

            }
            canProgressToNextLevel = false;

        }

    }

    private IEnumerator Fade(bool toBlack)
    {

        //if (toBlack) print("going black");
        //else print("unfadeing");

        float dir = 0f;
        if (toBlack) dir = 1f;

        float timeElapsed = 0;

        Color screenInitialColour = fadeScreen.color;
        Color stageInitialColour = stageText.color;
        Color descInitialColour = descriptionText.color;

        Color screenTargetColour = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, dir);
        Color stageTargetColour = new Color(stageText.color.r, stageText.color.g, stageText.color.b, dir);
        Color descTargetColour = new Color(descriptionText.color.r, descriptionText.color.g, descriptionText.color.b, dir);

        if (toBlack)
        {

            while (timeElapsed < fadeDuration)
            {

                fadeScreen.color = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;               

            }
            fadeScreen.color = screenTargetColour;

            timeElapsed = 0;

            while (timeElapsed < fadeDuration)
            {

                stageText.color = Color.Lerp(stageInitialColour, stageTargetColour, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;

            }
            stageText.color = stageTargetColour;

            timeElapsed = 0;

            while (timeElapsed < fadeDuration)
            {

                descriptionText.color = Color.Lerp(descInitialColour, descTargetColour, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;

            }
            descriptionText.color = descTargetColour;


        }
        else
        {

            while (timeElapsed < fadeDuration)
            {

                descriptionText.color = Color.Lerp(descInitialColour, descTargetColour, timeElapsed / fadeDuration);
                stageText.color = Color.Lerp(stageInitialColour, stageTargetColour, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;

            }
            stageText.color = stageTargetColour;
            descriptionText.color = descTargetColour;

            timeElapsed = 0;

            while (timeElapsed < fadeDuration)
            {

                fadeScreen.color = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;

            }
            fadeScreen.color = screenTargetColour;

        }

    }

    private IEnumerator PlayerDeadDoubleMessage()
    {

        yield return StartCoroutine(Fade(true));
        float timeElapsed = 0;
        while (timeElapsed < messageDuration)
        {

            timeElapsed += Time.deltaTime;
            if (Input.anyKey)
            {

                timeElapsed = messageDuration;

            }

            yield return null;

        }
        HideHUD(true);
        Camera.main.backgroundColor = fadeScreen.color;
        yield return StartCoroutine(Fade(false));

        playerDead = false;
        levelCount -= 1;

        SetMessageSettings();
        yield return StartCoroutine(Fade(true));
        StartCoroutine(GenerateLevel());

    }

    private void DestroyPreviousLevel()
    {

        Destroy(level);

        GameObject[] particles = Extensions.FindGameObjectsWithLayer(11);
        if (particles == null || particles.Length == 0) return;

        foreach (GameObject p in particles)
        {

            Destroy(p);

        }

        GameObject[] fragments = Extensions.FindGameObjectsWithLayer(8);
        if (fragments == null || fragments.Length == 0) return;

        foreach (GameObject f in fragments)
        {

            Destroy(f);

        }

    }

    IEnumerator CheckEnemyCount()
    {

        while (true)
        {

            enemyCount = 0;

            List<GameObject> children = new List<GameObject>();
            
            foreach (Transform tr in level.transform) children.Add(tr.gameObject);
            if (children.Count == 0) GenerateLevel();

            foreach (GameObject child in children)
            {

                if (child.TryGetComponent<EnemyController>(out EnemyController ec))
                {

                    enemyCount++; 

                }

            }

            if (enemyCount <= 0 || playerDead)
            {

                isTransitioning = true;
                SetMessageSettings();
                yield return new WaitForSeconds(timeBeforeLevelExit);
                yield return StartCoroutine(Fade(true));
                DestroyPreviousLevel();
                canProgressToNextLevel = true;
                yield break;

            }

            //Debug.Log("Count: " + enemyCount.ToString());

            yield return new WaitForSeconds(0.2f);

        }

    }

    private void SetMessageSettings()
    {

        Color set;

        if (playerDead)
        {

            if (lives > 0)
            {

                set = Color.HSVToRGB(0f, 0.7f, 1f);
                set = new Color(set.r, set.g, set.b, 0f);
                stageText.color = set;
                descriptionText.text = "You have " + lives.ToString() + " lives left.";
                stageText.text = "you died";
                return;

            }
            else
            {

                set = Color.HSVToRGB(0f, 0.7f, 1f);
                set = new Color(set.r, set.g, set.b, 0f);
                stageText.color = set;
                descriptionText.text = "Press any button to restart.";
                stageText.text = "game over";
                return;

            }

        }
        else if (levelCount > levelPrefabs.Length - 1)
        {

            set = Color.white;
            set = new Color(set.r, set.g, set.b, 0f);
            stageText.color = set;
            descriptionText.text = "Stop hacking loser.";
            stageText.text = "you win!";
            return;

        }

        levelPrefabs[levelCount].TryGetComponent<LevelSettings>(out LevelSettings settings);

        Color.RGBToHSV(settings.themeColor, out float h, out float s, out float v); 

        if (s == 0) set = Color.white;
        else set = Color.HSVToRGB(h, 0.7f, 1f);

        set = new Color(set.r, set.g, set.b, 0f);

        stageText.color = set;
        descriptionText.text = settings.description;
        stageText.text = "Stage " + (levelCount + 1).ToString();

    }

    public Vector2 GetLevelDimensions()
    {

        if (level == null) return Vector2.zero;
        level.TryGetComponent<LevelSettings>(out LevelSettings settings);

        if(settings == null)
        {

            return Vector2.zero;

        }

        return new Vector2(
            
                settings.horizontalSize, 
                settings.verticalSize
            
        );

    }

    public void SetPlayerDead()
    {

        playerDead = true;
        lives -= 1;

    }

}
