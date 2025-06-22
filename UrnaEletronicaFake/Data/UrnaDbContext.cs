using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Models;

namespace UrnaEletronicaFake.Data;

public class UrnaDbContext : DbContext
{
    public UrnaDbContext(DbContextOptions<UrnaDbContext> options) : base(options)
    {
    }

    public DbSet<Eleicao> Eleicoes { get; set; }
    public DbSet<Candidato> Candidatos { get; set; }
    public DbSet<Voto> Votos { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações das entidades
        modelBuilder.Entity<Eleicao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Candidato>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Partido).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Numero).IsRequired().HasMaxLength(10);
            entity.Property(c => c.Biografia).HasMaxLength(500);
            entity.Property(c => c.FotoUrl).HasMaxLength(200);
            entity.Property(c => c.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relacionamento com Eleição
            entity.HasOne(c => c.Eleicao)
                  .WithMany(e => e.Candidatos)
                  .HasForeignKey(c => c.EleicaoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Voto>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.CpfEleitor).IsRequired().HasMaxLength(11);
            entity.Property(v => v.SessaoId).HasMaxLength(50);
            entity.Property(v => v.IpAddress).HasMaxLength(100);
            entity.Property(v => v.UserAgent).HasMaxLength(200);
            entity.Property(v => v.HashVoto).IsRequired().HasMaxLength(64);
            entity.Property(v => v.DataVoto).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relacionamentos
            entity.HasOne(v => v.Eleicao)
                  .WithMany(e => e.Votos)
                  .HasForeignKey(v => v.EleicaoId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(v => v.Candidato)
                  .WithMany(c => c.Votos)
                  .HasForeignKey(v => v.CandidatoId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.TipoAcao).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Entidade).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Descricao).HasMaxLength(500);
            entity.Property(a => a.Usuario).HasMaxLength(100);
            entity.Property(a => a.IpAddress).HasMaxLength(100);
            entity.Property(a => a.DadosAnteriores).HasMaxLength(1000);
            entity.Property(a => a.DadosNovos).HasMaxLength(1000);
            entity.Property(a => a.DataAcao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Relacionamento com Eleição
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
            .HasIndex(a => a.DataAcao);
    }
} 