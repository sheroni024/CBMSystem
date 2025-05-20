using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class User
{
    public long UserId { get; set; }

    public string Token { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string IdproofType { get; set; } = null!;

    public string IdproofNumber { get; set; } = null!;

    public string Kycstatus { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public long? RoleId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsActive { get; set; }

    public int? Dob { get; set; }

    public virtual ICollection<Authentication> Authentications { get; set; } = new List<Authentication>();

    public virtual ICollection<FraudAction> FraudActions { get; set; } = new List<FraudAction>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<SecurityAuthentication> SecurityAuthentications { get; set; } = new List<SecurityAuthentication>();
}
