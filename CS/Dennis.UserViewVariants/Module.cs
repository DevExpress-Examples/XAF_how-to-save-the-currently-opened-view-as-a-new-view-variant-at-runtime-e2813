using System;
using DevExpress.ExpressApp;
using System.Collections.Generic;

namespace Dennis.UserViewVariants {
    public sealed partial class UserViewVariantsModule : ModuleBase {
        public UserViewVariantsModule() {
            ModelDifferenceResourceName = "Dennis.UserViewVariants.Model.DesignedDiffs";
            InitializeComponent();
        }
    }
}
