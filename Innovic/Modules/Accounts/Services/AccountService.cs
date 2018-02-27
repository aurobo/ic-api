using Innovic.App;
using Innovic.Modules.Accounts.Models;
using Innovic.Modules.Accounts.Options;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;

namespace Innovic.Modules.Accounts.Services
{
    public class AccountService : IDisposable
    {
        private InnovicContext _context;
        private UserManager<User> _userManager;

        public AccountService()
        {
            _context = new InnovicContext();
            _userManager = new UserManager<User>(new UserStore<User>(_context));

            var tokenProvider = Startup.DataProtectionProvider;
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<User>(tokenProvider.Create("Email"));
        }

        public async Task<IdentityResult> RegisterUser(UserOptions setModel)
        {
            User user = new User
            {
                UserName = setModel.UserName,
                Email = setModel.Email
            };

            var result = await _userManager.CreateAsync(user, setModel.Password);

            return result;
        }

        public async Task<User> FindUser(string userName, string password)
        {
            User user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public void Dispose()
        {
            _context.Dispose();
            _userManager.Dispose();
        }
    }
}