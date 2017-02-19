Imports System.Net.Sockets
Imports System.IO
Imports System.Net
Imports MySql.Data.MySqlClient


Module Module1

#Region "Dims"
    Private server As TcpListener
    Private client As New TcpClient
    Private ipendpoint As IPEndPoint = New IPEndPoint(IPAddress.Any, 8000) ' eingestellt ist port 8000. dieser muss ggf. freigegeben sein!
    Private list As New List(Of Connection)
#End Region

    Private Structure Connection
        Dim stream As NetworkStream
        Dim streamw As StreamWriter
        Dim streamr As StreamReader
        Dim name As String ' natürlich optional, aber für die identifikation des clients empfehlenswert.
    End Structure

    Sub Main()
        Console.WriteLine("Der Server läuft!")
        server = New TcpListener(ipendpoint)
        server.Start()
        While True ' wir warten auf eine neue verbindung...
            client = server.AcceptTcpClient
            Dim con As New Connection ' und erstellen für die neue verbindung eine neue connection...
            con.stream = client.GetStream
            con.streamr = New StreamReader(con.stream)
            con.streamw = New StreamWriter(con.stream)
            con.name = con.streamr.ReadLine ' falls das mit dem nick nicht gewünscht, auch diese zeile entfernen.
            list.Add(con) ' und fügen sie der liste der clients hinzu.
            Console.WriteLine(con.name & " has joined.")
            ' falls alle anderen das auch lesen sollen können, an alle clients weiterleiten.
            Dim t As New Threading.Thread(AddressOf ListenToConnection)
            t.Start(con)
        End While
    End Sub

    Private Sub ListenToConnection(ByVal con As Connection)
        Do
            Try
                Dim tmp As String = con.streamr.ReadLine ' warten, bis etwas empfangen wird...
                Console.WriteLine(con.name & ": " & tmp)
                'For Each c As Connection In list ' an alle clients weitersenden.
                'Try
                'c.streamw.WriteLine(con.nick & ": " & tmp)
                'c.streamw.Flush()
                'Catch
                'End Try
                'Next
            Catch ' die aktuelle überwachte verbindung hat sich wohl verabschiedet.
                list.Remove(con)
                Console.WriteLine(con.name & " has exit.")
                Exit Do
            End Try
        Loop
    End Sub

End Module