using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ViewVariantsModule;

namespace UserViewVariants {
    [DomainComponent]
    [DefaultProperty("Caption")]
    public class ViewVariantParameterObject {
        private readonly IModelList<IModelVariant> variants;
        public ViewVariantParameterObject(IModelList<IModelVariant> variants) {
            this.variants = variants;
        }
        [RuleFromBoolProperty(
            "RuleFromBoolProperty_ViewVariantParameterObject.IsUniqueCaption",
            "AddViewVariantContext",
            UsedProperties = "Caption",
            CustomMessageTemplate = "You must specify a different value, because there is already a view variant with the same caption."
         )]
        [Browsable(false)]
        public bool IsUniqueCaption {
            get {
                bool ok = true;
                foreach (IModelVariant variant in variants) {
                    if (variant.Caption == Caption) {
                        ok = false;
                        break;
                    }
                }
                return ok;
            }
        }
        [RuleRequiredField("RuleRequiredField_ViewVariantParameterObject.Caption", "AddViewVariantContext")]
        public string Caption { get; set; }
    }
}