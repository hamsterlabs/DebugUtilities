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
        private List<string> ColumnNames = new List<string>();

        public DataSetDumper()
        {
            ds = new DataSet();
            vector = OutputVector.Debug;
        }

        public DataSetDumper(DataSet d)
        {
            this.ds = d.Copy();
        }

        public DataSetDumper(DataTable d)
        {
            ds.Tables.Add(d.Copy());
        }

        public DataSetDumper(DataView d)
        {
            ds.Tables.Add(d.ToTable().Copy());
        }

        public void dump()
        {
            dump(this.ds);
        }

        public void dump(DataSet d)
        {
            for(int i = 0; i < d.Tables.Count; i++)
            {
                dump(d, i);
            }
        }

        public void dump(DataSet d, int table = 0)
        {
            Print(String.Format("# {0}", d.DataSetName));
            dump(d.Tables[table]);
            
        }

        public void dump(DataTable t)
        {
            Print(String.Format("# {0}", t.TableName));

            List<string> line = new List<string>();
            string cols = getColumnNames(t);
            Print(cols);

            foreach (DataRow row in t.Rows)
            {
                dump(row);
            }
        }

        public string getColumnNames(DataTable t)
        {
            ColumnNames.Clear();
            foreach (DataColumn col in t.Columns)
            {
                ColumnNames.Add(col.ColumnName);
            }
            return string.Join<string>(",", ColumnNames);
        }

        public void dump(DataRow row)
        {
            if (row.RowState != DataRowState.Deleted)
            {
                List<string> foo = new List<string>();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    foo.Add(String.Format("{0}", row[i].ToString()));
                }

                Print(string.Join<string>(",", foo));
                foo.Clear();
            }
        }

        public void dump(DataView v)
        {
            dump(v.ToTable());
        }


        public static void Print()
        {
            switch (vector)
            {
                case OutputVector.Console:
                    Console.WriteLine();
                    break;
                case OutputVector.Debug:
                    System.Diagnostics.Debug.WriteLine(Environment.NewLine);
                    break;

            }
        }

        public static void Print(string s)
        {
            switch (vector)
            {
                case OutputVector.Console:
                    Console.WriteLine(s);
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
