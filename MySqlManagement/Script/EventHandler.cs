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

namespace MySqlManagement.Script
{
    internal class EventHandler
    {
        private string _database = string.Empty;
        private string _pathDatabase = string.Empty;
        public void GetRequest(string pRequest)
        {
             string command = pRequest.Split(" ")[0].ToLower();
            switch (command) 
            {
                case "select":
                    Select(pRequest);
                    break;
                case "quit":
                    Engine.GetInstance().QuitApp();
                    break;
                case "use":
                    Use(pRequest);
                    break;
                case "create":
                    Create(pRequest);
                    break;
                case "show":
                    Show(pRequest);
                    break;
                case "insert":
                    Insert(pRequest);
                    break;
                case "describe":
                    Describe(pRequest);
                    break;
                default:
                    Console.WriteLine(command); 
                    break;
            }
        }

        private void Describe(string pRequest)
        {
            string[] req = pRequest.Split(" ");
            switch (req.Length)
            {
                case 1:
                    Console.WriteLine("Veuillez entrez 2 arguments");
                    break;
                case 2:
                    string path = Path.Combine(Engine.GetInstance().DatabasePath, _database);
                    if (Table.TableExist(path, $"{req[1]}.csv"))
                    {
                        string strHeaders = string.Empty;
                        Console.WriteLine("");
                        Console.WriteLine("-----------");
                        Console.WriteLine("");

                        foreach (string header in Table.GetHeader(Path.Combine(path, $"{req[1]}.csv")))
                        {
                            strHeaders += $"{header}     ";
                        }
                        Console.WriteLine(strHeaders);
                        Console.WriteLine("");
                        Console.WriteLine("-----------");
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine($"table {req[1]} n'existe pas dans la base {this._database}");
                    }
                    break;
            }
        }

        private void Select(string pRequest)
        {
            string[] req = pRequest.Split(" ");
            switch (req.Length)
            {
                case 1:
                    Console.WriteLine("Veuillez entrez 3 arguments");
                    break;
                case 2:
                    Console.WriteLine("Veuillez entrez 2 arguments");
                    break;
                case 3:
                    Console.WriteLine("Veuillez entrez 1 arguments");
                    break;
                case 4:
                    string tableName = req[3];
                    string fileName = $"{tableName}.csv";
                    List<string> rows = new List<string>() ;
                    if (Table.TableExist(this._pathDatabase, fileName))
                    {
                        string strHeaders = string.Empty;
                        Console.WriteLine("");
                        Console.WriteLine("-----------");
                        Console.WriteLine("");

                        foreach (string header in Table.GetHeader(Path.Combine(this._pathDatabase, fileName)))
                        {
                            strHeaders += $"{header}     ";
                        }
                        Console.WriteLine(strHeaders);
                        Console.WriteLine("");

                        foreach (Dictionary<string,string> r in Table.ReadTable(Path.Combine(this._pathDatabase, fileName)))
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
                    break;
            }
        }

        private void Show(string pRequest)
        {
            string[] req = pRequest.Split(" ");
            string name = string.Empty;
            switch (req.Length)
            {
                case 1:
                    Console.WriteLine("Veuillez entrez 2 arguments");
                    break;
                case 2:
                    if (pRequest.Split(" ")[1].ToLower() == "databases")
                    {
                        ShowDatabases();
                    }
                    else if(pRequest.Split(" ")[1].ToLower() == "tables")
                    {
                        ShowTables();
                    }
                    break;
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
                        Console.WriteLine($"La base {this._database} n'existe pas utilisé CREATEBATABASE pour l'utlisé");
                    }
                    break;
            }
        }

        private void ShowDatabases()
        {
            string[] databases = Directory.GetDirectories(Engine.GetInstance().DatabasePath);
            if(databases.Length  > 0)
            {
                foreach (string database in databases)
                {
                    Console.WriteLine(database);
                }
            }
        }

        private void ShowTables()
        {
            string[] files = Directory.GetFiles(Path.Combine(Engine.GetInstance().DatabasePath,this._database));
            if(files.Length > 0)
            {
                foreach (string file in files)
                {
                    Console.WriteLine(file);
                }
            }
        }

        private void Create(string pRequest) 
        {
            string[] req = pRequest.Split(" ");
            string name = string.Empty;
            switch (req.Length)
            {
                case 1:
                    Console.WriteLine("Veuillez entrez 2 arguments");
                    break;
                case 2:
                    Console.WriteLine("Veuillez entrez un argument");
                    break;
                case 3:
                case 4:
                    name = pRequest.Split(" ")[2]; // récupération du nom du fichier
                    if(pRequest.Split(" ")[1].ToLower() == "database")
                    {
                        CreateDatabase(name);
                    }
                    else
                    {
                        List<string> columns = new List<string>();
                        if(req.Length == 4)
                        {
                            string[] columnsTemp = req[3].Split(',');
                            foreach (string column in columnsTemp) 
                            {
                                string columnHeader = string.Empty;
                                columnHeader = column.Replace("(","");
                                columnHeader = columnHeader.Replace(")", "");
                                columns.Add(columnHeader);
                            }
                            CreateTable(name, columns);
                        }
                    }
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
    }
}
