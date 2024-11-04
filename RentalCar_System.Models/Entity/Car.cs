﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentalCar_System.Models.Entity;

public partial class Car
{
    public Guid CarId { get; set; }

    public Guid? UserId { get; set; }

    public string Name { get; set; }

    public string LicensePlate { get; set; }

    public string Brand { get; set; }

    public string Model { get; set; }

    public string Color { get; set; }

    public int? Seats { get; set; }

    public int? Year { get; set; }

    public decimal? Mileage { get; set; }

    public string Status { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }

    public string Image { get; set; }

    public virtual ICollection<CarAddress> CarAddresses { get; set; } = new List<CarAddress>();

    public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();

    public virtual User User { get; set; }
}