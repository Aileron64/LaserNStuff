using UnityEngine;
using System.Collections;

public class _Matrix3x4
{
	public class Row1x4
	{
		public float[] c = new float[4];
	}
		
	public _Matrix3x4()
	{ // three rows of 1x4
		for (int i = 0; i < 3; i++)
			r[i] = new Row1x4();
	}
	Row1x4[] r = new Row1x4[3];
		
	void NormalizeRow(int row, int basedOn)
	{ // treat matrix as 3x3 + result
		if (r[row].c[basedOn] == 0.0f ||	// not a proper base for normalization
			r[row].c[basedOn] == 1.0f)		// or: column already normalized
			return;
		float f = 1.0f / r[row].c[basedOn];
		for (int column = basedOn; column < 4; column++)
			r[row].c[column] *= f;
	}
	
	void SwapRow    (int srcRow, int dstRow)
	{ // swap the contents of two rows
		Row1x4 temp = r[dstRow];
		r[dstRow]   = r[srcRow];
		r[srcRow]   = temp;
	}
	
	void SubtractRow(int srcRow, int dstRow, int basedOn)
	{ // srcRow must be normalized!
		float f = r[dstRow].c[basedOn];
		for (int column = basedOn; column < 4; column++)
			r[dstRow].c[column] -= f * r[srcRow].c[column];
	}
			
	public Vector3 Gauss()
	{ // solve a 3x3 matrix via gauss
		int column = 0;
		for (int row = 0; row < 3; row++)
		{ // apply gauss onto all the rows
			// check if column is != 0. otherwise search for a row to swap
			if (r[row].c[column] == 0.0f)
				for (int swapRow = 0; swapRow < 3; swapRow++)
					if (swapRow != row &&
				        r[swapRow].c[column] != 0.0f)
					{ // found a candidate for row swapping
						SwapRow(swapRow, row);
						break;
					}
			// now normalize row and subtract from all others
			NormalizeRow(row, column);
			for (int aRow = 0; aRow < 3; aRow++)
				if (row != aRow)
					SubtractRow(row, aRow, column);
			column++;
		}
		// now return the solved matrix variables
		return new Vector3(r[0].c[3], r[1].c[3], r[2].c[3]);
	}
	
	public static void Build
		(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref _Matrix3x4 m)
	{ // build a 3x4 from vectors a,b,c,d
		m.r[0].c[0] = a.x; m.r[0].c[1] = b.x; m.r[0].c[2] = c.x; m.r[0].c[3] = d.x;
		m.r[1].c[0] = a.y; m.r[1].c[1] = b.y; m.r[1].c[2] = c.y; m.r[1].c[3] = d.y;
		m.r[2].c[0] = a.z; m.r[2].c[1] = b.z; m.r[2].c[2] = c.z; m.r[2].c[3] = d.z;
	}
	
	public override string ToString()
	{ // return the entire matrix as a string
		return string.Format(
			"{0:000.00} {1:000.00} {2:000.00} | {3:000.00}\n" +
			"{4:000.00} {5:000.00} {6:000.00} | {7:000.00}\n" +
			"{8:000.00} {9:000.00} {10:000.00} | {11:000.00}",
			r[0].c[0], r[0].c[1], r[0].c[2], r[0].c[3],
			r[1].c[0], r[1].c[1], r[1].c[2], r[1].c[3],
			r[2].c[0], r[2].c[1], r[2].c[2], r[2].c[3]
		);
	}	
}
