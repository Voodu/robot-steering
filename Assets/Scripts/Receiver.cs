using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    private bool _changed; //'synchronizacja' watkow

    private string _data = "";
    private string _ip = "127.0.0.1";

    private Thread _listenThread;
    private bool _on;
    private int _port = 5000;
    private string _protocol = "UDP";
    private TcpClient _tcpClient;
    private TcpListener _tcpListener;
    private UdpClient _udpClient;
    private IPEndPoint _udpEndPoint;

    private void Start()
    {
        _on = PlayerPrefsX.GetBool("localServerON", false);

        if (!_on)
        {
            return;
        }

        _protocol = PlayerPrefs.GetString("protocol", "UDP");
        _ip = PlayerPrefs.GetString("localServerIP", "127.0.0.1");
        _port = PlayerPrefs.GetInt("localServerPORT", 5000);

        ThreadStart ts;

        if (_protocol == "TCP")
        {
            ts = ListenTcp;
            _tcpListener = new TcpListener(IPAddress.Parse(_ip), _port);
            _tcpClient = new TcpClient();
        }
        else
        {
            ts = ListenUdp;
            _udpClient = new UdpClient(_port);
        }

        _listenThread = new Thread(ts);
        _listenThread.Start();
    }

    private void Update()
    {
        if (!_on)
        {
            return;
        }

        if (!_listenThread.IsAlive)
        {
            ThreadStart ts;
            if (_protocol == "TCP")
            {
                ts = ListenTcp;
            }
            else
            {
                ts = ListenUdp;
            }

            _listenThread = new Thread(ts);
            _listenThread.Start();
        }

        if (_changed)
        {
            UIApp.Instance.ExternalDisplay($"Odebrano: {_data}{'\n'}");
            _changed = false;
        }
    }


    private void ListenTcp()
    {
        _tcpListener.Start();
        _tcpClient = _tcpListener.AcceptTcpClient();

        var nwStream = _tcpClient.GetStream();
        var buffer = new byte[_tcpClient.ReceiveBufferSize];

        var bytesRead = nwStream.Read(buffer, 0, _tcpClient.ReceiveBufferSize);

        var dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        _data = dataReceived;
        _changed = true;
    }

    private void ListenUdp()
    {
        var bytes = _udpClient.Receive(ref _udpEndPoint);
        var dataReceived = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

        _data = dataReceived;
        _changed = true;
    }

    private void OnDisable()
    {
        if (_protocol == "TCP")
        {
            _tcpClient.Close();
            _tcpListener.Stop();
        }
    }
}