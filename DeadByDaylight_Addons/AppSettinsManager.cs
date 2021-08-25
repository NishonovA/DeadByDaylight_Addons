using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DeadByDaylight_Addons
{
    public class AppSettinsManager
    {
        private const string patchNumberKey = "patchNumber";

        public static string GetPatchNumber()
        {
            string patchNumber = ConfigurationManager.AppSettings.Get(patchNumberKey);
            return patchNumber;
        }
    }
}
