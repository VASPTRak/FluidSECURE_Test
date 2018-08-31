Imports log4net
Imports log4net.Config
Imports MySql.Data.MySqlClient
Imports System.Data.SqlClient

Public Class ExternalBAL
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(ExternalBAL))

    Shared Sub New()
        XmlConfigurator.Configure()
    End Sub

    Public Function GetSubscribersFromFluidSecure(lastUser_id As Integer) As DataTable
        Dim dal = New GeneralizedDAL()
        Try

            Dim ds As DataSet = New DataSet()

            'Dim Param As MySqlParameter() = New MySqlParameter(0) {}

            'Param(0) = New MySqlParameter("lastUser_id", SqlDbType.Int)
            'Param(0).Direction = ParameterDirection.Input
            'Param(0).Value = lastUser_id

            'ds = dal.ExecuteStoredProcedureGetDataSetForMySQL("usp_tt_UserData_GetuserMetaData", Param)
            Dim strQuery As String = "select * from wp_whvhk9f94w_usermeta where user_id in (select user_id from wp_whvhk9f94w_usermeta where meta_value='a:1:{s:10:""subscriber"";b:1;}') and user_id > " & lastUser_id & " order by user_id;"

            ds = dal.ExecuteStoredProcedureGetDataSetForMySQL(strQuery)

            Return ds.Tables(0)

        Catch ex As Exception

            log.Error("Error occurred in GetSubscriberFromFluidSecure Exception is :" + ex.Message)
            Return Nothing
        Finally

        End Try
    End Function

    Public Function UpdateLastRegisteredUserEntryInSubscribers(user_id As Integer) As Integer
        Dim result As Integer
        result = 0
        Try
            Dim dal = New GeneralizedDAL()
            Dim parcollection(0) As SqlParameter

            Dim Paruser_id = New SqlParameter("@user_id", SqlDbType.Int)

            Paruser_id.Direction = ParameterDirection.Input

            Paruser_id.Value = user_id

            parcollection(0) = Paruser_id

            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_subscribers_UpdateLastRegisteredUserEntryInSubscribers", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in UpdateLastRegisteredUserEntryInSubscribers Exception is :" + ex.Message)
            Return result
        End Try

    End Function

    Public Function GetLastRegisteredUserEntryInSubscribers() As Integer
        Dim result As Integer
        result = 0
        Try
            Dim dal = New GeneralizedDAL()

            Dim parcollection() As SqlParameter = New SqlParameter() {}


            result = dal.ExecuteStoredProcedureGetInteger("usp_tt_subscribers_GetLastRegisteredUserEntryInSubscribers", parcollection)

            Return result
        Catch ex As Exception
            log.Error("Error occurred in GetLastRegisteredUserEntryInSubscribers Exception is :" + ex.Message)
            Return result
        End Try

    End Function
End Class
