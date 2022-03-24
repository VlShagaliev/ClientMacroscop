using System;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace SocketClient
{
    class Program
    {
        
        const int port = 8888;
        const string address = "127.0.0.1";
        static string userName;
        static void Main(string[] args)
        {
            TcpClient client = null;
            Console.Write("Укажите папку для считывания файлов: ");
            string directoryFile = Console.ReadLine();
            string[] fileList = Directory.GetFiles(directoryFile, "*.txt");
            string[] textInFiles = new string[fileList.Length];

            for (int i = 0; i < fileList.Length; i++)
            {
                using (StreamReader reader = new(fileList[i]))
                {
                    textInFiles[i] = reader.ReadToEnd();
                    Console.WriteLine(textInFiles[i]);
                }
            }

            try
            {
                Console.Write("Введите свое имя: ");
                userName = Console.ReadLine();
                
                //if (client.Connected)
                //{
                    /*byte[] dataName = Encoding.Unicode.GetBytes(userName);
                    stream.Write(dataName, 0, dataName.Length);*/
                    for (int i = 0; i < textInFiles.Length; i++)
                    {
                        string temp = textInFiles[i];
                        Thread thread = new Thread(() => Request(temp));
                        thread.Start();
                        void Request(string temp)
                        {
                            client = new TcpClient(address, port);
                            NetworkStream stream = client.GetStream();
                            byte[] dataName = Encoding.Unicode.GetBytes(userName);
                            stream.Write(dataName, 0, dataName.Length);
                            Thread.Sleep(10); //исключаем одновременную отправку в поток имени и сообщения

                            // преобразуем сообщение в массив байтов
                            byte[] data = Encoding.Unicode.GetBytes(temp);

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

                            string message = builder.ToString();
                            Console.WriteLine($"Сервер: ({temp}) {message}");
                        }
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }

            }
        }
        
    }
}