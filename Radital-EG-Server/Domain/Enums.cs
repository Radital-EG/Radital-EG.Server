using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public enum GenderEnum
    {
        Male,
        Female
    }

    public enum StatusEnum
    {
        Active,
        Inactive
    }

    public enum SpecialityEnum
    {
        // Add specialities as needed
    }

    public enum DepartmentsEnum
    {
        // Add departments as needed
    }

    public enum RolesEnum
    {
        // Add roles as needed
    }

    public enum ImageModalitiesEnum
    {
        XRay,
        MRI,
        CTScan,
        Ultrasound
    }

    public enum ReportStatusEnum
    {
        Draft,
        Completed,
        Reviewed
    }

    public enum ReportingRequestStatusEnum
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum PrioritiesEnum
    {
        Low,
        Medium,
        High,
        Critical
    }

}
