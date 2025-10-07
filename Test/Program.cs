using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVirtualPOSService, VirtualPOSService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IAccessControlService, AccessControlService>();
builder.Services.AddScoped<IStudentService, StudentService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyMinimum32CharactersLongForProductionUse";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "University API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }}, Array.Empty<string>() }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
}

app.Run();

// ============================================================================
// ENTITIES
// ============================================================================

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

public enum Gender { Male, Female, Other }
public enum StudentStatus { Active, Graduated, Suspended, Withdrawn }
public enum CardType { Student, Staff, Visitor }
public enum CardStatus { Active, Blocked, Lost, Expired }
public enum ZoneType { Cafeteria, Library, Gym, Dormitory, Parking, Lab }
public enum AccessType { Entry, Exit }
public enum TransactionType { Deposit, Withdraw, AccessFee, SubscriptionFee, TuitionPayment, DormitoryPayment, LibraryFine, EventPayment, Refund, Transfer }
public enum TransactionStatus { Pending, Completed, Failed, Cancelled, Refunded }
public enum PaymentType { Tuition, Dormitory, Library, Event, Laboratory, Other }
public enum PaymentStatus { Pending, Processing, Completed, Failed, Cancelled, Refunded }
public enum VirtualPOSProvider { Get724, NestPay, Iyzico, PayTR, Test }
public enum POSTransactionStatus { Initiated, ThreeDSRequired, ThreeDSCompleted, Processing, Success, Failed, Cancelled }
public enum SubscriptionStatus { Active, Expired, Cancelled }

public class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public virtual Student? Student { get; set; }
    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class Faculty : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid FacultyId { get; set; }
    public virtual Faculty Faculty { get; set; } = null!;
}

