using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversitySystemMVC.Hasher
{
    public class Passphrase
    {
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}