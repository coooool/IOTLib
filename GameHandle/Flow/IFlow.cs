using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;

/// <summary>
/// һ������������
/// </summary>
public interface IFlow
{
    VariableDeclarations Vars { get; set; }

    //Stack<IFlowStateTransition> TransitionStack { get; }
}
