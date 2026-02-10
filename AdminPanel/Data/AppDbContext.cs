using AdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Reccomendation> Reccomendations => Set<Reccomendation>();
    public DbSet<Training> Trainings => Set<Training>();
    public DbSet<TrainsExercise> TrainsExercises => Set<TrainsExercise>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFood> UserFood => Set<UserFood>();
    public DbSet<UsersTrain> UsersTrain => Set<UsersTrain>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.ToTable("Exercises", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.MuscleGroup).HasColumnName("muscleGroup");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Kcal).HasColumnName("kcal");
            entity.Property(e => e.Protein).HasColumnName("protein");
            entity.Property(e => e.Carbohyd).HasColumnName("carbohyd");
            entity.Property(e => e.Fat).HasColumnName("fat");
        });

        modelBuilder.Entity<Reccomendation>(entity =>
        {
            entity.ToTable("Reccomendations", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
        });

        modelBuilder.Entity<Training>(entity =>
        {
            entity.ToTable("Trainings", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.Inventory).HasColumnName("inventory");
            entity.Property(e => e.MuscleGroup).HasColumnName("muscleGroup");
        });

        modelBuilder.Entity<TrainsExercise>(entity =>
        {
            entity.ToTable("TrainsExercises", "public");
            entity.HasKey(e => new { e.TrainId, e.ExerciseId });
            entity.Property(e => e.TrainId).HasColumnName("trainId");
            entity.Property(e => e.ExerciseId).HasColumnName("exerciseId");
            entity.Property(e => e.Ammount).HasColumnName("ammount");

            entity.HasOne(d => d.Train)
                .WithMany(p => p.TrainsExercises)
                .HasForeignKey(d => d.TrainId);

            entity.HasOne(d => d.Exercise)
                .WithMany(p => p.TrainsExercises)
                .HasForeignKey(d => d.ExerciseId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "public");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("userId").ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.FoodLike).HasColumnName("foodLike");
        });

        modelBuilder.Entity<UserFood>(entity =>
        {
            entity.ToTable("UserFood", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.FoodId).HasColumnName("foodId");
            entity.Property(e => e.Weight).HasColumnName("weight");
            entity.Property(e => e.Date).HasColumnName("date");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserFood)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Food)
                .WithMany(p => p.UserFood)
                .HasForeignKey(d => d.FoodId);
        });

        modelBuilder.Entity<UsersTrain>(entity =>
        {
            entity.ToTable("UsersTrain", "public");
            entity.HasKey(e => new { e.UserId, e.TrainId, e.DateCreated });
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.TrainId).HasColumnName("trainId");
            entity.Property(e => e.DateCreated).HasColumnName("dateCreated");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UsersTrain)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Train)
                .WithMany(p => p.UsersTrain)
                .HasForeignKey(d => d.TrainId);
        });
    }
}
