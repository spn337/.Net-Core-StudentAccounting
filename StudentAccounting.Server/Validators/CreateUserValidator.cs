using FluentValidation;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.DTOs;

namespace StudentAccounting.Server.Validators
{
    public class CreateUserValidator : AbstractValidator<UserCreateDTO>
    {
        public CreateUserValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty()
                .MaximumLength(Validated.MAX_LENGTH)
                .Matches(Validated.USERNAME_REGEX)
                .WithMessage("First name must contain only letters");

            RuleFor(user => user.LastName)
                .NotEmpty()
                .MaximumLength(Validated.MAX_LENGTH)
                .Matches(Validated.USERNAME_REGEX)
                .WithMessage("Last name must contain only letters");

            RuleFor(user => user.Age)
               .NotEmpty()
               .Must(age => age >= Validated.MIN_AGE && age <= Validated.MAX_AGE)
               .WithMessage($"Age must be between { Validated.MIN_AGE} and {Validated.MAX_AGE}");

            RuleFor(user => user.Email)
                .NotEmpty()
                .MaximumLength(Validated.MAX_LENGTH)
                .EmailAddress()
                .WithMessage("Please enter a valid email address");

            RuleFor(user => user.Password)
                .NotEmpty()
                .MaximumLength(Validated.MAX_LENGTH)
                .MinimumLength(Validated.MIN_LENGTH)
                .WithMessage($"Password must contain at least {Validated.MIN_LENGTH} characters");

            RuleFor(user => user.Password)
               .Matches(Validated.PASSWORD_WITH_LOWERCASE_REGEX)
               .WithMessage("Password must contain at least one lowercase letter")
               .Matches(Validated.PASSWORD_WITH_UPPERCASE_REGEX)
               .WithMessage("Password must contain at least one uppercase letter")
               .Matches(Validated.PASSWORD_WITH_DIGIT_REGEX)
               .WithMessage("Password must contain at least one digit")
               .Matches(Validated.PASSWORD_WITH_SPECIAL_CHARACTER_REGEX)
               .WithMessage("Password must contain at least one special character");

            RuleFor(user => user.ConfirmPassword)
                .NotEmpty()
                .Equal(user => user.Password)
                .WithMessage("Password not matched");
        }
    }
}
