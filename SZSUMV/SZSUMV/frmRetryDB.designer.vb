<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRetryDB
    Inherits System.Windows.Forms.Form

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
        Me.components = New System.ComponentModel.Container()
        Me.lblReConnect = New System.Windows.Forms.Label()
        Me.lblConCount = New System.Windows.Forms.Label()
        Me.tmCheck = New System.Windows.Forms.Timer(Me.components)
        Me.bgwQuery = New System.ComponentModel.BackgroundWorker()
        Me.btnRetry = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.SuspendLayout()
        '
        'lblReConnect
        '
        Me.lblReConnect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblReConnect.Location = New System.Drawing.Point(3, 38)
        Me.lblReConnect.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblReConnect.Name = "lblReConnect"
        Me.lblReConnect.Size = New System.Drawing.Size(177, 14)
        Me.lblReConnect.TabIndex = 0
        Me.lblReConnect.Text = "重新連線中...."
        '
        'lblConCount
        '
        Me.lblConCount.AutoSize = True
        Me.lblConCount.Location = New System.Drawing.Point(40, 58)
        Me.lblConCount.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblConCount.Name = "lblConCount"
        Me.lblConCount.Size = New System.Drawing.Size(53, 12)
        Me.lblConCount.TabIndex = 1
        Me.lblConCount.Text = "連線次數"
        '
        'tmCheck
        '
        Me.tmCheck.Interval = 1000
        '
        'bgwQuery
        '
        Me.bgwQuery.WorkerReportsProgress = True
        '
        'btnRetry
        '
        Me.btnRetry.Location = New System.Drawing.Point(12, 120)
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.Size = New System.Drawing.Size(75, 23)
        Me.btnRetry.TabIndex = 2
        Me.btnRetry.Text = "Retry"
        Me.btnRetry.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(93, 120)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 3
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(40, 12)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(100, 23)
        Me.ProgressBar1.Step = 1
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar1.TabIndex = 4
        '
        'frmRetryDB
        '
        Me.AcceptButton = Me.btnRetry
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(182, 174)
        Me.ControlBox = False
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnRetry)
        Me.Controls.Add(Me.lblConCount)
        Me.Controls.Add(Me.lblReConnect)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmRetryDB"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "連線異常重試中"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblReConnect As Label
    Friend WithEvents lblConCount As Label
    Friend WithEvents tmCheck As Timer
    Friend WithEvents bgwQuery As System.ComponentModel.BackgroundWorker
    Friend WithEvents btnRetry As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents ProgressBar1 As ProgressBar
End Class
