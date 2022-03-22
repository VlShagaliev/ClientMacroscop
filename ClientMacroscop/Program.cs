using System;
using System.Text;
using System.Net.Sockets;

namespace SocketClient
{
    class Program
    {
        
        const int port = 8888;
        const string address = "127.0.0.1";
        static void Main(string[] args)
        {
            TcpClient client = null;
            try
            {
                Console.Write("Введите свое имя: ");
                string userName = Console.ReadLine();
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
                if (client.Connected)
                {
                    byte[] dataName = Encoding.Unicode.GetBytes(userName);
                    stream.Write(dataName, 0, dataName.Length);
                    while (true)
                    {
                        Console.Write("Введите ваше сообщение: ");
                        // ввод сообщения
                        string message = Console.ReadLine();
                        message = String.Format($"{message}");

                        // преобразуем сообщение в массив байтов
                        byte[] data = Encoding.Unicode.GetBytes(message);

                        // отправка сообщения
                        stream.Write(data, 0, data.Length);

                        // получаем ответ
                        data = new byte[64]; // буфер для получаемых данных
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        message = builder.ToString();
                        Console.WriteLine("Сервер: {0}", message);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client!=null)
                {
                    client.Close();
                }
                
            }
        }
    }
}