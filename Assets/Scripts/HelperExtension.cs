using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

static class HelperExtension {
    static Random RNG = new Random();

    /// <summary>
    /// Converts the given elements to a string array.
    /// </summary>
    public static string[] ToStringArray<T>(this IEnumerable<T> convert) {
        return convert.OfType<object>().Select(x => x.ToString()).ToArray();
    }

    /// <summary>
    /// Joins the current elemens into a string.
    /// </summary>
    public static string Join<T>(this IEnumerable<T> join, string separator = " ") {
        return string.Join(separator, join.ToStringArray());
    }

    /// <summary>
    /// Joins the given elements into a string separated by rows and columns.
    /// </summary>
    public static string Join<T>(this IEnumerable<T> join, string separator = " ", int rowLen = 1, int colLen = 1) {
        var fullLog = new string[rowLen];

        for (var i = 0; i < fullLog.Length; i++)
            fullLog[i] = string.Format("[{0}] {1}\n", i, join.Slice(colLen * i, colLen).Join(separator));

        return fullLog.Join("");
    }

    /// <summary>
    /// Splits the given string into an string array.
    /// </summary>
    public static string[] Split(this string split, params string[] separator) {
        return split.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Shuffles the specified elements.
    /// </summary>
    public static T Shuffle<T>(this T source) where T : IList {
        if (source == null)
            throw new ArgumentNullException("toShuff");

        for (var j = source.Count; j >= 1; j--) {
            var item = RNG.Next(j);

            if (item < j - 1) {
                var temp = source[item];
                source[item] = source[j - 1];
                source[j - 1] = temp;
            }
        }

        return source;
    }

    /// <summary>
    /// Returns a random element from the specified collection.
    /// </summary>
    public static T Pick<T>(this IEnumerable<T> source) {
        var list = (source as IList<T>) ?? source.ToArray();

        if (list.Count == 0)
            throw new InvalidOperationException("Cannot pick an element from an empty set.");

        return list[RNG.Next(list.Count)];
    }

    /// <summary>
    /// Fills an array with the given value.
    /// </summary>
    public static T[] Fill<T>(this T[] source, T value) {
        return source.Fill(0, source.Length, value);
    }

    /// <summary>
    /// Fills an array with the given value from the start index up to the end index.
    /// </summary>
    public static T[] Fill<T>(this T[] source, int start, int end, T value) {
        if (start < 0 || start > end) throw new ArgumentOutOfRangeException("The start variable has invalid value.");
        if (end > source.Length || end < start) throw new ArgumentOutOfRangeException("The end variable has invalud value.");

        for (var i = start; i < end; i++) source[i] = value;

        return source;
    }

    /// <summary>
    /// Fills a 2D array with the given value.
    /// </summary>
    public static T[,] Fill<T>(this T[,] source, T value) {
        for (var i = 0; i < source.GetLength(0); i++) {
            for (var j = 0; j < source.GetLength(1); j++) {
                source[i, j] = value;
            }
        }

        return source;
    }

    /// <summary>
    /// Determines wether the specified int is even.
    /// </summary>
    public static bool IsEven(this int number) {
        return (number % 2 == 0);
    }

    /// <summary>
    /// Returns the factorial of the specified int.
    /// </summary>
    public static int Fact(this int number) {
        return (number > 1) ? number * Fact(number - 1) : 1;
    }

    /// <summary>
    /// Determines if the given int is within the given range
    /// </summary>
    public static bool IsInRange(this int number, int min, int max) {
        return (number >= min && number <= max);
    }

    /// <summary>
    /// Slices the specified element.
    /// </summary>
    public static IEnumerable<T> Slice<T>(this IEnumerable<T> slice, int skip, int count) {
        return slice.Skip(skip).Take(count);
    }

    /// <summary>
    /// Converts the given int to bool.
    /// </summary>
    public static bool ToBool(this int number) {
        return (number >= 1) ? true : false;
    }

    /// <summary>
    /// Makes the current int array to bool array.
    /// </summary>
    public static bool[] ToBoolArray(this int[] numbers) {
        return numbers.Select(x => x.ToBool()).ToArray();
    }

    /// <summary>
    /// Converts the given bool to int.
    /// </summary>
    public static int ToInt(this bool boolean) {
        return (boolean) ? 1 : 0;
    }

    /// <summary>
    /// Makes the current bool array to int array.
    /// </summary>
    public static int[] ToIntArray(this bool[] booleans) {
        return booleans.Select(x => x.ToInt()).ToArray();
    }

    /// <summary>
    /// Converts the given 2D array to 1D array.
    /// </summary>
    public static T[] ToArray1D<T>(this T[,] array2D) {
        var rowLen = array2D.GetLength(0);
        var colLen = array2D.GetLength(1);
        T[] setArrayOD = new T[rowLen * colLen];

        for (var i = 0; i < rowLen; i++) {
            for (var j = 0; j < colLen; j++) {
                setArrayOD[(colLen * i) + j] = array2D[i, j];
            }
        }

        return setArrayOD;
    }

    /// <summary>
    /// Converts the given 1D array to 2D array.
    /// </summary>
    public static T[,] ToArray2D<T>(this T[] array1D, int rowLen, int colLen) {
        T[,] setArrayTD = new T[rowLen, colLen];

        for (var i = 0; i < (rowLen * colLen); i++)
            setArrayTD[i / colLen, i % colLen] = (i < array1D.Length) ? array1D[i] : default(T);

        return setArrayTD;
    }

    /// <summary>
    /// Returns the given int to coord based on column's length.
    /// </summary>
    public static string ToCoord(this int position, int colLength) {
        var getRow = position / colLength;
        var getCol = position % colLength;

        return string.Format("{0}{1}", (char)(getCol + 'A'), getRow + 1);
    }

    /// <summary>
    /// Returns the given array into an object array.
    /// </summary>
    public static object[] ToObjectArray<T>(this IEnumerable<T> source) {
        return source.Select(x => (object)x).ToArray();
    }

    /// <summary>
    /// Gets a row of the given 2D array.
    /// </summary>
    public static T[] GetRow<T>(this T[,] array, int rowNumber) {
        return Enumerable.Range(0, array.GetLength(1)).Select(x => array[rowNumber, x]).ToArray();
    }

    /// <summary>
    /// Gets a column of the given 2D array.
    /// </summary>
    public static T[] GetCol<T>(this T[,] array, int columnNumber) {
        return Enumerable.Range(0, array.GetLength(0)).Select(x => array[x, columnNumber]).ToArray();
    }

    /// <summary>
    /// Reverses the given string.
    /// </summary>
    public static string Reverse(this string reverse) {
        var strArray = reverse.ToStringArray();
        Array.Reverse(strArray);
        return strArray.Join("");
    }

    /// <summary>
    /// Clamps the given value within the range of the given parameters.
    /// </summary>
    public static int Clamp(this int value, int min, int max) {
        return Math.Min(max, Math.Max(min, value));
    }
}