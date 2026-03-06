using Microsoft.AspNetCore.Identity;

namespace ADML_FINANCES.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public bool OnboardingConcluido { get; set; }
    public bool TourAtivo { get; set; }
    public int TourEtapa { get; set; }
}

