using System;
using System.Linq;
using System.Reflection;

namespace ShimmysAdminTools.Models
{
    public static class ReflectionExtensions
    {
        private static BindingFlags All => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        public static T DynGet<T>(this Type t, string name)
        {
            Console.WriteLine("get field");
            var field = t.GetField(name, All);
            if (field == null)
            {
                Console.WriteLine("fail");
                throw new Exception($"Failed to get field {name}");
            }
            Console.WriteLine("got field");
            var val = field.GetValue(null);
            if (val == null)
            {
                Console.WriteLine("vnul");
            } else
            {
                var gens = val.GetType().GetGenericArguments();
                Console.WriteLine($"RealType: {val.GetType().Name.Split('`')[0]}{(gens.Length > 0 ? $"<{string.Join(", ", gens.Select(x => x.Name))}>" : "")}");
            }

            return (T)val;
        }

        public static T DynGet<T, R>(this R inst, string name) where R : class
        {
            var field = typeof(R).GetField(name, All);
            if (field == null)
            {
                throw new Exception($"Failed to get field {name}");
            }
            return (T)field.GetValue(inst);
        }

        public static T DynInvoke<T>(this Type t, string name, params object[] parameters)
        {
            var meth = t.GetMethod(name, All);
            if (meth == null)
            {
                throw new Exception($"Failed to get method {name}");
            }

            return (T)meth.Invoke(null, parameters);
        }

        public static void DynInvoke(this Type t, string name, params object[] parameters)
        {
            var meth = t.GetMethod(name, All);
            if (meth == null)
            {
                throw new Exception($"Failed to get method {name}");
            }

            meth.Invoke(null, parameters);
        }

        public static void DynInvoke<R>(this R inst, string name, params object[] parameters) where R : class
        {
            var meth = typeof(R).GetMethod(name, All);
            if (meth == null)
            {
                throw new Exception($"Failed to get method {name}");
            }

            meth.Invoke(inst, parameters);
        }

        public static T DynInvoke<T, R>(this R inst, string name, params object[] parameters) where R : class
        {
            var meth = typeof(R).GetMethod(name, All);
            if (meth == null)
            {
                throw new Exception($"Failed to get method {name}");
            }

            return (T)meth.Invoke(inst, parameters);
        }
    }
}