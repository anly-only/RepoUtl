namespace csutl.ini;

public interface Format
{
    char Quote { get; set; }
    Quotes Quotes { get; set; }
    bool BlankLineAfterSection { get; set; }
    int CommentAlignMaxPos { get; set; } // 0 - disable comment align
    int ItemIndent { get; set; }
    int SubItemIndent { get; set; }
}

public enum Quotes
{
    Full, // 'aaa=bbb=ccc' = qqq 
          //MinLen, // aaa'=bbb='ccc = qqq 
          //MaxCount, // aaa'='bbb'='ccc = qqq 

}
