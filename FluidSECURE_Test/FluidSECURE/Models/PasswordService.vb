Public Class PasswordService

#Region "static members"
    Private Shared minimumLowerCaseChars As Integer = 2
    Private Shared minimumUpperCaseChars As Integer = 1
    Private Shared minimumNumericChars As Integer = 2
    Private Shared minimumSpecialChars As Integer = 1

    Private Shared allLowerCaseChars As String
    Private Shared allUpperCaseChars As String
    Private Shared allNumericChars As String
    Private Shared allSpecialChars As String
    Private Shared rnd As Random
#End Region


    Shared Sub New()

        ' Ranges not using confusing characters
        allLowerCaseChars = "abcdefghjkmnpqrstuvwxyz"
        allUpperCaseChars = "ABCDEFGHJKMNPQRSTUVWXYZ"
        allNumericChars = "123456789"
        allSpecialChars = "!@#%*()$?+-="


        rnd = New Random()
    End Sub


    Public Shared Function GeneratePassword() As String
        ' Get the required number of characters of each catagory and 
        ' add random charactes of all catagories
        Dim result = Convert.ToString(Convert.ToString(GetRandomString(allLowerCaseChars, minimumLowerCaseChars) & GetRandomString(allUpperCaseChars, minimumUpperCaseChars)) & GetRandomString(allSpecialChars, minimumSpecialChars) & GetRandomString(allNumericChars, minimumNumericChars))

        ' Shuffle the result 
        Dim arr = result.ToCharArray()
        result = New String(Shuffle(arr))
        Return result
    End Function

#Region "private methods"
    Private Shared Function GetRandomString(possibleChars As String, length As Integer) As String
        Dim result = String.Empty
        For position As Integer = 0 To length - 1
            Dim index = rnd.[Next](possibleChars.Length)
            result += possibleChars(index)
        Next
        Return result
    End Function

    Private Shared Function Shuffle(Of T)(list As T()) As T()
        Dim rnd As New Random()
        For i As Integer = 0 To list.Length - 1
            Dim temp = list(i)
            Dim randomIndex As Integer = rnd.[Next](i + 1)
            list(i) = list(randomIndex)
            list(randomIndex) = temp
        Next
        Return list
    End Function
#End Region
End Class
