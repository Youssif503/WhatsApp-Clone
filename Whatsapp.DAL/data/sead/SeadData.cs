using Microsoft.AspNetCore.Identity;

namespace Whatsapp.DAL.data.sead;

public static class SeadData
{
    public static List<IdentityRole> LoadRoles()
    {
        return new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "2"
            }
        };
    }
}