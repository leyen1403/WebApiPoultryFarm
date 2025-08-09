using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPoultryFarm.Application.Models
{
    public record TokenPair(
         string AccessToken,
         DateTime AccessTokenExpiresAt,
         string RefreshToken,
         DateTime RefreshTokenExpiresAt
     );
}
