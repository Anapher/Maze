using System;

namespace Tasks.Infrastructure.Core.Dtos
{
    public class TaskInfoDto
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int Commands { get; set; }
        public int TotalExecutions { get; set; }
        public bool IsActive { get; set; }
    }
}