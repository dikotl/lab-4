namespace Lab4;

public static class TaskHelper
{
    public static void BubbleSort<T>(T[] items, Comparison<T> cmp)
    {
        for (var i = 0; i < items.Length; ++i)
            for (var j = i + 1; j < items.Length; ++j)
                if (cmp(items[i], items[j]) > 0)
                    (items[j], items[i]) = (items[i], items[j]);
    }

    public static void BubbleSort<T>(T[] items) where T : IComparable<T>
    {
        for (var i = 0; i < items.Length; ++i)
            for (var j = i + 1; j < items.Length; ++j)
                if (items[i].CompareTo(items[j]) > 0)
                    (items[j], items[i]) = (items[i], items[j]);
    }

    public static T[] ResizeArray<T>(T[] a, int size)
    {
        var copy = new T[size];
        var until = int.Min(a.Length, size);
        for (var i = 0; i < until; ++i) copy[i] = a[i];
        return copy;
    }

    public static T[] Union<T>(T[] a, T[] b) where T : IComparable<T>
    {
        var union = new T[a.Length + b.Length];
        var i = 0;
        var j = 0;
        var k = 0;

        while (i < a.Length && j < b.Length)
        {
            var cmp = a[i].CompareTo(b[j]);

            if (cmp < 0)
            {
                // Copy element of `a` only
                union[k++] = a[i++];
            }
            else if (cmp > 0)
            {
                // Copy element of `b` only
                union[k++] = b[j++];
            }
            else
            {
                // Copy element and skip both
                union[k++] = a[i];
                ++i;
                ++j;
            }
        }

        // Copy the rest
        while (i < a.Length) union[k++] = a[i++];
        while (j < b.Length) union[k++] = b[j++];

        return ResizeArray(union, k);
    }

    public static T[] Intersection<T>(T[] a, T[] b) where T : IComparable<T>
    {
        var intersection = new T[int.Min(a.Length, b.Length)];
        var i = 0;
        var j = 0;
        var k = 0;

        while (i < a.Length && j < b.Length)
        {
            var cmp = a[i].CompareTo(b[j]);

            // Skip until first equal element
            if (cmp < 0)
            {
                ++i;
            }
            else if (cmp > 0)
            {
                ++j;
            }
            else
            {
                // Elements are equals, copy it
                intersection[k++] = a[i];
                i++;
                j++;
            }
        }

        return ResizeArray(intersection, k);
    }

    public static T[] Difference<T>(T[] a, T[] b) where T : IComparable<T>
    {
        var difference = new T[a.Length];
        var i = 0;
        var j = 0;
        var k = 0;

        while (i < a.Length && j < b.Length)
        {
            var cmp = a[i].CompareTo(b[j]);

            if (cmp < 0)
            {
                difference[k++] = a[i++];
            }
            else if (cmp > 0)
            {
                // Maybe next element?
                j++;
            }
            else
            {
                // Maybe next element?
                i++;
                j++;
            }
        }

        // Copy the rest
        while (i < a.Length) difference[k++] = a[i++];

        return ResizeArray(difference, k);
    }

    public static int ValidateTask6Action(string input) => int.Parse(input) switch
    {
        var action when action is (1 or 2 or 3) => action,
        var action => throw new FormatException($"Unknown action number: {action}")
    };
}
