using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Validation.Resources.Attributes.ErrorMessages;

namespace Validation.Attributes;

/// <summary>
/// Specifies the numeric range constraints for the value of a data field and defines exceptions for that range.
/// </summary>
public sealed class RangeExceptAttribute : RangeAttribute
{
    private IEnumerable<object> _exceptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeExceptAttribute"/> class 
    /// by using the specified minimum, maximum, and exception values.
    /// </summary>
    /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
    /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
    /// <param name="exceptions">Specifies values that are not allowed for the data field value.</param>
    public RangeExceptAttribute(int minimum, int maximum, params int[] exceptions) : base(minimum, maximum) =>
        SetUpValidation(exceptions?.Cast<object>());

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeExceptAttribute"/> class 
    /// by using the specified minimum, maximum, and exception values.
    /// </summary>
    /// <param name="minimum">Specifies the minimum value allowed for the data field value.</param>
    /// <param name="maximum">Specifies the maximum value allowed for the data field value.</param>
    /// <param name="exceptions">Specifies values that are not allowed for the data field value.</param>
    public RangeExceptAttribute(double minimum, double maximum, params double[] exceptions) : base(minimum, maximum) =>
        SetUpValidation(exceptions?.Cast<object>());

    /// <summary>
    /// Checks that the value of the data field is in the specified range and is not equal to any of the exception values.
    /// </summary>
    /// <param name="value">The data field value to validate.</param>
    /// <returns><see langword="true"/> if the specified value is in the range and 
    /// is not equal to any of the exception values; otherwise, <see langword="false"/>.</returns>
    public override bool IsValid(object? value)
    {
        foreach (object exception in _exceptions)
        {
            if (exception.Equals(value) is true)
                return false;
        }

        return base.IsValid(value);
    }

    [MemberNotNull(nameof(_exceptions))]
    private void SetUpValidation(IEnumerable<object>? exceptions)
    {
        _exceptions = exceptions ?? Enumerable.Empty<object>();

        if (_exceptions.Any() is true)
        {
            ErrorMessage = AttributesErrorMessages.RangeExceptAttribute +
                $"{string.Join(", ", _exceptions.Select(e => e.ToString()))}.";
        }
    }
}