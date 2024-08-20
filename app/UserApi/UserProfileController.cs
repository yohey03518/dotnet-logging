using Microsoft.AspNetCore.Mvc;
using UserApi.Entities;

namespace UserApi;

public class UserProfileController(UserManagementDbContext dbContext)
{
    [HttpPost("api/v1/user-profile")]
    public List<UserProfile> Add([FromBody] UserProfile userProfile)
    {
        dbContext.UserProfiles.Add(userProfile);
        dbContext.SaveChanges();
        
        return dbContext.UserProfiles.ToList();
    }
    
    [HttpPut("api/v1/user-profile")]
    public UserProfile Update([FromBody] UserProfile userProfile)
    {
        var profile = dbContext.UserProfiles.First(x => x.Id == userProfile.Id);
        profile.FirstName = userProfile.FirstName;
        profile.LastName = userProfile.LastName;
        profile.Email = userProfile.Email;
        profile.ModifiedBy = userProfile.ModifiedBy;
        profile.ModifiedOn = DateTime.Now;
        
        dbContext.SaveChanges();

        return profile;
    }
}