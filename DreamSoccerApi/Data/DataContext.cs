using Microsoft.EntityFrameworkCore;

namespace DreamSoccerApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}