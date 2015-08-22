Public Class Form1
    Private textChange As Boolean = False
    Private SaveText As String
    Private currentpath As String = System.IO.Directory.GetCurrentDirectory
    Private startpath = My.Application.Info.DirectoryPath
    Private savepath As String
    Private openpath As String = "Untitled"
    Private savetexts As String
    Private marginLeft As Integer = 200
    Private marginRight As Integer = 200
    Private marginTop As Integer = 250
    Private marginbottom As Integer = 200
    Private nochange As Boolean = False
    Private encode As System.Text.Encoding = System.Text.Encoding.UTF8
    Private waitcode As String = "getchar(); }" & vbCrLf
    Friend searchList As New ArrayList
    Friend replaceList As New ArrayList
    Private searchpath As Integer = 0
    Private nextIndex As Integer = 0
    Private cols As Integer
    Private rows As Integer
    Private printer As NSSPRNXA.prnx

    Private Sub build(Optional ByVal buildpath As String = "")
        TextSave(buildpath, True)
        'Processオブジェクトを作成
        Dim p As New System.Diagnostics.Process()
        Dim results As String = String.Empty
        Dim resuluterr As String = String.Empty
        'ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
        p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec")
        '出力を読み取れるようにする
        p.StartInfo.UseShellExecute = False
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.RedirectStandardInput = False
        p.StartInfo.RedirectStandardError = True
        'ウィンドウを表示しないようにする
        p.StartInfo.CreateNoWindow = True
        'コマンドラインを指定（"/c"は実行後閉じるために必要）
        p.StartInfo.Arguments = "/c gcc " & buildpath
        '起動
        p.Start()

        '出力を読み取る
        results += p.StandardOutput.ReadToEnd()
        resuluterr += p.StandardError.ReadToEnd
        If resuluterr = String.Empty Then
            ToolStripStatusLabel1.Text = "コンパイルは成功しました"
        Else
            ToolStripStatusLabel1.Text = "コンパイルに失敗しました"
        End If
        Me.Refresh()
        'プロセス終了まで待機する
        'WaitForExitはReadToEndの後である必要がある
        '(親プロセス、子プロセスでブロック防止のため)
        p.WaitForExit()

        p.Close()
        '出力された結果を表示
        Console.Text = results & vbCrLf & resuluterr
        If resuluterr = String.Empty Then
            System.IO.File.Copy(startpath & "\Resources\run.bat", "run.bat", True)
            Dim p2 As System.Diagnostics.Process = System.Diagnostics.Process.Start("run.bat")
            p2.WaitForExit()
            p2.Close()
        End If
        System.IO.File.Delete("run.bat")
        System.IO.File.Delete(buildpath)
    End Sub

    Private Sub Editor_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Editor.TextChanged
        If nochange = False Then
            textChange = True
            Me.Text = System.IO.Path.GetFileName(openpath) & " * - CProgrammerLite"
        Else
            nochange = False
        End If

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "Untitled - C ProgrammerLite"
        openpath = currentpath & "\Untitled.c"
        Me.ActiveControl = Me.Editor
        ToolStripStatusLabel2.Text = ("行：" & countrows())
    End Sub


    Private Sub cut()
        If Editor.SelectionLength > 0 Then
            Editor.Cut()
        End If
    End Sub
    Private Sub copy()
        If Editor.SelectionLength > 0 Then
            Editor.Copy()
        End If
    End Sub
    Private Sub paste()
        If Clipboard.GetDataObject(). _
            GetDataPresent(DataFormats.Text) = True Then
            Editor.Paste()
        End If
    End Sub
    Private Sub del()
        If Editor.SelectionLength > 0 Then
            Editor.SelectedText = ""
        End If
    End Sub
    Private Sub allselect()
        Editor.Focus()
        Editor.SelectAll()
    End Sub
    Private Sub undo()
        Editor.Focus()
        Editor.Undo()
    End Sub

    Private Sub redo()
        Editor.Focus()
        Editor.Redo()
    End Sub

    Private Sub ToolStripMenuItem2_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.DropDownOpening, ContextMenuStrip1.Opening
        If sender.name = "ContextMenuStrip1" Then
            If Editor.SelectionLength > 0 Then
                Cut2ToolStripMenuItem.Enabled = True
                Copy2ToolStripMenuItem.Enabled = True
                Delete2ToolStripMenuItem.Enabled = True

            Else
                Cut2ToolStripMenuItem.Enabled = False
                Copy2ToolStripMenuItem.Enabled = False
                Delete2ToolStripMenuItem.Enabled = False
            End If
        Else
            If Editor.SelectionLength > 0 Then
                cutToolStripMenuItem.Enabled = True
                CopyToolStripMenuItem.Enabled = True
                delToolStripMenuItem.Enabled = True

            Else
                cutToolStripMenuItem.Enabled = False
                CopyToolStripMenuItem.Enabled = False
                delToolStripMenuItem.Enabled = False
            End If
        End If

    End Sub

    Private Sub openfile()
        Dim openfiledialog As New OpenFileDialog
        openfiledialog.Filter = ("Cソースコード|*.c|テキストファイル|*.txt|すべてのファイル|*.*")
        openfiledialog.AddExtension = True
        If DialogResult.OK = openfiledialog.ShowDialog() Then
            Readfile(openfiledialog.FileName)
        End If
    End Sub

    Private Sub Readfile(ByVal filepath As String)
        nochange = True
        encode = GetEncoding(filepath)
        Dim sr As New System.IO.StreamReader(filepath, encode)
        openpath = filepath
        Dim s As String = sr.ReadToEnd()
        currentpath = System.IO.Path.GetDirectoryName(filepath)
        System.IO.Directory.SetCurrentDirectory(currentpath)
        savepath = filepath
        savetexts = s
        Me.Editor.Text = s
        Me.Text = System.IO.Path.GetFileName(filepath) & " - CProgrammerLite"
        textChange = False
        sr.Close()

        ToolStripStatusLabel1.Text = System.IO.Path.GetFileName(filepath) & "を、開きました"
    End Sub

    Private Sub TextSave(ByVal value As String, Optional ByVal complies As Boolean = False)
        Dim savedtext As String = Editor.Text
        If value = String.Empty Then
            value = openpath
        End If
        System.IO.File.WriteAllText(value, savedtext, encode)
        If complies = True Then
            savedtext = savedtext.Replace("}", waitcode)
            System.IO.File.WriteAllText(value, savedtext, encode)
            savedtext = Editor.Text
        Else
            textChange = False
            Me.Text = System.IO.Path.GetFileName(openpath) & " - CProgrammerLite"
            ToolStripStatusLabel1.Text = System.IO.Path.GetFileName(openpath) & "を、保存しました"
        End If
        
    End Sub
    Private Sub TextSaveAs()
        Dim savefiledialog1 As New SaveFileDialog
        savefiledialog1.FileName = System.IO.Path.GetFileName(SaveText)
        savefiledialog1.Filter = ("Cソースコード|*.c|テキストファイル|*.txt|すべてのファイル|*.*")
        If DialogResult.OK = savefiledialog1.ShowDialog() Then
            SaveFile(savefiledialog1.FileName)
            Me.Text = savefiledialog1.FileName & " - CProgrammerLite"
            textChange = False
            Me.Editor.Text = savetexts
            currentpath = System.IO.Path.GetDirectoryName(savefiledialog1.FileName)
            ToolStripStatusLabel1.Text = System.IO.Path.GetFileName(openpath) & "を、保存しました"
        End If
    End Sub
    Private Sub SaveFile(ByVal value As String)
        System.IO.File.WriteAllText(value, Editor.Text, encode)
    End Sub

    Private Sub UndoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UndoToolStripMenuItem.Click
        undo()
    End Sub

    Private Sub RedoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RedoToolStripMenuItem.Click
        redo()
    End Sub

    Private Sub delToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles delToolStripMenuItem.Click
        del()
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteToolStripMenuItem.Click
        paste()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyToolStripMenuItem.Click
        copy()
    End Sub

    Private Sub cutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cutToolStripMenuItem.Click
        cut()
    End Sub

    Private Sub ReplaceToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplaceToolStripMenuItem.Click
        Dim replacedialog As New Dialog2
        replacedialog.Show()
    End Sub

    Private Sub SearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchToolStripMenuItem.Click
        Dim searchdialog As New Dialog1
        searchdialog.Show()
    End Sub

    Private Sub AllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllToolStripMenuItem.Click
        allselect()
    End Sub

    Private Sub CompileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CompileToolStripMenuItem.Click
        build(startpath & "\temp.c")
    End Sub

    Private Sub NEWToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NEWToolStripMenuItem.Click
        Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location)
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        openfile()
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        TextSave(savepath)
    End Sub

    Private Sub SaveasToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveasToolStripMenuItem.Click
        TextSaveAs()
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If textChange = True Then
            If MessageBox.Show(System.IO.Path.GetFileName(openpath) & "は保存されていません" & vbCrLf & "終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub PagesettingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PagesettingToolStripMenuItem.Click
        PageSetupDialog1.PageSettings = New System.Drawing.Printing.PageSettings()
        PageSetupDialog1.PrinterSettings = New System.Drawing.Printing.PrinterSettings()

        ' 余白の最小値を設定する
        PageSetupDialog1.MinMargins = New System.Drawing.Printing.Margins(marginLeft, marginRight, marginTop, marginbottom)
        If PageSetupDialog1.ShowDialog() = DialogResult.OK Then

        End If

        ' 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
        PageSetupDialog1.Dispose()
    End Sub

    Private Sub PrintDocument1_PrintPage(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        e.HasMorePages = printer.PrintPage(e)
    End Sub

    Private Sub PrintToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintToolStripMenuItem.Click
        Dim ppd As PrintPreviewDialog = New PrintPreviewDialog
        ppd.Document = PrintDocument1
        printer = New NSSPRNXA.prnx()
        printer.Init(PrintDocument1)
        PrintDocument1.DefaultPageSettings.Landscape = False
        Dim aFont As Font = Editor.Font
        printer.SetPrintFont(aFont)
        printer.AddPageEnd()
        printer.BeginPrint()
        ppd.WindowState = FormWindowState.Maximized
        ppd.ShowDialog()
    End Sub

    Private Sub ToolStripMenuItem13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem13.Click
        Dim fdialog As New FontDialog
        fdialog.Font = Editor.Font
        fdialog.FontMustExist = True
        fdialog.ShowEffects = False
        If fdialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            nochange = True
            Editor.Font = fdialog.Font

        End If
    End Sub

    Private Sub ToolStripMenuItem14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Console.Text = String.Empty
    End Sub

    Private Sub Undo2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Undo2ToolStripMenuItem.Click
        undo()
    End Sub

    Private Sub allselectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles allselectToolStripMenuItem.Click
        allselect()
    End Sub

    Private Sub Replace2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Replace2ToolStripMenuItem.Click

    End Sub

    Private Sub Search2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Search2ToolStripMenuItem.Click

    End Sub

    Private Sub Delete2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete2ToolStripMenuItem.Click
        del()
    End Sub

    Private Sub paste2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles paste2ToolStripMenuItem.Click
        paste()
    End Sub

    Private Sub Copy2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Copy2ToolStripMenuItem.Click
        copy()
    End Sub

    Private Sub Cut2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cut2ToolStripMenuItem.Click
        cut()
    End Sub

    Private Sub Redo2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Redo2ToolStripMenuItem.Click
        redo()
    End Sub

    Function GetEncoding(ByVal path As String) As System.Text.Encoding
        Dim fs As New System.IO.FileStream(path, _
       System.IO.FileMode.Open, System.IO.FileAccess.Read)
        Dim bs(fs.Length - 1) As Byte
        'byte配列に読み込む
        fs.Read(bs, 0, bs.Length)
        fs.Close()
        Dim enc As System.Text.Encoding = GetCode(bs)
        Return enc
    End Function

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Dim about As New AboutBox1
        about.ShowDialog()
    End Sub

    Friend Sub search(ByVal searchword As String, Optional ByVal replace As Boolean = False, Optional ByVal replaceword As String = Nothing)
        searchpath = Editor.Text.IndexOf(searchword, 0)
        If searchpath >= 0 Then
            If replace = False Then
                Editor.Select(searchpath, searchword.Length)
            Else
                Editor.Select(searchpath, searchword.Length)
                If Dialog2.CheckBox1.CheckState = CheckState.Checked Then
                    If DialogResult.Yes = MessageBox.Show("置換しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) Then
                        Editor.SelectedText = replaceword
                    End If
                Else
                    Editor.SelectedText = replaceword
                End If

            End If
        Else
            ToolStripStatusLabel1.Text = """" & searchword & """" & "は見つかりませんでした"
            Beep()
        End If
    End Sub

    Friend Sub nextsearch(ByVal searchword As String, Optional ByVal replace As Boolean = False)
        searchpath = Editor.Text.IndexOf(searchword, nextIndex)
        If searchpath >= 0 Then
            Editor.Select(searchpath, searchword.Length)
            nextIndex = searchpath + searchword.Length
        Else
            ToolStripStatusLabel1.Text = """" & searchword & """" & "は見つかりませんでした"
            Beep()
            nextIndex = 0
        End If

    End Sub

    Private Sub ToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem5.Click
        nextsearch(searchList(searchList.Count - 1))
    End Sub

    Private Sub SourceEditor_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Editor.SelectionChanged
        ToolStripStatusLabel2.Text = ("行：" & countrows())
        ToolStripStatusLabel1.Text = String.Empty
    End Sub

    Public Function countrows()
        Dim str As String = Editor.Text

        Dim selectPos As Integer = Editor.SelectionStart


        Dim row As Integer = 1
        Dim startPos As Integer = 0
        Dim endPos As Integer
        While (True)
            endPos = str.IndexOf(vbLf, startPos)
            If (endPos < 0 Or endPos >= selectPos) Then
                Exit While
            End If
            startPos = endPos + 1
            row += 1
        End While

        Dim col As Integer = selectPos - startPos + 1
        cols = col
        rows = row

        Return row
    End Function


    Public Shared Function GetCode(ByVal bytes As Byte()) As System.Text.Encoding
        Const bEscape As Byte = &H1B
        Const bAt As Byte = &H40
        Const bDollar As Byte = &H24
        Const bAnd As Byte = &H26
        Const bOpen As Byte = &H28 ''('
        Const bB As Byte = &H42
        Const bD As Byte = &H44
        Const bJ As Byte = &H4A
        Const bI As Byte = &H49

        Dim len As Integer = bytes.Length
        Dim b1 As Byte, b2 As Byte, b3 As Byte, b4 As Byte

        'Encode::is_utf8 は無視

        Dim isBinary As Boolean = False
        Dim i As Integer
        For i = 0 To len - 1
            b1 = bytes(i)
            If b1 <= &H6 OrElse b1 = &H7F OrElse b1 = &HFF Then
                ''binary'
                isBinary = True
                If b1 = &H0 AndAlso i < len - 1 AndAlso bytes(i + 1) <= &H7F Then
                    'smells like raw unicode
                    Return System.Text.Encoding.Unicode
                End If
            End If
        Next
        If isBinary Then
            Return Nothing
        End If

        'not Japanese
        Dim notJapanese As Boolean = True
        For i = 0 To len - 1
            b1 = bytes(i)
            If b1 = bEscape OrElse &H80 <= b1 Then
                notJapanese = False
                Exit For
            End If
        Next
        If notJapanese Then
            Return System.Text.Encoding.ASCII
        End If

        For i = 0 To len - 3
            b1 = bytes(i)
            b2 = bytes(i + 1)
            b3 = bytes(i + 2)

            If b1 = bEscape Then
                If b2 = bDollar AndAlso b3 = bAt Then
                    'JIS_0208 1978
                    'JIS
                    Return System.Text.Encoding.GetEncoding(50220)
                ElseIf b2 = bDollar AndAlso b3 = bB Then
                    'JIS_0208 1983
                    'JIS
                    Return System.Text.Encoding.GetEncoding(50220)
                ElseIf b2 = bOpen AndAlso (b3 = bB OrElse b3 = bJ) Then
                    'JIS_ASC
                    'JIS
                    Return System.Text.Encoding.GetEncoding(50220)
                ElseIf b2 = bOpen AndAlso b3 = bI Then
                    'JIS_KANA
                    'JIS
                    Return System.Text.Encoding.GetEncoding(50220)
                End If
                If i < len - 3 Then
                    b4 = bytes(i + 3)
                    If b2 = bDollar AndAlso b3 = bOpen AndAlso b4 = bD Then
                        'JIS_0212
                        'JIS
                        Return System.Text.Encoding.GetEncoding(50220)
                    End If
                    If i < len - 5 AndAlso _
                        b2 = bAnd AndAlso b3 = bAt AndAlso b4 = bEscape AndAlso _
                        bytes(i + 4) = bDollar AndAlso bytes(i + 5) = bB Then
                        'JIS_0208 1990
                        'JIS
                        Return System.Text.Encoding.GetEncoding(50220)
                    End If
                End If
            End If
        Next

        'should be euc|sjis|utf8
        'use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
        Dim sjis As Integer = 0
        Dim euc As Integer = 0
        Dim utf8 As Integer = 0
        For i = 0 To len - 2
            b1 = bytes(i)
            b2 = bytes(i + 1)
            If ((&H81 <= b1 AndAlso b1 <= &H9F) OrElse _
                (&HE0 <= b1 AndAlso b1 <= &HFC)) AndAlso _
                ((&H40 <= b2 AndAlso b2 <= &H7E) OrElse _
                 (&H80 <= b2 AndAlso b2 <= &HFC)) Then
                'SJIS_C
                sjis += 2
                i += 1
            End If
        Next
        For i = 0 To len - 2
            b1 = bytes(i)
            b2 = bytes(i + 1)
            If ((&HA1 <= b1 AndAlso b1 <= &HFE) AndAlso _
                (&HA1 <= b2 AndAlso b2 <= &HFE)) OrElse _
                (b1 = &H8E AndAlso (&HA1 <= b2 AndAlso b2 <= &HDF)) Then
                'EUC_C
                'EUC_KANA
                euc += 2
                i += 1
            ElseIf i < len - 2 Then
                b3 = bytes(i + 2)
                If b1 = &H8F AndAlso (&HA1 <= b2 AndAlso b2 <= &HFE) AndAlso _
                    (&HA1 <= b3 AndAlso b3 <= &HFE) Then
                    'EUC_0212
                    euc += 3
                    i += 2
                End If
            End If
        Next
        For i = 0 To len - 2
            b1 = bytes(i)
            b2 = bytes(i + 1)
            If (&HC0 <= b1 AndAlso b1 <= &HDF) AndAlso _
                (&H80 <= b2 AndAlso b2 <= &HBF) Then
                'UTF8
                utf8 += 2
                i += 1
            ElseIf i < len - 2 Then
                b3 = bytes(i + 2)
                If (&HE0 <= b1 AndAlso b1 <= &HEF) AndAlso _
                    (&H80 <= b2 AndAlso b2 <= &HBF) AndAlso _
                    (&H80 <= b3 AndAlso b3 <= &HBF) Then
                    'UTF8
                    utf8 += 3
                    i += 2
                End If
            End If
        Next
        'M. Takahashi's suggestion
        'utf8 += utf8 / 2;

        System.Diagnostics.Debug.WriteLine( _
            String.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8))
        If euc > sjis AndAlso euc > utf8 Then
            'EUC
            Return System.Text.Encoding.GetEncoding(51932)
        ElseIf sjis > euc AndAlso sjis > utf8 Then
            'SJIS
            Return System.Text.Encoding.GetEncoding(932)
        ElseIf utf8 > euc AndAlso utf8 > sjis Then
            'UTF8
            Return System.Text.Encoding.UTF8
        End If

        Return Nothing
    End Function




End Class
