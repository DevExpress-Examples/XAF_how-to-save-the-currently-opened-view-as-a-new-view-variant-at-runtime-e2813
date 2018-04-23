Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports System.Collections.Generic

Namespace Dennis.UserViewVariants
	Public NotInheritable Partial Class UserViewVariantsModule
		Inherits ModuleBase
		Public Sub New()
			ModelDifferenceResourceName = "Model.DesignedDiffs"
			InitializeComponent()
		End Sub
	End Class
End Namespace
