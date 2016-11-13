using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Admin
    {
        [Display(Name = "ID")]
        [Required(ErrorMessage = "ID Required. Min 4 chars")]
        [RegularExpression("[\\d]{4,18}", ErrorMessage = "ID Required. Min 4 chars")]
        public string ID { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "You're forgetting something crucial.")]
        [RegularExpression("[a-zA-ZæøåÆØÅ \\d*/@\\-'~?\\+_\\\\=%&$£#]{5,18}")]
        public string adminPassword { get; set; }
    }

    public class Customer
    {
        [Display(Name = "National ID")]
        [Required(ErrorMessage = "11 digit NID Required")]
        [RegularExpression("[\\d]{11}")]
        public string nID { get; set; }
        [Required(ErrorMessage = "First name required")]
        [RegularExpression("[A-ZÆØÅa-zæøå \\d,.'-]{2,50}", ErrorMessage = "Min 2 chars with numbers and (,)(.)(')( )(-) special chars allowed")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Last name required")]
        [RegularExpression("[A-ZÆØÅa-zæøå \\d,.'-]{2,50}", ErrorMessage = "Min 2 chars with numbers and (,)(.)(')( )(-) special chars allowed")]
        public string lastName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password with 6 min chars required")]
        [RegularExpression("[a-zA-ZæøåÆØÅ \\d*/@\\-'~?\\+_\\\\=%&$£#]{6,50}")]
        public string password { get; set; }
    }

    public class Account
    {
        public int id { get; set; }
        [RegularExpression("[\\d]{11}", ErrorMessage = "Account must be 11 digits")]
        [Required(ErrorMessage = "11 digit account number required")]
        public string accountNumber { get; set; }
        [Required(ErrorMessage = "Balance with 2 decimal values required")]
        [RegularExpression("[0-9]{1,}[.]{1}[0-9]{2}", ErrorMessage = "Balance with 2 decimal values required")]
        public double balance { get; set; }
        public string nID { get; set; }
    }

    public class RegisteredPayment
    {
        public int id { get; set; }
        public string cutomerAccountNumber { get; set; }
        [Display(Name = "Receiver account:")]
        [Required(ErrorMessage = "Receiver account must be present")]
        [RegularExpression("[\\d]{11}", ErrorMessage = "Account must be 11 digits.")]
        public string targetAccountNumber { get; set; }
        [Display(Name = "Receipient name:")]
        [Required(ErrorMessage = "Receipient name must be present")]
        [RegularExpression("[A-ZÆØÅa-zæøå \\d,.'-]{2,50}", ErrorMessage ="Min 2 chars with numbers and (,)(.)(')( )(-) special chars allowed")]
        public string receiverName { get; set; }
        [Display(Name = "Amount to transfer:")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [RegularExpression("[\\d]{1,15}[.]{1}[\\d]{2}", ErrorMessage = "Must be in this format: 00.00")]
        [Required(ErrorMessage = "Amount required")]
        public double amount { get; set; }
        [Display(Name = "Due date:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [RegularExpression("[\\d]{2}[.][\\d]{2}[.][\\d]{4}", ErrorMessage = "Date must be in this format: dd.MM.yyyy")]
        [Required(ErrorMessage = "Date required")]
        public DateTime paymentDate { get; set; }
    }

    public class IssuedPayment
    {
        //public int id { get; set; }
        public string cutomerAccountNumber { get; set; }
        public string targetAccountNumber { get; set; }
        public string receiverName { get; set; }
        public double amount { get; set; }
        public DateTime issuedDate { get; set; }
    }
}