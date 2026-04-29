using Domain;
using Domain.People;
using RadiologistAppCore.DTOs;
using RadiologistAppCore.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace RadiologistAppCore.Services
{
    public class WorkloadManagementService : IWorkloadManagementService
    {
        private readonly IRepository<ReportingRequest> _requestRepository;
        private readonly IRepository<Radiologist> _radiologistRepository;
        private readonly ILogger<WorkloadManagementService> _logger;

        public WorkloadManagementService(
            IRepository<ReportingRequest> requestRepository,
            IRepository<Radiologist> radiologistRepository,
            ILogger<WorkloadManagementService> logger)
        {
            _requestRepository = requestRepository;
            _radiologistRepository = radiologistRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<RadiologistRequestResponseDto>> GetAssignedRequestsAsync(
            Guid radiologistId)
        {
            _logger.LogInformation("Fetching assigned requests for radiologist: {RadiologistId}", radiologistId);

            var radiologist = await _radiologistRepository.GetByIdAsync(radiologistId);
            if (radiologist is null)
                throw new UnauthorizedAccessException("Radiologist not found.");

            var requests = await _requestRepository.FindNestedSearchAsync(
                filter: r => r.AssignedRadiologist.Id == radiologistId,
                maxLevel: 2
            );

            return requests.Select(MapToResponseDto);
        }

        public async Task<RadiologistRequestResponseDto> GetAssignedRequestByIdAsync(
            Guid requestId, Guid radiologistId)
        {
            _logger.LogInformation("Fetching request {RequestId} for radiologist {RadiologistId}",
                requestId, radiologistId);

            var request = await _requestRepository.GetByIdNestedSearchAsync(requestId, maxLevel: 2);

            if (request is null)
                throw new KeyNotFoundException($"Reporting request with Id '{requestId}' was not found.");

            if (request.AssignedRadiologist?.Id != radiologistId)
                throw new UnauthorizedAccessException("You are not assigned to this reporting request.");

            return MapToResponseDto(request);
        }

        public async Task<RadiologistRequestResponseDto> UpdateRequestStatusAsync(
            Guid requestId, Guid radiologistId, ReportingRequestStatusEnum newStatus)
        {
            _logger.LogInformation(
                "Updating status of request {RequestId} to {Status} by radiologist {RadiologistId}",
                requestId, newStatus, radiologistId);

            var request = await _requestRepository.GetByIdNestedSearchAsync(requestId, maxLevel: 2);

            if (request is null)
                throw new KeyNotFoundException($"Reporting request with Id '{requestId}' was not found.");

            if (request.AssignedRadiologist?.Id != radiologistId)
                throw new UnauthorizedAccessException("You are not assigned to this reporting request.");

            request.Status = newStatus;

            await _requestRepository.UpdateAsync(request);
            await _requestRepository.CommitAsync(Guid.Empty);

            _logger.LogInformation("Request {RequestId} status updated to {Status}", requestId, newStatus);

            return MapToResponseDto(request);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Mapper
        // ─────────────────────────────────────────────────────────────────────

        private static RadiologistRequestResponseDto MapToResponseDto(ReportingRequest request)
        {
            return new RadiologistRequestResponseDto
            {
                Id = request.Id,

                PatientId = request.Image?.Patient?.Id ?? Guid.Empty,
                PatientName = request.Image?.Patient?.Name ?? string.Empty,
                PatientDateOfBirth = request.Image?.Patient?.DateOfBirth ?? default,
                PatientPhoneNumber = request.Image?.Patient?.PhoneNumber ?? string.Empty,
                PatientGender = request.Image?.Patient?.Gender ?? default,
                PatientAddress = request.Image?.Patient?.Address ?? string.Empty,
                PatientMedicalHistory = request.Image?.Patient?.MedicalHistory,
                PatientNotes = request.Image?.Patient?.Notes,

                MedicalImageId = request.Image?.Id ?? Guid.Empty,
                ImageModality = request.Image?.ImageModality ?? default,
                StorageReference = request.Image?.StorageReference ?? string.Empty,

                RequestedByName = request.RequestedBy?.Name ?? string.Empty,
                SuggestedDepartment = request.SuggestedDepartment ?? string.Empty,
                Priority = request.Priority,
                IsEmergency = request.IsEmergency,
                EmergencyJustification = request.EmergencyJustification,

                ReportId = request.ReportId,

                Status = request.Status,
                SubmissionTime = request.SubmissionTime,
                DueDate = request.DueDate
            };
        }
    }
}