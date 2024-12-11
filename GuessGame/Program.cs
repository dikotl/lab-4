using System;

namespace Task
{
    class Program
    {
        const int MaxAttempts = 50;

        public static void Main()
        {
            int from = 0;
            int to = 0;
            ParseRange(Console.ReadLine(), out from, out to);

            var attempt = 1;
            var guess = (to + from) / 2;

            for (; attempt <= MaxAttempts; ++attempt)
            {
                Console.WriteLine($"try {guess}");

                var cmp = ParseAnswer(Console.ReadLine());

                if (cmp == 0) break;

                if (cmp == 1)
                {
                    var offset = (to - guess) / 2;
                    from = guess;
                    guess += offset;
                }
                else if (cmp == -1)
                {
                    var offset = (guess - from) / 2;
                    to = guess;
                    guess -= offset;
                }
            }

            if (attempt != MaxAttempts)
                Console.WriteLine($"answer {guess}");
        }

        static int ParseAnswer(string input)
        {
            switch (input)
            {
                case "+":
                    return 1;
                case "-":
                    return -1;
                case "=":
                    return 0;
                default:
                    throw new Exception("unreachable");
            }
        }

        static void ParseRange(string input, out int from, out int to)
        {
            var range = input.Split();
            from = int.Parse(range[0]) - 1;
            to = int.Parse(range[1]) + 1;
        }
    }
}
