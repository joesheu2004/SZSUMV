Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Management
Imports System.Net.NetworkInformation
Imports System.Xml
Imports System.IO
Imports System.Web
Imports System.Web.Services.Protocols
Imports System.ServiceModel
Imports WindowsApplication1
'版本紀錄
'初版 20170401 ericlin
'------------------------------------------------------------------------------
'  Programming date: 2017/04/01
'------------------------------------------------------------------------------
'  Modification log
'  Ver.    Date       Programmer      Remark
' ------ ----------  ---------------- -------------------------------------------
'  1.0    20170401    ERIC LIN         初版                       
'  2.0    20170411    ERIC LIN         新增程式版本更新機制
'  2.1    20170412    ERIC LIN         增加異常處理類別
'  2.2    20170630    ERIC LIN         增加動態定義WEBSERVICE功能
'-------------------------------------------------------------------------------


Module ERICWEBLIBRARY
    Private ReadOnly Gv_strDBNAME As String = "SZ"
    Private ReadOnly Gv_strUID As String = "apps"
    Private ReadOnly Gv_strPWD As String = "apps"
    Private ReadOnly Gv_ErrorRetry As Integer = 10
    Private ReadOnly Gv_iTimeout As Integer = 300
    Private Gv_strCN As String = "Data Source=" & Gv_strDBNAME & "; User Id=" & Gv_strUID & "; Password=" & Gv_strPWD & "; Connection Timeout=" & Gv_iTimeout.ToString
    Private ReadOnly Gv_XMLName As String = "SFCS.xml"      'V1.4
#If DEBUG Then
    'Private Gv_sAPSERVER As String = "10.1.1.49"            'V1.4
    'Private Gv_sDeFaultURLPart As String = "/WEBTRAN/MES1001.asmx"  'v1.4 測試區
    Private Gv_sAPSERVER As String = "10.1.1.181"            'V1.4  正式區
    Private Gv_sDeFaultURLPart As String = "/WEBTRAN/MES1001.asmx"  'v1.4 正式區
#Else
    Private Gv_sAPSERVER As String = "10.1.1.181"            'V1.4  正式區
    Private Gv_sDeFaultURLPart As String = "/WEBTRAN/MES1001.asmx"  'v1.4 正式區
