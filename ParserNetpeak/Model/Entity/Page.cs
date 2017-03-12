using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace ParserNetpeak.Model.Entity
{
    [Table(Name = "Pages")]
    public class Page
    {
        public Page()
        {
            Tags = new List<Tag>();
        }

        public int Id { get; set; }
        public string PageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ServerResponseCode { get; set; }
        public string ServerResponseTime { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}