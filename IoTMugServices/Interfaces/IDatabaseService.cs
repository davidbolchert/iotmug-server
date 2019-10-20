using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IoTMug.Services.Interfaces
{
    public enum AscDesc
    {
        Asc, Desc
    }

    public interface IDatabaseService
    {
        TCore GetFirstOrDefault<TCore>(Expression<Func<TCore, bool>> firstOrDefault) where TCore : class;
        IEnumerable<TCore> Find<TCore>(Func<TCore, bool> where) where TCore : class;
        IEnumerable<TCore> GetAll<TCore>() where TCore : class;
        IEnumerable<TCore> Get<TCore>(Expression<Func<TCore, bool>> @where = null, int? skip = null, int? limit = null, Func<IQueryable<TCore>, IQueryable<TCore>> includeProperties = null) where TCore : class;
        void Add<TCore>(TCore entity) where TCore : class;
        void Update<TCore>(TCore entity) where TCore : class;
    }
}
