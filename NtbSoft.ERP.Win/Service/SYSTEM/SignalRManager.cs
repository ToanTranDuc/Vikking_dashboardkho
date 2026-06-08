using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Web.Services
{
    public sealed class SignalRManager
    {
        private static SignalRManager _instance = null;
        private static readonly object _lock = new object();

        private HubConnection _connection;
        private IHubProxy _hub;
        private string _serverUrl;

        public event Action<dynamic> OnNotificationReceived;


        public static SignalRManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SignalRManager();
                        }
                    }
                }
                return _instance;
            }
        }       
        private SignalRManager()
        {
            
        }

        // Method khởi tạo kết nối
        public async Task<bool> Initialize(string serverUrl)
        {
            try
            {
                _serverUrl = serverUrl.Replace(@"/api/", "");
                _connection = new HubConnection(_serverUrl);
                _hub = _connection.CreateHubProxy("HubService");

                // Đăng ký các sự kiện
                RegisterEvents();

                // Xử lý reconnection
                _connection.Closed += async () =>
                {
                    await Task.Delay(5000);
                    await StartConnection();
                };

                return await StartConnection();
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        private void RegisterEvents()
        {
            // Sự kiện chung
            _hub.On<dynamic>("ReceiveNotification", (message) =>
            {
                OnNotificationReceived?.Invoke(message);
            });
          
        }

        private async Task<bool> StartConnection()
        {
            try
            {
                if (_connection.State == ConnectionState.Disconnected)
                {
                    await _connection.Start();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
             
                return false;
            }
        }

        public async Task Disconnect()
        {
            if (_connection != null)
            {
                _connection.Stop();
                _connection.Dispose();
            }
        }

        public ConnectionState GetConnectionState()
        {
            return _connection?.State ?? ConnectionState.Disconnected;
        }
    }
}