using FluentValidation;
using UserManagementAPI.Models;

namespace UserManagementAPI.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(50);

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(50);

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(100);

        RuleFor(u => u.Department)
            .MaximumLength(50);
    }
}