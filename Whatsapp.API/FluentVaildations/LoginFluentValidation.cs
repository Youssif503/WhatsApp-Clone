using FluentValidation;
using Whatsapp.BLL.DTOs;

namespace Whatsapp.API.FluentVaildations;

public class LoginFluentValidation:AbstractValidator<LoginUserDto>
{
    public LoginFluentValidation()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .WithMessage("Email is required Ya3m");
        
        RuleFor(x => x.Password)
            .NotNull()
            .WithMessage("Password is required Ya3m");
    }
}