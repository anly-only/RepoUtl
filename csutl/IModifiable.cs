using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csutl
{
    public interface IModifiable
    {
        bool Initializing { get; }
        bool Modified { get; set; }
    }

    public static class IModifiableEx
    {
        public static void Modify(this IModifiable m, ref string member, string value)
        {
            if (m.Initializing)
            {
                member = value;
            }
            else if (member != value)
            {
                member = value;
                m.Modified = true;
            }
        }
    }
}
