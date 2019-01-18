#Region "Transaction"
Public Class TransactionsExportAPI
    Public Property TransactionFromDate() As String
        Get
            Return m_FromTransactionFromDate
        End Get
        Set(value As String)
            m_FromTransactionFromDate = value
        End Set
    End Property
    Private m_FromTransactionFromDate As String

    Public Property TransactionToDate() As String
        Get
            Return m_TransactionToDate
        End Get
        Set(value As String)
            m_TransactionToDate = value
        End Set
    End Property
    Private m_TransactionToDate As String

    Public Property CompanyName() As String
        Get
            Return m_CompanyName
        End Get
        Set(value As String)
            m_CompanyName = value
        End Set
    End Property
    Private m_CompanyName As String

End Class

Public Class RootTransactionObject
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String

    Public Property TransactionsExportDataObj() As List(Of TransactionsExportDATA)
        Get
            Return m_TransactionsExportDataObj
        End Get
        Set(value As List(Of TransactionsExportDATA))
            m_TransactionsExportDataObj = value
        End Set
    End Property
    Private m_TransactionsExportDataObj As List(Of TransactionsExportDATA)

End Class

Public Class TransactionsExportDATA
    Public Property TransactionDateTime() As String
        Get
            Return m_TransactionDateTime
        End Get
        Set(value As String)
            m_TransactionDateTime = value
        End Set
    End Property
    Private m_TransactionDateTime As String

    Public Property TransactionNumber() As String
        Get
            Return m_TransactionNumber
        End Get
        Set(value As String)
            m_TransactionNumber = value
        End Set
    End Property
    Private m_TransactionNumber As String

    Public Property CompanyName() As String
        Get
            Return m_CompanyName
        End Get
        Set(value As String)
            m_CompanyName = value
        End Set
    End Property
    Private m_CompanyName As String
    Public Property VehicleNumber() As String
        Get
            Return m_VehicleNumber
        End Get
        Set(value As String)
            m_VehicleNumber = value
        End Set
    End Property
    Private m_VehicleNumber As String

    Public Property PersonName() As String
        Get
            Return m_PersonName
        End Get
        Set(value As String)
            m_PersonName = value
        End Set
    End Property
    Private m_PersonName As String

    Public Property PersonPin() As String
        Get
            Return m_PersonPin
        End Get
        Set(value As String)
            m_PersonPin = value
        End Set
    End Property
    Private m_PersonPin As String

    Public Property FluisSecureLink() As String
        Get
            Return m_FluisSecureLink
        End Get
        Set(value As String)
            m_FluisSecureLink = value
        End Set
    End Property
    Private m_FluisSecureLink As String

    Public Property CurrentOdometer() As String
        Get
            Return m_CurrentOdometer
        End Get
        Set(value As String)
            m_CurrentOdometer = value
        End Set
    End Property
    Private m_CurrentOdometer As String

    Public Property DepartmentNumber() As String
        Get
            Return m_DepartmentNumber
        End Get
        Set(value As String)
            m_DepartmentNumber = value
        End Set
    End Property
    Private m_DepartmentNumber As String

    Public Property FuelQuantity() As String
        Get
            Return m_FuelQuantity
        End Get
        Set(value As String)
            m_FuelQuantity = value
        End Set
    End Property
    Private m_FuelQuantity As String

    Public Property FuelType() As String
        Get
            Return m_FuelType
        End Get
        Set(value As String)
            m_FuelType = value
        End Set
    End Property
    Private m_FuelType As String

    Public Property Hours() As String
        Get
            Return m_Hours
        End Get
        Set(value As String)
            m_Hours = value
        End Set
    End Property
    Private m_Hours As String

    Public Property Other() As String
        Get
            Return m_Other
        End Get
        Set(value As String)
            m_Other = value
        End Set
    End Property
    Private m_Other As String


    Public Property SiteNumber() As String
        Get
            Return m_SiteNumber
        End Get
        Set(value As String)
            m_SiteNumber = value
        End Set
    End Property
    Private m_SiteNumber As String

    Public Property TransactionCost() As String
        Get
            Return m_TransactionCost
        End Get
        Set(value As String)
            m_TransactionCost = value
        End Set
    End Property
    Private m_TransactionCost As String

End Class

Public Class TransactionsModels
    Public Property TransactionsModelsObj() As List(Of EachTransaction)
        Get
            Return m_TransactionsModelsObj
        End Get
        Set(value As List(Of EachTransaction))
            m_TransactionsModelsObj = value
        End Set
    End Property
    Private m_TransactionsModelsObj As List(Of EachTransaction)
