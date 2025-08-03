using System;
using System.Collections.Generic;

/// <summary>
/// Root container we serialize to JSON.  Version 0: just the needs list.
/// </summary>
[Serializable]
public class SavePayload
{
    public List<NeedRuntime> needs = new();   // Initialised so we never get null
}