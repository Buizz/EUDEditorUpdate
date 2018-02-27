Imports System.Net
Imports System.IO
Imports System.IO.Compression

Public Class Form1
    Dim status As Integer = 0
    Dim lastver As String
    Dim lastverurl As String
    Dim currrentver As String

    Dim folder As String = My.Application.Info.DirectoryPath
    Private Sub Download(filename As String, savedfilename As String)
        Dim Client As New WebClient
        Dim StrDownUrl As String = filename
        Dim StrDownFolder As String = folder & savedfilename


        ServicePointManager.Expect100Continue = True
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        '파일다운로드
        '다른 다운로드 명령이 있으나 진행율을 표시하려면 DownloadFileAsync 을 사용해야 함
        Client.DownloadFileAsync(New Uri(StrDownUrl), StrDownFolder)

        'down1 이라는 이름으로 이벤트 생성
        AddHandler Client.DownloadProgressChanged, AddressOf down1

        'downok 이라는 이름으로 이벤트 생성
        AddHandler Client.DownloadFileCompleted, AddressOf downok
    End Sub


    Private Sub down1(sender As System.Object, e As DownloadProgressChangedEventArgs)
        Dim StrTxt As String
        StrTxt = e.UserState & " DownLoad " & e.BytesReceived & "byte/" & e.TotalBytesToReceive & "byte"
        LabelTxt(StrTxt)

        With ProgressBar1
            .Maximum = e.TotalBytesToReceive  '프로그래스바에 최대범위를 파일용량으로 넣음
            .Value = e.BytesReceived          '가져오는 바이트양을 넣음
        End With

    End Sub


    Sub DeleteFilesFromFolder(Folder As String)
        If Directory.Exists(Folder) Then
            For Each _file As String In Directory.GetFiles(Folder)
                If _file <> Folder & "\EUDEditorUpdate.exe" Then
                    File.Delete(_file)
                End If
            Next
            For Each _folder As String In Directory.GetDirectories(Folder)
                DeleteFilesFromFolder(_folder)
            Next
            Try
                Directory.Delete(Folder)
            Catch ex As Exception

            End Try
        End If
    End Sub


    Private Sub downok(sender As System.Object, e As System.ComponentModel.AsyncCompletedEventArgs)
        LabelTxt("다운로드 완료....")
        status += 1
        Progress()
    End Sub

    Private Sub Progress()
        Select Case status
            Case 1
                Dim versionfile As FileStream = File.Open(folder & "\temp\Lastversion", FileMode.Open)
                Dim streamreader As New StreamReader(versionfile)



                lastver = streamreader.ReadLine()
                lastverurl = streamreader.ReadLine()

                Label2.Text = "EUDEditor2 " & lastver & " Downloading..."

                streamreader.Close()
                versionfile.Close()

                Download(lastverurl, "\temp\tmep.zip")
            Case 2
                'Dim startPath As String = "c:\example\start"
                Dim zipPath As String = folder & "\temp\tmep.zip"
                Dim extractPath As String = folder

                Try
                    ZipFile.ExtractToDirectory(zipPath, extractPath)
                Catch ex As Exception
                    MsgBox("예상치 못한 에러가 발생했습니다.")
                    Progress()
                    Exit Sub
                End Try


                DeleteFilesFromFolder(folder & "\temp")

                Try
                    Process.Start(folder & "\EUD Editor.exe")
                Catch ex As Exception

                End Try
                Close()
        End Select
    End Sub


    Private Sub LabelTxt(ByVal StrTxt As String)
        Label1.Text = StrTxt
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DeleteFilesFromFolder(folder)

        Directory.CreateDirectory(folder & "\temp")

        '버전 확인
        Download("https://github.com/Buizz/EUDEditor/raw/master/version/version", "\temp\Lastversion")
    End Sub
End Class

