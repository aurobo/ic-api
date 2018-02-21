using Innovic.JModels;
using Innovic.Models;
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
        private UserManager<IdentityUser> _userManager;

        public AccountService()
        {
            _context = new InnovicContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));

            var tokenProvider = Startup.DataProtectionProvider;
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(tokenProvider.Create("Email"));
        }

        public async Task<IdentityResult> RegisterUser(UserOptions setModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = setModel.UserName,
                Email = setModel.Email
            };

            var result = await _userManager.CreateAsync(user, setModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public void Dispose()
        {
            _context.Dispose();
            _userManager.Dispose();
        }
    }
}