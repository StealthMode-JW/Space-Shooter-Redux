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

    [SerializeField] List<Text> _ListWaveNum_Texts = new List<Text>();  //For all 3 Texts that make up 1 graphic
    [SerializeField] CanvasGroup _cg_WaveText;
    [SerializeField] CanvasGroup _cg_WaveNumText;
    [SerializeField] List<Text> _ListRankNum_Texts = new List<Text>();  //For all 3 Texts that make up 1 graphic
    [SerializeField] CanvasGroup _cg_RankText;

    [SerializeField] Slider _enemyThreat_Slider;
    [SerializeField] CanvasGroup _cg_EnemyThreatSlider;
    [SerializeField] Image _enemyThreat_SliderFill;
    [SerializeField] float _maxSliderVal = 50f;
    [SerializeField] Color _col_minThreat = Color.magenta;
    [SerializeField] Color _col_MaxThreat = Color.red;

    [SerializeField] ParticleSystem _sparkEffect_NewRank;
    [SerializeField] Vector3 _startPos_sparkNewRank = new Vector3(3, 1.4f, -6.5f);
    [SerializeField] ParticleSystem _sparkEffect_ThreatMeter;
    //[SerializeField] Vector3 _startPos_sparkThreat = new Vector3(3, 1.2f, -6.45f);
    //[SerializeField] Vector3 _endPos_sparkThreat = new Vector3(3, -1.4f, -6.45f);
    [SerializeField] Vector3 _startPos_sparkThreat = new Vector3(7.6f, 3, -1);
    [SerializeField] Vector3 _endPos_sparkThreat = new Vector3(7.6f, -3, -1);

    
    [SerializeField] float _minPosY_sparkThreat = 3.00f;
    [SerializeField] float _maxPosY_sparkThreat = -3.00f;

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

        _enemyThreat_Slider.value = 0;
        _cg_EnemyThreatSlider.alpha = 0;
        _enemyThreat_SliderFill.color = _col_minThreat;
    }


    


    public void UpdateScoreText(int newScore)
    {
        _scoreText.text = "Score: " + newScore;
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
            currentLives = 0;
        if(currentLives > _livesSprite.Length)
            Debug.LogWarning("CURRENT LIVES >> " + currentLives + " << is showing a higher value than LIVES SPRITE LENGTH >> " + _livesSprite.Length + " <<");
       
        _livesImg.sprite = _livesSprite[currentLives];

        if (currentLives <= 0)
            GameOverSequence();
        
        
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
    
    //From SpawnManager...
    public IEnumerator DisplayWaveNumberText(Wave wave)
    {
        int waveNum = wave.waveNum;
        int numEnemies = wave.wave_ENEMIES.numEnemies;
        
        yield return null;
        _cg_WaveText.alpha = 0;
        _cg_WaveNumText.alpha = 0;

        yield return StartCoroutine(UpdateRank(waveNum));

        float delay = 0.25f;
        float timeToFade = 1.0f;

        yield return StartCoroutine(FadeCanvasInOut(_cg_WaveText, true, delay, timeToFade));
        yield return StartCoroutine(FadeCanvasInOut(_cg_WaveText, false, delay, timeToFade));

        for (int i = 0; i < _ListWaveNum_Texts.Count; i++)
        {
            var text = _ListWaveNum_Texts[i];
            text.text = waveNum.ToString();
        }
        yield return StartCoroutine(FadeCanvasInOut(_cg_WaveNumText, true, delay, timeToFade));
        yield return StartCoroutine(FadeCanvasInOut(_cg_WaveNumText, false, delay, timeToFade));

        remainingEnemies = numEnemies;
        if (_adjustEnemyThreatSliderRoutine != null)
            StopCoroutine(_adjustEnemyThreatSliderRoutine);
        AdjustSliderValues();
        StartCoroutine(AdjustEnemyThreat_SliderValueRoutine(numEnemies));
        /*float fraction = (float) numEnemies / _maxSliderVal; // (max = 50 Enemies)
        _sliderFill.color = Color.Lerp(_sliderFill.color, _col_MaxThreat, fraction);*/
    }
    int remainingEnemies;
    //!!!!!
    float endValue;
    float endAlpha;
    Color endColor;


    bool isAdjustingSlider;
    Coroutine _adjustEnemyThreatSliderRoutine;
    public int AdjustEnemyThreat()
    {
        remainingEnemies--;
        if (remainingEnemies < 0)
            remainingEnemies = 0;
        
        AdjustSliderValues();
        if (isAdjustingSlider == false || _adjustEnemyThreatSliderRoutine == null)
            _adjustEnemyThreatSliderRoutine = 
                StartCoroutine(AdjustEnemyThreat_SliderValueRoutine(remainingEnemies));

        return remainingEnemies;
    }
    void AdjustSliderValues()
    {
        endValue = remainingEnemies / _maxSliderVal;
        float endColorFraction = endValue / _enemyThreat_Slider.maxValue;
        endColor = Color.Lerp(_col_minThreat, _col_MaxThreat, endColorFraction);
        endAlpha = (endValue > 0) ? 0.5f + 0.5f * endValue : 0f;
    }

    IEnumerator AdjustEnemyThreat_SliderValueRoutine(int numEnemies)
    {
        isAdjustingSlider = true;

        float duration = 2.0f;
        float elapsedTime = 0f;
        float startValue = _enemyThreat_Slider.value;
        
        endValue = numEnemies / _maxSliderVal; //   max 50 enemies
        
        float startColorFraction = 
            startValue / _enemyThreat_Slider.maxValue;
        Color startColor = 
            Color.Lerp(_col_minThreat, _col_MaxThreat, startColorFraction);
        float startAlpha = 
            (_enemyThreat_Slider.value > 0) ? 0.5f + 0.5f * _enemyThreat_Slider.value : 0f;

        // Define start and end Y positions for the spark effect
        float minPosY = 3.0f; // Y position at 0 enemies
        float maxPosY = -3.0f; // Y position at maximum enemies
        float totalDistance = maxPosY - minPosY; // Total distance the spark can travel

        // Calculate the start and end Y positions based on the number of enemies
        float startPosY = minPosY + totalDistance * startValue;
        float endPosY = minPosY + totalDistance * endValue;

        _sparkEffect_ThreatMeter.Play();

        //  Over the duration of 2 secs...
        while (elapsedTime < duration)
        {
            //  Adjust Slider value from less to more (as threat increases)
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            _enemyThreat_Slider.value = newValue;

            //  Adjust CanvasGroup alpha from transparent to opaque (as threat increases) 
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            _cg_EnemyThreatSlider.alpha = newAlpha;

            //  Adjust Slider Fill color from magenta to red (as threat increases)
            Color newColor = Color.Lerp(startColor, endColor, elapsedTime / duration);
            _enemyThreat_SliderFill.color = newColor;

            // Interpolate the Y position of the spark effect
            float newPosY = Mathf.Lerp(startPosY, endPosY, elapsedTime / duration);
            _sparkEffect_ThreatMeter.transform.position = 
                new Vector3(_sparkEffect_ThreatMeter.transform.position.x, newPosY, 
                _sparkEffect_ThreatMeter.transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _enemyThreat_Slider.value = endValue;
        _cg_EnemyThreatSlider.alpha = endAlpha;
        _enemyThreat_SliderFill.color = endColor;

        if(_enemyThreat_Slider.value <= 0)
            if (_sparkEffect_NewRank != null)
                _sparkEffect_NewRank.Play();

        // Apply final Y position to the spark effect
        _sparkEffect_ThreatMeter.transform.position = 
            new Vector3(_sparkEffect_ThreatMeter.transform.position.x, endPosY, 
            _sparkEffect_ThreatMeter.transform.position.z);
        _sparkEffect_ThreatMeter.Stop();

        isAdjustingSlider = false;
    }

    public IEnumerator UpdateRank(int waveNumber)
    {
        if(waveNumber > 1)
        {
            float delay = 0.25f;
            float timeToFade = 1.5f;

            //  Fade Out
            if (waveNumber > 2)
                yield return StartCoroutine(FadeCanvasInOut(_cg_RankText, false, delay, timeToFade));

            //  Adjust Rank
            string str_Rank = IntToRoman(waveNumber - 1);
            for (int i = 0; i < _ListRankNum_Texts.Count; i++)
            {
                var text = _ListRankNum_Texts[i];
                text.text = str_Rank;
            }

            //  Fade In
            yield return StartCoroutine(FadeCanvasInOut(_cg_RankText, true, delay, timeToFade));

            //yield return new WaitForSeconds(delay * 2);
        }
    }

    bool addSpaceToRank = true;

    public string IntToRoman(int num)
    {
        string[] Thousands = { "", "M", "MM", "MMM" };
        string[] Hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        string[] Tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        string[] Ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

        string romanNumeral = Thousands[num / 1000] +
                              Hundreds[(num % 1000) / 100] +
                              Tens[(num % 100) / 10] +
                              Ones[num % 10];

        if (addSpaceToRank && romanNumeral.Length > 1) 
            romanNumeral = string.Join(" ", romanNumeral.ToCharArray());
            
        return romanNumeral;
    }

    IEnumerator FadeCanvasInOut(CanvasGroup cg, bool doFadeIn, float delay, float timeToFade)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0;
        float startAlpha = doFadeIn ? 0 : 1;
        float endAlpha = doFadeIn ? 1 : 0;
        float alpha = startAlpha;

        while (timer < timeToFade)
        {
            alpha = Mathf.Lerp(startAlpha, endAlpha, timer / timeToFade);
            cg.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensuring the alpha is set to the final value after the loop finishes
        cg.alpha = endAlpha;
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
