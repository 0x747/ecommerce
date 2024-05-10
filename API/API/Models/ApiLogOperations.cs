using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace API.Models
{
    public class ApiLogOperations
    {
        private SqlConnection Connection;
        private void CreateConnection()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["EcomDbConnectionString"].ToString();
            Connection = new SqlConnection(ConnectionString);
        }

        public bool AddApiRequest(string ApiKey, string Method, string Route)
        {
            CreateConnection();
            SqlCommand insertCommand = new SqlCommand("AddApiRequest", Connection);
            insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
            int result = 0;
            try
            {
                insertCommand.Parameters.AddWithValue("@ApiKey", ApiKey);
                insertCommand.Parameters.AddWithValue("@Method", Method);
                insertCommand.Parameters.AddWithValue("@Route", Route);

                result = 0;
                Connection.Open();
                result = insertCommand.ExecuteNonQuery();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            } finally
            {
                Connection.Close();
            }

            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public IEnumerable<ApiLog> GetApiLogsByClientEmail(string Email)
        {
            List<ApiLog> apiLogs = new List<ApiLog>();
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetApiLogsByClientEmail", Connection);
            selectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommand);

            try
            {
                selectCommand.Parameters.AddWithValue("@Email", Email);

                Connection.Open();
                dataAdapter.Fill(dataTable);
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Get Log by Email]:", ex.Message);
            }
            finally
            {
                Connection.Close();

                foreach(DataRow row in dataTable.Rows)
                {
                    apiLogs.Add(
                        new ApiLog
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            ClientId = Convert.ToInt32(row["ClientId"]),
                            Method = row["Method"].ToString(),
                            Route = row["Route"].ToString(),
                            Timestamp = DateTime.Parse(row["Timestamp"].ToString())
                        }
                    );
                }
            }

            return apiLogs;
        }
    }
}