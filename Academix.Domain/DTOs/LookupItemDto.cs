namespace Academix.Domain.DTOs;

/// <summary>
/// Data transfer object for lookup items
/// </summary>
public class LookupItemDto
{
    /// <summary>
    /// The unique identifier of the lookup item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The localized name of the lookup item
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The code/key of the lookup item (optional)
    /// </summary>
    public string? Code { get; set; }
} 