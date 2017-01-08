using UnityEngine;
using System.Collections;

public class RandomZigzagPath {
	// Use this for initialization
	public Vector3 start;
	public Vector3 end {
		set {
			_end = value;
			if (null == vertices) {
				return;
			}
			vertices [vertices.Length - 1] = _end;
		}
		get {
			return _end;
		}
	}

	private Vector3 _end;
	public Vector3[] vertices;

	public RandomZigzagPath(Vector3 startPos, Vector3 endPos)
	{
		Init (startPos, endPos);
	}
	private void Init(Vector3 startPos, Vector3 endPos)
	{
		float distance = Vector3.Distance(startPos, endPos);
		int verticeCount = (int)distance * Random.Range(1, 4) + 3;
		vertices = new Vector3[verticeCount];
		vertices[0] = startPos;
		vertices[verticeCount - 1] = endPos;
		SetVertices(0, verticeCount - 1, startPos, endPos);
	}

	private void SetVertices(int startIndex, int endIndex, Vector3 startPos, Vector3 endPos)
	{
		
		int center = startIndex + (endIndex - startIndex) / 2; 
		if (0 == center || vertices.Length - 1 == center) 
		{
			return;
		}
		Vector3 vertex = Vector3.Lerp(startPos, endPos, Random.Range(0.4f, 0.6f));
		float displacement = Vector3.Distance(startPos, endPos) / 10;
		vertices[center] = new Vector3(vertex.x + Random.Range(-displacement, displacement), vertex.y + Random.Range(-displacement, displacement), vertex.z);

		if (0 == (endIndex - startIndex) / 2) {
			return;
		}
		SetVertices(startIndex, center , startPos, vertices[center]);
		SetVertices(center + 1, endIndex, vertices[center], endPos);
	}
}