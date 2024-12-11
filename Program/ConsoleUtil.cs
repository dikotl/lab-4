namespace Lab4;

public static class ConsoleUtil
{
    public static T Request<T>(Converter<string, T> converter, string? message = null, bool inline = false, bool bare = false)
    {
        while (true) try
            {
                return converter(Request(message, inline, bare));
            }
            catch (FormatException e) { Error(e.Message); }
    }

    public static T Request<T>(string? message = null, bool inline = false, bool bare = false) where T : IParsable<T>
    {
        while (true) try
            {
                var input = Request(message, inline, bare);
                return T.Parse(input, null);
            }
            catch (FormatException) { Error("Invalid input, illegal format"); }
            catch (OverflowException) { Error("Invalid input, overflow"); }
    }

    public static string Request(string? message = null, bool inline = false, bool bare = false)
    {
        if (!bare)
            if (inline)
            {
                if (message != null)
                    Console.Write($"{message}: ");
                else
                    Console.Write($": ");
            }
            else
            {
                if (message != null)
                    Console.Write($"{message}\n> ");
                else
                    Console.Write($"\n> ");
            }

        return Console.ReadLine() ?? "";
    }

    public static void Error(string? message = null)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;

        if (message != null)
            Console.WriteLine($"Error! {message}");
        else
            Console.WriteLine($"Error!");

        Console.ForegroundColor = oldColor;
    }

    public static T[] ReadArrayRandom<T>(Func<T> random) where T : IParsable<T>
    {
        var array = RequestArrayLength<T>();

        for (var i = 0; i < array.Length; ++i)
            array[i] = random();

        Console.WriteLine($"Generated array: {string.Join(' ', array)}");
        return array;
    }

    public static T[] ReadArrayOneLine<T>() where T : IParsable<T>
    {
        var input = Console.ReadLine()!.Split();
        return Array.ConvertAll(input, x => T.Parse(x, null));
    }

    public static T[] ReadArrayLineByLine<T>() where T : IParsable<T>
    {
        var array = RequestArrayLength<T>();

        for (var i = 0; i < array.Length; ++i)
            array[i] = RequestArrayElem<T>(i);

        return array;
    }

    public static T[] RequestArray<T>(Func<T> random) where T : IParsable<T>
    {
        const string message = """
        Select array input method:
            1. Random
            2. One line
            3. Line by line
        """;

        while (true) try
            {
                return Request<int>(message) switch
                {
                    1 => ReadArrayRandom(random),
                    2 => ReadArrayOneLine<T>(),
                    3 => ReadArrayLineByLine<T>(),
                    var x => throw new ArgumentException($"Unknown option: {x}"),
                };
            }
            catch (ArgumentException e) { Error(e.Message); }
    }

    private static T[] RequestArrayLength<T>() where T : IParsable<T>
    {
        int n;

        while (true) try
            {
                var input = Request("Input number of elements");
                n = int.Parse(input);
                break;
            }
            catch (FormatException) { Error("Invalid input, illegal format"); }
            catch (OverflowException) { Error("Invalid input, overflow"); }

        return new T[n];
    }

    private static T RequestArrayElem<T>(int index) where T : IParsable<T>
    {
        while (true) try
            {
                var input = Request($"{index}", inline: true);
                return T.Parse(input, null);
            }
            catch { Error("Invalid input, try again"); }
    }
}
