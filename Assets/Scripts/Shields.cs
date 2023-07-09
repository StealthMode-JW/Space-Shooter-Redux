using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Shields : MonoBehaviour
{
    [Header("SHIELDS")]
    [SerializeField] bool _isShieldEnabled = false;
    [SerializeField] ShieldStrength _shieldStrength;
    [SerializeField] GameObject _shieldObj;
    [SerializeField] SpriteRenderer _shieldSprite;
    [SerializeField] ParticleSystem _shieldExplosionEffect;

    [SerializeField] int _shieldLife = 0;

    [SerializeField] bool _isInvulnerable;
    [SerializeField] float _invulnerabilityTime = 1.5f;

    [SerializeField] Light2D _shieldsLight;
    [SerializeField] Color _fullShieldsCol = Color.blue;
    [SerializeField] Color _halfShieldsCol = Color.yellow;
    [SerializeField] Color _criticalShieldsCol = Color.red;
    private Color _targetColor;

    [SerializeField] float _steadyPulseSpeed = 5.0f;
    [SerializeField] float _transitionPulseSpeed = 30f;
    [SerializeField] float _colorTransitionSpeed = 3.0f;
    [SerializeField] float _minPulseIntensity = 0f;
    [SerializeField] float _maxPulseIntensity = 0.75f;

    [SerializeField] AudioSource _audioSource_Shields;  //  Starts later in Clip (plays sound earlier)
    [SerializeField] float _audioShields_StartTime = 0.5f;
    [SerializeField] AudioClip _shieldDamagedSound;

    public enum ShieldStrength
    {
        None,
        Critical,
        Half,
        Full
    };

    private void Start()
    {
        if (_shieldObj == null)
            _shieldObj = this.gameObject;
        DisableShields();
        if (_audioSource_Shields != null)
            _audioSource_Shields.time = _audioShields_StartTime;
    }
    

    private void Update()
    {
        UpdateShieldVisuals();
    }

    void DisableShields()
    {
        _isInvulnerable = false;
        _isShieldEnabled = false;
        _shieldLife = 0;
        _shieldObj.SetActive(false);

    }

    public IEnumerator ShieldHit(int dmgAmount)
    {
        _isInvulnerable = true;
        _shieldLife -= dmgAmount;
        if (_shieldLife < 0)
            _shieldLife = 0;

        var shieldStrength = GetShieldStrength_ByInt(_shieldLife);
        AdjustShieldStrength(shieldStrength);

        for (float t = 0; t < _invulnerabilityTime; t += Time.deltaTime)
        {
            // Light2D intensity pulse when invulnerable
            float pulse = Mathf.Sin(t * _transitionPulseSpeed) * 0.5f + 0.5f;
            _shieldsLight.intensity =
                Mathf.Lerp(_minPulseIntensity, _maxPulseIntensity, pulse);
            yield return null;
        }
        

        _audioSource_Shields.clip = _shieldDamagedSound;
        _audioSource_Shields.time = _audioShields_StartTime;
        _audioSource_Shields.Play();

        _isInvulnerable = false;
    }


    public void AdjustShieldStrength(ShieldStrength newShieldStrength)
    {
        _shieldStrength = newShieldStrength;
        StartCoroutine(PlayEnergyExplosion());

        if (_shieldStrength == ShieldStrength.None)
        {
            _isShieldEnabled = false;
            _shieldLife = 0;
            DisableShields();
        }
        else
        {
            _isShieldEnabled = true;

            if (_shieldStrength == ShieldStrength.Critical)
            {
                _shieldLife = 1;
                _shieldsLight.color = _criticalShieldsCol;
            }
            else if (_shieldStrength == ShieldStrength.Half)
            {
                _isShieldEnabled = true;
                _shieldLife = 2;
                _shieldsLight.color = _halfShieldsCol;
            }
            else if (_shieldStrength == ShieldStrength.Full)
            {
                _isShieldEnabled = true;
                _shieldLife = 3;
                
                _shieldsLight.color = _fullShieldsCol;
                
            }
        }
        
    }

    ShieldStrength GetShieldStrength_ByInt(int newVal)
    {
        if (newVal == 0)
            _shieldStrength = ShieldStrength.None;
        else if (newVal == 1)
            _shieldStrength = ShieldStrength.Critical;
        else if (newVal == 2)
            _shieldStrength = ShieldStrength.Half;
        else if(newVal == 3)
            _shieldStrength = ShieldStrength.Full;
        
        
        return _shieldStrength;
    }



    
    private void UpdateShieldVisuals()  //called in Update()
    {
        if (_isInvulnerable)
        {
            // Color transition during invulnerability
            _shieldsLight.color = Color.Lerp(_shieldsLight.color, _targetColor,
                Time.deltaTime * _colorTransitionSpeed);
        }
        else
        {
            Color col = _shieldSprite.color;
            float alpha = col.a;

            switch (_shieldLife)
            {
                case 3:
                    alpha = 1.0f;
                    _targetColor = _fullShieldsCol;
                    break;
                case 2:
                    alpha = 0.9f;
                    _targetColor = _halfShieldsCol;
                    break;
                case 1:
                    alpha = 0.8f;
                    _targetColor = _criticalShieldsCol;
                    break;
                case 0:
                    DisableShields();
                    //_shieldVisualizer.SetActive(false);
                    break;
            }
            //  Shield Alpha change when not invulnerable
            col.a = alpha;
            _shieldSprite.color = col;

            // Color change when not invulnerable
            _shieldsLight.color = _targetColor;

            // Light2D intensity pulse when not invulnerable
            float pulse = Mathf.Sin(Time.time * _steadyPulseSpeed) *
                (_maxPulseIntensity - _minPulseIntensity) * 0.5f +
                (_maxPulseIntensity + _minPulseIntensity) * 0.5f;

            _shieldsLight.intensity = pulse;
        }
    }
    IEnumerator PlayEnergyExplosion()
    {
        if (_shieldExplosionEffect != null)
        {
            _shieldExplosionEffect.Play();
            /*while (_shieldExplosionEffect.IsAlive(true))
                yield return null;*/

            yield return new WaitForSeconds(1.2f);

            _shieldExplosionEffect.Stop();
        }

    }

    public bool IsInvulernable()
    {
        return _isInvulnerable;
    }

    public int GetShieldStrength()
    {
        return _shieldLife;
    }





    
}
