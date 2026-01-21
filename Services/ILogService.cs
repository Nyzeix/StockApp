using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Services
{ }
public interface ILogService
{
    void LogInfo(string tag, string message);
    void LogWarning(string tag, string message);
    void LogError(string tag,string message, Exception ex);
}

