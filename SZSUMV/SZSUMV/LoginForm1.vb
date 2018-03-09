Imports System.DirectoryServices

Public Class LoginForm1
    Private Gv_sPath As String = "LDAP://szs.com.tw/DC=szs,DC=com,DC=tw"
    Private Gv_sFilterAttribute As String = ""

    ' TODO: 插入程式碼，利用提供的使用者名稱和密碼執行自訂驗證
    ' (請參閱 http://go.microsoft.com/fwlink/?LinkId=35339)。
    ' 如此便可將自訂主體附加到目前執行緒的主體，如下所示: 
    '     My.User.CurrentPrincipal = CustomPrincipal
    ' 其中 CustomPrincipal 是用來執行驗證的 IPrincipal 實作。
    ' 接著，My.User 便會傳回封裝在 CustomPrincipal 物件中的識別資訊，
    ' 例如使用者名稱、顯示名稱等。

    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        Dim Lv_bResult As Boolean = False
        Try
            Lv_bResult = IsAuthenticated("szs.com.tw", UsernameTextBox.Text, PasswordTextBox.Text)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        If Lv_bResult Then
            DialogResult = DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
    Public Function IsAuthenticated(ByVal domain As String, ByVal username As String, ByVal pwd As String) As Boolean

        Dim domainAndUsername As String = domain & "\" & username
        Dim entry As DirectoryEntry = New DirectoryEntry(Gv_sPath, username, pwd)

        Try
            'Bind to the native AdsObject to force authentication.
            Dim obj As Object = entry.NativeObject
            Dim search As DirectorySearcher = New DirectorySearcher(entry)

            search.Filter = "(SAMAccountName=" & username & ")"
            search.PropertiesToLoad.Add("cn")
            Dim result As SearchResult = search.FindOne()

            If (result Is Nothing) Then
                Return False
            End If

            'Update the new path to the user in the directory.
            Gv_sPath = result.Path
            Gv_sFilterAttribute = CType(result.Properties("cn")(0), String)

        Catch ex As Exception
            Throw New Exception("Error authenticating user. " & ex.Message)
        End Try

        Return True
    End Function

End Class
