using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp.Security;

namespace E2813.Demo.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
			DevExpress.ExpressApp.EasyTest.WinAdapter.RemotingRegistration.Register(4100);
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            DemoUserViewVariantWindowsFormsApplication winApplication = new DemoUserViewVariantWindowsFormsApplication();
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            winApplication.ConnectionString = E2813.Demo.Win.CodeCentralExampleInMemoryDataStoreProvider.ConnectionString;
            try {
                winApplication.Setup();
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
