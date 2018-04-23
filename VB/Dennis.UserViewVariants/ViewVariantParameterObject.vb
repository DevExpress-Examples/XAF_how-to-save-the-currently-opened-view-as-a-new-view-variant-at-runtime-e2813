Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.Model
Imports DevExpress.Persistent.Validation
Imports DevExpress.ExpressApp.ViewVariantsModule

Namespace Dennis.UserViewVariants
	<DomainComponent, DefaultProperty("Caption")> _
	Public Class ViewVariantParameterObject
		Private ReadOnly variants As IModelList(Of IModelVariant)
		Public Sub New(ByVal variants As IModelList(Of IModelVariant))
			Me.variants = variants
		End Sub
		<RuleFromBoolProperty("RuleFromBoolProperty_ViewVariantParameterObject.IsUniqueCaption", "AddViewVariantContext", UsedProperties := "Caption", CustomMessageTemplate := "You must specify a different value, because there is already a view variant with the same caption."), Browsable(False)> _
		Public ReadOnly Property IsUniqueCaption() As Boolean
			Get
				Dim ok As Boolean = True
				For Each [variant] As IModelVariant In variants
					If [variant].Caption = Caption Then
						ok = False
						Exit For
					End If
				Next [variant]
				Return ok
			End Get
		End Property
		Private privateCaption As String
		<RuleRequiredField("RuleRequiredField_ViewVariantParameterObject.Caption", "AddViewVariantContext")> _
		Public Property Caption() As String
			Get
				Return privateCaption
			End Get
			Set(ByVal value As String)
				privateCaption = value
			End Set
		End Property
	End Class
End Namespace