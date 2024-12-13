using ApiDiariosOficiais.DTO;
using FluentValidation;

namespace ApiDiariosOficiais.Validation
{
    public partial class RetrieveDataDTOValidator : AbstractValidator<RetrieveDataDTO>
    {
        public RetrieveDataDTOValidator()
        {
            RuleFor(x => x.TextToSearch)
            .NotEmpty().WithMessage("TextToSearch é obrigatório.")
            .NotNull().WithMessage("TextToSearch não pode ser nulo.")
            .Matches(@"^[a-zA-Z0-9\s/.\-]*$").WithMessage("TextToSearch só pode conter letras, números, espaço, /, - e .")
            .MaximumLength(50).WithMessage("TextToSearch deve conter no máximo 50 caracteres.");

            RuleFor(x => x.InitialDate)
                .NotEmpty().WithMessage("InitialDate é obrigatório.")
                .NotNull().WithMessage("InitialDate não pode ser nulo.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("InitialDate não pode estar no futuro.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate é obrigatório.")
                .NotNull().WithMessage("EndDate não pode ser nulo.")
                .GreaterThan(x => x.InitialDate).WithMessage("EndDate deve ser maior que InitialDate.");

            RuleFor(x => x.GetAcre)
                .NotNull().WithMessage("GetAcre é obrigatório.");

            RuleFor(x => x.GetAlagoas)
                .NotNull().WithMessage("GetAlagoas é obrigatório.");

            RuleFor(x => x.GetAmapa)
                .NotNull().WithMessage("GetAmapa é obrigatório.");

            RuleFor(x => x.GetRioDeJaneiro)
                .NotNull().WithMessage("GetRioDeJaneiro é obrigatório.");

            RuleFor(x => x.GetRioGrandeDoSul)
                .NotNull().WithMessage("GetRioGrandeDoSul é obrigatório.");

            RuleFor(x => x.GetSaoPaulo)
                .NotNull().WithMessage("GetSaoPaulo é obrigatório.");

            RuleFor(x => x.GetMinasGerais)
                .NotNull().WithMessage("GetMinasGerais é obrigatório.");

            RuleFor(x => x.InitialPage)
                .NotNull().WithMessage("InitialPage é obrigatório.");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(0).WithMessage("Pagina não pode ser negativa.");

        }
    }
}
