'版本紀錄
'初版 20161101 ericlin
'新增是否啟動自動更新功能由子form自行判斷 20161107 ericlin
'todo 新增非同步下載功能
'------------------------------------------------------------------------------
'  Program ID:SFCS_TEMPLATE
'  Program Name:SFCS_樣板
'  Programmer:Eric Lin
'  Programming date: 2016/11/01
'------------------------------------------------------------------------------
'  Modification log
'  Ver.    Date       Programmer      Remark
' ------ ----------  ---------------- -------------------------------------------
' 1.0    20161101    ERIC LIN         初版                       
' 1.1    20161117    ERIC LIN         增加資料庫連線異常重試機制
' 1.2    20161126    ERIC LIN         增加由DB取得SERVER資訊功能
' 1.3    20161207    ERIC LIN         增加由自動PARSE SQL函數
'-------------------------------------------------------------------------------
'Imports System.Data.OracleClient
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Management
Imports Oracle.ManagedDataAccess.Client ' ODP.NET, Managed Driver
Imports System.Net.NetworkInformation


Module SFCLIBRARY
    Private ReadOnly Gv_strDBNAME As String = "SZS"
    Private ReadOnly Gv_strUID As String = "apps"
    Private ReadOnly Gv_strPWD As String = "apps"
    Private ReadOnly Gv_ErrorRetry As Integer = 10
    Private ReadOnly Gv_iTimeout As Integer = 300
    Private Gv_IsQueryComplite As Boolean = False
    Private Gv_oConnection As OracleConnection
    Private Gv_strCN As String = "Data Source=" & Gv_strDBNAME & "; User Id=" & Gv_strUID & "; Password=" & Gv_strPWD & "; Connection Timeout=" & Gv_iTimeout.ToString
    Structure SERVERINFO
        Dim IP As String
        Dim PORT As String
        Dim USER As String
        Dim PASSWORD As String
    End Structure
    ''' <summary>
    ''' 取得SERVER資訊
    ''' </summary>
    ''' <returns>SERVERINFO</returns>
    Public Function GetSERVERINFO(ORGID As String) As SERVERINFO
        Dim Lv_dtResult As DataTable = New DataTable
        Dim Lv_sSQL As String = "select server_ip,server_port,server_user,server_pass from SFCS_SERVERINFO_all where server_name='LABEL_TEMPLATE_SERVER' and server_status=1 and server_type=2"
        Dim Lv_siResult As SERVERINFO
        Try
            GetSQLDATA(Lv_dtResult, Lv_sSQL, ORGID)
            If Lv_dtResult.Rows.Count > 0 Then
                Lv_siResult = New SERVERINFO
                Lv_siResult.IP = Lv_dtResult.Rows(0)("server_ip").ToString
                Lv_siResult.PORT = Lv_dtResult.Rows(0)("server_port").ToString
                Lv_siResult.USER = Lv_dtResult.Rows(0)("server_user").ToString
                Lv_siResult.PASSWORD = Lv_dtResult.Rows(0)("server_pass").ToString
                Return Lv_siResult
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Throw New ArgumentException("取得伺服器資訊失敗!" + ex.Message)
        End Try
    End Function
    Public Function GetAdapterName() As String
        Dim oNIFS As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim oNIF As NetworkInterface
        Dim oIPIP As IPInterfaceProperties
        For Each oNIF In oNIFS
            If oNIF.NetworkInterfaceType = 71 And oNIF.Speed > 1 Then
                oIPIP = oNIF.GetIPProperties()
                If oIPIP.UnicastAddresses(0).Address.ToString.Substring(0, 3) = "10." Then
                    Return oNIF.Name
                End If
            End If
        Next
        Return ""
    End Function
    Public Function GetIPaddress() As String
        'Dim myHost As String = System.Net.Dns.GetHostName
        'Dim myIPs As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(myHost)
        'For Each ipAddress As System.Net.IPAddress In myIPs.AddressList
        ' Return ipAddress.ToString()
        ' Next
        ' Return String.Empty
        Dim mso As ManagementObjectSearcher = New ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration where IPEnabled='TRUE'")
        Dim ip As String = String.Empty
        For Each mo As ManagementObject In mso.Get()
            If Not IsDBNull(mo("IPAddress")) Then
                Console.WriteLine(mo.GetPropertyValue("IPAddress"))
                Console.WriteLine(mo.GetPropertyValue("Description"))
                Console.WriteLine(mo.GetPropertyValue("Caption"))
                Console.WriteLine(mo.GetPropertyValue("SettingID"))
                Console.WriteLine(mo.GetPropertyValue("MACAddress"))

                For Each s As String In CType(mo.GetPropertyValue("IPAddress"), String())
                    If s.Substring(0, 3) = "10." Then
                        ip = s
                        Return ip
                    End If
                Next
            End If
        Next
        Return ""

    End Function
    Public Function GetSYSTEMTITLE() As String
        GetSYSTEMTITLE = ""
        If Gv_strDBNAME = "SZ" Then
            Return "-測試區"
        End If
        If Gv_strDBNAME = "SZS" Then
            Return "-正式區"
        End If
    End Function
    'Public Function GetConnecttion() As OracleConnection
    '    If Gv_oConnection Is Nothing Then
    '        Try
    '            Gv_oConnection = New OracleConnection(Gv_strCN)

    '        Catch ex As Exception
    '            Throw New ArgumentException("建立ORACLE連線物件失敗" + ex.Message)
    '        End Try
    '    End If
    '    If Gv_oConnection.ConnectionString = "" Then
    '        Try
    '            Gv_oConnection = New OracleConnection(Gv_strCN)
    '        Catch ex As Exception
    '            Throw New ArgumentException("建立ORACLE連線物件失敗" + ex.Message)
    '        End Try
    '    End If

    '    If Gv_oConnection.State <> ConnectionState.Open Then
    '        Try

    '            'MessageBox.Show(Gv_oConnection.State.ToString)
    '            Gv_oConnection.Open()
    '            'MessageBox.Show(Gv_oConnection.State.ToString)

    '        Catch ex As Exception
    '            MessageBox.Show(ex.Message)
    '            Dim oRetry As frmRetryDB = New frmRetryDB(Gv_strCN)
    '            Gv_oConnection = oRetry.GetConnecttion
    '            oRetry.Close()

    '            If Gv_oConnection Is Nothing Then
    '                Throw New ArgumentException("建立ORACLE連線物件失敗" + ex.Message)
    '            Else
    '                Return Gv_oConnection
    '            End If
    '        End Try
    '    End If
    '    Return Gv_oConnection
    'End Function
    '取得單筆資料回傳單欄位
    '傳入參數:SQL ,ORGID,MERGE:若為多欄位是否合併成一個字串以;區隔
    '回傳參數:所取得之第一筆資料中第一個欄位
    Public Function GetSingleDATA(SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As String
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        Dim Lv_sResult As String = ""
        GetSingleDATA = ""
        Try
            Lv_oCNT = GetConnecttion()
            If Lv_oCNT Is Nothing Then
                Exit Function
            End If
            Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;End;", Lv_oCNT)

            If Not IsNothing(TARN) Then
                Lv_oCMD.Transaction = TARN
            End If

            Lv_oCMD.ExecuteNonQuery()
            'Lv_oCMD.CommandText = "ALTER SESSION Set NLS_LANGUAGE = 'TRADITIONAL CHINESE'"
            'Lv_oCMD.ExecuteNonQuery()
            Lv_oCMD.CommandText = SQL
            'Lv_sResult = Lv_oCMD.ExecuteScalar().ToString()
            Dim Lv_drData As OracleDataReader = Lv_oCMD.ExecuteReader
            If Lv_drData.HasRows() Then
                Do While Lv_drData.Read()
                    Lv_sResult = Lv_drData(0).ToString
                    Return Lv_sResult
                Loop
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            If ex.Message.Contains("ORA-03135") Or ex.Message.Contains("ORA-03114") Then
                Gv_oConnection.Dispose()
                Throw New ArgumentException("連線失敗請再試一次" + vbCrLf + "GetSingleDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
            Else
                Throw New ArgumentException("GetSingleDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
            End If
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

    '取得單筆資料回傳多欄位
    '傳入參數:SQL ,ORGID,MERGE:若為多欄位是否合併成一個字串以;區隔
    '回傳參數:所取得之第一筆資料中第一個欄位
    Public Function GetSingleDATAArray(SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As List(Of String)
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing

        Try
            Lv_oCNT = GetConnecttion()
            Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;end;", Lv_oCNT)
            If Not IsNothing(TARN) Then
                Lv_oCMD.Transaction = TARN
            End If
            Lv_oCMD.ExecuteNonQuery()
            Lv_oCMD.CommandText = SQL
            Dim aResult As New List(Of String)
            Dim Lv_drData As OracleDataReader = Lv_oCMD.ExecuteReader()
            If Lv_drData.HasRows() Then
                Dim Lv_aResult(Lv_drData.FieldCount) As String
                Do While Lv_drData.Read()
                    For i As Integer = 0 To Lv_drData.FieldCount - 1
                        aResult.Add(Lv_drData(i).ToString)
                    Next
                Loop
            End If
            Return aResult
        Catch ex As Exception
            Console.WriteLine("GetSingleDATAArray=>" + ex.Message)
            Throw New ArgumentException("GetSingleDATAArray Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
        Finally
            ' 釋放資源
            If Not Lv_oCMD Is Nothing Then
                Lv_oCMD.Dispose()
            End If
            Lv_oCNT.Close()
        End Try
    End Function
    '取得多筆資料
    '傳入參數:DataTable,SQL ,ORGID
    '回傳參數:資料筆數
    Public Function GetSQLDATA(ByRef DATA As DataTable, SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As Int64
        GetSQLDATA = 0
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        Try
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
            Throw New ArgumentException("GetSQLDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
        Finally
            ' 釋放資源
            If Not Lv_oCMD Is Nothing Then
                Lv_oCMD.Dispose()
            End If
            Lv_oCNT.Close()
            'Lv_oCNT.Dispose()
        End Try

    End Function
    '執行指令
    '回傳交易物件
    Public Function EXECSQL(SQL As String, ORGID As String, Optional TRAN As OracleTransaction = Nothing) As OracleTransaction
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        Dim Lv_sResult As String = ""
        'Dim Lv_sRowID As OracleString
        Dim Lv_otResult As OracleTransaction
        Dim Lv_iEffectRow As Integer
        Try
            Lv_oCMD = New OracleCommand()
            Lv_oCNT = GetConnecttion()
            Lv_oCMD.Connection = Lv_oCNT
            If Not IsNothing(TRAN) Then
                Lv_oCMD.Transaction = TRAN
                Lv_otResult = TRAN
            Else
                Lv_otResult = Lv_oCNT.BeginTransaction(IsolationLevel.Serializable)
                Lv_oCMD.Transaction = Lv_otResult
            End If
            Lv_oCMD.CommandType = CommandType.Text
            Lv_oCMD.CommandText = "begin set_org_id(" + ORGID + ") ;end;"
            Lv_oCMD.ExecuteNonQuery()

            Lv_oCMD.CommandType = CommandType.Text
            Lv_oCMD.CommandText = SQL
            Lv_iEffectRow = Lv_oCMD.ExecuteNonQuery()

            Return Lv_otResult

        Catch ex As Exception
            Console.WriteLine("EXEC-SQL ERROR=>" + ex.Message)
            Console.WriteLine("ERROR SQL=>" + SQL)
            Throw New ArgumentException("EXEC-SQL ERROR=>" + ex.Message + vbCrLf + "ERROR SQL=>" + SQL)
        Finally
            ' 釋放資源
            'If Not Lv_oCMD Is Nothing Then
            ' Lv_oCMD.Dispose()
            ' End If
            'Lv_oCNT.Close()
        End Try

    End Function
    '執行StoreProcedure須回傳結果 請將result宣告在第一個變數
    '傳入參數:SQL ,ORGID
    '回傳參數:結果字串 若異常則回傳空白
    Public Function EXECSPWithResult(PROCNAME As String, ORGID As String, ByVal ORA_PARA As OracleParameterCollection, Optional TARN As OracleTransaction = Nothing) As String
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        Dim Lv_sResult As String = ""
        Dim Lv_sResultFieldName As String = ""
        Try
            Lv_oCNT = GetConnecttion()
            Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;end;", Lv_oCNT)
            If Not IsNothing(TARN) Then
                Lv_oCMD.Transaction = TARN
            End If
            Lv_oCMD.ExecuteNonQuery()
            Lv_oCMD.CommandText = PROCNAME
            Lv_oCMD.CommandType = CommandType.StoredProcedure
            Dim paramCollection As OracleParameterCollection = Lv_oCMD.Parameters

            Dim Lv_aryPARA(ORA_PARA.Count - 1) As OracleParameter
            ORA_PARA.CopyTo(Lv_aryPARA, 0)
            ORA_PARA.Clear()

            For i As Integer = 0 To Lv_aryPARA.Length - 1
                If Lv_aryPARA(i).Direction = ParameterDirection.ReturnValue Then
                    Lv_sResultFieldName = Lv_aryPARA(i).ToString
                End If
                paramCollection.Add(Lv_aryPARA(i))
            Next

            Lv_oCMD.ExecuteNonQuery()
            If Lv_sResultFieldName = "" Then
                '未設定result field    
                Return ""
            Else
                Return Lv_oCMD.Parameters.Item(Lv_sResultFieldName).Value.ToString
            End If

        Catch ex As Exception
            Console.WriteLine("EXECStoreProcedure ERROR=>" + ex.Message)
            Console.WriteLine("ERROR SP=>" + PROCNAME)
            Lv_sResult = "EXECStoreProcedure ERROR=>" + ex.Message + vbCrLf + "ERROR SP=>" + PROCNAME
            Throw New ArgumentException(Lv_sResult)
        Finally
            ' 釋放資源
            'If Not Lv_oCMD Is Nothing Then
            '    Lv_oCMD.Dispose()
            'End If
            'Lv_oCNT.Close()

        End Try
    End Function
    '執行StoreProcedure須回傳結果 請將result宣告在第一個變數
    '傳入參數:SQL ,ORGID
    '回傳參數:結果字串 若異常則回傳空白
    Public Function EXECSPWithResult_ODP(PROCNAME As String, ORGID As String, ByVal ORA_PARA As OracleParameter(), Optional TARN As OracleTransaction = Nothing) As String
        Dim Lv_oCMD As OracleCommand = Nothing
        Dim Lv_oCNT As OracleConnection = Nothing
        Dim Lv_sResult As String = ""
        Dim Lv_sResultFieldName As String = ""
        Try
            Lv_oCNT = GetConnecttion()
            Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;end;", Lv_oCNT)
            If Not IsNothing(TARN) Then
                Lv_oCMD.Transaction = TARN
            End If
            Lv_oCMD.ExecuteNonQuery()
            Lv_oCMD.CommandText = PROCNAME
            Lv_oCMD.CommandType = CommandType.StoredProcedure
            Dim paramCollection As OracleParameterCollection = Lv_oCMD.Parameters

            Dim Lv_aryPARA(ORA_PARA.Count - 1) As OracleParameter
            ORA_PARA.CopyTo(Lv_aryPARA, 0)
            'ORA_PARA.Clear()

            For i As Integer = 0 To Lv_aryPARA.Length - 1
                If Lv_aryPARA(i).Direction = ParameterDirection.ReturnValue Then
                    Lv_sResultFieldName = Lv_aryPARA(i).ToString
                End If
                paramCollection.Add(Lv_aryPARA(i))
            Next

            Lv_oCMD.ExecuteNonQuery()
            If Lv_sResultFieldName = "" Then
                '未設定result field    
                Return ""
            Else
                Return Lv_oCMD.Parameters.Item(Lv_sResultFieldName).Value.ToString
            End If

        Catch ex As Exception
            Console.WriteLine("EXECStoreProcedure ERROR=>" + ex.Message)
            Console.WriteLine("ERROR SP=>" + PROCNAME)
            Lv_sResult = "EXECStoreProcedure ERROR=>" + ex.Message + vbCrLf + "ERROR SP=>" + PROCNAME
            Throw New ArgumentException(Lv_sResult)
        Finally
            ' 釋放資源
            'If Not Lv_oCMD Is Nothing Then
            '    Lv_oCMD.Dispose()
            'End If
            'Lv_oCNT.Close()

        End Try
    End Function
    Public Function EXECMARCO(MARCO As String) As String
        Dim jscriptSource As String = "class Evaluator { public function Eval(expr : String) : String { return eval(expr); } }"
        Dim Lv_CodeDom As CodeDom.Compiler.CodeDomProvider = CodeDom.Compiler.CodeDomProvider.CreateProvider("JScript")
        '定義編譯參數
        Dim Lv_oCDPARM As CodeDom.Compiler.CompilerParameters = New CodeDom.Compiler.CompilerParameters()
        Lv_oCDPARM.GenerateInMemory = True      '產生在記憶體
        '進行編譯
        Dim Lv_oCPResult As CodeDom.Compiler.CompilerResults = Lv_CodeDom.CompileAssemblyFromSource(Lv_oCDPARM, jscriptSource)
        Dim Lv_oAssembly As Reflection.Assembly = Lv_oCPResult.CompiledAssembly    '編譯完成的IL檔
        '使用編譯完成IL建立執行實體
        Dim Lv_oInstance As Object = Activator.CreateInstance(Lv_oAssembly.GetType("Evaluator"))
        '取得開放的方法
        Dim Lv_oMethod As Reflection.MethodInfo = Lv_oAssembly.GetType("Evaluator").GetMethod("Eval")
        '呼叫方法取得回傳值
        Dim ret As Object = Lv_oMethod.Invoke(Lv_oInstance, New Object() {MARCO})
        Return ret.ToString
    End Function

    Function DeepClone(Of T)(ByRef orig As T) As T

        ' Don't serialize a null object, simply return the default for that object
        If (Object.ReferenceEquals(orig, Nothing)) Then Return Nothing

        Dim formatter As New BinaryFormatter()
        Dim stream As New MemoryStream()

        formatter.Serialize(stream, orig)
        stream.Seek(0, SeekOrigin.Begin)

        Return CType(formatter.Deserialize(stream), T)

    End Function
    'v1.3
    Function Pvt_GetInsertSQL(TABLENAME As String, WORKROW As DataRow, USER As String) As String
        Dim i As Integer
        Dim Lv_oColumns As DataColumnCollection = WORKROW.Table.Columns
        Dim Lv_sTemp, Lv_sResult As String

        Lv_sResult = "insert into " + TABLENAME + "("
        For i = 0 To Lv_oColumns.Count - 1
            If Lv_oColumns(i).ColumnName = "CREATION_DATE" Or Lv_oColumns(i).ColumnName = "CREATED_BY" Or Lv_oColumns(i).ColumnName = "LAST_UPDATE_DATE" Or Lv_oColumns(i).ColumnName = "LAST_UPDATED_BY" Then
                Lv_sResult = Lv_sResult + Lv_oColumns(i).ColumnName
            Else
                If WORKROW.Item(i).ToString <> "" Or Lv_oColumns(i).DataType.ToString = "System.String" Then
                    Lv_sResult = Lv_sResult + Lv_oColumns(i).ColumnName

                End If
            End If
            If i <> Lv_oColumns.Count - 1 Then
                If (Lv_oColumns(i).DataType.ToString.IndexOf("Int") >= 0 Or Lv_oColumns(i).DataType.ToString.IndexOf("Double") >= 0) And WORKROW.Item(i).ToString = "" Then
                Else
                    Lv_sResult = Lv_sResult + ","
                End If
            End If
        Next

        If Lv_sResult.Substring(Lv_sResult.Length - 1, 1) = "," Then
            Lv_sResult = Lv_sResult.Substring(0, Lv_sResult.Length - 1)
        End If
        Lv_sResult = Lv_sResult + ") values("


        For i = 0 To Lv_oColumns.Count - 1
            Lv_sTemp = ""
            If Lv_oColumns(i).ColumnName = "CREATION_DATE" Or Lv_oColumns(i).ColumnName = "CREATED_BY" Or Lv_oColumns(i).ColumnName = "LAST_UPDATE_DATE" Or Lv_oColumns(i).ColumnName = "LAST_UPDATED_BY" Then
                Select Case Lv_oColumns(i).ColumnName
                    Case "CREATION_DATE"
                        Lv_sTemp = Lv_sTemp + "sysdate"
                    Case "CREATED_BY"
                        Lv_sTemp = Lv_sTemp + "'" + USER + "'"
                    Case "LAST_UPDATE_DATE"
                        Lv_sTemp = Lv_sTemp + "sysdate"
                    Case "LAST_UPDATED_BY"
                        Lv_sTemp = Lv_sTemp + "'" + USER + "'"
                End Select
                If (i <> 0) Then
                    Lv_sTemp = "," + Lv_sTemp
                End If
            Else
                If WORKROW.Item(i).ToString <> "" Or Lv_oColumns(i).DataType.ToString = "System.String" Then
                    Select Case Lv_oColumns(i).DataType.ToString
                        Case "System.String"
                            Lv_sTemp = Lv_sTemp + "'" + WORKROW.Item(i).ToString + "' "
                        Case "System.DateTime"
                            Lv_sTemp = Lv_sTemp + "to_date('" + Convert.ToDateTime(WORKROW.Item(i)).ToString("u").Substring(0, 19) + "','YYYY/MM/DD HH24:MI:SS')"
                        Case Else
                            Lv_sTemp = Lv_sTemp + WORKROW.Item(i).ToString
                    End Select
                    If (i <> 0) Then
                        Lv_sTemp = "," + Lv_sTemp
                    End If

                End If
            End If
            Lv_sResult = Lv_sResult + Lv_sTemp
        Next
        Lv_sResult = Lv_sResult + ")"
        Return Lv_sResult
    End Function
    Function Pvt_GetUpdateSQL(TABLENAME As String, WORKROW As DataRow, USER As String) As String
        Dim i As Integer
        Dim Lv_oColumns As DataColumnCollection = WORKROW.Table.Columns
        Dim Lv_sResult, Lv_sWHERE, Lv_sTemp As String
        Lv_sTemp = ""
        Lv_sWHERE = "Where "
        For i = 0 To WORKROW.Table.PrimaryKey.Count - 1
            Dim Lv_sColumnName As String = WORKROW.Table.PrimaryKey(i).ColumnName.ToString
            Select Case WORKROW.Table.PrimaryKey(i).DataType.ToString
                Case "System.String"
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "='" + WORKROW.Item(Lv_sColumnName).ToString + "' "
                Case "System.DateTime"
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "=" + "to_date('" + Convert.ToDateTime(WORKROW.Item(Lv_sColumnName)).ToString("u").Substring(0, 19) + "','YYYY/MM/DD HH24:MI:SS')" + " "
                Case Else
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "=" + WORKROW.Item(Lv_sColumnName).ToString
            End Select
            If i < WORKROW.Table.PrimaryKey.Count - 1 Then
                Lv_sWHERE = Lv_sWHERE + " AND "
            End If
        Next

        Lv_sResult = "update " + TABLENAME + " set "
        For i = 0 To Lv_oColumns.Count - 1
            If Lv_oColumns(i).ColumnName = "CREATION_DATE" Or Lv_oColumns(i).ColumnName = "CREATED_BY" Or Lv_oColumns(i).ColumnName = "LAST_UPDATE_DATE" Or Lv_oColumns(i).ColumnName = "LAST_UPDATED_BY" Then
            Else
                If WORKROW.Item(i).ToString <> "" Or Lv_oColumns(i).DataType.ToString = "System.String" Then
                    Lv_sTemp = Lv_oColumns(i).ColumnName + " = "
                    Select Case Lv_oColumns(i).DataType.ToString
                        Case "System.String"
                            Lv_sTemp = Lv_sTemp + "'" + WORKROW.Item(i).ToString + "' "
                        Case "System.DateTime"
                            Lv_sTemp = Lv_sTemp + "to_date('" + Convert.ToDateTime(WORKROW.Item(i)).ToString("u").Substring(0, 19) + "','YYYY/MM/DD HH24:MI:SS')"
                        Case Else
                            Lv_sTemp = Lv_sTemp + WORKROW.Item(i).ToString
                    End Select
                    If (i <> 0) Then
                        Lv_sTemp = "," + Lv_sTemp
                    End If
                    Lv_sResult = Lv_sResult + Lv_sTemp
                End If
            End If
        Next
        If USER <> "" Then
            Lv_sResult = Lv_sResult + ", LAST_UPDATED_BY='" + USER + "',LAST_UPDATE_DATE=sysdate "
        End If
        Lv_sResult = Lv_sResult + Lv_sWHERE
        Return Lv_sResult
    End Function
    Function Pvt_GetDeleteSQL(TABLENAME As String, WORKROW As DataRow, USER As String) As String
        Dim i As Integer
        Dim Lv_oColumns As DataColumnCollection = WORKROW.Table.Columns
        Dim Lv_sResult, Lv_sWHERE, Lv_sTemp As String
        Lv_sTemp = ""
        Lv_sResult = "Delete " + TABLENAME

        Lv_sWHERE = " Where "
        For i = 0 To WORKROW.Table.PrimaryKey.Count - 1
            Dim Lv_sColumnName As String = WORKROW.Table.PrimaryKey(i).ColumnName.ToString
            Select Case WORKROW.Table.PrimaryKey(i).DataType.ToString
                Case "System.String"
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "='" + WORKROW.Item(Lv_sColumnName).ToString + "' "
                Case "System.DateTime"
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "=" + "to_date('" + Convert.ToDateTime(WORKROW.Item(Lv_sColumnName)).ToString("u").Substring(0, 19) + "','YYYY/MM/DD HH24:MI:SS')" + " "
                Case Else
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "=" + WORKROW.Item(Lv_sColumnName).ToString
            End Select
            If i < WORKROW.Table.PrimaryKey.Count - 1 Then
                Lv_sWHERE = Lv_sWHERE + " AND "
            End If
        Next
        Lv_sResult = Lv_sResult + Lv_sWHERE
        Return Lv_sResult
    End Function
    Function Pvt_GetTableNO(HEAD As String, DATESTR As String, TABLENAME As String, PKEY As String, SEQLEN As Integer, ORGID As String) As String
        Dim Lv_sTemp, Lv_sSQL As String
        'Lv_sDATEStr = DATADATE.Year.ToString.PadLeft(2, "0") + DATADATE.Month.ToString.PadLeft(2) + DATADATE.Day.ToString.PadLeft(2)
        If DATESTR <> "" Then
            Lv_sSQL = "Select COALESCE(MAX(" + PKEY + "),'X') AS SID FROM  " + TABLENAME + " WHERE (" + PKEY + " LIKE '" + HEAD + DATESTR + "%') and ORG_ID='" + ORGID + "'"
        Else
            Lv_sSQL = "Select COALESCE(MAX(" + PKEY + "),'X') AS SID FROM  " + TABLENAME + " WHERE (" + PKEY + " LIKE '" + HEAD + "%')  and ORG_ID='" + ORGID + "'"
        End If

        Lv_sTemp = GetSingleDATA(Lv_sSQL, ORGID)
        If Lv_sTemp = "" Then
            Return ""
        End If
        If Lv_sTemp = "X" Then
            Lv_sTemp = HEAD + DATESTR + "1".PadLeft(SEQLEN, CChar("0"))
        Else
            Lv_sTemp = HEAD + DATESTR + (CInt(Lv_sTemp.Substring(HEAD.Length + DATESTR.Length, SEQLEN)) + 1).ToString.PadLeft(SEQLEN, CChar("0"))
        End If
        Return Lv_sTemp
    End Function
    'upon

    '取得DB參數
    Public Function Get_PARAMETER_DB(TYPE As String, ORGID As String) As String
        Dim Lv_sResult As String
        Dim Lv_sSQL As String = "Select value from param where code=upper('" + TYPE + "')"
        Lv_sResult = GetSingleDATA(Lv_sSQL, ORGID)
        Return Lv_sResult
    End Function
    '函式名稱:版本更新
    '傳入參數:程式名稱
    '說    明:採用檔案日期判別    
    Public Function Check_AP_Version(APName As String) As String
        Return ""
    End Function
    'upon



    Class RetryDB
        Private SQL As String
        Private ORGID As String
        Private TRAN As OracleTransaction
        Private Gv_ErrorRetryNow As Integer = 0
        Private Gv_ErrorRetryITV As Integer = 5000
        Public Property pptSQL() As String
            Get
                Return SQL
            End Get
            Set(ByVal value As String)
                SQL = value
            End Set
        End Property
        Public Property pptORGID() As String
            Get
                Return ORGID
            End Get
            Set(ByVal value As String)
                ORGID = value
            End Set
        End Property
        Public Property pptTRAN() As OracleTransaction
            Get
                Return TRAN
            End Get
            Set(ByVal value As OracleTransaction)
                If Not value Is Nothing Then
                    TRAN = value
                End If
            End Set
        End Property
        Public Sub New(ByVal SQL As String, ByVal ORGID As String, ByVal TRAN As OracleTransaction)
            Me.SQL = SQL
            Me.ORGID = ORGID
            Me.TRAN = TRAN
        End Sub
        Public Function GetSingleDATARetry() As String
            Dim Lv_sResultStr As String = ""
            Try
                Lv_sResultStr = pvt_GetSingleDATA(SQL, ORGID, TRAN)
                Gv_ErrorRetryNow = 0
                Return Lv_sResultStr
            Catch ex As Exception
                If Gv_ErrorRetryNow <= Gv_ErrorRetry Then
                    pvt_GetSingleDATA(SQL, ORGID, TRAN)
                Else

                End If
            End Try
            Return Lv_sResultStr
        End Function
        Public Function pvt_GetSingleDATA(SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As String
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
                Dim oRetry As RetryDB = New RetryDB(SQL, ORGID, TARN)
                oRetry.GetSingleDATARetry()
                Throw New ArgumentException("GetSingleDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
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
    End Class






End Module
