﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentalCar_System.Models.Entity;

public partial class Cart
{
    public Guid CartId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? CarId { get; set; }

    public DateTime? DateAdded { get; set; }

    public virtual Car Car { get; set; }

    public virtual User User { get; set; }
}