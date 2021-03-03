using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;
        private readonly IUserDAO userDAO;
        private readonly ITransferDAO transferDAO;

        public TransferController(IAccountDAO _accountDAO, IUserDAO _userDAO, ITransferDAO _transferDAO)
        {
            accountDAO = _accountDAO;
            userDAO = _userDAO;
            transferDAO = _transferDAO;
        }

        [HttpGet("users")]
        public List<User> ListUsers()
        {
            return userDAO.GetUsers();
        }
    }
}
