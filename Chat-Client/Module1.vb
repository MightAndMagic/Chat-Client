Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Module Module1
    Dim client = Nothing
    Dim listenThread As New System.Threading.Thread(AddressOf listen)
    Dim reconnect As Boolean = False
    Dim globalNachricht As String = Nothing
    Sub Main()
        Dim ip As IPAddress = Nothing
        Dim port As Integer = 27590
        Console.Title = "OpenSchoolChat - Client"
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("OpenSchoolChat - Client | Alpha, Build 4")
        Try
            ip = IPAddress.Parse("127.0.0.1")
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("IP adress could not be read. Error code: {0}", e)
        End Try
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
        Console.SetCursorPosition(0, tempCursorTop)
        If nachricht <> "" Then
            Dim msg(1024) As Byte
            msg = System.Text.Encoding.Default.GetBytes(nachricht)
            Try
                client.GetStream.Write(msg, 0, msg.Length)
                globalNachricht = nachricht
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.WriteLine("{0} um {1} Uhr", client.Client.RemoteEndPoint, TimeOfDay.TimeOfDay)
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
        End If
        listen()
    End Sub
End Module
