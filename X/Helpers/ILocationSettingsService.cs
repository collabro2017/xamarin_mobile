using System;
using System.Threading.Tasks;

namespace X.Helpers
{
    public interface ILocationSettingsService
    {
        Task<String> OpenLocationSettings();
    }
}
