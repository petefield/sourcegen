using System.Runtime.CompilerServices;

namespace pfie.utilities;

public static class ThrowIfNullExtension
{
    public static T ThrowIfNull<T>(this T? source, [CallerArgumentExpression(nameof(source))] string? argumentName = null)
    {
        if (source is null)
            throw new ArgumentNullException(argumentName ?? string.Empty);

        return source;
    }

}
