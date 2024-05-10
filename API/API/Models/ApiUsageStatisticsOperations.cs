using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using System.Diagnostics;

namespace API.Models
{
    public class ApiUsageStatisticsOperations
    {
        private SqlConnection Connection;
        private void CreateConnection()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["EcomDbConnectionString"].ToString();
            Connection = new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Non-essential method. Fulfill API request even if this returns false.
        /// </summary>
        /// <param name="UsageModel"></param>
        /// <returns>A boolean</returns>
        public bool AddApiUsageStatistic(ApiUsageStatistic UsageModel, Stopwatch timer)
        {
            CreateConnection();
            int result = 0;
            SqlCommand insertStatCommand = new SqlCommand("AddApiUsageStatistic", Connection);
            insertStatCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                insertStatCommand.Parameters.AddWithValue("@Method", UsageModel.Method);
                insertStatCommand.Parameters.AddWithValue("@Route", UsageModel.Route);
                timer.Stop();
                insertStatCommand.Parameters.AddWithValue("@ResponseTime", timer.Elapsed);

                Connection.Open();
                result = insertStatCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Add Usage Stat]:", ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            if (result == 1)
            {
                return true;
            }

            return false;
        }
    }
}