public class Student : BaseEntity
{
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public string StudentNumber { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public virtual Department Department { get; set; } = null!;
    public string? ScholarshipType { get; set; }
    public decimal ScholarshipPercentage { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
}

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? PersonId { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}

public class Card : BaseEntity
{
    public string CardNumber { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public CardType CardType { get; set; }
    public CardStatus Status { get; set; } = CardStatus.Active;
    public DateTime? ExpiryDate { get; set; }
}

public class Transaction : BaseEntity
{
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public string TransactionNumber { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public decimal Amount { get; set; }
    public decimal BeforeBalance { get; set; }
    public decimal AfterBalance { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}

public class Payment : BaseEntity
{
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public string PaymentNumber { get; set; } = string.Empty;
    public PaymentType Type { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? Year { get; set; }
    public int? Semester { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public Guid? TransactionId { get; set; }
    public virtual Transaction? Transaction { get; set; }
}

public class VirtualPOSTransaction : BaseEntity
{
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public string TransactionId { get; set; } = string.Empty;
    public VirtualPOSProvider Provider { get; set; }
    public POSTransactionStatus Status { get; set; } = POSTransactionStatus.Initiated;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? CardHolderName { get; set; }
    public string? MaskedCardNumber { get; set; }
    public bool Require3DSecure { get; set; } = true;
    public string? ThreeDSHtmlContent { get; set; }
    public string? AuthCode { get; set; }
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }
    public string? IpAddress { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? SystemTransactionId { get; set; }
    public virtual Transaction? SystemTransaction { get; set; }
}

public class Zone : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public ZoneType Type { get; set; }
    public decimal? AccessFee { get; set; }
    public bool RequiresSubscription { get; set; }
}

public class AccessLog : BaseEntity
{
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public Guid ZoneId { get; set; }
    public virtual Zone Zone { get; set; } = null!;
    public Guid? CardId { get; set; }
    public AccessType AccessType { get; set; }
    public DateTime AccessTime { get; set; } = DateTime.UtcNow;
    public bool IsGranted { get; set; } = true;
    public bool IsSubscriptionAccess { get; set; }
    public decimal ChargedAmount { get; set; }
}

public class Subscription : BaseEntity
{
    public string SubscriptionNumber { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    public Guid ZoneId { get; set; }
    public virtual Zone Zone { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string SelectedDays { get; set; } = string.Empty;
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
}

// ============================================================================
// DATABASE
// ============================================================================

public class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<AccessLog> AccessLogs { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<VirtualPOSTransaction> VirtualPOSTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>().HasIndex(p => p.NationalId).IsUnique();
        modelBuilder.Entity<Person>().HasIndex(p => p.Email).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(s => s.StudentNumber).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Card>().HasIndex(c => c.CardNumber).IsUnique();
        modelBuilder.Entity<Transaction>().HasIndex(t => t.TransactionNumber).IsUnique();
        modelBuilder.Entity<Payment>().HasIndex(p => p.PaymentNumber).IsUnique();

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Transaction)
            .WithMany()
            .HasForeignKey(p => p.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VirtualPOSTransaction>()
            .HasOne(v => v.SystemTransaction)
            .WithMany()
            .HasForeignKey(v => v.SystemTransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AccessLog>().Property(a => a.ChargedAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Subscription>().Property(s => s.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Zone>().Property(z => z.AccessFee).HasPrecision(18, 2);
        modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Transaction>().Property(t => t.BeforeBalance).HasPrecision(18, 2);
        modelBuilder.Entity<Transaction>().Property(t => t.AfterBalance).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<VirtualPOSTransaction>().Property(v => v.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Student>().Property(s => s.ScholarshipPercentage).HasPrecision(5, 2);
    }
}

public interface IUnitOfWork : IDisposable
{
    IQueryable<T> Query<T>() where T : BaseEntity;
    Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity;
    Task AddAsync<T>(T entity) where T : BaseEntity;
    Task<bool> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly UniversityDbContext _context;
    public UnitOfWork(UniversityDbContext context) => _context = context;
    public IQueryable<T> Query<T>() where T : BaseEntity => _context.Set<T>().AsQueryable();
    public async Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity => await _context.Set<T>().FindAsync(id);
    public async Task AddAsync<T>(T entity) where T : BaseEntity => await _context.Set<T>().AddAsync(entity);
    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    public void Dispose() => _context.Dispose();
}

// ============================================================================
// SERVICES
// ============================================================================

public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto dto);
    Task<TokenDto> RegisterAsync(RegisterDto dto);
    Task<UserProfileDto> GetMyProfileAsync(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Query<User>()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return await GenerateTokenAsync(user);
    }

    public async Task<TokenDto> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            PersonId = dto.PersonId
        };

        await _unitOfWork.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var role = await _unitOfWork.Query<Role>().FirstOrDefaultAsync(r => r.Name == "Student");
        if (role != null)
        {
            await _unitOfWork.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
            await _unitOfWork.SaveChangesAsync();
        }

        return await GenerateTokenAsync(user);
    }

    public async Task<UserProfileDto> GetMyProfileAsync(Guid userId)
    {
        var user = await _unitOfWork.Query<User>()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("User not found");

        Person? person = null;
        if (user.PersonId.HasValue)
        {
            person = await _unitOfWork.Query<Person>()
                .Include(p => p.Student).ThenInclude(s => s!.Department)
                .Include(p => p.Cards)
                .FirstOrDefaultAsync(p => p.Id == user.PersonId.Value);
        }

        return new UserProfileDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            PersonId = person?.Id,
            FirstName = person?.FirstName,
            LastName = person?.LastName,
            PhoneNumber = person?.PhoneNumber,
            StudentNumber = person?.Student?.StudentNumber,
            DepartmentName = person?.Student?.Department?.Name
        };
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _unitOfWork.GetByIdAsync<User>(userId) ?? throw new Exception("User not found");

        if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        user.PasswordHash = HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        return await _unitOfWork.SaveChangesAsync();
    }

    private async Task<TokenDto> GenerateTokenAsync(User user)
    {
        var roles = await _unitOfWork.Query<UserRole>()
            .Where(ur => ur.UserId == user.Id)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        if (user.PersonId.HasValue)
            claims.Add(new Claim("PersonId", user.PersonId.Value.ToString()));

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var expires = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new TokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expires
        };
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    private bool VerifyPassword(string password, string hash) => HashPassword(password) == hash;
}

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto);
    Task<List<Transaction>> GetMyTransactionsAsync(Guid personId, int limit = 50);
    Task<decimal> GetBalanceAsync(Guid personId);
}

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    public TransactionService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<decimal> GetBalanceAsync(Guid personId)
    {
        var transactions = await _unitOfWork.Query<Transaction>()
            .Where(t => t.PersonId == personId && t.Status == TransactionStatus.Completed)
            .ToListAsync();

        return transactions.Sum(t =>
            (t.Type == TransactionType.Deposit || t.Type == TransactionType.Refund) ? t.Amount : -t.Amount);
    }

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto)
    {
        var currentBalance = await GetBalanceAsync(dto.PersonId);
        var newBalance = (dto.Type == TransactionType.Deposit || dto.Type == TransactionType.Refund)
            ? currentBalance + dto.Amount
            : currentBalance - dto.Amount;

        if (dto.Type != TransactionType.Deposit && dto.Type != TransactionType.Refund && currentBalance < dto.Amount)
            throw new Exception("Insufficient balance");

        var transaction = new Transaction
        {
            PersonId = dto.PersonId,
            TransactionNumber = $"TRX{DateTime.Now:yyyyMMddHHmmss}{DateTime.Now.Ticks % 1000:D3}",
            Type = dto.Type,
            Amount = dto.Amount,
            BeforeBalance = currentBalance,
            AfterBalance = newBalance,
            Description = dto.Description,
            Status = TransactionStatus.Completed,
            ReferenceNumber = dto.ReferenceNumber
        };

        await _unitOfWork.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        return transaction;
    }

    public async Task<List<Transaction>> GetMyTransactionsAsync(Guid personId, int limit = 50)
    {
        return await _unitOfWork.Query<Transaction>()
            .Where(t => t.PersonId == personId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(limit)
            .ToListAsync();
    }
}

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(CreatePaymentDto dto);
    Task<List<Payment>> GetMyPendingPaymentsAsync(Guid personId);
    Task<List<Payment>> GetMyPaymentsAsync(Guid personId);
    Task<Payment> PayAsync(Guid paymentId, Guid personId);
}

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionService _transactionService;

    public PaymentService(IUnitOfWork unitOfWork, ITransactionService transactionService)
    {
        _unitOfWork = unitOfWork;
        _transactionService = transactionService;
    }

    public async Task<Payment> CreatePaymentAsync(CreatePaymentDto dto)
    {
        var payment = new Payment
        {
            PersonId = dto.PersonId,
            PaymentNumber = $"PAY{DateTime.Now:yyyyMMdd}{DateTime.Now.Ticks % 10000:D4}",
            Type = dto.Type,
            Amount = dto.Amount,
            Description = dto.Description,
            Year = dto.Year,
            Semester = dto.Semester,
            DueDate = dto.DueDate,
            Status = PaymentStatus.Pending
        };

        await _unitOfWork.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        return payment;
    }

    public async Task<List<Payment>> GetMyPendingPaymentsAsync(Guid personId)
    {
        return await _unitOfWork.Query<Payment>()
            .Where(p => p.PersonId == personId && p.Status == PaymentStatus.Pending)
            .OrderBy(p => p.DueDate)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetMyPaymentsAsync(Guid personId)
    {
        return await _unitOfWork.Query<Payment>()
            .Where(p => p.PersonId == personId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Payment> PayAsync(Guid paymentId, Guid personId)
    {
        var payment = await _unitOfWork.GetByIdAsync<Payment>(paymentId) ?? throw new Exception("Payment not found");
        if (payment.PersonId != personId) throw new Exception("Unauthorized");
        if (payment.Status != PaymentStatus.Pending) throw new Exception("Payment already processed");

        var transactionType = payment.Type switch
        {
            PaymentType.Tuition => TransactionType.TuitionPayment,
            PaymentType.Dormitory => TransactionType.DormitoryPayment,
            PaymentType.Library => TransactionType.LibraryFine,
            PaymentType.Event => TransactionType.EventPayment,
            _ => TransactionType.Withdraw
        };

        var transaction = await _transactionService.CreateTransactionAsync(new CreateTransactionDto
        {
            PersonId = personId,
            Type = transactionType,
            Amount = payment.Amount,
            Description = $"Payment: {payment.Description}",
            ReferenceNumber = payment.PaymentNumber
        });

        payment.Status = PaymentStatus.Completed;
        payment.PaidDate = DateTime.UtcNow;
        payment.TransactionId = transaction.Id;
        await _unitOfWork.SaveChangesAsync();

        return payment;
    }
}

public interface IVirtualPOSService
{
    Task<VirtualPOSInitiateResponse> InitiatePaymentAsync(InitiatePaymentDto dto);
    Task<VirtualPOSTransaction> Complete3DSecureAsync(Complete3DSecureDto dto);
}

public class VirtualPOSService : IVirtualPOSService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionService _transactionService;
    private readonly IConfiguration _configuration;

    public VirtualPOSService(IUnitOfWork unitOfWork, ITransactionService transactionService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _transactionService = transactionService;
        _configuration = configuration;
    }

    public async Task<VirtualPOSInitiateResponse> InitiatePaymentAsync(InitiatePaymentDto dto)
    {
        var posTransaction = new VirtualPOSTransaction
        {
            PersonId = dto.PersonId,
            TransactionId = $"POS{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            Provider = dto.Provider,
            Amount = dto.Amount,
            CardHolderName = dto.CardHolderName,
            MaskedCardNumber = $"**** **** **** {dto.CardNumber[^4..]}",
            IpAddress = dto.IpAddress,
            Status = POSTransactionStatus.Initiated
        };

        await _unitOfWork.AddAsync(posTransaction);
        await _unitOfWork.SaveChangesAsync();

        var callbackUrl = $"{_configuration["AppSettings:BaseUrl"]}/api/pos/callback";
        var htmlContent = dto.Provider switch
        {
            VirtualPOSProvider.Test => GenerateTestHTML(posTransaction, callbackUrl),
            VirtualPOSProvider.Get724 => GenerateGet724HTML(posTransaction, dto, callbackUrl),
            VirtualPOSProvider.NestPay => GenerateNestPayHTML(posTransaction, dto, callbackUrl),
            _ => throw new Exception("Unsupported provider")
        };

        posTransaction.ThreeDSHtmlContent = htmlContent;
        posTransaction.Status = POSTransactionStatus.ThreeDSRequired;
        await _unitOfWork.SaveChangesAsync();

        return new VirtualPOSInitiateResponse
        {
            TransactionId = posTransaction.TransactionId,
            ThreeDSHtmlContent = htmlContent
        };
    }

    public async Task<VirtualPOSTransaction> Complete3DSecureAsync(Complete3DSecureDto dto)
    {
        var posTransaction = await _unitOfWork.Query<VirtualPOSTransaction>()
            .FirstOrDefaultAsync(t => t.TransactionId == dto.TransactionId) ?? throw new Exception("Transaction not found");

        bool isSuccess = dto.AuthCode != null && dto.ResponseCode == "00";

        if (isSuccess)
        {
            var transaction = await _transactionService.CreateTransactionAsync(new CreateTransactionDto
            {
                PersonId = posTransaction.PersonId,
                Type = TransactionType.Deposit,
                Amount = posTransaction.Amount,
                Description = $"POS Deposit via {posTransaction.Provider}",
                ReferenceNumber = posTransaction.TransactionId
            });

            posTransaction.Status = POSTransactionStatus.Success;
            posTransaction.AuthCode = dto.AuthCode;
            posTransaction.ResponseCode = dto.ResponseCode;
            posTransaction.ResponseMessage = "Transaction successful";
            posTransaction.CompletedAt = DateTime.UtcNow;
            posTransaction.SystemTransactionId = transaction.Id;
        }
        else
        {
            posTransaction.Status = POSTransactionStatus.Failed;
            posTransaction.ResponseCode = dto.ResponseCode;
            posTransaction.ResponseMessage = dto.ResponseMessage ?? "Transaction failed";
        }

        await _unitOfWork.SaveChangesAsync();
        return posTransaction;
    }

    private string GenerateTestHTML(VirtualPOSTransaction transaction, string callbackUrl)
    {
        return $@"
        <html><head><title>Test 3D Secure</title></head><body>
        <h2>Test Payment</h2>
        <p>Amount: {transaction.Amount:C}</p>
        <form method='POST' action='{callbackUrl}'>
            <input type='hidden' name='TransactionId' value='{transaction.TransactionId}' />
            <input type='hidden' name='AuthCode' value='TEST123' />
            <input type='hidden' name='ResponseCode' value='00' />
            <button type='submit' style='padding:10px 20px;background:#4CAF50;color:white;border:none;cursor:pointer;'>
                Approve Payment
            </button>
        </form>
        </body></html>";
    }

    private string GenerateGet724HTML(VirtualPOSTransaction transaction, InitiatePaymentDto dto, string callbackUrl)
    {
        var merchantId = _configuration["Get724:MerchantId"];
        var apiUrl = _configuration["Get724:ApiUrl"];

        return $@"
        <html><body onload='document.forms[0].submit()'>
        <form method='POST' action='{apiUrl}'>
            <input type='hidden' name='MerchantId' value='{merchantId}' />
            <input type='hidden' name='TransactionId' value='{transaction.TransactionId}' />
            <input type='hidden' name='Amount' value='{transaction.Amount:F2}' />
            <input type='hidden' name='CardNumber' value='{dto.CardNumber}' />
            <input type='hidden' name='CVV' value='{dto.CVV}' />
            <input type='hidden' name='SuccessUrl' value='{callbackUrl}' />
        </form></body></html>";
    }

    private string GenerateNestPayHTML(VirtualPOSTransaction transaction, InitiatePaymentDto dto, string callbackUrl)
    {
        var clientId = _configuration["NestPay:ClientId"];
        var storeKey = _configuration["NestPay:StoreKey"];
        var apiUrl = _configuration["NestPay:ApiUrl"];

        var hashData = $"{clientId}{transaction.TransactionId}{transaction.Amount:F2}{storeKey}";
        using var sha1 = SHA1.Create();
        var hash = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(hashData)));

        return $@"
        <html><body onload='document.forms[0].submit()'>
        <form method='POST' action='{apiUrl}'>
            <input type='hidden' name='clientid' value='{clientId}' />
            <input type='hidden' name='oid' value='{transaction.TransactionId}' />
            <input type='hidden' name='amount' value='{transaction.Amount:F2}' />
            <input type='hidden' name='pan' value='{dto.CardNumber}' />
            <input type='hidden' name='cv2' value='{dto.CVV}' />
            <input type='hidden' name='hash' value='{hash}' />
            <input type='hidden' name='okUrl' value='{callbackUrl}' />
            <input type='hidden' name='failUrl' value='{callbackUrl}' />
        </form></body></html>";
    }
}

public interface ICardService
{
    Task<List<CardDto>> GetMyCardsAsync(Guid personId);
    Task<CardDto> GenerateQRCodeAsync(Guid cardId);
}

public class CardService : ICardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionService _transactionService;

