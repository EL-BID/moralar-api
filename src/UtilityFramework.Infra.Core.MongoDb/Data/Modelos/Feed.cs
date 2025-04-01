namespace UtilityFramework.Infra.Core.MongoDb.Data.Modelos
{
    public class Feed : ModelBase
    {
        public string ProfileId { get; set; }
        public int Type { get; set; }
        public string ReferenceId { get; set; }

        public override string CollectionName => nameof(Feed);
    }
}