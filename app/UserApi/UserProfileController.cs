using Microsoft.AspNetCore.Mvc;

namespace UserApi;

public class UserProfileController(UserManagementDbContext dbContext)
{
    [HttpPost("api/v1/user-profile")]
    public List<UserProfile> Add(UserProfile userProfile)
    {
        dbContext.UserProfiles.Add(userProfile);
        dbContext.SaveChanges();
        
        return dbContext.UserProfiles.ToList();
    }
}