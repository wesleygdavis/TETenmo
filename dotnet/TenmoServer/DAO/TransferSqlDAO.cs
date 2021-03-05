﻿using System;
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

        public bool CreateTransfer(CreateTransfer transfer, int transferType, int status)
        {
            bool output = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (@transfertype, @status, @accountFrom, @accountTo, @amount);", conn);
                    cmd.Parameters.AddWithValue("@transfertype", transferType);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);
                    cmd.Parameters.AddWithValue("@accountFrom", GetAccountNumber(transfer.AccountFrom));
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

        public List<Transfer> GetTransferForUser(int userId, int type)
        {
            List<Transfer> transfers = new List<Transfer>();
            string sqlCondition = "(t.transfer_status_id <> 1)";
            if (type == 2)
            {
                sqlCondition = "(t.transfer_status_id = 1)";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    string sql = "SELECT transfer_id, tt.transfer_type_desc, ts.transfer_status_desc, u.username AS account_from_name, u2.username AS account_to_username, t.amount"
                        + " FROM transfers t"
                        + " JOIN accounts a ON t.account_from = a.account_id"
                        + " JOIN users u ON a.user_id = u.user_id"
                        + " JOIN accounts a2 ON t.account_to = a2.account_id"
                        + " JOIN users u2 ON a2.user_id = u2.user_id"
                        + " JOIN transfer_statuses ts ON t.transfer_status_id = ts.transfer_status_id"
                        + " JOIN transfer_types tt ON t.transfer_type_id = tt.transfer_type_id"
                        + $" WHERE (u.user_id = @userId OR u2.user_id = @userId) AND {sqlCondition};";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        Transfer transfer = GenerateTransferFromReader(reader);
                        transfers.Add(transfer);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return transfers;
        }

        private Transfer GenerateTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferType = Convert.ToString(reader["transfer_type_desc"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),
                AccountFromUserName = Convert.ToString(reader["account_from_name"]),
                AccountToUserName = Convert.ToString(reader["account_to_username"]),
                Amount = Convert.ToDecimal(reader["amount"])

            };

            

            return t;
        }

    }
}
