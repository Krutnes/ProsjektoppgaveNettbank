using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using ProsjektoppgaveNettbank;

namespace ProsjektoppgaveNettbank.Controllers
{
    public class BankController : Controller
    {
        public ActionResult AccountOverview()
        {
            if (Session["LoggedIn"] != null)
            {
                bool loggedIn = (bool)Session["LoggedIn"];
                if (loggedIn)
                {
                    BankCustomerBLL bll = new BankCustomerBLL();
                    bll.updatePendingPayments();
                    return View();
                }
            }
            return RedirectToAction("BankIndex", "Bank");
        }

        public ActionResult BankIndex()
        {
            if (Session["LoggedIn"] == null)
            {
                Session["LoggedIn"] = false;
            }
            else
            {
                if ((bool)Session["LoggedIn"])
                    return RedirectToAction("AccountOverview", "Bank");
            }
            return View();
        }
        /*
        [HttpPost]
        public ActionResult BankIndex(Customer customer)
        {
            System.Diagnostics.Debug.WriteLine("TEST " + customer.firstName);
            var bankBLL = new BankCustomerBLL();
            if (bankBLL.isLoginCorrect(customer))
            {
                Session["LoggedIn"] = true;
                Session["NID"] = customer.nID;
                return RedirectToAction("AccountOverview", "Bank");
            }
            Session["LoggedIn"] = false;
            ViewBag.LoggedIn = false;
            return View();
        }
        */

        public ActionResult CustomerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerLogin(Customer customer)
        {
            var bankBLL = new BankCustomerBLL();
            if (bankBLL.isLoginCorrect(customer))
            {
                Session["LoggedIn"] = true;
                Session["NID"] = customer.nID;
                return RedirectToAction("AccountOverview", "Bank");
            }
            Session["LoggedIn"] = false;
            ViewBag.LoggedIn = false;
            return View();
        }

        public string DeletePayment(string id)
        {
            var bankBLL = new BankCustomerBLL();
            string currentAccount = bankBLL.getRegisteredPaymentAccount(Convert.ToInt32(id));

            bankBLL.deletePayment(Convert.ToInt32(id));
            string allRegisteredPaymentsJSON = getRegisteredPayments(currentAccount);

            return allRegisteredPaymentsJSON;
        }

        public ActionResult EditPayment(string id)
        {
            if (Session["LoggedIn"] != null)
            {
                bool loggedIn = (bool)Session["LoggedIn"];
                if (loggedIn)
                {
                    var bankBLL = new BankCustomerBLL();
                    RegisteredPayment payment = bankBLL.findRegisteredPayment(Convert.ToInt32(id));
                    System.Diagnostics.Debug.WriteLine("TEST VIEW: " + payment.cutomerAccountNumber);
                    return View(payment);
                }
            }
            Session["LoggedIn"] = null;
            return RedirectToAction("BankIndex", "Bank");
        }

        [HttpPost]
        public ActionResult EditPayment(RegisteredPayment registeredPayment)
        {
            System.Diagnostics.Debug.WriteLine(registeredPayment.amount);
            var bankBLL = new BankCustomerBLL();
            if (!bankBLL.editPayment(registeredPayment))
                return View(registeredPayment);

            return RedirectToAction("AccountOverview", "Bank");
        }

        public string getCustomerAccounts()
        {
            var bankBLL = new BankCustomerBLL();
            List<Account> allCustomerAccounts = bankBLL.getCustomerAccounts((String)Session["NID"]);
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(allCustomerAccounts);
            return json;
        }

        public string getIssuedPaymentsforOneAccount(string accountNumberID)
        {
            string nID = (String)Session["NID"];
            var bankBLL = new BankCustomerBLL();
            List<IssuedPayment> allIssuedPayments = bankBLL.getIssuedPaymentsforOneAccount(accountNumberID);
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(allIssuedPayments);
            return json;
        }

        public string getRegisteredPayments(string id)
        {
            var bankBLL = new BankCustomerBLL();
            List<RegisteredPayment> allRegisteredPayments = bankBLL.getRegisteredPayments(id);
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(allRegisteredPayments);
            return json;
        }

        public ActionResult LogOut()
        {
            Session["LoggedIn"] = null;
            Session["NID"] = null;
            return RedirectToAction("BankIndex", "Bank");
        }


        public ActionResult RegisterCustomer()
        {
            var bankBLL = new BankCustomerBLL();
            bankBLL.populateDatabase();
            return View();
        }

        public ActionResult RegisterSinglePayment(string id)
        {
            if (Session["LoggedIn"] != null)
            {
                bool loggedIn = (bool)Session["LoggedIn"];
                if (loggedIn)
                {
                    Session["accountNumber"] = id;
                    return View();
                }
            }
            Session["LoggedIn"] = null;
            return RedirectToAction("BankIndex", "Bank");
        }


        [HttpPost]
        public ActionResult RegisterSinglePayment(RegisteredPayment registeredPayment)
        {
            if (Session["LoggedIn"] == null || Session["accountNumber"] == null)
            {
                Session["LoggedIn"] = null;
                Session["accountNumber"] = null;
                return RedirectToAction("BankIndex", "Bank");
            }
            registeredPayment.cutomerAccountNumber = (string)Session["accountNumber"];
            var bankBLL = new BankCustomerBLL();
            registeredPayment.amount = -((double) registeredPayment.amount);
            
            if (!bankBLL.registerPayment(registeredPayment))
                return RedirectToAction("RegisterSinglePayment", "Bank");
            Session["accountNumber"] = null;
            return RedirectToAction("AccountOverview", "Bank");
        }

