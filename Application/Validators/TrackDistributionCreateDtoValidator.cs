using Application.DTOs.TrackDistribution;
using FluentValidation;

namespace Application.Validators.TrackDistribution
{
    public class TrackDistributionCreateDtoValidator : AbstractValidator<TrackDistributionCreateDto>
    {
        public TrackDistributionCreateDtoValidator()
        {
            RuleFor(x => x.DspIds)
                .NotNull()
                .WithMessage("DspIds is required.")
                .NotEmpty()
                .WithMessage("At least one DSP is required.");

            RuleForEach(x => x.DspIds)
                .GreaterThan(0)
                .WithMessage("Each DSP id must be greater than 0.");

            RuleFor(x => x.DspIds)
                .Must(BeUnique)
                .When(x => x.DspIds is not null)
                .WithMessage("DSP ids must be unique.");
        }

        private static bool BeUnique(List<int>? dspIds)
        {
            if (dspIds is null)
            {
                return true;
            }

            return dspIds.Distinct().Count() == dspIds.Count;
        }
    }
}