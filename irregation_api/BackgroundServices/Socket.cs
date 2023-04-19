using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using irregation_api.Models.Json;

namespace irregation_api.Socket
{
    public class Socket : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        public Socket(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        private async Task DoWork(CancellationToken stoppingToken)
        {
            getUuids();

            var uri = new Uri("wss://ingress.mobilisis.com/ws-server/device");

            // start consumer
            var task = Task.Run(() => Consume());

            await task;


            async Task Consume()
            {
                while (!stoppingToken.IsCancellationRequested) {
                    ClientWebSocket client = await createClient(uri);

                    var subscribeObject = new
                    {
                        devices = GlobalUuid.devices
                    };
                    var byteArray = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(subscribeObject));
                    await client.SendAsync(byteArray, WebSocketMessageType.Text, true, CancellationToken.None);


                    var cancelationTask = Task.Delay(-1, stoppingToken);
                    var buffer = new byte[5120];
                    var count = GlobalUuid.devices.Count;

                    while (!stoppingToken.IsCancellationRequested && count == GlobalUuid.devices.Count)
                    {
                        try
                        {
                            Array.Clear(buffer, 0, buffer.Length);
                            var timeoutTokenSource = new CancellationTokenSource();
                            timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
                            var timeoutTask = Task.Delay(-1, timeoutTokenSource.Token);


                            var receiveTask = client.ReceiveAsync(buffer, CancellationToken.None);

                            var finishedTask = await Task.WhenAny(receiveTask, cancelationTask, timeoutTask);

                            if (receiveTask != null)
                            {
                                //Debug.WriteLine(Encoding.UTF8.GetString(buffer));
                                string jsonStr = Encoding.UTF8.GetString(buffer);

                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var myScopedService = scope.ServiceProvider.GetRequiredService<BackgroundServicesDAO>();
                                    if (jsonStr.Contains("records"))
                                    {
                                        SensorReading sensorReading = new SensorReading(jsonStr);
                                        myScopedService.updateDatabase(sensorReading);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    await closeSocket(client);
                }
            }
        }


        private async Task closeSocket(ClientWebSocket client) {
            try
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                Console.WriteLine("Gracefully closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hard closed.");
                Console.WriteLine(ex.ToString());
            }
        }

        private void getUuids() {
            using (var scope = _serviceProvider.CreateScope())
            {
                var myScopedService = scope.ServiceProvider.GetRequiredService<BackgroundServicesDAO>();
                {
                    myScopedService.updateUuids();
                }
            }
        }

        private async  Task<ClientWebSocket> createClient(Uri uri) {
            var client = new ClientWebSocket();
            await client.ConnectAsync(uri, CancellationToken.None);

            if (client.State != WebSocketState.Open)
            {
                throw new Exception("WebSocket connection is not 'OPEN'");
            }
            return client;
        }
    }
}
