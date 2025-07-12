using Microsoft.EntityFrameworkCore;

namespace Repository;
public class RepositoryContext(DbContextOptions options) : DbContext(options)
{
}
