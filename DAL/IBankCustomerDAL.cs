using System.Collections.Generic;
using Models;

namespace DAL
{
    public interface IBankCustomerDAL
    {
        bool deletePayment(int id);
        bool editPayment(RegisteredPayment payment);
        //void errorReport(string e);
        RegisteredPayment findRegisteredPayment(int id);
        //byte[] generateHash(string clearTextPassword);
        //string generateSalt();
        List<Account> getCustomerAccounts(string nid);
        List<IssuedPayment> getIssuedPayments(string accountNumber);
        List<IssuedPayment> getIssuedPaymentsforOneAccount(string accountNumberID);
        string getRegisteredPaymentAccount(int id);
        List<RegisteredPayment> getRegisteredPayments(string accountNumber);
        bool isLoginCorrect(Customer customer);
        void populateDatabase();
        void populatePaymentTables();
        void processRegisteredPayment(int id);
        bool registerDirectPayment(IssuedPayment payment);
        bool registerPayment(RegisteredPayment payment);
        void updatePendingPayments();
    }
}