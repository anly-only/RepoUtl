using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoUtl
{
    class RepoComboItem
    {
        internal string RepoPath { get; set; }
        internal string Postfix { get; set; }

        internal RepoComboItem(string s)
        {
            var ar = s.Split('|');
            RepoPath = ar[0];
            Postfix = string.Join("|", ar.Skip(1));
            if (Postfix.IsEmpty())
                Postfix = "modified";
        }

        internal string SaveToString() => $"{this.RepoPath}|{this.Postfix}";

        public override string ToString() => RepoPath;
    }
}
