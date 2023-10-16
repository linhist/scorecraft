using System;
using Scorecraft.Entities;

namespace Scorecraft.Data
{
    public class SofaEntity : Entity<int>
    {
        public int? IdRef { get; set; }

        public DateTime? Last { get; set; }

        public bool? Active { get; set; }
    }
}