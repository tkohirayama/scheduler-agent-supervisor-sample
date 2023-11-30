using System.Diagnostics;

namespace Ordering.Job.Agent
{
    public interface IRemoteService
    {
        Task PostOrderingInfoAsync(string data);
    }

    public class RemoteServiceMock : IRemoteService
    {
        private readonly double _availabilityRate;
        public RemoteServiceMock(IConfiguration configuration)
        {
            _availabilityRate = configuration.GetValue("RemoteServiceMock:AvailabilityRate", 100);
        }

        public async Task PostOrderingInfoAsync(string data)
        {
            await Task.Delay(1000).ConfigureAwait(false);

            Random r = new();
            int rInt = r.Next(0, 100);
            if((int)(rInt / _availabilityRate) > 0)
            {
                // まれに失敗
                throw new Exception();
            }
        }
    }
}