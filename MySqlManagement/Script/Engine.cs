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
        public readonly string[] KEYWORD =
        {
            "CREATE","INSERT INTO","USE","SHOW","DESCRIBE","SELECT"
        };
        public Utils.FormatEnum FormatEnum = Utils.FormatEnum.NONE;

        private static Engine instance { get; set; }
        public static Engine GetInstance() 
        {
            if(instance == null)
            {
                instance = new Engine();
            }
            return instance;
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

        public bool Login(string user, string password)
        {
            List<Dictionary<string,string>> datas = Table.Select(Path.Combine(this.DatabasePath, "Admin", "Users.csv"));
            foreach (Dictionary<string, string> data in datas)
            {
                if (data["user"].Equals(user) && data["password"].Equals(password))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
