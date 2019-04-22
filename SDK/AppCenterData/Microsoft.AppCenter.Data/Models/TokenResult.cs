using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Token result.
    /// </summary>
    public class TokenResult
    {
        /// <summary>
        /// The partition property.
        /// </summary>
        private String Partition;

        /// <summary>
        /// Cosmos db account name.
        /// </summary>
        private String DbAccount;

        /// <summary>
        /// Cosmos db database name within the specified account.
        /// </summary>
        private String DbName;

        /// <summary>
        /// Cosmos db collection name within the specified database.
        /// </summary>
        private String DbCollectionName;

        /// <summary>
        /// The token to be used to talk to cosmos db.
        /// </summary>
        private String Token;

        /// <summary>
        /// Possible values include: 'failed', 'unauthenticated', 'succeed'.
        /// </summary>
        private String Status;

      
        /// <summary>
        /// The UTC timestamp for a token expiration time.
        /// </summary>
        private DateTime ExpirationDate;

      
        /// <summary>
        /// The account id.
        /// </summary>
        private String AccountId;

       
        /// <summary>
        /// Get the partition value.
        /// </summary>
        /// <returns>The partition value.</returns>
        public String GetPartition()
        {
            return Partition;
        }

         /// <summary>
         /// Gets the expiration date.
         /// </summary>
         /// <returns>The expiration date.</returns>
        public DateTime GetExpirationDate()
        {
            return ExpirationDate;
        }

     
        /// <summary>
        /// Sets the partition.
        /// </summary>
        /// <returns>The TokenResult object itself.</returns>
        /// <param name="partition">The partition value to set.</param>
        public TokenResult SetPartition(String partition)
        {
            Partition = partition;
            return this;
        }

         /// <summary>
         /// Sets the expiration date.
         /// </summary>
         /// <returns>The expiration date.</returns>
         /// <param name="expirationDate">Expiration date.</param>
        public TokenResult SetExpirationDate(DateTime expirationDate)
        {
            ExpirationDate = expirationDate;
            return this;
        }

         /// <summary>
         /// Sets the account identifier.
         /// </summary>
         /// <returns>The account identifier.</returns>
         /// <param name="accountId">Account identifier.</param>
        public TokenResult SetAccountId(String accountId)
        {
            AccountId = accountId;
            return this;
        }

         /// <summary>
         /// Gets the db account.
         /// </summary>
         /// <returns>The db account.</returns>
        public String GetDbAccount()
        {
            return DbAccount;
        }

       
         /// <summary>
         /// Sets the db account.
         /// </summary>
         /// <returns>The db account.</returns>
         /// <param name="dbAccount">Db account.</param>
        public TokenResult SetDbAccount(String dbAccount)
        {
            DbAccount = dbAccount;
            return this;
        }

         /// <summary>
         /// Gets the name of the db.
         /// </summary>
         /// <returns>The db name.</returns>
        public String GetDbName()
        {
            return DbName;
        }

      
         /// <summary>
         /// Sets the name of the db.
         /// </summary>
         /// <returns>The db name.</returns>
         /// <param name="dbName">Db name.</param>
        public TokenResult SetDbName(String dbName)
        {
            DbName = dbName;
            return this;
        }

       
         /// <summary>
         /// Gets the name of the db collection.
         /// </summary>
         /// <returns>The db collection name.</returns>
        public String GetDbCollectionName()
        {
            return DbCollectionName;
        }

       
         /// <summary>
         /// Sets the name of the db collection.
         /// </summary>
         /// <returns>The db collection name.</returns>
         /// <param name="dbCollectionName">Db collection name.</param>
        public TokenResult SetDbCollectionName(String dbCollectionName)
        {
            DbCollectionName = dbCollectionName;
            return this;
        }

         /// <summary>
         /// Gets the token.
         /// </summary>
         /// <returns>The token.</returns>
        public String GetToken()
        {
            return Token;
        }

       
         /// <summary>
         /// Sets the token.
         /// </summary>
         /// <returns>The token.</returns>
         /// <param name="token">Token.</param>
        public TokenResult SetToken(String token)
        {
            Token = token;
            return this;
        }

         /// <summary>
         /// Gets the status.
         /// </summary>
         /// <returns>The status.</returns>
        public String getStatus()
        {
            return Status;
        }

         /// <summary>
         /// Gets the account identifier.
         /// </summary>
         /// <returns>The account identifier.</returns>
        public String GetAccountId()
        {
            return AccountId;
        }

      
         /// <summary>
         /// Sets the status.
         /// </summary>
         /// <returns>The status.</returns>
         /// <param name="status">Status.</param>
        public TokenResult SetStatus(String status)
        {
            Status = status;
            return this;
        }
    }
}