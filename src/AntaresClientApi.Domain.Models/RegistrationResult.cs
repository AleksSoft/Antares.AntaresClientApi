namespace AntaresClientApi.Domain.Models
{
    public class RegistrationResult
    {
        public bool IsEmailAlreadyExist { get; set; }

        public bool IsClientAlreadyExist { get; set; }

        public bool IsSuccess { get; set; }

        public ClientIdentity ClientIdentity { get; set; }
    }
}
