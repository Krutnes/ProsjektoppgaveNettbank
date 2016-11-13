using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Models;
using ProsjektoppgaveNettbank.Controllers;
using DAL;
using BLL;



namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void adminRegisterCustomer(Customer inCustomer)
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));

            // Act
            var actionResult = (ViewResult)controller.adminRegisterCustomer(Customer inCustomer);

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
        }
}
