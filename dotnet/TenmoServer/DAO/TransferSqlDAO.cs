using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool ReduceBalance(decimal amount, int userId)
        {
            
            bool output = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = balance - @amount WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                    output = true;
                  
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return output;
        
        }

        public bool IncreaseBalance(decimal amount, int userId)
        {
            bool output = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = balance + @amount WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                    output = true;

                }
            }
            catch (SqlException)
            {
                throw;
            }

            return output;

        }

        public bool CreateTransfer(CreateTransfer transfer)
        {
            bool output = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (2, 2, @accountFrom, @accountTo, @amount);", conn);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);
                    cmd.Parameters.AddWithValue("@accountFrom", GetAccountNumber(transfer.UserId));
                    cmd.Parameters.AddWithValue("@accountTo", GetAccountNumber(transfer.AccountTo));
                    cmd.ExecuteNonQuery();

                    output = true;
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return output;
        }

        private int GetAccountNumber(int userId)
        {
            int output = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT account_id FROM accounts WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        output = Convert.ToInt32(reader["account_id"]);
                    }
                    
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return output;

        }

    }
}
