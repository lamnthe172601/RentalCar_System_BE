﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentalCar_System.Models.Entity;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string FullName { get; set; } = "User";

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Image { get; set; } = "/images/default.jpg";

    public string Role { get; set; }

    public string Status { get; set; } = "ACTIVATED";

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
}