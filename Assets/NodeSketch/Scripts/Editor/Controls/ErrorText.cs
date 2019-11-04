using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ErrorText : Label
{
    public ErrorText() : base() { RemoveFromClassList("unity-label"); this.name = "error-text"; }

    public ErrorText(string text) : base(text)
    {
        RemoveFromClassList("unity-label");
        this.name = "error-text";
    }
}
