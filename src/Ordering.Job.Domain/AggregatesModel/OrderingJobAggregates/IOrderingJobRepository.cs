namespace Ordering.Job.Domain.AggregatesModel.OrderingJobAggregates;

public interface IOrderingJobRepository
{
    Task<OrderingJobState> GetAsync(string orderId);
    Task<List<OrderingJobState>> GetByStateAsync(ProcessState processState);
    Task<List<OrderingJobState>> GetByLockedByAsync(string lockedBy);
    Task Update(OrderingJobState orderingJobState);
}
