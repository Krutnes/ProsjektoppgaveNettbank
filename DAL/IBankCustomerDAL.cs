using System.Collections.Generic;
using Models;

namespace DAL
{
    public interface IBankCustomerDAL
    {
        bool deletePayment(int id);
        bool editPayment(RegisteredPayment payment);
        RegisteredPayment findRegisteredPayment(int id);
        List<Account> getCustomerAccounts(string nid);
        List<IssuedPayment> getIssuedPayments(string accountNumber);
        string getRegisteredPaymentAccount(int id);
        List<RegisteredPayment> getRegisteredPayments(string accountNumber);
        bool isLoginCorrect(Customer customer);
        void populateDatabase();
        void populatePaymentTables();
        void processRegisteredPayment(DbRegisteredPayment payment);
        bool registerPayment(RegisteredPayment payment);
        void updatePendingPayments();
    }
}