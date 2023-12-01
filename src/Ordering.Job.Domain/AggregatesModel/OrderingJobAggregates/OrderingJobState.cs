namespace Ordering.Job.Domain.AggregatesModel.OrderingJobAggregates;

public class OrderingJobState : Entity, IAggregateRoot
{
    public string? LockedBy { get; set; }
    public DateTimeOffset CompleteBy { get; set; }
    public ProcessState ProcessState { get; set; }
    public int FailureCount { get; set; }
}
