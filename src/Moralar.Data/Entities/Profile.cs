using MongoDB.Bson.Serialization.Attributes;
using Moralar.Data.Enum;
using System.Collections.Generic;
using UtilityFramework.Infra.Core.MongoDb.Data.Modelos;


namespace Moralar.Data.Entities
{
    [BsonIgnoreExtraElements]
    public class Profile : ModelBase
    {
        public Profile()
        {
            //CreditCards = new List<string>();
            DeviceId = new List<string>();
        }

        public string Name { get; set; }
        public string JobPost { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        //public string Login { get; set; }
        public string Password { get; set; }
        public string LastPassword { get; set; }
        public TypeUserProfile TypeProfile { get; set; }
        public string ProviderId { get; set; }
        //public TypeProvider TypeProvider { get; set; }
        public string Reason { get; set; }
        //public string Photo { get; set; }

        /*USO DA IUGU*/
        //public string AccountKey { get; set; }
        //public string AccountKeyDev { get; set; }
        public List<string> DeviceId { get; set; }
        public override string CollectionName => nameof(Profile);
    }
}