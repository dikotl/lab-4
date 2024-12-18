using System.Collections.Immutable;
using System.Text;

using static Lab4.TaskHelper;

using Arrival = (int time, int building, int exitOrder);

namespace Lab4;

public static class Tasks
{
    public static Action RequestTask()
    {
        // Show tasks list to the user.
        Console.WriteLine("""
        Available tasks:
            1. Main task (square negative numbers and find 2 max elements)
            2. Additional 1 (K-th decreasing element in the sequence)
            3. Additional 2 (guess the number)
            4. Additional 3 (indices of sorted array elements)
            5. Additional 4 (binary search)
            6. Additional 5 (union, intersection, difference)
            7. Additional 6 (shared elements in arrays)
            8. Additional 7 (students arrival time calculator)
        """);

        while (true) try
            {
                return ConsoleUtil.Request<int>("Input task number") switch
                {
                    1 => UseStd() ? Task1Advanced : Task1, // Variant 6
                    2 => Task2,
                    3 => Task3,
                    4 => Task4,
                    5 => Task5,
                    6 => UseStd() ? Task6Advanced : Task6,
                    7 => UseStd() ? Task7Advanced : Task7,
                    8 => Task8,
                    var task => throw new ArgumentException($"Unknown task: {task}"),
                };
            }
            catch (ArgumentException e) { ConsoleUtil.Error(e.Message); }
    }

    private static bool UseStd()
    {
        return ConsoleUtil.Request<int>("Use STD library? (0 for NO)") != 0;
    }

    private static Func<int> RandomIntAction()
    {
        var random = new Random();
        return () => random.Next(-100, 100);
    }

    private static Func<double> RandomDoubleAction()
    {
        var random = new Random();
        return () => random.Next(-100, 100);
    }

    /// <summary>
    /// Замінити всі від’ємні числа на їхні квадрати, після чого знайти серед
    /// усіх елементів масиву два максимальних.
    /// </summary>
    private static void Task1()
    {
        var changed = false;
        var array = ConsoleUtil.RequestArray(RandomDoubleAction());

        // Double negative elements
        for (var i = 0; i < array.Length; ++i)
            if (array[i] < 0)
            {
                array[i] = array[i] * array[i];
                changed = true;
            }

        if (changed)
            Console.WriteLine($"New array: {string.Join(' ', array)}");

        (double val, int idx)? max1 = null, max2 = null;

        for (var i = 0; i < array.Length; ++i)
        {
            if (!max1.HasValue || array[i] > max1?.val)
            {
                if (!max2.HasValue || array[i] > max2?.val)
                {
                    max2 = max1;
                }

                max1 = (array[i], i);
            }
            else if (!max2.HasValue || array[i] > max2?.val)
            {
                max2 = (array[i], i);
            }
        }

        if (max1.HasValue)
            if (max2.HasValue)
                Console.WriteLine(
                    "Max elements:" +
                    $"\n\t{max1?.val} at {max1?.idx}" +
                    $"\n\t{max2?.val} at {max2?.idx}"
                );
            else
                Console.WriteLine(
                    "Max element (array is too small):" +
                    $"\n\t{max1?.val} at {max1?.idx}"
                );
        else
            Console.WriteLine("Max element not found (array is too small)");
    }

    private static void Task1Advanced()
    {
        var array = ConsoleUtil.RequestArray(RandomDoubleAction());

        // Double negative elements
        var seq = array.Select((e, i) => (idx: i, val: (e < 0) ? (e * e) : e));

        Console.WriteLine("New array: {0}",
            seq.Aggregate(
                new StringBuilder(),
                (buf, e) => buf.Append(e.idx == 0 ? $"{e.val}" : $", {e.val}")
            )
        );

        // Pre-declare aggregate variables because C# type inference sucks.
        (int idx, double val)? max1 = null, max2 = null;

        // Dark magic
        (max1, max2) = seq.Aggregate(
            (a: max1, b: max2),
            (max, elem) =>
                (max.a == null || elem.val > max.a?.val)
                ? (max.b == null || elem.idx > max.b?.val)
                  ? (elem, max.a)
                  : (elem, max.b)
                : (max.b == null || elem.val > max.b?.val)
                  ? (max.a, elem)
                  : (max.a, max.b)
        );

        // Even darker magic
        Console.WriteLine(
            max1.HasValue
            ? max2.HasValue
              ? $"Max elements:\n\t{max1?.val} at {max1?.idx}\n\t{max2?.val} at {max2?.idx}"
              : $"Max element (array is too small):\n\t{max1?.val} at {max1?.idx}"
            : "Max element not found (array is too small)"
        );
    }

