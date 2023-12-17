using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Validation.Resources.Helpers.Verify.ExceptionMessages;

namespace Validation.Helpers;

/// <summary>
/// Provides verification methods.
/// </summary>
public static class Verify
{
    /// <summary>
    /// Verifies that the value is not <see langword="null"/> and throws an exception if verification fails.
    /// </summary>
    /// <typeparam name="T">Type of value to verify.</typeparam>
    /// <param name="value">Value to verify.</param>
    /// <param name="argumentExpression">The expression passed 
    /// to the method as an "<paramref name="value"/>" parameter.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void NotNull<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? argumentExpression = default)
        where T : class
    {
        if (value is null)
            throw new ArgumentNullException(argumentExpression, VerifyExceptionMessages.NotNull);
    }

    /// <summary>
    /// Verifies that the string is not <see langword="null"/> or empty and throws an exception if verification fails.
    /// </summary>
    /// <param name="value">String to verify.</param>
    /// <param name="argumentExpression">The expression passed 
    /// to the method as an "<paramref name="value"/>" parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void NotNullOrEmpty(
        string value,
        [CallerArgumentExpression(nameof(value))] string? argumentExpression = default)
    {
        if (string.IsNullOrEmpty(value) is true)
            throw new ArgumentException(VerifyExceptionMessages.NotNullOrEmpty, argumentExpression);
    }

    /// <summary>
    /// Verifies that the value is one of the named constants defined for the enumeration, 
    /// and throws an exception if verification fails.
    /// </summary>
    /// <typeparam name="T">Enumeration type.</typeparam>
    /// <param name="value">Value to verify.</param>
    /// <param name="argumentExpression">The expression passed 
    /// to the method as an "<paramref name="value"/>" parameter.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void EnumerationValue<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? argumentExpression = default)
        where T : struct, Enum
    {
        if (Enum.IsDefined(value) is false)
            throw new ArgumentException(VerifyExceptionMessages.EnumerationValue, argumentExpression);
    }

    /// <summary>
    /// Verifies the options instance and throws an exception if verification fails.
    /// </summary>
    /// <typeparam name="T">Type of options to verify.</typeparam>
    /// <param name="options">Options instance to verify.</param>
    /// <param name="optionsValidator">Validator to verify the options instance.</param>
    /// <exception cref="OptionsValidationException"></exception>
    public static void Options<T>(
        T options,
        IValidateOptions<T> optionsValidator)
        where T : class
    {
        NotNull(options);
        NotNull(optionsValidator);

        Type optionsType = typeof(T);
        ValidateOptionsResult validateOptionsResult = optionsValidator.Validate(optionsType.Name, options);

        if (validateOptionsResult.Failed is true)
            throw new OptionsValidationException(optionsType.Name, optionsType, validateOptionsResult.Failures);
    }
}