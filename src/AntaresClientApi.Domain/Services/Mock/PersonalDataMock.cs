using System;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services.Mock
{
    public class PersonalDataMock: IPersonalData
    {
        private readonly IMyNoSqlServerDataWriter<PersonalDataEntity> _dataWriter;

        public PersonalDataMock(IMyNoSqlServerDataWriter<PersonalDataEntity> dataWriter)
        {
            _dataWriter = dataWriter;
        }

        public async Task<ClientIdentity> RegisterClientAsync(
            string tenantId,
            string requestEmail,
            string requestPhone,
            string requestFullName,
            string requestCountryIso3Code,
            string requestAffiliateCode)
        {
            var iteration = 0;
            PersonalDataEntity entity;
            while (true)
            {
                iteration++;

                try
                {
                    var clientId = (long) (DateTime.UtcNow - DateTime.Parse("2020-01-01")).TotalSeconds;

                    entity = PersonalDataEntity.Generate(tenantId, clientId);
                    entity.Data.Email = requestEmail;
                    entity.Data.FullName = requestFullName;
                    entity.Data.Phone = requestPhone;
                    entity.Data.AffiliateCode = requestAffiliateCode;
                    entity.Data.CountryIso3Code = requestCountryIso3Code;

                    await _dataWriter.InsertAsync(entity);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR in PersonalDataMock.RegisterClientAsync. Iteration: {iteration}");
                    Console.WriteLine(ex);

                    if (iteration > 10)
                        throw;
                }
            }

            return new ClientIdentity()
                {
                    ClientId = entity.Data.ClientId,
                    TenantId = entity.Data.TenantId
            };
        }
    }
}
