
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports Google.Apis.Customsearch.v1
Imports Google.Apis.Customsearch.v1.Data

Module Module1

#Region "Public_Dims"                                                                               'i think here is nothing to say

    'Dim dt As DateTime = Date.Today.AddDays(-2) 'for Testing
    'Dim gApiK As String = "AIzaSyC0WgLkbntFiBylDoSoYwPFZZVu2PETT_U"                                'googleapi key  key=API_KEY
    'Dim sEngine As String = "000363274925813725598:6h3f2fhmhby"                                    'serchengine ID
    Dim dt As DateTime = Date.Today
    Dim month As String = dt.ToString("MM", CultureInfo.InvariantCulture)
    Dim year As String = dt.ToString("yyyy", CultureInfo.InvariantCulture)
    Dim day As String = dt.ToString("dd", CultureInfo.InvariantCulture)
    Dim pages As Integer = 0
    Const quote As String = Chr(34)                                                                 'quote(") for text 
    Public spiderurl = "http://www.movie-blog.org/" & year & "/" & month & "/" & day & "/"          'the mainurl from Today 
    Public list As New ArrayList

#End Region


    'Public Function goggleSearch(searchWord As String)              ' bullshit need to pay for more requests (Dayly limit)
    '    Dim aReturn As String = Nothing
    '    Try
    '        Dim query As String = searchWord & " Film"
    '        Dim customSearchService As New CustomsearchService(New Google.Apis.Services.BaseClientService.Initializer() With {.ApiKey = gApiK})
    '        Dim listRequest As Google.Apis.Customsearch.v1.CseResource.ListRequest = customSearchService.Cse.List(query)
    '        listRequest.Cx = sEngine
    '        Dim search As Search = listRequest.Execute()
    '        For Each item In search.Items
    '            'Console.WriteLine("Title : " + item.Title + Environment.NewLine + "Link : " + item.Link + Environment.NewLine + Environment.NewLine)
    '            Return item.Title
    '        Next
    '    Catch ex As Exception
    '        Console.WriteLine(ex.Message, Environment.NewLine)                                       'Errorhandling for user debugging
    '        Console.ReadLine()
    '    End Try
    '    Return aReturn
    'End Function

    Sub Main()

        pFinder()                                                                                    'Find numbrs of pages from Date.Today

        Try
            Dim i As Integer = 1
            While i <= pages
                Dim aList As ArrayList = getShit(spiderurl & "page/" & i & "/")                      'Take every page and search for Links
                For Each item As String In aList
                    If Not list.Contains(item) Then                                                  'check if item is allready in list
                        If item.Contains("720p") Or item.Contains("1080p") Then                      'only choose HD movies or series
                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                If Not item.Contains("s0") Or Not item.Contains("s1") Or Not item.Contains("s2") Then
                                    list.Add(item)                                                           'add the item
                                End If
                            End If
                            End If
                    End If
                Next
                i += 1                                                                               'Next Site
            End While

            'Dim file As System.IO.StreamWriter                                                      'write the links to file(only for testing?)
            'file = My.Computer.FileSystem.OpenTextFileWriter("c:\temp\MyTest.html", True)
            Dim outP As New ArrayList
            Dim x As Integer = 1
            For Each item As String In list
                Dim retStr As String = findTitle(item)
                If Not retStr = Nothing Then
                    If Not outP.Contains(retStr) Then
                        outP.Add(retStr)
                        Console.WriteLine((String.Format("{0:000}", x) & ": " & retStr), Environment.NewLine)
                        x += 1
                    End If
                End If
                'Console.WriteLine(item, Environment.NewLine)
                'Dim trim As String = Replace(item, spiderurl, "")                                   'removes the main url from string (better Reading)
                'file.WriteLine("<p><a href=" & quote & item & quote & ">" & trim & "</a></p>")      'for html Sytax in File(needs mor testing)
            Next
            'file.Close()                                                                            'close the Streamwriter

        Catch ex As Exception
            Console.WriteLine(ex.Message, Environment.NewLine)                                       'Errorhandling for user debugging
            Console.ReadLine()
        End Try
        Console.WriteLine("Ende.....")
        Console.ReadLine()                                                                           'let the Consolewindow stay open
    End Sub

    Private Sub pFinder()
        Try
            '<span class="pages">Seite 2 von 11</span>  find 11
            Dim strRegex As String = "<span.*?Seite 1 von (.*?)<\/span>"                                            'crasy regex string to find Pagenumbers 
            'regextutorial https://www.vb-paradise.de/index.php/Thread/34042-RegEx-Tutorial-Blutige-Anf%C3%A4nger-und-Fortgeschrittene/
            Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(spiderurl & "page/1/")      'new Webrequest
            Dim myWebResponse = CType(Request.GetResponse(), HttpWebResponse)                                       'shitty thing
            Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())                                'streamreader to get response from webrequest live
            Dim strSource = myStreamReader.ReadToEnd                                                                'finish reading of response
            Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                  'regex options to find the string in response
            Dim HrefMatch As Match = HrefRegex.Match(strSource)                                                     'Find matches in String
            While HrefMatch.Success = True                                                                          'String Found = True 1 or more strings matching
                Dim num As String = HrefMatch.Groups(1).Value                                                       'get first match
                Console.WriteLine("Pages: " & num, Environment.NewLine)                                             'output for testing
                pages = Convert.ToInt32(num)                                                                        'convert String to Integer
                HrefMatch = HrefMatch.NextMatch                                                                     'More matches? get them and print this shit out
            End While
        Catch ex As Exception
            Console.WriteLine(ex.Message, Environment.NewLine)                                                      'Errorhandling for user debugging
            Console.ReadLine()
        End Try
    End Sub
    Private Function getShit(ByVal url As String) As ArrayList
        Dim aReturn As New ArrayList                                                                                 'create the returnvariable
        Try
            Dim strRegex As String = "<a.*?href=""(.*?)"".*?>(.*?)</a>"                                              'crasy regex string to find Pagenumbers
            Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create(url)                         'new Webrequest
            Dim myWebResponse = CType(Request.GetResponse(), HttpWebResponse)                                        'shitty thing
            Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())                                 'streamreader to get response from webrequest live
            Dim strSource = myStreamReader.ReadToEnd                                                                 'finish reading of response
            Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                   'regex options to find the string in response 
            Dim HrefMatch As Match = HrefRegex.Match(strSource)                                                      'Find matches in String
            While HrefMatch.Success = True                                                                           'String Found = True 1 or more strings matching
                Dim pUrl As String = HrefMatch.Groups(1).Value                                                       'get first match
                If pUrl.Contains(spiderurl) AndAlso Not pUrl.Contains("page") AndAlso Not pUrl.Contains("#") Then aReturn.Add(pUrl) 'precheck if link contains everything we want and fill the ReturnArray
                HrefMatch = HrefMatch.NextMatch                                                                      'go to next match
            End While
        Catch ex As Exception
            Console.WriteLine(ex.Message, Environment.NewLine)                                                       'Errorhandling for user debugging
            Console.ReadLine()
        End Try
        Return aReturn
    End Function


    Private Function findTitle(ByVal str As String)
        Dim aReturn As String = Nothing
        Try
            Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
            '           {http://www.movie-blog.org/2017/02/01/(.*?)-[1-2]{1}[0-9]{3}-.*?}
            'example url http://www.movie-blog.org/2017/02/01/kung-fu-killer-german-dts-dl-2014-1080p-bluray-x264-leethd-2/
            'regexstr [1-2]{1}[0-9]{3} find 4 digits 1666 2999 1587 for year
            Dim strRegex As String = spiderurl & "(.*?\-[1-2]{1}[0-9]{3})\-.*?"                                     'with year
            'Dim strRegex As String = spiderurl & "(.*?)\-[1-2]{1}[0-9]{3}\-.*?"                                    'without year
            Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                  'regex options to find the string in response 
            Dim HrefMatch As Match = HrefRegex.Match(str)                                                           'Find matches in String
            If HrefMatch.Success = True Then                                                                        'String Found = True 1 or more strings matching
                Dim regStr As String = HrefMatch.Groups(1).Value                                                    'get match
                'regStr = regStr.Substring(0, 1).ToUpper() & regStr.Substring(1)
                regStr = regStr.Replace("ae", "ä").Replace("oe", "ö").Replace("ue", "ü").Replace("-", " ")          'replace chars
                aReturn = ti.ToTitleCase(regStr)                                                                    'Word correction (uppercase chars)
                Return aReturn
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message, Environment.NewLine)                                                      'Errorhandling for user debugging
            Console.ReadLine()
        End Try
        Return aReturn
    End Function


    Private Function getRealName(ByVal url As String)
        Dim aReturn As String = Nothing
        Try
            Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://www.moviepilot.de/movies/" & url)                         'new Webrequest
            Dim myWebResponse = CType(Request.GetResponse(), HttpWebResponse)                                        'shitty thing
            Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())                                 'streamreader to get response from webrequest live
            Dim strSource = myStreamReader.ReadToEnd                                                                 'finish reading of response

            '<h1 class="movie--headline" itemprop="name">Alice in den Städten</h1>
            'Dim strRegex As String = "<h1 class\=" & "movie\-\-headline"" itemprop\=""name"">(.*?)<\/h1>"               'regex string to find title
            Dim strRegex As String = "(.*?)"
            Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                   'regex options to find the string in response 
            Dim HrefMatch As Match = HrefRegex.Match(strSource)                                                      'Find matches in String
            If HrefMatch.Success = True Then                                                                         'String Found = True 1 or more strings matching
                Dim regStr As String = HrefMatch.Groups(1).Value                                                     'get match
                aReturn = regStr                                                                                     '
                Return aReturn
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message, Environment.NewLine)                                                       'Errorhandling for user debugging
            Console.ReadLine()
        End Try
        Return aReturn
    End Function 'for testing doesnt work xD

End Module
