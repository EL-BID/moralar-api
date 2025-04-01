using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;

namespace Moralar.WebApi
{
    internal class ProlongExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(7);
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = TimeSpan.FromDays(14);

        }
    }
}