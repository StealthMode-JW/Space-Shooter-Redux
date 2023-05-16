using UnityEngine;
using UnityEngine.UI;

public class ThrusterController : MonoBehaviour
{
    [SerializeField]
    ParticleSystem thrusterParticles;
    private bool isPlaying;
    bool _isOverheated;

    [SerializeField]
    Slider _thrusterSlider;

    [SerializeField]
    Image _sliderFill;
    [SerializeField]
    Color _startingCol = Color.yellow;
    [SerializeField]
    Color _endingCol = Color.red;
    [SerializeField]
    Color _cooldownCol = new Color(1, 0, 0, 0.33f);

    [SerializeField]
    float _fillSpeed = 0.5f;
    [SerializeField]
    float _cooldownSpeed = 0.25f;
    [SerializeField]
    float _automaticCooldownSpeed = 0.125f;
    [SerializeField]
    float _cooldownPulseSpeed = 5.0f;

    [SerializeField]
    AudioSource _audioSource;
    [SerializeField]
    AudioClip _thrusterInUseSound;
    [SerializeField]
    AudioClip _thrusterCooldownSound;
    [SerializeField]
    AudioClip _overheatingSound;
    [SerializeField]
    AudioClip _readySound;
    

    private void Update()
    {
        if (_isOverheated == false && Input.GetKey(KeyCode.LeftShift))
        {
            if (!isPlaying)
            {
                thrusterParticles.Play();
                isPlaying = true;
                _audioSource.PlayOneShot(_thrusterInUseSound);
            }

            _thrusterSlider.value += Time.deltaTime * _fillSpeed;

            if (_thrusterSlider.value >= 1.0f)
            {
                _audioSource.PlayOneShot(_overheatingSound);
                _isOverheated = true;
            }
        }
        else
        {
            if (isPlaying)
            {
                thrusterParticles.Stop();
                isPlaying = false;
                _audioSource.PlayOneShot(_thrusterCooldownSound);
            }

            if (_thrusterSlider.value > 0)
            {
                float speed = _isOverheated ? _automaticCooldownSpeed : _cooldownSpeed;

                _thrusterSlider.value -= Time.deltaTime * speed;

                if (_isOverheated && _thrusterSlider.value <= 0)
                {
                    _audioSource.PlayOneShot(_readySound);
                    _isOverheated = false;
                }
            }
        }
        if (_isOverheated)
        {
            float pulse = Mathf.Sin(Time.time * _cooldownPulseSpeed) * 0.5f + 0.5f;

            _sliderFill.color = Color.Lerp(_endingCol, _cooldownCol, pulse);
        }
        else
        {
            _sliderFill.color = Color.Lerp(_startingCol, _endingCol, _thrusterSlider.value);
        }

    }

    public bool IsThrusterPlaying()
    {
        return isPlaying;
    }
}
