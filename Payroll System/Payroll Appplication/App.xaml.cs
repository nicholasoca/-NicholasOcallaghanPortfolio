using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Payroll_Appplication
{
    public partial class App : Application
    {
        //Defining the connection String
        public static string ConnectionString = "server=localhost;user=root;database=Bergvallei;password=";

        // Other application startup logic
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
        }
    }
}
