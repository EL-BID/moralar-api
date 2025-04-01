namespace UtilityFramework.Infra.Core.MongoDb.Data.Modelos
{
    public class Profile : ModelBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhotoFacebook { get; set; }
        public string PhotoProfile { get; set; }
        public override string CollectionName => nameof(Profile);
    }
}