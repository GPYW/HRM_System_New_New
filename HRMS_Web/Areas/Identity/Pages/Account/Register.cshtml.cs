// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using HRMS_Web.DataAccess.Data;
using HRMS_Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS_Web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel

    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(31, ErrorMessage = "The first name field should have a maximum of 31 characters")]
            [DataType(DataType.Text)]
            [Display(Name = "EmployeeID")]
            public string EmployeeID { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "The first name field should have a maximum of 255 characters")]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }


            [Required]
            [StringLength(255, ErrorMessage = "The second name field should have a maximum of 255 characters")]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "The address field should have a maximum of 255 characters")]
            [DataType(DataType.Text)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [StringLength(10, ErrorMessage = "The PhoneNumber field should have a maximum of 10 characters")]
            [DataType(DataType.Text)]
            [Display(Name = "PhoneNumber")]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "join_date")]
            public DateTime join_date { get; set; }

            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "DOB")]
            public DateTime DOB { get; set; }

            [Required]
            [Display(Name = "Department")]
            public string DepartmentID { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> DepartmentList { get; set; }

            public string? Role { get; set; }
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            }

            Input = new InputModel
            {
                EmployeeID = await GenerateEmployeeIDAsync(),
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                DepartmentList = _context.Department.Select(d => new SelectListItem
                {
                    Text = d.DepartmentName,
                    Value = d.DepartmentID
                }).ToList()
            };

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("/EmployeeManagement/Index");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Address = Input.Address;
                user.PhoneNumber = Input.PhoneNumber;
                user.DOB = Input.DOB;
                user.join_date = Input.join_date;
                user.CompanyID = Input.EmployeeID;
                user.DepartmentID = Input.DepartmentID;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    // TempData["TempPassword"] = Input.Password; // Store the password temporarily in TempData

                    if (!string.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Employee);
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    var subject = "Confirm your email";
                    var body = $@"
                <div style='font-family: Arial, sans-serif; color: #333; line-height: 1.6;'>
                    <h2 style='color: #17a2b8;'>Confirm Your Email</h2>
                    <p>Thank you for creating an account with us. Please confirm your account by clicking the button below:</p>
                    <p style='text-align: center;'>
                        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' 
                           style='background-color: #17a2b8; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirm Account</a>
                    </p>
                    <p>If the button above doesn't work, copy and paste the following link into your web browser:</p>
                    <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{HtmlEncoder.Default.Encode(callbackUrl)}</a></p>
                    <hr>
                    <h3>Account Credentials</h3>
                    <p><strong>Username:</strong> {Input.Email}</p>
                    <p><strong>Password:</strong> {Input.Password}</p>
                    <p>Please change your password after your first login for security purposes.</p>
                    <hr>
                    <p style='font-size: 12px; color: #999;'>If you did not create this account, please ignore this email.</p>
                </div>";

                    await _emailSender.SendEmailAsync(Input.Email, subject, body);

                    // if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    // {
                    //     return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    // }
                    // else
                    // {
                    //     await _signInManager.SignInAsync(user, isPersistent: false);
                    //     return LocalRedirect(returnUrl);
                    // }
                    return LocalRedirect(returnUrl); // Redirect to the current user's home page
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Input.DepartmentList = _context.Department.Select(d => new SelectListItem
            {
                Text = d.DepartmentName,
                Value = d.DepartmentID
            }).ToList();

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<string> GenerateEmployeeIDAsync()
        {
            var maxEmployeeID = await _context.ApplicationUser
                .OrderByDescending(e => e.CompanyID)
                .Select(e => e.CompanyID)
                .FirstOrDefaultAsync();

            int nextId = 1;

            if (!string.IsNullOrEmpty(maxEmployeeID) && int.TryParse(maxEmployeeID.Substring(2), out int currentId))
            {
                nextId = currentId + 1;
            }

            return $"OS{nextId:D4}"; // Formats the number as a 4-digit string, e.g., OS0001
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
