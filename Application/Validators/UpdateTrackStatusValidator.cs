using Application.DTOs.Tracks;
using FluentValidation;

namespace Application.Validators.Tracks
{
    public class UpdateTrackStatusDtoValidator : AbstractValidator<UpdateTrackStatusDto>
    {
        public UpdateTrackStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Track status must be Draft, Submitted, or Distributed.");
        }
    }
}