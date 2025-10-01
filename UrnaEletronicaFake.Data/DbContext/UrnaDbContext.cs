using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Data.DbContext;

public class UrnaDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public UrnaDbContext(DbContextOptions<UrnaDbContext> options) : base(options)
    {
    }

    public DbSet<Eleicao> Eleicoes { get; set; }
    public DbSet<CargoEleitoral> CargosEleitorais { get; set; }
    public DbSet<Candidato> Candidatos { get; set; }
    public DbSet<Voto> Votos { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da tabela Eleicao
        modelBuilder.Entity<Eleicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.DataInicio)
                .IsRequired();
            entity.Property(e => e.DataFim)
                .IsRequired();
            entity.Property(e => e.Ativa)
                .IsRequired()
                .HasDefaultValue(true);
            entity.Property(e => e.Tipo)
                .IsRequired()
                .HasConversion<string>();

            entity.HasIndex(e => new { e.DataInicio, e.DataFim });
            entity.HasIndex(e => e.Ativa);
        });

        // Configuração da tabela CargoEleitoral
        modelBuilder.Entity<CargoEleitoral>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(c => c.Descricao)
                .HasMaxLength(500);
            entity.Property(c => c.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            entity.HasIndex(c => c.Nome);
            entity.HasIndex(c => c.Ativo);
        });

        // Configuração da tabela Candidato
        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(c => c.Numero)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(c => c.Partido)
                .HasMaxLength(50);
            entity.Property(c => c.Cargo)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(c => c.Ativo)
                .IsRequired()
                .HasDefaultValue(true);
            entity.Property(c => c.Foto)
                .HasColumnType("BLOB");

            entity.HasIndex(c => c.Numero);
            entity.HasIndex(c => c.Cargo);
            entity.HasIndex(c => c.Ativo);
            entity.HasIndex(c => new { c.Numero, c.Cargo }).IsUnique();
        });

        // Configuração da tabela Voto
        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.EleitorId)
                .IsRequired()
                .HasMaxLength(12);
            entity.Property(v => v.CandidatoNumero)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(v => v.Cargo)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(v => v.DataVoto)
                .IsRequired();
            entity.Property(v => v.TerminalId)
                .HasMaxLength(50);
            entity.Property(v => v.SessionId)
                .HasMaxLength(100);
            entity.Property(v => v.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(VotingStatus.Completed);

            entity.HasIndex(v => v.EleitorId);
            entity.HasIndex(v => v.CandidatoNumero);
            entity.HasIndex(v => v.DataVoto);
            entity.HasIndex(v => v.TerminalId);
            entity.HasIndex(v => new { v.EleitorId, v.Cargo }).IsUnique();

            // Relacionamento com Candidato (se existir)
            entity.HasOne<Candidato>()
                .WithMany()
                .HasForeignKey(v => new { v.CandidatoNumero, v.Cargo })
                .HasPrincipalKey(c => new { c.Numero, c.Cargo })
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da tabela Auditoria
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Acao)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(a => a.UsuarioId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(a => a.Descricao)
                .HasMaxLength(500);
            entity.Property(a => a.IpAddress)
                .HasMaxLength(45);
            entity.Property(a => a.UserAgent)
                .HasMaxLength(500);
            entity.Property(a => a.DataHora)
                .IsRequired();
            entity.Property(a => a.TerminalId)
                .HasMaxLength(50);
            entity.Property(a => a.SessaoId)
                .HasMaxLength(100);
            entity.Property(a => a.Sucesso)
                .IsRequired()
                .HasDefaultValue(true);
            entity.Property(a => a.MensagemErro)
                .HasMaxLength(500);

            entity.HasIndex(a => a.Acao);
            entity.HasIndex(a => a.UsuarioId);
            entity.HasIndex(a => a.DataHora);
            entity.HasIndex(a => a.TerminalId);
            entity.HasIndex(a => a.Sucesso);
        });

        // Dados iniciais
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Cargos Eleitorais iniciais
        modelBuilder.Entity<CargoEleitoral>().HasData(
            new CargoEleitoral { Id = 1, Nome = "Presidente", Descricao = "Presidente da República", Ativo = true },
            new CargoEleitoral { Id = 2, Nome = "Governador", Descricao = "Governador do Estado", Ativo = true },
            new CargoEleitoral { Id = 3, Nome = "Senador", Descricao = "Senador da República", Ativo = true },
            new CargoEleitoral { Id = 4, Nome = "Deputado Federal", Descricao = "Deputado Federal", Ativo = true },
            new CargoEleitoral { Id = 5, Nome = "Deputado Estadual", Descricao = "Deputado Estadual", Ativo = true },
            new CargoEleitoral { Id = 6, Nome = "Prefeito", Descricao = "Prefeito Municipal", Ativo = true },
            new CargoEleitoral { Id = 7, Nome = "Vereador", Descricao = "Vereador Municipal", Ativo = true }
        );

        // Eleição inicial
        modelBuilder.Entity<Eleicao>().HasData(
            new Eleicao
            {
                Id = 1,
                Nome = "Eleição Geral 2024",
                DataInicio = new DateTime(2024, 10, 1),
                DataFim = new DateTime(2024, 10, 31),
                Ativa = true,
                Tipo = ElectionType.Presidential.ToString()
            }
        );
    }
}