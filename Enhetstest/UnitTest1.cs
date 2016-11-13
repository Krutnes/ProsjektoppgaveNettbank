using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Models;
using DAL;
using BLL;
using ProsjektoppgaveNettbank.Controllers;
using System.Linq;



namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        public void adminEditAccount()
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));

            // Act
            var actionResult = (ViewResult)controller.AdminEditAccount("1");

            // Assert
            Assert.AreEqual(result.RouteName, "");
            Assert.AreEqual(result.RouteValues.Values.First(), "AdminOverview");
        }

        public void isAdminLoginCorrect()
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));

            // Act
            var actionResult = (ViewResult)controller.AdminEditAccount("0");
            var accountResultat = (Account)actionResult.Model;

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
            Assert.AreEqual(accountResultat.accountNumber, 0);
        }
        [TestMethod]
        public void adminDeleteCustomer()
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));
            var delCust = new Customer()
            {
                nID = null
            };

            // Act
            var actionResult = (ViewResult)controller.AdminDeleteCustomer(delCust);
            var resultat = (Customer)actionResult.Model;

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
        }

        /*    public void errorReport()
            {
                var controller = new BankController(new BankAdminBLL(new BankDALStub()));
                string text = "awdwad \n";

                var actionResult = (ViewResult)BankCustomerDAL.errorReport(text);

                Assert.IsTrue(actionResult.ViewData.ModelState.Count == 1);

            } */


    }
}
