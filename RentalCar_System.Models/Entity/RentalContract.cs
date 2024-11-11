﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentalCar_System.Models.Entity;

public partial class RentalContract
{
    public Guid ContractId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? CarId { get; set; }

    public DateTime RentalDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; }

    public string Feedback { get; set; }

    public int? Rating { get; set; }

    public Guid? PaymentId { get; set; }

    public virtual Car Car { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; }
}