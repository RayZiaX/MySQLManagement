using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlManagement.Script
{
    internal class Utils
    {
        public enum FormatEnum
        {
            NONE = 0,
            CSV = 1,
            ARRAY = 2,
            JSON = 3
        }
        public enum TypeEnum
        {
            NONE = 0,
            DATABASE = 1,
            TABLE = 2
        }

        public static void ShowMessage(string message)
        {
            Console.WriteLine($"{message}");
        }

        public class ResponseSelect
        {
            public string Column = string.Empty;
            public string TableName = string.Empty;
            public string WhereClause = string.Empty;
            public ResponseSelect(string[] pReq) 
            {
                if( pReq.Length > 0 ) 
                {
                    switch (pReq.Length)
                    {
                        case 1:
                            ShowMessage("vous avez rentrez aucune table !");
                            break;
                        case 2:
                            Column = pReq[0];
                            TableName = pReq[1];
                            ShowMessage("vous avez rentrez aucune condition toutes les données on été récupérer !");
                            break;
                        case 3:
                            Column = pReq[0];
                            TableName = pReq[1];
                            WhereClause = pReq[2];
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public class ResponseCreate
        {
            public TypeEnum Type = TypeEnum.NONE;
            public string name = string.Empty;
            public List<string> Values = null; 
            public ResponseCreate(string[] req)
            {
                switch(req.Length)
                {
                    case 1:
                        ShowMessage("Vous n'avez entré aucun nom pour votre base de données ou table");
                        break;
                    case 2:
                    case 3:
                        switch (req[0].ToLower())
                        {
                            case "database":
                            case "databases":
                                Type = TypeEnum.DATABASE;
                                break;
                            case "table":
                            case "tables":
                                Type = TypeEnum.TABLE;
                                Values = req[2].Replace("(", "").Replace(")", "").Split(",").ToList();
                                break;
                            default:
                                Type = TypeEnum.NONE;
                                break;
                        }
                        name = req[1];
                        break;
                }
            }
        }

        public class ResponseInsert
        {
            public string TableName = string.Empty;
            public List<string> Values = new List<string>();
            public List<string> headers = new List<string>();
        }
    }
}
