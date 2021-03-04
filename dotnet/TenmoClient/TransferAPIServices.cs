using TenmoClient.Data;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;


namespace TenmoClient
{
    class TransferAPIServices
    {

        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string TRANSFER_URL = API_BASE_URL + "transfer";
        private readonly RestClient client = new RestClient();

        public List<User> GetUsers(string token)
        {
            List<User> output = new List<User>();
            RestRequest request = new RestRequest(TRANSFER_URL + "/users");
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                ProcessErrorResponse(response);
            }
            else if (!response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                output = response.Data;
            }

            return output;
        }

        public void ProcessErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error occurred - unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Please Login");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("You do not have the correct permission for the requested action");
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public API_Transfer CreateTransfer( int accountTo, decimal amount, int userId)
        {
            API_Transfer transfer = new API_Transfer()
            { AccountTo = accountTo, Amount = amount, UserId = userId };

            return transfer;
        }

        public string Transfer(API_Transfer transfer, string token)
        {
            RestRequest request = new RestRequest(TRANSFER_URL);
            request.AddJsonBody(transfer);
            client.Authenticator = new JwtAuthenticator(token);
            IRestResponse response = client.Post(request);

            return response.Content;


        }
    }
}
