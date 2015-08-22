Imports System.Windows.Forms

Public Class Dialog2
    Dim searchHistory() As String
    Dim replaceHistroy() As String

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchButton.Click
        If ComboBox1.Text.Length > 0 Then
            Form1.search(ComboBox1.Text, True, ComboBox2.Text)
            ListAdd()
            Me.Close()
        Else
            Beep()
        End If

    End Sub

    Private Sub Dialog1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        searchHistory = DirectCast(Form1.searchList.ToArray(GetType(String)), String())
        ComboBox1.Items.AddRange(searchHistory)
        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
        End If
        replaceHistroy = DirectCast(Form1.replaceList.ToArray(GetType(String)), String())
        ComboBox2.Items.AddRange(replaceHistroy)
        If ComboBox2.Items.Count > 0 Then
            ComboBox2.SelectedIndex = ComboBox2.Items.Count - 1
        End If
        Me.ActiveControl = ComboBox1
        Me.CheckBox1.CheckState = CheckState.Checked
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If ComboBox1.Text.Length > 0 Then
            Form1.Editor.Text = Form1.Editor.Text.Replace(ComboBox1.Text, ComboBox2.Text)
            ListAdd()
        Else
            Beep()
        End If

    End Sub

    Private Sub ListAdd()
        If Form1.searchList.IndexOf(ComboBox1.Text) < 0 Then
            Form1.searchList.Add(ComboBox1.Text)
        End If
        If Form1.replaceList.IndexOf(ComboBox1.Text) < 0 Then
            Form1.replaceList.Add(ComboBox2.Text)
        End If
    End Sub

    Private Sub Dialog1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            OK_Button_Click(SearchButton, Nothing)
        End If
    End Sub
End Class
