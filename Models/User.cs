using System;
using System.Collections.Generic;

namespace Ecommerce_web_app.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool Active { get; set; }

    public string? FullName { get; set; }

    public int? RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Salt { get; set; }

    public virtual Role? Role { get; set; }
}
