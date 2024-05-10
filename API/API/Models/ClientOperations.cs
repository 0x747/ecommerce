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

            string City = string.IsNullOrEmpty(ClientModel.City) ? "" : ClientModel.City;
            string State = string.IsNullOrEmpty(ClientModel.State) ? "" : ClientModel.State;

            int result = 0;
            Connection.Open();

            try {
                insertCommand.CommandType = CommandType.StoredProcedure;
                insertCommand.Parameters.AddWithValue("@Name", ClientModel.Name);
                insertCommand.Parameters.AddWithValue("@Email", ClientModel.Email);
                insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                insertCommand.Parameters.AddWithValue("@CountryCode", ClientModel.CountryCode);
                insertCommand.Parameters.AddWithValue("@ContactNumber", ClientModel.ContactNumber);
                insertCommand.Parameters.AddWithValue("@Address", ClientModel.Address);
                insertCommand.Parameters.AddWithValue("@Country", ClientModel.Country);
                insertCommand.Parameters.AddWithValue("@State", State);
                insertCommand.Parameters.AddWithValue("@City", City);
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

        public bool AuthenticateCredentialsById(int Id, string Password)
        {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetClientPasswordById", Connection);
            selectCommand.CommandType = CommandType.StoredProcedure;

            selectCommand.Parameters.AddWithValue("@Id", Id);

            SqlParameter returnValue = new SqlParameter("@Password", SqlDbType.NVarChar, 64);
            returnValue.Direction = ParameterDirection.Output;
            selectCommand.Parameters.Add(returnValue);

            Connection.Open();
            selectCommand.ExecuteNonQuery();
            Connection.Close();

            string PasswordHash = returnValue.Value.ToString();

            if (!string.IsNullOrEmpty(PasswordHash))
            {
                return BCrypt.Net.BCrypt.EnhancedVerify(Password, PasswordHash);
            }
            else
            {
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

        public bool UpdateClientPersonalDetails(Client clientModel)
        {
            CreateConnection();
            SqlCommand updateCommand = new SqlCommand("UpdateClientPersonalDetails", Connection);
            updateCommand.CommandType = CommandType.StoredProcedure;
            int result = 0;
            try
            {
                updateCommand.Parameters.AddWithValue("@Id", clientModel.Id);
                updateCommand.Parameters.AddWithValue("@Name", clientModel.Name);
                updateCommand.Parameters.AddWithValue("@Address", clientModel.Address);
                updateCommand.Parameters.AddWithValue("@Country", clientModel.Country);
                updateCommand.Parameters.AddWithValue("@State", clientModel.State);
                updateCommand.Parameters.AddWithValue("@City", clientModel.City);
                updateCommand.Parameters.AddWithValue("@CountryCode", clientModel.CountryCode);
                updateCommand.Parameters.AddWithValue("@ContactNumber", clientModel.ContactNumber);

                Connection.Open();
                result = updateCommand.ExecuteNonQuery();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Update Client Details]:", ex.Message);
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

        public bool UpdateClientEmail(Client ClientModel)
        {
            if (!AuthenticateCredentialsById(ClientModel.Id, ClientModel.Password))
            {
                return false;
            }

            CreateConnection();
            SqlCommand updateCommand = new SqlCommand("UpdateClientEmail", Connection);
            updateCommand.CommandType = CommandType.StoredProcedure;
            int result = 0;

            try
            {
                updateCommand.Parameters.AddWithValue("@Email", ClientModel.Email);
                updateCommand.Parameters.AddWithValue("@Id", ClientModel.Id);

                Connection.Open();
                result = updateCommand.ExecuteNonQuery();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Update Email]:", ex.Message);
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

        public bool UpdateClientPassword(Client ClientModel)
        {
            if (!AuthenticateCredentialsById(ClientModel.Id, ClientModel.Password))
            {
                return false;
            }

            CreateConnection();
            SqlCommand updatePasswordCommand = new SqlCommand("UpdateClientPassword", Connection);
            updatePasswordCommand.CommandType = CommandType.StoredProcedure;

            int result = 0;

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(ClientModel.NewPassword, 13);

                updatePasswordCommand.Parameters.AddWithValue("@Id", ClientModel.Id);
                updatePasswordCommand.Parameters.AddWithValue("@NewPassword", hashedPassword);

                Connection.Open();
                result = updatePasswordCommand.ExecuteNonQuery();
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Update Password]:", ex.Message);
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