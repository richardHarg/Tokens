using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLH.Tokens.ASPNETCore
{
    /// <summary>
    /// ASPNETCore version of 'TokenService' injecting TokenConfig via Options pattern
    /// </summary>
    public class OptionsTokenService : TokenService
    {
        public OptionsTokenService(IOptions<TokenConfig> config) : base(config.Value)
        {

        }
    }
}
