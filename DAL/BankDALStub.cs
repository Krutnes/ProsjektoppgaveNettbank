using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.IO;

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
            if (admin.ID.Equals("1111") && admin.adminPassword.Equals("admin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IBankAdminDAL.adminEditAccount(Account account, string oldAccountNumber)
        {
            throw new NotImplementedException();
        }

        bool IBankAdminDAL.adminEditCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        bool IBankAdminDAL.adminRegisterCustomer(Customer inCustomer)
        {
            throw new NotImplementedException();
        }

        List<Account> IBankAdminDAL.deleteAccount(string accountNumber)
        {
            throw new NotImplementedException();
        }

        List<Customer> IBankAdminDAL.deleteCustomer(string nID)
        {
            if (nID == null)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        Customer IBankAdminDAL.findCustomer(string nID)
        {
            throw new NotImplementedException();
        }

        string IBankAdminDAL.generateBankAccountNumber()
        {
            throw new NotImplementedException();
        }

        List<Customer> IBankAdminDAL.getAllCustomers()
        {
            throw new NotImplementedException();
        }

        bool IBankAdminDAL.isAdminLoginCorrect(Admin admin)
        {
            throw new NotImplementedException();
        }

        List<Account> IBankAdminDAL.newAccount(string nID)
        {
            throw new NotImplementedException();
        }

        /*  public bool errorReport(string e)
          {
              if (new FileInfo("ErrorLog.txt").Length == 0)
              {
                  return false;
              }
              else
                  return true;
          } */




    }
}
