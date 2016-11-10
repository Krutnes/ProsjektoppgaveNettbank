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
                ViewBag.LoggedIn = false;
            }
            else
            {
                ViewBag.LoggedIn = (bool)Session["LoggedIn"];
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
                ViewBag.LoggedIn = true;
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
                bool LoggedIn = (bool)Session["LoggedIn"];
                if (LoggedIn)
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
                bool LoggedIn = (bool)Session["LoggedIn"];
                if (LoggedIn)
                {
                    var bankBLL = new BankBLL();
                    RegisteredPayment payment = bankBLL.findRegisteredPayment(Convert.ToInt32(id));
                    return View(payment);
                }
            }
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
                bool LoggedIn = (bool)Session["LoggedIn"];
                if (LoggedIn)
                {
                    Session["accountNumber"] = id;
                    return View();
                }
            }
            return RedirectToAction("BankIndex", "Bank");
        }


        [HttpPost]
        public ActionResult RegisterSinglePayment(RegisteredPayment registeredPayment)
        {
            if (Session["LoggedIn"] == null || Session["accountNumber"] == null)
            {
                return RedirectToAction("BankIndex", "Bank");
            }
            registeredPayment.accountNumberFrom = (string) Session["accountNumber"];
            var bankBLL = new BankBLL();
            bankBLL.registerPayment(registeredPayment);
            ViewBag.AccountNumber = registeredPayment.accountNumberFrom;
            return RedirectToAction("AccountOverview", "Bank");
        }

        public string DeletePayment(string id)
        {
            var bankBLL = new BankBLL();
            string currentAccount = bankBLL.getRegisteredPaymentAccount(Convert.ToInt32(id));

            bankBLL.deletePayment(Convert.ToInt32(id));
            string allRegisteredPayments = getRegisteredPayments(currentAccount);

            return allRegisteredPayments;
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
            ViewBag.LoggedIn = null;
            Session["NID"] = null;
            return RedirectToAction("BankIndex", "Bank");
        }

        public ActionResult AdminLogin()
        {
            if (Session["AdminLoggedIn"] == null)
            {
                Session["AdminLoggedIn"] = false;
                ViewBag.AdminLoggedIn = false;
            }
            else
            {
                ViewBag.AdminLoggedIn = (bool)Session["AdminLoggedIn"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(Admin admin)
        {
            var bankBLL = new BankBLL();
            if (bankBLL.isAdminLoginCorrect(admin))
            {
                Session["AdminLoggedIn"] = true;
                ViewBag.AdminLoggedIn = true;
                Session["ID"] = admin.ID;
                return RedirectToAction("AdminOverview", "Bank");
            }
            Session["AdminLoggedIn"] = false;
            ViewBag.AdminLoggedIn = false;
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
            return RedirectToAction("BankIndex", "Bank");
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
            var bankBLL = new BankBLL();
            Customer customer = bankBLL.findCustomer(nid);
            List<Account> customerAccounts = bankBLL.getCustomerAccounts(nid);


            return View(customerAccounts);
        }

        public string AdminDeleteBankAccount(string accountNumber)
        {
            var bankBLL = new BankBLL();
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(bankBLL.adminDeleteAccount(accountNumber));
        }
    }
}