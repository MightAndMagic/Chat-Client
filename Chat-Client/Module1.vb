﻿Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Module Module1
    Dim client As New TcpClient
    Dim listenThread As New System.Threading.Thread(AddressOf listen)
    Dim reconnect As Boolean = False
    Sub Main()
        Dim ip As IPAddress = Nothing
        Dim port As Integer = 27590
        Console.Title = "OpenSchoolChat - Client"
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("OpenSchoolChat - Client | Alpha, Build 3")
        Try
            ip = IPAddress.Parse("127.0.0.1")
        Catch e As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("IP adress could not be read. Error code: {0}", e)
        End Try
        While True
            connect(ip, port)
            listenThread.Start()
            While client.Connected
                send()
            End While
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Connection closed. Reconnecting...")
        End While
        Console.Read()
    End Sub
    Sub connect(ip, port)
        Try
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
            Dim nachricht As String = Console.ReadLine()
            Dim msg(1024) As Byte
            msg = System.Text.Encoding.ASCII.GetBytes(nachricht)
            client.GetStream.Write(msg, 0, msg.Length)
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.WriteLine("{0} um {1} Uhr", client.Client.RemoteEndPoint, TimeOfDay.TimeOfDay)
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine(nachricht & vbCrLf)
    End Sub
    Sub listen()
        Dim nachricht As String = Nothing
        Dim msg(1024) As Byte
        Try
            Dim stream As NetworkStream = client.GetStream()
            Dim i As Int32
            i = stream.Read(msg, 0, msg.Length)
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Connection closed.")
            End
        End Try
        nachricht = System.Text.Encoding.ASCII.GetString(msg)
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("Von Server um " & TimeOfDay & " Uhr")
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine(nachricht)
        listen()
    End Sub
End Module