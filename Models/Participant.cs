using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MatchMaker.Models
{
    public class Participant
    {
        public string Name { get; set; }
        public string Match { get; set; }
    }
}
