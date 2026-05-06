using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public bool IsActive { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedDate { get; set; }
   // public DateTime ModifiedDate { get; set; }
    public int RoleTemplateId { get; set; }
    public bool IsBranchUser { get; set; }
    public string? BranchCode { get; set; }

    


}

public class ApplicationRole : IdentityRole<int>
{
}

public class RoleTemplateDD
{
    public int Value { get; set; }
    public string Text { get; set; }
}