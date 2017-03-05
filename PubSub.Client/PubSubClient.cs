using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace PubSub.Client
{
    public class PubSubClient
    {
        /// <summary>
        /// Unique user name
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Server url (for example: http://localhost:51324/signalr)
        /// </summary>
        public string Url { get; private set; }

        private string _key;

        private Action<string> dataReceived;
        /// <summary>
        /// Data received from server event
        /// </summary>
        public Action<string> OnDataReceived
        {
            get
            {
                return dataReceived;
            }
            set
            {
                dataReceived = value;
                _proxy.On("SendMessage", dataReceived);
            }
        }

        protected HubConnection _connection;
        protected IHubProxy _proxy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName">Unique user name</param>
        /// <param name="url">Server url (for example: http://localhost:51324/signalr)</param>
        public PubSubClient(string userName,string url, string key)
        {
            this.UserName = userName;
            this.Url = url;
            _key = key;
        }

        public async Task Start()
        {
            _connection = new HubConnection(Url);
            _connection.Headers.Add("CustomAuthorization",
                  Crypto.Encrypt(string.Format("username={0}",UserName), _key));

            _proxy = _connection.CreateHubProxy("PubSubHub");

            if (dataReceived != null)
                OnDataReceived = dataReceived;

            await _connection.Start();
        }
        public void Stop()
        {
            _connection.Stop();
        }

        public virtual async void Publish(string data)
        {
           await _proxy.Invoke("Publish", data);
        }
        
        public virtual async void Subscribe(string groupName)
        {
            await _proxy.Invoke("Subscribe", groupName);
        }

        public virtual async void UnSubscribe(string groupName)
        {
            await _proxy.Invoke("UnSubscribe", groupName);
        }
    }
}
