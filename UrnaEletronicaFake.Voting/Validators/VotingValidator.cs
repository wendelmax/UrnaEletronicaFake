using FluentValidation;
using UrnaEletronicaFake.Shared.DTOs;
using UrnaEletronicaFake.Shared.Constants;

namespace UrnaEletronicaFake.Voting.Validators;

public class VotingRequestValidator : AbstractValidator<VotingRequest>
{
    public VotingRequestValidator()
    {
        RuleFor(x => x.EleitorId)
            .NotEmpty()
            .WithMessage("ID do eleitor é obrigatório")
            .Length(VotingConstants.MIN_ELEITOR_ID_LENGTH, VotingConstants.MAX_ELEITOR_ID_LENGTH)
            .WithMessage($"ID do eleitor deve ter entre {VotingConstants.MIN_ELEITOR_ID_LENGTH} e {VotingConstants.MAX_ELEITOR_ID_LENGTH} dígitos")
            .Matches(@"^\d+$")
            .WithMessage("ID do eleitor deve conter apenas números");

        RuleFor(x => x.CandidateNumber)
            .NotEmpty()
            .WithMessage("Número do candidato é obrigatório")
            .MaximumLength(10)
            .WithMessage("Número do candidato deve ter no máximo 10 caracteres")
            .Matches(@"^[0-9]+$|^BRANCO$|^NULO$")
            .WithMessage("Número do candidato inválido");

        RuleFor(x => x.ElectionType)
            .IsInEnum()
            .WithMessage("Tipo de eleição inválido");

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("Timestamp é obrigatório")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Timestamp não pode ser no futuro");

        RuleFor(x => x.TerminalId)
            .MaximumLength(50)
            .WithMessage("ID do terminal deve ter no máximo 50 caracteres");

        RuleFor(x => x.SessionId)
            .MaximumLength(100)
            .WithMessage("ID da sessão deve ter no máximo 100 caracteres");
    }
}

public class VotingResultValidator : AbstractValidator<VotingResult>
{
    public VotingResultValidator()
    {
        RuleFor(x => x.Success)
            .NotEmpty()
            .WithMessage("Status de sucesso é obrigatório");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Mensagem é obrigatória")
            .MaximumLength(500)
            .WithMessage("Mensagem deve ter no máximo 500 caracteres");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status do voto inválido");

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("Timestamp é obrigatório");

        RuleFor(x => x.ErrorCode)
            .MaximumLength(50)
            .WithMessage("Código de erro deve ter no máximo 50 caracteres")
            .When(x => !x.Success);
    }
}

