using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Models;

namespace UrnaEletronicaFake.Data;

public class UrnaDbContext : DbContext
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

        // Configuração da Eleição
        modelBuilder.Entity<Eleicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configuração do Cargo Eleitoral
        modelBuilder.Entity<CargoEleitoral>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            entity.Property(c => c.QuantidadeDigitos).IsRequired();
            entity.Property(c => c.Ordem).IsRequired();
            entity.Property(c => c.Ativo).HasDefaultValue(true);
            
            entity.HasOne(c => c.Eleicao)
                  .WithMany(e => e.CargosEleitorais)
                  .HasForeignKey(c => c.EleicaoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração do Candidato
        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Partido).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Numero).IsRequired();
            entity.Property(c => c.Foto).HasMaxLength(500);
            entity.Property(c => c.Ativo).HasDefaultValue(true);
            
            entity.HasOne(c => c.Eleicao)
                  .WithMany(e => e.Candidatos)
                  .HasForeignKey(c => c.EleicaoId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(c => c.CargoEleitoral)
                  .WithMany(c => c.Candidatos)
                  .HasForeignKey(c => c.CargoEleitoralId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração do Voto
        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.CpfEleitor).IsRequired().HasMaxLength(20);
            entity.Property(v => v.HashVoto).IsRequired().HasMaxLength(500);
            entity.Property(v => v.SessaoId).HasMaxLength(100);
            entity.Property(v => v.DataVoto).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(v => v.Eleicao)
                  .WithMany(e => e.Votos)
                  .HasForeignKey(v => v.EleicaoId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(v => v.CargoEleitoral)
                  .WithMany()
                  .HasForeignKey(v => v.CargoEleitoralId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(v => v.Candidato)
                  .WithMany(c => c.Votos)
                  .HasForeignKey(v => v.CandidatoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuração da Auditoria
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Acao).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Entidade).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Detalhes).HasMaxLength(500);
            entity.Property(a => a.IpAddress).HasMaxLength(45);
            entity.Property(a => a.UserAgent).HasMaxLength(200);
            entity.Property(a => a.DataHora).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(a => a.Eleicao)
                  .WithMany(e => e.Auditorias)
                  .HasForeignKey(a => a.EleicaoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Índices para melhor performance
        modelBuilder.Entity<Voto>()
            .HasIndex(v => v.CpfEleitor);
            
        modelBuilder.Entity<Voto>()
            .HasIndex(v => v.EleicaoId);
            
        modelBuilder.Entity<Candidato>()
            .HasIndex(c => new { c.EleicaoId, c.Numero })
            .IsUnique();
            
        modelBuilder.Entity<Auditoria>()
            .HasIndex(a => a.DataHora);
    }
} 