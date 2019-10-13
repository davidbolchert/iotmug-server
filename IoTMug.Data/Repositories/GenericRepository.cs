using IoTMug.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IoTMug.Data.Repositories
{
    public class GenericRepository : IDatabaseService
    {
        private readonly IoTMugContext _context;

        public GenericRepository(IoTMugContext context) => _context = context;

        public void Add<TCore>(TCore entity) where TCore : class
        {
            _context.Set<TCore>().Add(entity);
            _context.SaveChanges();
        }

        public IEnumerable<TCore> Find<TCore>(Func<TCore, bool> where) where TCore : class
            => _context.Set<TCore>().Where(where);

        public IEnumerable<TCore> GetAll<TCore>() where TCore : class
            => _context.Set<TCore>().ToList();

        public TCore GetFirstOrDefault<TCore>(Expression<Func<TCore, bool>> id) where TCore : class
            => _context.Set<TCore>().FirstOrDefault(id);

        public void Update<TCore>(TCore entity) where TCore : class
        {
            _context.Attach(entity);
            _context.SaveChanges();
        }

        public IEnumerable<TCore> Get<TCore>(Expression<Func<TCore, bool>> @where = null, int? skip = null, int? limit = null, Func<IQueryable<TCore>, IQueryable<TCore>> includeProperties = null) where TCore : class
        {
            IQueryable<TCore> query = null;

            query = _context.Set<TCore>().AsQueryable();

            if (@where != null) query = query.Where(@where);

            if (skip.HasValue) query = query.Skip(skip.Value);

            if (limit.HasValue)
            {
                if (limit.Value != 0) query = query.Take(limit.Value);
                else return Enumerable.Empty<TCore>();
            }

            if (includeProperties != null) query = includeProperties?.Invoke(query);

            return query.ToList();
        }
    }
}
