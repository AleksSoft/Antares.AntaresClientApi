using System;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApiTests
{
    public static class MyNoSqlServerDataWriterHelper
    {
        public static async Task<TEntity> GetOrDefaultAsync<TEntity>(this IMyNoSqlServerDataWriter<TEntity> writer, string partitionKey, string rowKey) where TEntity: IMyNoSqlDbEntity, new()
        {
            try
            {
                var entity = await writer.GetAsync(partitionKey, rowKey);
                return entity;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Row not found"))
                    return default(TEntity);

                throw;
            }
        }
    }
}
