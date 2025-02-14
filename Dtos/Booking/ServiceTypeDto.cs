﻿using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Dtos.Booking
{
    public class ServiceTypeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }   
    }
}
