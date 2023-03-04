using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace csutl.ini.impl;

class FileImpl : IniFile, IModifiable
{
    List<Section> sections = new List<Section>();
    public Format Format { get; } = new FormatImpl();

    public IReadOnlyList<ini.Section> Sections => this.sections;
    public Dictionary<string, ini.Section> map = new Dictionary<string, ini.Section>();

    public bool Modified { get; set; }
    public bool Initializing { get; private set; }

    internal FileImpl()
    {
    }

    internal FileImpl(string file)
    {
        this.LoadFile(file);
    }

    void Clear()
    {
        this.map.Clear();
        this.sections.Clear();
        var sec = new Section(string.Empty, this);
        this.sections.Add(sec);
        this.map[sec.Name] = sec;
        this.Modified = false;
    }

    public void LoadFile(string file)
    {
        try
        {
            if (System.IO.File.Exists(file))
                this.LoadIniBody(System.IO.File.ReadAllText(file));
        }
        catch (Exception e)
        {
            throw new Exception(file + ": " + e.Message);
        }
        finally
        {
            this.Modified = false;
        }
    }

    public void LoadIniBody(string body)
    {
        try
        {
            this.Initializing = true;
            this.Clear();
            var p = new Parser(this.Format);
            p.ParseIniBody(body,
                (section, comment) =>
                {
                    Section sec;
                    if (section == "" && this.sections.Count() == 1)
                        sec = this.sections.First();
                    else
                    {
                        sec = new Section(section, this);
                        sec.Comment = comment;
                        this.sections.Add(sec);
                        if (sec.IsName)
                            this.map[sec.Name] = sec;
                    }
                },
                (key, value, comment) =>
                {
                    this.sections[this.sections.Count - 1].AddItem(key, value, comment);
                });

            if (this.Format.BlankLineAfterSection)
                foreach (Section sec in this.sections)
                    sec.TrimEnd();
        }
        finally
        {
            this.Initializing = false;
            Debug.Assert(!this.Modified);
        }
    }

    public void Save(string file)
    {
        try
        {
            var lines = new List<string>();
            System.IO.File.WriteAllLines(file, this.GetText());
            this.Modified = false;
        }
        catch (Exception e)
        {
            throw new Exception(file + ": " + e.Message);
        }
    }

    public IEnumerable<string> GetText()
    {
        foreach (Section sec in this.sections)
        {
            string last = null;
            foreach (var line in sec.GetText(false))
            {
                last = line;
                yield return line;
            }

            if (this.Format.BlankLineAfterSection && !string.IsNullOrEmpty(last))
                yield return string.Empty;
        }
    }

    public ini.Section FindSection(string name)
    {
        this.map.TryGetValue(name, out var section);
        return section;
    }

    public ini.Section GetSection(string name, string defaultComment = "")
    {
        ini.Section sec = this.FindSection(name);
        if (sec == null)
        {
            sec = new Section(name, this);
            sec.Comment = defaultComment;
            this.sections.Add((Section)sec);
            this.map[sec.Name] = sec;
            this.Modified = true;
        }
        return sec;
    }

    public ini.Section this[string name, string defaultComment = ""] => this.GetSection(name, defaultComment);
}
