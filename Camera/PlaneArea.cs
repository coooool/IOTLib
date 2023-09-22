using System;
using UnityEngine;

[Serializable]
public struct PlaneArea
{
	//public Transform center;

	public float width;

	public float length;

	public PlaneArea(/*Transform center, */float width, float length)
	{
		//this.center = center;
		this.width = width;
		this.length = length;
	}
}
