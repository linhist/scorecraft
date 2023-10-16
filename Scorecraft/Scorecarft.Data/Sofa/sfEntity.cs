using System;
using Scorecraft.Entities;

namespace Scorecraft.Data.Sofa
{
    public class sfEntity : Entity<int>
    {
        public int IdRef { get; set; }

        public DateTime Last { get; set; }

        public bool Active { get; set; }
    }
}