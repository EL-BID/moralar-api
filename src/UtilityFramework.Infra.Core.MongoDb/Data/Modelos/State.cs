namespace UtilityFramework.Infra.Core.MongoDb.Data.Modelos
{
    public class State : ModelBase
    {
        public string Name { get; set; }
        public string Uf { get; set; }
        public override string CollectionName => nameof(State);

    }
}