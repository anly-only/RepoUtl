using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace cmd_ini
{
#if false
    
    Макросы вида <\d+> например <1>, <2> и т.д. заменяются отмеченными файлами (макроса <0> не бывает).  

#endif


    public interface ICmdMacros
    {
        IEnumerable<(string key, string value)> Items { get; }

        // return null if not found
        string GetMacroText(string key);

        // if value == null => remove macro
        void SetMacro(string key, string value);
    }


    public static class ICmdMacrosEx
    {
        // add <1>, <2>, ...
        public static void SetNumberedMacros(this ICmdMacros m, IEnumerable<string> args)
        {
            int ix = 1;
            foreach (string a in args)
            {
                m.SetMacro(ix.ToString(), a);
                ix++;
            }
        }

        // remove <1>, <2>, ...
        public static void RemoveNumberedMacros(this ICmdMacros m)
        {
            List<string> remove = [];

            foreach (var a in m.Items)
                if (Regex.IsMatch(a.key, @"^\d+$"))
                    remove.Add(a.key);

            foreach (var key in remove)
                m.SetMacro(key, null);
        }
    }


    public class CmdMacros : ICmdMacros
    {
        Dictionary<string, string> macros = [];

        public CmdMacros()
        {
        }

        public IEnumerable<(string key, string value)> Items =>
            this.macros.Select(a => (a.Key, a.Value));

        public string GetMacroText(string macro)
        {
            Debug.Assert(!string.IsNullOrEmpty(macro));
            string text = null;
            if (!this.macros.TryGetValue(macro, out text))
            {
                if (char.IsLetter(macro[0]) && Enum.TryParse<Environment.SpecialFolder>(macro, out var value))
                    text = Environment.GetFolderPath(value);
                else if (Guid.TryParse(macro, out var guid))
                    text = GetGuidPath(guid);
            }
            return text;
        }

        public void SetMacro(string macro, string text)
        {
            if (string.IsNullOrEmpty(text))
                this.macros.Remove(macro);
            else
                this.macros[macro] = text;
        }

        static string GetGuidPath(Guid guid)
        {
            string ret = null;
            try
            {
                ret = SHGetKnownFolderPath(guid, 0);
            }
            catch
            {
            }
            return ret;
        }

        [DllImport("shell32",
            CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        static extern string SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guid, uint dwFlags,
            int hToken = 0);
    }
}
