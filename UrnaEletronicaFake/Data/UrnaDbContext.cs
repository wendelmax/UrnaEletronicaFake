using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Models;
using System;

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
            
            // Dados de seed para eleição
            entity.HasData(new Eleicao
            {
                Id = 1,
                Titulo = "Eleição para Presidente da República",
                Descricao = "Eleição presidencial de 2024",
                DataInicio = DateTime.Now.AddDays(-1),
                DataFim = DateTime.Now.AddDays(30),
                Ativa = true,
                DataCriacao = DateTime.Now
            });
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
                  
            // Dados de seed para cargo
            entity.HasData(new CargoEleitoral
            {
                Id = 1,
                Nome = "Presidente",
                QuantidadeDigitos = 2,
                Ordem = 1,
                Ativo = true,
                EleicaoId = 1
            });
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
                  
            // Dados de seed para candidatos
            entity.HasData(
                new Candidato
                {
                    Id = 1,
                    Nome = "João Silva",
                    Partido = "Partido A",
                    Numero = "10",
                    Biografia = "Candidato do Partido A",
                    EleicaoId = 1,
                    CargoEleitoralId = 1,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Candidato
                {
                    Id = 2,
                    Nome = "Maria Santos",
                    Partido = "Partido B",
                    Numero = "20",
                    Biografia = "Candidata do Partido B",
                    EleicaoId = 1,
                    CargoEleitoralId = 1,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Candidato
                {
                    Id = 3,
                    Nome = "Pedro Costa",
                    Partido = "Partido C",
                    Numero = "30",
                    Biografia = "Candidato do Partido C",
                    EleicaoId = 1,
                    CargoEleitoralId = 1,
                    Ativo = true,
                    DataCriacao = DateTime.Now
                }
            );
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
            entity.Property(a => a.TipoAcao).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Entidade).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Descricao).HasMaxLength(500);
            entity.Property(a => a.Usuario).HasMaxLength(100);
            entity.Property(a => a.IpAddress).HasMaxLength(100);
            entity.Property(a => a.DataAcao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(a => a.DadosAnteriores).HasMaxLength(1000);
            entity.Property(a => a.DadosNovos).HasMaxLength(1000);
            
            entity.HasOne(a => a.Eleicao)
                  .WithMany(e => e.Auditorias)
                  .HasForeignKey(a => a.EleicaoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Auditoria>()
            .HasIndex(a => a.DataAcao);

        // Índices para melhor performance
        modelBuilder.Entity<Voto>()
            .HasIndex(v => v.CpfEleitor);
            
        modelBuilder.Entity<Voto>()
            .HasIndex(v => v.EleicaoId);
            
        modelBuilder.Entity<Candidato>()
            .HasIndex(c => new { c.EleicaoId, c.Numero })
            .IsUnique();
    }
} 