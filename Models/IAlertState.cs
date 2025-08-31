using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesDiscounts.Models
{
    public interface IAlertState
    {
        Task SaveAsync(AlertSettings settings);
        Task<AlertSettings?> GetAsync(long chatId);
        Task<List<AlertSettings>> GetAllEnabledAsync();
        Task DisableAsync(long chatId);
    }
}
