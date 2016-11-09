using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProsjektoppgaveNettbank.Models
{
    public class Customer
    {
        [Display(Name = "National ID")]
        [Required(ErrorMessage = "Personnummer må oppgis")]
        [RegularExpression("[\\d]{11}")]
        public string nID { get; set; }
        [Required(ErrorMessage = "Last name must be present")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Last name must be present")]
        public string lastName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password must be present")]
        [RegularExpression("[a-zA-ZæøåÆØÅ \\d*/@\\-'~?\\+_\\\\=%&$£#]{6,50}")]
        public string password { get; set; }
    }

    public class Account
    {
        [Required(ErrorMessage = "Account number must be 11 digits")]
        public string accountNumber { get; set; }
        [RegularExpression("[\\d]{11}")]
        public double balance { get; set; }
    }

    public class RegisteredPayment
    {
        public int id { get; set; }
        public string accountNumberFrom { get; set; }
        [Display(Name = "Receiver account:")]
        [Required(ErrorMessage = "Receiver account must be present")]
        [RegularExpression("[\\d]{11}", ErrorMessage = "Account must be 11 digits.")]
        public string accountNumberTo { get; set; }
        [Display(Name = "Receipient name:")]
        [Required(ErrorMessage = "Receipient name must be present")]
        public string receiverName { get; set; }

        [Display(Name = "Amount to transfer:")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [RegularExpression("[\\d]{1,15}[.]{1}[\\d]{2}", ErrorMessage = "Must be in this format: 00.00")]
        [Required(ErrorMessage = "Amount required")]
        public double amount { get; set; }
        [Display(Name = "Due date:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [RegularExpression("[\\d]{2}[.][\\d]{2}[.][\\d]{4}", ErrorMessage = "Date must be in this format: 00.00.0000")]
        [Required(ErrorMessage = "Date required")]
        public DateTime paymentDate { get; set; }
    }

    public class IssuedPayment
    {
        //public int id { get; set; }
        public string accountNumberFrom { get; set; }
        public string accountNumberTo { get; set; }
        public string receiverName { get; set; }
        public double amount { get; set; }
        public DateTime issuedDate { get; set; }
    }
}