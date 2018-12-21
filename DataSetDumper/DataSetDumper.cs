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

        public DataSetDumper(DataSet d)
        {
            vector = OutputVector.Debug;
            this.ds = new DataSet();
            ds.DataSetName = d.DataSetName;
            foreach(DataTable dt in ds.Tables)
            {
                ds.Tables.Add(dt.Copy());
            }
        }

        #region System.Diagnosistics.Debug.WriteLine 

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

        #endregion

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


        #region class methods
        private void dumpDataView(DataView dv)
        {
            dumpDataSet(dv.Table);
        }

        public void dumpDataSet(DataSet ds, int table, string filePath)
        {
            try
            {
                dumpDataTable(ds.Tables[table], filePath);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine(x.StackTrace);
            }
        }

        public void dumpDataTable(DataTable dt, string filePath)
        {
            dumpDataTable(dt, filePath, false);
        }

        public void dumpDataTable(DataTable dt, string filePath, Boolean append)
        {
            try
            {
                if ((!append) && File.Exists(filePath))
                    File.Delete(filePath);

                List<string> colnames = new List<string>();
                foreach (DataColumn col in dt.Columns)
                {
                    colnames.Add(col.ColumnName);
                }

                using (StreamWriter writer = new StreamWriter(filePath, append))
                {
                    if (!append)
                        writer.WriteLine(string.Join<string>(",", colnames));

                    foreach (DataRow dr in dt.Rows)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sb.Append(DataSetDumper.FormatCSV(dr[dc.ColumnName].ToString()) + ",");
                        }
                        sb.Remove(sb.Length - 1, 1);
                        writer.WriteLine(sb.ToString());
                    }
                }

                //			foreach (DataRow row in dt.Rows)
                //			{
                //				if (row.RowState != DataRowState.Deleted)
                //				{
                //					List<string> foo = new List<string>();
                //					foreach (string col in colnames)
                //					{
                //						foo.Add(String.Format("{0}", row[col].ToString()));
                //					}
                //	
                //					Console.WriteLine(string.Join<string>(",", foo));
                //					foo.Clear();
                //				}
                //			}
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine(x.StackTrace);
            }
        }
        
        private void dumpDataTable(DataTable dt)
        {
            try
            {
                List<string> line = new List<string>();
                List<string> colnames = new List<string>();
                foreach (DataColumn col in dt.Columns)
                {
                    colnames.Add(col.ColumnName);
                }

                line.Add(string.Join<string>(",", colnames));
                Console.WriteLine(dt.TableName);
                Console.WriteLine(string.Join<string>(",", colnames));
                foreach (DataRow row in dt.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        List<string> foo = new List<string>();
                        foreach (string col in colnames)
                        {
                            foo.Add(String.Format("{0}", row[col].ToString()));
                        }

                        Console.WriteLine(string.Join<string>(",", foo));
                        foo.Clear();
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                Console.WriteLine(x.StackTrace);
            }
        }

        private void dumpDataSet(DataTable dt)
        {
            dumpDataTable(dt);
        }

        #endregion
    }
}
