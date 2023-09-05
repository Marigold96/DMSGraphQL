using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions;
public static class UserExtensions
{

    public static string Initials(this UserModel userModel)
    {
        var userDescription = userModel.Description.Split(" ");
        if (userDescription.Count() > 1)
        {
            return userDescription[0].Substring(0, 1).ToUpper() + userDescription[1].Substring(0, 1).ToUpper();
        }
        else
        {
            return userDescription[0].Substring(0, 2).ToUpper();
        }
    }

}
