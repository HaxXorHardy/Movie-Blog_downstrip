Imports System.Security.Cryptography
Imports System.IO
Imports System.Text

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim rd As New RijndaelManaged

        Dim md5 As New MD5CryptoServiceProvider
        Dim key() As Byte = md5.ComputeHash(Encoding.UTF8.GetBytes(TextBox2.Text))

        md5.Clear()
        rd.Key = key
        rd.GenerateIV()

        Dim iv() As Byte = rd.IV
        Dim ms As New MemoryStream

        ms.Write(iv, 0, iv.Length)

        Dim cs As New CryptoStream(ms, rd.CreateEncryptor, CryptoStreamMode.Write)
        Dim data() As Byte = System.Text.Encoding.UTF8.GetBytes(TextBox1.Text)

        cs.Write(data, 0, data.Length)
        cs.FlushFinalBlock()

        Dim encdata() As Byte = ms.ToArray()
        TextBox3.Text = Convert.ToBase64String(encdata)
        cs.Close()
        rd.Clear()
        TextBox1.Text = ""
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim rd As New RijndaelManaged
        Dim rijndaelIvLength As Integer = 16
        Dim md5 As New MD5CryptoServiceProvider
        Dim key() As Byte = md5.ComputeHash(Encoding.UTF8.GetBytes(TextBox2.Text))

        md5.Clear()

        Dim encdata() As Byte = Convert.FromBase64String(TextBox3.Text)
        Dim ms As New MemoryStream(encdata)
        Dim iv(15) As Byte

        ms.Read(iv, 0, rijndaelIvLength)
        rd.IV = iv
        rd.Key = key

        Dim cs As New CryptoStream(ms, rd.CreateDecryptor, CryptoStreamMode.Read)

        Dim data(ms.Length - rijndaelIvLength) As Byte
        Dim i As Integer = cs.Read(data, 0, data.Length)

        TextBox1.Text = System.Text.Encoding.UTF8.GetString(data, 0, i)
        cs.Close()
        rd.Clear()
        TextBox3.Text = ""
    End Sub

End Class
