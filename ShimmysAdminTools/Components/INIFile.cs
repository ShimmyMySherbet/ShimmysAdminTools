using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShimmysAdminTools.Components
{
    // Basic INI File reader/write I threw together a while ago
    public class INIFile
    {
        private List<IniLine> Data = new List<IniLine>();
        private string LoadFile = "";
        public bool HasUnsavedChanges { get; protected set; }

        public INIFile(string content = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                foreach (string line in content.Split('\n'))
                {
                    if (!line.StartsWith("#") & !string.IsNullOrEmpty(line) & line.Contains("="))
                    {
                        string Key = line.Split('=')[0];
                        string Value = line.Remove(0, Key.Length + 1);
                        Data.Add(new IniLine()
                        {
                            Key = Key,
                            Value = Value,
                            IsDataEntry = true,
                            Line = ""
                        });
                    }
                    else
                    {
                        Data.Add(new IniLine()
                        {
                            IsDataEntry = false,
                            Line = line
                        });
                    }
                }
            }
        }

        public Dictionary<string, string> DataDictionary
        {
            get
            {
                var dict = new Dictionary<string, string>();
                foreach (var x in Data)
                {
                    if (x.IsDataEntry)
                    {
                        dict.Add(x.Key, x.Value);
                    }
                }

                return dict;
            }
        }

        public List<string> Keys
        {
            get
            {
                var res = new List<string>();
                Data.ForEach(x => { if (x.IsDataEntry) { res.Add(x.Key); } });
                return res;
            }
        }

        public List<string> Values
        {
            get
            {
                var res = new List<string>();
                Data.ForEach(x => { if (x.IsDataEntry) { res.Add(x.Value); } });
                return res;
            }
        }

        public object this[string Key, Type T]
        {
            get
            {
                object ent = this[Key];
                string estr = ent.ToString();
                if (T == typeof(bool)) return Convert.ToBoolean(estr);
                if (T == typeof(double)) return Convert.ToDouble(estr);
                if (T == typeof(int)) return Convert.ToInt32(estr);
                if (T == typeof(long)) return Convert.ToInt64(estr);
                if (T == typeof(string)) return estr;
                if (T == typeof(byte)) return Convert.ToByte(estr);
                if (T == typeof(char)) return Convert.ToChar(estr);
                if (T == typeof(DateTime)) return Convert.ToDateTime(estr);
                if (T == typeof(decimal)) return Convert.ToDecimal(estr);
                if (T == typeof(short)) return Convert.ToInt16(estr);
                if (T == typeof(sbyte)) return Convert.ToSByte(estr);
                if (T == typeof(float)) return Convert.ToSingle(estr);
                if (T == typeof(ushort)) return Convert.ToUInt16(estr);
                if (T == typeof(uint)) return Convert.ToUInt32(estr);
                if (T == typeof(ulong)) return Convert.ToUInt64(estr);
                return ent;
            }
        }

        public T Val<T>(string Key) => (T)this[Key, typeof(T)];

        public string this[string Key]
        {
            get
            {
                return Data.Where(x => { if (x.IsDataEntry) { return (x.Key.ToLower() ?? "") == (Key.ToLower() ?? ""); } else { return false; } }).First().Value;
            }
            set
            {
                HasUnsavedChanges = true;
                bool found = false;
                foreach (var x in Data)
                {
                    if (x.IsDataEntry)
                    {
                        if ((x.Key.ToLower() ?? "") == (Key.ToLower() ?? ""))
                        {
                            x.Value = value.ToString();
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    Data.Add(new IniLine()
                    {
                        IsDataEntry = true,
                        Key = Key,
                        Value = value.ToString(),
                        Line = ""
                    });
                }
            }
        }

        public void PatchKey(string Key, object DefaultValue)
        {
            if (!KeySet(Key))
            {
                this[Key] = DefaultValue.ToString();
                HasUnsavedChanges = true;
            }
        }

        public bool KeySet(string Key)
        {
            bool ret = false;
            foreach (var x in Data)
            {
                if (x.IsDataEntry)
                {
                    if ((x.Key.ToLower() ?? "") == (Key.ToLower() ?? ""))
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        public void Save(string File = "", bool Overwrite = true)
        {
            if (string.IsNullOrEmpty(File))
            {
                if (!string.IsNullOrEmpty(LoadFile))
                {
                    File = LoadFile;
                }
                else
                {
                    throw new Exception("Cannot save; no specified file or load file.");
                }
            }
            HasUnsavedChanges = false;
            if (Overwrite)
            {
                System.IO.File.WriteAllText(File, ToINIString());
            }
            else
            {
                System.IO.File.AppendAllLines(File, IniLines());
            }
        }

        public void Save(Stream Stream, Encoding Encoding = null)
        {
            if (Encoding == null)
            {
                Encoding = Encoding.UTF8;
            }
            HasUnsavedChanges = false;
            var Bytes = Encoding.GetBytes(ToINIString());
            Stream.Write(Bytes, 0, Bytes.Count());
        }

        public string ToINIString()
        {
            var Lines = new List<string>();
            foreach (var entry in Data)
            {
                if (entry.IsDataEntry)
                {
                    Lines.Add($"{entry.Key}={entry.Value}");
                }
                else
                {
                    Lines.Add(entry.Line);
                }
            }

            return string.Join(Environment.NewLine, Lines);
        }

        public void WriteComment(string Comment)
        {
            HasUnsavedChanges = true;
            Data.Add(new IniLine() { IsDataEntry = false, Line = "#" + Comment });
        }

        public void WriteLine(string Line = "")
        {
            HasUnsavedChanges = true;
            Data.Add(new IniLine() { IsDataEntry = false, Line = Line });
        }

        private List<string> IniLines()
        {
            var Lines = new List<string>();
            foreach (var entry in Data)
            {
                if (entry.IsDataEntry)
                {
                    Lines.Add($"{entry.Key}={entry.Value}");
                }
                else
                {
                    Lines.Add(entry.Line);
                }
            }

            return Lines;
        }

        private partial class IniLine
        {
            public bool IsDataEntry = false;
            public string Key = "";
            public string Line = "";
            public string Value = "";
        }
    }
}