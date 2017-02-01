
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions

Module Module1

#Region "Public_Dims"                                                                               'i think here is nothing to say

    'Dim dt As DateTime = Date.Today.AddDays(-2) 'for Testing
    Dim dt As DateTime = Date.Today
    Dim month As String = dt.ToString("MM", CultureInfo.InvariantCulture)
    Dim year As String = dt.ToString("yyyy", CultureInfo.InvariantCulture)
    Dim day As String = dt.ToString("dd", CultureInfo.InvariantCulture)
    Dim pages As Integer = 0
    Const quote As String = """"                                                                    'quote(") for text maybe char(34) is better but this works also :D
    Public spiderurl = "http://www.movie-blog.org/" & year & "/" & month & "/" & day & "/"          'the mainurl from Today 
    Public list As New ArrayList

#End Region


    Sub Main()
        pFinder()                                                                                   'Find numbrs of pages from Date.Today

        Try
            Dim i As Integer = 1
            While i <= pages
                Dim aList As ArrayList = getShit(spiderurl & "page/" & i & "/")                     'Take every page and search for Links
                For Each item As String In aList
                    If Not list.Contains(item) Then                                                 'check if item is allready in list
                        If item.Contains("720p") Or item.Contains("1080p") Then                     'only choose HD movies or series
                            list.Add(item)                                                          'add the item
                        End If
                    End If
                Next
                i += 1                                                                              'Next Site
            End While

            Dim file As System.IO.StreamWriter                                                      'write the links to file(only for testing?)
            file = My.Computer.FileSystem.OpenTextFileWriter("c:\temp\MyTest.html", True)
            For Each item As String In list
                Console.WriteLine(item, Environment.NewLine)
                Dim trim As String = Replace(item, spiderurl, "")                                   'removes the main url from string (better Reading)
                file.WriteLine("<p><a href=" & quote & item & quote & ">" & trim & "</a></p>")      'for html Sytax in File(needs mor testing)
            Next
            file.Close()                                                                            'close the Streamwriter

        Catch ex As Exception
        End Try
        Console.ReadLine()                                                                          'let the Consolewindow stay open
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
