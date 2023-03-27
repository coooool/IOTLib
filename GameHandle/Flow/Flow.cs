using System.Collections;
using System.Collections.Generic;
using IOTLib;
using UnityEngine;

public class Flow : IFlow
{
    private VariableDeclarations m_Vars = null;
    //private Stack<IFlowStateTransition> m_TransitionStack = null;

    public VariableDeclarations Vars
    {
        get 
        {
            if(m_Vars == null) m_Vars = new VariableDeclarations();
            return m_Vars; 
        }
        set { m_Vars = value; }
    }

    //public Stack<IFlowStateTransition> TransitionStack
    //{
    //    get
    //    {
    //        if (m_TransitionStack == null) m_TransitionStack = new();
    //        return m_TransitionStack;
    //    }
    //}

    //public void PushTransitionHistory(IFlowStateTransition transition)
    //{
    //    TransitionStack.Push(transition);
    //}

    //public Stack<IFlowStateTransition> GetTransitionHistory()
    //{
    //    return TransitionStack;
    //}

    public void Reset()
    {
        if(m_Vars != null)
        {
            m_Vars.Clear();
        }

        //if(m_TransitionStack != null)
        //{
        //    m_TransitionStack.Clear();
        //}
    }
}
