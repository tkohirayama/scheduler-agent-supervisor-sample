namespace Ordering.Job.Infrastructure.EntityConfigurations;

class OrderingJobStateEntityTypeConfiguration : IEntityTypeConfiguration<OrderingJobState>
{
    public void Configure(EntityTypeBuilder<OrderingJobState> orderConfiguration)
    {
        orderConfiguration.ToTable("ordering_job_state");

        orderConfiguration.Ignore(b => b.DomainEvents);

        orderConfiguration
            .Property(o => o.Id)
            .HasColumnName("order_id");

        orderConfiguration
            .Property(o => o.LockedBy)
            .HasColumnName("locked_by");

        orderConfiguration
            .Property(o => o.LockedBy)
            .HasColumnName("OrderStatusId");

        orderConfiguration
            .Property(o => o.CompleteBy)
            .HasColumnName("complete_by");

        orderConfiguration
            .Property(o => o.ProcessState)
            .HasColumnName("process_state");

        orderConfiguration
            .Property(o => o.FailureCount)
            .HasColumnName("failure_count");
    }
}
