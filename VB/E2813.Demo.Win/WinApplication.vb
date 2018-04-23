Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp.Win
Imports System.Collections.Generic

Namespace E2813.Demo.Win
	Partial Public Class DemoUserViewVariantWindowsFormsApplication
		Inherits WinApplication
		Public Sub New()
			InitializeComponent()
			DelayedViewItemsInitialization = True
		End Sub

		Private Sub E2813WindowsFormsApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs) Handles MyBase.DatabaseVersionMismatch
#If EASYTEST Then
			e.Updater.Update()
			e.Handled = True
#Else
			'if (System.Diagnostics.Debugger.IsAttached) {
				e.Updater.Update()
				e.Handled = True
			'}
'            else {
'                throw new InvalidOperationException(
'                    "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application.\r\n" +
'                    "This error occurred  because the automatic database update was disabled when the application was started without debugging.\r\n" +
'                    "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " +
'                    "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " +
'                    "or manually create a database using the 'DBUpdater' tool.\r\n" +
'                    "Anyway, refer to the 'Update Application and Database Versions' help topic at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm " +
'                    "for more detailed information. If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/");
'            }
#End If
		End Sub
	End Class
End Namespace
