using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private InputField _localServerIp;
    private bool _localServerIpValid;
    private bool _localServerOn;
    [SerializeField] private InputField _localServerPort;
    private bool _localServerPortValid;

    [SerializeField] private Toggle _localServerToggle;

    [SerializeField] private InputField _receiverIp;
    private bool _receiverIpValid;
    [SerializeField] private InputField _receiverPort;
    private bool _receiverPortValid;

    [SerializeField] private Button _startButton;

    // Use this for initialization
    private void Start()
    {
        //tcp

        _receiverIp.text = PlayerPrefs.GetString("receiverIP", "");
        CheckReceiverIp();

        _receiverPort.text = PlayerPrefs.GetInt("receiverPORT", 0).ToString();
        CheckReceiverPort();

        //server
        _localServerOn = PlayerPrefsX.GetBool("localServerON", _localServerToggle.isOn);

        if (_localServerOn != _localServerToggle.isOn)
        {
            _localServerToggle.isOn = _localServerOn;
            ToggleLocalServer();
        }

        _localServerIp.interactable = _localServerPort.interactable = _localServerToggle.isOn;

        _localServerIp.text = PlayerPrefs.GetString("localServerIP", "");
        CheckLocalServer();

        _localServerPort.text = PlayerPrefs.GetInt("localServerPORT", 0).ToString();
        CheckLocalServerPort();

        SetButton();
    }

    private void Update()
    {
        if (Input.GetButton("Submit"))
        {
            StartApp();
        }
    }

    private bool SetButton()
    {
        if ((_localServerOn && (!_localServerIpValid || !_localServerPortValid)) ||
            !_receiverIpValid || !_receiverPortValid)
        {
            _startButton.interactable = false;
            return false;
        }

        _startButton.interactable = true;
        return true;
    }

    public void ToggleLocalServer()
    {
        _localServerOn = !_localServerOn;
        _localServerIp.interactable = _localServerPort.interactable = _localServerOn;
        SetButton();
    }

    public void CheckReceiverIp()
    {
        var cb = _receiverIp.colors;
        cb.normalColor = Color.white;

        var addrString = _receiverIp.text;
        IPAddress address;
        if (IPAddress.TryParse(addrString, out address) && address != null)
        {
            _receiverIpValid = true;
        }
        else
        {
            cb.normalColor = Color.red;
            _receiverIpValid = false;
        }

        _receiverIp.colors = cb;
        SetButton();
    }

    public void CheckReceiverPort()
    {
        var cb = _receiverPort.colors;
        cb.normalColor = Color.white;
        int port;
        if (int.TryParse(_receiverPort.text, out port) && _receiverPort.text != null && port > 0)
        {
            _receiverPortValid = true;
        }
        else
        {
            cb.normalColor = Color.red;
            _receiverPortValid = false;
        }

        _receiverPort.colors = cb;
        SetButton();
    }

    public void CheckLocalServer()
    {
        var cb = _localServerIp.colors;
        cb.normalColor = Color.white;

        var addrString = _localServerIp.text;
        IPAddress address;
        if (IPAddress.TryParse(addrString, out address))
        {
            _localServerIpValid = true;
        }
        else
        {
            cb.normalColor = Color.red;
            _localServerIpValid = false;
        }

        _localServerIp.colors = cb;
        SetButton();
    }

    public void CheckLocalServerPort()
    {
        var cb = _localServerPort.colors;
        cb.normalColor = Color.white;
        int port;
        if (int.TryParse(_localServerPort.text, out port) && _localServerPort.text != null && port > 0)
        {
            _localServerPortValid = true;
        }
        else
        {
            cb.normalColor = Color.red;
            _localServerPortValid = false;
        }

        _localServerPort.colors = cb;
        SetButton();
    }

    public void StartApp()
    {
        if (SetButton())
        {
            if (_receiverIpValid)
            {
                PlayerPrefs.SetString("receiverIP", _receiverIp.text);
            }

            if (_receiverPortValid)
            {
                PlayerPrefs.SetInt("receiverPORT", int.Parse(_receiverPort.text));
            }

            PlayerPrefsX.SetBool("localServerON", _localServerOn);

            if (_localServerIpValid)
            {
                PlayerPrefs.SetString("localServerIP", _localServerIp.text);
            }

            if (_localServerPortValid)
            {
                PlayerPrefs.SetInt("localServerPORT", int.Parse(_localServerPort.text));
            }

            PlayerPrefs.Save();
            SceneManager.LoadScene("app");
        }
    }

    public void ToOptions()
    {
        if (_receiverIpValid)
        {
            PlayerPrefs.SetString("receiverIP", _receiverIp.text);
        }

        if (_receiverPortValid)
        {
            PlayerPrefs.SetInt("receiverPORT", int.Parse(_receiverPort.text));
        }

        PlayerPrefsX.SetBool("localServerON", _localServerOn);

        if (_localServerIpValid)
        {
            PlayerPrefs.SetString("localServerIP", _localServerIp.text);
        }

        if (_localServerPortValid)
        {
            PlayerPrefs.SetInt("localServerPORT", int.Parse(_localServerPort.text));
        }

        PlayerPrefs.Save();

        SceneManager.LoadScene("options");
    }

    public void Exit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}