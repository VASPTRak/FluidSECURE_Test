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


Partial Public Class ExportTransactions
    
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
    '''DDL_DateType control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_DateType As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''DDL_TransactionStatus control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_TransactionStatus As Global.System.Web.UI.WebControls.DropDownList
    
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
    '''DDL_ExportOption control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_ExportOption As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''RFD_ExportOption control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RFD_ExportOption As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''seperatorDiv control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents seperatorDiv As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''DDL_Separator control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_Separator As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''DDL_CustomizedExportTemplate control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents DDL_CustomizedExportTemplate As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''txtFileName control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents txtFileName As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''RFVFileName control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents RFVFileName As Global.System.Web.UI.WebControls.RequiredFieldValidator
    
    '''<summary>
    '''ddl_DecimalQTY control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents ddl_DecimalQTY As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''divDecimailType control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents divDecimailType As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''ddl_DecimalType control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents ddl_DecimalType As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''chk_FATransaction control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents chk_FATransaction As Global.System.Web.UI.WebControls.CheckBox
    
    '''<summary>
    '''btnExportTransactions control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents btnExportTransactions As Global.System.Web.UI.WebControls.Button
    
    '''<summary>
    '''bttnExportTemplate control.
    '''</summary>
    '''<remarks>
    '''Auto-generated field.
    '''To modify move field declaration from designer file to code-behind file.
    '''</remarks>
    Protected WithEvents bttnExportTemplate As Global.System.Web.UI.WebControls.Button
End Class