    public CardService(IUnitOfWork unitOfWork, ITransactionService transactionService)
    {
        _unitOfWork = unitOfWork;
        _transactionService = transactionService;
    }

    public async Task<List<CardDto>> GetMyCardsAsync(Guid personId)
    {
        var cards = await _unitOfWork.Query<Card>()
            .Where(c => c.PersonId == personId)
            .ToListAsync();

        var balance = await _transactionService.GetBalanceAsync(personId);

        return cards.Select(c => new CardDto
        {
            Id = c.Id,
            CardNumber = c.CardNumber,
            CardType = c.CardType.ToString(),
            Status = c.Status.ToString(),
            Balance = balance,
            QRCode = GenerateQRCode(c),
            IsActive = c.Status == CardStatus.Active
        }).ToList();
    }

    public async Task<CardDto> GenerateQRCodeAsync(Guid cardId)
    {
        var card = await _unitOfWork.Query<Card>()
            .FirstOrDefaultAsync(c => c.Id == cardId) ?? throw new Exception("Card not found");

        var balance = await _transactionService.GetBalanceAsync(card.PersonId);

        return new CardDto
        {
            Id = card.Id,
            CardNumber = card.CardNumber,
            CardType = card.CardType.ToString(),
            Status = card.Status.ToString(),
            Balance = balance,
            QRCode = GenerateQRCode(card),
            IsActive = card.Status == CardStatus.Active
        };
    }

