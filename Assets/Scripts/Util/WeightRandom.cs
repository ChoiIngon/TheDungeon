using System.Collections.Generic;
using UnityEngine;

namespace Util
{
	public class WeightRandom<T>
	{
		const int MAX_WEIGHT = 9999999;
		private int total_weight = 0;
		private List<Tuple<int, T>> entry = new List<Tuple<int, T>>();

		public void SetWeight(T value, int weight)
		{
			if(0 == weight)
			{
				return;
			}

			total_weight += weight;
			if (MAX_WEIGHT < total_weight)
			{
				throw new System.OverflowException("total weight is over the limit");
			}

			entry.Add(new Tuple<int, T>(total_weight, value));
		}

		public T Random()
		{
			if (0 == entry.Count)
			{
				throw new System.SystemException("no entry in random");
			}

			int r = UnityEngine.Random.Range(0, total_weight);
			foreach (var itr in entry)
			{
				if(r <= itr.first)
				{
					return itr.second;
				}
			}
			return entry[entry.Count - 1].second;
		}
	}
}
