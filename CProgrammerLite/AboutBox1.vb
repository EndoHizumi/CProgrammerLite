Public NotInheritable Class AboutBox1

    Private version As String = My.Application.Info.Version.ToString
    Private minversionI As Integer = My.Application.Info.Version.Minor
    Private MajversionI As Integer = My.Application.Info.Version.Major
    Private DelevepCode As String() = {"α", "β", "Relece Preview"}
    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' フォームのタイトルを設定します。
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("バージョン情報 {0}", ApplicationTitle)
        ' バージョン情報ボックスに表示されたテキストをすべて初期化します。
        ' TODO: [プロジェクト] メニューの下にある [プロジェクト プロパティ] ダイアログの [アプリケーション] ペインで、アプリケーションのアセンブリ情報を 
        '    カスタマイズします。
        If minversionI <= 3 Then
            version = DelevepCode(0) & "  ( " & version & " )"
        ElseIf 7 >= minversionI >= 4 Then
            version = DelevepCode(1) & "  ( " & version & " )"
        ElseIf minversionI >= 8 Then
            version = DelevepCode(2) & " ( " & version & " )"
        Else
            version = version
        End If
#If DEBUG Then
        version += " DEBUG"
#End If
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("バージョン {0}", version)
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = My.Application.Info.Description
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

End Class
