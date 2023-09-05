using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models;
public record LoginData(bool IsSuccessful = false, string? AccessToken = null, string? RefreshToken = null);