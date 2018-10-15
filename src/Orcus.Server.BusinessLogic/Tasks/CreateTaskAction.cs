using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using CodeElements.BizRunner;
using CodeElements.BizRunner.Generic;
using Orcus.Server.Connection.Tasks;
using Orcus.Server.Data.EfClasses.Tasks;

namespace Orcus.Server.BusinessLogic.Tasks
{
    public interface ICreateTaskAction : IGenericActionWriteDbAsync<OrcusTask, TaskReference>
    {
    }

    public class CreateTaskAction : BusinessActionErrors, ICreateTaskAction
    {
        public Task<TaskReference> BizActionAsync(OrcusTask inputData)
        {
            AddValidationResults(inputData.Validate(new ValidationContext(inputData)));
            if (HasErrors)
                return null;


        }
    }
}