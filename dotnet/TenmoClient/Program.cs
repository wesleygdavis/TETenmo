using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountAPIService accountService = new AccountAPIService();
        private static readonly TransferAPIServices transferService = new TransferAPIServices();
        

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.Clear();
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in. Press any key to continue.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                            Console.ReadLine();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection. Press any key to return to login.");
                    Console.ReadLine();
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            string token = UserService.GetToken();
            while (menuSelection != 0)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    consoleService.printBalance(accountService.GetBalance(token));
                    Console.ReadLine();
                }
                else if (menuSelection == 2)
                {
                    
                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    Console.Clear();
                    List<User> userList = transferService.GetUsers(token);
                    consoleService.printUserList(userList);
                    int transferUserId = consoleService.PromptForTransferID("transfer", userList);
                    decimal transferAmount = consoleService.PromptForTransferAmount("transfer");
                    API_Transfer transfer = transferService.CreateTransfer(transferUserId, transferAmount, UserService.GetUserId());
                    string responseMessage = transferService.Transfer(transfer,token);
                    consoleService.PromptPrintMessage(responseMessage);
                    Console.ReadLine();
                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                    
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
