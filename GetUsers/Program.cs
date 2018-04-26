using System;
using Renci.SshNet;

namespace GetUsers
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.WriteLine("Get Users!!");
            var ConnInfo = new ConnectionInfo("x.x.x.x", 22, "fakeUser",
                new AuthenticationMethod[] {

                    new PasswordAuthenticationMethod("fakeUser", "fakePass")             
                });

            using (var sshclient = new SshClient(ConnInfo))
            {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand("pwd"))
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.Result);
                }
                sshclient.Disconnect();
            }

            Console.WriteLine("Press any key to continue.");
            string address = Console.ReadLine();
        }
    }
}
