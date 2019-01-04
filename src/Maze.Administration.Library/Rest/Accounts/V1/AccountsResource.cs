using System.Collections.Generic;
using System.Threading.Tasks;
using Maze.Administration.Library.Clients;
using Maze.Administration.Library.Clients.Helpers;
using Maze.Server.Connection.Accounts;

namespace Maze.Administration.Library.Rest.Accounts.V1
{
    public class AccountsResource : VersionedResource<AccountsResource>
    {
        public AccountsResource() : base("accounts")
        {
        }

        public static Task<List<AccountDto>> GetAccounts(IRestClient restClient) => CreateRequest().Execute(restClient).Return<List<AccountDto>>();

        public static Task<AccountDto> PostAccount(PasswordProvidingAccountDto account, IRestClient restClient) =>
            CreateRequest(HttpVerb.Post, null, account).Execute(restClient).Return<AccountDto>();

        public static Task<AccountDto> GetAccount(int accountId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Get, accountId).Execute(restClient).Return<AccountDto>();

        public static Task PutAccount(PasswordProvidingAccountDto account, IRestClient restClient) =>
            CreateRequest(HttpVerb.Put, account.AccountId, account).Execute(restClient);

        public static Task PutAccount(AccountDto account, IRestClient restClient) =>
            CreateRequest(HttpVerb.Put, account.AccountId, account).Execute(restClient);

        public static Task DeleteAccount(int accountId, IRestClient restClient) =>
            CreateRequest(HttpVerb.Delete, accountId).Execute(restClient);
    }
}