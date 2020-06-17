using System;

namespace SonarqueReport
{
    /// <summary>
    /// https://stackoverflow.com/questions/21611674/how-to-auto-generate-a-c-sharp-class-file-from-a-json-string/34274199#34274199
    /// https://stackoverflow.com/questions/47593385/cannot-find-paste-special-option-in-visual-studio-2017/47700637#47700637
    /// </summary>
    public class Rootobject
    {
        public int total { get; set; }
        public int p { get; set; }
        public int ps { get; set; }
        public Paging paging { get; set; }
        public int effortTotal { get; set; }
        public int debtTotal { get; set; }
        public Issue[] issues { get; set; }
        public Component[] components { get; set; }
        public Rule[] rules { get; set; }
        public User[] users { get; set; }
        public Language[] languages { get; set; }
        public Facet[] facets { get; set; }
    }

    public class Paging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
    }

    public class Issue
    {
        public string key { get; set; }
        public string rule { get; set; }
        public string severity { get; set; }
        public string component { get; set; }
        public string project { get; set; }
        public int line { get; set; }
        public string hash { get; set; }
        public Textrange textRange { get; set; }
        public Flow[] flows { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string effort { get; set; }
        public string debt { get; set; }
        public string author { get; set; }
        public object[] tags { get; set; }
        public string[] transitions { get; set; }
        public string[] actions { get; set; }
        public object[] comments { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime updateDate { get; set; }
        public string type { get; set; }
        public string organization { get; set; }
        public string assignee { get; set; }
    }

    public class Textrange
    {
        public int startLine { get; set; }
        public int endLine { get; set; }
        public int startOffset { get; set; }
        public int endOffset { get; set; }
    }

    public class Flow
    {
        public Location[] locations { get; set; }
    }

    public class Location
    {
        public string component { get; set; }
        public Textrange1 textRange { get; set; }
        public string msg { get; set; }
    }

    public class Textrange1
    {
        public int startLine { get; set; }
        public int endLine { get; set; }
        public int startOffset { get; set; }
        public int endOffset { get; set; }
    }

    public class Component
    {
        public string organization { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }
        public bool enabled { get; set; }
        public string qualifier { get; set; }
        public string name { get; set; }
        public string longName { get; set; }
        public string path { get; set; }
    }

    public class Rule
    {
        public string key { get; set; }
        public string name { get; set; }
        public string lang { get; set; }
        public string status { get; set; }
        public string langName { get; set; }
    }

    public class User
    {
        public string login { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public bool active { get; set; }
    }

    public class Language
    {
        public string key { get; set; }
        public string name { get; set; }
    }

    public class Facet
    {
        public string property { get; set; }
        public Value[] values { get; set; }
    }

    public class Value
    {
        public string val { get; set; }
        public int count { get; set; }
    }
}
