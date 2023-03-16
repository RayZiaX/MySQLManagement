using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlManagement.Script
{
    internal class Engine
    {
        private bool _disposed = false;
        private EventHandler _handler = null;
        public string DatabasePath = string.Empty;
        public static char Separator = ';';
        private static Engine _instance { get; set; }
        public static Engine GetInstance() 
        {
            if(_instance == null)
            {
                _instance = new Engine();
            }
            return _instance;
        }

        protected Engine()
        {
            _handler = new EventHandler();
            DatabasePath = Path.Combine(Directory.GetCurrentDirectory(),"DataBases");
        }

        public void Update()
        {
            Console.WriteLine("Vous etes entrain d'utilisé le service MySqlEngine");
            while (!_disposed) 
            {
               string? input = Console.ReadLine();
               this._handler.GetRequest(input);
            }
        }

        public void QuitApp() 
        {
            _disposed = true;
        }

    }
}