    private string GenerateQRCode(Card card)
    {
        var data = new { CardId = card.Id, CardNumber = card.CardNumber, Timestamp = DateTime.UtcNow.Ticks };
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)));
    }
}

public interface IAccessControlService
{
    Task<List<AccessLog>> GetMyAccessLogsAsync(Guid personId, int limit);
}

public class AccessControlService : IAccessControlService
{
    private readonly IUnitOfWork _unitOfWork;
    public AccessControlService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<AccessLog>> GetMyAccessLogsAsync(Guid personId, int limit = 50)
    {
        return await _unitOfWork.Query<AccessLog>()
            .Where(a => a.PersonId == personId)
            .Include(a => a.Zone)
            .OrderByDescending(a => a.AccessTime)
            .Take(limit)
            .ToListAsync();
    }
}

public interface IStudentService
{
    Task<Student> CreateStudentAsync(CreateStudentDto dto);
}

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    public StudentService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Student> CreateStudentAsync(CreateStudentDto dto)
    {
        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            NationalId = dto.NationalId,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender
        };

        await _unitOfWork.AddAsync(person);
        await _unitOfWork.SaveChangesAsync();

        var student = new Student
        {
            PersonId = person.Id,
            StudentNumber = $"STU{DateTime.Now:yyyyMMdd}{DateTime.Now.Ticks % 10000:D4}",
            DepartmentId = dto.DepartmentId,
            ScholarshipType = dto.ScholarshipType,
            ScholarshipPercentage = dto.ScholarshipPercentage
        };

        await _unitOfWork.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        return student;
    }
}

