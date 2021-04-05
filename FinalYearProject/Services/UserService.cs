using System;
using System.Collections.Generic;
using System.Linq;
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
                FirstName = RegisterPayload.FirstName,
                LastName = RegisterPayload.LastName,
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

        public async Task<User> FetchUserProfile(string Email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == Email.ToUpper());
        }

        public async Task<User> FetchUserWithControlEval(string Email)
        {
            return await _context.Users.Include(x => x.ControlEvaluations).ThenInclude(x => x.ControlsList).SingleOrDefaultAsync(x => x.NormalizedEmail == Email.ToUpper());
        }

        public async Task<List<ControlEvaluations>> FetchUserControlEvaluations(string Email)
        {
            return await _context.Users.Where(x => x.NormalizedEmail == Email.ToUpper())
                .SelectMany(x => x.ControlEvaluations).Include(x => x.ControlsList).ToListAsync();
        }

        public async Task CreateUserControlEvaluation(string Email, ControlEvaluationPayload ControlEvaluationPayload)
        {
            var user = await FetchUserWithControlEval(Email);
            
            ControlEvaluations controlEvaluation = new()
            {
                AuditName = ControlEvaluationPayload.AuditTitle,
                DateCreated = DateTimeOffset.Now,
                LeadAuditor = ControlEvaluationPayload.LeadAuditor,
                ControlStage = ControlStage.InProgress
            };
            user.ControlEvaluations.Add(controlEvaluation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> FetchAllUserProfiles()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task ChangeUserRole(User User, Role Role)
        {
            await _userManager.AddToRoleAsync(User, Role.ToString());
        }
    }
}