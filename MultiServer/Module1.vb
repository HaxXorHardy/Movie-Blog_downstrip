Imports System.Net.Sockets
Imports System.IO
Imports System.Net

Module Module1
    Private server As TcpListener
    Private client As New TcpClient
    Private ipendpoint As IPEndPoint = New IPEndPoint(IPAddress.Any, 8000) ' eingestellt ist port 8000. dieser muss ggf. freigegeben sein!
    Private list As New List(Of Connection)

    Private Structure Connection
        Dim stream As NetworkStream
        Dim streamw As StreamWriter
        Dim streamr As StreamReader
        Dim nick As String ' natürlich optional, aber für die identifikation des clients empfehlenswert.
    End Structure

    Sub Main()
        Console.WriteLine("Der Server läuft!")
        server = New TcpListener(ipendpoint)
        server.Start()

        While True ' wir warten auf eine neue verbindung...
            client = server.AcceptTcpClient

            Dim c As New Connection ' und erstellen für die neue verbindung eine neue connection...
            c.stream = client.GetStream
            c.streamr = New StreamReader(c.stream)
            c.streamw = New StreamWriter(c.stream)

            c.nick = c.streamr.ReadLine ' falls das mit dem nick nicht gewünscht, auch diese zeile entfernen.

            list.Add(c) ' und fügen sie der liste der clients hinzu.
            Console.WriteLine(c.nick & " has joined.")
            ' falls alle anderen das auch lesen sollen können, an alle clients weiterleiten.

            Dim t As New Threading.Thread(AddressOf ListenToConnection)
            t.Start(c)
        End While
    End Sub

    Private Sub ListenToConnection(ByVal con As Connection)
        Do
            Try
                Dim tmp As String = con.streamr.ReadLine ' warten, bis etwas empfangen wird...
                Console.WriteLine(con.nick & ": " & tmp)
                For Each c As Connection In list ' an alle clients weitersenden.
                    Try
                        c.streamw.WriteLine(con.nick & ": " & tmp)
                        c.streamw.Flush()
                    Catch
                    End Try
                Next
            Catch ' die aktuelle überwachte verbindung hat sich wohl verabschiedet.
                list.Remove(con)
                Console.WriteLine(con.nick & " has exit.")
                Exit Do
            End Try
        Loop
    End Sub
End Module
