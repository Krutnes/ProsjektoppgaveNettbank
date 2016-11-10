using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace DAL
{
    public class BankDAL
    {
        public List<Account> getCustomerAccounts(string nid)
        {
            using (var db = new BankDBContext())
            {
                List<Account> accounts = new List<Account>();
                List<DbAccount> dbAccounts = db.Accounts.Where(a => a.NID.Equals(nid)).ToList();
                foreach(DbAccount dba in dbAccounts)
                {
                    accounts.Add(new Account()
                    {
                        accountNumber = dba.accountNumber,
                        balance = dba.balance
                    });
                }
                return accounts;
            }
        }

        public List<RegisteredPayment> getRegisteredPayments(string accountNumber)
        {
            using (var db = new BankDBContext())
            {
                List<RegisteredPayment> registeredPayments = new List<RegisteredPayment>();
                IEnumerable<DbRegisteredPayment> dbRegisteredPayments = db.RegisteredPayments.
                    Where(a => a.accountNumberFrom.Equals(accountNumber)).ToList();
                foreach (DbRegisteredPayment regPayment in dbRegisteredPayments)
                {
                    registeredPayments.Add(new RegisteredPayment()
                    {
                        id = regPayment.id,
                        accountNumberFrom = regPayment.accountNumberFrom,
                        accountNumberTo = regPayment.accountNumberTo,
                        amount = regPayment.amount,
                        paymentDate = regPayment.paymentDate,
                        receiverName = regPayment.receiverName
                    });
                }
                return registeredPayments;
            }
        }

        public List<IssuedPayment> getIssuedPayments(string accountNumber)
        {
            using (var db = new BankDBContext())
            {
                List<IssuedPayment> issuedPayments = new List<IssuedPayment>();
                IEnumerable<DbIssuedPayment> dbIssuedPayments = db.IssuedPayments.
                    Where(a => a.accountNumberFrom.Equals(accountNumber)).ToList();
                foreach (DbIssuedPayment issuedPayment in dbIssuedPayments)
                {
                    issuedPayments.Add(new IssuedPayment()
                    {
                        accountNumberFrom = issuedPayment.accountNumberFrom,
                        accountNumberTo = issuedPayment.accountNumberTo,
                        amount = issuedPayment.amount,
                        issuedDate = issuedPayment.issuedDate,
                        receiverName = issuedPayment.receiverName
                    });
                }
                return issuedPayments;
            }
        }

        public RegisteredPayment findRegisteredPayment(int id)
        {
            using (var db = new BankDBContext())
            {
                DbRegisteredPayment dbPayment = db.RegisteredPayments.FirstOrDefault(
                    p => p.id == id);
                if (dbPayment == null)
                    return null;
                RegisteredPayment rp = new RegisteredPayment()
                {
                    id = dbPayment.id,
                    accountNumberFrom = dbPayment.accountNumberFrom,
                    accountNumberTo = dbPayment.accountNumberTo,
                    amount = dbPayment.amount,
                    paymentDate = dbPayment.paymentDate,
                    receiverName = dbPayment.receiverName
                };
                return rp;
            }
        }

        public bool editPayment(RegisteredPayment payment)
        {
            using (var db = new BankDBContext())
            {
                DbRegisteredPayment regPayment = db.RegisteredPayments.
                    FirstOrDefault(rp => rp.id == payment.id);
                if (regPayment != null)
                {
                    regPayment.accountNumberTo = payment.accountNumberTo;
                    regPayment.receiverName = payment.receiverName;
                    regPayment.amount = payment.amount;
                    regPayment.paymentDate = payment.paymentDate;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool deletePayment(int id)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbRegisteredPayment payment = db.RegisteredPayments.Find(id);
                    db.RegisteredPayments.Remove(payment);
                    db.SaveChanges();
                    // ADD LOG ENTRY
                    return true;
                }
                catch(Exception e)
                {
                    // LAGRE TIL FIL
                    return false;
                }
                
            }
        }

        public string getRegisteredPaymentAccount(int id)
        {
            using(var db = new BankDBContext())
            {
                return db.RegisteredPayments.FirstOrDefault(p => p.id == id).fromAccount.accountNumber;
            }
        }

        public bool registerPayment(RegisteredPayment payment)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    db.RegisteredPayments.Add(new DbRegisteredPayment()
                    {
                        accountNumberFrom = payment.accountNumberFrom,
                        accountNumberTo = payment.accountNumberTo,
                        amount = payment.amount,
                        fromAccount = db.Accounts.Find(payment.accountNumberFrom),
                        paymentDate = payment.paymentDate,
                        receiverName = payment.receiverName
                    });
                    db.SaveChanges();
                    // ADD LOG ENTRY
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool isLoginCorrect(Customer customer)
        {
            using (var db = new BankDBContext())
            {
                DbCustomer customerFound = db.Customers.FirstOrDefault(c => c.NID.Equals(customer.nID));
                if (customerFound != null)
                {
                    byte[] checkPassword = generateHash(customer.password + customerFound.salt);
                    bool validCustomer = customerFound.password.SequenceEqual(checkPassword);
                    return validCustomer;
                }
                return false;
            }
        }

        private byte[] generateHash(string clearTextPassword)
        {
            byte[] inData, outData;
            var algorithm = System.Security.Cryptography.SHA512.Create();
            inData = System.Text.Encoding.ASCII.GetBytes(clearTextPassword);
            outData = algorithm.ComputeHash(inData);
            return outData;
        }

        private string generateSalt()
        {
            byte[] randomArray = new byte[10];
            string randomString;

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomArray);
            randomString = Convert.ToBase64String(randomArray);
            return randomString;
        }

        public bool isAdminLoginCorrect(Admin admin)
        {
            using (var db = new BankDBContext())
            {
                DbAdmin adminFound = db.Admins.FirstOrDefault(c => c.ID.Equals(admin.ID));
                if (adminFound != null)
                {
                    byte[] checkPassword = generateHash(admin.adminPassword + adminFound.adminsalt);
                    bool validAdmin = adminFound.adminpassword.SequenceEqual(checkPassword);
                    return validAdmin;
                }
                return false;
            }
        }

        public List<Customer> getAllCustomers()
        {
            using (var db = new BankDBContext())
            {
                List<Customer> alleKunder = db.Customers.Select(k => new Customer()
                {
                    nID = k.NID,
                    firstName = k.firstName,
                    lastName = k.lastName
                }).ToList();
                return alleKunder;
            }
        }

        public List<Customer> deleteCustomer(string nID)
        {
            //System.Diagnostics.Debug.WriteLine("DAL PARAMETER TEST: " + nID);
            var db = new BankDBContext();
            try
            {
                DbCustomer deleteCustomer = db.Customers.FirstOrDefault(pk => pk.NID.Equals(nID));
                if (deleteCustomer == null)
                {
                    return null;
                }

                IEnumerable<DbAccount> accounts = db.Accounts.Where(a => a.NID.Equals(nID)).ToList();
                IEnumerable<DbRegisteredPayment> registeredPayments;
                IEnumerable<DbIssuedPayment> issuedPayments;
                foreach (DbAccount account in accounts)
                {
                    registeredPayments = db.RegisteredPayments.Where(rp => rp.accountNumberFrom.Equals(account.accountNumber)).ToList();
                    issuedPayments = db.IssuedPayments.Where(ip => ip.accountNumberFrom.Equals(account.accountNumber)).ToList();
                    foreach (DbRegisteredPayment rp in registeredPayments)
                    {
                        db.RegisteredPayments.Remove(rp);
                    }
                    foreach (DbIssuedPayment ip in issuedPayments)
                    {
                        db.IssuedPayments.Remove(ip);
                    }
                    db.Accounts.Remove(account);
                }

                db.Customers.Remove(deleteCustomer);
                db.SaveChanges();
                return db.Customers.Select(c => new Customer()
                    {
                        nID = c.NID,
                        firstName = c.firstName,
                        lastName = c.lastName
                    })
                .ToList();
            }
            catch (Exception feil)
            {
                System.Diagnostics.Debug.WriteLine("DB ERROR: " + feil.ToString());
                return null;
            }
        }

        public Customer findCustomer(string nID)
        {
            var db = new BankDBContext();

            var foundCustomer = db.Customers.FirstOrDefault(pk => pk.NID.Equals(nID));

            if (foundCustomer == null)
            {
                return null;
            }
            else
            {
                var customer = new Customer()
                {
                    nID = foundCustomer.NID,
                    firstName = foundCustomer.firstName,
                    lastName = foundCustomer.lastName
                };
                return customer;
            }
        }

        public void populateDatabase()
        {
            var db = new BankDBContext();
            
            // Admin 
            DbAdmin dbAdmin1 = new DbAdmin
            {
                ID = "1111"
            };
            string adminSalt = generateSalt();
            string adminpasswordAndSalt = "admin" + adminSalt;
            byte[] adminhashedPassword = generateHash(adminpasswordAndSalt);
            dbAdmin1.adminpassword = adminhashedPassword;
            dbAdmin1.adminsalt = adminSalt;

            // Customer 1
            DbCustomer dbCustomer1 = new DbCustomer
            {
                firstName = "Hillary",
                lastName = "Clinton",
                NID = "24126712345"
            };
            string salt = generateSalt();
            string passwordAndSalt = "What emails?" + salt;
            byte[] hashedPassword = generateHash(passwordAndSalt);
            dbCustomer1.password = hashedPassword;
            dbCustomer1.salt = salt;

            var cust1Accounts = new List<DbAccount>();
            DbAccount account1_1 = new DbAccount
            {
                NID = "24126712345",
                accountNumber = "05396666666",
                balance = 69000.49,
                customer = dbCustomer1,
                issuedPayments = new List<DbIssuedPayment>(),
                registeredPayments = new List<DbRegisteredPayment>()
            };
            cust1Accounts.Add(account1_1);
            dbCustomer1.accounts = cust1Accounts;

            // Customer 2
            DbCustomer dbCustomer2 = new DbCustomer
            {
                firstName = "Kjell Inge",
                lastName = "Røkke",
                NID = "01026622334"
            };
            salt = generateSalt();
            passwordAndSalt = "rekerFTW" + salt;
            hashedPassword = generateHash(passwordAndSalt);
            dbCustomer2.password = hashedPassword;
            dbCustomer2.salt = salt;

            var cust2Accounts = new List<DbAccount>();
            DbAccount account2_1 = new DbAccount
            {
                NID = "01026622334",
                accountNumber = "05397777777",
                balance = 5000000.00,
                customer = dbCustomer2,
                issuedPayments = new List<DbIssuedPayment>(),
                registeredPayments = new List<DbRegisteredPayment>()
            };

            DbAccount account2_2 = new DbAccount
            {
                NID = "01026622334",
                accountNumber = "05398888888",
                balance = 49363.00,
                customer = dbCustomer2,
                issuedPayments = new List<DbIssuedPayment>(),
                registeredPayments = new List<DbRegisteredPayment>()
            };
            cust2Accounts.Add(account2_1);
            cust2Accounts.Add(account2_2);
            dbCustomer2.accounts = cust2Accounts;

            // Customer 3
            DbCustomer dbCustomer3 = new DbCustomer
            {
                firstName = "Donald",
                lastName = "Trump",
                NID = "14064634567"
            };
            salt = generateSalt();
            passwordAndSalt = "CrookedHillary4prison" + salt;
            hashedPassword = generateHash(passwordAndSalt);
            dbCustomer3.password = hashedPassword;
            dbCustomer3.salt = salt;

            var cust3Accounts = new List<DbAccount>();
            DbAccount account3_1 = new DbAccount
            {
                NID = "14064634567",
                accountNumber = "05399999999",
                balance = 20000000000.99,
                customer = dbCustomer3,
                issuedPayments = new List<DbIssuedPayment>(),
                registeredPayments = new List<DbRegisteredPayment>()
            };

            DbAccount account3_2 = new DbAccount
            {
                NID = "14064634567",
                accountNumber = "05390000000",
                balance = 390000000.49,
                customer = dbCustomer3,
                issuedPayments = new List<DbIssuedPayment>(),
                registeredPayments = new List<DbRegisteredPayment>()
            };

            DbAccount account3_3 = new DbAccount
            {
                NID = "14064634567",
                accountNumber = "05391111111",
                balance = 127000.49,
                customer = dbCustomer3,
                issuedPayments = new List<DbIssuedPayment>(),
                registeredPayments = new List<DbRegisteredPayment>()
            };
            cust3Accounts.Add(account3_1);
            cust3Accounts.Add(account3_2);
            cust3Accounts.Add(account3_3);
            dbCustomer3.accounts = cust3Accounts;

            try
            {
                db.Admins.Add(dbAdmin1);
                db.Customers.Add(dbCustomer1);
                db.Customers.Add(dbCustomer2);
                db.Customers.Add(dbCustomer3);
                db.SaveChanges();

                db.Accounts.Add(account1_1);
                db.Accounts.Add(account2_1);
                db.Accounts.Add(account2_2);
                db.Accounts.Add(account3_1);
                db.Accounts.Add(account3_2);
                db.Accounts.Add(account3_3);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Feil i DB");
            }
            populatePaymentTables();
        }

        public void populatePaymentTables()
        {
            BankDBContext db = new BankDBContext();
            DbAccount trumpAccount = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05390000000"));
            if (trumpAccount == null)
                return;
            // Trump
            DbRegisteredPayment payment01 = new DbRegisteredPayment()
            {
                //Account1
                accountNumberFrom = "05390000000",
                accountNumberTo = "00000000001",
                amount = 75.00,
                receiverName = "S. Jon R.",
                paymentDate = new DateTime(2016, 10, 22),
                fromAccount = trumpAccount
            }; //END OF payment1

            DbRegisteredPayment payment02 = new DbRegisteredPayment()
            {
                //Account2
                accountNumberFrom = "05390000000",
                accountNumberTo = "00000000002",
                amount = 75.00,
                receiverName = "A Krutnes",
                paymentDate = new DateTime(2016, 10, 23),
                fromAccount = trumpAccount
            }; //END OF payment02

            DbRegisteredPayment payment03 = new DbRegisteredPayment()
            {
                //Account3
                accountNumberFrom = "05390000000",
                accountNumberTo = "00000000003",
                amount = 75.00,
                receiverName = "Mirko Grimm",
                paymentDate = new DateTime(2016, 10, 24),
                fromAccount = trumpAccount
            }; //END OF payment03
            try
            {
                db.RegisteredPayments.Add(payment01);
                db.RegisteredPayments.Add(payment02);
                db.RegisteredPayments.Add(payment03);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Feil i DB" + e.ToString());
            }

        }
    }
}