using Moralar.Data.Enum;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;

namespace Moralar.Data.Entities
{
    public class LogAction : ModelBase
    {

        public TypeAction TypeAction { get; set; }
        public TypeResposible TypeResposible { get; set; }
        public LocalAction LocalAction { get; set; }
        public string Message { get; set; }
        public string ReferenceId { get; set; }
        public string ResponsibleId { get; set; }
        public string ResponsibleName { get; set; }
        public string Justification { get; set; }
        public string MessageEx { get; set; }
        public string StackTrace { get; set; }
        public string ClientIp { get; set; }

        public override string CollectionName => nameof(LogAction);
    }
}