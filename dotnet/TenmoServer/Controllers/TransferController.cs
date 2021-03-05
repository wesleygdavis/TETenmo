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
        public List<GetUser> ListUsers()
        {
            string username = User.Identity.Name;
            return userDAO.GetUserList(username);
        }

        [HttpPost]
        public ActionResult CreateTransfer(CreateTransfer transfer)
        {
            decimal accountBalance = accountDAO.GetBalance(transfer.AccountFrom);

            if(transfer.AccountFrom == transfer.AccountTo)
            {
                return BadRequest("Invalid recipient. Cannot send money to yourself.");
            }
            else if (transfer.Amount <= accountBalance)
            {
                bool reduceSuccess = transferDAO.ReduceBalance(transfer.Amount, transfer.AccountFrom);
                if (!reduceSuccess)
                {
                    return StatusCode(500, "Unable to withdraw funds / server issue.");
                }
                bool increaseSuccess = transferDAO.IncreaseBalance(transfer.Amount, transfer.AccountTo);
                if (!increaseSuccess)
                {
                    return StatusCode(500, "Unable to add funds / server issue.");
                
                }
                bool createTransferSuccess = transferDAO.CreateTransfer(transfer, 2, 2);
                if (!createTransferSuccess)
                {
                    return StatusCode(500, "Unable to record transaction / server issue.");
                }

                return Ok("Transfer successful.");
            }
            else 
            {
                return BadRequest("Insufficient funds.");
            
            }   
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetTransferList()
        {
            User user = userDAO.GetUser(User.Identity.Name);
            List<Transfer> transferList = transferDAO.GetTransferForUser(user.UserId, 1);
            return Ok(transferList);
        }

        [HttpPost("request")]
        public ActionResult MakeRequest(CreateTransfer transfer)
        {
            if (transfer.AccountFrom == transfer.AccountTo)
            {
                return BadRequest("Invalid recipient. Cannot request money from yourself.");
            }
            else
            {
                bool createTransferSuccess = transferDAO.CreateTransfer(transfer, 1, 1);
                if (!createTransferSuccess)
                {
                    return StatusCode(500, "Unable to record request / server issue.");
                }

                return Ok("Request successful.");
            }
        }
    }
}
