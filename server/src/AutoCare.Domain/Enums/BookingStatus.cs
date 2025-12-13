namespace AutoCare.Domain.Enums
{
    /// <summary>
/// Represents the status of a booking throughout its lifecycle
/// Status flow: Pending → Confirmed → InProgress → Completed
/// Can be Cancelled at any stage before Completed
/// </summary>
    public enum BookingStatus
    {
        /// <summary>
    /// Booking created but not yet confirmed by service center
    /// Customer can cancel at this stage
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Booking confirmed by service center staff
    /// Customer can still cancel (may incur penalty)
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Service work has started
    /// Cannot be cancelled by customer
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Service work completed successfully
    /// Terminal state - no further changes allowed
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Booking was cancelled by customer or service center
    /// Terminal state - no further changes allowed
    /// </summary>
    Cancelled = 5

    }
}