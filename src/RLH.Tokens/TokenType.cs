using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLH.Tokens
{
    /// <summary>
    /// Enum used to set the 'Type' of token to create/validate
    /// </summary>
    public enum TokenType
    {
        JWT,
        PASSWORD_RESET,
        CONFIRM_ACCOUNT
    }
}
