
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions

Module Module1

#Region "Public_Dims"
    Dim dt As DateTime = Date.Today.AddDays(-2)
    Dim month As String = dt.ToString("MM", CultureInfo.InvariantCulture)
    Dim year As String = dt.ToString("yyyy", CultureInfo.InvariantCulture)
    Dim day As String = dt.ToString("dd", CultureInfo.InvariantCulture)
    Dim pages As Integer = 0
    Const quote As String = """"

    Public spiderurl = "http://www.movie-blog.org/" & year & "/" & month & "/" & day & "/"
    Public list As New ArrayList


#End Region


    Sub Main()

        Console.WriteLine(spiderurl, Environment.NewLine)
        pFinder()

        Try
            Dim i As Integer = 1
            While i <= pages
                Dim aList As ArrayList = getShit(spiderurl & "page/" & i & "/")
                For Each item As String In aList
                    If Not list.Contains(item) Then
                        If item.Contains("720p") Or item.Contains("1080p") Then
                            list.Add(item)
                        End If
                    End If
                Next
                i += 1
            End While



            Dim file As System.IO.StreamWriter
            file = My.Computer.FileSystem.OpenTextFileWriter("c:\temp\MyTest.html", True)
            For Each item As String In list
                Console.WriteLine(item, Environment.NewLine)
                Dim trime As String = Replace(item, spiderurl, "")
                file.WriteLine("<p><a href=" & quote & item & quote & ">" & trime & "</a></p>")
            Next
            file.Close()




        Catch ex As Exception
        End Try
        Console.ReadLine()
    End Sub

    Private Sub pFinder()
        Try
            ''''<span class="pages">Seite 2 von 11</span>
            Dim strRegex As String = "<span.*?Seite 1 von (.*?)<\/span>"
            Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(spiderurl & "page/1/")
            Dim myWebResponse = CType(Request.GetResponse(), HttpWebResponse)
            Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())
            Dim strSource = myStreamReader.ReadToEnd
            Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
            Dim HrefMatch As Match = HrefRegex.Match(strSource)
            While HrefMatch.Success = True
                Dim num As String = HrefMatch.Groups(1).Value
                Console.WriteLine("Pages: " & num, Environment.NewLine)
                pages = Convert.ToInt32(num)
                HrefMatch = HrefMatch.NextMatch
            End While
        Catch ex As Exception
        End Try
    End Sub
    Private Function getShit(ByVal url As String) As ArrayList

        Dim aReturn As New ArrayList
        Try
            Dim strRegex As String = "<a.*?href=""(.*?)"".*?>(.*?)</a>"
            Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)
            Dim myWebResponse = CType(Request.GetResponse(), HttpWebResponse)
            Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())
            Dim strSource = myStreamReader.ReadToEnd
            Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)
            Dim HrefMatch As Match = HrefRegex.Match(strSource)
            While HrefMatch.Success = True
                Dim pUrl As String = HrefMatch.Groups(1).Value
                If pUrl.Contains(spiderurl) AndAlso Not pUrl.Contains("page") AndAlso Not pUrl.Contains("#") Then aReturn.Add(pUrl)
                HrefMatch = HrefMatch.NextMatch
            End While
        Catch ex As Exception
        End Try
        Return aReturn
    End Function





End Module
