using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Data;

namespace API.Models {
    public class ClientOperations {

        private SqlConnection Connection;
        private void CreateConnection() {
            string ConnectionString = ConfigurationManager.ConnectionStrings["EcomDbConnectionString"].ToString();
            Connection = new SqlConnection(ConnectionString);
        }

        public HttpResponseMessage AddClient(Client ClientModel) {
            CreateConnection();

            HttpResponseMessage responseMessage;
            SqlCommand insertCommand = new SqlCommand("AddClient", Connection);
            insertCommand.CommandType = CommandType.StoredProcedure;

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(ClientModel.Password, 13);

            insertCommand.CommandType = CommandType.StoredProcedure;
            insertCommand.Parameters.AddWithValue("@Name", ClientModel.Name);
            insertCommand.Parameters.AddWithValue("@Email", ClientModel.Email);
            insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
            insertCommand.Parameters.AddWithValue("@CountryCode", ClientModel.CountryCode);
            insertCommand.Parameters.AddWithValue("@ContactNumber", ClientModel.ContactNumber);
            insertCommand.Parameters.AddWithValue("@Address", ClientModel.Address);
            insertCommand.Parameters.AddWithValue("@Country", ClientModel.Country);
            insertCommand.Parameters.AddWithValue("@State", ClientModel.State);
            insertCommand.Parameters.AddWithValue("@City", ClientModel.City);

            int result = 0;

            Connection.Open();

            try {
                result = insertCommand.ExecuteNonQuery();
            }
            catch (SqlException e) {
                System.Diagnostics.Debug.WriteLine("[Add Client Error]:", e.Message);
            }

            Connection.Close();

            if (result == 1) {
                //responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.Created);
                int ClientId = GetClientByEmail(ClientModel.Email).First().Id;
                responseMessage = new ApiKeyOperations().CreateApiKey(ClientId);
            } else {
                responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }

            return responseMessage;
        }

        public bool AuthenticateCredentials(string Email, string Password) {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetClientPasswordByEmail", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            selectCommand.Parameters.AddWithValue("@Email", Email);

            SqlParameter returnValue = new SqlParameter("@Password", SqlDbType.NVarChar, 64);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            Connection.Open();
            selectCommand.ExecuteNonQuery();
            Connection.Close();

            string PasswordHash = returnValue.Value.ToString();

            if (!string.IsNullOrEmpty(PasswordHash)) {
                return BCrypt.Net.BCrypt.EnhancedVerify(Password, PasswordHash);
            }
            else {
                return false;
            }
        }

        public int GetTotalClients() {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetTotalClients", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter returnValue = new SqlParameter("@TotalClients", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            Connection.Open();
            selectCommand.ExecuteNonQuery();
            Connection.Close();

            return Convert.ToInt32(returnValue.Value);
        }

        public int GetTotalDistinctCountries() {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetDistinctCountriesCount", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            SqlParameter returnValue = new SqlParameter("@TotalDistinctCountries", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            Connection.Open();
            selectCommand.ExecuteNonQuery();
            Connection.Close();

            return Convert.ToInt32(returnValue.Value);
        }

        public IEnumerable<Client> GetClientByEmail(string Email) {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetClientByEmail", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            selectCommand.Parameters.AddWithValue("@Email", Email);

            return GenerateClientList(selectCommand, Connection);
        }

        public IEnumerable<Client> GenerateClientList(SqlCommand selectCommand, SqlConnection Connection) {

            List<Client> ClientList = new List<Client>();

            selectCommand.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter dataAdapter = new SqlDataAdapter(selectCommand);
            DataTable dataTable = new DataTable();

            Connection.Open();

            try {
                dataAdapter.Fill(dataTable);
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
            }

            Connection.Close();


            foreach (DataRow row in dataTable.Rows) {
                string tempDateModified = row["DateModified"].ToString();

                ClientList.Add(
                    new Client
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString(),
                        Email = row["Email"].ToString(),
                        Password = row["Password"].ToString(),
                        CountryCode = row["CountryCode"].ToString(),
                        ContactNumber = row["ContactNumber"].ToString(),
                        Address = row["Address"].ToString(),
                        Country = row["Country"].ToString(),
                        State = row["State"].ToString(),
                        City = row["City"].ToString(),
                        DateAdded = DateTime.Parse(row["DateAdded"].ToString()),
                        DateModified = string.IsNullOrWhiteSpace(tempDateModified) ? (DateTime?)null : DateTime.Parse(tempDateModified)
                    }
                );
            }

            return ClientList;
        }
    }
}