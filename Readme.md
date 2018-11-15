<!-- default file list -->
*Files to look at*:

* [Module.Designer.cs](./CS/Dennis.UserViewVariants/Module.Designer.cs) (VB: [Module.Designer.vb](./VB/Dennis.UserViewVariants/Module.Designer.vb))
* [UserViewVariantsController.cs](./CS/Dennis.UserViewVariants/UserViewVariantsController.cs) (VB: [UserViewVariantsController.vb](./VB/Dennis.UserViewVariants/UserViewVariantsController.vb))
* [ViewVariantParameterObject.cs](./CS/Dennis.UserViewVariants/ViewVariantParameterObject.cs) (VB: [ViewVariantParameterObject.vb](./VB/Dennis.UserViewVariants/ViewVariantParameterObject.vb))
<!-- default file list end -->
# How to save the currently opened View as a new View Variant at runtime


<p>This example provides reusable UserViewVariants modules that allow your end-users to add View Variants dynamically. Refer to the <a href="http://community.devexpress.com/blogs/eaf/archive/2011/07/04/best-practices-of-creating-reusable-xaf-modules-by-example-of-a-view-variants-module-extension.aspx"><u>Best practices of creating reusable XAF modules by example of a View Variants module extension</u></a> blog post for more information. See functional tests for the implemented functionality in the <em>UserViewVariants\Functional Tests\E2813.ets</em> file.</p>
<p><strong><br>IMPORTANT NOTES<br></strong>Due to the <u><a href="http://documentation.devexpress.com/#Xaf/CustomDocument2580">application model generation specifics on the Web</a></u> it makes sense to use this solution on the Web only if you store your end-user model differences in the database: <a href="https://www.devexpress.com/Support/Center/p/K18137">How to store users' model differences separately for each user in the database</a>.<strong><br></strong></p>
<p><strong><br>See also:</strong></p>
<p><a href="https://www.devexpress.com/Support/Center/p/T537863">How to save and share custom view settings</a></p>

<br/>


