﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated. 
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class Tank
    
    '''<summary>
    '''up_Main control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents up_Main As Global.System.Web.UI.UpdatePanel
    
    '''<summary>
    '''lblHeader control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents lblHeader As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''message control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents message As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''ErrorMessage control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents ErrorMessage As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''txtTankNo control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTankNo As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''RFDTankNo control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RFDTankNo As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''txtTankIdHide control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTankIdHide As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''HDF_TotalTank control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents HDF_TotalTank As Global.System.Web.UI.WebControls.HiddenField
    
    '''<summary>
    '''txtTankName control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTankName As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''RFVtxtTank control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RFVtxtTank As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''ddlFuelType control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents ddlFuelType As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''RFVFuelType control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RFVFuelType As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''txtExportCode control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtExportCode As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''txtAddress control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtAddress As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''txtPROBEMacAddress control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtPROBEMacAddress As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''txtRefillNotice control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtRefillNotice As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''CV_RefillNotice control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents CV_RefillNotice As Global.System.Web.UI.WebControls.CompareValidator
    
    '''<summary>
    '''divConstantA control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents divConstantA As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''txtConstantA control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtConstantA As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''CVConstantA control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents CVConstantA As Global.System.Web.UI.WebControls.CompareValidator
    
    '''<summary>
    '''Chk_TankMonitor control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
   Protected WithEvents Chk_TankMonitor As Global.System.Web.UI.WebControls.CheckBox
    
    '''<summary>
    '''divConstantB control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents divConstantB As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''txtConstantB control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtConstantB As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''CVConstantB control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents CVConstantB As Global.System.Web.UI.WebControls.CompareValidator
    
    '''<summary>
    '''txtTankMonitorNo control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTankMonitorNo As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''CVTankMonitorNumber control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
   Protected WithEvents CVTankMonitorNumber As Global.System.Web.UI.WebControls.CompareValidator
    
    '''<summary>
    '''divConstantC control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents divConstantC As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''txtConstantC control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtConstantC As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''CVConstantC control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents CVConstantC As Global.System.Web.UI.WebControls.CompareValidator
    
    '''<summary>
    '''ddlTankChart control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents ddlTankChart As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''divConstantD control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents divConstantD As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''txtConstantD control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtConstantD As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''CVConstantD control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents CVConstantD As Global.System.Web.UI.WebControls.CompareValidator
    
    '''<summary>
    '''divCompany control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents divCompany As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''DDL_Customer control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_Customer As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''RFV_Cust control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RFV_Cust As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''btnSave control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnSave As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''btnCancel control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnCancel As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''btnSaveAndAddNew control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnSaveAndAddNew As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''btnFirst control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnFirst As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''btnprevious control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnprevious As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''lblof control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents lblof As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''btnNext control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnNext As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''btnLast control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnLast As Global.System.Web.UI.WebControls.Button
End Class
