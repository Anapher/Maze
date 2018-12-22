using FluentMigrator;

namespace Tasks.Infrastructure.Server.Migrations
{
    [Migration(20180911)]
    public class InitialCreate : Migration
    {
        public override void Up()
        {
            Create.Table("TaskReference")
                .WithColumn("TaskId").AsGuid().PrimaryKey()
                .WithColumn("IsCompleted").AsBoolean().NotNullable()
                .WithColumn("IsEnabled").AsBoolean().NotNullable()
                .WithColumn("AddedOn").AsInt64().NotNullable();

            Create.Table("TaskSession")
                .WithColumn("TaskSessionId").AsString()
                .WithColumn("TaskReferenceId").AsGuid().ForeignKey("TaskReference", "TaskId")
                .WithColumn("Description").AsString()
                .WithColumn("CreatedOn").AsInt64().NotNullable();
            Create.PrimaryKey().OnTable("TaskSession").Columns("TaskSessionId", "TaskReferenceId");

            Create.Table("TaskTransmission")
                .WithColumn("TaskTransmissionId").AsInt32().PrimaryKey()
                .WithColumn("TaskReferenceId").AsGuid().NotNullable()
                .WithColumn("TargetId").AsInt32().Nullable()
                .WithColumn("CreatedOn").AsInt64().NotNullable();

            Create.Table("TaskExecution")
                .WithColumn("TaskExecutionId").AsGuid().PrimaryKey()
                .WithColumn("TaskSessionId").AsString().NotNullable()
                .WithColumn("TaskReferenceId").AsGuid().NotNullable()
                .WithColumn("TargetId").AsInt32().Nullable()
                .WithColumn("CreatedOn").AsInt64().NotNullable();

            Create.ForeignKey().FromTable("TaskExecution").ForeignColumns("TaskSessionId", "TaskReferenceId").ToTable("TaskSession")
                .PrimaryColumns("TaskSessionId", "TaskReferenceId");

            Create.Table("CommandResult")
                .WithColumn("CommandResultId").AsGuid().PrimaryKey()
                .WithColumn("TaskExecutionId").AsGuid().ForeignKey("TaskExecution", "TaskExecutionId")
                .WithColumn("CommandName").AsString().NotNullable()
                .WithColumn("Result").AsString().NotNullable()
                .WithColumn("Status").AsInt32().Nullable()
                .WithColumn("FinishedAt").AsInt64().NotNullable();
        }

        public override void Down()
        {
        }
    }
}
