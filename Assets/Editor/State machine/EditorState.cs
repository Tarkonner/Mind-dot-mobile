using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorState 
{
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}
