using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using System.Configuration;


namespace Shareable_data_generator
{
    public static partial class Functions
    {

        public static object GetPropertyValue(this object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return null;

            var methods = name.Split('.');

            object current = obj;
            object result = null;
            foreach (var method in methods)
            {
                var prop = current?.GetType().GetProperty(method, BindingFlags.Public
                                                                | BindingFlags.NonPublic
                                                                | BindingFlags.Instance
                                                                | BindingFlags.GetProperty);
                result = prop != null ? prop.GetValue(current, null) : null;
                current = result;
            }
            return result;
        }


        public static bool isNumber(object p_Value)
        {
            try
            {
                if (double.Parse(p_Value.ToString()).GetType().Equals(typeof(double)))
                    return true;
                else
                    return false;
            }
            catch 
            {
                return false;
            }
        }
        //<StackPanel HorizontalAlignment="Left" VerticalAlignment="Top">
        //</StackPanel>
        public static List<string> DataList(string SQLstring, string ConnectionString)
        {
            List<string> list = new List<string>();

            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();

            string query = SQLstring;
            SqlCommand cmd = new SqlCommand(query, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }

            conn.Close();

            return list;
        }

        public static List<string> GetValuesFromDB(string SQLstring, string ConnectionString)
        {
            List<string> ValuesFromDB = new List<string>();
            string LocationqueryString = SQLstring;
            using (SqlConnection Locationconnection =
                                     new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(LocationqueryString, Locationconnection);
                try
                {
                    Locationconnection.Open();
                    SqlDataReader Locationreader = command.ExecuteReader();
                    while (Locationreader.Read())
                    {
                        for (int i = 0; i <= Locationreader.FieldCount - 1; i++)
                        {
                            ValuesFromDB.Add(Locationreader[i].ToString());
                        }
                    }
                    Locationreader.Close();
                    return ValuesFromDB;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

        }










    }



}

