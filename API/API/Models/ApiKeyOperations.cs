using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using System.Net.Http;
using System.Drawing.Text;
using System.Security.Cryptography;

namespace API.Models {
    public class ApiKeyOperations {

        /// <summary>
        /// Creates a connection to the SQL Database. 
        /// </summary>
        private SqlConnection Connection;
        private const int _lengthOfKey = 64;

        public string GenerateApiKey() {

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[_lengthOfKey];
            rng.GetBytes(bytes);

            string base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");

            return base64String;
        }
        private void CreateConnection() {
            string ConnectionString = ConfigurationManager.ConnectionStrings["EcomDbConnectionString"].ToString();
            Connection = new SqlConnection(ConnectionString);
        }

        public HttpResponseMessage CreateApiKey(int ClientId) {
            CreateConnection();
            HttpResponseMessage responseMessage;

            SqlCommand insertCommand = new SqlCommand("AddApiKey", Connection);
            insertCommand.CommandType = CommandType.StoredProcedure;

            insertCommand.Parameters.AddWithValue("@ClientId", ClientId);
            insertCommand.Parameters.AddWithValue("@ApiKey", GenerateApiKey());

            int result = 0;

            Connection.Open();

            try {
                result = insertCommand.ExecuteNonQuery();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("[API Generation Error]:", ex.Message);
            }

            Connection.Close();

            if (result == 1) {
                responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }
            else {
                responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                responseMessage.ReasonPhrase = "Error creating API Key";
            }

            return responseMessage;
        }

        public bool ValidateKey(string key) {
            CreateConnection();
            
            SqlCommand selectCommand = new SqlCommand("GetApiKey", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            selectCommand.Parameters.AddWithValue("@ApiKey", key);
            SqlParameter returnValue = new SqlParameter("@KeyCount", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            int result = 0;

            Connection.Open();

            try {
                selectCommand.ExecuteNonQuery();
                result = Convert.ToInt32(returnValue.Value);
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            Connection.Close();

            if (result == 1) {
                return true;
            }
            else { return false; }
        }

        public IEnumerable<ApiKey> GetApiKeyByClientId(int ClientId) {
            CreateConnection();
            List<ApiKey> apiKey = new List<ApiKey>();

            SqlCommand selectCommand = new SqlCommand("GetApiKeyInfo", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            selectCommand.Parameters.AddWithValue("@ClientId", ClientId);
            DataTable dataTable = new DataTable(); 
            SqlDataAdapter sqlDataAdapter  = new SqlDataAdapter(selectCommand);

            Connection.Open();
            sqlDataAdapter.Fill(dataTable);
            Connection.Close();

            foreach(DataRow row in  dataTable.Rows) {
                apiKey.Add(
                    new ApiKey
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Key = row["ApiKey"].ToString(),
                        ClientId = Convert.ToInt32(row["ClientId"]),
                        AmountOfRequests = Convert.ToInt32(row["AmountOfRequests"]),
                        DateCreated = DateTime.Parse(row["DateCreated"].ToString()),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        Limit = Convert.ToInt32(row["Limit"])
                    }
                );
            }

            return apiKey;
        }

        public int GetTotalApiRequests() { 
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetTotalApiRequests", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter returnValue = new SqlParameter("@TotalRequests", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            Connection.Open();
            selectCommand.ExecuteNonQuery();
            Connection.Close();

            if (returnValue.Value != System.DBNull.Value)
            {
                return Convert.ToInt32(returnValue.Value);
            }
            return 0;
        }

        public bool GetApiKeyStatus(string ApiKey)
        {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetApiKeyStatus", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            selectCommand.Parameters.AddWithValue("@ApiKey", ApiKey);

            SqlParameter returnValue = new SqlParameter("@Status", SqlDbType.Bit);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            Connection.Open();
            selectCommand.ExecuteNonQuery();
            Connection.Close();
            
            if (returnValue.Value != System.DBNull.Value)
            {
                return Convert.ToBoolean(returnValue.Value);
            }

            return false;
        }

        public bool IncrementRequestCount(string ApiKey)
        {
            CreateConnection();

            SqlCommand updateCommand = new SqlCommand("IncrementRequestCount", Connection);
            updateCommand.CommandType = CommandType.StoredProcedure;

            updateCommand.Parameters.AddWithValue("@ApiKey", ApiKey);

            Connection.Open();
            int result = 0;
            try
            {
                result = updateCommand.ExecuteNonQuery();
            } catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Increment Count]:", ex.Message);
            }

            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public bool DisableApiKey(Client ClientModel)
        {
            if (!(new ClientOperations().AuthenticateCredentialsById(ClientModel.Id, ClientModel.Password))) {
                return false;
            }

            CreateConnection();
            int result = 0;
            SqlCommand DisableKeyCommand = new SqlCommand("DisableApiKey", Connection);
            DisableKeyCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                DisableKeyCommand.Parameters.AddWithValue("@ClientId", ClientModel.Id);

                Connection.Open();
                result = DisableKeyCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Disable API Key]:", ex.Message);
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

        public bool EnableApiKey(Client ClientModel)
        {
            if (!(new ClientOperations().AuthenticateCredentialsById(ClientModel.Id, ClientModel.Password)))
            {
                return false;
            }

            CreateConnection();
            int result = 0;
            SqlCommand EnableKeyCommand = new SqlCommand("EnableApiKey", Connection);
            EnableKeyCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                EnableKeyCommand.Parameters.AddWithValue("@ClientId", ClientModel.Id);

                Connection.Open();
                result = EnableKeyCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Enable API Key]:", ex.Message);
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