#End If
    Private Gv_WSType As Type = Nothing                     'V1.4
    Private Gv_WSInstance As Object = Nothing               'V1.4


    Structure SERVERINFO
        Dim IP As String
        Dim PORT As String
        Dim USER As String
        Dim PASSWORD As String
    End Structure

    ''' <summary>
    ''' 取得APSERVERIP
    ''' </summary>
    ''' <returns>APSERVER IP</returns>
    Private Function Pvt_GetAPServerIP() As String
        Dim oXML As XmlDocument = New XmlDocument()
        Try
            oXML.Load(Gv_XMLName)
            Dim oXMLNODE As XmlNode = oXML.SelectSingleNode("SFCS/APSERVER")
            If oXMLNODE.Attributes.ItemOf("COUNT").Value <> "" Then
                Dim oXMLNODELST = oXMLNODE.SelectNodes("AP")
                If oXMLNODELST.Count > 0 Then
                    Return oXMLNODELST.Item(Minute(Now()) Mod oXMLNODELST.Count).Attributes.ItemOf("IP").Value
                End If
            End If
            Return Gv_sAPSERVER
        Catch ex As Exception
            Return Gv_sAPSERVER
        End Try
    End Function
    Private Function Pvt_GetWSURL(Optional URLPART As String = "") As String
        If URLPART = "" Then
            Return "http://" + Pvt_GetAPServerIP() + "/WEBTRAN/MES1001.asmx"
        Else
            Return "http://" + Pvt_GetAPServerIP() + URLPART
        End If
    End Function

    Private Function GetWSClassName(WSURL As String) As String
        Dim Lv_aPartTemp() As String = WSURL.Split("/")
        Dim Lv_aPart() As String = Lv_aPartTemp(Lv_aPartTemp.Length - 1).Split(".")
        Return Lv_aPart(0)
    End Function
    Public Function GetWSTYPE(WSURL As String, WSNAMESPACE As String) As Type
        Dim Lv_ClassName = GetWSClassName(WSURL)
        Dim Lv_oWebClient As Net.WebClient = New Net.WebClient
        Dim Lv_stmWSDL As Stream = Lv_oWebClient.OpenRead(WSURL + "?WSDL")
        Dim Lv_sdSERVICEDESC As System.Web.Services.Description.ServiceDescription = Web.Services.Description.ServiceDescription.Read(Lv_stmWSDL)
        Dim Lv_sdiServiceDI As System.Web.Services.Description.ServiceDescriptionImporter = New Web.Services.Description.ServiceDescriptionImporter
        Lv_sdiServiceDI.AddServiceDescription(Lv_sdSERVICEDESC, "", "")
        Dim Lv_cnsCodeNameSpace As CodeDom.CodeNamespace = New CodeDom.CodeNamespace(WSNAMESPACE)
        Dim Lv_ccuCodeComplieUnit As CodeDom.CodeCompileUnit = New CodeDom.CodeCompileUnit
        Lv_ccuCodeComplieUnit.Namespaces.Add(Lv_cnsCodeNameSpace)
        Lv_sdiServiceDI.Import(Lv_cnsCodeNameSpace, Lv_ccuCodeComplieUnit)
        '動態編譯
        Dim Lv_vbcpCodeProvider As VBCodeProvider = New VBCodeProvider
        Dim Lv_iccICodeCompiler As CodeDom.Compiler.ICodeCompiler = Lv_vbcpCodeProvider.CreateCompiler
        Dim Lv_cpCodePARA As System.CodeDom.Compiler.CompilerParameters = New System.CodeDom.Compiler.CompilerParameters
        Lv_cpCodePARA.GenerateExecutable = False
        Lv_cpCodePARA.GenerateInMemory = True
        Lv_cpCodePARA.ReferencedAssemblies.Add("System.Data.dll")
        '取得編譯結果
        Dim Lv_crCOMResult As System.CodeDom.Compiler.CompilerResults = Lv_iccICodeCompiler.CompileAssemblyFromDom(Lv_cpCodePARA, Lv_ccuCodeComplieUnit)
        If Lv_crCOMResult.Errors.HasErrors Then
            Dim Lv_sErrorStr As String = ""
            For Each tComError In Lv_crCOMResult.Errors
                Lv_sErrorStr = Lv_sErrorStr + tComError.ToString
            Next
            Throw New ArgumentException("取得WS異常(GetWSTYPE)" + vbCrLf + Lv_sErrorStr)
            Return Nothing
        End If
        '取得Assembly
        Dim Lv_asmResult As System.Reflection.Assembly = Lv_crCOMResult.CompiledAssembly
        Dim Lv_tType As Type = Lv_asmResult.GetType(WSNAMESPACE + "." + Lv_ClassName, True, True)
        Return Lv_tType
    End Function
    Public Function WSInvoke(WSURL As String, WSNAMESPACE As String, WSMETHODNAME As String, WSPARAM As Object()) As Object
        Try
            If Gv_WSType = Nothing Then
                Gv_WSType = GetWSTYPE(WSURL, WSNAMESPACE)
                Gv_WSInstance = Activator.CreateInstance(Gv_WSType)
            End If
            '指定參數
            Dim Lv_aryPARMType() As Type = Nothing
            If WSPARAM Is Nothing Then
                ReDim Lv_aryPARMType(0)
            Else
                Dim Lv_iParmLen = WSPARAM.Length
                ReDim Lv_aryPARMType(WSPARAM.Length)
                For index = 0 To WSPARAM.Length - 1
                    Lv_aryPARMType(index) = WSPARAM(index).GetType
                Next
            End If
            '若沒有overload的話，第二個參數便不需要，這邊要注意的是WsiProfiles.BasicProfile1_1本身不支援Web Service overload，因此需要改成不遵守WsiProfiles.BasicProfile1_1協議 
            'Dim Lv_imtInvokeMethod As System.Reflection.MethodInfo = Lv_tType.GetMethod(WSMETHODNAME, Lv_aryPARMType)
            Dim Lv_imtInvokeMethod As Reflection.MethodInfo = Gv_WSType.GetMethod(WSMETHODNAME)
            'invoke
            Return Lv_imtInvokeMethod.Invoke(Gv_WSInstance, WSPARAM)
        Catch ex As Exception
            Throw New ArgumentException(ex.InnerException.Message)
        End Try
    End Function


    ''' <summary>
    ''' 取得單筆資料回傳單欄位
    ''' </summary>
    ''' <param name="SQL"></param>
    ''' <param name="ORGID"></param>
    ''' <returns>所取得之第一筆資料中第一個欄位</returns>
    Public Function GetSingleDATA_WEB(SQL As String, ORGID As String) As String
        'Dim Lv_sWS = New WebReference.MES1001
        'Try
        '    Dim Lv_sResult = Lv_sWS.IGetSingleData(SQL, ORGID)
        '    Return Lv_sResult
        'Catch ex As Exception
        '    Throw New ArgumentException("Exception Occured(GetSingleDATA_WEB)" + ex.Message)
        'End Try
        Try
            'Dim baseAddress As New Uri(Pvt_GetWSURL())
            'Dim WSHttpBinding As BasicHttpBinding = New ServiceModel.BasicHttpBinding
            'Dim myEndpoint As EndpointAddress = New EndpointAddress(baseAddress)
            'Dim mychfac As ChannelFactory(Of ServiceReferencePROD.MES1001Soap) = New ChannelFactory(Of ServiceReferencePROD.MES1001Soap)(WSHttpBinding, myEndpoint)
            'Dim Lv_sWS = mychfac.CreateChannel()
            'Dim Lv_sWS As ServiceReferencePROD.MES1001SoapClient = New ServiceReferencePROD.MES1001SoapClient

            Dim Lv_sWS As ServiceReferencePROD.MES1001SoapClient = BuildPRODBinding()
            'Dim Lv_sWS As ServiceReferenceTEST.MES1001SoapClient = BuildTESTBinding()
            Dim Lv_sResult = Lv_sWS.IGetSingleData(SQL, ORGID)
            Return Lv_sResult
        Catch ex As Exception
            Throw New ArgumentException("Exception Occured(GetSingleDATA_WEB)" + ex.Message)
        End Try

        'Dim Lv_aPARAM(1) As Object
        'Lv_aPARAM(0) = SQL
        'Lv_aPARAM(1) = ORGID
        'Try
        '    Dim Lv_sResult = WSInvoke(Pvt_GetWSURL(), "DymWS", "IGetSingleData", Lv_aPARAM)
        '    Return Lv_sResult
        'Catch ex As Exception
        '    Throw New ArgumentException("Exception Occured(GetSingleDATA_WEB)" + ex.Message)
        'End Try

    End Function

    ''' <summary>
    ''' 執行SQL WEB版
    ''' </summary>
    ''' <param name="SQL">陣列型態 將所欲交易之SQL全部一次性傳入</param>
    ''' <param name="ORGID"></param>
    ''' <returns>回傳異常字串</returns>
    Public Function EXECSQL_WEB(SQL() As String, ORGID As String) As String

        Try
            'Dim baseAddress As New Uri(Pvt_GetWSURL())
            'Dim WSHttpBinding As BasicHttpBinding = New ServiceModel.BasicHttpBinding
            'Dim myEndpoint As EndpointAddress = New EndpointAddress(baseAddress)
            'Dim mychfac As ChannelFactory(Of ServiceReferencePROD.MES1001Soap) = New ChannelFactory(Of ServiceReferencePROD.MES1001Soap)(WSHttpBinding, myEndpoint)
            'Dim Lv_sWS = mychfac.CreateChannel()
            Dim Lv_sWS As ServiceReferencePROD.MES1001SoapClient = BuildPRODBinding()
            'Dim Lv_sWS As ServiceReferenceTEST.MES1001SoapClient = BuildTESTBinding()

            Dim Lv_sResult = Lv_sWS.IEXECSQL(SQL, ORGID)
            Return Lv_sResult
        Catch ex As Exception
            Throw ex
        End Try

        'Dim Lv_aPARAM(1) As Object
        'Lv_aPARAM(0) = SQL
        'Lv_aPARAM(1) = ORGID
        'Try
        '    Dim Lv_sResult = WSInvoke(Pvt_GetWSURL(), "DymWS", "IEXECSQL", Lv_aPARAM)
        '    Return Lv_sResult
        'Catch ex As Exception
        '    'Throw New ArgumentException("Exception Occured(EXECSQL_WEB)" + ex.Message)
        '    Throw ex
        'End Try
    End Function

    ''' <summary>
    '''取得多筆資料 WEB版
    ''' </summary>
    ''' <param name="DATA"></param>
    ''' <param name="SQL"></param>
    ''' <param name="ORGID"></param>
    ''' <returns>DataTable</returns>
    Public Function GetSQLData_WEB(ByRef DATA As DataTable, SQL As String, ORGID As String) As DataTable

        '20170606 動態建立WEBSERVICE效能過差需改回原始建立方式
        Try
            'Dim baseAddress As New Uri(Pvt_GetWSURL())
            'Dim WSHttpBinding As BasicHttpBinding = New ServiceModel.BasicHttpBinding
            'Dim myEndpoint As EndpointAddress = New EndpointAddress(baseAddress)
            'Dim mychfac As ChannelFactory(Of ServiceReferencePROD.MES1001Soap) = New ChannelFactory(Of ServiceReferencePROD.MES1001Soap)(WSHttpBinding, myEndpoint)
            'Dim Lv_sWS = mychfac.CreateChannel()
            'Dim Lv_sWS As ServiceReferencePROD.MES1001SoapClient = New ServiceReferencePROD.MES1001SoapClient
            Dim Lv_sWS As ServiceReferencePROD.MES1001SoapClient = BuildPRODBinding()
            'Dim Lv_sWS As ServiceReferenceTEST.MES1001SoapClient = BuildTESTBinding()

            'Dim Lv_sWS As ServiceReferenceTEST.MES1001SoapClient = New ServiceReferenceTEST.MES1001SoapClient
            Dim Lv_dtResult = Lv_sWS.IGetSQLDataEX(SQL, ORGID)

            DATA.Load(Lv_dtResult.CreateDataReader())
            'mychfac.Close()
            'For i As Integer = 0 To Lv_dtResult.Rows.Count - 1
            '    DATA.ImportRow(Lv_dtResult.Rows(i))
            'Next
            Return Lv_dtResult
        Catch ex As Exception
            Throw New ArgumentException("Exception Occured(GetSQLData_WEB)" + ex.Message)
            Return Nothing
        End Try

        'Dim Lv_aPARAM(1) As Object
        'Lv_aPARAM(0) = SQL
        'Lv_aPARAM(1) = ORGID
        'Try
        '    Dim Lv_dtResult = WSInvoke(Pvt_GetWSURL(), "DymWS", "IGetSQLDataEX", Lv_aPARAM)
        '    DATA.Load(Lv_dtResult.CreateDataReader())
        '    Return Lv_dtResult
        'Catch ex As Exception
        '    Throw New ArgumentException("Exception Occured(GetSQLData_WEB)" + ex.InnerException.Message)
        '    Return Nothing
        'End Try
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
                If (Lv_oColumns(i).DataType.ToString.IndexOf("Int") >= 0 Or Lv_oColumns(i).DataType.ToString.IndexOf("Double") >= 0 Or Lv_oColumns(i).DataType.ToString.IndexOf("Decimal") >= 0) And WORKROW.Item(i).ToString = "" Then
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
        For i = 0 To WORKROW.Table.PrimaryKey.Length - 1
            Dim Lv_sColumnName As String = WORKROW.Table.PrimaryKey(i).ColumnName.ToString
            Select Case WORKROW.Table.PrimaryKey(i).DataType.ToString
                Case "System.String"
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "='" + WORKROW.Item(Lv_sColumnName).ToString + "' "
                Case "System.DateTime"
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "=" + "to_date('" + Convert.ToDateTime(WORKROW.Item(Lv_sColumnName)).ToString("u").Substring(0, 19) + "','YYYY/MM/DD HH24:MI:SS')" + " "
                Case Else
                    Lv_sWHERE = Lv_sWHERE + Lv_sColumnName + "=" + WORKROW.Item(Lv_sColumnName).ToString
            End Select
            If i < WORKROW.Table.PrimaryKey.Length - 1 Then
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
            If i < WORKROW.Table.PrimaryKey.Length - 1 Then
                Lv_sWHERE = Lv_sWHERE + " AND "
            End If
        Next
        Lv_sResult = Lv_sResult + Lv_sWHERE
        Return Lv_sResult
    End Function
    Function Get_DATASTR(DATEFORMAT As String, ORGID As String) As String
        Try
            Return GetSingleDATA_WEB("Select TO_CHAR(SYSDATE,'" + DATEFORMAT + "') FROM DUAL", ORGID)
        Catch ex As Exception
            Throw New ArgumentException("取得日期字串失敗" + ex.Message)
        End Try
        Return 0
    End Function
    Function Pvt_GetTableNO(HEAD As String, DATESTR As String, TABLENAME As String, PKEY As String, SEQLEN As Integer, ORGID As String) As String
        Dim Lv_sTemp, Lv_sSQL As String
        Dim Lv_sDATEStr = ""
        'Lv_sDATEStr = DATADATE.Year.ToString.PadLeft(2, "0") + DATADATE.Month.ToString.PadLeft(2) + DATADATE.Day.ToString.PadLeft(2)
        If DATESTR <> "" Then
            Lv_sDATEStr = Get_DATASTR(DATESTR, ORGID)
            '    Lv_sSQL = "Select COALESCE(MAX(" + PKEY + "),'X') AS SID FROM  " + TABLENAME + " WHERE (" + PKEY + " LIKE '" + HEAD + Lv_sDATEStr + "%') and ORG_ID='" + ORGID + "'"
            'Else
            '    Lv_sSQL = "Select COALESCE(MAX(" + PKEY + "),'X') AS SID FROM  " + TABLENAME + " WHERE (" + PKEY + " LIKE '" + HEAD + "%')  and ORG_ID='" + ORGID + "'"
        End If
        Lv_sSQL = "Select COALESCE(MAX(" + PKEY + "),'X') AS SID FROM  " + TABLENAME + " WHERE (" + PKEY + " LIKE '" + HEAD + Lv_sDATEStr + "%') and ORG_ID='" + ORGID + "'"
        Lv_sTemp = GetSingleDATA_WEB(Lv_sSQL, ORGID)
        If Lv_sTemp = "" Then
            Return ""
        End If
        If Lv_sTemp = "X" Then
            Lv_sTemp = HEAD + Lv_sDATEStr + "1".PadLeft(SEQLEN, CChar("0"))
        Else
            Lv_sTemp = HEAD + Lv_sDATEStr + (CInt(Lv_sTemp.Substring(HEAD.Length + DATESTR.Length, SEQLEN)) + 1).ToString.PadLeft(SEQLEN, CChar("0"))
        End If
        Return Lv_sTemp
    End Function


    Private Function BuildPRODBinding() As ServiceReferencePROD.MES1001SoapClient
        Dim Binding As BasicHttpBinding = New BasicHttpBinding()
        Binding.Name = "MES1001Soap"
        Dim endpoint As EndpointAddress = New EndpointAddress("http://eip.szs.com.tw/WEBTRAN/MES1001.asmx")
        Binding.MaxReceivedMessageSize = Int32.MaxValue
        Binding.MaxBufferSize = Int32.MaxValue
        Binding.MaxBufferPoolSize = Int32.MaxValue
        Binding.ReaderQuotas = New XmlDictionaryReaderQuotas()
        Binding.ReaderQuotas.MaxDepth = Int32.MaxValue
        Binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue
        Binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue
        Binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue
        Binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue
        Dim client = New ServiceReferencePROD.MES1001SoapClient(Binding, endpoint)
        Return client
    End Function
    Private Function BuildTESTBinding() As ServiceReferenceTEST.MES1001SoapClient
        Dim Binding As BasicHttpBinding = New BasicHttpBinding()
        Binding.Name = "MES1001Soap"
        Dim endpoint As EndpointAddress = New EndpointAddress("http://10.1.1.49/WEBTRAN/MES1001.asmx")
        Binding.MaxReceivedMessageSize = Int32.MaxValue
        Binding.MaxBufferSize = Int32.MaxValue
        Binding.MaxBufferPoolSize = Int32.MaxValue
        Binding.ReaderQuotas = New XmlDictionaryReaderQuotas()
        Binding.ReaderQuotas.MaxDepth = Int32.MaxValue
        Binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue
        Binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue
        Binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue
        Binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue
        Dim client = New ServiceReferenceTEST.MES1001SoapClient(Binding, endpoint)
        Return client
    End Function

    ''' <summary>
    ''' 取得SERVER資訊
    ''' </summary>
    ''' 傳入參數:SERVERNAME:server名稱
    ''' <returns>SERVERINFO</returns>
    Public Function GetSERVERINFO(ORGID As String, SERVERNAME As String) As SERVERINFO
        Dim Lv_dtResult As DataTable = New DataTable
