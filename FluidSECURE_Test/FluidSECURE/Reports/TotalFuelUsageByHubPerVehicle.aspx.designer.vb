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


Partial Public Class TotalFuelUsageByHubPerVehicle
    
    '''<summary>
    '''UP_Main control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents UP_Main As Global.System.Web.UI.UpdatePanel
    
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
    Protected WithEvents message As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''ErrorMessage control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents ErrorMessage As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''txtTransactionDateFrom control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTransactionDateFrom As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''txtTransactionDateTo control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTransactionDateTo As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''txtTransactionTimeFrom control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTransactionTimeFrom As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''txtTransactionTimeTo control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtTransactionTimeTo As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''DDL_Dept control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_Dept As Global.System.Web.UI.WebControls.DropDownList
    
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
    '''RDF_Customer control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RDF_Customer As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''lst_Vehicle control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents lst_Vehicle As Global.System.Web.UI.WebControls.ListBox
    
    '''<summary>
    '''DDL_HubName control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_HubName As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''btnGenarateReport control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnGenarateReport As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''Up_Vehicle control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents Up_Vehicle As Global.System.Web.UI.UpdatePanel
    
    '''<summary>
    '''HDF_VehicleId control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents HDF_VehicleId As Global.System.Web.UI.WebControls.HiddenField
    
    '''<summary>
    '''HDF_VehicleNumber control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents HDF_VehicleNumber As Global.System.Web.UI.WebControls.HiddenField
    
    '''<summary>
    '''HDF_DeptId control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents HDF_DeptId As Global.System.Web.UI.WebControls.HiddenField
    
    '''<summary>
    '''HDF_DeptNumber control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents HDF_DeptNumber As Global.System.Web.UI.WebControls.HiddenField
    
    '''<summary>
    '''lblVehicleMessage control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents lblVehicleMessage As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''UP_Fuel control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents UP_Fuel As Global.System.Web.UI.UpdatePanel
    
    '''<summary>
    '''gv_Vehicles control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents gv_Vehicles As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''btnOk control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnOk As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''lblDepartmentMessage control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents lblDepartmentMessage As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''UpdatePanel1 control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents UpdatePanel1 As Global.System.Web.UI.UpdatePanel
    
    '''<summary>
    '''grd_Dept control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents grd_Dept As Global.System.Web.UI.WebControls.GridView
    
    '''<summary>
    '''btndeptOK control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btndeptOK As Global.System.Web.UI.WebControls.Button
End Class