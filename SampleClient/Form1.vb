Imports System.Net.Sockets
Imports System.IO

Public Class Form1
    Private stream As NetworkStream
    Private streamw As StreamWriter
    Private streamr As StreamReader
    Private client As New TcpClient
    Private t As New Threading.Thread(AddressOf Listen)
    Private Delegate Sub DAddItem(ByVal s As String)
    Private nick As String = "unknown"

    Private Sub AddItem(ByVal s As String)
        ListBox1.Items.Add(s)
    End Sub

    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            client.Connect("127.0.0.1", 8000) ' hier die ip des servers eintragen. 
            ' da dieser beim testen wohl lokal läuft, hier die loopback-ip 127.0.0.1.
            If client.Connected Then
                stream = client.GetStream
                streamw = New StreamWriter(stream)
                streamr = New StreamReader(stream)

                streamw.WriteLine(nick) ' das ist optional.
                streamw.Flush()

                t.Start()
            Else
                MessageBox.Show("Verbindung zum Server nicht möglich!")
                Application.Exit()
            End If
        Catch ex As Exception
            MessageBox.Show("Verbindung zum Server nicht möglich!")
            Application.Exit()
        End Try
    End Sub

    Private Sub Listen()
        While client.Connected
            Try
                Me.Invoke(New DAddItem(AddressOf AddItem), streamr.ReadLine)
            Catch
                MessageBox.Show("Verbindung zum Server nicht möglich!")
                Application.Exit()
            End Try
        End While
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        streamw.WriteLine(TextBox1.Text)
        streamw.Flush()
        TextBox1.Clear()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        nick = InputBox("Nickname: ", "Namen festlegen", "unknown")
    End Sub
End Class
