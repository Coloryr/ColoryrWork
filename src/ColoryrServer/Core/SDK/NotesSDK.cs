﻿using System;

namespace ColoryrServer.SDK;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NotesSDK : Attribute
{
    public string Text;
    public string[] Input;
    public string[] Output;

    public NotesSDK(string Text, string[] Input = null, string[] Output = null)
    {
        this.Text = Text;
        this.Input = Input ?? new string[1];
        this.Output = Output ?? new string[1];
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DLLIN : Attribute
{
    public bool Debug;
    public DLLIN(bool debug = false)
    {
        Debug = debug;
    }
}