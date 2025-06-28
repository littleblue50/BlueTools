using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using InteropGenerator.Helpers;
using Microsoft.CodeAnalysis;

namespace InteropGenerator.Extensions;

/// <summary>
///     Extensions methods for <see cref="AttributeData" /> type.
/// </summary>
public static class AttributeDataExtensions {
    /// <summary>
    ///     Tries to get the location of the input <see cref="AttributeData" /> instance.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData" /> instance to get the location for.</param>
    /// <returns>The resulting location for <paramref name="attributeData" />, if a syntax reference is available.</returns>
    public static Location? GetLocation(this AttributeData attributeData) {
        if (attributeData.ApplicationSyntaxReference is { } syntaxReference) {
            return syntaxReference.SyntaxTree.GetLocation(syntaxReference.Span);
        }

        return null;
    }

    /// <summary>
    ///     Tries to get a constructor argument at a given index from the input <see cref="AttributeData" /> instance.
    /// </summary>
    /// <typeparam name="T">The type of constructor argument to retrieve.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData" /> instance to get the argument from.</param>
    /// <param name="index">The index of the argument to try to retrieve.</param>
    /// <param name="result">The resulting argument, if it was found.</param>
    /// <returns>Whether or not an argument of type <typeparamref name="T" /> at position <paramref name="index" /> was found.</returns>
    public static bool TryGetConstructorArgument<T>(this AttributeData attributeData, int index, [NotNullWhen(true)] out T? result) {
        if (attributeData.ConstructorArguments.Length > index &&
            attributeData.ConstructorArguments[index].Value is T argument) {
            result = argument;

            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    ///     Tries to get a multi-value constructor argument at a given index from the input <see cref="AttributeData" /> instance.
    /// </summary>
    /// <typeparam name="T">The type of constructor argument to retrieve.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData" /> instance to get the argument from.</param>
    /// <param name="index">The index of the argument to try to retrieve.</param>
    /// <param name="result">The resulting argument, if it was found.</param>
    /// <returns>Whether or not an argument of type <typeparamref name="T" /> at position <paramref name="index" /> was found.</returns>
    public static bool TryGetMultiValueConstructorArgument<T>(this AttributeData attributeData, int index, [NotNullWhen(true)] out ImmutableArray<T>? result) {
        if (attributeData.ConstructorArguments.Length > index) {

            using ImmutableArrayBuilder<T> values = new();

            foreach (TypedConstant typedConstant in attributeData.ConstructorArguments[index].Values) {
                if (typedConstant.Value is not T value) {
                    result = default;
                    return false;
                }
                values.Add(value);
            }

            result = values.ToImmutable();
            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    ///     Tries to get a given named argument value from an <see cref="AttributeData" /> instance, if present.
    /// </summary>
    /// <typeparam name="T">The type of argument to check.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData" /> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="value">The resulting argument value, if present.</param>
    /// <returns>
    ///     Whether or not <paramref name="attributeData" /> contains an argument named <paramref name="name" /> with a
    ///     valid value.
    /// </returns>
    public static bool TryGetNamedArgument<T>(this AttributeData attributeData, string name, out T? value) {
        foreach (KeyValuePair<string, TypedConstant> properties in attributeData.NamedArguments) {
            if (properties.Key == name) {
                value = (T?)properties.Value.Value;

                return true;
            }
        }

        value = default;

        return false;
    }
}
