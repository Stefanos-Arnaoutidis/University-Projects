using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UniSystem.Models;

public partial class UniversityGradesDbContext : DbContext
{
    public UniversityGradesDbContext()
    {
    }

    public UniversityGradesDbContext(DbContextOptions<UniversityGradesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseHasStudent> CourseHasStudents { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Secretary> Secretaries { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.IdCourse).HasName("PK__Course__C18577570D71D950");

            entity.ToTable("Course");

            entity.Property(e => e.IdCourse).HasColumnName("idCOURSE");
            entity.Property(e => e.CourseSemester)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.ProfessorsAfm).HasColumnName("PROFESSORS_AFM");

            entity.HasOne(d => d.ProfessorsAfmNavigation).WithMany(p => p.Courses)
                .HasForeignKey(d => d.ProfessorsAfm)
                .HasConstraintName("FK_Course_Professors");
        });

        modelBuilder.Entity<CourseHasStudent>(entity =>
        {
            entity.HasKey(e => new { e.CourseIdCourse, e.StudentsRegistrationNumber }).HasName("PK__Course_h__A12B81E5062BBCBC");

            entity.ToTable("Course_has_Students");

            entity.Property(e => e.CourseIdCourse).HasColumnName("COURSE_idCOURSE");
            entity.Property(e => e.StudentsRegistrationNumber).HasColumnName("STUDENTS_RegistrationNumber");

            entity.HasOne(d => d.CourseIdCourseNavigation).WithMany(p => p.CourseHasStudents)
                .HasForeignKey(d => d.CourseIdCourse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChS_Course");

            entity.HasOne(d => d.StudentsRegistrationNumberNavigation).WithMany(p => p.CourseHasStudents)
                .HasForeignKey(d => d.StudentsRegistrationNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChS_Students");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.Afm).HasName("PK__Professo__C6906E63DB96DD41");

            entity.Property(e => e.Afm)
                .ValueGeneratedNever()
                .HasColumnName("AFM");
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Professors)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Professors_Users");
        });

        modelBuilder.Entity<Secretary>(entity =>
        {
            entity.HasKey(e => e.Phonenumber).HasName("PK__Secretar__9FDCA5A67C3B7427");

            entity.Property(e => e.Phonenumber).ValueGeneratedNever();
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Secretaries)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Secretaries_Users");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.RegistrationNumber).HasName("PK__Students__E8864603B139B719");

            entity.Property(e => e.RegistrationNumber).ValueGeneratedNever();
            entity.Property(e => e.Department)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.UsersUsername)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("USERS_username");

            entity.HasOne(d => d.UsersUsernameNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.UsersUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__Users__F3DBC57356E5F295");

            entity.Property(e => e.Username)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
