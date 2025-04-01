using System;

namespace UtilityFramework.Infra.Core.MongoDb.Data
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.All)]
    public class MongoIndex : Attribute { }
}