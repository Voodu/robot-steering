using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource _appSound;
    private Scene _currentScene;
    private bool _isSoundOn;

    [SerializeField] private AudioSource _menuSound;
    [SerializeField] private AudioSource _nitroSound;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        _isSoundOn = PlayerPrefsX.GetBool("soundON", false);

        _currentScene = SceneManager.GetActiveScene();
        SceneManager.sceneLoaded += (scene, mode) => _currentScene = scene;
    }

    private void Update()
    {
        if (!_isSoundOn)
        {
            if (_menuSound.isPlaying)
            {
                _menuSound.Stop();
            }

            if (_appSound.isPlaying)
            {
                _appSound.Stop();
            }

            if (_nitroSound.isPlaying)
            {
                _nitroSound.Stop();
            }

            return;
        }

        switch (_currentScene.name)
        {
            case "menu":
                if (!_menuSound.isPlaying)
                {
                    _appSound.Stop();
                    _nitroSound.Stop();
                    _menuSound.Play();
                }

                break;
            case "options":
                if (!_menuSound.isPlaying)
                {
                    _appSound.Stop();
                    _nitroSound.Stop();
                    _menuSound.Play();
                }

                break;
            case "app":
                if (!_appSound.isPlaying && !_nitroSound.isPlaying)
                {
                    _menuSound.Stop();
                    _appSound.Play();
                }

                if (Input.GetButton("Nitro") && !Input.GetButton("Brake") &&
                    (Math.Abs(Input.GetAxis("Horizontal")) > 0.01 || Math.Abs(Input.GetAxis("Vertical")) > 0.01))
                {
                    if (!_nitroSound.isPlaying)
                    {
                        _nitroSound.Play();
                    }

                    _appSound.Pause();
                }
                else
                {
                    if (!_appSound.isPlaying)
                    {
                        _appSound.Play();
                    }

                    _nitroSound.Pause();
                }

                break;
        }
    }

    public void ToggleSound()
    {
        _isSoundOn = !_isSoundOn;
    }
}