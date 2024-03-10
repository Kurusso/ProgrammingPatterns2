using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Interfaces
{
    public interface IHasUserId
    {
        Guid UserId { get; set; }
    }
}
