using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepremSafe.Service.Interfaces
{
    public interface IAiService
    {
        Task<string> GenerateCalmMessageAsync(bool isSafe);
    }
}