End Class

Public Class EachTransaction
    Public Property TransactionDateTime() As String
        Get
            Return m_TransactionDateTime
        End Get
        Set(value As String)
            m_TransactionDateTime = value
        End Set
    End Property
    Private m_TransactionDateTime As String

    Public Property VehicleNumber() As String
        Get
            Return m_VehicleNumber
        End Get
        Set(value As String)
            m_VehicleNumber = value
        End Set
    End Property
    Private m_VehicleNumber As String
    Public Property PersonPIN() As String
        Get
            Return m_PersonPIN
        End Get
        Set(value As String)
            m_PersonPIN = value
        End Set
    End Property
    Private m_PersonPIN As String

    Public Property FluidSecureLink() As String
        Get
            Return m_FluidSecureLink
        End Get
        Set(value As String)
            m_FluidSecureLink = value
        End Set
    End Property
    Private m_FluidSecureLink As String

    Public Property FuelQuantity() As String
        Get
            Return m_FuelQuantity
        End Get
        Set(value As String)
            m_FuelQuantity = value
        End Set
    End Property
    Private m_FuelQuantity As String

    Public Property Odometer() As String
        Get
            Return m_Odometer
        End Get
        Set(value As String)
            m_Odometer = value
        End Set
    End Property
    Private m_Odometer As String

    Public Property Hours() As String
        Get
            Return m_Hours
        End Get
        Set(value As String)
            m_Hours = value
        End Set
    End Property
    Private m_Hours As String

    Public Property CompanyName() As String
        Get
            Return m_CompanyName
        End Get
        Set(value As String)
            m_CompanyName = value
        End Set
    End Property
    Private m_CompanyName As String

End Class

#End Region

#Region "Department"
Public Class DepartmentsExportAPI
    Public Property CompanyName() As String
        Get
            Return m_CompanyName
        End Get
        Set(value As String)
            m_CompanyName = value
        End Set
    End Property
    Private m_CompanyName As String
End Class

Public Class RootDepartmentObject
    Public Property ResponceMessage() As String
        Get
            Return m_ResponceMessage
        End Get
        Set(value As String)
            m_ResponceMessage = value
        End Set
    End Property
    Private m_ResponceMessage As String
    Public Property ResponceText() As String
        Get
            Return m_ResponceText
        End Get
        Set(value As String)
            m_ResponceText = value
        End Set
    End Property
    Private m_ResponceText As String

    Public Property DepartmentExportDataObj() As List(Of DepartmentPassingModel)
        Get
            Return m_DepartmentExportDataObj
        End Get
        Set(value As List(Of DepartmentPassingModel))
            m_DepartmentExportDataObj = value
        End Set
    End Property
    Private m_DepartmentExportDataObj As List(Of DepartmentPassingModel)

End Class

Public Class DepartmentPassingModel
    Public Property CompanyName() As String
        Get
            Return m_CompanyName
        End Get
        Set(value As String)
            m_CompanyName = value
        End Set
    End Property
    Private m_CompanyName As String

    Public Property DepartmentName() As String
        Get
            Return m_DepartmentName
        End Get
        Set(value As String)
            m_DepartmentName = value
        End Set
    End Property
    Private m_DepartmentName As String

    Public Property DepartmentNumber() As String
        Get
            Return m_DepartmentNumber
        End Get
        Set(value As String)
            m_DepartmentNumber = value
        End Set
    End Property
    Private m_DepartmentNumber As String

    Public Property Address() As String
        Get
            Return m_Address
        End Get
        Set(value As String)
            m_Address = value
        End Set
    End Property
    Private m_Address As String
    Public Property Address2() As String
        Get
            Return m_Address2
        End Get
        Set(value As String)
            m_Address2 = value
        End Set
    End Property
    Private m_Address2 As String

    Public Property AccountNumber() As String
        Get
            Return m_AccountNumber
        End Get
        Set(value As String)
            m_AccountNumber = value
        End Set
    End Property
    Private m_AccountNumber As String

    Public Property ExportCode() As String
        Get
            Return m_ExportCode
        End Get
        Set(value As String)
            m_ExportCode = value
        End Set
    End Property
    Private m_ExportCode As String

End Class


Public Class DepartmentModels
    Public Property DepartmentPassingObj() As List(Of DepartmentPassingModel)
        Get
            Return m_DepartmentPassingObjObj
        End Get
        Set(value As List(Of DepartmentPassingModel))
            m_DepartmentPassingObjObj = value
        End Set
    End Property
    Private m_DepartmentPassingObjObj As List(Of DepartmentPassingModel)
End Class


#End Region