    private static void Task2()
    {
        var useSort = ConsoleUtil.Request<int>("Use sorting? (0 for NO)") != 0;

        // static int ValidateN(string input)
        // {
        //     var n = int.Parse(input);
        //     if (n < 1 || n > 20_000)
        //         throw new ArgumentException("Value out of range (1 <= N <= 20000)");
        //     return n;
        // }

        // Elements are not unique btw.
        var array = ConsoleUtil.RequestArray(RandomIntAction());
        var k = ConsoleUtil.Request<int>(message: "Input K", inline: true);
        var found = null as (int idx, int val)?;

        if (useSort)
        {
            var sorted = new int[array.Length];
            Array.Copy(array, sorted, array.Length);
            Array.Sort(sorted);

            found = (idx: Array.IndexOf(array, sorted[^k]), val: sorted[^k]);
        }
        else
        {
            // Used to skip values. Requires additional memory, but who cares.
            var skip = new bool[array.Length];

            // Find max value index, then add it to the skip table. Repeat K times.
            for (var i = 0; i < k; ++i)
            {
                found = null;

                // Find max value.
                for (int j = 1; j < array.Length; j++)
                    if (!skip[j] && (found == null || array[j] > found?.val))
                        found = (idx: j, val: array[j]);

                if (found is (var idx, _))
                    skip[idx] = true;
            }

        }

        Console.WriteLine(
            found.HasValue
            ? $"Found K-th element {found?.val} at {found?.idx + 1}"
            : "Value was not found"
        );
    }

    private static void Task3()
    {
        static (int from, int to) UserInputRange(string input) => input.Trim().Split() switch
        {
        [var from, var to] => (int.Parse(from) - 1, int.Parse(to) + 1),
            var x => throw new FormatException($"Invalid input: '{x}'"),
        };

        static int UserInput(string input) => input.Trim() switch
        {
            "+" => 1,
            "-" => -1,
            "=" => 0,
            var x => throw new FormatException($"Invalid input: '{x}'"),
        };

        const int MaxAttempts = 50;

        var range = ConsoleUtil.Request(UserInputRange, message: "Input number range (from to)");
        var attempt = 1;
        var guess = (range.to + range.from) / 2;

        for (; attempt <= MaxAttempts; ++attempt)
        {
            Console.WriteLine($"try {guess}");

            var cmp = ConsoleUtil.Request(UserInput, message: "Is it that number?", inline: true);

            if (cmp == 0)
            {
                break;
            }

            if (cmp == 1)
            {
                var offset = (range.to - guess) / 2;
                range.from = guess;
                guess += offset;
            }
            else if (cmp == -1)
            {
                var offset = (guess - range.from) / 2;
                range.to = guess;
                guess -= offset;
            }
        }

        if (attempt == MaxAttempts)
            Console.WriteLine("fail");
        else
            Console.WriteLine($"answer {guess}");
    }

    private static void Task4()
    {
        var a = ConsoleUtil.RequestArray(RandomIntAction());
        var b = ConsoleUtil.RequestArray(RandomIntAction());

        while (b.Length > a.Length)
        {
            ConsoleUtil.Error("Length of the second array must be less or equals to the length of the first array");
            b = ConsoleUtil.RequestArray(RandomIntAction());
        }

        var indices = Enumerable.Range(0, a.Length).ToArray();
        var merged = Enumerable.Zip(indices, a).ToArray();
        var result = new StringBuilder();

        BubbleSort(merged, (a, b) => a.Second.CompareTo(b.Second));

        var sortedElements = string.Join(' ', merged.Select(e => e.Second));
        Console.WriteLine($"Sorted array: {sortedElements}");

        foreach (var y in b)
        {
            for (var i = 0; i < merged.Length; ++i)
            {
                if (merged[i].Second == y)
                {
                    result.Append($"{i + 1} ");
                    break;
                }
            }
        }

        Console.WriteLine($"Indices of the elements in second array in sorted array: {result}");
    }

