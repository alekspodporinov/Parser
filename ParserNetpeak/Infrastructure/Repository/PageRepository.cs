using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using ParserNetpeak.Model.Context;
using ParserNetpeak.Model.Entity;

namespace ParserNetpeak.Infrastructure.Repository
{
    /// <summary>
    ///     Реализация Repository для объекта Page
    /// </summary>
    public class PageRepository : IRepository<Page>
    {
        private readonly ContextParserDb _db;

        private bool _disposed;

        public PageRepository()
        {
            _db = new ContextParserDb();
        }

        public IEnumerable<Page> GetList()
        {
            return _db.Pages.Include(p => p.Tags);
        }

        public Page Get(int id)
        {
            return _db.Pages.Find(id);
        }

        public void Create(Page page)
        {
            _db.Pages.Add(page);
        }

        public void CreateRange(IEnumerable<Page> items)
        {
            _db.Pages.AddRange(items);
        }

        public void Update(Page page)
        {
            _db.Entry(page).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var page = _db.Pages.Find(id);
            if (page != null)
                _db.Pages.Remove(page);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            _disposed = true;
        }
    }
}