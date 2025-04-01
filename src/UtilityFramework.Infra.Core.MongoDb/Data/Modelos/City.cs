namespace UtilityFramework.Infra.Core.MongoDb.Data.Modelos
{
    public class City : ModelBase
    {
        public string Name { get; set; }
        public string StateId { get; set; }
        public string StateName { get; set; }
        public override string CollectionName => nameof(City);
    }
}