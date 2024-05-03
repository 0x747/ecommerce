using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Http;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace API.Models {

    /// <summary>
    /// Includes method to perform CRUD operations on the Vendors table. 
    /// </summary>
    public class VendorOperations {

        /// <summary>
        /// Creates a connection to the SQL Database. 
        /// </summary>
        private SqlConnection Connection;
        private void CreateConnection() {
            string ConnectionString = ConfigurationManager.ConnectionStrings["EcomDbConnectionString"].ToString();
            Connection = new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Inserts a new vendor into the Vendors table.
        /// </summary>
        /// <param name="VendorModel">The vendor model with Name, Email, Password, CountryCode, ContactNumber, Address, Country, State, and City</param>
        /// <returns>An HttpResponseMessage.</returns>
        public HttpResponseMessage AddVendor(Vendor VendorModel) {
            HttpResponseMessage responseMessage;
            CreateConnection();
            SqlCommand insertCommand = new SqlCommand("AddVendor", Connection);

            string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(VendorModel.Password, 13);

            insertCommand.CommandType = CommandType.StoredProcedure;
            insertCommand.Parameters.AddWithValue("@Name", VendorModel.Name);
            insertCommand.Parameters.AddWithValue("@Email", VendorModel.Email);
            insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
            insertCommand.Parameters.AddWithValue("@CountryCode", VendorModel.CountryCode);
            insertCommand.Parameters.AddWithValue("@ContactNumber", VendorModel.ContactNumber);
            insertCommand.Parameters.AddWithValue("@Address", VendorModel.Address);
            insertCommand.Parameters.AddWithValue("@Country", VendorModel.Country);
            insertCommand.Parameters.AddWithValue("@State", VendorModel.State);
            insertCommand.Parameters.AddWithValue("@City", VendorModel.City);

            int result = 0;

            Connection.Open();

            try {
                result = insertCommand.ExecuteNonQuery();
            }
            catch (SqlException e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            Connection.Close();

            responseMessage = result == 1 ? new HttpResponseMessage(System.Net.HttpStatusCode.Created) : new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

            return responseMessage;
        }

        /// <summary>
        /// Returns a list of all vendors.
        /// </summary>
        /// <returns>A List of all vendors.</returns>
        public IEnumerable<Vendor> GetAllVendors() {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetAllVendors", Connection);

            return GenerateVendorList(selectCommand, Connection);
        }

        /// <summary>
        /// Returns all vendors with the matching name.
        /// </summary>
        /// <param name="vendorName">Name of the vendors.</param>
        /// <param name="IsExactMatch">Specifies an exact match.</param>
        /// <returns>A List of all vendors with the matching name.</returns>
        public IEnumerable<Vendor> GetVendorByName(string vendorName, bool IsExactMatch) {
            CreateConnection();

            SqlCommand selectCommand = new SqlCommand("GetVendorByName", Connection);
            
            selectCommand.Parameters.AddWithValue("@Name", vendorName = IsExactMatch ? vendorName : $"%{vendorName.Replace(' ', '%')}%");
            selectCommand.Parameters.AddWithValue("@IsExactMatch", IsExactMatch);

            return GenerateVendorList(selectCommand, Connection);
        }

        /// <summary>
        /// Helper to execute select commands and generate a list of vendors.
        /// </summary>
        /// <param name="selectCommand">The SQL command specifying the select stored procedure and connection instance.</param>
        /// <returns>A List of vendors.</returns>
        public IEnumerable<Vendor> GenerateVendorList(SqlCommand selectCommand, SqlConnection Connection) {
            
            List<Vendor> VendorList = new List<Vendor>();

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

                VendorList.Add(
                    new Vendor
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString(),
                        Email = row["Email"].ToString(),
                        Password = row["Password"].ToString(),
                        CountryCode = row["CountryCode"].ToString(),
                        ContactNumber = row["ContactNumber"].ToString(),
                        Address = row["Address"].ToString(),
                        Country = row["Address"].ToString(),
                        State = row["State"].ToString(),
                        City = row["City"].ToString(),
                        DateAdded = DateTime.Parse(row["DateAdded"].ToString()),
                        DateModified = string.IsNullOrWhiteSpace(tempDateModified) ? (DateTime?)null : DateTime.Parse(tempDateModified)
                    }
                );
            }

            return VendorList;
        }

        
    }
}
