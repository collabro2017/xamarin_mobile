using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websockets;

namespace X
{
    public class ActionCableEventArgs : EventArgs
    {
        public string Type { get; }
        public string Channel { get; }
        public string Message { get; }

        public ActionCableEventArgs(string rawMessage)
        {
            try
            {
                JToken jObj = JToken.Parse(rawMessage);

                if (jObj["type"] != null)
                {
                    Type = jObj["type"].Value<string>();

                    if (Type == "ping")
                    {
                        Message = jObj["message"].Value<string>();
                    }
                    else if (Type == "confirm_subscription")
                    {
                        Channel = GetChannel(jObj["identifier"].Value<string>());
                    }


                }
                else
                {
                    Channel = GetChannel(jObj["identifier"].Value<string>());
                    Message = jObj["message"].ToString();
                }


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception occured: {0}", e.Message);
            }
        }

        private string GetChannel(string identifier)
        {
            return JObject.Parse(identifier)["channel"].Value<string>();
        }
    }

    public class ActionCableClient
    {
        public event EventHandler<ActionCableEventArgs> Pinged;
        public event EventHandler<ActionCableEventArgs> Connected;
        public event EventHandler<ActionCableEventArgs> MessageReceived;
        public event EventHandler<ActionCableEventArgs> Disconnected;
        public int LogLevel { get; set; }
        public string RemoteUrl { get; set; }
        private int _port = 0;
        public int Port
        {
            get { return _port; }
            set
            {
                if (_port < 0 || _port > 65535)
                    throw new ArgumentOutOfRangeException(nameof(Port), "Port number should be within the range 0 - 65535.");

                _port = value;
            }
        }
        private string _channel = "";
        public string Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
            }
        }
        public bool IgnorePings { get; set; }
        private string identifierString = "";
        private Websockets.IWebSocketConnection _client;



        public ActionCableClient(string remoteUrl, string channel)
        {
            RemoteUrl = remoteUrl;
            Channel = channel;
            LogLevel = 0;
        }

        public Task<bool> ConnectAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            if (_client == null)
            {
                InitializeClient();
            }

            Action openedConnectionHandler = null;
            openedConnectionHandler = () => {

                Log("Websocket connection opened", 0);

                if (identifierString == "")
                    identifierString = GetIdentifierString();

                var connectRequest = new
                {
                    command = "subscribe",
                    identifier = identifierString
                };

                string commandRequestString = JsonConvert.SerializeObject(connectRequest);

                Log("Subscribing to " + Channel, 1);

                Action<string> onMessageHandler = null;
                onMessageHandler = (obj) => {
                    var e = new ActionCableEventArgs(obj);
                    if (e.Type == "confirm_subscription" && e.Channel == Channel)
                    {
                        Log("Subscription success", 1);
                        _client.OnMessage -= onMessageHandler;
                        _client.OnOpened -= openedConnectionHandler;

                        _client.OnMessage += HandleIncomingMessage;

                        RaiseConnected(obj);

                        tcs.SetResult(true);
                    }
                };

                _client.OnMessage += onMessageHandler;
                _client.Send(commandRequestString);

            };

            _client.OnOpened += openedConnectionHandler;

            _client.Open(string.Format("{0}{1}{2}", RemoteUrl, Port > 0 ? ":" : "", Port > 0 ? Port.ToString() : ""));

            Log("Opening websocket connection", 0);

            return tcs.Task;

        }

        public void Send(string message)
        {

        }

        public void Perform(string action, string message)
        {
            var a = new
            {
                action = action,
                message = message
            };
            string actionString = JsonConvert.SerializeObject(a);

            if (identifierString == "")
                identifierString = GetIdentifierString();

            var cmd = new
            {
                command = "message",
                identifier = identifierString,
                data = actionString
            };
            string cmdString = JsonConvert.SerializeObject(cmd);

            Log("Sending " + cmdString, 1);

            _client.Send(cmdString);
        }

        private void InitializeClient()
        {
            _client = Websockets.WebSocketFactory.Create();

            _client.OnClosed += HandleConnectionClosed;
            _client.OnError += HandleConnectionError;
            _client.OnDispose += HandleWebSocketDisposed;
        }

        void HandleConnectionClosed()
        {
            Log("Connection closed", 2);
            _client.OnMessage -= HandleIncomingMessage;
        }

        void HandleIncomingMessage(string obj)
        {
            //Log("Message received: " + obj, 0);
            var arg = new ActionCableEventArgs(obj);
            if (arg.Type == "ping" && IgnorePings)
                return;

            RaiseMessageReceived(obj);
        }

        void HandleConnectionError(string obj)
        {
            Log("Error: " + obj, 2);
        }

        void RaiseMessageReceived(string message)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler(this, new ActionCableEventArgs(message));
            }
        }

        void RaiseConnected(string message)
        {
            var handler = Connected;
            if (handler != null)
            {
                handler(this, new ActionCableEventArgs(message));
            }
        }

        void HandleWebSocketDisposed(IWebSocketConnection obj)
        {
            Log("Websocket disposed", 0);

            _client.OnClosed -= HandleConnectionClosed;
            _client.OnError -= HandleConnectionError;
            _client.OnDispose -= HandleWebSocketDisposed;

            _client = null;
        }

        string GetIdentifierString()
        {
            var id = new
            {
                channel = Channel
            };

            return JsonConvert.SerializeObject(id);
        }

        void Log(string message, int level = 3)
        {
            if (level >= LogLevel)
            {
                System.Diagnostics.Debug.WriteLine("[ActionCableClient] " + message);
            }
        }
    }
}