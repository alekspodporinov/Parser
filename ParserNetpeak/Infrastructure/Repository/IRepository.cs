using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParserNetpeak.Infrastructure.Repository
{
    /// <summary>
    ///     Интерфейс для реализации паттерна Repository
    /// </summary>
    /// <typeparam name="T">
    ///     T - должен быть классом
    /// </typeparam>
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetList();
        T Get(int id);
        void Create(T item);
        void CreateRange(IEnumerable<T> items);
        void Update(T item);
        void Delete(int id);
        void Save();
        Task SaveAsync();
    }
}