namespace CancunSurvivor.Booking.Api.Enums
{
    /// <summary>
    /// The service result.
    /// </summary>
    public enum ServiceResult
    {
        /// <summary>
        /// Service layer performed the operation as expected.
        /// </summary>
        Ok,

        /// <summary>
        /// Nothing was found on service layer.
        /// </summary>
        NotFound,

        /// <summary>
        /// No content returned from service layer.
        /// </summary>
        NoContent,

        /// <summary>
        /// Validation failed on service layer.
        /// </summary>
        ValidationFailed,
    }
}
