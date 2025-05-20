using System;
using System.Collections.Generic;

namespace CBMSystem.Models;

public partial class EmailSetting
{
    public int Id { get; set; }

    public string Host { get; set; } = null!;

    public int Port { get; set; }

    public bool EnableSsl { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FromEmail { get; set; } = null!;
}
