using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace MySqlManagement.Script
{
    internal class EventHandler
    {
        private string _database = string.Empty;
        private string _pathDatabase = string.Empty;
        private CommandEnum command = CommandEnum.NONE;
        public enum CommandEnum
        {
                CREATE = 0,
                INSERT = 1,
                USE = 2,
                SHOW = 3,
                DESCRIBE = 4,
                SELECT = 5,
                QUIT = 98,
                NONE = 99

        }
        /// <summary>
        /// Methode qui permet de récupére les informations de la requête et définir le type de requête
        /// </summary>
        /// <param name="pRequest">l'entrée de l'utilisateur</param>
        public void GetRequest(string pRequest)
        {
            string command = GetCommand(pRequest);
            switch (this.command) 
            {
                case CommandEnum.SELECT:
                    if ((!string.IsNullOrEmpty(command) || command != " ") && this.DbConnect())
                    {
                        Select(command);
                    }
                    else
                    {
                        Utils.ShowMessage("Aucune informations n'est saisie !");
                    }
                    break;
                case CommandEnum.QUIT:
                    Engine.GetInstance().QuitApp();
                    break;
                case CommandEnum.USE:
                    Use(command);
                    break;
                case CommandEnum.CREATE:
                    Create(command);
                    break;
                case CommandEnum.SHOW:
                    Show(command);
                    break;
                case CommandEnum.INSERT:
                    if (this.DbConnect())
                    {
                        Insert(pRequest);
                    }
                    break;
                case CommandEnum.DESCRIBE:
                    if (this.DbConnect())
                    {
                        Describe(command);
                    }
                    break;
                default:
                    Console.WriteLine(command); 
                    break;
            }
        }

        #region requete sql
        private void Describe(string pCommand)
        {
            if (Table.Exist(this.SetPathTable(pCommand.Trim())))
            {
                string strHeaders = string.Empty;
                Console.WriteLine("");
                Console.WriteLine("-----------");
                Console.WriteLine("");

                foreach (string header in Table.GetHeader(Path.Combine(this.SetPathTable(pCommand.Trim()))))
                {
                    strHeaders += $"{header}     ";
                }
                Console.WriteLine(strHeaders);
                Console.WriteLine("");
                Console.WriteLine("-----------");
                Console.WriteLine("");
            }

        }

        public void Select(string pRequest)
        {
            string[] req = pRequest.Replace("from","").Split(" ").Where(d => !string.IsNullOrEmpty(d)).ToArray();
            Utils.ResponseSelect selectReq = new Utils.ResponseSelect(req);
            if (Table.Exist(this.SetPathTable(selectReq.TableName)))
            {
                List<string> headers = Table.GetHeader(this.SetPathTable(selectReq.TableName));
                List<Dictionary<string,string>> datas = Table.Select(this.SetPathTable(selectReq.TableName));
                switch (Engine.GetInstance().FormatEnum)
                {
                    case Utils.FormatEnum.CSV:
                        PrintCSV(datas);
                        break;
                    case Utils.FormatEnum.ARRAY:
                        PrintArrayStyle(headers, datas);
                        break;
                    case Utils.FormatEnum.JSON:
                        PrintJson(datas);
                        break;
                }
            }
            else
            {
                Utils.ShowMessage($"La table {selectReq.TableName} n'existe pas dans la base {this._database}");
            }
        }

        private void Use(string pRequest)
        {
            string[] req = pRequest.Split(" ");
            switch (req.Length)
            {
                case 1:
                    Console.WriteLine("Veuillez entrez 2 arguments");
                    break;
                case 2:
                    if (DbExist(Path.Combine(Engine.GetInstance().DatabasePath, req[1])))
                    {
                        this._database = req[1];
                        this._pathDatabase = Path.Combine(Engine.GetInstance().DatabasePath, this._database);
                        Console.WriteLine($"Vous utilisez la base de donnée {this._database}");
                    }
                    else
                    {
                        Console.WriteLine($"La base {req[1]} n'existe pas utilisé CREATEBATABASE pour l'utlisé");
                    }
                    break;
            }
        }
        #endregion
        
        #region Affichage des tables ou des bases de données
        private void Show(string pCommand)
        {
            pCommand = pCommand.Trim();
            switch (pCommand)
            {
                case "databases":
                    ShowDatabases();
                    break;
                case "tables":
                    ShowTables();
                    break;
            }
        }
        private void ShowDatabases()
        {
            string[] databases = Directory.GetDirectories(Engine.GetInstance().DatabasePath);
            if(databases.Length  > 0)
            {
                Console.WriteLine("----");
                Console.WriteLine("");
                foreach (string database in databases)
                {
                    Console.WriteLine(Path.GetFileName(database));
                    Console.WriteLine("");
                    Console.WriteLine("----");
                }
            }
        }

        private void ShowTables()
        {
            string[] files = Directory.GetFiles(Path.Combine(Engine.GetInstance().DatabasePath,this._database));
            if(files.Length > 0)
            {
                Console.WriteLine("----");
                Console.WriteLine("");
                foreach (string file in files)
                {
                    Console.WriteLine(Path.GetFileName(file).Replace(".csv",""));
                    Console.WriteLine("");
                    Console.WriteLine("----");
                }
            }
        }
        #endregion

        private void Create(string pCommand) 
        {
            Utils.ResponseCreate res = new Utils.ResponseCreate(pCommand.Split(" ").Where(d => !string.IsNullOrEmpty(d)).ToArray());

            switch (res.Type)
            {
                case Utils.TypeEnum.NONE:
                    Utils.ShowMessage("Aucune information entrée");
                    break;
                case Utils.TypeEnum.DATABASE:
                    CreateDatabase(res.name);
                    break;
                case Utils.TypeEnum.TABLE:
                    CreateTable(res.name,res.Values);
                    break;
            }

        }

        private void CreateDatabase(string baseName)
        {
            string pathTemp = Path.Combine(Engine.GetInstance().DatabasePath, baseName);
            if (Directory.Exists(pathTemp))
            {
                Console.WriteLine($"Une base de donnée avec le nom {baseName} existe déjà");
            }
            else
            {
                Directory.CreateDirectory(pathTemp);
            }
            _database = baseName;
        }

        private void CreateTable(string pRequest, List<string> columns)
        {
            string csvFilePath = Path.Combine(Engine.GetInstance().DatabasePath,this._database,$"{pRequest}.csv");
            Table table = null;
            List<string> values = new List<string>();
            DataTable dt = new DataTable();
            if (!File.Exists(csvFilePath))
            {
                table = new Table(dt, csvFilePath);
                if(columns != null && columns.Count > 0)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        DataColumn col = new DataColumn();
                        col.ColumnName = columns[i];
                        dt.Columns.Add(col);
                    }
                }
                table.CreateCSV(values);
            }
            else
            {
                Console.WriteLine($"Il y a déjà une table avec le nom {pRequest} dans la base");
            }
            //table.Close();
        }

        private void Insert(string pRequest) 
        {
            string[] req = pRequest.Split(" ");
            string name = string.Empty;
            List<string> values = new List<string>();
            switch (req.Length)
            {
                case 1:
                    Console.WriteLine("Veuillez entrez 4 arguments");
                    break;
                case 2:
                    Console.WriteLine("Veuillez entrez 3 arguments");
                    break;
                case 3:
                    Console.WriteLine("Veuillez entrez 2 arguments");
                    break;
                case 4:
                    Console.WriteLine("Veuillez entrez 1 argument");
                    break;
                case 5:
                    name = req[2]; // nom de la table
                    values = TreatValues(req[4].Split(","));
                    InsertData(name, values);
                    break;
                default:
                    break;
            }
        }

        private void InsertData(string name, List<string> values)
        {
            string extension = ".csv";
            string path = Path.Combine(Engine.GetInstance().DatabasePath, _database, $"{name}{extension}");
            Table table = new Table(path);
            table.Insert(Table.GetHeader(path), values);
        }

        private List<string> TreatValues(string[] values) 
        {
            return (from value in values select value.Replace("(", "").Replace(")", "")).ToList();
        }

        private bool DbExist(string pDdPath)
        {
            if(Directory.Exists(pDdPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string GetCommand(string pCommand)
        {
            string command = string.Empty;
            string cmdTemp = pCommand.ToLower();
            string[] keywords = Engine.GetInstance().KEYWORD;
            for (int i = 0; i < keywords.Length; i++)
            {
                if (cmdTemp.Contains(keywords[i].ToLower()))
                {
                    this.command = (CommandEnum)i;
                    command = pCommand.Replace(keywords[i].ToLower(), "");
                }
            }
            if(pCommand.ToLower().Equals("quit") || pCommand.ToLower().Equals("exit"))
            {
                this.command = CommandEnum.QUIT;
            }
            return command;
        }

        private string SetPathTable(string pTableName)
        {
            return Path.Combine(Engine.GetInstance().DatabasePath, this._database, $"{pTableName}.csv");
        }

        private void PrintArrayStyle(List<string> headers, List<Dictionary<string,string>> datas)
        {
            string strHeaders = string.Empty;
            List<string> rows = new List<string>();
            Console.WriteLine("");
            Console.WriteLine("-----------");
            Console.WriteLine("");

            foreach (string header in headers)
            {
                strHeaders += $"{header}     ";
            }
            Console.WriteLine(strHeaders);
            Console.WriteLine("");

            foreach (Dictionary<string, string> r in datas)
            {
                string row = string.Empty;
                foreach (string value in r.Values)
                {
                    row += $"{value}     ";
                }
                rows.Add(row);
            }

            foreach (string row in rows)
            {
                Console.WriteLine(row);
                Console.WriteLine("");
            }
            Console.WriteLine("-----------");
        }
    
        private void PrintCSV(List<Dictionary<string, string>> datas)
        {
            foreach (Dictionary<string, string> row in datas)
            {
                foreach (var data in row)
                {
                    Utils.ShowMessage($"{data.Key}: {data.Value}");
                }
            }
        }
        private void PrintJson(List<Dictionary<string, string>> datas)
        {
            string json = JsonSerializer.Serialize(datas);
            Console.WriteLine(json);
        }

        private bool DbConnect()
        {
            if (!string.IsNullOrEmpty(this._database))
            {
                return true;
            }
            else{
                Utils.ShowMessage("Veuillez vous connecté à une base de données");
                return false;
            }
        }
    }
}
