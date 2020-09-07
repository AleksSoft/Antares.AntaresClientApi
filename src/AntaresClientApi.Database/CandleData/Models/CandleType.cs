namespace AntaresClientApi.Database.CandleData.Models
{
    public enum CandleType
    {
        /// <summary>
        /// Unspecified candle type.
        /// </summary>
        None,

        /// <summary>
        /// Indicates one-minute time frame. 
        /// </summary>
        Minute,

        /// <summary>
        /// Indicates one-hour time frame. 
        /// </summary>
        Hour,

        /// <summary>
        /// Indicates one-day time frame. 
        /// </summary>
        Day,

        /// <summary>
        /// Indicates one-month time frame. 
        /// </summary>
        Month
    }
}
