using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Axle.Engine
{
    /// <summary>
    /// Static utility class
    /// </summary>
    public static class Utils
    {
        private static Random _rng = new Random();
        private static string _characters = Utils.Shuffle("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        /// <summary>
        /// Generated GUIDs of a fixed size
        /// </summary>
        /// <param name="size">The size</param>
        /// <returns>A guid</returns>
        public static string GenerateGUID(int size)
        {
            var builder = new StringBuilder(size);
            int random;
            for (int i = 0; i < size; i++)
            {
                random = (int)(_rng.NextDouble() * _characters.Length);
                builder.Append(_characters[random]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Shuffles a string
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>A new shuffled string</returns>
        public static string Shuffle(string str)
        {
            List<int> indices = new List<int>(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                indices.Add(i);
            }

            Shuffle<int>(indices);

            char[] chars = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                chars[i] = str[indices[i]];
            }

            return new string(chars);
        }

        /// <summary>
        /// Shuffles a list in-place.
        /// </summary>
        /// <param name="list">The list</param>
        /// <typeparam name="T">The data type contained in the list</typeparam>
        /// <returns>The shuffled list</returns>
        public static List<T> Shuffle<T>(List<T> list)
        {
            int random;
            T temp;
            for (int i = 0; i < list.Count; i++)
            {
                random = (int)(_rng.NextDouble() * (list.Count - i));
                temp = list[i + random];
                list[i + random] = list[i];
                list[i] = temp;
            }
            return list;
        }

        /// <summary>
        /// Runs a work function across a list of items, allowing control over
        /// execution concurrency.
        /// </summary>
        /// <param name="items">The list of items</param>
        /// <param name="maxTasks">The maximum number of tasks to be run concurrently</param>
        /// <param name="work">The work function</param>
        /// <typeparam name="T">The datatype of items</typeparam>
        /// <typeparam name="U">The datatype returned from the work function</typeparam>
        /// <returns>A list of objects produced</returns>
        public static List<U> RunTasks<T, U>(List<T> items, int maxTasks, Func<T, Task<U>> work)
        {
            var tasks = new Task<U>[maxTasks];
            var result = new List<U>(items.Count);
            int counter = 0;

            while (counter < items.Count)
            {
                int stop = Math.Min(items.Count, counter + maxTasks);
                int k = 0;

                int batchSize = stop - counter;
                if (batchSize < maxTasks)
                {
                    tasks = new Task<U>[batchSize];
                }

                for (; counter < stop; counter++)
                {
                    tasks[k++] = work(items[counter]);
                }

                Task.WaitAll(tasks);

                for (int i = 0; i < tasks.Length; i++)
                {
                    result.Add(tasks[i].Result);
                }
            }

            return result;
        }

        /// <summary>
        /// Runs a work function across a list of items, allowing control over
        /// execution concurrency.
        /// </summary>
        /// <param name="items">The list of items</param>
        /// <param name="maxTasks">The maximum number of tasks to be run concurrently</param>
        /// <param name="work">The work function</param>
        /// <typeparam name="T">The datatype of items</typeparam>
        public static void RunTasks<T>(List<T> items, int maxTasks, Func<T, Task> work)
        {
            var tasks = new Task[maxTasks];
            int counter = 0;

            while (counter < items.Count)
            {
                int stop = Math.Min(items.Count, counter + maxTasks);
                int k = 0;

                int batchSize = stop - counter;
                if (batchSize < maxTasks)
                {
                    tasks = new Task[batchSize];
                }

                for (; counter < stop; counter++)
                {
                    tasks[k++] = work(items[counter]);
                }

                Task.WaitAll(tasks);
            }
        }
    }
}