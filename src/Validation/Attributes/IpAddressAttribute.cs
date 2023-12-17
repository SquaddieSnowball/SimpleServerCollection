using System.ComponentModel.DataAnnotations;
using System.Net;
using Validation.Resources.Attributes.ErrorMessages;

namespace Validation.Attributes;

/// <summary>
/// Validates an IP address.
/// </summary>
public sealed class IpAddressAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IpAddressAttribute"/> class.
    /// </summary>
    public IpAddressAttribute() => SetUpValidation();

    /// <summary>
    /// Checks that the value of the data field is an IP address.
    /// </summary>
    /// <param name="value">The data field value to validate.</param>
    /// <returns><see langword="true"/> if the specified value is an IP address; otherwise, <see langword="false"/>.</returns>
    public override bool IsValid(object? value)
    {
        string? stringValue = value?.ToString();

        return (IPAddress.TryParse(stringValue, out IPAddress? ipAddress) is true) &&
            (ipAddress.ToString().Equals(stringValue, StringComparison.Ordinal) is true);
    }

    private void SetUpValidation()
    {
        ErrorMessage = AttributesErrorMessages.IpAddressAttribute;
    }
}