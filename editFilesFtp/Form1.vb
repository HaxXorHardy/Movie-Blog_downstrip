Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml

Public Class Form1

#Region "Dims"
    Dim nl As String = Environment.NewLine
    Private WithEvents ftpWebclient As New WebClient

#End Region

    Private Sub Form1_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Dim settings As New ArrayList
        settings = settingHelper()
        Dim getList As New ArrayList
        getList = getFile(settings)
        fillBox(getList)
        TextBox1.Text = Nothing
        statLabel.Text = "All loaded!"
    End Sub

    Private Sub fillBox(ByVal list As ArrayList)
        For i As Integer = 0 To list.Count - 1
            Dim sHelp As String = list(i)
            If sHelp.Contains("#") Then
                sHelp = sHelp.Replace("#", "")
                CheckedListBox1.Items.Insert(i, sHelp)
                CheckedListBox1.SetItemChecked(i, True)
            Else
                CheckedListBox1.Items.Insert(i, sHelp)
                CheckedListBox1.SetItemChecked(i, False)
            End If
        Next
    End Sub

    Private Function getFile(ByVal settings As ArrayList)

        Dim aReturn As New ArrayList
        Try
            Dim pathFile As String = My.Application.Info.DirectoryPath & "\" & settings(0)
            File.Delete(pathFile)

            Dim myUri As New Uri(settings(1) & settings(0))
            ftpWebclient.Credentials = New System.Net.NetworkCredential(settings(2), settings(3))
            ftpWebclient.DownloadFile(myUri, pathFile)

            'now read the file and fill aReturn
            Dim txtReader As New StreamReader(pathFile)
            Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
            Dim sLine As String = Nothing
            Do
                sLine = txtReader.ReadLine()
                If Not sLine Is Nothing Then
                    sLine = ti.ToTitleCase(sLine.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss").Replace("-", " "))
                    aReturn.Add(sLine)
                End If
            Loop Until sLine Is Nothing
            txtReader.Close()
            Return aReturn
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
        Return aReturn
    End Function

    Private Function settingHelper()            'read the setting.xml file
        Dim aReturn As New ArrayList
        Try
            Dim settingPath = My.Application.Info.DirectoryPath & "\settings.xml"   'path to file
            Dim settings As New XmlDocument()
            Dim nodelist As XmlNodeList
            Dim node As XmlElement
            settings.Load(settingPath)                                      'load the file
            nodelist = settings.SelectNodes("/settings/options")            'Xml-tree
            For Each node In nodelist
                aReturn.Add(node.Attributes.GetNamedItem("settingFile").Value.ToString)
                aReturn.Add(deCrypt(node.SelectSingleNode("ftpserver").InnerText))
                aReturn.Add(deCrypt(node.SelectSingleNode("username").InnerText))
                aReturn.Add(deCrypt(node.SelectSingleNode("password").InnerText))
                Debug.WriteLine("filename: " & aReturn(0) & nl & "Ftpserver: " & aReturn(1) & nl & "Username: " & aReturn(2) & nl & "Password: " & aReturn(3), nl)
            Next
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
        Return aReturn
    End Function

    Private Sub buttonAdd_Click(sender As Object, e As EventArgs) Handles buttonAdd.Click
        Try
            If Not TextBox1.Text = Nothing Then
                CheckedListBox1.Items.Insert(CheckedListBox1.Items.Count, TextBox1.Text)
                CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, True)
                TextBox1.Text = Nothing
                statLabel.Text = "Item added!"
            Else
                statLabel.Text = "Empty field!"
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
    End Sub

    Private Sub buttonRemove_Click(sender As Object, e As EventArgs) Handles buttonRemove.Click
        Try
            For i = CheckedListBox1.Items.Count - 1 To 0 Step -1
                If Not CheckedListBox1.GetItemChecked(i) Then
                    CheckedListBox1.Items.RemoveAt(i)
                    statLabel.Text = "Item(s) removed!"
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
    End Sub

    Private Function getItems()
        Dim aReturn As New ArrayList
        Try
            For i As Integer = 0 To (CheckedListBox1.Items.Count - 1)
                If CheckedListBox1.GetItemChecked(i) Then
                    aReturn.Add(CheckedListBox1.Items(i).ToString & "#")
                Else
                    aReturn.Add(CheckedListBox1.Items(i).ToString)
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
        Return aReturn
    End Function

    Private Sub writeList(ByVal list As ArrayList, ByVal settings As ArrayList)
        Try
            Dim pathFile As String = My.Application.Info.DirectoryPath & "\" & settings(0)
            Dim txtWriter As New StreamWriter(pathFile)
            For Each item As String In list
                txtWriter.WriteLine(item)
            Next
            txtWriter.Close()
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
    End Sub

    Private Function setFile(ByVal settings As ArrayList)
        Dim aReturn As Boolean = False
        Try
            Dim pathFile As String = My.Application.Info.DirectoryPath & "\" & settings(0)
            Dim myUri As New Uri(settings(1) & settings(0))
            ftpWebclient.Credentials = New System.Net.NetworkCredential(settings(2), settings(3))
            ftpWebclient.UploadFileAsync(myUri, pathFile)
            aReturn = True
        Catch ex As Exception
            Debug.WriteLine(ex.ToString(), nl)
        End Try
        Return aReturn
    End Function

    Private Function deCrypt(ByVal pw As String)
        Dim aReturn As String
        Dim cr As String = "Ö;EY,/rwÖZrG{rvx$f.Ät2br5Ü,VER"
        Dim rd As New RijndaelManaged
        Dim rijndaelIvLength As Integer = 16
        Dim md5 As New MD5CryptoServiceProvider
        Dim key() As Byte = md5.ComputeHash(Encoding.UTF8.GetBytes(cr))
        md5.Clear()
        Dim encdata() As Byte = Convert.FromBase64String(pw)
        Dim ms As New MemoryStream(encdata)
        Dim iv(15) As Byte
        ms.Read(iv, 0, rijndaelIvLength)
        rd.IV = iv
        rd.Key = key
        Dim cs As New CryptoStream(ms, rd.CreateDecryptor, CryptoStreamMode.Read)
        Dim data(ms.Length - rijndaelIvLength) As Byte
        Dim i As Integer = cs.Read(data, 0, data.Length)
        aReturn = System.Text.Encoding.UTF8.GetString(data, 0, i)
        cs.Close()
        rd.Clear()
        Return aReturn
    End Function

    Private Sub ftpWebclient_DownloadFileCompleted(sender As Object, e As AsyncCompletedEventArgs) Handles ftpWebclient.DownloadFileCompleted
        If e.Error IsNot Nothing Then
            MessageBox.Show(e.Error.Message)
        End If
    End Sub

    Private Sub ftpWebclient_UploadFileCompleted(sender As Object, e As System.Net.UploadFileCompletedEventArgs) Handles ftpWebclient.UploadFileCompleted
        If e.Error IsNot Nothing Then
            MessageBox.Show(e.Error.Message)
        End If
    End Sub

    Private Sub ftpWebclient_DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs) Handles ftpWebclient.DownloadProgressChanged
        progBar.Value = e.ProgressPercentage
    End Sub

    Private Sub ftpWebclient_UploadProgressChanged(sender As Object, e As System.Net.UploadProgressChangedEventArgs) Handles ftpWebclient.UploadProgressChanged
        progBar.Value = e.ProgressPercentage
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim settings As New ArrayList
        settings = settingHelper()
        Dim saveItems As New ArrayList
        saveItems = getItems()
        writeList(saveItems, settings)
        statLabel.Text = "All done! Seeya!"
        If setFile(settings) Then
            MessageBox.Show("Uploading Finished!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Somthing went wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

End Class
