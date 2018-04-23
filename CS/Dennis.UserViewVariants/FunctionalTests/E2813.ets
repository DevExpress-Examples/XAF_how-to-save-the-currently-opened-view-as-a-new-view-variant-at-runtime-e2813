;#DropDB E2813
#Application E2813.Demo.Win
#Application E2813.Demo.Web

!ActionAvailable User View Variants(Edit View Variant)
!ActionAvailable User View Variants(Delete View Variant)

*Action User View Variants(New View Variant)
*FillForm
 Caption = New View Variant 1
*Action OK

*CheckActionValue View(New View Variant 1)
*ActionAvailable User View Variants(Edit View Variant)
*ActionAvailable User View Variants(Delete View Variant)

*Action User View Variants(Delete View Variant)
!ActionAvailable User View Variants(Edit View Variant)
!ActionAvailable User View Variants(Delete View Variant)

*Action User View Variants(New View Variant)
*FillForm
 Caption = New View Variant 2
*Action OK

*CheckActionValue View(New View Variant 2)
*Action User View Variants(Edit View Variant)
*FillForm
 Caption = New View Variant 2 Edited
*Action OK
*CheckActionValue View(New View Variant 2 Edited)

*Action View(Default)
*ActionAvailable User View Variants(Edit View Variant)
!ActionAvailable User View Variants(Delete View Variant)

*Action User View Variants(New View Variant)
*FillForm
 Caption = ''
*Action OK

*CheckValidationResult
 Message = Problems were detected during data validation. Please read the information below to understand what the issues are and how you can correct them.
 Info = "Caption" must not be empty.
*OptionalAction Close

*FillForm
 Caption = 'Default'
*Action OK

*CheckValidationResult
 Message = Problems were detected during data validation. Please read the information below to understand what the issues are and how you can correct them.
 Info = 'You must specify a different value, because there is already a view variant with the same caption.'
*OptionalAction Close