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
                Host = "x.x.x.x",
                Port = 22,
                Login = "root",
                Password = "fakePass"
            };

            GetUnixUsers(connData);

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Makes a connection to a Unix machine and gets all users.
        /// </summary>
        /// <param name="connData"></param>
        static void GetUnixUsers(ConnectionData connData)
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
                using (var cmd = sshclient.CreateCommand("cat /etc/passwd")) // this file stores the machine users.
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    ProccessPasswdContent(cmd.Result);
                    //Console.WriteLine(cmd.Result);
                }               
                sshclient.Disconnect();
                Console.WriteLine("Connection Closed.");

            }
            #endregion
        }


        private static void ProccessPasswdContent(string passwdContent)
        {
            string[] lines = passwdContent.TrimEnd().Split('\n');
            foreach (var line in lines)
            {
                Console.WriteLine(line);
                
            }
        }

        #region Auxiliar data structures
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
        #endregion
    }
}
