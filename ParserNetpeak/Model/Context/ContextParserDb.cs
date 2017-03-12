using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserNetpeak.Model.Entity;

namespace ParserNetpeak.Model.Context
{
    /// <summary>
    ///     Контекст для подключения к бд SQLite
    /// </summary>
    public class ContextParserDb : DbContext
    {

        public ContextParserDb() : base("DefaultConnection")
        { }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