#If DEBUG Then
        Dim Lv_sSQL As String = "select server_ip,server_port,server_user,server_pass from SFCS_SERVERINFO_all where server_name='" + SERVERNAME + "' and server_status=1 and server_type=2"
#Else
        Dim Lv_sSQL As String = "select server_ip,server_port,server_user,server_pass from SFCS_SERVERINFO_all where server_name='" + SERVERNAME + "' and server_status=1 and server_type=1"
#End If

        Dim Lv_siResult As SERVERINFO
        Try
            GetSQLData_WEB(Lv_dtResult, Lv_sSQL, ORGID)
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

    Public Sub SaveSteamToFile(SRCSTREAM As Stream, STREAMLEN As UInt64, PATH As String)
        Try
            Dim buffer As Byte() = New Byte(STREAMLEN) {}
            Dim Lv_iReadCount, I As UInt64
            Dim Lv_iBlockLen = 1024
            If STREAMLEN < Lv_iBlockLen Then
                Lv_iBlockLen = STREAMLEN
            End If

            Lv_iReadCount = SRCSTREAM.Read(buffer, 0, Lv_iBlockLen)
            If File.Exists(PATH) Then
                File.Delete(PATH)
            End If
            Dim Lv_oFile As BinaryWriter = New BinaryWriter(File.Open(PATH, FileMode.CreateNew, FileAccess.Write))
            While Lv_iReadCount > 0
                Lv_oFile.Write(buffer, 0, Lv_iReadCount)
                Lv_iReadCount = SRCSTREAM.Read(buffer, 0, Lv_iBlockLen)
                I = I + 1
            End While
            Lv_oFile.Close()
        Catch ex As Exception
            Throw New ArgumentException("存入檔案失敗!!=>" + ex.Message)
        End Try
    End Sub
    Public Sub SaveTEXTToFile(SRCTEXT As String, PATH As String)
        Try
            If File.Exists(PATH) Then
                File.Delete(PATH)
            End If
            Dim Lv_swFile As New StreamWriter(PATH)
            Lv_swFile.Write(SRCTEXT)
            Lv_swFile.Close()


            'Dim Lv_oFile As StringWriter = New StringWriter(File.Open(PATH, FileMode.CreateNew, FileAccess.Write))
            'Lv_oFile.Write(SRCTEXT)
            'Lv_oFile.Close()
        Catch ex As Exception
            Throw New ArgumentException("存入檔案失敗!!=>" + ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 檢查目前程式版本是否正確
    ''' </summary>
    ''' <param name="FILEINFO">執行檔名稱+";"+執行檔長度+";"+執行檔日期</param>
    ''' <param name="ORGID"></param>
    ''' <param name="PRGID">執行程式名稱</param>
    ''' <param name="SERVERTYPE">1.正式區2.測試區</param>
    ''' <returns></returns>
    Public Function CHECKFILEUPDATE_WEB(FILEINFO As List(Of String), ORGID As String, PRGID As String, SERVERTYPE As String) As String()
        Dim Lv_aPARAM(3) As Object
        Dim Lv_aryFileInfo As String() = FILEINFO.ToArray

        Lv_aPARAM(0) = Lv_aryFileInfo
        Lv_aPARAM(1) = ORGID
        Lv_aPARAM(2) = PRGID
        Lv_aPARAM(3) = SERVERTYPE
        Try
            Dim Lv_sResult = WSInvoke(Pvt_GetWSURL(), "DymWS", "ICHKPRGVER", Lv_aPARAM)
            Return Lv_sResult
        Catch soapex As SoapException
            Dim Lv_oSOAPINFO = New SoapExceptionInfo(soapex)
            Throw New ArgumentException("Exception Occured(CHECKFILEUPDATE_WEB)" + Lv_oSOAPINFO.ERRMSG)
        End Try

    End Function
    ''' <summary>
    ''' 檢查目前環境檔案版本是否正確
    ''' </summary>
    ''' <param name="FILEINFO">環境檔案名稱+";"+環境檔案長度+";"+環境檔案日期</param>
    ''' <param name="ORGID"></param>
    ''' <param name="PRGID">執行程式名稱</param>
    ''' <param name="SERVERTYPE">1.正式區2.測試區</param>
    ''' <returns></returns>
    Public Function CHECKENVFILEUPDATE_WEB(FILEINFO As List(Of String), ORGID As String, PRGID As String, SERVERTYPE As String) As String()
        Dim Lv_aPARAM(3) As Object
        Dim Lv_aryFileInfo As String() = FILEINFO.ToArray

        Lv_aPARAM(0) = Lv_aryFileInfo
        Lv_aPARAM(1) = ORGID
        Lv_aPARAM(2) = PRGID
        Lv_aPARAM(3) = SERVERTYPE
        Try
            Dim Lv_sResult = WSInvoke(Pvt_GetWSURL(), "DymWS", "ICHKENVVER", Lv_aPARAM)
            Return Lv_sResult
        Catch soapex As SoapException
            Dim Lv_oSOAPINFO = New SoapExceptionInfo(soapex)
            Throw New ArgumentException("Exception Occured(CHECKENVFILEUPDATE_WEB)" + Lv_oSOAPINFO.ERRMSG)
        End Try

    End Function

    Class SoapExceptionInfo
        Public ERRNUM As String = String.Empty
        Public ERRMSG As String = String.Empty
        Public ERRSRC As String = String.Empty
        Sub New()
        End Sub
        Sub New(SOAPPACK As SoapException)
            Dim Lv_oXML As XmlDocument = New XmlDocument
            Lv_oXML.LoadXml(SOAPPACK.Detail.OuterXml)
            Dim Lv_oNode As XmlNode = Lv_oXML.DocumentElement.SelectSingleNode("Error")
            ERRNUM = Lv_oNode.SelectSingleNode("ErrorNumber").InnerText
            ERRMSG = Lv_oNode.SelectSingleNode("ErrorMessage").InnerText
            ERRSRC = Lv_oNode.SelectSingleNode("ErrorSource").InnerText
        End Sub
    End Class


End Module
