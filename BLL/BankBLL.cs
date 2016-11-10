﻿using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BankBLL
    {
        public List<Account> getCustomerAccounts(string nid)
        {
            BankDAL db = new BankDAL();
            return db.getCustomerAccounts(nid);
        }

        public List<RegisteredPayment> getRegisteredPayments(string accountNumber)
        {
            BankDAL db = new BankDAL();
            return db.getRegisteredPayments(accountNumber);
        }

        public List<IssuedPayment> getIssuedPayments(string accountNumber)
        {
            BankDAL db = new BankDAL();
            return db.getIssuedPayments(accountNumber);
        }

        public RegisteredPayment findRegisteredPayment(int id)
        {
            BankDAL db = new BankDAL();
            return db.findRegisteredPayment(id);
        }

        public bool editPayment(RegisteredPayment payment)
        {
            BankDAL db = new BankDAL();
            return db.editPayment(payment);
        }

        public bool deletePayment(int id)
        {
            BankDAL db = new BankDAL();
            return db.deletePayment(id);
        }

        public string getRegisteredPaymentAccount(int id)
        {
            BankDAL db = new BankDAL();
            return db.getRegisteredPaymentAccount(id);
        }

        public bool registerPayment(RegisteredPayment payment)
        {
            BankDAL db = new BankDAL();
            return db.registerPayment(payment);
        }

        public bool isLoginCorrect(Customer customer)
        {
            BankDAL db = new BankDAL();
            return db.isLoginCorrect(customer);
        }     

        public void populateDatabase()
        {
            BankDAL db = new BankDAL();
            db.populateDatabase();
        }

        public void populatePaymentTables()
        {
            BankDAL db = new BankDAL();
            db.populatePaymentTables();
        }

        public Customer findCustomer(string nID)
        {
            BankDAL db = new BankDAL();
            return db.findCustomer(nID);
        }

        public bool isAdminLoginCorrect(Admin admin)
        {
            BankDAL db = new BankDAL();
            return db.isAdminLoginCorrect(admin);
        }

        public List<Customer> getAllCustomers()
        {
            BankDAL db = new BankDAL();
            return db.getAllCustomers();
        }

        public bool deleteCustomer(string nID)
        {
            BankDAL db = new BankDAL();
            return db.deleteCustomer(nID);
        }
    }

}