using System;
using System.IO;

public class Eula {

    /// <summary>
    /// End User License Agreement full text
    /// Note: this file is ended with ".csv" just to be managed by the version files, but it is a text file
    /// </summary>
    public static string Text { get; set; }

    public static bool ReadFile(string path) {
        Text = File.ReadAllText(path, System.Text.Encoding.UTF8);
        return true;
    }
}