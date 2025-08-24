using FluentValidation;
using TodoApi.Dtos;

namespace TodoApi
{
    public class TodoUpdateDtoValidator : AbstractValidator<TodoUpdateDto>
    {
        public TodoUpdateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description != null);
        }
    }
}
