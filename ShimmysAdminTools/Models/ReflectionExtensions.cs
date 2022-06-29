using System;
using System.Linq;
using System.Reflection;

namespace ShimmysAdminTools.Models
{
    public static class ReflectionExtensions
    {
        private static BindingFlags All => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static T DynGet<T>(this Type t, string name)
        {
            var field = t.GetField(name, All);
            if (field == null)
            {
                throw new Exception($"Failed to get field {name}");
            }
            var val = field.GetValue(null);
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


        public static void DynSet<T, R>(this R inst, string name, T value) where R : class
        {
            var field = typeof(R).GetField(name, All);
            if (field == null)
            {
                throw new Exception($"Failed to get field {name}");
            }
            field.SetValue(inst, value);
        }

        public static bool TryDynSet<T, R>(this R inst, string name, T value) where R : class
        {
            var field = inst.GetType().GetField(name, All);
            if (field == null)
            {
                return false;
            }
            //if (!field.FieldType.IsAssignableFrom(typeof(T)))
            //{
            //    return false;
            //}

            field.SetValue(inst, value);
            return true;
        }
        public static void DynSet<R>(this R inst, string name, object value) where R : class
        {
            var field = typeof(R).GetField(name, All);
            if (field == null)
            {
                throw new Exception($"Failed to get field {name}");
            }
            field.SetValue(inst, value);
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

        public static bool TryDynInvoke<R>(this R inst, string name, params object[] parameters) where R : class
        {
            var meth = inst.GetType().GetMethod(name, All);
            if (meth == null)
            {
                return false;
            }
            //var prms = meth.GetParameters();
            //if (prms.Length != parameters.Length)
            //    return false;
            //for (int i = 0; i < prms.Length; i++)
            //{
            //    if (parameters[i] != null && !prms[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
            //    {
            //        return false;
            //    }
            //}

            meth.Invoke(inst, parameters);
            return true;
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