using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LegalAndGeneralConsultantCRM.Models;
using LegalAndGeneralConsultantCRM.Models.Deposits;
using LegalAndGeneralConsultantCRM.Models.LeadFollowUp;
using LegalAndGeneralConsultantCRM.Models.Leads;
using LegalAndGeneralConsultantCRM.Models.ProgramsTalk;
using LegalAndGeneralConsultantCRM.Models.Referrals;
using LegalAndGeneralConsultantCRM.Models.Services;
using LegalAndGeneralConsultantCRM.Models.Students;
using LegalAndGeneralConsultantCRM.Models.Universiies;
using LegalAndGeneralConsultantCRM.Models.VisaApplications;
using System.Reflection.Emit;
using LegalAndGeneralConsultantCRM.Models.CalendarEvents;
using LegalAndGeneralConsultantCRM.Models.ActivitiesLog;
using LegalAndGeneralConsultantCRM.Models.Branches;

namespace LegalAndGeneralConsultantCRM.Areas.Identity.Data;

public class LegalAndGeneralConsultantCRMContext : IdentityDbContext<LegalAndGeneralConsultantCRMUser>
{
	public LegalAndGeneralConsultantCRMContext(DbContextOptions<LegalAndGeneralConsultantCRMContext> options)
		: base(options)
	{
	}
	public DbSet<Lead> Leads { get; set; }
	public DbSet<TargetTask> TargetTask { get; set; }
	public DbSet<Frenchise> Frenchise { get; set; }
	public DbSet<Domain> Domain { get; set; }
	public DbSet<LeadAssignEmployee> LeadAssignEmployees { get; set; }

	public DbSet<FollowUp> FollowUps { get; set; }

	public DbSet<Referral> Referrals { get; set; }
	public DbSet<University> Universities { get; set; }
	public DbSet<OfferProgram> Programs { get; set; }
	public DbSet<Education> Educations { get; set; }
	public DbSet<Student> Students { get; set; }
	public DbSet<LeadConversionDetail> LeadConversionDetails { get; set; }
	public DbSet<AcademicRecord> StudentAcademias { get; set; }
	public DbSet<ProgramInTalk> ProgramInTalks { get; set; }
	public DbSet<Deposit> Deposits { get; set; }
	public DbSet<VisaApplication> VisaApplications { get; set; }
	public DbSet<Notification> Notifications { get; set; }
	public DbSet<StudentMessage> StudentMessages { get; set; }
	public DbSet<StudentEnrollment> StudentEnrollments { get; set; }
	public DbSet<Service> Services { get; set; }
	public DbSet<ClubMember> ClubMembers { get; set; }
	public DbSet<Course> Courses { get; set; }
	public DbSet<UniversityCourse> UniversityCourses { get; set; }
	public DbSet<Incentive> Incentives { get; set; }
	public DbSet<Scholarship> Scholarships { get; set; }
	public DbSet<CalendarEvent> CalendarEvents { get; set; }
	public DbSet<LeadHistory> LeadHistories { get; set; }
	public DbSet<ActivityLog> ActivityLogs { get; set; }
	public DbSet<Branch> Branches { get; set; }
	public DbSet<Consultationfee> Consultationfees { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<AcademicRecord>()
	.HasOne(a => a.Student)
	.WithMany(s => s.AcademicRecords)
	.HasForeignKey(a => a.StudentId)
	.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<AcademicRecord>()
			.HasOne(a => a.Education)
			.WithMany(e => e.AcademicRecords)
			.HasForeignKey(a => a.EducationLevelId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Deposit>()
				.Property(d => d.Amount)
				.HasColumnType("decimal(18, 2)");

		modelBuilder.Entity<Deposit>()
			  .Property(d => d.AccountNumber)
			  .HasColumnType("decimal(18, 2)");

		modelBuilder.Entity<LeadAssignEmployee>()
	 .HasKey(lae => new { lae.EmployeeId, lae.LeadId });

		modelBuilder.Entity<LeadAssignEmployee>()
			.HasOne(lae => lae.Employee)
			.WithMany()
			.HasForeignKey(lae => lae.EmployeeId);

		modelBuilder.Entity<LeadAssignEmployee>()
			.HasOne(lae => lae.Lead)
			.WithMany(l => l.Assignees)
			.HasForeignKey(lae => lae.LeadId);


		modelBuilder.Entity<Course>()
				 .Property(c => c.OtherCosts)
				 .HasColumnType("decimal(18,2)");

		modelBuilder.Entity<UniversityCourse>()
				.Property(uc => uc.TuitionFee)
				.HasColumnType("decimal(18,2)");

		modelBuilder.Entity<UniversityCourse>()
				.HasKey(uc => new { uc.UniversityId, uc.CourseId });
		modelBuilder.Entity<LegalAndGeneralConsultantCRMUser>()
				.HasOne(u => u.Branch)
				.WithMany() // If Branch has a collection of users, use .WithMany(b => b.Users)
				.HasForeignKey(u => u.BrandId)
				.OnDelete(DeleteBehavior.Restrict);
	}
}
