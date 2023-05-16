using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    [SerializeField]
    TMP_Text _scoreText;
   
    [SerializeField]
    Image _livesImg;
    [SerializeField]
    Sprite[] _livesSprite;

    [Space(25)]

    [SerializeField]
    Text _gameOverText;
    [SerializeField]
    bool _isUsingRearrangeGameOverText = false;
  



    [SerializeField]
    float _timeToFadeText = 0.75f;
    [SerializeField]
    float _timeToPauseFade = 0.25f;
    [SerializeField]
    int _numTimesToFlickerText = 10;

    [Space(10)]
    [SerializeField]
    GameObject _textPanelGO;
    [SerializeField]
    Text _asteroidCluster_text;
    [SerializeField]
    Text _asteroidShower_text;

    [Space(10)]

    [SerializeField]
    TMP_Text _restartText;
    Color _restartCol;

    GameManager _gameManager;
    SpawnManager _spawnManager;

    [SerializeField]
    GameObject _dummyStartScreenGO;

    [SerializeField]
    List<string> List_Rearrange_GameOver = new List<string>();

    void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        UpdateLives(3);
        _restartCol = _restartText.color;
        _restartCol.a = 0.0f;
        _restartText.color = _restartCol;
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.Log("UManager: Start() GameManager == null");
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("UManager: Start() SpawnManager == null");
        TriggerDummyStartScreen(false);
    }
    public void UpdateScoreText(int newScore)
    {
        _scoreText.text = "Score: " + newScore;
    }

    public void UpdateLives(int currentLives)
    {
        if(currentLives >= 0)
        {
            _livesImg.sprite = _livesSprite[currentLives];

            if (currentLives <= 0)
                GameOverSequence();
        }
        
    }

    public void FlashAsteroidWarning(SpawnManager.AsteroidState asteroidState)
    {
        _textPanelGO.gameObject.SetActive(true);
        Text textToSend = _asteroidCluster_text;
        if (asteroidState == SpawnManager.AsteroidState.Asteroid_Cluster)
        {
            textToSend = _asteroidCluster_text;
            _asteroidCluster_text.enabled = true;
            _asteroidShower_text.enabled = false;
        }
            
        else if (asteroidState == SpawnManager.AsteroidState.Asteroid_Shower)
        {
            textToSend = _asteroidShower_text;
            _asteroidCluster_text.enabled = false;
            _asteroidShower_text.enabled = true;
        }
            
        else
            Debug.LogWarning("UIManager: FlashAsteroidWarning() tried to send a text to Flicker but asteroidState was invalid >> " + asteroidState.ToString());

        StartCoroutine(FlickerText(textToSend, false, 1.0f, .5f, 100));
        StartCoroutine(TurnOffWarningTextPanel(asteroidState));

    }
    IEnumerator TurnOffWarningTextPanel(SpawnManager.AsteroidState asteroidState)
    {
        
        yield return null;
        if(_spawnManager != null)
        {
            bool isOkayToRemove = false;
            yield return new WaitForSeconds(2.0f);

            while(isOkayToRemove == false)
            {
                yield return null;
                SpawnManager.AsteroidState state = _spawnManager.GetAsteroidState();
                if(state != asteroidState)
                    isOkayToRemove = true;
            }
            _asteroidCluster_text.enabled = false;
            _asteroidShower_text.enabled = false;
            _textPanelGO.SetActive(false);
        }
    }
    
    void GameOverSequence()
    {
        if (_gameOverText != null)
            _gameOverText.gameObject.SetActive(true);

        StartCoroutine(FlickerText(_gameOverText, true, _timeToFadeText, _timeToPauseFade, _numTimesToFlickerText));


        if (_gameManager != null)
            _gameManager.GameOver();


        StartCoroutine(DisplayRestartText());
    }
    
    IEnumerator FlickerText(Text textToFade, bool doStartActive, float timeToFade, float timeToPause, int numTimesToFlicker)
    {
        if(textToFade != null)
        {
            if (doStartActive)  //  Start with Text ON and fade OUT first
            {
                WaitForSeconds waitForSeconds = new WaitForSeconds(timeToPause);
                float elapsedTime;
                Color textCol = textToFade.color;

                for (int i = 0; i < numTimesToFlicker; i++)
                {
                    yield return waitForSeconds;
                    yield return waitForSeconds;

                    textCol.a = 1.0f;
                    textToFade.color = textCol;
                    elapsedTime = 0.0f;

                    while (elapsedTime < timeToFade)
                    {
                        yield return null;
                        elapsedTime += Time.deltaTime;
                        textCol.a = 1 - Mathf.Clamp01(elapsedTime / timeToFade);
                        textToFade.color = textCol;
                    }

                    yield return waitForSeconds;
                    elapsedTime = 0.0f;

                    while (elapsedTime < timeToFade)
                    {
                        yield return null;
                        elapsedTime += Time.deltaTime;
                        textCol.a = Mathf.Clamp01(elapsedTime / timeToFade);
                        textToFade.color = textCol;
                    }
                }
            }
            else // Start with text OFF and fade IN first
            {
                WaitForSeconds waitForSeconds = new WaitForSeconds(timeToPause);
                float elapsedTime;
                Color textCol = textToFade.color;

                for (int i = 0; i < numTimesToFlicker; i++)
                {
                    yield return waitForSeconds;
                    yield return waitForSeconds;

                    textCol.a = 0.0f;
                    textToFade.color = textCol;
                    elapsedTime = 0.0f;

                    while (elapsedTime < timeToFade)
                    {
                        yield return null;
                        elapsedTime += Time.deltaTime;
                        textCol.a = Mathf.Clamp01(elapsedTime / timeToFade);
                        textToFade.color = textCol;
                    }

                    yield return waitForSeconds;
                    elapsedTime = 0.0f;

                    while (elapsedTime < timeToFade)
                    {
                        yield return null;
                        elapsedTime += Time.deltaTime;
                        textCol.a = 1 - Mathf.Clamp01(elapsedTime / timeToFade);
                        textToFade.color = textCol;
                    }
                }
            }
            
        }
    }

    

    public void SwapLivesSprite(int spriteIndex)
    {
        _livesImg.sprite = _livesSprite[spriteIndex];
    }

    /*public IEnumerator MoveShipUI_Routine(RectTransform rt_ShipUI, CanvasGroup cg_ShipUI, int spriteIndex, Vector2 startPos, Vector2 endPos, float timeToMove)
    {
        
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;

            // Fade in the CanvasGroup
            if (t < 0.5f)
            {
                cg_ShipUI.alpha = t * 2; // This will reach 1 when t is 0.5
            }

            // Use Lerp to smoothly transition from the start position to the end position
            rt_ShipUI.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }
        _livesImg.sprite = _livesSprite[spriteIndex];
        yield return new WaitForSeconds(0.1f);
        Reset_ShipUIs(rt_ShipUI, cg_ShipUI, startPos);
    }

    public void Reset_ShipUIs(RectTransform rt_ShipUI, CanvasGroup cg_ShipUI, Vector2 startPos)
    {
        rt_ShipUI.anchoredPosition = startPos;
        cg_ShipUI.alpha = 0f;
    }
*/

    IEnumerator DisplayRestartText()
    {
        yield return new WaitForSeconds(2.5f);
        if (_restartText != null)
        {
            _restartText.gameObject.SetActive(true);
            Color textCol = _restartText.color;
            textCol.a = 0.0f;
            float elapsedTime = 0.0f;
            float timeToFadeIn = 3.0f;
            while (elapsedTime < timeToFadeIn)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
                textCol.a = Mathf.Clamp01(elapsedTime / timeToFadeIn);
                _restartText.color = textCol;
            }
            textCol.a = 1.0f;
            _restartText.color = textCol;
        }
            
    }

    public void RemoveGameOverTexts()
    {
        StartCoroutine(RemoveRestartTextRoutine());
        if (_isUsingRearrangeGameOverText)
            StartCoroutine(RearrangeGameOverTextRoutine());
    }

    IEnumerator RemoveRestartTextRoutine()
    {
        if (_restartText != null)
        {

            Color textCol = _restartText.color;
            textCol.a = 1.0f;
            float elapsedTime = 0.0f;
            float timeToFadeOut = 1.0f;
            while (elapsedTime < timeToFadeOut)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
                textCol.a = 1 - Mathf.Clamp01(elapsedTime / timeToFadeOut);
                _restartText.color = textCol;
            }
            textCol.a = 0.0f;
            _restartText.color = textCol;

            _restartText.gameObject.SetActive(false);
        }
    }

    IEnumerator RearrangeGameOverTextRoutine()
    {
        
        
        //Wait for GameOvertitle to fade out for last time.
        while (_gameOverText.color.a > 0.2)
            yield return null;

        StopCoroutine("FlickerText");
        StartCoroutine(FlickerText(_gameOverText, false, 0.25f, 0.25f, 100));
        Text bgText = _gameOverText.transform.GetChild(0).GetComponent<Text>();
        
        /*if(bgText != null)
            bgText.gameObject.SetActive(false);*/

        while(_gameOverText != null && bgText != null)
        {
            if(List_Rearrange_GameOver != null && List_Rearrange_GameOver.Count > 0)
            {
                int ranIndex = Random.Range(0, List_Rearrange_GameOver.Count);
                string newWord = List_Rearrange_GameOver[ranIndex];
                _gameOverText.text = newWord.ToUpper();
                bgText.text = newWord.ToUpper();
                yield return new WaitForSeconds(0.75f);
            }
            
        }

    }

    public void TriggerDummyStartScreen(bool onTrue_offFalse)
    {
        if(_dummyStartScreenGO != null)
        {
            if (onTrue_offFalse)
                _dummyStartScreenGO.SetActive(true);
            else
                _dummyStartScreenGO.SetActive(false);
        }
    }
}
