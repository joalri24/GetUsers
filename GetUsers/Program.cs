using System;
using Renci.SshNet;

namespace GetUsers
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.WriteLine("Get Users!!");

            var connData = new ConnectionData()
            {
                Host = "xx.xx.xx.xx",
                Port = 22,
                Login = "fakeUser",
                Password = "fakePass"
            };

            GetUsers(connData);

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Make a connection to the target machine and gets all users.
        /// </summary>
        /// <param name="connData"></param>
        static void GetUsers(ConnectionData connData)
        {
            #region Set Connection Info
            var ConnInfo = new ConnectionInfo(connData.Host,connData.Port, connData.Login,
                new AuthenticationMethod[] {

                    new PasswordAuthenticationMethod(connData.Login, connData.Password)
                });
            #endregion

            #region Connect and execute commands
            using (var sshclient = new SshClient(ConnInfo))
            {
                Console.WriteLine("Connecting...");
                sshclient.Connect();
                Console.WriteLine("Connection succeded.");
                using (var cmd = sshclient.CreateCommand("pwd"))
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine($"Return Value = {cmd.Result}");
                }               
                sshclient.Disconnect();
                Console.WriteLine("Connection Closed.");

            }
            #endregion
        }

        /// <summary>
        /// Data needed to make the connection to a Unix machine.
        /// </summary>
        private struct ConnectionData
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }

        }
    }
}
