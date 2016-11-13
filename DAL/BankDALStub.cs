using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    public class BankDALStub : DAL.IBankAdminDAL 
    {
        public bool adminEditAccount(Account account, string AccountNumber)
        {
            if (AccountNumber == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool adminEditCustomer(Customer customer)
        {
            if (customer == null)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public bool adminRegisterCustomer(Customer inCustomer)
        {
            if(inCustomer.firstName == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public List<Account> deleteAccount(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public List<Customer> deleteCustomer(string nID)
        {
            throw new NotImplementedException();
        }

        public Account findAccount(string accNumber)
        {
            throw new NotImplementedException();
        }

        public Customer findCustomer(string nID)
        {
            throw new NotImplementedException();
        }

        public string generateBankAccountNumber()
        {
            throw new NotImplementedException();
        }

        public List<Customer> getAllCustomers()
        {
            throw new NotImplementedException();
        }

        public bool isAdminLoginCorrect(Admin admin)
        {
            if (admin.ID.Equals("1111") && admin.adminPassword.Equals("admin") )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Account> newAccount(string nID)
        {
            throw new NotImplementedException();
        }
    }
}
