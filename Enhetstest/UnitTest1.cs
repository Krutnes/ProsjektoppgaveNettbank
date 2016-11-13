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
            Assert.AreEqual(actionResult.ViewName, "");
        }
        [TestMethod]
        public void adminEditAccount_Ikke_Funnet_Ved_View()
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
        public void adminEditAccount_ikke_funnet_Post()
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));
            var accountResultat = new Account()
            {
                accountNumber = "12345678901",
                balance = 0,
            };

            // Act
            var actionResult = (ViewResult)controller.AdminEditAccount("0");

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
        }


    }
}
