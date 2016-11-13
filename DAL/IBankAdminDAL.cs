using System.Collections.Generic;
using Models;

namespace DAL
{
    public interface IBankAdminDAL
    {
        
        bool adminEditAccount(Account account, string oldAccountNumber);
        bool adminEditCustomer(Customer customer); 
        bool adminRegisterCustomer(Customer inCustomer);
        
        List<Account> deleteAccount(string accountNumber);
        List<Customer> deleteCustomer(string nID);
        Account findAccount(string accNumber);
        Customer findCustomer(string nID);
        List<Customer> getAllCustomers();
        
        bool isAdminLoginCorrect(Admin admin);
        List<Account> newAccount(string nID);
    }
}