// ============================================================================
// DTOs
// ============================================================================

public record LoginDto(string Username, string Password);
public record RegisterDto(string Username, string Email, string Password, Guid? PersonId);
public record TokenDto { public string AccessToken { get; set; } = ""; public DateTime ExpiresAt { get; set; } }
public record ApiResponse<T> { public bool Success { get; set; } public T? Data { get; set; } public string Message { get; set; } = ""; }

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public Guid? PersonId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentNumber { get; set; }
    public string? DepartmentName { get; set; }
}

public class CardDto
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string QRCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
public record CreateStudentDto(string FirstName, string LastName, string NationalId, string Email, string? PhoneNumber, DateTime DateOfBirth, Gender Gender, Guid DepartmentId, string? ScholarshipType, decimal ScholarshipPercentage);
public record CreateTransactionDto { public Guid PersonId { get; init; } public TransactionType Type { get; init; } public decimal Amount { get; init; } public string Description { get; init; } = ""; public string? ReferenceNumber { get; init; } }
public record CreatePaymentDto { public Guid PersonId { get; init; } public PaymentType Type { get; init; } public decimal Amount { get; init; } public string Description { get; init; } = ""; public int? Year { get; init; } public int? Semester { get; init; } public DateTime DueDate { get; init; } }
public record InitiatePaymentDto { public Guid PersonId { get; init; } public VirtualPOSProvider Provider { get; init; } public decimal Amount { get; init; } public string CardNumber { get; init; } = ""; public string CardHolderName { get; init; } = ""; public string CVV { get; init; } = ""; public int ExpiryMonth { get; init; } public int ExpiryYear { get; init; } public string? IpAddress { get; init; } }
public record Complete3DSecureDto { public string TransactionId { get; init; } = ""; public string? AuthCode { get; init; } public string? ResponseCode { get; init; } public string? ResponseMessage { get; init; } }
public class VirtualPOSInitiateResponse { public string TransactionId { get; set; } = ""; public string ThreeDSHtmlContent { get; set; } = ""; }

