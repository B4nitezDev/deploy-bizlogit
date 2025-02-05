using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace deploy_cli.Entities
{
    public class Config
    {
        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
