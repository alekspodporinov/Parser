using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using ParserNetpeak.Model.Context;
using ParserNetpeak.Model.Entity;

namespace ParserNetpeak.Infrastructure.Repository
{
    /// <summary>
    ///     Реализация Repository для объекта Tag
    /// </summary>
    public class TagRepository : IRepository<Tag>
    {
        private readonly ContextParserDb _db;

        private bool _disposed;

        public TagRepository()
        {
            _db = new ContextParserDb();
        }

        public IEnumerable<Tag> GetList()
        {
            return _db.Tags;
        }

        public Tag Get(int id)
        {
            return _db.Tags.Find(id);
        }

        public void Create(Tag tag)
        {
            _db.Tags.Add(tag);
        }

        public void CreateRange(IEnumerable<Tag> items)
        {
            _db.Tags.AddRange(items);
        }

        public void Update(Tag tag)
        {
            _db.Entry(tag).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var tag = _db.Tags.Find(id);
            if (tag != null)
                _db.Tags.Remove(tag);
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