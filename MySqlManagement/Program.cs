using MySqlManagement.Script;
using System;
using System.Runtime.CompilerServices;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            if(!string.IsNullOrEmpty(input))
            {
                if(input.Contains("-u") && input.Contains("-p"))
                {
                    Engine.GetInstance().Update();
                }else if(input.Contains("-u") && input.Contains("-p") && input.Contains("-l"))
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
        }
    }
}