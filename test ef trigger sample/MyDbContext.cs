using EntityFramework.Triggers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_ef_trigger_sample
{
    public class MyDbContext : DbContextWithTriggers
    {
        public MyDbContext() : base("myConnectStr")
        {
        }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
    }

    public class Order
    {
        public Order()
        {
            Triggers<Order>.Updating += x =>
            {
                try
                {
                    var a = x.Original;
                    var b = x.Original.Customer;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            };
        }

        public int Id { get; set; }
        public string Customer { get; set; }

        public IEnumerable<OrderDetail> Details1Error { get; set; }     //if delete this 2 lines, it ok no error
        public ICollection<OrderDetail> Details2Error { get; set; }     //but I want to use this.
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public virtual Order Order { get; set; }
    }

    public class OrderRepo : test_ef_trigger_sample.MyRepo.RepositoryBase<Order>
    {
        public MyDbContext context { get; set; }

        public OrderRepo(MyDbContext context) : base(context)
        {
            this.context = context;
        }
    }

    public class OrderDetailRepo : test_ef_trigger_sample.MyRepo.RepositoryBase<OrderDetail>
    {
        public MyDbContext context { get; set; }

        public OrderDetailRepo(MyDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}
