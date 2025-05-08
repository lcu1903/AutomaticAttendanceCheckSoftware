using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{

    [MaxLength(128)]
    /// <summary>
    /// Mã nhân viên (Key)
    /// </summary>
    public override string Id { get; set; } = null!;
    [JsonIgnore]
    public override string PasswordHash { get; set; } = null!;
    public override string UserName { get; set; } = null!;
    [MaxLength(200)]
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    override public string? PhoneNumber { get; set; }
    override public string Email { get; set; } = null!;

    public DateTime? BirthdayValue { get; set; }
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    [MaxLength(500)]
    public string? Address { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
    public virtual SystemPosition? Position { get; set; }
    public virtual SystemDepartment? Department { get; set; }
    public virtual Student? Student { get; set; }
    public virtual Teacher? Teacher { get; set; }


}