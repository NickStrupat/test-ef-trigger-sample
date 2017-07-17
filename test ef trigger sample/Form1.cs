using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_ef_trigger_sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TestTrigger();
        }

        public void TestTrigger()
        {
            using (var myDB = new MyDbContext())
            {
                var n1 = new Order()
                {
                    Id = 1,
                    Customer = "cus"
                };
                myDB.Order.Add(n1);
                var n2 = new OrderDetail()
                {
                    Id = 1,
                    Order = n1
                };
                myDB.OrderDetail.Add(n2);
                myDB.SaveChanges();
             
                var m = myDB.OrderDetail.FirstOrDefault();
                m.Order.Customer = "cusMod" + DateTime.Now.ToString();
                var myOrderDetail = new OrderDetailRepo(myDB);
                myOrderDetail.Update(m);

                

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
