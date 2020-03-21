// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

using UnityEngine;
using System;
using System.IO;

namespace mset {
	[System.Serializable]
	public class SHEncoding {
		public float[] 	 c = new float[27];			//spherical harmonics weights	
		public Vector4[] cBuffer = new Vector4[9];	//shader buffer for weights
		
		public SHEncoding() {
			clearToBlack();
		}

		public void clearToBlack() {
			for(int i=0; i<27; ++i) { c[i] = 0.0f; }
			for(int i=0; i<9; ++i) { cBuffer[i] = Vector4.zero; }
		}
		
		public void copyFrom(SHEncoding src) {
			for(int i=0; i<27; ++i) {
				this.c[i] = src.c[i];
			}
			copyToBuffer();
		}
		
		public void copyToBuffer() {
			for(int i=0; i<9; ++i) {
				float eqc = sEquationConstants[i];
				cBuffer[i].x = c[i*3+0]*eqc;
				cBuffer[i].y = c[i*3+1]*eqc;
				cBuffer[i].z = c[i*3+2]*eqc;
			}
		}
		
		public static float[] sEquationConstants = {
			0.28209479f,	// l= 0 m= 0
	
			0.4886025f,		// l= 1 m=-1
			0.4886025f,		// l= 1 m= 0
			0.4886025f,		// l= 1 m= 1
	
			1.09254843f,	// l= 2 m=-2
			1.09254843f,	// l= 2 m=-1
			0.315391565f,	// l= 2 m= 0
			1.09254843f,	// l= 2 m= 1
			0.546274215f,	// l= 2 m= 2
		};
	};
}
