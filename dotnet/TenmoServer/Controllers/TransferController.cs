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
            return userDAO.GetUserList();
        }

       [HttpPost]
       public ActionResult CreateTransfer(CreateTransfer transfer)
        {
            decimal accountBalance = accountDAO.GetBalance(transfer.UserId);
            if(transfer.UserId == transfer.AccountTo)
            {
                return BadRequest("Invalid recipient.");
            }
            else if (transfer.Amount <= accountBalance)
            {
                bool reduceSuccess = transferDAO.ReduceBalance(transfer.Amount, transfer.UserId);
                if (!reduceSuccess)
                {
                    return StatusCode(500, "Unable to withdraw funds / server issue.");
                }
                bool increaseSuccess = transferDAO.IncreaseBalance(transfer.Amount, transfer.AccountTo);
                if (!increaseSuccess)
                {
                    return StatusCode(500, "Unable to add funds / server issue.");
                
                }
                bool createTransferSuccess = transferDAO.CreateTransfer(transfer);
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
       
        
    }
}
