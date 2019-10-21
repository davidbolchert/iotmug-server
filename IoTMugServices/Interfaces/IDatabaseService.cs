using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        Task AddAsync<TCore>(TCore entity) where TCore : class;
        Task UpdateAsync<TCore>(TCore entity) where TCore : class;

        Task DeleteAsync<TCore>(TCore entity) where TCore : class;
    }
}
