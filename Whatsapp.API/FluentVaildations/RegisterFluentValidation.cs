using System.Data;
using FluentValidation;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Whatsapp.BLL.DTOs;

namespace Whatsapp.API.FluentVaildations;

public class RegisterFluentValidation:AbstractValidator<RegisterUserDto>
{
    public RegisterFluentValidation()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("Email is required");
        
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("Must be a valid email address");
        
        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        
        RuleFor(u => u.Password)
            .MinimumLength(5)
            .WithMessage("Password must be at least 5 characters long");
        
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage("First Name is required");
        
        RuleFor(u => u.LastName)
            .NotEmpty()
            .WithMessage("Last Name is required");
        

    }
}