
namespace Moralar.UtilityFramework.Application.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct)]
    public class DataPropertie : Attribute
    {
        public string PropertieName { get; set; }

        public DataPropertie(string propertieName)
        {
            PropertieName = propertieName;
        }
    }
}
