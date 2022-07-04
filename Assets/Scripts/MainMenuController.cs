using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    private float menuGlideTime = 0.5f;

    [SerializeField]
    private float changeSceneFadeTime = 1.5f;

    [SerializeField]
    private RectTransform mainSection;

    [SerializeField]
    private RectTransform playSection;

    [SerializeField]
    private RectTransform settingsSection;

    [SerializeField]
    private RectTransform screenBounds;

    [SerializeField]
    private RawImage fadeScreen;

    [SerializeField]
    private TextMeshProUGUI gameTypeDescription;

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;

    //[SerializeField]
    //private string campaignDescription;

    //[SerializeField]
    //private string dungeonDescription;

    //[SerializeField]
    //private string endlessDescription;

    private MenuMode mode;
    private Resolution[] resolutions;

    private enum MenuMode
    {

        MainMenu,
        PlayMenu,
        SettingsMenu,
        Quit,
        Transitioning

    }

    private void Start()
    {
        
        mode = MenuMode.Transitioning;
        mainSection.gameObject.SetActive(true);
        playSection.gameObject.SetActive(false);
        settingsSection.gameObject.SetActive(false);
        fadeScreen.gameObject.SetActive(true);

        gameTypeDescription.text = "";

        StartCoroutine(FadeFromScene(1.5f));
        //organise UI parts

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string> { };

        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {

            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {

                currentResolutionIndex = i;

            }

        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }

    public void PlayButton()
    {

        if (mode.Equals(MenuMode.MainMenu))
        {

            mode = MenuMode.PlayMenu;

            float newX = (screenBounds.rect.width)/2 + ((mainSection.rect.width) / 2);
            LeanTween.moveLocalX(mainSection.gameObject, -newX, menuGlideTime);

            playSection.localPosition = new Vector2(newX, playSection.localPosition.y);
            playSection.gameObject.SetActive(true);

            mainSection.gameObject.SetActive(false);    

            LeanTween.moveLocalX(playSection.gameObject, 0, menuGlideTime);

        }

    }

    public void SettingsButton()
    {

        if (mode.Equals(MenuMode.MainMenu))
        {

            mode = MenuMode.SettingsMenu;

            float newX = (screenBounds.rect.width) / 2 + ((mainSection.rect.width) / 2);
            LeanTween.moveLocalX(mainSection.gameObject, newX, menuGlideTime);

            settingsSection.localPosition = new Vector2(-newX, playSection.localPosition.y);
            settingsSection.gameObject.SetActive(true);

            mainSection.gameObject.SetActive(false);

            LeanTween.moveLocalX(settingsSection.gameObject, 0, menuGlideTime);

        }

    }

    public void BackButton_PlaySection()
    {

        if (mode.Equals(MenuMode.PlayMenu))
        {

            mode = MenuMode.MainMenu;

            float newX = (screenBounds.rect.width) / 2 + ((playSection.rect.width) / 2);
            LeanTween.moveLocalX(playSection.gameObject, newX, menuGlideTime);

            mainSection.localPosition = new Vector2(-newX, playSection.localPosition.y);
            mainSection.gameObject.SetActive(true);

            playSection.gameObject.SetActive(false);

            LeanTween.moveLocalX(mainSection.gameObject, 0, menuGlideTime);

        }

    }

    public void BackButton_SettingsSection()
    {

        if (mode.Equals(MenuMode.SettingsMenu))
        {

            mode = MenuMode.MainMenu;

            float newX = (screenBounds.rect.width) / 2 + ((playSection.rect.width) / 2);
            LeanTween.moveLocalX(settingsSection.gameObject, -newX, menuGlideTime);

            mainSection.localPosition = new Vector2(newX, playSection.localPosition.y);
            mainSection.gameObject.SetActive(true);

            settingsSection.gameObject.SetActive(false);

            LeanTween.moveLocalX(mainSection.gameObject, 0, menuGlideTime);

        }

    }

    public void CampaignButton()
    {

        if (mode.Equals(MenuMode.PlayMenu))
        {

            mode = MenuMode.Transitioning;
            LeanTween.color(fadeScreen.gameObject, new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f), changeSceneFadeTime);
            StartCoroutine(FadeToScene(changeSceneFadeTime, 1));
            
        }

    }

    public void CampaignButtonHover()
    {

        if (mode.Equals(MenuMode.PlayMenu))
        {

            gameTypeDescription.text = "play through many small stages.\n\nyou have 3 lives and you restart once you die.\n\nsee how far you can get!";

        }

    }

    public void DungeonButtonHover()
    {


        if (mode.Equals(MenuMode.PlayMenu))
        {

            gameTypeDescription.text = "coming soon!";

        }

    }

    public void EndlessButtonHover()
    {

        if (mode.Equals(MenuMode.PlayMenu))
        {

            gameTypeDescription.text = "coming soon!";

        }

    }

    public void GameButtonUnhover()
    {

        gameTypeDescription.text = "";

    }

    public void QuitButton()
    {

        if (mode.Equals(MenuMode.MainMenu))
        {

            mode = MenuMode.Quit;
            Application.Quit();

        }

    }

    public void SetVolume(float setVolume)
    {

        //Debug.Log(setVolume);

    }

    public void FullscreenToggle(bool fullscreen)
    {

        Screen.fullScreen = fullscreen;

    }

    public void ResolutionDropdown(int option)
    {

        Screen.SetResolution(resolutions[option].width, resolutions[option].height, Screen.fullScreen);

    }

    private IEnumerator FadeToScene(float time, int scene)
    {

        float timeElapsed = 0;
        Color startingColour = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
        Color targetColour = new Color(startingColour.r, startingColour.g, startingColour.b, 1f);

        fadeScreen.gameObject.SetActive(true);

        while(timeElapsed < time)
        {

            fadeScreen.color = Color.Lerp(startingColour, targetColour, timeElapsed/time);
            timeElapsed += Time.deltaTime;
            yield return null;

        }

        SceneManager.LoadSceneAsync(scene);

    }
    private IEnumerator FadeFromScene(float time)
    {

        float timeElapsed = 0;
        Color startingColour = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
        Color targetColour = new Color(startingColour.r, startingColour.g, startingColour.b, 0f);

        fadeScreen.gameObject.SetActive(true);

        while (timeElapsed < time)
        {

            fadeScreen.color = Color.Lerp(startingColour, targetColour, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            yield return null;

        }

        fadeScreen.gameObject.SetActive(false);
        mode = MenuMode.MainMenu;

    }

}
