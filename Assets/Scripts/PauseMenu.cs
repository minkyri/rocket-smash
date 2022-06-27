using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PostProcessVolume ppv;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private RawImage restartFadeScreen;
    [SerializeField] private float restartFadeDuration = 0.4f;
    [SerializeField] private float pauseFadeDuration = 0.4f;
    private DepthOfField dof;
    private bool pauseLock = false;

    private void Start()
    {

        ppv.profile.TryGetSettings(out dof);
        //Resume();

    }

    private void Update()
    {

        if (pauseLock) return;

        if(!GameController.instance.levelController.isTransitioning && !GameController.instance.levelController.playerDead)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {

                if (GameIsPaused)
                {

                    Resume();

                }
                else
                {

                    Pause();

                }

            }

        } 

    }

    public void Resume()
    {

        if (pauseLock) return;
        pauseMenuUI.SetActive(false);
        StartCoroutine(FadePauseMenu(true));

    }

    void Pause()
    {

        if (pauseLock) return;
        pauseMenuUI.SetActive(true);
        StartCoroutine(FadePauseMenu(false));
        
    }

    IEnumerator FadePauseMenu(bool fade)
    {

        pauseLock = true;
        float timeElapsed = 0;

        Color buttonInitialColour = buttons[0].colors.normalColor;
        Color buttonTargetColour;

        Color textInitialColour = texts[0].color;
        Color textTargetColour;

        float timeScaleInitial;
        float timeScaleTarget;

        float initialAperture;
        float targetAperture;

        dof.active = true;

        if (fade)
        {

            timeScaleInitial = 0f;
            timeScaleTarget = 1f;

            initialAperture = 0.8f;
            targetAperture = 3f;

            buttonInitialColour = new Color(buttonInitialColour.r, buttonInitialColour.g, buttonInitialColour.b, 28f/100f);
            buttonTargetColour = new Color(buttonInitialColour.r, buttonInitialColour.g, buttonInitialColour.b, 0);

            textInitialColour = new Color(textInitialColour.r, textInitialColour.g, textInitialColour.b, 1);
            textTargetColour = new Color(textInitialColour.r, textInitialColour.g, textInitialColour.b, 0);

        }
        else
        {

            timeScaleInitial = 1f;
            timeScaleTarget = 0f;

            initialAperture = 3f;
            targetAperture = 0.8f;

            buttonInitialColour = new Color(buttonInitialColour.r, buttonInitialColour.g, buttonInitialColour.b, 0);
            buttonTargetColour = new Color(buttonInitialColour.r, buttonInitialColour.g, buttonInitialColour.b, 28f/100f);

            textInitialColour = new Color(textInitialColour.r, textInitialColour.g, textInitialColour.b, 0);
            textTargetColour = new Color(textInitialColour.r, textInitialColour.g, textInitialColour.b, 1);

        }

        while (timeElapsed < pauseFadeDuration)
        {

            foreach (Button btn in buttons)
            {

                ColorBlock color = btn.colors;
                color.normalColor = Color.Lerp(buttonInitialColour, buttonTargetColour, timeElapsed / pauseFadeDuration);

            }
            foreach (TextMeshProUGUI txt in texts)
            {

                txt.color = Color.Lerp(textInitialColour, textTargetColour, timeElapsed / pauseFadeDuration);

            }
            dof.aperture.value = Mathf.Lerp(initialAperture, targetAperture, timeElapsed / pauseFadeDuration);
            Time.timeScale = Mathf.Lerp(timeScaleInitial, timeScaleTarget, timeElapsed / pauseFadeDuration);

            timeElapsed += Time.unscaledDeltaTime;
            pauseFadeDuration = pauseFadeDuration * (0.9999f);
            yield return null;

        }

        Time.timeScale = timeScaleTarget;
        dof.aperture.value = targetAperture;
        foreach (Button btn in buttons)
        {

            ColorBlock color = btn.colors;
            color.normalColor = buttonTargetColour;

        }
        foreach (TextMeshProUGUI txt in texts)
        {

            txt.color = textTargetColour;

        }

        if (fade)
        {

            GameIsPaused = false;
            dof.active = false;

        }
        else
        {

            GameIsPaused = true;
            dof.active = true;

        }
        
        pauseLock = false;

    }

    public void Quit()
    {

        if (pauseLock) return;
        Application.Quit();

    }

    public IEnumerator IERestart()
    {

        restartFadeScreen.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(true));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void Restart()
    {

        if (pauseLock) return;
        StartCoroutine(IERestart());

    }

    private IEnumerator Fade(bool toBlack)
    {

        //if (toBlack) print("going black");
        //else print("unfadeing");

        float dir = 0f;
        if (toBlack) dir = 1f;

        float timeElapsed = 0;

        Color screenInitialColour = restartFadeScreen.color;

        Color screenTargetColour = new Color(restartFadeScreen.color.r, restartFadeScreen.color.g, restartFadeScreen.color.b, dir);

        if (toBlack)
        {

            while (timeElapsed < restartFadeDuration)
            {

                restartFadeScreen.color = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / restartFadeDuration);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;

            }
            restartFadeScreen.color = screenTargetColour;

        }
        else
        {

            while (timeElapsed < restartFadeDuration)
            {

                restartFadeScreen.color = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / restartFadeDuration);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;

            }
            restartFadeScreen.color = screenTargetColour;

        }

    }

}
