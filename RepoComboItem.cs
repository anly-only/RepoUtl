using System.Linq;

namespace RepoUtl
{
    class RepoComboItem
    {
        internal string RepoPath { get; set; }
        internal string Postfix { get; set; }

        internal RepoComboItem(string s)
        {
            var ar = s.Split('|');
            this.RepoPath = ar[0];
            this.Postfix = string.Join("|", ar.Skip(1));
            if (this.Postfix.IsEmpty())
                this.Postfix = "modified";
        }

        internal string SaveToString() => $"{this.RepoPath}|{this.Postfix}";

        public override string ToString() => this.RepoPath;
    }
}
