Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Module Module1
    Dim client = Nothing
    Dim listenThread As New System.Threading.Thread(AddressOf listen)
    Dim reconnect As Boolean = False
    Dim globalNachricht As String = Nothing
    Dim nickname
    Sub Main()
        Dim ip As IPAddress = Nothing
        Dim port As Integer = 27590
        Dim ipOK As Boolean = False
        Console.Title = "OpenSchoolChat - Client"
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("OpenSchoolChat - Client | Alpha, Build 4")
        While ipOK = False
            Try
                Console.Write("Enter the server's IP: ")
                ip = IPAddress.Parse(Console.ReadLine())
                ipOK = True
            Catch e As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("IP adress could not be read. Error code: {0}", e)
                Console.WriteLine("Try again please.")
                Console.ForegroundColor = ConsoleColor.White
            End Try
        End While
        While True
            connect(ip, port)
            Try
                listenThread.Start()
            Catch ex As Exception
                Try
                    listenThread.Resume()
                Catch e As Exception
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("Error starting listener: {0}", e)
                    Console.ReadLine()
                    End
                End Try
            End Try
            While client.Connected
                send()
            End While
        End While
        Console.Read()
    End Sub
    Sub connect(ip, port)
        Try
            client = New TcpClient
            client.Connect(ip, port)
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Connected to server on {0}", client.Client.RemoteEndPoint)
            nickname = client.Client.RemoteEndPoint
        Catch e As SocketException
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Could not connect to server. Retrying...")
            connect(ip, port)
        End Try
    End Sub
    Sub send()
        Console.ForegroundColor = ConsoleColor.White
        Dim tempCursorTop = Console.CursorTop
        Dim nachricht As String = Console.ReadLine()
        'Console.SetCursorPosition(0, tempCursorTop)
        If nachricht <> "" Then
            If isSetNick(nachricht) Then
                Try
                    If nachricht.Substring(9, nachricht.Length - 9) <> "" Then
                        nickname = nachricht.Substring(9, nachricht.Length - 9)
                    End If
                Catch ex As Exception
                End Try
            End If
            Dim msg(1024) As Byte
            msg = System.Text.Encoding.Default.GetBytes(nachricht)
            Try
                client.GetStream.Write(msg, 0, msg.Length)
                globalNachricht = nachricht
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine("{0} um {1} Uhr", nickname, TimeOfDay.TimeOfDay)
                Console.ForegroundColor = ConsoleColor.White
                Console.WriteLine(nachricht)
            Catch e As Exception
                client.Close()
            End Try
        End If
    End Sub
    Sub listen()
        Dim nachricht As String = Nothing
        Dim msg(1024) As Byte
        Dim i As Int32
        Try
            Dim stream As NetworkStream = client.GetStream()
            i = stream.Read(msg, 0, msg.Length)
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Connection closed. Type anything to reconnect:")
            Console.ForegroundColor = ConsoleColor.White
            listenThread.Suspend()
        End Try
        nachricht = System.Text.Encoding.Default.GetString(msg, 0, i)
        If nachricht <> "" And nachricht <> globalNachricht Then
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("Von Server um " & TimeOfDay & " Uhr")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine(nachricht)
            globalNachricht = ""
        End If
        listen()
    End Sub
    Function isSetNick(ByVal nachricht) As Boolean
        Try
            If nachricht.substring(1, 7) = "setnick" Then
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function
End Module
