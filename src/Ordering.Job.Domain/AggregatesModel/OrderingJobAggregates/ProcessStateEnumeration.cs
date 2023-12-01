namespace Ordering.Job.Domain.AggregatesModel.OrderingJobAggregates;

public class ProcessState : Enumeration
{
    public static ProcessState Pending = new ProcessState(0, nameof(Pending).ToLowerInvariant());
    public static ProcessState Processing = new ProcessState(1, nameof(Processing).ToLowerInvariant());
    public static ProcessState Processed = new ProcessState(10, nameof(Processed).ToLowerInvariant());
    public static ProcessState Error = new ProcessState(20, nameof(Error).ToLowerInvariant());

    public ProcessState(int id, string name)
        : base(id, name)
    {
    }

    public static IEnumerable<ProcessState> List()
    {
        return new[] { Pending, };
    }
}
