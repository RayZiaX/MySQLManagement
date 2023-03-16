using MySqlManagement.Script;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace MyApp // Note: actual namespace depends on the project name.
{
    //mybdd -u root -p root -l --sql “CREATE DATABASE database”
    internal class Program
    {

        static void Main(string[] args)
        {
            RequestArgs req = CheckArgs(args);
            if (req.version)
            {
                Console.WriteLine("Version Logiciel: 0.5.0");
            }
            if(req.help)
            {
                Console.WriteLine("-------");
                Console.WriteLine(" ");
                Console.WriteLine("-h ou --help: Permet d'afficher les différents paramètre de lancement");
                Console.WriteLine(" ");
                Console.WriteLine("-v ou --version: Permet d'afficher la dernier version en production de l'application");
                Console.WriteLine(" ");
                Console.WriteLine("-l --login: Permet de testé la connexion au serveur sql.");
                Console.WriteLine(" ");
                Console.WriteLine("-u --user: Indique à l'application de nom d'utilisateur qui sera utilisé sur le serveur.");
                Console.WriteLine("");
                Console.WriteLine("-p --password: Indique à l'application de mot de passe qui sera utilisé sur le serveur pour la connexion.");
                Console.WriteLine(" ");
                Console.WriteLine("-s --sql: Permet de réaliser une requête sql et quitter l'application.");
                Console.WriteLine("");
                Console.WriteLine("-f --forma: Permet de retourné un type de format pour les donnée des tables (CSV, JSON et Tableau visuel)");
                Console.WriteLine("");
                Console.WriteLine("-------");

            }
            if (req.login && (req.options?.Count == 0 || req.options == null ))
            {
                Console.WriteLine("Connexion en cours... ");
                if (Engine.GetInstance().Login(req.user, req.password))
                {
                    Console.WriteLine("Connexion réussi");
                    Engine.GetInstance().Update();
                }
                else
                {
                    Console.WriteLine("Connexion refusée");
                    Thread.Sleep(5000);
                }
            }
            else if(req.options?.Count > 0)
            {

            }
        }

        private static RequestArgs CheckArgs(string[] args)
        {
            RequestArgs request = new RequestArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-u") || args[i].Equals("--user"))
                {
                    request.user = args[i + 1];
                }else if (args[i].Equals("-p") || args[i].Equals("--password"))
                {
                    request.password = args[i + 1];
                }else if (args[i].Equals("-l") || args[i].Equals("--login"))
                {
                    request.login = true;
                }else if(args[i].Equals("-v") || args[i].Equals("--version"))
                {
                    request.version = true;
                }else if(args[i].Equals("-h") || args[i].Equals("--help"))
                {
                    request.help = true;
                }
                /*else if (args[i].Equals("-s") || args[i].Equals("--sql")) { }
                {
                    request.options = new List<string>();
                    for (int j = i; j < args.Length; j++)
                    {
                        request.options.Add(args[j]);
                    }
                    break;
                }*/
            }
            return request;
        }

        struct RequestArgs
        {
            public string user { get; set; }
            public string password { get; set; }
            public bool login { get; set; }
            public bool version { get; set; }
            public bool help { get; set; }
            public string format { get; set; }
            public List<string>? options{ get; set; }
        }
    }
}