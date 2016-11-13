using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace DAL
{
    public class BankCustomerDAL
    {
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
                catch (Exception e)
                {
                    return false;
                }
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
                    try
                    {
                        regPayment.customerAccountNumber = payment.cutomerAccountNumber;
                        regPayment.targetAccountNumber = payment.targetAccountNumber;
                        regPayment.amount = payment.amount;
                        regPayment.paymentDate = payment.paymentDate;
                        db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        public static void errorReport(string e)
        {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog.txt");
            DateTime currentTime = DateTime.Now;
            string[] lines = { "Error at " + currentTime + ":\n" + e.ToString() + 
                    "\n_________________________________\n"};
            System.IO.File.AppendAllLines(path, lines);
        }

        public RegisteredPayment findRegisteredPayment(int id)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbRegisteredPayment dbPayment = db.RegisteredPayments.FirstOrDefault(
                        p => p.id == id);
                    if (dbPayment == null)
                        return null;
                    RegisteredPayment rp = new RegisteredPayment()
                    {
                        id = dbPayment.id,
                        cutomerAccountNumber = dbPayment.customerAccountNumber,
                        targetAccountNumber = dbPayment.targetAccountNumber,
                        amount = dbPayment.amount,
                        paymentDate = dbPayment.paymentDate,
                        receiverName = dbPayment.receiverName
                    };
                    return rp;
                }
                catch (Exception e)
                {
                    
                    return null;
                }
            }
        }

        public static byte[] generateHash(string clearTextPassword)
        {
            byte[] inData, outData;
            var algorithm = System.Security.Cryptography.SHA512.Create();
            inData = System.Text.Encoding.ASCII.GetBytes(clearTextPassword);
            outData = algorithm.ComputeHash(inData);
            return outData;
        }

        public static string generateSalt()
        {
            byte[] randomArray = new byte[10];
            string randomString;

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomArray);
            randomString = Convert.ToBase64String(randomArray);
            return randomString;
        }


        public List<Account> getCustomerAccounts(string nid)
        {
            try
            {
                using (var db = new BankDBContext())
                {
                    List<Account> accounts = new List<Account>();
                    List<DbAccount> dbAccounts = db.Accounts.Where(a => a.NID.Equals(nid)).ToList();
                    foreach (DbAccount dba in dbAccounts)
                    {
                        accounts.Add(new Account()
                        {
                            accountNumber = dba.accountNumber,
                            balance = dba.balance,
                            nID = dba.NID
                        });
                    }
                    return accounts;
                }
            }
            catch (Exception e)
            {
               
                return null;
            }
        }

        public List<IssuedPayment> getIssuedPayments(string accountNumber)
        {
            using (var db = new BankDBContext())
            {
                List<IssuedPayment> issuedPayments = new List<IssuedPayment>();
                IEnumerable<DbIssuedPayment> dbIssuedPayments = db.IssuedPayments.
                    Where(a => a.customerAccountNumber.Equals(accountNumber)).ToList();
                foreach (DbIssuedPayment issuedPayment in dbIssuedPayments)
                {
                    issuedPayments.Add(new IssuedPayment()
                    {
                        cutomerAccountNumber = issuedPayment.customerAccountNumber,
                        targetAccountNumber = issuedPayment.targetAccountNumber,
                        amount = issuedPayment.amount,
                        issuedDate = issuedPayment.issuedDate,
                        receiverName = issuedPayment.receiverName
                    });
                }
                return issuedPayments;
            }
        }

        public List<RegisteredPayment> getRegisteredPayments(string accountNumber)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    List<RegisteredPayment> registeredPayments = new List<RegisteredPayment>();
                    IEnumerable<DbRegisteredPayment> dbRegisteredPayments = db.RegisteredPayments.
                        Where(a => a.customerAccountNumber.Equals(accountNumber)).ToList();
                    foreach (DbRegisteredPayment regPayment in dbRegisteredPayments)
                    {
                        registeredPayments.Add(new RegisteredPayment()
                        {
                            id = regPayment.id,
                            cutomerAccountNumber = regPayment.customerAccountNumber,
                            targetAccountNumber = regPayment.targetAccountNumber,
                            amount = regPayment.amount,
                            paymentDate = regPayment.paymentDate,
                            receiverName = regPayment.receiverName
                        });
                    }
                    return registeredPayments;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public string getRegisteredPaymentAccount(int id)
        {
            using (var db = new BankDBContext())
            {
                return db.RegisteredPayments.FirstOrDefault(p => p.id == id).customerAccountNumberFK.accountNumber;
            }
        }

        public bool isLoginCorrect(Customer customer)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbCustomer customerFound = db.Customers.FirstOrDefault(c => c.NID.Equals(customer.nID));
                    if (customerFound != null)
                    {
                        byte[] checkPassword = generateHash(customer.password + customerFound.salt);
                        bool validCustomer = customerFound.password.SequenceEqual(checkPassword);
                        return validCustomer;
                    }
                }
                catch (Exception e)
                {
                    errorReport(e.ToString());
                    return false;
                }
                return false;
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
            string salt = generateSalt();
            string passwordAndSalt = "admin" + salt;
            byte[] adminhashedPassword = generateHash(passwordAndSalt);
            dbAdmin1.adminpassword = adminhashedPassword;
            dbAdmin1.adminsalt = salt;

            // Customer 1
            DbCustomer dbCustomer1 = new DbCustomer
            {
                firstName = "Hillary",
                lastName = "Clinton",
                NID = "24126712345"
            };
            salt = generateSalt();
            passwordAndSalt = "What emails?" + salt;
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
                //db.SaveChanges();

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
                System.Diagnostics.Debug.WriteLine("Feil i DB: " + e.ToString());
            }
            populatePaymentTables();
        }

        public void populatePaymentTables()
        {
            BankDBContext db = new BankDBContext();
            DbAccount trumpAccount000 = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05390000000"));
            DbAccount trumpAccount999 = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05399999999"));
            DbAccount trumpAccount111 = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05391111111"));

            DbAccount rokkeAccount777 = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05397777777"));
            DbAccount rokkeAccount888 = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05398888888"));

            DbAccount hillaryAccount666 = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals("05396666666"));
            
            
            if (trumpAccount000 == null || trumpAccount999 == null || trumpAccount111 == null || 
                rokkeAccount777 == null || rokkeAccount888 == null || hillaryAccount666 == null)
                return;
            
            // Trump pending payments
            DbRegisteredPayment trumpPayment01 = new DbRegisteredPayment()
            {
                //Account1
                customerAccountNumber = "05390000000",
                targetAccountNumber = "05397777777",
                amount = -75.00,
                receiverName = "Kjell Inge Røkke",
                paymentDate = new DateTime(2016, 11, 25),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = rokkeAccount777
            }; //END OF payment1

            DbRegisteredPayment trumpPayment02 = new DbRegisteredPayment()
            {
                //Account2
                customerAccountNumber = "05390000000",
                targetAccountNumber = "05396666666",
                amount = -75.00,
                receiverName = "Hillary \"Hilldog\" Clinton",
                paymentDate = new DateTime(2016, 11, 25),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = hillaryAccount666
            }; //END OF payment02

            DbRegisteredPayment trumpPayment03 = new DbRegisteredPayment()
            {
                //Account3
                customerAccountNumber = "05390000000",
                targetAccountNumber = "05398888888",
                amount = -75.00,
                receiverName = "Kjell Inge Røkke",
                paymentDate = new DateTime(2016, 11, 25),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = rokkeAccount888
            }; //END OF payment03
            // END of Trump payments

            // Hillary Clinton pending payments
            DbRegisteredPayment hildogPayment01 = new DbRegisteredPayment()
            {
                //Account1
                customerAccountNumber = "05396666666",
                targetAccountNumber = "05397777777",
                amount = -75.00,
                receiverName = "Kjell Inge Røkke",
                paymentDate = new DateTime(2016, 11, 25),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = rokkeAccount777
            }; //END OF payment1

            DbRegisteredPayment hildogPayment02 = new DbRegisteredPayment()
            {
                //Account2
                customerAccountNumber = "05396666666",
                targetAccountNumber = "05390000000",
                amount = -75.00,
                receiverName = "Donald Trump",
                paymentDate = new DateTime(2016, 11, 25),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = hillaryAccount666
            }; //END OF payment02

            DbRegisteredPayment hildogPayment03 = new DbRegisteredPayment()
            {
                //Account3
                customerAccountNumber = "05396666666",
                targetAccountNumber = "05398888888",
                amount = -75.00,
                receiverName = "Kjell Inge Røkke",
                paymentDate = new DateTime(2016, 11, 25),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = rokkeAccount888
            }; //END OF payment03
            // END of Hillary payments

            // Adding transaction history
            // Payment history for Røkke
            DbIssuedPayment rokkeIssuedPayment01 = new DbIssuedPayment()
            {
                customerAccountNumber = "05397777777",
                targetAccountNumber = "05396666666",
                receiverName = "Hillary Clinton",
                amount = 3593.93,
                issuedDate = new DateTime(2016, 10, 27),
                customerAccountNumberFK = rokkeAccount777,
                targetAccountNumberFK = hillaryAccount666
            };

            DbIssuedPayment rokkeIssuedPayment02 = new DbIssuedPayment()
            {
                customerAccountNumber = "05397777777",
                targetAccountNumber = "05399999999",
                receiverName = "Donald Trump",
                amount = 3593.93,
                issuedDate = new DateTime(2016, 10, 27),
                customerAccountNumberFK = rokkeAccount777,
                targetAccountNumberFK = trumpAccount999
            };
            // END OF røkke payments

            // Payment history for Trump
            DbIssuedPayment trumpIssuedPayment01 = new DbIssuedPayment()
            {
                customerAccountNumber = "05397777777",
                targetAccountNumber = "05396666666",
                receiverName = "Hillary Clinton",
                amount = 3593.93,
                issuedDate = new DateTime(2016, 10, 27),
                customerAccountNumberFK = rokkeAccount777,
                targetAccountNumberFK = hillaryAccount666
            };

            DbIssuedPayment trumpIssuedPayment02 = new DbIssuedPayment()
            {
                customerAccountNumber = "05390000000",
                targetAccountNumber = "05397777777",
                receiverName = "Kjell Inge Røkke",
                amount = -2000.22,
                issuedDate = new DateTime(2016, 10, 27),
                customerAccountNumberFK = trumpAccount000,
                targetAccountNumberFK = rokkeAccount777
            };

            DbIssuedPayment trumpIssuedPayment03 = new DbIssuedPayment()
            {
                customerAccountNumber = "05391111111",
                targetAccountNumber = "05398888888",
                receiverName = "Kjell Inge Røkke",
                amount = -2000.22,
                issuedDate = new DateTime(2016, 10, 27),
                customerAccountNumberFK = trumpAccount111,
                targetAccountNumberFK = rokkeAccount888
            };
            // END OF Trump payments
            try
            {
                db.RegisteredPayments.Add(trumpPayment01);
                db.RegisteredPayments.Add(trumpPayment02);
                db.RegisteredPayments.Add(trumpPayment03);
                db.RegisteredPayments.Add(hildogPayment01);
                db.RegisteredPayments.Add(hildogPayment02);
                db.RegisteredPayments.Add(hildogPayment03);
                db.IssuedPayments.Add(rokkeIssuedPayment01);
                db.IssuedPayments.Add(rokkeIssuedPayment02);
                db.IssuedPayments.Add(trumpIssuedPayment01);
                db.IssuedPayments.Add(trumpIssuedPayment02);
                db.IssuedPayments.Add(trumpIssuedPayment03);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error in  DB" + e.ToString());
            }
        }

        public void processRegisteredPayment(DbRegisteredPayment payment)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbAccount customerAccount = db.Accounts.FirstOrDefault(
                        a => a.accountNumber.Equals(payment.customerAccountNumber));
                    DbAccount targetAccount = db.Accounts.FirstOrDefault(
                        a => a.accountNumber.Equals(payment.targetAccountNumber));
                    DbIssuedPayment ip = new DbIssuedPayment()
                    {
                        customerAccountNumber = payment.customerAccountNumber,
                        targetAccountNumber = payment.targetAccountNumber,
                        receiverName = payment.receiverName,
                        amount = payment.amount,
                        issuedDate = DateTime.Now,
                        customerAccountNumberFK = customerAccount
                    };
                    if (targetAccount != null)
                        ip.targetAccountNumberFK = targetAccount;
                    db.IssuedPayments.Add(ip);

                    DbRegisteredPayment test = db.RegisteredPayments.FirstOrDefault(rp => rp.id == payment.id);
                    db.RegisteredPayments.Remove(test);

                    customerAccount.balance += payment.amount;
                    db.Entry(customerAccount).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    errorReport(e.ToString());
                    return;
                }
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
                        customerAccountNumber = payment.cutomerAccountNumber,
                        targetAccountNumber = payment.targetAccountNumber,
                        amount = payment.amount,
                        customerAccountNumberFK = db.Accounts.Find(payment.cutomerAccountNumber),
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

        public void updatePendingPayments()
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    List<DbRegisteredPayment> pendingPayments = db.RegisteredPayments.ToList();
                    foreach (DbRegisteredPayment currentPayment in pendingPayments)
                    {
                        DateTime date = currentPayment.paymentDate;
                        if (date < DateTime.Today)
                        {
                            processRegisteredPayment(currentPayment);
                        }
                    }
                }
                catch (Exception e)
                {
                    errorReport(e.ToString());
                    return;
                }
            }
        }
    }

    public class BankAdminDAL
    {
        public bool adminEditAccount(Account account, string oldAccountNumber) 
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbAccount dbaccount = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals(oldAccountNumber));
                    System.Diagnostics.Debug.WriteLine("TEST DAL ACCOUNT: " + dbaccount.accountNumber);

                    //customerAccountNumber from DbIssuedPayments
                    IEnumerable<DbIssuedPayment> issuedPayments = db.IssuedPayments
                       .Where(ip => ip.customerAccountNumber.Equals(oldAccountNumber)).ToList();
                    foreach (DbIssuedPayment dbip in issuedPayments)
                    {
                        dbip.customerAccountNumber = account.accountNumber;
                        db.SaveChanges();
                    }
                    //customerAccountNumber from DbRegisteredPayments 
                    IEnumerable<DbRegisteredPayment> registeredPayments = db.RegisteredPayments
                        .Where(rp => rp.customerAccountNumber.Equals(oldAccountNumber)).ToList();
                    foreach (DbRegisteredPayment dbrp in registeredPayments)
                    {
                        dbrp.customerAccountNumber = account.accountNumber;
                        db.SaveChanges();
                    }

                    if (dbaccount != null)
                    {
                        dbaccount.accountNumber = account.accountNumber;
                        dbaccount.balance = account.balance;
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    BankCustomerDAL.errorReport(e.ToString());
                    return false;
                }
                return false;
            }
        }
        public bool adminEditCustomer(Customer customer)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbCustomer dbcustomer = db.Customers.FirstOrDefault(c => c.NID == customer.nID);
                    if (dbcustomer != null)
                    {
                        dbcustomer.firstName = customer.firstName;
                        dbcustomer.lastName = customer.lastName;
                        string salt = BankCustomerDAL.generateSalt();
                        string passwordAndSalt = customer.password + salt;
                        byte[] hashedpassword = BankCustomerDAL.generateHash(passwordAndSalt);
                        dbcustomer.password = hashedpassword;
                        dbcustomer.salt = salt;
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    BankCustomerDAL.errorReport(e.ToString());
                    return false;
                }
                return false;
            }
        }

        public bool adminRegisterCustomer(Customer inCustomer)
        {
            try
            {


                var newCustomer = new DbCustomer()
                {

                    firstName = inCustomer.firstName,
                    lastName = inCustomer.lastName,
                    NID = inCustomer.nID

                };

                var db = new BankDBContext();
                string salt = BankCustomerDAL.generateSalt();
                string passwordAndSalt = inCustomer.password + salt;
                byte[] hashedpassword = BankCustomerDAL.generateHash(passwordAndSalt);
                newCustomer.password = hashedpassword;
                newCustomer.salt = salt;
                db.Customers.Add(newCustomer);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                BankCustomerDAL.errorReport(e.ToString());
                return false;
            }
        }

        public List<Account> deleteAccount(string accountNumber)
        {
            try
            {

                var db = new BankDBContext();

                DbAccount account = db.Accounts.FirstOrDefault(a => a.accountNumber.Equals(accountNumber));
                string nid = account.NID;
                IEnumerable<DbRegisteredPayment> registeredPayments = db.RegisteredPayments.
                    Where(rp => rp.customerAccountNumber.Equals(account.accountNumber)).ToList();

                foreach (DbRegisteredPayment rp in registeredPayments)
                {
                    db.RegisteredPayments.Remove(rp);
                }
                IEnumerable<DbIssuedPayment> issuedPayments = db.IssuedPayments.
                    Where(ip => ip.customerAccountNumber.Equals(account.accountNumber)).ToList();
                foreach (DbIssuedPayment ip in issuedPayments)
                {
                    db.IssuedPayments.Remove(ip);
                }
                db.Accounts.Remove(account);
                db.SaveChanges();
                //DbCustomer currentCustomer = db.Customers.FirstOrDefault(c => c.NID.Equals(nid));
                System.Diagnostics.Debug.WriteLine("DAL: Kunde NID: " + nid);
                List<Account> remainingAccounts = db.Accounts.Where(a => a.NID.Equals(nid)).Select(a => new Account()
                {
                    accountNumber = a.accountNumber,
                    balance = a.balance
                })
                .ToList();
                System.Diagnostics.Debug.WriteLine("DAL: SLETT ACCOUNT RESTERENDE KONTOER: " + remainingAccounts.Count());
                return remainingAccounts;
            }
            catch (Exception e)
            {
                BankCustomerDAL.errorReport(e.ToString());
                return null;
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
                    registeredPayments = db.RegisteredPayments.Where(rp => rp.customerAccountNumber.Equals(account.accountNumber)).ToList();
                    issuedPayments = db.IssuedPayments.Where(ip => ip.customerAccountNumber.Equals(account.accountNumber)).ToList();
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
            catch (Exception e)
            {
                BankCustomerDAL.errorReport(e.ToString());
                return null;
            }
        
        }

        public Account findAccount(string accNumber)
        {
            var db = new BankDBContext();
            try
            {

                var foundAccount = db.Accounts.FirstOrDefault(pk => pk.accountNumber.Equals(accNumber));
                System.Diagnostics.Debug.WriteLine("TEST DAL FINDACCOUNT: " + foundAccount.accountNumber);
                if (foundAccount == null)
                {
                    return null;
                }
                else
                {
                    var account = new Account()
                    {
                        accountNumber = foundAccount.accountNumber,
                        balance = foundAccount.balance,
                        nID = foundAccount.NID
                    };
                    return account;
                }
            }
            catch (Exception e)
            {
                BankCustomerDAL.errorReport(e.ToString());
                return null;
            }
        }

        public Customer findCustomer(string nID)
        {
            var db = new BankDBContext();
            try
            {

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
            catch (Exception e)
            {
                BankCustomerDAL.errorReport(e.ToString());
                return null;
            }
        }

        public List<Customer> getAllCustomers()
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    List<Customer> alleKunder = db.Customers.Select(k => new Customer()
                    {
                        nID = k.NID,
                        firstName = k.firstName,
                        lastName = k.lastName
                    }).ToList();
                    return alleKunder;
                }
                catch (Exception e)
                {
                    BankCustomerDAL.errorReport(e.ToString());
                    return null;
                }
            }
        }

        private string generateBankAccountNumber()
        {
            var chars = "0123456789";
            var stringChars = new char[7];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public bool isAdminLoginCorrect(Admin admin)
        {
            using (var db = new BankDBContext())
            {
                try
                {
                    DbAdmin adminFound = db.Admins.FirstOrDefault(c => c.ID.Equals(admin.ID));
                    if (adminFound != null)
                    {
                        byte[] checkPassword = BankCustomerDAL.generateHash(admin.adminPassword + adminFound.adminsalt);
                        bool validAdmin = adminFound.adminpassword.SequenceEqual(checkPassword);
                        return validAdmin;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    BankCustomerDAL.errorReport(e.ToString());
                    return false;
                }
            }
        }

        public List<Account> newAccount(string nID)
        {

            string newAccountNumber = generateBankAccountNumber();
            var db = new BankDBContext();

            /*if (db.isAccountAlreadyPresent(newAccountNumber))
            {

            }
            */
            var accountNew = new DbAccount()
            {
                accountNumber = "0539" + newAccountNumber,
                balance = 0,
                NID = nID
            };
            try
            {
                db.Accounts.Add(accountNew);
                db.SaveChanges();
                return db.Accounts.
                    Where(a => a.NID.Equals(nID)).Select(a => new Account()
                    {
                        accountNumber = a.accountNumber,
                        balance = a.balance,
                        nID = a.NID
                    }).ToList();
            }
            catch (Exception e)
            {
                BankCustomerDAL.errorReport(e.ToString());
                return null;
            }
            }
        }
    }
