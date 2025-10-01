using FluentValidation;
using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;
using UrnaEletronicaFake.Shared.Constants;

namespace UrnaEletronicaFake.Administration.Validation;

public class EleicaoValidator : AbstractValidator<Eleicao>
{
    public EleicaoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome da eleição é obrigatório")
            .MaximumLength(200)
            .WithMessage("Nome da eleição deve ter no máximo 200 caracteres");

        RuleFor(x => x.DataInicio)
            .NotEmpty()
            .WithMessage("Data de início é obrigatória")
            .Must(BeValidStartDate)
            .WithMessage("Data de início deve ser no futuro");

        RuleFor(x => x.DataFim)
            .NotEmpty()
            .WithMessage("Data de fim é obrigatória")
            .Must((eleicao, dataFim) => BeValidEndDate(eleicao.DataInicio, dataFim))
            .WithMessage("Data de fim deve ser posterior à data de início");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de eleição inválido");

        RuleFor(x => x.Ativa)
            .NotEmpty()
            .WithMessage("Status ativo é obrigatório");
    }

    private static bool BeValidStartDate(DateTime startDate)
    {
        return startDate > DateTime.Now.AddMinutes(-5);
    }

    private static bool BeValidEndDate(DateTime startDate, DateTime endDate)
    {
        return endDate > startDate;
    }
}

public class CandidatoValidator : AbstractValidator<Candidato>
{
    public CandidatoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome do candidato é obrigatório")
            .MaximumLength(200)
            .WithMessage("Nome do candidato deve ter no máximo 200 caracteres");

        RuleFor(x => x.Numero)
            .NotEmpty()
            .WithMessage("Número do candidato é obrigatório")
            .MaximumLength(10)
            .WithMessage("Número do candidato deve ter no máximo 10 caracteres")
            .Matches(@"^[0-9]+$")
            .WithMessage("Número do candidato deve conter apenas números");

        RuleFor(x => x.Cargo)
            .NotEmpty()
            .WithMessage("Cargo é obrigatório")
            .MaximumLength(100)
            .WithMessage("Cargo deve ter no máximo 100 caracteres");

        RuleFor(x => x.Partido)
            .MaximumLength(50)
            .WithMessage("Partido deve ter no máximo 50 caracteres");

        RuleFor(x => x.Ativo)
            .NotEmpty()
            .WithMessage("Status ativo é obrigatório");
    }
}

public class CargoEleitoralValidator : AbstractValidator<CargoEleitoral>
{
    public CargoEleitoralValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome do cargo é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome do cargo deve ter no máximo 100 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(500)
            .WithMessage("Descrição deve ter no máximo 500 caracteres");

        RuleFor(x => x.Ativo)
            .NotEmpty()
            .WithMessage("Status ativo é obrigatório");
    }
}
