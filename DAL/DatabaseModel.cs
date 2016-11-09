using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ProsjektoppgaveNettbank.Models
{
    public class DbAdmin
    {
        public string ID { get; set; }
        public byte[] adminpassword { get; set; }
        public string adminsalt { get; set; }
    }

    public class DbCustomer
    {
        public string NID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public byte[] password { get; set; }
        public string salt { get; set; }
        public List<DbAccount> accounts { get; set; }
    }

    public class DbAccount
    {
        public string accountNumber { get; set; }
        public double balance { get; set; }
        public string NID { get; set; }
        public virtual DbCustomer customer { get; set; }
        public virtual List<DbRegisteredPayment> registeredPayments { get; set; }
        public virtual List<DbIssuedPayment> issuedPayments { get; set; }
    }

    public class DbRegisteredPayment
    {
        [Key]
        public int id { get; set; }
        public string accountNumberFrom { get; set; }
        public string accountNumberTo { get; set; }
        public string receiverName { get; set; }
        public double amount { get; set; }

        [Column(TypeName = "date")]
        public DateTime paymentDate { get; set; }
        public virtual DbAccount fromAccount { get; set; }
    }

    public class DbIssuedPayment
    {
        [Key]
        public int id { get; set; }
        public string accountNumberFrom { get; set; }
        public string accountNumberTo { get; set; }
        public string receiverName { get; set; }
        public double amount { get; set; }

        [Column(TypeName = "date")]
        public DateTime issuedDate { get; set; }
        public virtual DbAccount account { get; set; }
    }

    public class DbAccountChangesLog
    {
        public int NID { get; set; }
        public DateTime date { get; set; }
        public bool isInboundPayment { get; set; }
        public string accountNumberIn { get; set; }
        public string accountNumberOut { get; set; }
    }

    public class BankDBContext : DbContext
    {
        public BankDBContext()
            : base("name=BankDatabase")
        {
            Database.CreateIfNotExists();
        }
        public DbSet<DbAdmin> Admins { get; set; }
        public DbSet<DbCustomer> Customers { get; set; }
        public DbSet<DbAccount> Accounts { get; set; }
        public DbSet<DbRegisteredPayment> RegisteredPayments { get; set; }
        public DbSet<DbIssuedPayment> IssuedPayments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCustomer>().HasKey(s => s.NID);
            modelBuilder.Entity<DbAccount>().HasKey(s => s.accountNumber);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}