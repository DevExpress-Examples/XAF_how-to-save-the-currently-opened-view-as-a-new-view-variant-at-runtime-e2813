Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Utils
Imports DevExpress.ExpressApp.Model
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.ExpressApp.Templates
Imports DevExpress.ExpressApp.Model.Core
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.ExpressApp.ViewVariantsModule

Namespace Dennis.UserViewVariants
	Public Class UserViewVariantsController
		Inherits ViewController
		Private Const STR_HasViewVariants_EnabledKey As String = "HasViewVariants"
		Private Const STR_IsRootViewVariant_EnabledKey As String = "IsRootViewVariant"
		Private Const STR_NewViewVariant_Id As String = "NewViewVariant"
		Private Const STR_DeleteViewVariant_Id As String = "DeleteViewVariant"
		Private Const STR_EditViewVariant_Id As String = "EditViewVariant"
		Private Const STR_UserViewVariants_Image As String = "Action_Copy"
		Private Const STR_NewViewVariant_Image As String = "Action_New"
		Private Const STR_DeleteViewVariant_Image As String = "Action_Delete"
		Private Const STR_EditViewVariant_Image As String = "Action_Edit"
		Private Const STR_UserViewVariants_Id As String = "UserViewVariants"

		Private ReadOnly userViewVariantsCore As SingleChoiceAction
		Protected changeVariantMainWindowController As ChangeVariantMainWindowController
		Protected changeVariantController As ChangeVariantController
		Protected rootModelViewVariants As IModelList(Of IModelVariant)
		Private modelViews As IModelList(Of IModelView)

		Public Sub New()
			userViewVariantsCore = New SingleChoiceAction(Me, STR_UserViewVariants_Id, PredefinedCategory.View) With {.ImageName = STR_UserViewVariants_Image, .PaintStyle = ActionItemPaintStyle.CaptionAndImage, .Caption = CaptionHelper.ConvertCompoundName(STR_UserViewVariants_Id), .ItemType = SingleChoiceActionItemType.ItemIsOperation, .ShowItemsOnClick = True}
			Dim addViewVariantItem As New ChoiceActionItem(STR_NewViewVariant_Id, CaptionHelper.ConvertCompoundName(STR_NewViewVariant_Id), STR_NewViewVariant_Id) With {.ImageName = STR_NewViewVariant_Image}
			Dim removeViewVariantItem As New ChoiceActionItem(STR_DeleteViewVariant_Id, CaptionHelper.ConvertCompoundName(STR_DeleteViewVariant_Id), STR_DeleteViewVariant_Id) With {.ImageName = STR_DeleteViewVariant_Image}
			Dim editViewVariantItem As New ChoiceActionItem(STR_EditViewVariant_Id, CaptionHelper.ConvertCompoundName(STR_EditViewVariant_Id), STR_EditViewVariant_Id) With {.ImageName = STR_EditViewVariant_Image}
			userViewVariantsCore.Items.Add(addViewVariantItem)
			userViewVariantsCore.Items.Add(editViewVariantItem)
			userViewVariantsCore.Items.Add(removeViewVariantItem)
			AddHandler userViewVariantsCore.Execute, AddressOf UserViewVariants_Execute
		End Sub
		Private Sub UserViewVariants_Execute(ByVal sender As Object, ByVal e As SingleChoiceActionExecuteEventArgs)
			UserViewVariants(e)
		End Sub
		Protected Overridable Sub UserViewVariants(ByVal e As SingleChoiceActionExecuteEventArgs)
			Dim data As String = Convert.ToString(e.SelectedChoiceActionItem.Data)
			If data = STR_NewViewVariant_Id OrElse data = STR_EditViewVariant_Id Then
				ShowViewVariantParameterDialog(e, data)
			ElseIf data = STR_DeleteViewVariant_Id Then
				DeleteViewVariant()
			End If
		End Sub
		Protected Overrides Overloads Sub OnActivated()
			MyBase.OnActivated()
			Initialize()
			UpdateUserViewVariantsAction()
		End Sub
		Protected Overrides Overloads Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				UnsubscribeFromEvents()
			End If
			MyBase.Dispose(disposing)
		End Sub
		Private Sub UnsubscribeFromEvents()
			RemoveHandler userViewVariantsCore.Execute, AddressOf UserViewVariants_Execute
			If changeVariantController IsNot Nothing AndAlso changeVariantController.ChangeVariantAction IsNot Nothing Then
				RemoveHandler changeVariantController.ChangeVariantAction.Execute, AddressOf ChangeVariantAction_Executed
			End If
		End Sub
		Private Sub Initialize()
			changeVariantMainWindowController = Frame.GetController(Of ChangeVariantMainWindowController)()
			changeVariantController = Frame.GetController(Of ChangeVariantController)()
			AddHandler changeVariantController.ChangeVariantAction.Executed, AddressOf ChangeVariantAction_Executed
			modelViews = CType(View.Model.Application.Views, IModelList(Of IModelView))
			rootModelViewVariants = CType((CType(modelViews(GetRootViewId()), IModelViewVariants)).Variants, IModelList(Of IModelVariant))
		End Sub
		Private Sub ChangeVariantAction_Executed(ByVal sender As Object, ByVal e As ActionBaseEventArgs)
			UpdateUserViewVariantsAction()
		End Sub
		Private Function GetRootViewId() As String
			Dim variantsInfo As VariantsInfo = changeVariantMainWindowController.GetVariants(View)
			Return If(variantsInfo IsNot Nothing, variantsInfo.RootViewId, View.Id)
		End Function
		Protected Overridable Sub ShowViewVariantParameterDialog(ByVal e As SingleChoiceActionExecuteEventArgs, ByVal mode As String)
			Dim viewCaption As String = String.Empty
			Dim parameter As New ViewVariantParameterObject(rootModelViewVariants)
			If mode = STR_NewViewVariant_Id Then
				parameter.Caption = String.Format("{0}_{1:g}", View.Caption, DateTime.Now)
				viewCaption = CaptionHelper.GetLocalizedText("Texts", "NewViewVariantParameterCaption")
			End If
			If mode = STR_EditViewVariant_Id AndAlso changeVariantController.ChangeVariantAction.SelectedItem IsNot Nothing Then
				parameter.Caption = changeVariantController.ChangeVariantAction.SelectedItem.Caption
				viewCaption = CaptionHelper.GetLocalizedText("Texts", "EditViewVariantParameterCaption")
			End If
			Dim dialogController As DialogController = Application.CreateController(Of DialogController)()
			AddHandler dialogController.Accepting, AddressOf dialogController_Accepting
			dialogController.Tag = mode
			Dim dv As DetailView = Application.CreateDetailView(ObjectSpaceInMemory.CreateNew(), parameter, False)
			dv.ViewEditMode = ViewEditMode.Edit
			dv.Caption = viewCaption
			e.ShowViewParameters.CreatedView = dv
			e.ShowViewParameters.Controllers.Add(dialogController)
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow
		End Sub
		Protected Sub dialogController_Accepting(ByVal sender As Object, ByVal e As DialogControllerAcceptingEventArgs)
			Dim dialogController As DialogController = CType(sender, DialogController)
			RemoveHandler dialogController.Accepting, AddressOf dialogController_Accepting
			Dim data As String = Convert.ToString(dialogController.Tag)
			Dim parameter As ViewVariantParameterObject = TryCast(dialogController.Window.View.CurrentObject, ViewVariantParameterObject)
			If data = STR_NewViewVariant_Id Then
				NewViewVariant(parameter)
			ElseIf data = STR_EditViewVariant_Id Then
				EditViewVariant(parameter)
			End If
		End Sub
		Protected Overridable Sub NewViewVariant(ByVal parameter As ViewVariantParameterObject)
			'It is necessary to save the current View settings into the application model before copying them.
			View.SaveModel()
			'Identifier of a new view variant will be based on the identifier of the root view variant.
			Dim newViewVariantId As String = String.Format("{0}_{1}", GetRootViewId(), Guid.NewGuid())
			' Adds a new child node of the IModelVariant type with a specific identifier to the parent IModelViewVariants node.
			Dim newModelViewVariant As IModelVariant = (CType(rootModelViewVariants, ModelNode)).AddNode(Of IModelVariant)(newViewVariantId)
			' Creates a new node of the IModelView type by cloning the settings of the current View and then sets the clone to the View property of the view variant created above.
			newModelViewVariant.View = TryCast((CType(modelViews, ModelNode)).AddClonedNode(CType(View.Model, ModelNode), newViewVariantId), IModelView)
			'Sets the Caption property of the view variant created above to the caption specified by an end-user in the dialog.
			newModelViewVariant.Caption = parameter.Caption
			'It is necessary to add a default view variant node for the current View for correct operation of the Change Variant Action.
			If rootModelViewVariants.Count = 1 Then
				Dim currentModelViewVariant As IModelVariant = (CType(rootModelViewVariants, ModelNode)).AddNode(Of IModelVariant)(View.Id)
				currentModelViewVariant.Caption = CaptionHelper.GetLocalizedText("Texts", "DefaultViewVariantCaption")
				currentModelViewVariant.View = View.Model
			End If
			'Updates the Change Variant Action structure based on the model customizations above.
			changeVariantController.RefreshVariantsAction()
			'Sets the current view variant to the newly created one.
			UpdateCurrentViewVariant(True)
			'Updates the items of our User View Variant Action based on the current the Change Variant Action structure. 
			UpdateUserViewVariantsAction()
		End Sub
		'This method does almost the same work as NewViewVariant, but in reverse order.
		Protected Overridable Sub DeleteViewVariant()
			Dim variantsInfo As VariantsInfo = GetVariantsInfo()
			'You should not be able to remove the root view variant.
			If variantsInfo IsNot Nothing AndAlso variantsInfo.CurrentVariantId <> GetRootViewId() Then
				UpdateCurrentViewVariant(False)
				rootModelViewVariants(variantsInfo.CurrentVariantId).Remove()
				modelViews(variantsInfo.CurrentVariantId).Remove()
				changeVariantController.RefreshVariantsAction()
				UpdateUserViewVariantsAction()
			End If
			If rootModelViewVariants.Count = 1 Then
				rootModelViewVariants.ClearNodes()
			End If
		End Sub
		Protected Overridable Sub EditViewVariant(ByVal parameter As ViewVariantParameterObject)
			Dim variantsInfo As VariantsInfo = GetVariantsInfo()
			If variantsInfo IsNot Nothing Then
				rootModelViewVariants(variantsInfo.CurrentVariantId).Caption = parameter.Caption
			End If
			changeVariantController.RefreshVariantsAction()
		End Sub
		Private Sub UpdateCurrentViewVariant(ByVal isNew As Boolean)
			Dim action As SingleChoiceAction = changeVariantController.ChangeVariantAction
			If (Not isNew) AndAlso action.Items.Count > 1 Then
				action.DoExecute(action.Items(action.Items.Count - 2))
			End If
			If isNew AndAlso action.Items.Count > 0 Then
				action.DoExecute(action.Items(action.Items.Count - 1))
			End If
		End Sub
		Private Sub UpdateUserViewVariantsAction()
			Dim hasViewVariants As Boolean = changeVariantController.ChangeVariantAction.Items.Count > 0
			UserViewVarintsAction.Items.FindItemByID(STR_EditViewVariant_Id).Enabled(STR_HasViewVariants_EnabledKey) = hasViewVariants
			UserViewVarintsAction.Items.FindItemByID(STR_DeleteViewVariant_Id).Enabled(STR_HasViewVariants_EnabledKey) = UserViewVarintsAction.Items.FindItemByID(STR_EditViewVariant_Id).Enabled(STR_HasViewVariants_EnabledKey)

			If changeVariantController.ChangeVariantAction.SelectedItem IsNot Nothing Then
				Dim variantInfo As VariantInfo = CType(changeVariantController.ChangeVariantAction.SelectedItem.Data, VariantInfo)
				UserViewVarintsAction.Items.FindItemByID(STR_DeleteViewVariant_Id).Enabled(STR_IsRootViewVariant_EnabledKey) = variantInfo.ViewID <> GetRootViewId()
			End If
		End Sub
		Private Function GetVariantsInfo() As VariantsInfo
			Return changeVariantMainWindowController.GetVariants(GetRootViewId())
		End Function
		Public ReadOnly Property UserViewVarintsAction() As SingleChoiceAction
			Get
				Return userViewVariantsCore
			End Get
		End Property
	End Class
End Namespace