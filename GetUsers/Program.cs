using System;
using System.Collections.Generic;
using Renci.SshNet;

namespace GetUsers
{
    class Program
    {
        static List<string> linesToPrint = new List<string>();

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
            Console.Read();
        }

        /// <summary>
        /// Makes a connection to a Unix machine and gets all users.
        /// </summary>
        /// <param name="connData"></param>
        static void GetUnixUsers(ConnectionData connData)
        {
            var shadowUsers = new Dictionary<string, UnixUser>();
            
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
                using (var cmd = sshclient.CreateCommand("cat /etc/shadow")) // this file stores the machine users with the password status.
                {
                    cmd.Execute();
                    //Console.WriteLine("Command>" + cmd.CommandText);
                    ProccessShadowContent(cmd.Result, ref shadowUsers, connData.Host);
                    //Console.WriteLine(cmd.Result);
                }
                using (var cmd = sshclient.CreateCommand("cat /etc/passwd")) // this file stores the machine users.
                {
                    cmd.Execute();
                    //Console.WriteLine("Command>" + cmd.CommandText);
                    ProccessPasswdContent(cmd.Result, shadowUsers);
                    //Console.WriteLine(cmd.Result);
                }               
                sshclient.Disconnect();
                Console.WriteLine("Connection Closed.");

            }
            #endregion
        }


        private static void ProccessPasswdContent(string passwdContent, Dictionary<string, UnixUser> users)
        {
            string[] lines = passwdContent.TrimEnd().Split('\n');
            foreach (var line in lines)
            {
                // Passwd line example:
                // root:x:0:0:root:/root:/bin/bash
                // <login>:<pasword Encrypted?>:<UserID>:<GroupID>:<Home>:<User's Bash path>
                var userData = line.Split(':');
                int userId = Int32.Parse(userData[2]);

                // 0 is reserved for root, and values between 1 and 999 are reserved for the os.
                if (userId != 0 && userId <= 999)
                    continue; 

                Console.WriteLine(users[userData[0]].ToString());
                
            }
        }

        private static void ProccessShadowContent(string passwdContent, ref Dictionary<string,UnixUser> users, string host)
        {
            string[] lines = passwdContent.TrimEnd().Split('\n');
            foreach (var line in lines)
            {
                // Shadow line example:
                // root:$n$xxxggg::0:99999:7:::
                var userData = line.Split(':');
                UnixUser.UnixStatus userStatus;

                switch (userData[1]) // the password field
                {
                    case "*": // The password is locked
                        userStatus = UnixUser.UnixStatus.Locked;
                        break;

                    case "!!": // The password is expired
                        userStatus = UnixUser.UnixStatus.Expired;
                        break;

                    default:
                        userStatus = UnixUser.UnixStatus.Enabled;
                        break;
                }

                users.Add(userData[0],new UnixUser() { Login = userData[0], Host = host, Status = userStatus });
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

        private struct UnixUser
        {
            public enum UnixStatus
            {
                Enabled,
                Expired,
                Locked
            }
            public string Login { get; set; }
            public UnixStatus Status { get; set; }
            public string Host;

            public override string ToString()
            {
                return $"{Host} {Login} {Status}";
            }
        }
        #endregion
    }
}
