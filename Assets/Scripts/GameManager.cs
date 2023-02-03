using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    bool _isGameOver;

    [SerializeField]
    bool _isUsingTimeWarp = true;
    [SerializeField]
    TimeWarp _timeWarp;
    [SerializeField]
    bool _isReloadingScene;

   

    [Header("Creating Levels")]
    [SerializeField]
    bool _doIncreaseDifficulty = false;

    public static bool isGamePaused = false;

    [SerializeField]
    bool _canPause = true;
    [SerializeField]
    float _timeTilCanPause = 1.0f;
    [SerializeField]
    bool _isQuitMenuActivated = false;
    [SerializeField]
    GameObject _quitMenu;
    [Header("ESC - Quit Button")]
    public Slider quit_Slider;
    [SerializeField]
    float _sliderSpeed = 1.0f;
    [SerializeField]
    bool _isPressingQuit = false;
    [SerializeField]
    bool _isReleasingQuit = false;
    [SerializeField]
    bool _hasQuitReset = true;
    [SerializeField]
    bool _hasQuitByButton = false;
    [SerializeField]
    bool _didQuit = false;
    [SerializeField]
    /*AudioSource _audioSourceLoop;     //  may not need to assign this if it just loops on Awake()
    [SerializeField]*/
    AudioSource _audioSourceEffects;
    [SerializeField]
    AudioClip _laserClip;
    [SerializeField]
    AudioClip _explosionClip;
    [SerializeField]
    AudioClip _powerupClip;

    [SerializeField]
    GameObject _playerUI_GO;
    [SerializeField]
    GameObject _explosionUI_GO;
    [SerializeField]
    GameObject _thrusterUI_GO;

    [SerializeField]
    RectTransform _rect_PlayerUI;
    [SerializeField]
    Vector2 _startingPos_PlayerUI = new Vector2(0.0f, 23.5f);


    private void Start()
    {
        _isGameOver = false;
        _isReloadingScene = false;
        quit_Slider.minValue = 0.0f;
        quit_Slider.maxValue = 1.0f;
        quit_Slider.wholeNumbers = false;
        quit_Slider.value = 0.0f;
    }

    private void Update()
    {
        if (!isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.R) && _isGameOver && _isReloadingScene == false)
            {
                _isReloadingScene = true;

                if (_isUsingTimeWarp)
                {
                    if (_timeWarp == null)
                        _timeWarp = GameObject.Find("Global Volume").GetComponent<TimeWarp>();
                    if (_timeWarp != null)
                    {
                        StartCoroutine(_timeWarp.TimeWarpRoutine());
                        StartCoroutine(LoadScene_Async());

                        UIManager ui = GameObject.Find("Canvas").GetComponent<UIManager>();
                        ui.RemoveGameOverTexts();
                    }

                    else
                        SceneManager.LoadScene("Game");
                }
                else
                    SceneManager.LoadScene("Game");
            }
        }
        
        if(_didQuit == false)
        {
            if (!isGamePaused && _canPause)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    PauseGame(true);
                

            }
            else if (isGamePaused && _isQuitMenuActivated)
            {

                if (Input.GetKeyDown(KeyCode.Space))
                    StartCoroutine(ReturnToGameRoutine());
                else if(Input.GetKeyDown(KeyCode.Return))
                    GoToMainMenu();
                else if (Input.GetKey(KeyCode.Escape))   //  if holding ESC key
                {
                    sliderVal = quit_Slider.value;
                    if (!quit_Slider.isActiveAndEnabled)
                    {
                        quit_Slider.gameObject.SetActive(true);
                        quit_Slider.enabled = true;
                        _audioSourceEffects.clip = _laserClip;
                        _audioSourceEffects.Play();
                    }
                    sliderVal = quit_Slider.value;
                    _isPressingQuit = true;
                    //testDeltaVal += Time.deltaTime * _sliderSpeed;
                   // testVal += Time.unscaledDeltaTime * _sliderSpeed;
                    sliderVal += Time.unscaledDeltaTime * _sliderSpeed;
                    if(sliderVal > 1.0f)
                        sliderVal = 1.0f;
                    quit_Slider.value = sliderVal;
                    
                    if(quit_Slider.value >= 1.0f)
                    {
                        _didQuit = true;
                        StartCoroutine(QuitRoutine(true));
                    }
                }
                else
                {                                   //  When releasing ESC key
                    
                    _isPressingQuit = false;
                    
                    if (quit_Slider.value > 0.0f)
                    {
                        _isReleasingQuit = true;
                       // testDeltaVal -= Time.deltaTime * _sliderSpeed;
                       // testVal -= Time.unscaledDeltaTime * _sliderSpeed;
                        sliderVal -= Time.unscaledDeltaTime * _sliderSpeed;
                        if (sliderVal <= 0.0f)
                            sliderVal = 0.0f;
                        quit_Slider.value = sliderVal;
                        
                    }

                    if (quit_Slider.value <= 0.0f)
                    {
                        _isReleasingQuit = false;
                        _hasQuitReset = true;
                        if (quit_Slider.isActiveAndEnabled)
                        {
                            
                            quit_Slider.enabled = false;
                            quit_Slider.gameObject.SetActive(false);
                        }
                            
                    }

                    
                }
            }
        }
        
    }
    //public float testDeltaVal = 0.1f;
    //public float testVal = 0.1f;
    public float sliderVal;
    public float timeScale = 1.0f;
    void PauseGame(bool doPause)
    {
        isGamePaused = doPause;
        _canPause = !doPause;
        if (doPause)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1.0f;
        ActivateQuitMenu(doPause);
        

        
    }

    void ActivateQuitMenu(bool doActivate)
    {
        
        _quitMenu.SetActive(doActivate);
        _isQuitMenuActivated = doActivate;
        _rect_PlayerUI.anchoredPosition = _startingPos_PlayerUI;
        _thrusterUI_GO.SetActive(false);
    }
    public void OnQuitButtonPress()     //On-Click Button (Canvas / Quit_Menu / Quit_Button
    {
        _didQuit = true;
        StartCoroutine(QuitRoutine(false));
    }

    public void OnSpaceButtonPress()    //On-Click Button (Canvas / Quit_Menu / BackToSpace_Button
    {
        StartCoroutine(ReturnToGameRoutine());
    }
    IEnumerator ReturnToGameRoutine()
    {
        yield return null;
        _thrusterUI_GO.SetActive(true);
        _audioSourceEffects.clip = _powerupClip;
        _audioSourceEffects.Play();
        Vector2 targetAnchPos = new Vector2(0.0f, 350.0f);
        float elapsedTime = 0.0f;
        float targetTime = 0.5f;
        while (elapsedTime < targetTime)
        {
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
            float y = Mathf.Lerp(_startingPos_PlayerUI.y, targetAnchPos.y, elapsedTime / targetTime);
            Vector2 newVector2 = new Vector2(0.0f, y);
            _rect_PlayerUI.anchoredPosition = newVector2;
        }

        PauseGame(false);
    }

    public void OnMainMenuButtonPress()    //On-Click Button (Canvas / Quit_Menu / MainMenu_Button
    {
        GoToMainMenu();
    }
    void GoToMainMenu()
    {
        isGamePaused = false;
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    int countQuits = 0;
    IEnumerator QuitRoutine(bool _didManuallySlide)
    {
        
        if(countQuits < 1)
        {
            
            countQuits++;
            // _didManuallySlide == true, skip the animation to make slider go up
            //  otherwise, that means they press the Yes, Quit button
            //      so animate the laser going across to destroy the ship
            if (_didManuallySlide == false && _isQuitMenuActivated)
            {
                
                if (!quit_Slider.isActiveAndEnabled)
                {
                    quit_Slider.gameObject.SetActive(true);
                    quit_Slider.enabled = true;
                    _audioSourceEffects.clip = _laserClip;
                    _audioSourceEffects.Play();
                }
                var sliderVal = quit_Slider.value;
                _isPressingQuit = true;

                while(sliderVal < 1.0f)
                {
                    sliderVal += Time.unscaledDeltaTime * _sliderSpeed;
                    if (sliderVal > 1.0f)
                        sliderVal = 1.0f;
                    quit_Slider.value = sliderVal;
                    yield return null;
                }
                if (quit_Slider.value >= 1.0f)
                    _didQuit = true;
            }

            //  Now that Sliding is done for both, trigger explosionUI
            _explosionUI_GO.SetActive(true);
            _audioSourceEffects.clip = _explosionClip;
            _audioSourceEffects.Play();

            Animator anim = _explosionUI_GO.GetComponent<Animator>();
            
            anim.enabled = true;
            anim.Play("ExplosionUI");
            yield return new WaitForSecondsRealtime(0.5f);
            Image playerImg = _playerUI_GO.GetComponent<Image>();
            playerImg.enabled = false;
            //  Now quit application after 1 second.
            yield return new WaitForSecondsRealtime(0.5f);
            
            Quit();
        }
        

    }
    private void Quit()
    {
        Application.Quit();
    }


    public void GameOver()
    {
        _isGameOver = true;
    }

    IEnumerator LoadScene_Async()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.isDone == false)
        {
            if (_timeWarp.DidTimeWarpEnd() == true)
                asyncLoad.allowSceneActivation = true;
            
            yield return null;
        }
    }
}
