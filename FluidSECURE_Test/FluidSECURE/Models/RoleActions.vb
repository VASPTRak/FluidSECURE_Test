Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework

Public Class RoleActions
    Shared Function AddRole(RoleName As String) As Integer
        ' Access the application context and create result variables.
        Dim context As ApplicationDbContext = New ApplicationDbContext()
        Dim IdRoleResult As IdentityResult
        'Dim IdUserResult As IdentityResult

        ' Create a RoleStore object by using the ApplicationDbContext object. 
        ' The RoleStore is only allowed to contain IdentityRole objects.
        Dim roleStore = New RoleStore(Of IdentityRole)(context)

        ' Create a RoleManager object that is only allowed to contain IdentityRole objects.
        ' When creating the RoleManager object, you pass in (as a parameter) a new RoleStore object. 
        Dim roleMgr = New RoleManager(Of IdentityRole)(roleStore)

        ' Then, you create the "canEdit" role if it doesn't already exist.

        If Not roleMgr.RoleExists(RoleName) Then
            IdRoleResult = roleMgr.Create(New IdentityRole() With { _
                .Name = RoleName
            })
        Else
            Return -2
        End If

        Return 1
    End Function

    Shared Function GetRoles() As List(Of IdentityRole)
        ' Access the application context and create result variables.
        Dim context As ApplicationDbContext = New ApplicationDbContext()

        Dim roleStore = New RoleStore(Of IdentityRole)(context)

        Dim roleMgr = New RoleManager(Of IdentityRole)(roleStore)



        Dim Roles = roleMgr.Roles().ToList()
        Return Roles
    End Function


    Shared Function GetRolesByName(roleName As String) As List(Of IdentityRole)
        ' Access the application context and create result variables.
        Dim context As ApplicationDbContext = New ApplicationDbContext()

        Dim roleStore = New RoleStore(Of IdentityRole)(context)

        Dim roleMgr = New RoleManager(Of IdentityRole)(roleStore)



        Dim Roles = roleMgr.Roles().ToList()
        Dim UniueRole = New List(Of IdentityRole)

        ' Access the application context and create result
        For Each role As IdentityRole In Roles
            If (role.Name.Contains(roleName)) Then
                UniueRole.Add(role)
            End If
        Next
        Return UniueRole

    End Function

    Shared Function GetRolesById(roleid As String) As IdentityRole
        ' Access the application context and create result variables.
        Dim context As ApplicationDbContext = New ApplicationDbContext()

        Dim roleStore = New RoleStore(Of IdentityRole)(context)

        Dim roleMgr = New RoleManager(Of IdentityRole)(roleStore)



        Dim Roles = roleMgr.Roles().ToList()
        Dim UniueRole = New IdentityRole()

        ' Access the application context and create result
        For Each role As IdentityRole In Roles
            If (role.Id = roleid) Then
                UniueRole = role
            End If
        Next

        Return UniueRole

    End Function


    Shared Sub AddUserRoles()

        Dim context As ApplicationDbContext = New ApplicationDbContext()

        Dim roleStore = New RoleStore(Of IdentityRole)(context)

        Dim roleMgr = New RoleManager(Of IdentityRole)(roleStore)

        Dim IdRoleResult As IdentityResult

        If (Not roleMgr.RoleExists("User")) Then
            IdRoleResult = roleMgr.Create(New IdentityRole() With { _
                             .Name = "User" _
                         })

        End If

        If (Not roleMgr.RoleExists("SuperAdmin")) Then
            IdRoleResult = roleMgr.Create(New IdentityRole() With { _
                             .Name = "SuperAdmin" _
                         })

        End If

        If (Not roleMgr.RoleExists("CustomerAdmin")) Then
            IdRoleResult = roleMgr.Create(New IdentityRole() With { _
                             .Name = "CustomerAdmin" _
                         })

        End If

        If (Not roleMgr.RoleExists("Reports Only")) Then
            IdRoleResult = roleMgr.Create(New IdentityRole() With { _
                             .Name = "Reports Only" _
                         })

        End If

        If (Not roleMgr.RoleExists("Support")) Then
            IdRoleResult = roleMgr.Create(New IdentityRole() With {
                             .Name = "Support"
                         })

        End If

    End Sub

End Class