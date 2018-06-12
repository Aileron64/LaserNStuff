using UnityEngine;
using System.Collections.Generic;

public class Statistics
{ // various statistical functions

	public class DiscreetCDF
	{ // represents a set of CDF data
		public float       runningTotal = 0.0f;
		public List<float> entries      = new List<float>(1000);
		// clear to some sensible values
		public void Clear()
			{ runningTotal = 0.0f; entries.Clear(); }
		public bool IsValid()
		{ // is this CDF data in a state so we can use it?
			return runningTotal > 0.0f &&	// we have a positive total
				   entries != null &&		// AND a list
				   entries.Count > 0;		// AND entries in the list
		}
		// get a random value in range [0,runningTotal]
		public float GetRandom()
			{ return Mathf.Abs(runningTotal) * Random.value; }
		public int   Sample(float v)
		{ // return the index which value is >= v. if not found, return -1
			if (! IsValid())
				return -1;
			 // every entry signifies: "chance goes up to this value"
			for (int i = 0; i < entries.Count; i++)
				if (entries[i] >= v)
					return i;
			return -1;		
		}
		
		public void Get(ref List<float> from)
		{ // sum up the contents and create an offset discreet CDF
			Clear();
			if (from.Count <= 0)
				return;
			for (int i = 0; i < from.Count; i++)
			{ // calculate runningTotal and add it's current to the list Value
				float v = Mathf.Abs(from[i]);
				entries.Add(v + runningTotal);
				runningTotal += v;
			}
		}

		List<int> singleSample = new List<int>(1);
		public int SampleRandom()
		{ // sample once over established CDF
			SampleRandom(1, ref singleSample);
			return singleSample[0];
		}		
		public void SampleRandom
			(int amount, ref List<int> rndKeyList)
		{ // calculate a cumulative distribution function and pick a random candidate
			amount = Mathf.Clamp(amount, 1, amount);
			rndKeyList.Clear();
			if (! IsValid())
				return;
			// now pick x random elements from the list
			for (int n = 0; n < amount; n++)
				rndKeyList.Add(Sample(GetRandom()));
		}
	}
}
