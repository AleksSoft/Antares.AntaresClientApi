namespace AntaresClientApi.Domain.Models
{
    public class PersonalData
    {
        public PersonalData(string tenantId, long clientId)
        {
            ClientId = clientId;
            TenantId = tenantId;
        }

        public PersonalData()
        {
        }

        public long ClientId { get; set; }
        public string TenantId { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string CountryIso3Code { get; set; }
        public string AffiliateCode { get; set; }
    }
}
