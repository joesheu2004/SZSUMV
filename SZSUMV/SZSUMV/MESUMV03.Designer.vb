<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MESUMV03
    Inherits WindowsApplication1.BaseForm_NOMenu_

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cbPlant = New System.Windows.Forms.ComboBox()
        Me.cbBuilding = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbFloor = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnGetEmp = New System.Windows.Forms.Button()
        Me.btnEquipment = New System.Windows.Forms.Button()
        Me.btnWo = New System.Windows.Forms.Button()
        Me.tbWoNumber = New System.Windows.Forms.TextBox()
        Me.tbEquipment = New System.Windows.Forms.TextBox()
        Me.tbEmpno = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbToolingQR = New System.Windows.Forms.TextBox()
        Me.btnClear4 = New System.Windows.Forms.Button()
        Me.btnClear1 = New System.Windows.Forms.Button()
        Me.btnClear2 = New System.Windows.Forms.Button()
        Me.btnClear3 = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.cbDutykind = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnFAI = New System.Windows.Forms.Button()
        Me.tbFAIcode = New System.Windows.Forms.TextBox()
        Me.lbFAIDesc = New System.Windows.Forms.Label()
        Me.lbAssembly = New System.Windows.Forms.Label()
        Me.lbEmpName = New System.Windows.Forms.Label()
        Me.btnClear5 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'tbDebugBase
        '
        Me.tbDebugBase.Location = New System.Drawing.Point(758, 50)
        '
        'cbPlant
        '
        Me.cbPlant.BackColor = System.Drawing.Color.LightGoldenrodYellow
        Me.cbPlant.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.cbPlant.FormattingEnabled = True
        Me.cbPlant.Location = New System.Drawing.Point(52, 12)
        Me.cbPlant.Name = "cbPlant"
        Me.cbPlant.Size = New System.Drawing.Size(145, 24)
        Me.cbPlant.TabIndex = 0
        '
        'cbBuilding
        '
        Me.cbBuilding.BackColor = System.Drawing.Color.LightGoldenrodYellow
        Me.cbBuilding.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.cbBuilding.FormattingEnabled = True
        Me.cbBuilding.Location = New System.Drawing.Point(264, 12)
        Me.cbBuilding.Name = "cbBuilding"
        Me.cbBuilding.Size = New System.Drawing.Size(120, 24)
        Me.cbBuilding.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label1.Location = New System.Drawing.Point(8, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(40, 16)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "廠區"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label2.Location = New System.Drawing.Point(222, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 16)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "建物"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label3.Location = New System.Drawing.Point(415, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 16)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "樓層"
        '
        'tbFloor
        '
        Me.tbFloor.BackColor = System.Drawing.Color.LightGoldenrodYellow
        Me.tbFloor.Font = New System.Drawing.Font("細明體", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.tbFloor.Location = New System.Drawing.Point(462, 13)
        Me.tbFloor.Name = "tbFloor"
        Me.tbFloor.Size = New System.Drawing.Size(120, 27)
        Me.tbFloor.TabIndex = 5
        Me.tbFloor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Gainsboro
        Me.Panel1.Location = New System.Drawing.Point(5, 264)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(877, 497)
        Me.Panel1.TabIndex = 6
        '
        'btnClear
        '
        Me.btnClear.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnClear.Location = New System.Drawing.Point(732, 195)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(150, 50)
        Me.btnClear.TabIndex = 7
        Me.btnClear.Text = "清空資料"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnGetEmp
        '
        Me.btnGetEmp.Font = New System.Drawing.Font("細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnGetEmp.Location = New System.Drawing.Point(45, 83)
        Me.btnGetEmp.Name = "btnGetEmp"
        Me.btnGetEmp.Size = New System.Drawing.Size(100, 30)
        Me.btnGetEmp.TabIndex = 8
        Me.btnGetEmp.Text = "選擇技師"
        Me.btnGetEmp.UseVisualStyleBackColor = True
        '
        'btnEquipment
        '
        Me.btnEquipment.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnEquipment.Location = New System.Drawing.Point(313, 83)
        Me.btnEquipment.Name = "btnEquipment"
        Me.btnEquipment.Size = New System.Drawing.Size(100, 30)
        Me.btnEquipment.TabIndex = 11
        Me.btnEquipment.Text = "機台"
        Me.btnEquipment.UseVisualStyleBackColor = True
        '
        'btnWo
        '
        Me.btnWo.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnWo.Location = New System.Drawing.Point(581, 83)
        Me.btnWo.Name = "btnWo"
        Me.btnWo.Size = New System.Drawing.Size(100, 30)
        Me.btnWo.TabIndex = 13
        Me.btnWo.Text = "工單"
        Me.btnWo.UseVisualStyleBackColor = True
        '
        'tbWoNumber
        '
        Me.tbWoNumber.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.tbWoNumber.Location = New System.Drawing.Point(551, 118)
        Me.tbWoNumber.Name = "tbWoNumber"
        Me.tbWoNumber.Size = New System.Drawing.Size(160, 27)
        Me.tbWoNumber.TabIndex = 14
        '
        'tbEquipment
        '
        Me.tbEquipment.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.tbEquipment.Location = New System.Drawing.Point(283, 118)
        Me.tbEquipment.Name = "tbEquipment"
        Me.tbEquipment.Size = New System.Drawing.Size(160, 27)
        Me.tbEquipment.TabIndex = 15
        '
        'tbEmpno
        '
        Me.tbEmpno.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.tbEmpno.Location = New System.Drawing.Point(15, 117)
        Me.tbEmpno.Name = "tbEmpno"
        Me.tbEmpno.Size = New System.Drawing.Size(160, 27)
        Me.tbEmpno.TabIndex = 16
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("微軟正黑體", 12.0!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label4.Location = New System.Drawing.Point(15, 195)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(108, 20)
        Me.Label4.TabIndex = 17
        Me.Label4.Text = "母模QR Code"
        '
        'tbToolingQR
        '
        Me.tbToolingQR.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.tbToolingQR.Location = New System.Drawing.Point(15, 219)
        Me.tbToolingQR.Name = "tbToolingQR"
        Me.tbToolingQR.Size = New System.Drawing.Size(160, 27)
        Me.tbToolingQR.TabIndex = 18
        '
        'btnClear4
        '
        Me.btnClear4.Location = New System.Drawing.Point(177, 221)
        Me.btnClear4.Name = "btnClear4"
        Me.btnClear4.Size = New System.Drawing.Size(30, 23)
        Me.btnClear4.TabIndex = 19
        Me.btnClear4.Text = "X"
        Me.btnClear4.UseVisualStyleBackColor = True
        '
        'btnClear1
        '
        Me.btnClear1.Location = New System.Drawing.Point(177, 119)
        Me.btnClear1.Name = "btnClear1"
        Me.btnClear1.Size = New System.Drawing.Size(30, 23)
        Me.btnClear1.TabIndex = 20
        Me.btnClear1.Text = "X"
        Me.btnClear1.UseVisualStyleBackColor = True
        '
        'btnClear2
        '
        Me.btnClear2.Location = New System.Drawing.Point(446, 120)
        Me.btnClear2.Name = "btnClear2"
        Me.btnClear2.Size = New System.Drawing.Size(30, 23)
        Me.btnClear2.TabIndex = 21
        Me.btnClear2.Text = "X"
        Me.btnClear2.UseVisualStyleBackColor = True
        '
        'btnClear3
        '
        Me.btnClear3.Location = New System.Drawing.Point(714, 120)
        Me.btnClear3.Name = "btnClear3"
        Me.btnClear3.Size = New System.Drawing.Size(30, 23)
        Me.btnClear3.TabIndex = 22
        Me.btnClear3.Text = "X"
        Me.btnClear3.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnSave.Location = New System.Drawing.Point(580, 195)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(150, 50)
        Me.btnSave.TabIndex = 23
        Me.btnSave.Text = "存檔"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'cbDutykind
        '
        Me.cbDutykind.BackColor = System.Drawing.Color.LightGoldenrodYellow
        Me.cbDutykind.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.cbDutykind.FormattingEnabled = True
        Me.cbDutykind.Location = New System.Drawing.Point(677, 12)
        Me.cbDutykind.Name = "cbDutykind"
        Me.cbDutykind.Size = New System.Drawing.Size(120, 24)
        Me.cbDutykind.TabIndex = 24
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Label5.Location = New System.Drawing.Point(632, 18)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(40, 16)
        Me.Label5.TabIndex = 25
        Me.Label5.Text = "班別"
        '
        'btnFAI
        '
        Me.btnFAI.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnFAI.Location = New System.Drawing.Point(313, 183)
        Me.btnFAI.Name = "btnFAI"
        Me.btnFAI.Size = New System.Drawing.Size(100, 30)
        Me.btnFAI.TabIndex = 26
        Me.btnFAI.Text = "首件時機"
        Me.btnFAI.UseVisualStyleBackColor = True
        '
        'tbFAIcode
        '
        Me.tbFAIcode.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.tbFAIcode.Location = New System.Drawing.Point(283, 219)
        Me.tbFAIcode.Name = "tbFAIcode"
        Me.tbFAIcode.Size = New System.Drawing.Size(50, 27)
        Me.tbFAIcode.TabIndex = 27
        '
        'lbFAIDesc
        '
        Me.lbFAIDesc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbFAIDesc.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.lbFAIDesc.Location = New System.Drawing.Point(337, 219)
        Me.lbFAIDesc.Name = "lbFAIDesc"
        Me.lbFAIDesc.Size = New System.Drawing.Size(140, 27)
        Me.lbFAIDesc.TabIndex = 28
        Me.lbFAIDesc.Text = "lbFAIDesc"
        Me.lbFAIDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbAssembly
        '
        Me.lbAssembly.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbAssembly.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.lbAssembly.Location = New System.Drawing.Point(551, 152)
        Me.lbAssembly.Name = "lbAssembly"
        Me.lbAssembly.Size = New System.Drawing.Size(160, 23)
        Me.lbAssembly.TabIndex = 29
        Me.lbAssembly.Text = "lbAssembly"
        Me.lbAssembly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbEmpName
        '
        Me.lbEmpName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lbEmpName.Font = New System.Drawing.Font("Times New Roman", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.lbEmpName.Location = New System.Drawing.Point(15, 151)
        Me.lbEmpName.Name = "lbEmpName"
        Me.lbEmpName.Size = New System.Drawing.Size(160, 23)
        Me.lbEmpName.TabIndex = 30
        Me.lbEmpName.Text = "lbEmpName"
        Me.lbEmpName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnClear5
        '
        Me.btnClear5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.btnClear5.Location = New System.Drawing.Point(480, 221)
        Me.btnClear5.Name = "btnClear5"
        Me.btnClear5.Size = New System.Drawing.Size(30, 23)
        Me.btnClear5.TabIndex = 31
        Me.btnClear5.Text = "X"
        Me.btnClear5.UseVisualStyleBackColor = True
        '
        'MESUMV03
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(896, 773)
        Me.Controls.Add(Me.btnClear5)
        Me.Controls.Add(Me.lbEmpName)
        Me.Controls.Add(Me.lbAssembly)
        Me.Controls.Add(Me.lbFAIDesc)
        Me.Controls.Add(Me.tbFAIcode)
        Me.Controls.Add(Me.btnFAI)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.cbDutykind)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnClear3)
        Me.Controls.Add(Me.btnClear2)
        Me.Controls.Add(Me.btnClear1)
        Me.Controls.Add(Me.btnClear4)
        Me.Controls.Add(Me.tbToolingQR)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.tbEmpno)
        Me.Controls.Add(Me.tbEquipment)
        Me.Controls.Add(Me.tbWoNumber)
        Me.Controls.Add(Me.btnWo)
        Me.Controls.Add(Me.btnEquipment)
        Me.Controls.Add(Me.btnGetEmp)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.tbFloor)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbBuilding)
        Me.Controls.Add(Me.cbPlant)
        Me.Name = "MESUMV03"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "母模與工件資訊綁定"
        Me.Controls.SetChildIndex(Me.tbDebugBase, 0)
        Me.Controls.SetChildIndex(Me.cbPlant, 0)
        Me.Controls.SetChildIndex(Me.cbBuilding, 0)
        Me.Controls.SetChildIndex(Me.Label1, 0)
        Me.Controls.SetChildIndex(Me.Label2, 0)
        Me.Controls.SetChildIndex(Me.Label3, 0)
        Me.Controls.SetChildIndex(Me.tbFloor, 0)
        Me.Controls.SetChildIndex(Me.Panel1, 0)
        Me.Controls.SetChildIndex(Me.btnClear, 0)
        Me.Controls.SetChildIndex(Me.btnGetEmp, 0)
        Me.Controls.SetChildIndex(Me.btnEquipment, 0)
        Me.Controls.SetChildIndex(Me.btnWo, 0)
        Me.Controls.SetChildIndex(Me.tbWoNumber, 0)
        Me.Controls.SetChildIndex(Me.tbEquipment, 0)
        Me.Controls.SetChildIndex(Me.tbEmpno, 0)
        Me.Controls.SetChildIndex(Me.Label4, 0)
        Me.Controls.SetChildIndex(Me.tbToolingQR, 0)
        Me.Controls.SetChildIndex(Me.btnClear4, 0)
        Me.Controls.SetChildIndex(Me.btnClear1, 0)
        Me.Controls.SetChildIndex(Me.btnClear2, 0)
        Me.Controls.SetChildIndex(Me.btnClear3, 0)
        Me.Controls.SetChildIndex(Me.btnSave, 0)
        Me.Controls.SetChildIndex(Me.cbDutykind, 0)
        Me.Controls.SetChildIndex(Me.Label5, 0)
        Me.Controls.SetChildIndex(Me.btnFAI, 0)
        Me.Controls.SetChildIndex(Me.tbFAIcode, 0)
        Me.Controls.SetChildIndex(Me.lbFAIDesc, 0)
        Me.Controls.SetChildIndex(Me.lbAssembly, 0)
        Me.Controls.SetChildIndex(Me.lbEmpName, 0)
        Me.Controls.SetChildIndex(Me.btnClear5, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbPlant As ComboBox
    Friend WithEvents cbBuilding As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents tbFloor As TextBox
    Friend WithEvents btnClear As Button
    Friend WithEvents btnGetEmp As Button
    Friend WithEvents btnEquipment As Button
    Friend WithEvents btnWo As Button
    Friend WithEvents tbWoNumber As TextBox
    Friend WithEvents tbEquipment As TextBox
    Friend WithEvents tbEmpno As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents tbToolingQR As TextBox
    Friend WithEvents btnClear4 As Button
    Friend WithEvents btnClear1 As Button
    Friend WithEvents btnClear2 As Button
    Friend WithEvents btnClear3 As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents cbDutykind As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents btnFAI As Button
    Friend WithEvents tbFAIcode As TextBox
    Friend WithEvents lbFAIDesc As Label
    Friend WithEvents lbAssembly As Label
    Friend WithEvents lbEmpName As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnClear5 As Button
End Class
