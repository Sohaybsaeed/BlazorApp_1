namespace RetailChannel.Models.Model
{
    public partial class UserAccess
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? BranchId { get; set; }
        public string FormName { get; set; }
        public bool? FullAccess { get; set; }
        public int? ApplicationFunctionalityId { get; set; }
        public bool? AllowAccess { get; set; }
    }
}
