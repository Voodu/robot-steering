using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private Slider _accelerationSlider;
    [SerializeField] private Text _accelerationValue;
    [SerializeField] private Toggle _brake;
    [SerializeField] private Dropdown _communicationType;
    [SerializeField] private Slider _gearSlider;
    [SerializeField] private Text _gearValue;
    private bool _isMusicOn;
    [SerializeField] private Toggle _music;
    [SerializeField] private Slider _nitroSlider;
    [SerializeField] private Text _nitroValue;
    [SerializeField] private Slider _powerSlider;
    [SerializeField] private Text _powerValue;

    private void Start()
    {
        _isMusicOn = PlayerPrefsX.GetBool("soundON", false);
        if (_music.isOn != _isMusicOn)
        {
            _music.isOn = _isMusicOn;
            ToggleSound();
        }

        _brake.isOn = PlayerPrefsX.GetBool("brakeON", false);
        _accelerationSlider.value = PlayerPrefs.GetInt("accelerate", 8);
        _powerSlider.value = PlayerPrefs.GetInt("power", 50);
        _nitroSlider.value = PlayerPrefs.GetInt("nitro", 2);
        _gearSlider.value = PlayerPrefs.GetInt("gear", 10);
        _communicationType.captionText.text = PlayerPrefs.GetString("protocol", "UDP");
        _communicationType.value = _communicationType.captionText.text == "UDP" ? 0 : 1;
        _accelerationValue.text = _accelerationSlider.value.ToString(CultureInfo.InvariantCulture);
        _powerValue.text = _powerSlider.value.ToString(CultureInfo.InvariantCulture) + '%';
        _nitroValue.text = _nitroSlider.value.ToString(CultureInfo.InvariantCulture);
        _gearValue.text = _gearSlider.value.ToString(CultureInfo.InvariantCulture);
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            ToMenu();
        }
    }

    public void UpdateAccelText()
    {
        _accelerationValue.text = _accelerationSlider.value.ToString(CultureInfo.InvariantCulture);
    }

    public void UpdatePowerText()
    {
        _powerValue.text = _powerSlider.value.ToString(CultureInfo.InvariantCulture) + '%';
    }

    public void UpdateNitroText()
    {
        _nitroValue.text = _nitroSlider.value.ToString(CultureInfo.InvariantCulture);
    }

    public void UpdateGearText()
    {
        _gearValue.text = _gearSlider.value.ToString(CultureInfo.InvariantCulture);
    }

    public void ToggleSound()
    {
        AudioManager.Instance.ToggleSound();
    }

    public void ToMenu()
    {
        PlayerPrefs.SetInt("accelerate", (int) _accelerationSlider.value);
        PlayerPrefs.SetInt("power", (int) _powerSlider.value);
        PlayerPrefs.SetInt("nitro", (int) _nitroSlider.value);
        PlayerPrefs.SetInt("gear", (int) _gearSlider.value);
        PlayerPrefs.SetString("protocol", _communicationType.captionText.text);
        PlayerPrefsX.SetBool("soundON", _music.isOn);
        PlayerPrefsX.SetBool("brakeON", _brake.isOn);
        PlayerPrefs.Save();
        SceneManager.LoadScene("menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}