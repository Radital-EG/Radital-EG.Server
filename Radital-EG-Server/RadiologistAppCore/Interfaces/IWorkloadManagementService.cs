using RadiologistAppCore.DTOs;

namespace RadiologistAppCore.Interfaces
{
    public interface IWorkloadManagementService
    {
        /// <summary>
        /// Fetches all reporting requests assigned to the authenticated radiologist.
        /// </summary>
        Task<IEnumerable<RadiologistRequestResponseDto>> GetAssignedRequestsAsync(Guid radiologistId);

        /// <summary>
        /// Fetches a single assigned request by Id.
        /// Throws <see cref="KeyNotFoundException"/> if not found.
        /// Throws <see cref="UnauthorizedAccessException"/> if not assigned to this radiologist.
        /// </summary>
        Task<RadiologistRequestResponseDto> GetAssignedRequestByIdAsync(Guid requestId, Guid radiologistId);

        /// <summary>
        /// Updates the status of an assigned request (e.g., InProgress).
        /// Throws <see cref="KeyNotFoundException"/> if not found.
        /// Throws <see cref="UnauthorizedAccessException"/> if not assigned to this radiologist.
        /// </summary>
        Task<RadiologistRequestResponseDto> UpdateRequestStatusAsync(
            Guid requestId, Guid radiologistId, Domain.ReportingRequestStatusEnum newStatus);
    }
}