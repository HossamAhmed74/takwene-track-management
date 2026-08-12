using System.Text.RegularExpressions;
using Application.DTOs.Tracks;
using FluentValidation;

namespace Application.Validators.Tracks
{
    public class TrackCreateDtoValidator : AbstractValidator<TrackCreateDto>
    {
        public TrackCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Track title is required.")
                .Length(2, 300)
                .WithMessage("Track title must be between 2 and 300 characters.");

            RuleFor(x => x.ArtistId)
                .GreaterThan(0)
                .WithMessage("ArtistId must be greater than 0.");

            RuleFor(x => x.Isrc)
                .NotEmpty()
                .WithMessage("ISRC is required.")
                .Must(BeValidIsrc)
                .WithMessage("ISRC must be 12 alphanumeric characters. Hyphens and spaces are ignored.");

            RuleFor(x => x.ReleaseDate)
                .Must(date => date != default)
                .WithMessage("ReleaseDate is required.");

            RuleFor(x => x.Genre)
                .NotEmpty()
                .WithMessage("Genre is required.")
                .Length(2, 100)
                .WithMessage("Genre must be between 2 and 100 characters.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Track status must be Draft, Submitted, or Distributed.");
        }

        private static bool BeValidIsrc(string? isrc)
        {
            if (string.IsNullOrWhiteSpace(isrc))
            {
                return false;
            }

            var normalizedIsrc = isrc
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .ToUpperInvariant();

            return Regex.IsMatch(normalizedIsrc, "^[A-Z0-9]{12}$");
        }
    }
}