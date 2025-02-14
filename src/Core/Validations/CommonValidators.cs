﻿using System.Numerics;
using System.Text.RegularExpressions;

namespace Ocluse.LiquidSnow.Validations;

/// <summary>
/// Common validators useful for validating common types.
/// </summary>
public partial class CommonValidators
{
    /// <summary>
    /// The message to display when a value is required but not provided.
    /// </summary>
    public virtual string ValueRequiredMessage => "This field is required";

    /// <summary>
    /// The message to display when a value must be greater than zero but is not.
    /// </summary>
    public virtual string ValueMustBeGreaterThanZeroMessage => "This field must be greater than zero";

    /// <summary>
    /// The message to display when a value must be less than zero but is not.
    /// </summary>
    public virtual string ValueMustBeLessThanZeroMessage => "This field must be less than zero";

    /// <summary>
    /// The message to display when a value is not in a valid format.
    /// </summary>
    public virtual string FormatInvalidMessage => "This field is not in a valid format";

    /// <summary>
    /// The message to display when a value is larger than expected.
    /// </summary>
    public virtual string ValueLargerThanExpectedMessage => "This field is larger than expected";

    /// <summary>
    /// The message to display when a value is smaller than expected.
    /// </summary>
    public virtual string ValueSmallerThanExpectedMessage => "This field is smaller than expected";

    /// <summary>
    /// The message to display when a value is not equal to the expected value.
    /// </summary>
    public virtual string ValueNotEqualMessage => "This field is not equal to the expected value";

    /// <summary>
    /// Validates that the provided value is not null.
    /// </summary>
    public Task<ValidationResult> NotNull<T>(T? item)
    {
        if (item == null)
        {
            return Task.FromResult(ValidationResult.NotValid(ValueRequiredMessage));
        }
        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that the provided string is not null or empty.
    /// </summary>
    public Task<ValidationResult> StringNotEmpty(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(ValidationResult.NotValid(ValueRequiredMessage));
        }

        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that the provided string is not null or whitespace.
    /// </summary>
    public Task<ValidationResult> StringNotWhiteSpace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult(ValidationResult.NotValid(ValueRequiredMessage));
        }

        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that a number is greater than zero
    /// </summary>
    public Task<ValidationResult> PositiveNumber<T>(T value) where T : INumber<T>
    {
        if (value == null)
        {
            return Task.FromResult(ValidationResult.NotValid(ValueRequiredMessage));
        }

        if (value <= T.Zero)
        {
            return Task.FromResult(ValidationResult.NotValid(ValueMustBeGreaterThanZeroMessage));
        }

        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that a number is less than zero
    /// </summary>
    public Task<ValidationResult> NegativeNumber<T>(T value) where T : INumber<T>
    {
        if (value == null)
        {
            return Task.FromResult(ValidationResult.NotValid(ValueRequiredMessage));
        }

        if (value >= T.Zero)
        {
            return Task.FromResult(ValidationResult.NotValid(ValueMustBeLessThanZeroMessage));
        }

        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that the provided string is a valid Uri
    /// </summary>
    public Task<ValidationResult> IsUri(string? value)
    {
        if (value == null)
        {
            return Task.FromResult(ValidationResult.NotValid(ValueRequiredMessage));
        }
        if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? uriResult))
        {
            return Task.FromResult(ValidationResult.NotValid(FormatInvalidMessage));
        }
        if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
        {
            return Task.FromResult(ValidationResult.NotValid(FormatInvalidMessage));
        }
        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that the provided string matches a phone number regex.
    /// </summary>
    public Task<ValidationResult> IsPhoneNumber(string? value)
    {
        ValidationResult result;

        if (string.IsNullOrEmpty(value))
        {
            result = ValidationResult.ValidResult;
        }
        else
        {
            Regex regex = PhoneNumberRegex();
            if (regex.IsMatch(value))
            {
                result = ValidationResult.ValidResult;
            }
            else
            {
                result = ValidationResult.NotValid(FormatInvalidMessage);
            }
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Validates that the provided string matches an email regex.
    /// </summary>
    public Task<ValidationResult> IsEmail(string? value)
    {
        ValidationResult result;

        if (string.IsNullOrEmpty(value))
        {
            result = ValidationResult.ValidResult;
        }
        else
        {
            Regex regex = EmailRegex();
            if (regex.IsMatch(value))
            {
                result = ValidationResult.ValidResult;
            }
            else
            {
                result = ValidationResult.NotValid(FormatInvalidMessage);
            }
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Validates that the provided value is less than the <paramref name="test"/> value.
    /// </summary>
    public Task<ValidationResult> LessThan<T>(T value, T test) where T : IComparable<T>
    {
        if (value.CompareTo(test) < 0)
        {
            return Task.FromResult(ValidationResult.ValidResult);
        }
        return Task.FromResult(ValidationResult.NotValid(ValueLargerThanExpectedMessage));
    }

    /// <summary>
    /// Validates that the provided value is less than or equal to the <paramref name="test"/> value.
    /// </summary>
    public Task<ValidationResult> LessThanOrEqual<T>(T value, T test) where T : IComparable<T>
    {
        if (value.CompareTo(test) <= 0)
        {
            return Task.FromResult(ValidationResult.ValidResult);
        }
        return Task.FromResult(ValidationResult.NotValid(ValueLargerThanExpectedMessage));
    }

    /// <summary>
    /// Validates that the provided value is greater than the <paramref name="test"/> value.
    /// </summary>
    public Task<ValidationResult> GreaterThan<T>(T value, T test) where T : IComparable<T>
    {
        if (value.CompareTo(test) > 0)
        {
            return Task.FromResult(ValidationResult.ValidResult);
        }
        return Task.FromResult(ValidationResult.NotValid(ValueSmallerThanExpectedMessage));
    }

    /// <summary>
    /// Validates that the provided value is greater than or equal to the <paramref name="test"/> value.
    /// </summary>
    public Task<ValidationResult> GreaterThanOrEqual<T>(T value, T test) where T : IComparable<T>
    {
        if (value.CompareTo(test) >= 0)
        {
            return Task.FromResult(ValidationResult.ValidResult);
        }
        return Task.FromResult(ValidationResult.NotValid(ValueSmallerThanExpectedMessage));
    }

    /// <summary>
    /// Validates that the provided value is equal to the <paramref name="expected"/> value.
    /// </summary>
    public Task<ValidationResult> Equal<T>(T value, T expected)
    {
        if(EqualityComparer<T>.Default.Equals(value, expected))
        {
            return Task.FromResult(ValidationResult.ValidResult);
        }
        return Task.FromResult(ValidationResult.NotValid(ValueNotEqualMessage));
    }


    #region Regular Expressions

    [GeneratedRegex("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$")]
    private static partial Regex PhoneNumberRegex();

    [GeneratedRegex("[^@ \\t\\r\\n]+@[^@ \\t\\r\\n]+\\.[^@ \\t\\r\\n]+")]
    private static partial Regex EmailRegex();

    #endregion
}
