using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ILoggerService
    {
        void LogError(string errorMessage);

        void LogError(string errorMessage, params object[] args);

        void LogException(Exception ex);

        void LogInfo(string infoMessage);

        void LogInfo(string infoMessage, params object[] args);
    }
}
