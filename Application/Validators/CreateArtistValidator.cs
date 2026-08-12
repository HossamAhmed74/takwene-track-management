using Application.DTOs.Artist;
using FluentValidation;

namespace Application.Validators.Artist
{
    public class ArtistCreateDtoValidator : AbstractValidator<ArtistCreateDto>
    {
        public ArtistCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Artist name is required.")
                .Length(2, 200)
                .WithMessage("Artist name must be between 2 and 200 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email is not valid.");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required.")
                .Length(2, 100)
                .WithMessage("Country must be between 2 and 100 characters.");
        }
    }
}