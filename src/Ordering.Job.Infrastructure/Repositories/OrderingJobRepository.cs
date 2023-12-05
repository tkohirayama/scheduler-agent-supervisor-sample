
namespace Ordering.Job.Infrastructure.Repositories;
public class OrderingJobStateRepository : IOrderingJobStateRepository
{
    private readonly OrderingJobContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public OrderingJobStateRepository(OrderingJobContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<OrderingJobState> FindAsync(int orderId)
    {
        return await _context.OrderingJobStates
            .Where(o => o.Id == orderId)
            .SingleOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<OrderingJobState>> GetByLockedByAsync(string lockedBy)
    {
        return await _context.OrderingJobStates
            .Where(o => o.LockedBy == lockedBy)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<OrderingJobState>> FindByStateAsync(ProcessState processState)
    {
        return await _context.OrderingJobStates
            .Where(o => o.ProcessState == processState)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public OrderingJobState Update(OrderingJobState orderingJobState)
    {
        return _context.OrderingJobStates
                .Update(orderingJobState)
                .Entity;
    }
}