// ============================================================================
// CONTROLLERS
// ============================================================================

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<TokenDto>>> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(new ApiResponse<TokenDto> { Success = true, Data = result, Message = "Login successful" });
        }
        catch (Exception ex)
        {
            return Unauthorized(new ApiResponse<TokenDto> { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<TokenDto>>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(new ApiResponse<TokenDto> { Success = true, Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<TokenDto> { Success = false, Message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var profile = await _authService.GetMyProfileAsync(userId);
        return Ok(new ApiResponse<UserProfileDto> { Success = true, Data = profile });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _authService.ChangePasswordAsync(userId, dto);
            return Ok(new ApiResponse<bool> { Success = true, Message = "Password changed" });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool> { Success = false, Message = ex.Message });
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccessControlService _accessControlService;
    private readonly IPaymentService _paymentService;

    public ProfileController(ITransactionService transactionService, IAccessControlService accessControlService, IPaymentService paymentService)
    {
        _transactionService = transactionService;
        _accessControlService = accessControlService;
        _paymentService = paymentService;
    }

    [HttpGet("balance")]
    public async Task<ActionResult<ApiResponse<object>>> GetBalance()
    {
        var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
        var balance = await _transactionService.GetBalanceAsync(personId);
        return Ok(new ApiResponse<object> { Success = true, Data = new { Balance = balance } });
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<List<Transaction>>>> GetTransactions([FromQuery] int limit = 50)
    {
        var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
        var transactions = await _transactionService.GetMyTransactionsAsync(personId, limit);
        return Ok(new ApiResponse<List<Transaction>> { Success = true, Data = transactions });
    }

    [HttpGet("access-logs")]
    public async Task<ActionResult<ApiResponse<List<AccessLog>>>> GetAccessLogs([FromQuery] int limit = 50)
    {
        var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
        var logs = await _accessControlService.GetMyAccessLogsAsync(personId, limit);
        return Ok(new ApiResponse<List<AccessLog>> { Success = true, Data = logs });
    }

    [HttpGet("pending-payments")]
    public async Task<ActionResult<ApiResponse<List<Payment>>>> GetPendingPayments()
    {
        var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
        var payments = await _paymentService.GetMyPendingPaymentsAsync(personId);
        return Ok(new ApiResponse<List<Payment>> { Success = true, Data = payments });
    }

    [HttpGet("payments")]
    public async Task<ActionResult<ApiResponse<List<Payment>>>> GetPayments()
    {
        var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
        var payments = await _paymentService.GetMyPaymentsAsync(personId);
        return Ok(new ApiResponse<List<Payment>> { Success = true, Data = payments });
    }

    [HttpPost("pay/{paymentId}")]
    public async Task<ActionResult<ApiResponse<Payment>>> Pay(Guid paymentId)
    {
        try
        {
            var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
            var payment = await _paymentService.PayAsync(paymentId, personId);
            return Ok(new ApiResponse<Payment> { Success = true, Data = payment, Message = "Payment completed" });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<Payment> { Success = false, Message = ex.Message });
        }
    }
}

[ApiController]
[Route("api/pos")]
public class POSController : ControllerBase
{
    private readonly IVirtualPOSService _virtualPOSService;
    public POSController(IVirtualPOSService virtualPOSService) => _virtualPOSService = virtualPOSService;

    [HttpPost("initiate")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<VirtualPOSInitiateResponse>>> Initiate([FromBody] InitiatePaymentDto dto)
    {
        try
        {
            var result = await _virtualPOSService.InitiatePaymentAsync(dto);
            return Ok(new ApiResponse<VirtualPOSInitiateResponse> { Success = true, Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<VirtualPOSInitiateResponse> { Success = false, Message = ex.Message });
        }
    }

    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback([FromForm] Complete3DSecureDto dto)
    {
        try
        {
            var result = await _virtualPOSService.Complete3DSecureAsync(dto);
            if (result.Status == POSTransactionStatus.Success)
                return Content($"<html><body><h2>Payment Successful!</h2><p>{result.Amount:C} added to your account.</p></body></html>", "text/html");
            else
                return Content($"<html><body><h2>Payment Failed</h2><p>{result.ResponseMessage}</p></body></html>", "text/html");
        }
        catch (Exception ex)
        {
            return Content($"<html><body><h2>Error</h2><p>{ex.Message}</p></body></html>", "text/html");
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;
    public CardsController(ICardService cardService) => _cardService = cardService;

    [HttpGet("my-cards")]
    public async Task<ActionResult<ApiResponse<List<CardDto>>>> GetMyCards()
    {
        var personId = Guid.Parse(User.FindFirst("PersonId")!.Value);
        var cards = await _cardService.GetMyCardsAsync(personId);
        return Ok(new ApiResponse<List<CardDto>> { Success = true, Data = cards });
    }

    [HttpGet("{cardId}/qr")]
    public async Task<ActionResult<ApiResponse<CardDto>>> GetQR(Guid cardId)
    {
        var card = await _cardService.GenerateQRCodeAsync(cardId);
        return Ok(new ApiResponse<CardDto> { Success = true, Data = card });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IPaymentService _paymentService;

    public AdminController(IStudentService studentService, IPaymentService paymentService)
    {
        _studentService = studentService;
        _paymentService = paymentService;
    }

    [HttpPost("students")]
    public async Task<ActionResult<ApiResponse<Student>>> CreateStudent([FromBody] CreateStudentDto dto)
    {
        var student = await _studentService.CreateStudentAsync(dto);
        return Ok(new ApiResponse<Student> { Success = true, Data = student });
    }

    [HttpPost("payments")]
    public async Task<ActionResult<ApiResponse<Payment>>> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        var payment = await _paymentService.CreatePaymentAsync(dto);
        return Ok(new ApiResponse<Payment> { Success = true, Data = payment });
    }
}

// ============================================================================
// DATA SEEDER
// ============================================================================

public static class DataSeeder
{
    public static async Task SeedAsync(UniversityDbContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Staff" },
                new Role { Name = "Student" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any(u => u.Username == "admin"))
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var admin = new User
            {
                Username = "admin",
                Email = "admin@university.edu",
                PasswordHash = HashPassword("Admin123!")
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();

            await context.UserRoles.AddAsync(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
            await context.SaveChangesAsync();
        }

        if (!context.Faculties.Any())
        {
            var faculty = new Faculty { Name = "Engineering", Code = "ENG" };
            await context.Faculties.AddAsync(faculty);
            await context.SaveChangesAsync();

            await context.Departments.AddAsync(new Department
            {
                Name = "Computer Engineering",
                Code = "CE",
                FacultyId = faculty.Id
            });
            await context.SaveChangesAsync();
        }

        if (!context.Zones.Any())
        {
            context.Zones.Add(new Zone
            {
                Name = "Cafeteria",
                Code = "CAF",
                Type = ZoneType.Cafeteria,
                AccessFee = 25m
            });
            await context.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }
}