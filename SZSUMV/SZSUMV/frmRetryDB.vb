Imports Oracle.ManagedDataAccess.Client ' ODP.NET, Managed Driver
Public Class frmRetryDB
    Implements IDisposable
    Private Gv_SQL As String
    Private Gv_ORGID As String
    Private Gv_TRAN As OracleTransaction
    Private Gv_ReTryInterval As Integer = 5000
    Private Gv_ReTryTime As Integer
    Private Gv_ReTryLimit As Integer = 5
    Private Gv_RETRYTYPE As Integer '0.GetConnecttion 1.GetSingleDATA 2.GetSQLDATA
    Private Gv_IS_RETRY As Boolean = False
    Private Gv_StatusMessage As String '狀態訊息
    Private Gv_TimmerCount As Integer = -1
    Private Gv_SQLResultStr As String '取得資料內容
    Private Gv_ISError As Boolean = True '是否連線異常
    Private Gv_OraConnection As OracleConnection
    Private Shared Gv_Connstring As String         '連線字串

    Public Sub New(RETRYTYPE As Integer, SQL As String, ORGID As String, TRAN As OracleTransaction, Optional ReTryLimit As Integer = 5, Optional ReTryInterval As Integer = 5000)
        ' 設計工具需要此呼叫。
        InitializeComponent()
        ' 在 InitializeComponent() 呼叫之後加入所有初始設定。
        Pvt_SetDefaultParm()
        Gv_SQL = SQL
        Gv_ORGID = ORGID
        Gv_TRAN = TRAN
        Gv_RETRYTYPE = RETRYTYPE
        Gv_ReTryInterval = ReTryInterval
        Gv_ReTryLimit = ReTryLimit

    End Sub
    Public Sub New(CONNSTR As String)
        ' 設計工具需要此呼叫。
        InitializeComponent()
        ' 在 InitializeComponent() 呼叫之後加入所有初始設定。
        Console.WriteLine("新建物件" + CONNSTR)
        Pvt_SetDefaultParm()
        Gv_Connstring = CONNSTR
        Gv_RETRYTYPE = 0

    End Sub


    Protected Overrides Sub Finalize()
        Gv_OraConnection.Dispose()
    End Sub
    Private Sub Pvt_SetDefaultParm()
        Gv_ReTryTime = 0
        tmCheck.Enabled = False
        tmCheck.Interval = 1000
        btnRetry.Enabled = False
        btnClose.Enabled = True
    End Sub
    Public Function RETRY_GetSingleDATA() As String
        Gv_RETRYTYPE = 1
        bgwQuery.RunWorkerAsync()
        'tmCheck.Enabled = True
        Me.ShowDialog()
        Return Gv_SQLResultStr
    End Function
    Public Function GetConnecttion() As OracleConnection
        Console.WriteLine("外部呼叫GetConnecttion" + Gv_Connstring)
        Gv_RETRYTYPE = 0
        bgwQuery.RunWorkerAsync()
        Me.ShowDialog()
        If Gv_ISError Then
            Return Nothing
        Else
            Return Gv_OraConnection
        End If

    End Function
    Private Function pvt_GetSingleDATA(SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As String
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        pvt_GetSingleDATA = ""
        Try
            Lv_oCNT = GetConnecttion()
            Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;End;", Lv_oCNT)
            If Not IsNothing(TARN) Then
                Lv_oCMD.Transaction = TARN
            End If
            Lv_oCMD.ExecuteNonQuery()
            Lv_oCMD.CommandText = SQL
            Dim Lv_sResult As String = ""
            Dim Lv_drData As OracleDataReader = Lv_oCMD.ExecuteReader
            If Lv_drData.HasRows() Then
                Do While Lv_drData.Read()
                    Lv_sResult = Lv_drData(0).ToString
                    Return Lv_sResult
                Loop
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            'Throw New ArgumentException("GetSingleDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
        Finally
            ' 釋放資源
            If Not Lv_oCMD Is Nothing Then
                Lv_oCMD.Dispose()
            End If
            If Not IsNothing(Lv_oCNT) And IsNothing(TARN) Then
                Lv_oCNT.Close()
            End If
        End Try
    End Function
    Private Function Pvt_GetSQLDATA(ByRef DATA As DataTable, SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As Int64
        Pvt_GetSQLDATA = 0
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        Try
            MessageBox.Show("Pvt_GetSQLDATA")
            Lv_oCNT = GetConnecttion()
            Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;end;", Lv_oCNT)
            If Not IsNothing(TARN) Then
                Lv_oCMD.Transaction = TARN
            End If
            Lv_oCMD.ExecuteNonQuery()
            Lv_oCMD.CommandText = SQL

            Dim Lv_drData As OracleDataReader = Lv_oCMD.ExecuteReader()
            If Lv_drData.HasRows() Then
                DATA.Load(Lv_drData)
                Return DATA.Rows.Count
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.WriteLine("ERROR SQL=>" + SQL)
            'Throw New ArgumentException("GetSQLDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
        Finally
            ' 釋放資源
            If Not Lv_oCMD Is Nothing Then
                Lv_oCMD.Dispose()
            End If
            Lv_oCNT.Close()
        End Try
    End Function
    Public Function Pvt_GetConnecttion() As OracleConnection

        If Gv_OraConnection Is Nothing Then
            Try
                Gv_OraConnection = New OracleConnection(Gv_Connstring)
            Catch ex As Exception
                Console.WriteLine("連線重試異常-1" + ex.Message)
            End Try
        End If
        If Gv_OraConnection.ConnectionString = "" Then
            Try
                Gv_OraConnection.ConnectionString = Gv_Connstring
            Catch ex As Exception
                Console.WriteLine("連線重試異常-1" + ex.Message)
            End Try
        End If
        If Gv_OraConnection.State <> ConnectionState.Open Then
            Try
                Gv_OraConnection.Open()
            Catch ex As Exception
                Console.WriteLine("連線重試異常-2" + ex.Message)
            End Try
        End If
        Return Gv_OraConnection
    End Function

    Private Sub tmCheck_Tick(sender As Object, e As EventArgs) Handles tmCheck.Tick
        If Not Gv_ISError Then
            tmCheck.Enabled = False
        End If
        If Not Gv_IS_RETRY And Gv_ReTryTime < Gv_ReTryLimit Then
            If Gv_TimmerCount = -1 Then
                Gv_TimmerCount = Gv_ReTryInterval
            ElseIf Gv_TimmerCount = 0 And Not bgwQuery.IsBusy Then
                Dim Lv_iProgress As Integer
                Gv_ReTryTime = Gv_ReTryTime + 1
                Lv_iProgress = CInt(Gv_ReTryTime / Gv_ReTryLimit * 100)
                ProgressBar1.Value = Lv_iProgress
                lblConCount.Text = "重試次數=>" + Gv_ReTryTime.ToString
                Console.WriteLine("TIMER啟動連線")
                bgwQuery.RunWorkerAsync()
            Else
                lblReConnect.Text = "等待重新連線中(" + (Gv_TimmerCount / 1000).ToString + ")...."
                Gv_TimmerCount = Gv_TimmerCount - tmCheck.Interval
            End If

            'ElseIf Gv_ReTryTime >= Gv_ReTryLimit Then
            '    Console.WriteLine("TIMER判斷中止" + Gv_ReTryTime.ToString)
            '    tmCheck.Enabled = False
            '    btnRetry.Enabled = True
            '    btnClose.Enabled = True
            '    Gv_TimmerCount = -1
            '    lblReConnect.Text = "重試次數已達上限，請確認是否繼續重試"
        End If
    End Sub

    Private Delegate Sub UpdateUICallBack(ByVal newText As String, ByVal c As Control)
    Private Sub UpdateUI(ByVal newText As String, ByVal c As Control)
        If Me.InvokeRequired() Then
            Try
                Dim cb As New UpdateUICallBack(AddressOf UpdateUI)
                Me.Invoke(cb, newText, c)
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try

        Else
            c.Text = newText
        End If


    End Sub

    Private Sub bgwQuery_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwQuery.DoWork
        Gv_IS_RETRY = True
        Dim Lv_sResult As String = ""
        If Gv_RETRYTYPE = 0 Then
            Console.WriteLine("背景處理-Gv_RETRYTYPE = 0 時間=>" + CType(Now, String))
            Try
                Gv_StatusMessage = "重新連線中...."
                bgwQuery.ReportProgress(1)
                Gv_OraConnection = Pvt_GetConnecttion()
            Catch ex As Exception
            Finally
                If Not Gv_OraConnection Is Nothing Then
                    If Gv_OraConnection.State = ConnectionState.Open Then
                        Gv_SQLResultStr = "ok"
                        Gv_StatusMessage = "重新連線成功...."
                    Else
                        Console.WriteLine("背景處理-Gv_RETRYTYPE = 0 等待重新連線中1.... 時間=>" + CType(Now, String))
                        Gv_StatusMessage = "等待重新連線中...."
                        bgwQuery.ReportProgress(1)
                        Console.WriteLine("背景處理-Gv_RETRYTYPE = 0 等待重新連線中2....時間=>" + CType(Now, String))
                    End If
                Else
                    Gv_StatusMessage = "等待重新連線中...."
                    bgwQuery.ReportProgress(1)
                End If
            End Try
        End If

        If Gv_RETRYTYPE = 1 Then
            Console.WriteLine("背景處理-Gv_RETRYTYPE = 1 時間=>" + CType(Now, String))
            Try
                Gv_StatusMessage = "重新連線中...."
                'UpdateUI("重新連線中....", lblReConnect)
                bgwQuery.ReportProgress(1)
                Lv_sResult = pvt_GetSingleDATA(Gv_SQL, Gv_ORGID, Gv_TRAN)
            Catch ex As Exception
            Finally
                If Lv_sResult <> "" Then
                    Gv_SQLResultStr = Lv_sResult
                    Gv_StatusMessage = "重新連線成功...."
                Else
                    Gv_StatusMessage = "等待重新連線中...."
                    bgwQuery.ReportProgress(1)
                End If
            End Try
        End If
    End Sub

    Private Sub bgwQuery_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwQuery.RunWorkerCompleted
        Console.WriteLine("背景處理完成 時間=>" + CType(Now, String))
        Gv_IS_RETRY = False
        Gv_TimmerCount = -1
        If Gv_SQLResultStr <> "" Then
            Gv_ISError = False
            Me.Close()
        End If
        If Gv_ReTryTime < Gv_ReTryLimit Then
            Gv_TimmerCount = Gv_ReTryInterval
            tmCheck.Enabled = True
            Console.WriteLine("仍有重試次數" + Gv_ReTryTime.ToString + " 繼續重試時間=>" + CType(Now, String))
        ElseIf Gv_ReTryTime >= Gv_ReTryLimit Then
            tmCheck.Enabled = False
            Console.WriteLine("無重試次數 timer disable時間=>" + CType(Now, String))
            btnRetry.Enabled = True
            btnClose.Enabled = True
            Gv_TimmerCount = -1
            lblReConnect.Text = "重試次數已達上限，請確認是否繼續重試"
        End If
    End Sub

    Private Sub btnRetry_Click(sender As Object, e As EventArgs) Handles btnRetry.Click
        Gv_ReTryTime = 1
        ProgressBar1.Value = CInt(Gv_ReTryTime / Gv_ReTryLimit * 100)
        lblConCount.Text = "重試次數=>" + Gv_ReTryTime.ToString
        btnRetry.Enabled = False
        btnClose.Enabled = True

        While bgwQuery.IsBusy
            Threading.Thread.Sleep(1000)
        End While

        bgwQuery.RunWorkerAsync()
        Console.WriteLine("重新Retry 時間=>" + CType(Now, String))
    End Sub

    Private Sub bgwQuery_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bgwQuery.ProgressChanged
        lblReConnect.Text = Gv_StatusMessage
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class