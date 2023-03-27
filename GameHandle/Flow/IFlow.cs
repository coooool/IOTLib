using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;

/// <summary>
/// 一个基础运行流
/// </summary>
public interface IFlow
{
    VariableDeclarations Vars { get; set; }

    //Stack<IFlowStateTransition> TransitionStack { get; }
}
