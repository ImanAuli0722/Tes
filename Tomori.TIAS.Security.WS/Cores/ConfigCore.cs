using System;
using System.Reflection;
using Tomori.TIAS.Security.WS.Helpers;
using Tomori.TIAS.Security.WS.Repositories;

namespace Tomori.TIAS.Security.WS.Cores
{
    public class ConfigCore
    {
        private readonly ConfigRepo configRepo = new ConfigRepo();

        public string GetValue(string key)
        {
            try
            {
                return configRepo.GetValue(key);
            }
            catch (Exception ex)
            {
                LogHelper.SaveError(MethodBase.GetCurrentMethod(), ex.Message);
                return null;
            }
        }
    }
}