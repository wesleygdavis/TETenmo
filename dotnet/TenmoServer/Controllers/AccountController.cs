using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;

        public AccountController(IAccountDAO _accountDAO, IUserDAO _userDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
        }
               
        [HttpGet("balance")]
        public decimal GetBalance()
        {
            User user = userDAO.GetUser(User.Identity.Name);
            return accountDAO.GetBalance(user.UserId);
        }
    }
}
