using MySqlManagement.Script;
using System;
using System.ComponentModel.Design;
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
                PrintPaperGuide();
            }
            if (req.login)
            {
                TestConnexion(req);
            }
            if ((!string.IsNullOrEmpty(req.user) && !string.IsNullOrEmpty(req.password)) && !req.login && string.IsNullOrEmpty(req.options))
            {
                Engine.GetInstance().FormatEnum = req.format;
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
            if (!string.IsNullOrEmpty(req.options))
            {
                string command = req.options.Replace("\"","");
                string[] commands = command.Split(';');
                for (int i = 0; i < commands.Length; i++)
                {
                    Engine.GetInstance().GetHandler().GetRequest(commands[i]) ;
                }
                Thread.Sleep(5000);
            }

        }

        private static RequestArgs CheckArgs(string[] args)
        {

            RequestArgs request = new RequestArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-u" || args[i] == "--user")
                {
                    request.user = args[i + 1];
                }
                if (args[i] == "-p" || args[i] == "--password")
                {
                    request.password = args[i + 1];
                }
                if (args[i] == "-l" || args[i] == "--login")
                {
                    request.login = true;
                }
                if(args[i] == "-v" || args[i] == "--version")
                {
                    request.version = true;
                }
                if(args[i] == "-h" || args[i] == "--help")
                {
                    request.help = true;
                }
                
                if (args[i] == "-s" || args[i] == "--sql")
                {
                    request.options = args[i + 1];
                }
            }
            request.format = GetFormat(args);
            return request;
        }

        private static Utils.FormatEnum GetFormat(string[] req)
        {
            string strTemp = string.Empty;
            for (int i = 0; i < req.Length; i++)
            {
                if (req[i].ToLower().Contains("-f"))
                {
                    strTemp = req[i].Replace("-f=", "");
                }
                else if (req[i].ToLower().Contains("--format"))
                {
                    strTemp = req[i].Replace("--format=", "");
                }
            }
            switch (strTemp.ToLower())
            {
                case "csv":
                    return Utils.FormatEnum.CSV;
                case "json":
                    return Utils.FormatEnum.JSON;
                case "array":
                default:
                    return Utils.FormatEnum.ARRAY;
            }
        }

        private static void PrintPaperGuide()
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

            Thread.Sleep(5000);
        }

        private static void TestConnexion(RequestArgs pReq)
        {
            if (Engine.GetInstance().Login(pReq.user, pReq.password))
            {
                Console.WriteLine("Connexion réussi");
            }
            else
            {
                Console.WriteLine("Connexion refusée");
            }
            Thread.Sleep(5000);
        }
        struct RequestArgs
        {
            public string user { get; set; }
            public string password { get; set; }
            public bool login { get; set; }
            public bool version { get; set; }
            public bool help { get; set; }
            public Utils.FormatEnum format { get; set; }
            public string? options{ get; set; }
        }
    }
}