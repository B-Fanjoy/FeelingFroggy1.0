using System;

public interface IUISupportedObjective
{
    string GetUIText();

    bool IsFinished { get; }

    event Action<IUISupportedObjective> ProgressUpdated;
}
