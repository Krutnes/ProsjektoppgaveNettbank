using System.Collections.Generic;
using Models;

namespace BLL
{
    public interface AdminLogic
    {
        List<Account> adminDeleteAccount(string accountNumber);
        List<Customer> adminDeleteCustomer(string nID);
        bool adminEditAccount(Account account, string oldAccountNumber);
        bool adminEditCustomer(Customer customer);
        bool adminRegisterCustomer(Customer inCustomer);
        Account findAccount(string accNumber);
        Customer findCustomer(string nID);
        List<Customer> getAllCustomers();
        bool isAdminLoginCorrect(Admin admin);
        List<Account> newAccount(string nID);
    }
}