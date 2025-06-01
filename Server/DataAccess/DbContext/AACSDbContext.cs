using System.Models;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Contexts;

public partial class ApplicationDbContext
{
    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectSchedule> SubjectSchedules { get; set; }

    public virtual DbSet<SubjectScheduleDetail> SubjectScheduleDetails { get; set; }

    public virtual DbSet<SubjectScheduleStudent> SubjectScheduleStudents { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }
    public virtual DbSet<Class> Classes { get; set; }
    public virtual DbSet<UserFaceEmbedding> UserFaceEmbeddings { get; set; }
    public virtual DbSet<Attendance> Attendances { get; set; }

    public static void AacsOnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
       {
           entity.HasKey(e => e.AttendanceId).HasName("Attendances_pkey");

           entity.Property(e => e.AttendanceId).HasMaxLength(128);
           entity.Property(e => e.CreateDate).HasPrecision(6);
           entity.Property(e => e.CreatedUserId).HasMaxLength(128);
           entity.Property(e => e.StatusId).HasMaxLength(128);
           entity.Property(e => e.SubjectScheduleId).HasMaxLength(128);
           entity.Property(e => e.UpdateDate).HasPrecision(6);
           entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
           entity.Property(e => e.UserId).HasMaxLength(128);

           entity.HasOne(d => d.SubjectSchedule).WithMany(p => p.Attendances)
               .HasForeignKey(d => d.SubjectScheduleId)
               .HasConstraintName("Attendances_SubjectScheduleId_fkey");
       });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("Classes_pkey");

            entity.HasIndex(e => e.ClassCode, "Classes_ClassCode_key").IsUnique();

            entity.Property(e => e.ClassId).HasMaxLength(128);
            entity.Property(e => e.ClassCode).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.DepartmentId).HasMaxLength(128);
            entity.Property(e => e.SchoolYearEnd).HasPrecision(6);
            entity.Property(e => e.SchoolYearStart).HasPrecision(6);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);

            entity.HasOne(d => d.Department).WithMany(p => p.Classes)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("Classes_DepartmentId_fkey");
        });
        modelBuilder.Entity<Semester>(entity =>
       {
           entity.HasKey(e => e.SemesterId).HasName("Semesters_pkey");

           entity.Property(e => e.SemesterId).HasMaxLength(128);
           entity.Property(e => e.CreateDate).HasPrecision(6);
           entity.Property(e => e.CreatedUserId).HasMaxLength(128);
           entity.Property(e => e.EndDate).HasPrecision(6);
           entity.Property(e => e.SemesterName).HasMaxLength(128);
           entity.Property(e => e.StartDate).HasPrecision(6);
           entity.Property(e => e.UpdateDate).HasPrecision(6);
           entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
       });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("Students_pkey");

            entity.HasIndex(e => e.StudentCode, "Students_StudentCode_key").IsUnique();

            entity.HasIndex(e => e.StudentId, "Students_StudentId_idx");

            entity.Property(e => e.StudentId).HasMaxLength(128);
            entity.Property(e => e.ClassId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.StudentCode).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("Students_ClassId_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("Subjects_pkey");

            entity.Property(e => e.SubjectId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.SubjectCode).HasMaxLength(128);
            entity.Property(e => e.SubjectName).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
        });

        modelBuilder.Entity<SubjectSchedule>(entity =>
        {
            entity.HasKey(e => e.SubjectScheduleId).HasName("SubjectSchedules_pkey");

            entity.HasIndex(e => e.SemesterId, "SubjectSchedules_SemesterId_idx");

            entity.HasIndex(e => e.SubjectId, "SubjectSchedules_SubjectId_idx");

            entity.HasIndex(e => e.TeacherId, "SubjectSchedules_TeacherId_idx");

            entity.Property(e => e.SubjectScheduleId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.EndDate).HasPrecision(6);
            entity.Property(e => e.RoomNumber).HasMaxLength(10);
            entity.Property(e => e.SemesterId).HasMaxLength(128);
            entity.Property(e => e.StartDate).HasPrecision(6);
            entity.Property(e => e.SubjectId).HasMaxLength(128);
            entity.Property(e => e.TeacherId).HasMaxLength(128);
            entity.Property(e => e.TeachingAssistant).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);

            entity.HasOne(d => d.Semester).WithMany(p => p.SubjectSchedules)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SubjectSchedules_SemesterId_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectSchedules)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("SubjectSchedules_SubjectId_fkey");

            entity.HasOne(d => d.Teacher).WithMany(p => p.SubjectScheduleTeachers)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("SubjectSchedules_TeacherId_fkey");

            entity.HasOne(d => d.TeachingAssistantNavigation).WithMany(p => p.SubjectScheduleTeachingAssistant)
                .HasForeignKey(d => d.TeachingAssistant)
                .HasConstraintName("SubjectSchedules_TeachingAssistant_fkey");
        });

        modelBuilder.Entity<SubjectScheduleDetail>(entity =>
        {
            entity.HasKey(e => e.SubjectScheduleDetailId).HasName("SubjectScheduleDetails_pkey");

            entity.HasIndex(e => new { e.ScheduleDate, e.StartTime, e.EndTime }, "SubjectScheduleDetails_ScheduleDate_StartTime_EndTime_idx");

            entity.Property(e => e.SubjectScheduleDetailId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.EndTime).HasPrecision(6);
            entity.Property(e => e.ScheduleDate).HasPrecision(6);
            entity.Property(e => e.StartTime).HasPrecision(6);
            entity.Property(e => e.SubjectScheduleId).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
        });

        modelBuilder.Entity<SubjectScheduleStudent>(entity =>
        {
            entity.HasKey(e => e.SubjectScheduleStudentId).HasName("SubjectScheduleStudents_pkey");

            entity.HasIndex(e => e.SubjectScheduleId, "SubjectScheduleStudents_SubjectScheduleId_idx");

            entity.Property(e => e.SubjectScheduleStudentId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.StudentId).HasMaxLength(128);
            entity.Property(e => e.SubjectScheduleId).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);

            entity.HasOne(d => d.Student).WithMany(p => p.SubjectScheduleStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("SubjectScheduleStudents_StudentId_fkey");

            entity.HasOne(d => d.SubjectSchedule).WithMany(p => p.SubjectScheduleStudents)
                .HasForeignKey(d => d.SubjectScheduleId)
                .HasConstraintName("SubjectScheduleStudents_SubjectScheduleId_fkey");
        });
        modelBuilder.Entity<Teacher>(entity =>
       {
           entity.HasKey(e => e.TeacherId).HasName("Teachers_pkey");

           entity.HasIndex(e => e.TeacherId, "Teachers_TeacherId_idx");

           entity.Property(e => e.TeacherId).HasMaxLength(128);
           entity.Property(e => e.CreateDate).HasPrecision(6);
           entity.Property(e => e.CreatedUserId).HasMaxLength(128);
           entity.Property(e => e.TeacherCode).HasMaxLength(128);
           entity.Property(e => e.UpdateDate).HasPrecision(6);
           entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
           entity.Property(e => e.UserId).HasMaxLength(128);
       });
        modelBuilder.Entity<UserFaceEmbedding>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserFaceEmbeddings_pkey");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(128);
            entity.Property(e => e.Embedding).IsRequired();
        });
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.Property(e => e.AttendanceId).HasMaxLength(128);
            entity.Property(e => e.CreateDate).HasPrecision(6);
            entity.Property(e => e.CreatedUserId).HasMaxLength(128);
            entity.Property(e => e.StatusId).HasMaxLength(128);
            entity.Property(e => e.SubjectScheduleId).HasMaxLength(128);
            entity.Property(e => e.UpdateDate).HasPrecision(6);
            entity.Property(e => e.UpdatedUserId).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(128);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.AttendanceTime).HasPrecision(6);
        });

    }
}