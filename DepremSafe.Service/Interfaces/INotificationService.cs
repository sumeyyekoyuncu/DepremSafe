using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepremSafe.Core.DTOs;

namespace DepremSafe.Core.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendEarthquakeAlert(List<string> tokens, EarthquakeData earthquakeData);
        Task<bool> SendHelpRequest(List<string> nearbyUserTokens, HelpNotificationRequest data);
        Task<bool> SendEmergencyUpdate(List<string> tokens, string title, string body);
        Task<bool> SendTestNotification(string token, string title, string body, string type);
        Task<int> SendToMultipleDevices(List<string> tokens, Dictionary<string, string> data);
    }
}
