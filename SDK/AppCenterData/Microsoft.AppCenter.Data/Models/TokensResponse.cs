using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Tokens response.
    /// </summary>
    public class TokensResponse
    {
        /// <summary>
        /// The m tokens.
        /// </summary>
        private List<TokenResult> Tokens;

         /// <summary>
         /// Gets the tokens.
         /// </summary>
         /// <returns>The tokens.</returns>
        public List<TokenResult> GetTokens()
        {
            return Tokens;
        }
    }
}