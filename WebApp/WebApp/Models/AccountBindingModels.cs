using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebApp.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    #region My Binding Models

    public class UserRegistrationBindingModel
    {
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        public string Username { get; set; }

        //[Required]
        public string Birthday { get; set; }

        //[Required]
        public string Address { get; set; }

        [Required]
        public string AgeGroup { get; set; }            // djak, penzioner, regularan

        public byte[] Document { get; set; }
    }

    public class PriceListBindingModel
    {
        public string TicketName { get; set; }
        public double Price { get; set; }
    }

    public class LineBindingModel
    {
        public string Lineid { get; set; }
        public string Description { get; set; }
    }

    public class BuyTicketBindingModel
    {
        public string AgeGroup { get; set; }
        public string Price { get; set; }
        public string IssuingTime { get; set; }
        public string ExpirationTime { get; set; }
        public string Description { get; set; }
        public string TicketTypeId { get; set; }
    }

    public class UserTicketBindingModel
    {
        public string TicketId { get; set; }
        public string IssuingTime { get; set; }
        public string ExpirationTime { get; set; }
        public string TicketType { get; set; }
        //public string Price { get; set; }
    }

    public class UserStatusBindingModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AgeGroup { get; set; }
        public byte[] Document { get; set; }
        public string Birthday { get; set; }
        public string Status { get; set; }
    }

    public class TicketVerificationBindingModel
    {
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class LineStBindingModel
    {
        public string LineId { get; set; }              // za liniju
        public string LineType { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }

        public List<StationBindingModel> Stations { get; set; }
    }

    public class StationBindingModel
    {
        public string Name { get; set; }         // za stanicu
        public string Address { get; set; }
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }
        public string IsStation { get; set; }
    }

    public class SetTimetableBindingModel
    {
        public string LineId { get; set; }
        public string Day { get; set; }
        public string Schedule { get; set; }
    }

    public class PricelistBindingModel
    {
        public string IssueDate { get; set; }
        public string ExpireDate { get; set; }
        public double TimeTicketPrice { get; set; }
        public double DailyTicketPrice { get; set; }
        public double MonthlyTicketPrice { get; set; }
        public double AnnualTicketPrice { get; set; }
    }

    #endregion
}
