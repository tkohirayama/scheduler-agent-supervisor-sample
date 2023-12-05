namespace Ordering.Job.Domain.AggregatesModel.OrderingJobAggregates;

public interface IOrderingJobStateRepository : IRepository<OrderingJobState>
{
    Task<OrderingJobState> FindAsync(int orderId);
    Task<List<OrderingJobState>> FindByStateAsync(ProcessState processState);
    Task<List<OrderingJobState>> GetByLockedByAsync(string lockedBy);
    OrderingJobState Update(OrderingJobState orderingJobState);
}
