using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CBMSystem.Models;

public partial class CbmsContext : DbContext
{
    public CbmsContext()
    {
    }

    public CbmsContext(DbContextOptions<CbmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Authentication> Authentications { get; set; }

    public virtual DbSet<BillPayment> BillPayments { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<CustomerAccount> CustomerAccounts { get; set; }

    public virtual DbSet<DematAccount> DematAccounts { get; set; }

    public virtual DbSet<EmailSetting> EmailSettings { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FraudAction> FraudActions { get; set; }

    public virtual DbSet<FraudReport> FraudReports { get; set; }

    public virtual DbSet<FraudType> FraudTypes { get; set; }

    public virtual DbSet<Investment> Investments { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SecurityAuthentication> SecurityAuthentications { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.AccountNumber, "UQ_Accounts_AccountNumber").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.AccountType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BranchCodeId).HasColumnName("BranchCodeID");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Currency)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accounts_CustomerAccounts");
        });

        modelBuilder.Entity<Authentication>(entity =>
        {
            entity.HasKey(e => e.AuthId);

            entity.ToTable("Authentication");

            entity.Property(e => e.AuthId)
                .ValueGeneratedNever()
                .HasColumnName("AuthID");
            entity.Property(e => e.LastAttempt).HasColumnType("datetime");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("OTP");
            entity.Property(e => e.Otpexpiry)
                .HasColumnType("datetime")
                .HasColumnName("OTPExpiry");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Authentications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Authentication_Users");
        });

