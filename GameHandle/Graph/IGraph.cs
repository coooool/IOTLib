using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraph 
{
    public string Name { get; set; }

    public GraphTypeEnum GraphType { get; }
}
