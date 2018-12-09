using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIApp : MonoBehaviour
{
    [SerializeField] private const int BuffSize = 300;
    public static UIApp Instance;
    private string _baseText = "";
    private Queue<string> _externalBuffer;
    private Queue<string> _internalBuffer;
    private bool _isSet;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Text _externalConsole;
    [SerializeField] private Text _info;
    [SerializeField] private Text _internalConsole;

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

        _internalBuffer = new Queue<string>();
        _externalBuffer = new Queue<string>();

        _externalConsole.text = _internalConsole.text = "";
        SetInfo();
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            ToMenu();
        }

        var psmain = _particle.main;
        if (Input.GetButton("Nitro") &&
            (Math.Abs(Input.GetAxis("Horizontal")) > 0.01 || Math.Abs(Input.GetAxis("Vertical")) > 0.01) &&
            !Input.GetButton("Brake"))
        {
            psmain.startLifetime = 0.5f;
        }
        else
        {
            psmain.startLifetime = 0;
        }

        SetInfo();
    }

    private void Display(ref Queue<string> buffer, ref Text console, string content, bool append = true)
    {
        if (append)
        {
            if (buffer.Count >= BuffSize)
            {
                buffer.Dequeue();
            }

            buffer.Enqueue(content);
            console.text = "";
            var sb = new StringBuilder();
            foreach (var s in buffer)
            {
                sb.Append(s);
            }

            console.text += sb.ToString();
        }
        else
        {
            console.text = content;
        }
    }

    public void InternalDisplay(string content, bool append = true)
    {
        Display(ref _internalBuffer, ref _internalConsole, content, append);
    }

    public void ExternalDisplay(string content, bool append = true)
    {
        Display(ref _externalBuffer, ref _externalConsole, content, append);
    }

    private void SetInfo()
    {
        _info.text = "";
        if (!_isSet)
        {
            _baseText =
                $"{Control.Instance.Protocol}:\t{PlayerPrefs.GetString("receiverIP")}:{PlayerPrefs.GetInt("receiverPORT")}{'\n'}";
            if (PlayerPrefsX.GetBool("localServerON", false))
            {
                _baseText +=
                    $"Serwer:\t{PlayerPrefs.GetString("localServerIP")}{':'}{PlayerPrefs.GetInt("localServerPORT")}{'\n'}";
            }

            _isSet = true;
        }

        _info.text = _baseText;
        _info.text += $"{Control.Instance.MaxPercentPower}% mocy maksymalnej\n";
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}