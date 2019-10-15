using System.Collections.Generic;
using System.Linq;

public struct Tuple <T1, T2> {
	public readonly T1 first;
	public readonly T2 second;

	public Tuple(T1 item1, T2 item2) {
		first = item1;
		second = item2;
	}
}

public static class ListExtra
{
	public static void Resize<T>(this List<T> list, int size, T c = default(T))
	{
		if (size < list.Count)
		{
			list.RemoveRange(size, list.Count - size);
		}
		else if (size > list.Count)
		{
			list.AddRange(Enumerable.Repeat(c, size - list.Count));
		}
	}
}