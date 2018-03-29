﻿using HoveyTech.Core.Services;

namespace HoveyTech.Core.EfCore
{
    public abstract class BaseService : BaseService<IHasQueryableTransaction, IQueryableTransaction>
    {

    }
}
