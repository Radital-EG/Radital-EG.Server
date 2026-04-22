using Domain;
using Domain.People;
using HospitalRequestsAppCore.DTOs;
using HospitalRequestsAppCore.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace HospitalRequestsAppCore.Services
{
    /// <summary>
    /// Implements the business logic for managing reporting requests on the Hospital side.
    /// Covers US-01 (create) and US-02 (status tracking).
    /// </summary>
    public class ReportingRequestsManagementService : IReportingRequestsManagementService
    {
        private readonly IRepository<ReportingRequest> _requestRepository;
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<MedicalImage> _imageRepository;
        private readonly ILogger<ReportingRequestsManagementService> _logger;

        public ReportingRequestsManagementService(
            IRepository<ReportingRequest> requestRepository,
            IRepository<Patient> patientRepository,
            IRepository<MedicalImage> imageRepository,
            ILogger<ReportingRequestsManagementService> logger)
        {
            _requestRepository = requestRepository;
            _patientRepository = patientRepository;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ReportingRequestResponseDto> CreateRequestAsync(CreateReportingRequestDto dto)
        {
            _logger.LogInformation("Creating new reporting request for patient: {PatientName}", dto.PatientName);

            // 1. Create the patient record
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                Name = dto.PatientName,
                DateOfBirth = dto.PatientDateOfBirth,
                PhoneNumber = dto.PatientPhoneNumber,
                Gender = dto.PatientGender,
                Address = dto.PatientAddress,
                MedicalHistory = dto.PatientMedicalHistory,
                Notes = dto.PatientNotes
            };
            await _patientRepository.InsertAsync(patient);
            await _patientRepository.CommitAsync(Guid.Empty);

            // 2. Create the medical image record
            var medicalImage = new MedicalImage
            {
                Id = Guid.NewGuid(),
                Patient = patient,
                ImageModality = dto.ImageModality,
                StorageReference = dto.StorageReference
            };
            await _imageRepository.InsertAsync(medicalImage);
            await _imageRepository.CommitAsync(Guid.Empty);

            // 3. Create the reporting request
            var reportingRequest = new ReportingRequest
            {
                Id = Guid.NewGuid(),
                Image = medicalImage,
                SuggestedDepartment = dto.SuggestedDepartment,
                Status = ReportingRequestStatusEnum.Pending,
                SubmissionTime = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                IsEmergency = dto.IsEmergency,
                EmergencyJustification = dto.EmergencyJustification
            };
            await _requestRepository.InsertAsync(reportingRequest);
            await _requestRepository.CommitAsync(Guid.Empty);

            _logger.LogInformation("Reporting request created with Id: {RequestId}", reportingRequest.Id);

            return MapToResponseDto(reportingRequest);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ReportingRequestResponseDto>> GetAllRequestsAsync()
        {
            _logger.LogInformation("Fetching all reporting requests");

            var requests = await _requestRepository.GetAllNestedSearchAsync(maxLevel: 2);

            return requests.Select(MapToResponseDto);
        }

        /// <inheritdoc/>
        public async Task<ReportingRequestResponseDto?> GetRequestByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching reporting request with Id: {RequestId}", id);

            var request = await _requestRepository.GetByIdNestedSearchAsync(id, maxLevel: 2);

            if (request == null)
            {
                _logger.LogWarning("Reporting request with Id: {RequestId} was not found", id);
                return null;
            }

            return MapToResponseDto(request);
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        private static ReportingRequestResponseDto MapToResponseDto(ReportingRequest request)
        {
            return new ReportingRequestResponseDto
            {
                Id = request.Id,
                PatientName = request.Image?.Patient?.Name ?? string.Empty,
                PatientDateOfBirth = request.Image?.Patient?.DateOfBirth ?? default,
                ImageModality = request.Image?.ImageModality ?? default,
                SuggestedDepartment = request.SuggestedDepartment ?? string.Empty,
                Priority = request.Priority,
                IsEmergency = request.IsEmergency,
                Status = request.Status,
                SubmissionTime = request.SubmissionTime,
                DueDate = request.DueDate
            };
        }
    }
}
