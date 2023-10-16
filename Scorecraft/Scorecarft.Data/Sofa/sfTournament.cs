using Scorecraft.Data.Sofa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorecarft.Data.Sofa
{
    public class sfTournament : sfEntity
    {
        public string Slug { get; set; }

        public string Colors { get; set; }

        public string Logo { get; set; }

        public virtual sfRegion Region { get; set; }
    }
}
