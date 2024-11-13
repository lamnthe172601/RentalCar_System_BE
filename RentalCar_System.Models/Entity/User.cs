﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentalCar_System.Models.Entity;

public partial class User
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = " User";

    public string Email { get; set; }

    public string Password { get; set; }

    public string PhoneNumber { get; set; }

    public string PhotoUrl { get; set; } = "images\\image-profile\\default.jpg";

    public string Role { get; set; } = "customer";

    public string Status { get; set; } = " ACTIVATED";

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
}