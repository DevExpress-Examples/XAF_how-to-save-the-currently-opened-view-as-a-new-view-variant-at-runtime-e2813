using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.ViewVariantsModule;

namespace Dennis.UserViewVariants {
    public class UserViewVariantsController : ViewController {
        private const string STR_HasViewVariants_EnabledKey = "HasViewVariants";
        private const string STR_IsRootViewVariant_EnabledKey = "IsRootViewVariant";
        private const string STR_NewViewVariant_Id = "NewViewVariant";
        private const string STR_DeleteViewVariant_Id = "DeleteViewVariant";
        private const string STR_EditViewVariant_Id = "EditViewVariant";
        private const string STR_UserViewVariants_Image = "Action_Copy";
        private const string STR_NewViewVariant_Image = "Action_New";
        private const string STR_DeleteViewVariant_Image = "Action_Delete";
        private const string STR_EditViewVariant_Image = "Action_Edit";
        private const string STR_UserViewVariants_Id = "UserViewVariants";

        private readonly SingleChoiceAction userViewVariantsCore;
        protected ChangeVariantMainWindowController changeVariantMainWindowController;
        protected ChangeVariantController changeVariantController;
        protected IModelList<IModelVariant> rootModelViewVariants;
        private IModelList<IModelView> modelViews;