        modelBuilder.Entity<BillPayment>(entity =>
        {
            entity.HasKey(e => e.BillId);

            entity.ToTable("Bill Payments");

            entity.Property(e => e.BillId)
                .ValueGeneratedNever()
                .HasColumnName("BillID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BillerName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Account).WithMany(p => p.BillPayments)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Bill Payments_Accounts");

            entity.HasOne(d => d.Status).WithMany(p => p.BillPayments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Bill Payments_Status");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.BranchCodeId);

            entity.ToTable("Branch");

            entity.Property(e => e.BranchCodeId)
                .ValueGeneratedNever()
                .HasColumnName("BranchCodeID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BranchManager)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(e => e.CardId)
                .ValueGeneratedNever()
                .HasColumnName("CardID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.CardType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Cvv)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("CVV");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Account).WithMany(p => p.Cards)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Cards_Accounts");

            entity.HasOne(d => d.Status).WithMany(p => p.Cards)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Cards_Status");
        });

        modelBuilder.Entity<CustomerAccount>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8752671B1");

            entity.HasIndex(e => e.AadhaarNumber, "UQ__Customer__72CF7959DA625C00").IsUnique();

            entity.HasIndex(e => e.PancardNumber, "UQ__Customer__8ED62C220D491BA0").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534DA9F6F27").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.AadhaarNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.PancardNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PANCardNumber");
            entity.Property(e => e.ProfileImage).IsUnicode(false);
            entity.Property(e => e.SignatureImage).IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<DematAccount>(entity =>
        {
            entity.HasKey(e => e.DematId).HasName("PK__DematAcc__E6B4B80DBE606B20");

            entity.HasIndex(e => e.AccountNumber, "UQ__DematAcc__BE2ACD6FA70D0C9B").IsUnique();

            entity.Property(e => e.DematId).HasColumnName("DematID");
            entity.Property(e => e.AadhaarLinked).HasDefaultValue(false);
            entity.Property(e => e.AccountNumber).HasMaxLength(20);
            entity.Property(e => e.AccountStatus)
                .HasMaxLength(10)
                .HasDefaultValue("Active");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(15, 2)");
            entity.Property(e => e.BrokerName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Panlinked)
                .HasDefaultValue(false)
                .HasColumnName("PANLinked");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.DematAccounts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Customer_Demat");
        });

        modelBuilder.Entity<EmailSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailSet__3214EC0788E17DAF");

            entity.Property(e => e.EnableSsl).HasColumnName("EnableSSL");
            entity.Property(e => e.FromEmail).HasMaxLength(255);
            entity.Property(e => e.Host).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("EmployeeID");
            entity.Property(e => e.BranchCodeId).HasColumnName("BranchCodeID");
            entity.Property(e => e.Departmemt)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.BranchCode).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BranchCodeId)
                .HasConstraintName("FK_Employee_Branch");
        });

        modelBuilder.Entity<FraudAction>(entity =>
        {
            entity.HasKey(e => e.FraudActionId).HasName("PK__FraudAct__177679FBCF2A5065");

            entity.ToTable("FraudAction");

            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FraudReport).WithMany(p => p.FraudActions)
                .HasForeignKey(d => d.FraudReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FraudAction_FraudReport");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.FraudActions)
                .HasForeignKey(d => d.PerformedBy)
                .HasConstraintName("FK_FraudAction_Users");
        });

        modelBuilder.Entity<FraudReport>(entity =>
        {
            entity.HasKey(e => e.FraudReportId).HasName("PK__FraudRep__85081F260DF700F7");

            entity.ToTable("FraudReport");

            entity.Property(e => e.AccountNumber).HasMaxLength(30);
            entity.Property(e => e.ReportedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Received");

            entity.HasOne(d => d.Customer).WithMany(p => p.FraudReports)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FraudReport_CustomerAccounts");

            entity.HasOne(d => d.FraudType).WithMany(p => p.FraudReports)
                .HasForeignKey(d => d.FraudTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FraudReport_FraudType");
        });

        modelBuilder.Entity<FraudType>(entity =>
        {
            entity.HasKey(e => e.FraudTypeId).HasName("PK__FraudTyp__F1C222E31D1DBF5F");

            entity.ToTable("FraudType");

            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Investment>(entity =>
        {
            entity.HasKey(e => e.InvestmentId).HasName("PK__Investme__91D937AB62D849B7");

            entity.Property(e => e.InvestmentId).HasColumnName("InvestmentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DematId).HasColumnName("DematID");
            entity.Property(e => e.InvestmentAmount).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.InvestmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InvestmentName).HasMaxLength(255);
            entity.Property(e => e.InvestmentType).HasMaxLength(50);
            entity.Property(e => e.Nav)
                .HasColumnType("decimal(15, 4)")
                .HasColumnName("NAV");
            entity.Property(e => e.PolicyNumber).HasMaxLength(50);
            entity.Property(e => e.RedemptionDate).HasColumnType("datetime");
            entity.Property(e => e.SchemeId).HasColumnName("SchemeID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.StockPrice).HasColumnType("decimal(15, 4)");
            entity.Property(e => e.UnitsPurchased).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.AppliedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.InterestRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.LoanAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoanType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Status).WithMany(p => p.Loans)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Loans_Status");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId)
                .ValueGeneratedNever()
                .HasColumnName("NotificationID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.PaymentId)
                .ValueGeneratedNever()
                .HasColumnName("PaymentID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PayeeName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Account).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Payments_Accounts");

            entity.HasOne(d => d.Status).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Payments_Status");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.Permissions).HasColumnType("text");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SecurityAuthentication>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("Security & Authentication");

            entity.Property(e => e.LogId)
                .ValueGeneratedNever()
                .HasColumnName("LogID");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LoginTime).HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Status).WithMany(p => p.SecurityAuthentications)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Security & Authentication_Status");

            entity.HasOne(d => d.User).WithMany(p => p.SecurityAuthentications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Security & Authentication_Users");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BalanceAfterTransaction).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BranchCodeId).HasColumnName("BranchCodeID");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AccountNumberNavigation).WithMany(p => p.Transactions)
                .HasPrincipalKey(p => p.AccountNumber)
                .HasForeignKey(d => d.AccountNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Accounts");

            entity.HasOne(d => d.BranchCode).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BranchCodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Branch");

            entity.HasOne(d => d.Status).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Transactions_Status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Dob)
                .HasComputedColumnSql("(CONVERT([int],format([DateOfBirth],'yyyyMMdd')))", false)
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IdproofNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDProofNumber");
            entity.Property(e => e.IdproofType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDProofType");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsDelete).HasColumnName("isDelete");
            entity.Property(e => e.Kycstatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("KYCStatus");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
