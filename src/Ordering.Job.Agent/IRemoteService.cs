using System.Diagnostics;
using Ordering.Job.Domain.Messages;

namespace Ordering.Job.Agent
{
    public interface IRemoteService
    {
        Task<bool> PostOrderingInfoAsync(RequestModel request);
    }

    public class RemoteServiceMock : IRemoteService
    {
        private readonly double _availabilityRate;
        public RemoteServiceMock(IConfiguration configuration)
        {
            // _availabilityRate = configuration.GetValue("RemoteServiceMock:AvailabilityRate", 100);
        }

        public async Task<bool> PostOrderingInfoAsync(RequestModel request)
        {
            await Task.Run(() =>
            {
                if (request.OrderId % 3 == 0)
                {
                    // 3の倍数の注文IDはエラーとする
                    throw new RemoteServiceException();
                }
            }).ConfigureAwait(false);
            return true;
        }
    }

    public class RemoteServiceException : Exception
    {
    }
}