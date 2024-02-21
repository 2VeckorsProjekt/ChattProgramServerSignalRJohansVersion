using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProgramProjektOOP;

internal class BannedIP
{
    public string address {  get; private set; }

    public BannedIP(string IP) 
    {
        this.address = IP;
    }
}
