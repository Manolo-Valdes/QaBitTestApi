using Microsoft.EntityFrameworkCore;
using QaBitTestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QaBitTestApi.Db
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ClientSuscription>  ClientSuscriptions { get; set; }
    }
}
