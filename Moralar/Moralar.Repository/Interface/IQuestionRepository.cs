﻿using Moralar.Data.Entities;
using UtilityFramework.Infra.Core.MongoDb.Business;

namespace Moralar.Repository.Interface
{
    public interface IQuestionRepository : IBusinessBaseAsync<Question>
    {

    }
}