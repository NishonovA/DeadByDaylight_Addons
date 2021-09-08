using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DeadByDaylight_Addons
{
    public class AppSettingsManager
    {
        private const string patchNumberKey = "patchNumber";
        private const string backgroundColorKey = "backgroundColor";
        private const string textColorKey = "textColor";

        public static string GetPatchNumber()
        {
            string patchNumber = ConfigurationManager.AppSettings.Get(patchNumberKey);
            return patchNumber;
        }

        public static string GetBackgroundColor()
        {
            string backgroundColor = ConfigurationManager.AppSettings.Get(backgroundColorKey);
            return backgroundColor;
        }

        public static string GetTextColor()
        {
            string textColor = ConfigurationManager.AppSettings.Get(textColorKey);
            return textColor;
        }
    }
}
