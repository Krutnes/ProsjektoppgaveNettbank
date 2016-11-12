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

        [HttpPost]
        public ActionResult BankIndex(Customer customer)
        {
            var bankBLL = new BankBLL();
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

        public ActionResult RegisterCustomer()
        {
            var bankBLL = new BankBLL();
            bankBLL.populateDatabase();
            return View();
        }

        public ActionResult AccountOverview()
        {
            if (Session["LoggedIn"] != null)
            {
                bool loggedIn = (bool)Session["LoggedIn"];
                if (loggedIn)
                {
                    return View();
                }
            }
            return RedirectToAction("BankIndex", "Bank");
        }


        public ActionResult EditPayment(string id)
        {
            if (Session["LoggedIn"] != null)
            {
                bool loggedIn = (bool)Session["LoggedIn"];
                if (loggedIn)
                {
                    var bankBLL = new BankBLL();
                    RegisteredPayment payment = bankBLL.findRegisteredPayment(Convert.ToInt32(id));
                    return View(payment);
                }
            }
            Session["LoggedIn"] = null;
            return RedirectToAction("BankIndex", "Bank");
        }

        [HttpPost]
        public ActionResult EditPayment(RegisteredPayment registeredPayment)
        {
            var bankBLL = new BankBLL();
            if(!bankBLL.editPayment(registeredPayment))
                return View(registeredPayment);

            return RedirectToAction("AccountOverview", "Bank");
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
            registeredPayment.accountNumberFrom = (string) Session["accountNumber"];
            var bankBLL = new BankBLL();
            if (!bankBLL.registerPayment(registeredPayment))
                return RedirectToAction("RegisterSinglePayment", "Bank");
            Session["accountNumber"] = null;
            return RedirectToAction("AccountOverview", "Bank");
        }

        public string DeletePayment(string id)
        {
            var bankBLL = new BankBLL();
            string currentAccount = bankBLL.getRegisteredPaymentAccount(Convert.ToInt32(id));

            bankBLL.deletePayment(Convert.ToInt32(id));
            string allRegisteredPaymentsJSON = getRegisteredPayments(currentAccount);

            return allRegisteredPaymentsJSON;
        }

        public string getCustomerAccounts()
        {
            var bankBLL = new BankBLL();
            List<Account> allCustomerAccounts = bankBLL.getCustomerAccounts((String)Session["NID"]);
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(allCustomerAccounts);
            return json;
        }

        public string getRegisteredPayments(string id)
        {
            var bankBLL = new BankBLL();
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


        // GJØR SESSION OG DELING AV ADMIN/KUNDE ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public ActionResult AdminLogin()
        {
            if (Session["AdminLoggedIn"] != null && (bool)Session["AdminLoggedIn"])
                    return RedirectToAction("AdminOverview", "Bank");
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            var bankBLL = new BankBLL();
            if (bankBLL.isAdminLoginCorrect(admin))
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
                    var bankBLL = new BankBLL();
                    List<Customer> allCustomers = bankBLL.getAllCustomers();
                    return View(allCustomers);
                }
            }
            Session["AdminLoggedIn"] = null;
            return RedirectToAction("AdminLogin", "Bank");
        }
        

        public string AdminDeleteCustomer(string id)
        {
            var bankBLL = new BankBLL();
            var jsonSerializer = new JavaScriptSerializer();
            string json = jsonSerializer.Serialize(bankBLL.adminDeleteCustomer(id));
            return json;
        }

        public string GetAllCustomers()
        {
            var bankBLL = new BankBLL();
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(bankBLL.getAllCustomers());
        }

        public ActionResult AdminCustomerDetails(string nid)
        {
            if (Session["AdminLoggedIn"] != null)
            {
                if ((bool)Session["AdminLoggedIn"])
                {
                    var bankBLL = new BankBLL();
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
            var bankBLL = new BankBLL();
            List<Account> remainingAccounts = bankBLL.adminDeleteAccount(accountNumber);
            System.Diagnostics.Debug.WriteLine("KOMMER HIT: " + remainingAccounts.Count);
            foreach (Account i in remainingAccounts)
                System.Diagnostics.Debug.WriteLine(i.accountNumber + "\n");
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(remainingAccounts);
        }
        
        public ActionResult AdminEditCustomer(string nid)
        {
            if (Session["AdminLoggedIn"] != null)
            {
                if ((bool)Session["AdminLoggedIn"])
                {
                    var bankBLL = new BankBLL();
                    Customer customer = bankBLL.findCustomer(nid);
                    return View(customer);
                }
            }
            Session["AdminLoggedIn"] = null;
            return RedirectToAction("AdminLogin", "Bank");
        }
        
        [HttpPost]
        public ActionResult AdminEditCustomer(Customer customer)
        {
            var bankBLL = new BankBLL(); ;
            if (!bankBLL.adminEditCustomer(customer))
                return View(customer);
            
            return RedirectToAction("AdminOverview", "Bank");
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
            var db = new BankBLL();
            string password = inCustomer.password;
            if (!db.AdminRegisterCustomer(inCustomer))
                return View(inCustomer);
            return RedirectToAction("AdminOverview");
        }

        public string AdminCreateNewAccount(string nid)
        {
            BankBLL bankBLL = new BankBLL();
            List<Account> customerAccounts = bankBLL.newAccount(nid);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(customerAccounts);
            

        }
    }
}