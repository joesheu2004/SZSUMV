'Imports Oracle.ManagedDataAccess.Client
Imports System.IO



Public Class MESUMV03
    Public Const p_department_code As String = "CNC"
    Public Const p_org_id As Integer = 1
    Public Const p_wo_dept As String = "PL"
    Private Shared ReadOnly Gv_VERSION As String = "0001"

    Public Gv_dtPlant As DataTable = New DataTable
    Private Gv_dtBuilding As DataTable = New DataTable
    Private Gv_dtDutykind As DataTable = New DataTable
    Private Gv_dtEmp As DataTable = New DataTable
    Private Gv_dtEquipment As DataTable = New DataTable
    Private Gv_dtWO As DataTable = New DataTable
    Private Gv_dtFAI As DataTable = New DataTable
    Private Shared Gv_USER As String = "SFCSUSER"

    Private Sub MESUMV03_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Pvt_PGALREADYLOAD() Then
            MessageBox.Show("不允許重複開啟程式")
            Close()
        End If

        If Not Pvt_CheckPRIV("1") Then
            MessageBox.Show("請先登入AD")
            Close()
        End If

        lbEmpName.Text = Nothing
        lbAssembly.Text = Nothing
        lbEmpName.Text = Nothing
        lbFAIDesc.Text = Nothing

        'Dim oraCon As New OracleConnection()
        'oraCon = GetConnecttion()

        Dim query_string As String = "select code,value from code_tab_all where org_id=" & p_org_id &
            " and close_flag='N' and type='PLANT' order by 1"
        'Dim dt_rows As Integer = 0
        'dt_rows = Pvt_GetSQLDATA(Gv_dtPlant, query_string, p_org_id)

        GetSQLData_WEB(Gv_dtPlant, query_string, p_org_id)
        If Gv_dtPlant.Rows.Count > 0 Then
            cbPlant.DataSource = Gv_dtPlant
            cbPlant.DisplayMember = "value"
            cbPlant.ValueMember = "code"
        End If
        'building
        Dim qs_building As String = "select code from code_tab where org_id=" & p_org_id &
            " and type='SAHO_BUDLING' and close_flag='N' order by code"
        'dt_rows = 0
        'dt_rows = Pvt_GetSQLDATA(Gv_dtBuilding, qs_building, p_org_id)

        GetSQLData_WEB(Gv_dtBuilding, qs_building, p_org_id)
        If Gv_dtBuilding.Rows.Count > 0 Then
            cbBuilding.DataSource = Gv_dtBuilding
            cbBuilding.DisplayMember = "code"
            cbBuilding.ValueMember = "code"
        End If
        Dim qs_dutykind As String = "select duty_kind,name from ps_duty where org_id=" & p_org_id &
            " and close_flag='N' and loc='T' and duty_kind <> 'I' order by duty_kind "
        'dt_rows = 0
        'dt_rows = Pvt_GetSQLDATA(Gv_dtDutykind, qs_dutykind, p_org_id)
        GetSQLData_WEB(Gv_dtDutykind, qs_dutykind, p_org_id)
        If Gv_dtDutykind.Rows.Count > 0 Then
            cbDutykind.DataSource = Gv_dtDutykind
            cbDutykind.DisplayMember = "name"
            cbDutykind.ValueMember = "duty_kind"
        End If

        Me.Text = GetSYSTEMTITLE() + " 程式版本Ver-[" + Gv_VERSION + "] IP-" + GetIPaddress()
        'Me.WindowState = FormWindowState.Maximized
    End Sub
    '2018.3.9 Joe
    'Private Function Pvt_GetSQLDATA(ByRef DATA As DataTable, SQL As String, ORGID As String, Optional TARN As OracleTransaction = Nothing) As Int64
    '    Pvt_GetSQLDATA = 0
    '    Dim Lv_oCMD As OracleCommand = Nothing
    '    Dim Lv_oCNT As OracleConnection = Nothing
    '    Try
    '        'MessageBox.Show("Pvt_GetSQLDATA")
    '        Lv_oCNT = GetConnecttion()
    '        Lv_oCMD = New OracleCommand("begin set_org_id(" + ORGID + ") ;end;", Lv_oCNT)
    '        If Not IsNothing(TARN) Then
    '            Lv_oCMD.Transaction = TARN
    '        End If
    '        Lv_oCMD.ExecuteNonQuery()
    '        Lv_oCMD.CommandText = SQL

    '        Dim Lv_drData As OracleDataReader = Lv_oCMD.ExecuteReader()
    '        If Lv_drData.HasRows() Then
    '            DATA.Load(Lv_drData)
    '            Return DATA.Rows.Count
    '        End If
    '    Catch ex As Exception
    '        Console.WriteLine(ex.Message)
    '        Console.WriteLine("ERROR SQL=>" + SQL)
    '        'Throw New ArgumentException("GetSQLDATA Error Occured" + vbCrLf + ex.Message + vbCrLf + SQL)
    '    Finally
    '        ' 釋放資源
    '        If Not Lv_oCMD Is Nothing Then
    '            Lv_oCMD.Dispose()
    '        End If
    '        Lv_oCNT.Close()
    '    End Try
    'End Function

    Private Function Pvt_PGALREADYLOAD() As Boolean

        Dim Lv_sPName As String = Process.GetCurrentProcess.ProcessName
        Dim Lv_iHAndle As Integer = Process.GetCurrentProcess.Id
        For Each Lv_pName As Process In Process.GetProcesses()
            Dim Lv_sName = Lv_pName.ProcessName
            If Lv_sPName = Lv_pName.ProcessName And Lv_iHAndle <> Lv_pName.Id Then
                Return True
            End If

        Next
        Return False
    End Function
    Private Function Pvt_CheckPRIV(CHECKTYPE As String) As Boolean
        Dim DomainName As String = Environment.UserDomainName
        Dim UserName As String = Environment.UserName '使用者帳號 
        Gv_USER = UserName
        Dim path As String = "WinNT://" + DomainName + "/" + UserName
        Dim FullName As String

        Try
            Using sds As System.DirectoryServices.DirectoryEntry = New System.DirectoryServices.DirectoryEntry(path)
                FullName = sds.Properties("FullName").Value.ToString() '全名 
                Gv_USER = FullName
                If Gv_USER.Trim <> "" AndAlso DomainName = "SZS" Then
                    Return True
                Else
                    If CHECKTYPE = "1" Then
                        Dim Lv_frmLogin As LoginForm1 = New LoginForm1()
                        Lv_frmLogin.ShowDialog()
                        If Lv_frmLogin.DialogResult = DialogResult.OK Then
                            Lv_frmLogin.Close()
                            Return True
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Return False
        End Try
        MessageBox.Show("DomainName=>" + DomainName + "UserName=>" + UserName + " FullName=>" + FullName)
        Return False
    End Function

    Private Sub btnClear1_Click(sender As Object, e As EventArgs) Handles btnClear1.Click
        tbEmpno.Text = Nothing
        lbEmpName.Text = Nothing
        Panel1.Controls.Clear()
        Gv_dtEmp.Clear()
    End Sub

    Private Sub btnClear3_Click(sender As Object, e As EventArgs) Handles btnClear3.Click
        tbWoNumber.Text = Nothing
        lbAssembly.Text = Nothing
        If Gv_dtWO.Rows.Count > 0 Then
            Gv_dtWO.Clear()
        End If
        Panel1.Controls.Clear()
    End Sub

    Private Sub btnClear4_Click(sender As Object, e As EventArgs) Handles btnClear4.Click
        tbToolingQR.Text = Nothing
    End Sub

    Private Sub btnGetEmp_Click(sender As Object, e As EventArgs) Handles btnGetEmp.Click
        Panel1.Controls.Clear()
        Gv_dtEmp.Clear()
        tbEmpno.Text = Nothing
        'Dim oraCon As New OracleConnection()
        'oraCon = GetConnecttion()
        Dim qs_emp As String = "select empno,empno||' '||emp_name emp_name from mes_emp_equipments_temp_v " &
            " where org_id=" & p_org_id & " and wo_dept='" & p_wo_dept & "' and eq_plant='" &
            cbPlant.SelectedValue.ToString & "' and eq_building='" & cbBuilding.SelectedValue.ToString & "'" &
            " and eq_floor=" & tbFloor.Text & " and duty_kind='" & cbDutykind.SelectedValue.ToString & "'" &
            " group by empno,empno||' '||emp_name order by empno"
        'Dim dt_rows As Integer = 0
        'dt_rows = 
        GetSQLData_WEB(Gv_dtEmp, qs_emp, p_org_id)
        Dim x, y As Integer
        If Gv_dtEmp.Rows.Count > 0 Then
            For i As Integer = 0 To Gv_dtEmp.Rows.Count - 1 Step 1
                Dim btn_emp As New Button
                btn_emp.Text = Gv_dtEmp.Rows(i)("emp_name").ToString
                btn_emp.Name = Gv_dtEmp.Rows(i)("empno").ToString
                btn_emp.Size = New Size(150, 50)
                btn_emp.Font = New Font("Tahoma", 12)
                Panel1.Controls.Add(btn_emp)
                'AddHandler bt_Model.Click, AddressOf SelectModel_Sub
                AddHandler btn_emp.Click, AddressOf Me.btn_emp_Click
                x = 3 + (150 * Math.Truncate((i / 9)))
                y = 3 + (50 * (i Mod 5))
                'MessageBox.Show("x is " & x.ToString & ",y is " & y.ToString)
                btn_emp.Location = New Point(x, y)
            Next
        End If
    End Sub
    Private Sub btn_emp_Click(sender As Object, e As EventArgs)
        Dim button As Button = TryCast(sender, Button)
        tbEmpno.Text = button.Name.ToString
        lbEmpName.Text = Mid(button.Text.ToString, 9)
        'BT_WO_LIST.Select()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        Panel1.Controls.Clear()
        tbEmpno.Text = Nothing
        tbEquipment.Text = Nothing
        tbFAIcode.Text = Nothing
        tbWoNumber.Text = Nothing
        tbToolingQR.Text = Nothing
        lbEmpName.Text = Nothing
        lbAssembly.Text = Nothing
        lbFAIDesc.Text = Nothing
    End Sub

    Private Sub btnEquipment_Click(sender As Object, e As EventArgs) Handles btnEquipment.Click
        If Gv_dtEquipment.Rows.Count > 0 Then
            Gv_dtEquipment.Clear()
        End If
        Panel1.Controls.Clear()
        tbEquipment.Text = Nothing
        'Dim oraCon As New OracleConnection()
        'oraCon = GetConnecttion()
        Dim qs_str As String = "select eq_code from mes_emp_equipments_temp_v " &
            " where org_id=" & p_org_id & " and wo_dept='" & p_wo_dept & "' and eq_plant='" &
            cbPlant.SelectedValue.ToString & "' and eq_building='" & cbBuilding.SelectedValue.ToString & "'" &
            " and eq_floor=" & tbFloor.Text & " and duty_kind='" & cbDutykind.SelectedValue.ToString & "'" &
            " and empno='" & tbEmpno.Text.Trim & "' order by eq_code"
        'MessageBox.Show(cbPlant.SelectedValue.ToString & "/" & cbBuilding.SelectedValue.ToString & "/" & tbFloor.Text.ToString & "/" & cbDutykind.SelectedValue.ToString & "/" & tbEmpno.Text.ToString.Trim)
        'Dim dt_rows As Integer = 0
        'dt_rows = Pvt_GetSQLDATA(Gv_dtEquipment, qs_str, p_org_id)
        GetSQLData_WEB(Gv_dtEquipment, qs_str, p_org_id)
        Dim x, y As Integer
        If Gv_dtEquipment.Rows.Count > 0 Then
            For i As Integer = 0 To Gv_dtEquipment.Rows.Count - 1 Step 1
                Dim btn_equipment As New Button
                btn_equipment.Text = Gv_dtEquipment.Rows(i)("eq_code").ToString
                btn_equipment.Name = Gv_dtEquipment.Rows(i)("eq_code").ToString
                btn_equipment.Size = New Size(150, 50)
                btn_equipment.Font = New Font("Tahoma", 12)
                Panel1.Controls.Add(btn_equipment)
                'AddHandler bt_Model.Click, AddressOf SelectModel_Sub
                AddHandler btn_equipment.Click, AddressOf Me.btn_equipment_Click
                x = 3 + (150 * Math.Truncate((i / 9)))
                y = 3 + (50 * (i Mod 9))
                'MessageBox.Show("x is " & x.ToString & ",y is " & y.ToString)
                btn_equipment.Location = New Point(x, y)
            Next
        End If

    End Sub
    Private Sub btn_equipment_Click(sender As Object, e As EventArgs)
        Dim button As Button = TryCast(sender, Button)
        tbEquipment.Text = button.Name.ToString
    End Sub

    Private Sub btnClear2_Click(sender As Object, e As EventArgs) Handles btnClear2.Click
        Gv_dtEquipment.Clear()
        Panel1.Controls.Clear()
        tbEquipment.Text = Nothing
    End Sub

    Private Sub btnWo_Click(sender As Object, e As EventArgs) Handles btnWo.Click
        If Pvt_CheckHeader("H") = False Then
            MessageBox.Show("錯誤：請先設定廠區等資訊")
            Exit Sub
        End If
        If String.IsNullOrEmpty(tbEquipment.Text.ToString.Trim) Then
            MessageBox.Show("錯誤：請先選擇機台")
            Exit Sub
        End If
        Panel1.Controls.Clear()
        If Gv_dtWO.Rows.Count > 0 Then
            Gv_dtWO.Clear()
        End If
        tbWoNumber.Text = Nothing
        lbAssembly.Text = Nothing

        'Dim oraCon As New OracleConnection()
        'oraCon = GetConnecttion()
        Dim qs_wo As String = "select a.wo_number,a.wo_number||' '||wj.assembly_item assembly_item " &
            " from mes_wo_equipments_temp_v a,wo_job wj" &
            " where a.org_id=" & p_org_id & " and a.wo_dept='" & p_wo_dept & "' and a.eq_code='" & tbEquipment.Text.ToString.Trim & "'" &
            " and a.eq_plant='" & cbPlant.SelectedValue.ToString & "'" &
            " and a.eq_building = '" & cbBuilding.SelectedValue.ToString.Trim & "'" &
            " and a.eq_floor=" & tbFloor.Text &
            " and nvl(a.duty_kind,'X')='" & cbDutykind.SelectedValue.ToString.Trim & "'" &
            " and a.org_id=wj.org_id(+) and a.wo_number = wj.wo_number(+) and a.wo_number is not null order by 1"
        'Dim dt_rows As Integer = 0
        'dt_rows = Pvt_GetSQLDATA(Gv_dtWO, qs_wo, p_org_id)

        GetSQLData_WEB(Gv_dtWO, qs_wo, p_org_id)

        If Gv_dtWO.Rows.Count > 0 Then
            Dim x, y As Integer
            For i As Integer = 0 To Gv_dtWO.Rows.Count - 1 Step 1
                Dim btn_wo As New Button
                btn_wo.Text = Gv_dtWO.Rows(i)("assembly_item").ToString
                btn_wo.Name = Gv_dtWO.Rows(i)("wo_number").ToString
                btn_wo.Size = New Size(150, 50)
                btn_wo.Font = New Font("Tahoma", 12)
                Panel1.Controls.Add(btn_wo)
                'AddHandler bt_Model.Click, AddressOf SelectModel_Sub
                AddHandler btn_wo.Click, AddressOf Me.btn_wo_Click
                x = 3 + (150 * Math.Truncate((i / 9)))
                y = 3 + (50 * (i Mod 9))
                btn_wo.Location = New Point(x, y)
            Next

        Else
            MessageBox.Show("機台" & tbEquipment.Text & "沒有預設的工單")
        End If
    End Sub
    Private Sub btn_wo_Click(sender As Object, e As EventArgs)
        Dim button As Button = TryCast(sender, Button)
        tbWoNumber.Text = button.Name.ToString
        lbAssembly.Text = Mid(button.Text.ToString, Len(button.Name.ToString.Trim) + 2)
    End Sub
    Private Function Pvt_CheckHeader(CHECKTYPE As String) As Boolean
        'If CHECKTYPE = "H" Then
        If String.IsNullOrEmpty(cbPlant.SelectedValue.ToString.Trim) Or String.IsNullOrEmpty(cbBuilding.SelectedValue.ToString.Trim) Or String.IsNullOrEmpty(tbFloor.Text.ToString.Trim) Or String.IsNullOrEmpty(cbDutykind.SelectedValue.ToString.Trim) Then
            Return False
        Else
            Return True
        End If
        'End If
    End Function

    Private Sub btnFAI_Click(sender As Object, e As EventArgs) Handles btnFAI.Click
        If Pvt_CheckHeader("H") = False Then
            MessageBox.Show("錯誤：請先設定廠區等資訊")
            Exit Sub
        End If
        If Gv_dtFAI.Rows.Count > 0 Then
            Gv_dtFAI.Clear()
        End If
        Panel1.Controls.Clear()
        tbFAIcode.Text = Nothing
        lbFAIDesc.Text = Nothing

        '
        'Dim oraCon As New OracleConnection()
        'oraCon = GetConnecttion()
        Dim qs_fai As String = "select code,value from mes_code_tab" &
            " where org_id=" & p_org_id & " and type='UMV_FAI_CODE' and close_flag='N' order by code"
        'Dim dt_rows As Integer = 0
        'dt_rows = Pvt_GetSQLDATA(Gv_dtFAI, qs_fai, p_org_id)
        GetSQLData_WEB(Gv_dtFAI, qs_fai, p_org_id)
        If Gv_dtFAI.Rows.Count > 0 Then
            Dim x, y As Integer
            For i As Integer = 0 To Gv_dtFAI.Rows.Count - 1 Step 1
                Dim btn_fai As New Button
                btn_fai.Text = Gv_dtFAI.Rows(i)("value").ToString
                btn_fai.Name = Gv_dtFAI.Rows(i)("code").ToString
                btn_fai.Size = New Size(150, 50)
                btn_fai.Font = New Font("Tahoma", 12)
                Panel1.Controls.Add(btn_fai)
                'AddHandler bt_Model.Click, AddressOf SelectModel_Sub
                AddHandler btn_fai.Click, AddressOf Me.btn_fai_Click
                x = 3 + (150 * Math.Truncate((i / 9)))
                y = 3 + (50 * (i Mod 9))
                btn_fai.Location = New Point(x, y)
            Next

        Else
            MessageBox.Show("錯誤：查無首件時機代碼，請通知MIS")
        End If
    End Sub
    Private Sub btn_fai_Click(sender As Object, e As EventArgs)
        Dim button As Button = TryCast(sender, Button)
        tbFAIcode.Text = button.Name.ToString
        lbFAIDesc.Text = button.Text
    End Sub

    Private Sub btnClear5_Click(sender As Object, e As EventArgs) Handles btnClear5.Click
        If Gv_dtFAI.Rows.Count > 0 Then
            Gv_dtFAI.Clear()
        End If
        Panel1.Controls.Clear()
        tbFAIcode.Text = Nothing
        lbFAIDesc.Text = Nothing

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Pvt_CheckHeader("H") = False Then
            MessageBox.Show("錯誤：請先設定廠區等資訊")
            Exit Sub
        End If
        '檢查技師
        If String.IsNullOrEmpty(tbEmpno.Text.ToString.Trim) Then
            MessageBox.Show("錯誤：技師工號不允許為空值")
            Exit Sub
        Else
            'Dim oraCon As New OracleConnection()
            'oraCon = GetConnecttion()
            Dim qs_test As String = "select empno from ps_employee where org_id=" & p_org_id &
                            " and empno='" & tbEmpno.Text.ToString.Trim & "' and lev_date is null"
            Dim qs_result As String = Nothing
            'qs_result = GetSingleDATA(qs_test, p_org_id)
            qs_result = GetSingleDATA_WEB(qs_test, p_org_id)
            If String.IsNullOrEmpty(qs_result) Then
                MessageBox.Show("錯誤：工號" & tbEmpno.Text & "查無此人員")

                Exit Sub
            End If
        End If
        '檢查機台
        If String.IsNullOrEmpty(tbEquipment.Text.ToString.Trim) Then
            MessageBox.Show("錯誤：機台編號不允許為空值")
            Exit Sub
        Else
            'Dim oraCon As New OracleConnection()
            'oraCon = GetConnecttion()
            Dim qs_test As String = "select eq_code from mes_equipment where org_id=" & p_org_id &
                            " and wo_dept='" & p_wo_dept & "'" &
                            " and eq_code='" & tbEquipment.Text.ToString.Trim & "'" &
                            " and eq_plant='" & cbPlant.SelectedValue & "'" &
                            " and eq_building='" & cbBuilding.SelectedValue & "'" &
                            " and eq_floor=" & tbFloor.Text & "'" &
                            " and eq_enable='Y'"
            Dim qs_result As String = Nothing
            'qs_result = GetSingleDATA(qs_test, p_org_id)
            qs_result = GetSingleDATA_WEB(qs_test, p_org_id)
            If String.IsNullOrEmpty(qs_result) Then
                MessageBox.Show("錯誤：機台編號" & tbEquipment.Text & " 異常!!!")
                Exit Sub
            End If
        End If
        '檢查工單
        If String.IsNullOrEmpty(tbWoNumber.Text.ToString.Trim) Then
            MessageBox.Show("錯誤：工單號碼不可為空值")
            Exit Sub
        Else
            '
            'Dim oraCon As New OracleConnection()
            'oraCon = GetConnecttion()
            Dim qs_test As String = "select wo_number from wo_job where org_id=" & p_org_id &
                " and wo_number='" & tbWoNumber.Text.Trim & "'"
            Dim qs_result As String = Nothing
            'qs_result = GetSingleDATA(qs_test, p_org_id)
            qs_result = GetSingleDATA_WEB(qs_test, p_org_id)
            If String.IsNullOrEmpty(qs_result) Then
                MessageBox.Show("錯誤：工單號碼" & tbWoNumber.Text & " 不存在ERP!!!")
                Exit Sub
            End If
        End If
        '檢查母模qr code
        If String.IsNullOrEmpty(tbToolingQR.Text.ToString.Trim) Then
            MessageBox.Show("錯誤：母模QR Code不可為空值")
        End If
        '檢查首件時機
        If String.IsNullOrEmpty(tbFAIcode.Text.Trim) Then
            MessageBox.Show("錯誤：首件時機不可為空值")
            Exit Sub
        Else
            'Dim oraCon As New OracleConnection()
            'oraCon = GetConnecttion()
            Dim qs_test As String = "select code from mes_code_tab where org_id=" & p_org_id &
                " and type='UMV_FAI_CODE' and close_flag='N' and code='" & tbFAIcode.Text.Trim & "'"
            Dim qs_result As String = Nothing
            'qs_result = GetSingleDATA(qs_test, p_org_id)
            qs_result = GetSingleDATA_WEB(qs_test, p_org_id)
            If String.IsNullOrEmpty(qs_result) Then
                MessageBox.Show("錯誤：首件時機代碼" & tbFAIcode.Text & " 不存在系統!!!")
                Exit Sub
            End If
        End If
        '
    End Sub

    Private Sub tbDebugBase_BindingContextChanged(sender As Object, e As EventArgs) Handles tbDebugBase.BindingContextChanged
#If DEBUG Then
        ISAutoUpdate = False
#Else
        ISAutoUpdate = True
#End If

    End Sub
End Class
