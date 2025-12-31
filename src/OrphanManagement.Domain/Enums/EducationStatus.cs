namespace OrphanManagement.Domain.Enums;

/// <summary>
/// Defines the education status of an orphan
/// </summary>
public enum EducationStatus
{
    /// <summary>
    /// Not currently enrolled in any educational program
    /// </summary>
    NotEnrolled = 1,
    
    /// <summary>
    /// Enrolled in preschool/kindergarten
    /// </summary>
    Preschool = 2,
    
    /// <summary>
    /// Enrolled in primary/elementary school
    /// </summary>
    PrimarySchool = 3,
    
    /// <summary>
    /// Enrolled in middle/intermediate school
    /// </summary>
    MiddleSchool = 4,
    
    /// <summary>
    /// Enrolled in high/secondary school
    /// </summary>
    HighSchool = 5,
    
    /// <summary>
    /// Enrolled in university or college
    /// </summary>
    University = 6,
    
    /// <summary>
    /// Enrolled in vocational or technical training
    /// </summary>
    VocationalTraining = 7,
    
    /// <summary>
    /// Completed education
    /// </summary>
    Graduated = 8
}
