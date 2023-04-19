using IdentityModel.Client;
using irregation_api.Models.Response;

namespace irregation_api.BackgroundServices
{
    public class CredentialsController : BackgroundService
    {
        private readonly ValveClient _valveClient;

        public CredentialsController(ValveClient valveClient) { 
            _valveClient = valveClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TokenResponse token = await _valveClient.getAccessToken();
                await Task.Delay((token.ExpiresIn * 1000) - 10000);
            }
        }
    }
}
