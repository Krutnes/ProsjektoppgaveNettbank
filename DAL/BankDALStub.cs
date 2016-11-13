using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAL
{
    public class BankDALStub : DAL.IBankAdminDAL 
    {
        public bool adminRegisterCustomer(Customer inCustomer)
        {
            if(inCustomer.firstName == "")
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public bool isAdminLoginCorrect(Admin admin)
        {
            if (admin.ID.Equals("1111") && admin.adminPassword.Equals("admin") )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
