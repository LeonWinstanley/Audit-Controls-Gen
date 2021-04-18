using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FinalYearProject.Enums;
using FinalYearProject.Models;
using FinalYearProject.Models.Payloads;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Flurl;

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
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly EmailService _emailService;
        private readonly ControlsService _controlsService;

        public UserService(DatabaseContext Context, UserManager<User> UserManager, PasswordHasher<User> PasswordHasher, SignInOutService SignInOutService, ISnackbar Snackbar, NavigationManager NavigationManager, AuthenticationStateProvider AuthenticationStateProvider, EmailService EmailService, ControlsService ControlsService)
        {
            _context = Context;
            _userManager = UserManager;
            _passwordHasher = PasswordHasher;
            _signInOutService = SignInOutService;
            _snackbar = Snackbar;
            _navigationManager = NavigationManager;
            _authenticationStateProvider = AuthenticationStateProvider;
            _emailService = EmailService;
            _controlsService = ControlsService;
        }

        public async Task RegisterUser(RegisterPayload RegisterPayload)
        {
            var user = new User
            {
                UserName = RegisterPayload.EmailAddress,
                Email = RegisterPayload.EmailAddress,
                FirstName = RegisterPayload.FirstName,
                LastName = RegisterPayload.LastName,
// #if DEBUG
//                 EmailConfirmed = true
// #endif
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
// #if RELEASE
            await SendConfirmationEmail(user.Email);
// #endif
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
            if (Email == "admin")
            {
                Email = "admin@admin";
            }
            return await _context.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == Email.ToUpper());
        }

        public async Task<User> FetchUserWithControlEval(string Email)
        {
            return await _context.Users.Include(x => x.ControlEvaluations).ThenInclude(x => x.ControlsList).SingleOrDefaultAsync(x => x.NormalizedEmail == Email.ToUpper());
        }

        public async Task<List<User>> FetchAllUsersWithControlEval()
        {
            return await _context.Users.Include(x => x.ControlEvaluations).ThenInclude(x => x.ControlsList)
                .ToListAsync();
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

        public async Task EditUserSaveChanges(User User, string role)
        {
            await _userManager.UpdateAsync(User);
            var roles = await _userManager.GetRolesAsync(User);
            await _userManager.RemoveFromRolesAsync(User, roles);
            await _userManager.AddToRoleAsync(User, role);
            await _context.SaveChangesAsync();
        }

        public async Task<string> ExtractUserRole(User user)
        {
            var rolesAsync = await _userManager.GetRolesAsync(user);
            return rolesAsync[0];
        }
        
        public async Task<User> GetCurrentUserAsync()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var authenticationStateUser = authenticationState.User;
            
            return await _userManager.FindByNameAsync(authenticationStateUser.Identity.Name);

        }

        public async Task UpdatePasswordAsync(PasswordResetPayload ResetPayload)
        {
            var user = await GetCurrentUserAsync();
            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, ResetPayload.CurrentPassword, ResetPayload.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                throw new Exception();
            }
            await _signInOutService.ResetUserLoginAsync(user);
        }

        public async Task UpdateEmailAddressAsync(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                throw new Exception();
            }

            var user = await GetCurrentUserAsync();
            var currentEmail = await _userManager.GetEmailAsync(user);

            if (currentEmail == "admin@admin")
            {
                throw new Exception("This is the admin account. You can only do this manually in the SQL server.");
            }
            var userId = await _userManager.GetUserIdAsync(user);
            var resetCode = await _userManager.GenerateChangeEmailTokenAsync(user, Email);

            resetCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetCode));
            Email = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Email));
            userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));

            var confirmationUrl = _navigationManager.BaseUri.AppendPathSegments("ConfirmEmailChange", userId, Email, resetCode);

            var emailMessage = $"Hi {user.FirstName} {user.LastName}, \n\n" +
                               "You have requested an email address change, to confirm this please follow the url. \n\n" +
                               $"{HtmlEncoder.Default.Encode(confirmationUrl)} /n/n" +
                               "If you did not request this change, please disregard this email. \n\n" +
                               "Thanks \n" +
                               "Audit Controls Gen";
            
            await _emailService.SendEmailAsync(currentEmail, "Confirm your email address change", emailMessage);

        }

        public async Task SendConfirmationEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));

            Url confirmationUrl = _navigationManager.BaseUri.AppendPathSegments("ConfirmAccount", userId, code);
            
            var emailMessage = $"Hi {user.FirstName} {user.LastName}. \n\n" + 
                               "You have created an account using our software, to confirm your account please follow the url. \n\n" + 
                               $"{HtmlEncoder.Default.Encode(confirmationUrl)}\n\n" +
                               "Have a nice day.\n" +
                               "Audit Controls Gen";
            
            await _emailService.SendEmailAsync(Email, "Confirm your account", emailMessage);
        }

        public async Task RemoveUser(User user)
        {
            // first remove from any roles
            var rolesAsync = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesAsync);
            
            // check if any ControlEvaluations exist.
            if (user.ControlEvaluations != null)
            {
                // next remove any linking ControlEvaluations Controls
                var controlsList = await _controlsService.FetchAllControls();
                foreach (var controlEval in user.ControlEvaluations)
                {
                    if (controlEval.ControlsList != null)
                    {
                        controlEval.ControlsList.RemoveRange(0, controlEval.ControlsList.Count);
                    }
                }
                
                // next remove any linking ControlEvaluations
                user.ControlEvaluations.RemoveRange(0, user.ControlEvaluations.Count);
            }
            
            // lastly remove user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"An unexpected error occurred whilst attempting to delete the user '{user.FirstName} {user.LastName}'.");
            }
            
        }
        
        public async Task<User> FetchUserFromControlEval(ControlEvaluations controlEvaluations)
        {
            var user =  await _context.Users.SingleOrDefaultAsync(x => x.ControlEvaluations.Contains(controlEvaluations));
            return await FetchUserWithControlEval(user.Email);
        }
    }
}