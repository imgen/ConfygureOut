using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace ConfygureOut
{
    internal static class CommonExtensions
    {
        private static readonly DateTime Date1970Jan1Utc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static TimeZoneInfo HongKongTimeZoneInfo =>
            TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        public static long ToJavaScriptTicks(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                // TODO: Replace with the commented Exception instead.
                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
                //throw new Exception("Kind of dateTime is not specified.");            }
            }

            return (long)Math.Round(dateTime.ToUniversalTime().Subtract(Date1970Jan1Utc).TotalMilliseconds);
        }

        public static DateTime JavaScriptTicksToDateTime(this long jsTicks)
        {
            return DateTime.SpecifyKind(new DateTime(jsTicks * 10000 + 621355968000000000), DateTimeKind.Utc);
        }

        public static bool IsNullable(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public static bool IsInt(this string s)
        {
            if (string.IsNullOrEmpty(s) || !int.TryParse(s, out _))
            {
                return false;
            }
            return true;
        }

        public static string ToNullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        public static bool IsNull(this object obj) => obj == null;

        public static bool IsNotNull(this object obj) => obj != null;

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence) => sequence == null || !sequence.Any();

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrEmptyAfterTrim(this string str) => string.IsNullOrEmpty(str?.Trim());

        public static DateTime ConvertUtcToHkt(this DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, HongKongTimeZoneInfo);
        }

        public static DateTime ConvertHktToUtc(this DateTime hkt)
        {
            return TimeZoneInfo.ConvertTimeToUtc(hkt, HongKongTimeZoneInfo);
        }
        
        public static T[] Shuffle<T>(this IEnumerable<T> sequence)
        {
            var array = sequence.ToArray();
            var provider = new RNGCryptoServiceProvider();
            int n = array.Length;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = box[0] % n;
                n--;
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return array;
        }

        public static string ShortenString(this string str, int maxLength,
            string fill = "...", 
            bool skipMiddle = true, 
            int endCharsToKeep = 10)
        {
            if (str.Length <= maxLength)
            {
                return str;
            }
            fill = fill ?? string.Empty;
            if (!skipMiddle)
            {
                return str.Substring(0, maxLength - fill.Length) + fill;
            }
            endCharsToKeep = endCharsToKeep <= 0 ? 10 : endCharsToKeep;
            return str.Substring(0, maxLength - endCharsToKeep - fill.Length) + fill +
                   str.Substring(str.Length - endCharsToKeep, endCharsToKeep);
        }

        public static bool IsNull<T>(T? nullable) where T : struct
        {
            return nullable == null;
        }

        public static string GetPrincipalName(this object obj)
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }

        public static string ToCamelCase(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            var chars = str.ToLower().Skip(1).Prepend(char.ToUpper(str[0])).ToArray();
            return new string(chars);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> sequence, T newItem)
            => sequence.PrependAll(newItem);

        public static IEnumerable<T> PrependAll<T>(this IEnumerable<T> sequence, params T[] newItems)
        {
            return newItems.Concat(sequence);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, T newItem)
            => sequence.AppendAll(newItem);

        public static IEnumerable<T> AppendAll<T>(this IEnumerable<T> sequence, params T[] newItems) => sequence.Concat(newItems);

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            var items = sequence as T[] ?? sequence.ToArray();
            foreach (var item in items)
            {
                action(item);
            }

            return items;
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (var ms = new MemoryStream((int)stream.Length))
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
        {
            if (type.GetCustomAttributes<TAttribute>(true).FirstOrDefault() is TAttribute att)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttr>(this Type type, 
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, 
            Func<TAttr, bool> checker = null)
            where TAttr: Attribute
        {
            return type.GetPropertiesWithAttribute(typeof(TAttr), bindingFlags, attr => checker?.Invoke(attr as TAttr) ?? true);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute(this Type type,
            Type attrType,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance,
            Func<Attribute, bool> checker = null)
        {
            return type.GetProperties()
                            .Where(x => x.GetCustomAttributes(true)
                                   .Any(attr => attr.GetType() == attrType && (checker?.Invoke(attr as Attribute)?? true))
                                  );
        }

        public static List<T> AddItem<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        public static void Add<T>(this List<T> list, IEnumerable<T> items) => list.AddRange(items);

        public static string Prepend(this string str, string prefix)
        {
            return prefix + str;
        }

        public static string JoinString(this string separator, params object[] items) =>
            string.Join(separator, items);

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count < size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        // code adjusted to prevent horizontal overflow
        public static string GetFullPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> exp)
        {
            if (!TryFindMemberExpression(exp.Body, out var memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        // code adjusted to prevent horizontal overflow
        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (!IsConversion(exp) || !(exp is UnaryExpression)) return false;
            memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
            return memberExp != null;
        }

        private static bool IsConversion(Expression exp)
        {
            return exp.NodeType == ExpressionType.Convert ||
                   exp.NodeType == ExpressionType.ConvertChecked;
        }

        public static IEnumerable<(TSource item, int index)> WithIndices<TSource>(this IEnumerable<TSource> sequence)
        {
            return sequence.Select((item, index) => (item, index));
        }

        public static string[] GetFiles(this string path, string searchPattern, SearchOption searchOption)
        {
            var searchPatterns = searchPattern.Split('|');
            var files = searchPatterns.SelectMany(
                sp => Directory.GetFiles(path, sp, searchOption)
            ).ToList();
            files.Sort();
            return files.ToArray();
        }

        public static int? ParseInt(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            return int.TryParse(str, out var result)? result : (int?)null;
        }

        public static long? ParseLong(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            return long.TryParse(str, out var result) ? result : (long?)null;
        }

        public static float? ParseFloat(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            return float.TryParse(str, out var result) ? result : (float?)null;
        }

        public static double? ParseDouble(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            return double.TryParse(str, out var result) ? result : (double?)null;
        }

        public static decimal? ParseDecimal(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return null;
            }
            return decimal.TryParse(str, out var result) ? result : (decimal?)null;
        }

        public static string FormatString(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static TEnum[] GetEnumValues<TEnum>(this TEnum obj)
            where TEnum: struct 
        {
            return (TEnum[]) Enum.GetValues(typeof(TEnum));
        }

        public static DateTimeOffset RoundToHour(this DateTimeOffset dateTime)
        {
            var updated = dateTime.AddMinutes(30);
            return new DateTimeOffset(updated.Year, updated.Month, updated.Day,
                                 updated.Hour, 0, 0, dateTime.Offset);
        }

        public static DateTime ParseDateTime(this string format, string str)
        {
            return DateTime.ParseExact(str, format, null);
        }

        public static object GetDefaultValue(this Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}
