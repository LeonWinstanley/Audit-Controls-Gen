using System;
using System.Threading.Tasks;
using FinalYearProject.Enums;
using FinalYearProject.Models;
using FinalYearProject.Models.Payloads;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FinalYearProject.Services
{
    public class UserService
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly SignInOutService _signInOutService;
        private readonly ISnackbar _snackbar;
        private readonly NavigationManager _navigationManager;

        public UserService(DatabaseContext Context, UserManager<User> UserManager, PasswordHasher<User> PasswordHasher, SignInOutService SignInOutService, ISnackbar Snackbar, NavigationManager NavigationManager)
        {
            _context = Context;
            _userManager = UserManager;
            _passwordHasher = PasswordHasher;
            _signInOutService = SignInOutService;
            _snackbar = Snackbar;
            _navigationManager = NavigationManager;

        }

        public async Task RegisterUser(RegisterPayload RegisterPayload)
        {
            var user = new User
            {
                UserName = RegisterPayload.EmailAddress,
                Email = RegisterPayload.EmailAddress,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, RegisterPayload.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }
            }

            await _userManager.AddToRoleAsync(user, Role.Auditor.ToString());
            _snackbar.Add("Registeration Successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/", true);
        }

        public async Task Login(SignInPayload SignInPayload)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == SignInPayload.EmailAddress);

            if (user == null)
            {
                throw new Exception("Login was unsuccessful. Please try again.");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, SignInPayload.Password) !=
                PasswordVerificationResult.Success)
            {
                throw new Exception("Login was unsuccessful. Please try again.");
            }

            if (!user.EmailConfirmed)
            {
                throw new Exception("Account has not been confirmed.");
            }

            await _signInOutService.SignInAsync(user);

            _snackbar.Add("Login Successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/", true);
        }

        public async Task Logout()
        {
            await _signInOutService.SignOutAsync();
            _snackbar.Add("Logout successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/", true);
        }
    }
}