    private static void Task5()
    {
        var a = ConsoleUtil.RequestArray(RandomIntAction());
        var b = ConsoleUtil.RequestArray(RandomIntAction());

        BubbleSort(a);
        Console.WriteLine($"Sorted array: {string.Join(' ', a)}");

        foreach (var x in b)
        {
            var present = false;

            for (var i = 0; i < a.Length && !present; ++i)
            {
                var j = i;

                while (j < a.Length && x == a[j]) ++j;

                if (present = j > i)
                {
                    Console.WriteLine($"{i + 1} {j}");
                }
            }

            if (!present)
            {
                Console.WriteLine("0");
            }
        }
    }

    private static void Task6()
    {
        var action = ConsoleUtil.Request<int>(ValidateTask6Action, message: """
        Select action:
            1 - Union
            2 - Intersection
            3 - Difference
        """);

        Console.WriteLine("Input first set\n> ");
        var a = ConsoleUtil.ReadArrayOneLine<int>();

        Console.WriteLine("Input first set\n> ");
        var b = ConsoleUtil.ReadArrayOneLine<int>();

        var result = action switch
        {
            1 => Union(a, b),
            2 => Intersection(a, b),
            3 => Difference(a, b),
            _ => throw new Exception("Unreachable"),
        };

        Console.WriteLine($"Result: {string.Join(' ', result)}");
    }

    private static void Task6Advanced()
    {
        var action = ConsoleUtil.Request<int>(ValidateTask6Action, message: """
        Select action:
            1 - Union
            2 - Intersection
            3 - Difference
        """);

        Console.WriteLine("Input first set\n> ");
        var a = ConsoleUtil.ReadArrayOneLine<int>();

        Console.WriteLine("Input first set\n> ");
        var b = ConsoleUtil.ReadArrayOneLine<int>();

        var result = action switch
        {
            1 => a.Union(b).ToImmutableSortedSet(),
            2 => a.Intersect(b).ToImmutableSortedSet(),
            3 => a.Except(b).ToImmutableSortedSet(),
            _ => throw new Exception("Unreachable"),
        };

        Console.WriteLine($"Result: {string.Join(' ', result)}");
    }

    private static void Task7()
    {
        var n = ConsoleUtil.Request<int>(message: "Number of arrays", inline: true);
        var a = new int[n][];

        for (var i = 0; i < a.Length; ++i)
        {
            a[i] = ConsoleUtil.RequestArray(RandomIntAction());
            BubbleSort(a[i]);
        }

        var sharedElements = a[0];

        for (var i = 1; i < a.Length; i++)
        {
            sharedElements = Intersection(sharedElements, a[i]);
        }

        Console.WriteLine($"Shared elements: {string.Join(' ', sharedElements)}");
    }

    private static void Task7Advanced()
    {
        var n = ConsoleUtil.Request<int>(message: "Number of arrays", inline: true);
        var a = new int[n][];

        for (var i = 0; i < a.Length; ++i)
        {
            a[i] = ConsoleUtil.RequestArray(RandomIntAction());
        }

        var sharedElements =
            a[0]
            .Where(elem => a.All(row => row.Contains(elem)))
            .OrderBy(x => x);

        Console.WriteLine($"Shared elements: {string.Join(' ', sharedElements)}");
    }

    private static void Task8()
    {
        static void ProcessArrivals(Arrival[] arrivals, int[] pointsInTime, int building, int timeOffset, int indexOffset)
        {
            for (int i = 0; i < pointsInTime.Length; i++)
            {
                arrivals[indexOffset + i] = (
                    time: pointsInTime[i] + timeOffset,
                    building: building,
                    exitOrder: i + 1
                );
            }
        }

        var timeOffset1 = ConsoleUtil.Request<int>(message: "Time from first building", inline: true);
        var pointsInTime1 = ConsoleUtil.RequestArray<int>(RandomIntAction());

        var timeOffset2 = ConsoleUtil.Request<int>(message: "Time from second building", inline: true);
        var pointsInTime2 = ConsoleUtil.RequestArray<int>(RandomIntAction());

        var arrivals = new Arrival[pointsInTime1.Length + pointsInTime2.Length];
        ProcessArrivals(arrivals, pointsInTime1, 1, timeOffset1, 0);
        ProcessArrivals(arrivals, pointsInTime2, 2, timeOffset2, pointsInTime1.Length);

        // Comparison is fine
        BubbleSort(arrivals);

        Console.WriteLine("Arrivals:");

        foreach (var (time, building, exitOrder) in arrivals)
        {
            Console.WriteLine($"  Arrival time: at {time} from building number {building}, {exitOrder} in order");
        }
    }
}
