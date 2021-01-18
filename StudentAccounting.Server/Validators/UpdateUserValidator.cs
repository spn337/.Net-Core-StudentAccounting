using FluentValidation;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.DTOs;


namespace StudentAccounting.Server.Validators
{
    public class UpdateUserValidator : AbstractValidator<UserUpdateDTO>
    {
        public UpdateUserValidator()
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
        }
    }
}