        // GJØR SESSION OG DELING AV ADMIN/KUNDE ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public string AdminCreateNewAccount(string nid)
        {
            BankAdminBLL BankAdminBLL = new BankAdminBLL();
            List<Account> customerAccounts = BankAdminBLL.newAccount(nid);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(customerAccounts);
        }

        public ActionResult AdminCustomerDetails(string nid)
        {
            if (Session["AdminLoggedIn"] != null)
            {
                if ((bool)Session["AdminLoggedIn"])
                {
                    var bankBLL = new BankCustomerBLL();
                    bankBLL.updatePendingPayments();
                    List<Account> customerAccounts = bankBLL.getCustomerAccounts(nid);
                    ViewBag.NID = (String)nid;
                    return View(customerAccounts);
                }
            }
            Session["AdminLoggedIn"] = null;
            return RedirectToAction("AdminLogin", "Bank");
        }

        public string AdminDeleteBankAccount(string accountNumber)
        {
            var BankAdminBLL = new BankAdminBLL();
            List<Account> remainingAccounts = BankAdminBLL.adminDeleteAccount(accountNumber);
            System.Diagnostics.Debug.WriteLine("KOMMER HIT: " + remainingAccounts.Count);
            foreach (Account i in remainingAccounts)
                System.Diagnostics.Debug.WriteLine(i.accountNumber + "\n");
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(remainingAccounts);
        }

        public string AdminDeleteCustomer(string id)
        {
            var BankAdminBLL = new BankAdminBLL();
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(BankAdminBLL.adminDeleteCustomer(id));
        }

        public ActionResult AdminEditCustomer(string nid)
        {
            if (Session["AdminLoggedIn"] != null)
            {
                if ((bool)Session["AdminLoggedIn"])
                {
                    var BankAdminBLL = new BankAdminBLL();
                    Customer customer = BankAdminBLL.findCustomer(nid);
                    return View(customer);
                }
            }
            Session["AdminLoggedIn"] = null;
            return RedirectToAction("AdminLogin", "Bank");
        }

        [HttpPost]
        public ActionResult AdminEditCustomer(Customer customer)
        {
            var BankAdminBLL = new BankAdminBLL(); ;
            if (!BankAdminBLL.adminEditCustomer(customer))
                return View(customer);

            return RedirectToAction("AdminOverview", "Bank");
        }



        public ActionResult AdminLogin()
        {
            if (Session["AdminLoggedIn"] != null && (bool)Session["AdminLoggedIn"])
                return RedirectToAction("AdminOverview", "Bank");
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            var bankAdminBLL = new BankAdminBLL();
            if (bankAdminBLL.isAdminLoginCorrect(admin))
            {
                Session["AdminLoggedIn"] = true;
                return RedirectToAction("AdminOverview", "Bank");
            }
            Session["AdminLoggedIn"] = null;
            return View();
        }

        public ActionResult AdminOverview()
        {
            if (Session["AdminLoggedIn"] != null)
            {
                bool AdminLoggedIn = (bool)Session["AdminLoggedIn"];
                if (AdminLoggedIn)
                {
                    var BankAdminBLL = new BankAdminBLL();
                    List<Customer> allCustomers = BankAdminBLL.getAllCustomers();
                    return View(allCustomers);
                }
            }
            Session["AdminLoggedIn"] = null;
            return RedirectToAction("AdminLogin", "Bank");
        }

        public ActionResult AdminEditAccount(string accNumber)
        {
            var bankBLL = new BankAdminBLL();
            Account account = bankBLL.findAccount(accNumber);
            Session["AccountNumber"] = (string)accNumber;
            return View(account);
        }

        [HttpPost]
        public ActionResult AdminEditAccount(Account account)
        {

            var bankBLL = new BankAdminBLL();
            if (!bankBLL.adminEditAccount(account, (string)Session["AccountNumber"]))
            {
                return View(account);
            }
            string nid = bankBLL.findAccount(account.accountNumber).nID;
            System.Diagnostics.Debug.Write("TEST nid" + nid);
            return Redirect("/Bank/AdminCustomerDetails/?nid=" + nid);
        }

        public ActionResult AdminRegisterCustomer() // REGEX NEEDED ::::::::::::::::::::::::::::::::::::::::::::::::
        {
            if (Session["AdminLoggedIn"] != null && (bool)Session["AdminLoggedIn"])
                return View();

            Session["AdminLoggedIn"] = null;
            return RedirectToAction("AdminLogin", "Bank");
        }

        [HttpPost]
        public ActionResult AdminRegisterCustomer(Customer inCustomer)
        {
            var db = new BankAdminBLL();
            string password = inCustomer.password;
            if (!db.adminRegisterCustomer(inCustomer))
                return View(inCustomer);
            return RedirectToAction("AdminOverview");
        }

        public string GetAllCustomers()
        {
            var BankAdminBLL = new BankAdminBLL();
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(BankAdminBLL.getAllCustomers());
        }
    }
}
