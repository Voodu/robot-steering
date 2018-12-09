using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Control : MonoBehaviour
{
    public static Control Instance;

    private readonly float[] _control = {255, 255};
    private readonly float[] _inputRead = {0, 0};
    private readonly float[] _lastControl = {255, 255};

    [Range(1, 50)] [SerializeField] private float _accelerateRate = 5;
    private bool _brake;

    private string _cmd = "";
    [SerializeField] private SpriteRenderer _downArrow;
    [SerializeField] private SpriteRenderer _fire;

    private bool _instantBrake;

    private string _ip = "127.0.0.1";
    private string _lastCmd = "";
    [SerializeField] private SpriteRenderer _leftArrow;
    [Range(1, 100)] [SerializeField] private int _maxPercentPower = 50;
    private bool _nitro;
    [Range(1, 5)] [SerializeField] private int _nitroMultiplier = 2;
    private int _port = 5000;
    [Range(0, 100)] [SerializeField] private int _powerPercentPerGear = 20;
    [SerializeField] private SpriteRenderer _rightArrow;
    [SerializeField] private SpriteRenderer _speedometerForward;
    [SerializeField] private SpriteRenderer _speedometerSideward;
    [SerializeField] private SpriteRenderer _stop;

    [SerializeField] private SpriteRenderer _upArrow;

    public string Protocol { get; private set; } = "UDP";

    public float Accelerate => _accelerateRate;

    public int MaxPercentPower => _maxPercentPower;

    public int NitroMultiplier => _nitroMultiplier;

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

        _upArrow.color = _downArrow.color = _leftArrow.color = _rightArrow.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _fire.enabled = false;
        _stop.enabled = false;

        Protocol = PlayerPrefs.GetString("protocol", "UDP");
        _ip = PlayerPrefs.GetString("receiverIP", "127.0.0.1");
        _port = PlayerPrefs.GetInt("receiverPORT", 5000);

        _instantBrake = PlayerPrefsX.GetBool("brakeON", false);

        _accelerateRate = PlayerPrefs.GetInt("accelerate", 8);
        _maxPercentPower = PlayerPrefs.GetInt("power", 50);
        _nitroMultiplier = PlayerPrefs.GetInt("nitro", 2);
        _powerPercentPerGear = PlayerPrefs.GetInt("gear", 10);
    }


    private void Update()
    {
        _upArrow.color = _downArrow.color =
            _leftArrow.color = _rightArrow.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); //do kolorowania strzalek

        _brake = Input.GetButton("Brake");
        _control[0] = _inputRead[0] = Input.GetAxis("Vertical");
        _control[1] = _inputRead[1] = Input.GetAxis("Horizontal");
        _nitro = (Math.Abs(_inputRead[0]) > 0.01 || Math.Abs(_inputRead[1]) > 0.01) && !_brake && Input.GetButton("Nitro"); //jak jest odczyt z pionu/poziomu i nie ma hamulca, to wtedy ustawia nitro

        //warunki sprawdzaja, czy uzytkownik nie chce ustawic np. -10 albo 110% mocy
        if (Input.GetButtonDown("GearDown"))
        {
            _maxPercentPower = _maxPercentPower - _powerPercentPerGear > 0
                ? _maxPercentPower - _powerPercentPerGear
                : 0;
        }
        else if (Input.GetButtonDown("GearUp"))
        {
            _maxPercentPower = _maxPercentPower + _powerPercentPerGear < 100
                ? _maxPercentPower + _powerPercentPerGear
                : 100;
        }

        //formatowanie odczytanego inputu <-1; 1> na PWM-owy <0; 510>
        _control[0] = _control[0] * 255 + 255;
        _control[1] = _control[1] * 255 + 255;
        //0-255-510

        //tylko ustawianie grafik
        if (_brake)
        {
            _stop.enabled = true;
        }
        else
        {
            _stop.enabled = false;

            if (_inputRead[0] > 0) //up
            {
                _upArrow.color = new Color(1, 1, 1);
            }

            if (_inputRead[0] < 0) //down
            {
                _downArrow.color = new Color(1, 1, 1);
            }

            if (_inputRead[1] > 0) //right
            {
                _rightArrow.color = new Color(1, 1, 1);
            }

            if (_inputRead[1] < 0) //left
            {
                _leftArrow.color = new Color(1, 1, 1);
            }
        }

        _fire.enabled = _nitro;

        //kontrola maksymalnej mocy
        if (Math.Abs(255 - _control[0]) > 255 * 0.01 * _maxPercentPower
        ) //jesli obecna wartosc mialaby byc wieksza od maksymalnej...
        {
            if (_inputRead[0] > 0)
            {
                _control[0] = 255 + 255f * 0.01f * _maxPercentPower; //... to ustaw (w odpowiedni sposob) na maksymalna
            }
            else
            {
                _control[0] = 255 - 255f * 0.01f * _maxPercentPower;
            }
        }

        if (Math.Abs(255 - _control[1]) > 255 * 0.01 * _maxPercentPower)
        {
            if (_inputRead[1] > 0)
            {
                _control[1] = 255 + 255f * 0.01f * _maxPercentPower;
            }
            else
            {
                _control[1] = 255f - 255f * 0.01f * _maxPercentPower;
            }
        }

        //hamowanie liniowe (ustawia wartosc obecnej komendy na przeciwna do ostatniej wyslanej)
        if (_brake)
        {
            if (Math.Abs(Math.Round(_lastControl[0]) - 255) > 0.01)
            {
                if (_lastControl[0] > 255)
                {
                    _control[0] = 0;
                }
                else
                {
                    _control[0] = 510;
                }
            }
            else
            {
                _control[0] = 255;
            }

            if (Math.Abs(Math.Round(_lastControl[1]) - 255) > 0.01)
            {
                if (_lastControl[1] > 255)
                {
                    _control[1] = 0;
                }
                else
                {
                    _control[1] = 510;
                }
            }
            else
            {
                _control[1] = 255;
            }

            _nitro = true; //do szybszego hamowania
        }

        //'liniowanie' przyrostu predkosci
        _control[0] = NormalizeSpeed(_lastControl[0], _control[0], _nitro && Math.Abs(_inputRead[0]) > 0.01 ? _nitroMultiplier : 1);
        _control[1] = NormalizeSpeed(_lastControl[1], _control[1], _nitro && Math.Abs(_inputRead[1]) > 0.01 ? _nitroMultiplier : 1);

        //hamowanie natychmiastowe
        if (_brake && _instantBrake)
        {
            _control[0] = _control[1] = 255;
        }

        _lastControl[0] = _control[0];
        _lastControl[1] = _control[1];

        _speedometerForward.transform.eulerAngles = new Vector3(0, 0, -90 + Math.Abs(_control[0] - 255) * (90f / 255f));
        _speedometerSideward.transform.eulerAngles = new Vector3(0, 0, -(_control[1] - 255) * (90f / 255f));

        var buff = "";
        _cmd = "";

        //formatowanie odczytanych wartosci i wstawianie do wysylanej komendy
        buff = Math.Round(_control[0]).ToString(CultureInfo.InvariantCulture);
        while (buff.Length < 3)
        {
            buff = '0' + buff;
        }

        _cmd += buff + '&';

        buff = Math.Round(_control[1]).ToString(CultureInfo.InvariantCulture);
        while (buff.Length < 3)
        {
            buff = '0' + buff;
        }

        _cmd += buff;

        //wysylanie komendy
        if (_cmd != _lastCmd)
        {
            if (Protocol == "TCP")
            {
                if (SendTcp(_ip, _port, _cmd))
                {
                    UIApp.Instance.InternalDisplay($"Wysłano: {_cmd}{'\n'}");
                }
                else
                {
                    UIApp.Instance.InternalDisplay($"Błąd połączenia z {_ip}:{_port}\nSprawdź, czy serwer przyjmuje połączenia lub wróć do menu i podaj inne IP i/lub port.\n");
                }
            }
            else
            {
                if (SendUdp(_ip, _port, _cmd))
                {
                    UIApp.Instance.InternalDisplay($"Wysłano: {_cmd}{'\n'}");
                }
                else
                {
                    UIApp.Instance.InternalDisplay($"Błąd połączenia z {_ip}:{_port}\nSprawdź, czy serwer przyjmuje połączenia lub wróć do menu i podaj inne IP i/lub port.\n");
                }
            }

            _lastCmd = _cmd;
        }
    }

    private bool SendTcp(string ip, int port, string data)
    {
        var client = new TcpClient();
        try
        {
            client.Connect(IPAddress.Parse(ip), port);
        }
        catch (Exception)
        {
            return false;
        }

        var dataStream = client.GetStream();
        var msg = Encoding.ASCII.GetBytes(data);
        dataStream.Write(msg, 0, msg.Length);
        dataStream.Close();
        client.Close();
        return true;
    }

    private bool SendUdp(string ip, int port, string data)
    {
        var sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var sendingEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        var sendBuffer = Encoding.ASCII.GetBytes(data);

        try
        {
            sendingSocket.SendTo(sendBuffer, sendingEndPoint);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }


    private float NormalizeSpeed(float lastSpeed, float rawSpeed, float nitroBonus)
    {
        return lastSpeed + (rawSpeed - lastSpeed) * 0.0001f * _accelerateRate * nitroBonus;
    }

    //uruchmiana podczas zamkniecia okna/wylaczenia aplikacji, by wyzerowac wtedy predkosc
    private void OnDisable()
    {
        if (Protocol == "TCP")
        {
            SendTcp(_ip, _port, "255&255&0");
        }
        else
        {
            SendUdp(_ip, _port, "255&255&0");
        }
    }
}