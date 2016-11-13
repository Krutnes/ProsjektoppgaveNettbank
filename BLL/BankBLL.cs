using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BLL
{
    public class BankCustomerBLL
    {
        public bool deletePayment(int id)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.deletePayment(id);
        }

        public bool editPayment(RegisteredPayment payment)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.editPayment(payment);
        }

        public RegisteredPayment findRegisteredPayment(int id)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.findRegisteredPayment(id);
        }

        public List<Account> getCustomerAccounts(string nid)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.getCustomerAccounts(nid);
        }

        public List<RegisteredPayment> getRegisteredPayments(string accountNumber)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.getRegisteredPayments(accountNumber);
        }

        public List<IssuedPayment> getIssuedPaymentsforOneAccount(string nID, int id)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.getIssuedPaymentsforOneAccount(nID, id);
        }

        public List<IssuedPayment> getIssuedPayments(string accountNumber)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.getIssuedPayments(accountNumber);
        }

        public string getRegisteredPaymentAccount(int id)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.getRegisteredPaymentAccount(id);
        }

        public bool isLoginCorrect(Customer customer)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.isLoginCorrect(customer);
        }

        public void populateDatabase()
        {
            BankCustomerDAL db = new BankCustomerDAL();
            db.populateDatabase();
        }

        public void populatePaymentTables()
        {
            BankCustomerDAL db = new BankCustomerDAL();
            db.populatePaymentTables();
        }

        public bool registerPayment(RegisteredPayment payment)
        {
            BankCustomerDAL db = new BankCustomerDAL();
            return db.registerPayment(payment);
        }

        public void updatePendingPayments()
        {
            BankCustomerDAL db = new BankCustomerDAL();
            db.updatePendingPayments();
        }

    }

    public class BankAdminBLL
    {
        public List<Customer> adminDeleteCustomer(string nID)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.deleteCustomer(nID);
        }

        public List<Account> adminDeleteAccount(string accountNumber)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.deleteAccount(accountNumber);
        }

        public bool adminEditAccount(Account account, string oldAccountNumber)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.adminEditAccount(account, oldAccountNumber);
        }

        public bool adminEditCustomer(Customer customer)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.adminEditCustomer(customer);
        }

        public Account findAccount(string accNumber)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.findAccount(accNumber);
        }

        public bool adminRegisterCustomer(Customer inCustomer)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.adminRegisterCustomer(inCustomer);
        }

        public Customer findCustomer(string nID)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.findCustomer(nID);
        }

        public List<Customer> getAllCustomers()
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.getAllCustomers();
        }

        public bool isAdminLoginCorrect(Admin admin)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.isAdminLoginCorrect(admin);
        }

        public List<Account> newAccount(string nID)
        {
            BankAdminDAL db = new BankAdminDAL();
            return db.newAccount(nID);
        }
    }
}
