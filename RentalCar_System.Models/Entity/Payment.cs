﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentalCar_System.Models.Entity;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid? ContractId { get; set; }

    public Guid? UserId { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; }

    public virtual RentalContract Contract { get; set; }

    public virtual User User { get; set; }
}