using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdP
{
  public class CustomPasswordOptions:PasswordOptions
  {
    public long Id { get; set; }
    
  }
}
