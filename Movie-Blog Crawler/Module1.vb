Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Xml

Module Module1

#Region "Public_Dims"                                                                               'i think here is nothing to say

    Dim dt As DateTime = Date.Today.AddDays(-2) 'for Testing
    'Dim dt As DateTime = Date.Today
    Dim month As String = dt.ToString("MM", CultureInfo.InvariantCulture)
    Dim year As String = dt.ToString("yyyy", CultureInfo.InvariantCulture)
    Dim day As String = dt.ToString("dd", CultureInfo.InvariantCulture)
    Dim pages As Integer = 0
    Const quote As String = Chr(34)                                                                 'quote(") for text 
    Public spiderurl = "http://www.movie-blog.org/" & year & "/" & month & "/" & day & "/"          'the mainurl from Today 
    Dim nl = Environment.NewLine
    Dim setSerie As Boolean
    Dim setQuality As String
    Dim setSource As String
    Dim setAll As Boolean

#End Region

    Sub Main()
        Try
            settingHelper()
            Dim findArr As New ArrayList
            findArr = readFilms()
            pFinder()                                                                                       'Find numbrs of pages from Date.Today
            Dim listH As New ArrayList
            listH = listHelper()                                                                            'creates a link list from pages
            Dim outP As New ArrayList
            Dim x As Integer = 1
            For Each item As String In listH
                Dim retStr As String = findTitle(item)
                If Not retStr = Nothing Then
                    If Not outP.Contains(retStr) Then
                        For Each arr As String In findArr
                            If retStr.Contains(arr) Then Console.WriteLine("Gefunden!!!!")
                        Next
                        outP.Add(retStr)
                            Console.WriteLine((String.Format("{0:000}", x) & ": " & retStr), nl)
                        writeFile(item.ToString, retStr)
                        x += 1
                    End If
                End If
            Next

            Console.WriteLine("Ende.....")
            Console.ReadLine()                                                                                  'let the Consolewindow stay open
            File.Delete("c:\temp\MyTest.html")
        Catch ex As Exception
            Console.WriteLine(ex.Message, nl)                                                               'Errorhandling for user debugging
            Console.ReadLine()
        End Try
    End Sub

    Private Function listHelper()
        Dim aReturn As New ArrayList
        Dim i As Integer = 1
        While i <= pages
            Dim aList As ArrayList = getShit(spiderurl & "page/" & i & "/")                                         'Take every page and search for Links
            For Each item As String In aList
                If Not aReturn.Contains(item) Then                                                                  'check if item is allready in list
                    Select Case setAll
                        Case True
                            aReturn.Add(item)
                        Case False
                            Select Case setSerie
                                Case True
                                    If item.Contains("-s0") Or item.Contains("-s1") Or item.Contains("-s2") Then
                                        Select Case setQuality
                                            Case "1080p"
                                                If item.Contains("1080p") Then
                                                    Select Case setSource
                                                        Case "BluRay"
                                                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "DVD"
                                                            If item.Contains("dvd") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "other"
                                                            If Not item.Contains("bdrip") AndAlso Not item.Contains("dvd") AndAlso Not item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                    End Select
                                                End If
                                            Case "720p"
                                                If item.Contains("720p") Then
                                                    Select Case setSource
                                                        Case "BluRay"
                                                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "DVD"
                                                            If item.Contains("dvd") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "other"
                                                            If Not item.Contains("bdrip") AndAlso Not item.Contains("dvd") AndAlso Not item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                    End Select
                                                End If
                                            Case "SD"
                                                If Not item.Contains("720p") Or Not item.Contains("1080p") Then
                                                    Select Case setSource
                                                        Case "BluRay"
                                                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "DVD"
                                                            If item.Contains("dvd") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "other"
                                                            If Not item.Contains("bdrip") AndAlso Not item.Contains("dvd") AndAlso Not item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                    End Select
                                                End If
                                        End Select
                                    End If
                                Case False
                                    If Not item.Contains("-s0") AndAlso Not item.Contains("-s1") AndAlso Not item.Contains("-s2") Then
                                        Select Case setQuality
                                            Case "1080p"
                                                If item.Contains("1080p") Then
                                                    Select Case setSource
                                                        Case "BluRay"
                                                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "DVD"
                                                            If item.Contains("dvd") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "other"
                                                            If Not item.Contains("bdrip") AndAlso Not item.Contains("dvd") AndAlso Not item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                    End Select
                                                End If
                                            Case "720p"
                                                If item.Contains("720p") Then
                                                    Select Case setSource
                                                        Case "BluRay"
                                                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "DVD"
                                                            If item.Contains("dvd") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "other"
                                                            If Not item.Contains("bdrip") AndAlso Not item.Contains("dvd") AndAlso Not item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                    End Select
                                                End If
                                            Case "SD"
                                                If Not item.Contains("720p") Or Not item.Contains("1080p") Then
                                                    Select Case setSource
                                                        Case "BluRay"
                                                            If item.Contains("bdrip") Or item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "DVD"
                                                            If item.Contains("dvd") Then
                                                                aReturn.Add(item)
                                                            End If
                                                        Case "other"
                                                            If Not item.Contains("bdrip") AndAlso Not item.Contains("dvd") AndAlso Not item.Contains("bluray") Then
                                                                aReturn.Add(item)
                                                            End If
                                                    End Select
                                                End If
                                        End Select
                                    End If
                            End Select
                    End Select

                    'If item.Contains("720p") Or item.Contains("1080p") Then                                         'only choose HD movies or series
                    '    If item.Contains("bdrip") Or item.Contains("bluray") Then
                    '        If Not item.Contains("s0") Or Not item.Contains("s1") Or Not item.Contains("s2") Then
                    '            aReturn.Add(item)                                                                   'add the item
                    '        End If
                    '    End If
                    'End If
                End If
            Next
            i += 1                                                                                                  'Next Site
        End While
        Return aReturn
    End Function

    Private Sub writeFile(ByVal strUrl As String, ByVal strName As String)
        Dim filePath As String = My.Application.Info.DirectoryPath & "\MyTest.html"
        Dim fileWrite As System.IO.StreamWriter                                                                  'write the links to file(only for testing?)
        If Not File.Exists(filePath) Then
            fileWrite = My.Computer.FileSystem.OpenTextFileWriter(filePath, True)
            fileWrite.WriteLine("<h1>Filme " & dt.Date & "</h1>")
            fileWrite.Close()
        End If
        fileWrite = My.Computer.FileSystem.OpenTextFileWriter(filePath, True)
        'Console.WriteLine(str, nl)
        fileWrite.WriteLine("<p><a href=" & quote & strUrl & quote & ">" & strName & "</a></p>")                       'for html Sytax in File(needs mor testing)
        fileWrite.Close()                                                                                        'close Streamwriter
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
                Console.WriteLine("Pages: " & num, nl)                                                              'output for testing
                pages = Convert.ToInt32(num)                                                                        'convert String to Integer
                HrefMatch = HrefMatch.NextMatch                                                                     'More matches? get them and print this shit out
            End While
        Catch ex As Exception
            Console.WriteLine(ex.Message, nl)                                                                       'Errorhandling for user debugging
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
                If pUrl.Contains(spiderurl) AndAlso Not pUrl.Contains("page") AndAlso Not pUrl.Contains("#") Then
                    aReturn.Add(pUrl)                                                                                'precheck if link contains everything we want and fill the ReturnArray
                    'Console.WriteLine(pUrl, nl, nl)
                End If
                'Console.WriteLine(pUrl, nl & nl)                                                                    'only for debug
                HrefMatch = HrefMatch.NextMatch                                                                      'go to next match
            End While
        Catch ex As Exception
            Console.WriteLine(ex.Message, nl)                                                                        'Errorhandling for user debugging
            Console.ReadLine()
        End Try
        Return aReturn
    End Function

    Private Function findTitle(ByVal str As String)
        Dim aReturn As String = Nothing
        Try
            Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
            'example url http://www.movie-blog.org/2017/02/01/kung-fu-killer-german-dts-dl-2014-1080p-bluray-x264-leethd-2/
            Dim strRegexY As String = spiderurl & "(.*?\-[1-2]{1}[0-9]{3})\-.*?"                                     'with year    'regexstr [1-2]{1}[0-9]{3} find 4 digits 1666 2999 1587 for year
            'Dim strRegex As String = spiderurl & "(.*?)\-[1-2]{1}[0-9]{3}\-.*?"                                    'without year

            'example url http://www.movie-blog.org/2017/02/03/ein-riskanter-plan-german-dl-1080p-bluray-x264-sons-2/
            Dim strRegexG As String = spiderurl & "(.*?)\-german\-.*?"                                              'if not contains a Year

            Dim HrefRegexY As New Regex(strRegexY, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                  'regex options to find the string in response 
            Dim HrefMatchY As Match = HrefRegexY.Match(str)                                                           'Find matches in String
            If HrefMatchY.Success = True Then                                                                        'String Found = True 1 or more strings matching
                Dim regStrY As String = HrefMatchY.Groups(1).Value                                                    'get match
                regStrY = regStrY.Replace("ae", "ä").Replace("oe", "ö").Replace("ue", "ü").Replace("-", " ")          'replace chars
                aReturn = ti.ToTitleCase(regStrY)                                                                    'Word correction (uppercase chars)
                Return aReturn
            Else
                Dim HrefRegexG As New Regex(strRegexG, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                  'regex options to find the string in response 
                Dim HrefMatchG As Match = HrefRegexG.Match(str)                                                           'Find matches in String
                If HrefMatchG.Success = True Then                                                                        'String Found = True 1 or more strings matching
                    Dim regStrG As String = HrefMatchG.Groups(1).Value                                                    'get match
                    regStrG = regStrG.Replace("ae", "ä").Replace("oe", "ö").Replace("ue", "ü").Replace("-", " ")          'replace chars
                    aReturn = ti.ToTitleCase(regStrG)                                                                    'Word correction (uppercase chars)
                    'Console.WriteLine(aReturn & nl, nl)
                    Return aReturn
                Else
                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message, nl)                                                                       'Errorhandling for user debugging
            Console.ReadLine()
        End Try
        Return aReturn
    End Function

    Private Sub settingHelper()
        Dim settingPath = My.Application.Info.DirectoryPath & "\settings.xml"
        If System.IO.File.Exists(settingPath) Then
            Dim settingsRead As XmlReader = New XmlTextReader(settingPath)
            While (settingsRead.Read())
                Dim type = settingsRead.NodeType
                If (type = XmlNodeType.Element) Then
                    If (settingsRead.Name = "quality") Then
                        setQuality = settingsRead.ReadInnerXml.ToString()
                    End If
                    If (settingsRead.Name = "source") Then
                        setSource = settingsRead.ReadInnerXml.ToString()
                    End If
                    If (settingsRead.Name = "series") Then
                        setSerie = Convert.ToBoolean(settingsRead.ReadInnerXml.ToString())
                    End If
                    If (settingsRead.Name = "listall") Then
                        setAll = Convert.ToBoolean(settingsRead.ReadInnerXml.ToString())
                    End If
                End If
            End While
        Else
            Console.WriteLine("The filename you selected was not found.", nl)
        End If
    End Sub

    Private Function readFilms()
        Dim txtReader As New StreamReader(My.Application.Info.DirectoryPath & "\releases.txt")
        Dim sLine As String = ""
        Dim aReturn As New ArrayList()
        Do
            sLine = txtReader.ReadLine()
            If Not sLine Is Nothing Then
                aReturn.Add(sLine)
            End If
        Loop Until sLine Is Nothing
        txtReader.Close()
        Return aReturn
    End Function

    'Private Function getRealName(ByVal url As String)  'dont work only for testing
    '    Dim aReturn As String = Nothing
    '    Try
    '        Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("http://www.moviepilot.de/movies/" & url)                         'new Webrequest
    '        Dim myWebResponse = CType(Request.GetResponse(), HttpWebResponse)                                        'shitty thing
    '        Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())                                 'streamreader to get response from webrequest live
    '        Dim strSource = myStreamReader.ReadToEnd                                                                 'finish reading of response

    '        '<h1 class="movie--headline" itemprop="name">Alice in den Städten</h1>
    '        'Dim strRegex As String = "<h1 class\=" & "movie\-\-headline"" itemprop\=""name"">(.*?)<\/h1>"               'regex string to find title
    '        Dim strRegex As String = "(.*?)"
    '        Dim HrefRegex As New Regex(strRegex, RegexOptions.IgnoreCase Or RegexOptions.Compiled)                   'regex options to find the string in response 
    '        Dim HrefMatch As Match = HrefRegex.Match(strSource)                                                      'Find matches in String
    '        If HrefMatch.Success = True Then                                                                         'String Found = True 1 or more strings matching
    '            Dim regStr As String = HrefMatch.Groups(1).Value                                                     'get match
    '            aReturn = regStr                                                                                     '
    '            Return aReturn
    '        End If
    '    Catch ex As Exception
    '        Console.WriteLine(ex.Message, nl)                                                       'Errorhandling for user debugging
    '        Console.ReadLine()
    '    End Try
    '    Return aReturn
    'End Function

    'Public Function goggleSearch(searchWord As String)              ' bullshit need to pay for more requests (Dayly limit)
    '    Dim aReturn As String = Nothing
    '    Try
    '        Dim query As String = searchWord & " Film"
    '        Dim customSearchService As New CustomsearchService(New Google.Apis.Services.BaseClientService.Initializer() With {.ApiKey = gApiK})
    '        Dim listRequest As Google.Apis.Customsearch.v1.CseResource.ListRequest = customSearchService.Cse.List(query)
    '        listRequest.Cx = sEngine
    '        Dim search As Search = listRequest.Execute()
    '        For Each item In search.Items
    '            'Console.WriteLine("Title : " + item.Title + nl + "Link : " + item.Link + nl + nl)
    '            Return item.Title
    '        Next
    '    Catch ex As Exception
    '        Console.WriteLine(ex.Message, nl)                                       'Errorhandling for user debugging
    '        Console.ReadLine()
    '    End Try
    '    Return aReturn
    'End Function


End Module
