using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {

        bool ReduceBalance(decimal amount, int userId);
        bool IncreaseBalance(decimal amount, int userId);
        bool CreateTransfer(CreateTransfer createTransfer);
        
    }
}
