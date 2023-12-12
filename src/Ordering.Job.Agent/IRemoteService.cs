using Ordering.Job.Domain.Messages;

namespace Ordering.Job.Agent
{
    public class RemoteServiceMock : IRemoteService
    {
        ILogger<RemoteServiceMock> _logger;

        public RemoteServiceMock(ILogger<RemoteServiceMock> logger)
        {
            _logger = logger;
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
                else
                {
                    _logger.LogInformation("注文ID:{OrderId}", request.OrderId);
                }
            }).ConfigureAwait(false);
            return true;
        }
    }
    public interface IRemoteService
    {
        Task<bool> PostOrderingInfoAsync(RequestModel request);
    }


    public class RemoteServiceException : Exception
    {
    }
}