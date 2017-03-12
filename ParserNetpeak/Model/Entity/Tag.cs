using System.Data.Linq.Mapping;

namespace ParserNetpeak.Model.Entity
{
    [Table(Name = "Tags")]
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InnerHtml { get; set; }
        public int PageId { get; set; }
    }
}