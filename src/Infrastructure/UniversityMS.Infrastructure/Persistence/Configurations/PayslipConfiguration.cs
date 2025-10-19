using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.HasKey(x => x.Id);

        // Table
        builder.ToTable("Payslips", schema: "payroll");

        // Properties
        builder.Property(x => x.Id)
            .HasColumnName("PayslipId")
            .ValueGeneratedNever();

        builder.Property(x => x.PayrollId)
            .IsRequired();

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.PayrollNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Month)
            .IsRequired();

        builder.Property(x => x.Period)
            .HasMaxLength(10)
            .IsRequired()
            .HasComment("MM/YYYY format");

        builder.Property(x => x.FilePath)
            .HasMaxLength(500);

        builder.Property(x => x.FileName)
            .HasMaxLength(256);

        builder.Property(x => x.FileSize)
            .HasComment("Byte cinsinden");

        builder.Property(x => x.FileHash)
            .HasMaxLength(256)
            .HasComment("SHA256 hash");

        // Money Value Objects
        builder.OwnsOne(x => x.GrossSalary, nav =>
        {
            nav.Property(m => m.Amount)
                .HasColumnName("GrossSalary")
                .HasPrecision(18, 2);
            nav.Property(m => m.Currency)
                .HasColumnName("GrossSalaryCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.TotalDeductions, nav =>
        {
            nav.Property(m => m.Amount)
                .HasColumnName("TotalDeductions")
                .HasPrecision(18, 2);
            nav.Property(m => m.Currency)
                .HasColumnName("TotalDeductionsCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.NetSalary, nav =>
        {
            nav.Property(m => m.Amount)
                .HasColumnName("NetSalary")
                .HasPrecision(18, 2);
            nav.Property(m => m.Currency)
                .HasColumnName("NetSalaryCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.IncomeTax, nav =>
        {
            nav.Property(m => m.Amount)
                .HasColumnName("IncomeTax")
                .HasPrecision(18, 2);
            nav.Property(m => m.Currency)
                .HasColumnName("IncomeTaxCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.SGKEmployeeContribution, nav =>
        {
            nav.Property(m => m.Amount)
                .HasColumnName("SGKEmployeeContribution")
                .HasPrecision(18, 2);
            nav.Property(m => m.Currency)
                .HasColumnName("SGKEmployeeContributionCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(x => x.SGKEmployerContribution, nav =>
        {
            nav.Property(m => m.Amount)
                .HasColumnName("SGKEmployerContribution")
                .HasPrecision(18, 2);
            nav.Property(m => m.Currency)
                .HasColumnName("SGKEmployerContributionCurrency")
                .HasMaxLength(3);
        });

        // Work Day Info
        builder.Property(x => x.WorkingDays)
            .IsRequired();

        builder.Property(x => x.ActualWorkDays)
            .IsRequired();

        builder.Property(x => x.OvertimeHours)
            .HasPrecision(10, 2);

        builder.Property(x => x.LeaveDays)
            .HasDefaultValue(0);

        builder.Property(x => x.AbsentDays)
            .HasDefaultValue(0);

        // Payment Info
        builder.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.BankAccount)
            .HasMaxLength(34);

        builder.Property(x => x.BankName)
            .HasMaxLength(100);

        builder.Property(x => x.IBAN)
            .HasMaxLength(34);

        // Status
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(PayslipStatus.Draft);

        builder.Property(x => x.GeneratedDate)
            .IsRequired();

        builder.Property(x => x.PrintedDate)
            .IsRequired(false);

        builder.Property(x => x.EmailSentDate)
            .IsRequired(false);

        builder.Property(x => x.DownloadedDate)
            .IsRequired(false);

        // Digital Signature
        builder.Property(x => x.DigitalSignature)
            .HasMaxLength(2048)
            .HasComment("E-imza verisi");

        builder.Property(x => x.IsApproved)
            .HasDefaultValue(false);

        builder.Property(x => x.ApprovedDate)
            .IsRequired(false);

        builder.Property(x => x.ApprovedBy)
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        // Audit Fields
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(256);

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(256);

        // Relationships
        builder.HasOne(x => x.Payroll)
            .WithMany()
            .HasForeignKey(x => x.PayrollId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Payslips_Payroll_PayrollId");

        // Indexes
        builder.HasIndex(x => x.EmployeeId)
            .HasDatabaseName("IX_Payslips_EmployeeId");

        builder.HasIndex(x => x.PayrollId)
            .HasDatabaseName("IX_Payslips_PayrollId")
            .IsUnique();

        builder.HasIndex(x => new { x.EmployeeId, x.Year, x.Month })
            .HasDatabaseName("IX_Payslips_Employee_Year_Month");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Payslips_Status");

        builder.HasIndex(x => x.GeneratedDate)
            .HasDatabaseName("IX_Payslips_GeneratedDate");

        builder.HasIndex(x => x.EmailSentDate)
            .HasDatabaseName("IX_Payslips_EmailSentDate")
            .IsUnique(false);

        // Query Filter - Soft Delete (varsa)
        // builder.HasQueryFilter(x => !x.IsDeleted);
    }
}