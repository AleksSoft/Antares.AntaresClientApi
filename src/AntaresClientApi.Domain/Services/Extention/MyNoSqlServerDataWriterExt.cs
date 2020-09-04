using System;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models.MyNoSql;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services.Extention
{
    public static class MyNoSqlServerDataWriterExt
    {
        public static async Task<TEntity> TryGetAsync<TEntity>(this IMyNoSqlServerDataWriter<TEntity> writer, string partitionKey, string rowKey) where TEntity: IMyNoSqlDbEntity, new()
        {
            try
            {
                var entity = await writer.GetAsync(partitionKey, rowKey);
                return entity;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Row not found"))
                    return default(TEntity);

                throw;
            }
        }

        public static async Task<bool> TryInsertAsync<TEntity>(this IMyNoSqlServerDataWriter<TEntity> writer, TEntity entity) where TEntity : IMyNoSqlDbEntity, new()
        {
            try
            {
                await writer.InsertAsync(entity);
                return true;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Call failed with status code 409 (Conflict)"))
                    return false;

                throw;
            }
        }
    }
}
