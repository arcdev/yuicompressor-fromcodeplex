function rcbItems_ClientItemsRequesting(sender, eventArgs) {
var context = eventArgs.get_context();
context["FilterString"] = eventArgs.get_text();
}

function rcbItems_ClientItemsRequested(sender, eventArgs) {
if (eventArgs.set_cancel) eventArgs.set_cancel(true);
}

function ValidateFields(sender, args) {
var valid = true;
var rate = <%= RateType %>;
if (rate == 1) {
valid = ValidateCombobox('<%= ddlSearchEmployee.ClientID %>') && valid;
valid = ValidateRadNumericTextbox('<%= rntCustomRate.ClientID %>', 0, 999999) && valid;
} else if (rate == 2) {
valid = ValidateCombobox('<%= rcbItems.ClientID %>') && valid;
valid = ValidateRadNumericTextbox('<%= rntCustomPrice.ClientID %>', 0, 999999) && valid;
}
args.IsValid = valid;
}

function RefreshRates() {
if ($.isFunction(GetParentWindow().RefreshRates))
GetParentWindow().RefreshRates();
else GetParentWindow().__doPostBack('','');
Close();
}