namespace mapsProjAPI.Models
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int? VerificationCode { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? ValidUntill { get; set; }
        public string? Description { get; set; }
        public int AttemptCounter { get; set; }
        public DateTime? LockedForLogin { get; set; }
    }
}