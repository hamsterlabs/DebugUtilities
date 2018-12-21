using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace HamsterLabs.DebugUtilities
{
    /*  Utilities for helping debug DataSets
        It helps a bit to name DataSets & DataTables
     */
    public class DataSetDumper
    {
        public static OutputVector vector { get; set; }
        public enum OutputVector { Debug = 1, Console = 2 };

        private DataSet ds;

        public DataSetDumper()
        {
            ds = new DataSet();
            vector = OutputVector.Debug;
        }

        //public DataSetDumper(DataSet d)
        //{
        //    vector = OutputVector.Debug;
        //    this.ds = new DataSet();
        //    ds.DataSetName = d.DataSetName;
        //    foreach(DataTable dt in ds.Tables)
        //    {
        //        ds.Tables.Add(dt.Copy());
        //    }
        //}

        public static void dump(DataSet d)
        {
            for(int i = 0; i < d.Tables.Count; i++)
            {
                dump(d, i);
            }
        }

        public static void dump(DataSet d, int table = 0)
        {
            Print(String.Format("# {0}", d.DataSetName));
            dump(d.Tables[table]);
            
        }

        public static void dump(DataTable t)
        {
            Print(String.Format("# {0}", t.TableName));
            
        }

        public static void dump(DataView v)
        {
            dump(v.ToTable());
        }

        public static void Print(string s)
        {
            switch (vector)
            {
                case OutputVector.Console:
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine(s);
                    break;
            }
        }

        public static string FormatCSV(string input)
        {
            try
            {
                if (input == null)
                    return string.Empty;

                bool containsQuote = false;
                bool containsComma = false;
                int len = input.Length;
                for (int i = 0; i < len && (containsComma == false || containsQuote == false); i++)
                {
                    char ch = input[i];
                    if (ch == '"')
                        containsQuote = true;
                    if (ch == (char)39)
                        containsQuote = true;
                    else if (ch == ',')
                        containsComma = true;
                    else if (ch == '`')
                        containsQuote = true;

                }

                if (containsQuote && containsComma)
                    input = input.Replace("\"", "\"\"");

                if (containsComma)
                    return "\"" + input + "\"";
                else
                    return input;
            }
            catch
            {
                throw;
            }
        }
    }
}