        public UserViewVariantsController() {
            userViewVariantsCore = new SingleChoiceAction(this, STR_UserViewVariants_Id, PredefinedCategory.View) {
                ImageName = STR_UserViewVariants_Image,
                PaintStyle = ActionItemPaintStyle.CaptionAndImage,
                Caption = CaptionHelper.ConvertCompoundName(STR_UserViewVariants_Id),
                ItemType = SingleChoiceActionItemType.ItemIsOperation,
                ShowItemsOnClick = true
            };
            ChoiceActionItem addViewVariantItem = new ChoiceActionItem(STR_NewViewVariant_Id, CaptionHelper.ConvertCompoundName(STR_NewViewVariant_Id), STR_NewViewVariant_Id) {
                ImageName = STR_NewViewVariant_Image
            };
            ChoiceActionItem removeViewVariantItem = new ChoiceActionItem(STR_DeleteViewVariant_Id, CaptionHelper.ConvertCompoundName(STR_DeleteViewVariant_Id), STR_DeleteViewVariant_Id) {
                ImageName = STR_DeleteViewVariant_Image
            };
            ChoiceActionItem editViewVariantItem = new ChoiceActionItem(STR_EditViewVariant_Id, CaptionHelper.ConvertCompoundName(STR_EditViewVariant_Id), STR_EditViewVariant_Id) {
                ImageName = STR_EditViewVariant_Image
            };
            userViewVariantsCore.Items.Add(addViewVariantItem);
            userViewVariantsCore.Items.Add(editViewVariantItem);
            userViewVariantsCore.Items.Add(removeViewVariantItem);
            userViewVariantsCore.Execute += UserViewVariants_Execute;
        }
        private void UserViewVariants_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            UserViewVariants(e);
        }
        protected virtual void UserViewVariants(SingleChoiceActionExecuteEventArgs e) {
            string data = Convert.ToString(e.SelectedChoiceActionItem.Data);
            if (data == STR_NewViewVariant_Id || data == STR_EditViewVariant_Id)
                ShowViewVariantParameterDialog(e, data);
            else if (data == STR_DeleteViewVariant_Id)
                DeleteViewVariant();
        }
        protected override void OnActivated() {
            base.OnActivated();
            Initialize();
            UpdateUserViewVariantsAction();
        }
        protected override void Dispose(bool disposing) {
            if (disposing)
                UnsubscribeFromEvents();
            base.Dispose(disposing);
        }
        private void UnsubscribeFromEvents() {
            userViewVariantsCore.Execute -= UserViewVariants_Execute;
            if (changeVariantController != null && changeVariantController.ChangeVariantAction != null)
                changeVariantController.ChangeVariantAction.Execute -= ChangeVariantAction_Executed;
        }
        private void Initialize() {
            changeVariantMainWindowController = Frame.GetController<ChangeVariantMainWindowController>();
            changeVariantController = Frame.GetController<ChangeVariantController>();
            changeVariantController.ChangeVariantAction.Executed += ChangeVariantAction_Executed;
            modelViews = (IModelList<IModelView>)View.Model.Application.Views;
            rootModelViewVariants = (IModelList<IModelVariant>)((IModelViewVariants)modelViews[GetRootViewId()]).Variants;
        }
        private void ChangeVariantAction_Executed(object sender, ActionBaseEventArgs e) {
            UpdateUserViewVariantsAction();
        }
        private string GetRootViewId() {
            VariantsInfo variantsInfo = changeVariantMainWindowController.GetVariants(View);
            return variantsInfo != null ? variantsInfo.RootViewId : View.Id;
        }
        protected virtual void ShowViewVariantParameterDialog(SingleChoiceActionExecuteEventArgs e, string mode) {
            string viewCaption = string.Empty;
            ViewVariantParameterObject parameter = new ViewVariantParameterObject(rootModelViewVariants);
            if (mode == STR_NewViewVariant_Id) {
                parameter.Caption = string.Format("{0}_{1:g}", View.Caption, DateTime.Now);
                viewCaption = CaptionHelper.GetLocalizedText("Texts", "NewViewVariantParameterCaption");
            }
            if (mode == STR_EditViewVariant_Id && changeVariantController.ChangeVariantAction.SelectedItem != null) {
                parameter.Caption = changeVariantController.ChangeVariantAction.SelectedItem.Caption;
                viewCaption = CaptionHelper.GetLocalizedText("Texts", "EditViewVariantParameterCaption");
            }
            DialogController dialogController = Application.CreateController<DialogController>();
            dialogController.Accepting += dialogController_Accepting;
            dialogController.Tag = mode;
            DetailView dv = Application.CreateDetailView(ObjectSpaceInMemory.CreateNew(), parameter, false);
            dv.ViewEditMode = ViewEditMode.Edit;
            dv.Caption = viewCaption;
            e.ShowViewParameters.CreatedView = dv;
            e.ShowViewParameters.Controllers.Add(dialogController);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        }
        protected void dialogController_Accepting(object sender, DialogControllerAcceptingEventArgs e) {
            DialogController dialogController = (DialogController)sender;
            dialogController.Accepting -= dialogController_Accepting;
            string data = Convert.ToString(dialogController.Tag);
            ViewVariantParameterObject parameter = dialogController.Window.View.CurrentObject as ViewVariantParameterObject;
            if (data == STR_NewViewVariant_Id)
                NewViewVariant(parameter);
            else if (data == STR_EditViewVariant_Id)
                EditViewVariant(parameter);
        }
        protected virtual void NewViewVariant(ViewVariantParameterObject parameter) {
            //It is necessary to save the current View settings into the application model before copying them.
            View.SaveModel();
            //Identifier of a new view variant will be based on the identifier of the root view variant.
            string newViewVariantId = String.Format("{0}_{1}", GetRootViewId(), Guid.NewGuid());
            // Adds a new child node of the IModelVariant type with a specific identifier to the parent IModelViewVariants node.
            IModelVariant newModelViewVariant = ((ModelNode)rootModelViewVariants).AddNode<IModelVariant>(newViewVariantId);
            // Creates a new node of the IModelView type by cloning the settings of the current View and then sets the clone to the View property of the view variant created above.
            newModelViewVariant.View = ((ModelNode)modelViews).AddClonedNode((ModelNode)View.Model, newViewVariantId) as IModelView;
            //Sets the Caption property of the view variant created above to the caption specified by an end-user in the dialog.
            newModelViewVariant.Caption = parameter.Caption;
            //It is necessary to add a default view variant node for the current View for correct operation of the Change Variant Action.
            if (rootModelViewVariants.Count == 1) {
                IModelVariant currentModelViewVariant = ((ModelNode)rootModelViewVariants).AddNode<IModelVariant>(View.Id);
                currentModelViewVariant.Caption = CaptionHelper.GetLocalizedText("Texts", "DefaultViewVariantCaption");
                currentModelViewVariant.View = View.Model;
            }
            //Updates the Change Variant Action structure based on the model customizations above.
            changeVariantController.RefreshVariantsAction();
            //Sets the current view variant to the newly created one.
            UpdateCurrentViewVariant(true);
            //Updates the items of our User View Variant Action based on the current the Change Variant Action structure. 
            UpdateUserViewVariantsAction();
        }
        //This method does almost the same work as NewViewVariant, but in reverse order.
        protected virtual void DeleteViewVariant() {
            VariantsInfo variantsInfo = GetVariantsInfo();
            //You should not be able to remove the root view variant.
            if (variantsInfo != null && variantsInfo.CurrentVariantId != GetRootViewId()) {
                UpdateCurrentViewVariant(false);
                rootModelViewVariants[variantsInfo.CurrentVariantId].Remove();
                modelViews[variantsInfo.CurrentVariantId].Remove();
                changeVariantController.RefreshVariantsAction();
                UpdateUserViewVariantsAction();
            }
            if (rootModelViewVariants.Count == 1)
                rootModelViewVariants.ClearNodes();
        }
        protected virtual void EditViewVariant(ViewVariantParameterObject parameter) {
            VariantsInfo variantsInfo = GetVariantsInfo();
            if (variantsInfo != null)
                rootModelViewVariants[variantsInfo.CurrentVariantId].Caption = parameter.Caption;
            changeVariantController.RefreshVariantsAction();
        }
        private void UpdateCurrentViewVariant(bool isNew) {
            SingleChoiceAction action = changeVariantController.ChangeVariantAction;
            if (!isNew && action.Items.Count > 1)
                action.DoExecute(action.Items[action.Items.Count - 2]);
            if (isNew && action.Items.Count > 0)
                action.DoExecute(action.Items[action.Items.Count - 1]);
        }
        private void UpdateUserViewVariantsAction() {
            bool hasViewVariants = changeVariantController.ChangeVariantAction.Items.Count > 0;
            UserViewVarintsAction.Items.FindItemByID(STR_DeleteViewVariant_Id).Enabled[STR_HasViewVariants_EnabledKey] = UserViewVarintsAction.Items.FindItemByID(STR_EditViewVariant_Id).Enabled[STR_HasViewVariants_EnabledKey] = hasViewVariants;

            if (changeVariantController.ChangeVariantAction.SelectedItem != null) {
                VariantInfo variantInfo = (VariantInfo)changeVariantController.ChangeVariantAction.SelectedItem.Data;
                UserViewVarintsAction.Items.FindItemByID(STR_DeleteViewVariant_Id).Enabled[STR_IsRootViewVariant_EnabledKey] = variantInfo.ViewID != GetRootViewId();
            }
        }
        private VariantsInfo GetVariantsInfo() {
            return changeVariantMainWindowController.GetVariants(GetRootViewId());
        }
        public SingleChoiceAction UserViewVarintsAction {
            get { return userViewVariantsCore; }
        }
    }
}