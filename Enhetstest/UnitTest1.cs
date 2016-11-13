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
            var actionResult = (ViewResult)controller.Endre(1);

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
        }
        [TestMethod]
        public void adminEditAccount_Ikke_Funnet_Ved_View()
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));

            // Act
            var actionResult = (ViewResult)controller.Endre(0);
            var kundeResultat = (Kunde)actionResult.Model;

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
            Assert.AreEqual(kundeResultat.id, 0);
        }
        [TestMethod]
        public void adminEditAccount_ikke_funnet_Post()
        {
            // Arrange
            var controller = new BankController(new BankAdminBLL(new BankDALStub()));
            var innKunde = new Kunde()
            {
                id = 0,
                fornavn = "Per",
                etternavn = "Olsen",
                adresse = "Osloveien 82",
                postnr = "1234",
                poststed = "Oslo"
            };

            // Act
            var actionResult = (ViewResult)controller.Endre(0, innKunde);

            // Assert
            Assert.AreEqual(actionResult.ViewName, "");
        }
    }
