using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlManagement.Script
{
    internal class Table
    {
        private DataTable _table;
        private string _path;
        public Table(DataTable pTable,string pPath) 
        {
            this._table = pTable;
            this._path = pPath;
        }

        public Table(string pPath)
        {
            this._path = pPath;
            this._table = new DataTable();
        }

        public void CreateCSV(List<string> values)
        {
            List<string> columns = new List<string>();
            columns.Add(CreateHeaderCSV()); // Ajout des headers

            File.WriteAllLines(_path, columns);
        }




        public string CreateHeaderCSV()
        {
            DataColumn[] col = _table.Columns.Cast<DataColumn>().ToArray();
            return string.Join(Engine.Separator, col.Select(c => c.ColumnName));
        }

        public static List<Dictionary<string, string>> ReadTable(string path)
        {
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            using (var reader = new StreamReader(path))
            {
                // Lire l'en-tête
                string[] headers = reader.ReadLine().Split(Engine.Separator);

                // Lire les données
                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(Engine.Separator);
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row.Add(headers[i], values[i]);
                    }
                    data.Add(row);
                }
                reader.Close();
            }
            return data;
        }

        public static List<string> GetHeader(string path) 
        {
            string[] headers;
            using (var reader = new StreamReader($"{path}"))
            {
                headers = reader.ReadLine().Split(Engine.Separator);
                reader.Close();
            }
            return headers.ToList();
        }

        public static bool TableExist(string pPath, string pTableName)
        {
            if (File.Exists(Path.Combine(pPath,pTableName)))
            {
                return true;
            }
            return false;
        }

        public void Insert(List<string> pColumn, List<string> pValues)
        {
            Dictionary<string, string> newData = new Dictionary<string, string>();
            if(pColumn.Count == pValues.Count)
            {
                for(int i = 0;i < pColumn.Count;i++)
                {
                    newData.Add(pColumn[i], pValues[i]);
                }

                using (StreamWriter writer = new StreamWriter(_path, true))
                {
                    // Écrire les données
                    writer.WriteLine(string.Join(Engine.Separator, newData.Values));
                }
            }
            else
            {
                Console.WriteLine($"Vous n'avez pas entrez toutes les valeurs de la table {pColumn.Count}");
            }
        }
    }
}
