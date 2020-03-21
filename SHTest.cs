using System.Collections;
using System.Collections.Generic;
using mset;
using UnityEngine;
[ExecuteInEditMode]
public class SHTest : MonoBehaviour {

	
	// Use this for initialization
	void OnEnable () {
		SHEncoding shEncoding=new SHEncoding();
		
		shEncoding.clearToBlack();
		Vector4[] shdata=new Vector4[7];
		var sh = LightmapSettings.lightProbes.bakedProbes[3];
		//  GetShaderConstantsFromNormalizedSH(sh, out shdata);
//		for (int i = 0; i < 7; i++)
//		{
//			shdata[i] = new Vector4(LightProbeMake.getFloatFromSH(sh, i*4),LightProbeMake.getFloatFromSH(sh, i*4+1),
//				LightProbeMake.getFloatFromSH(sh, i*4+2),LightProbeMake.getFloatFromSH(sh, i*4+3));
//		}
  LightProbeMake.GetShaderConstantsFromNormalizedSH(sh, out shdata);
 for (int i = 0; i < 7; i++)
 {
	 shdata[i] = shdata[i] * 1.1f+Vector4.one*0.1f;
//  
	 print(shdata[i]);
 }
//	
//		{
//			shEncoding.c[i]=LightProbeMake.getFloatFromSH(sh, i);
//		}
//		shEncoding.copyToBuffer();
//		
//		for(uint i = 0; i < 9; ++i) {
//			Shader.SetGlobalVector("_SH" + i, shEncoding.cBuffer[i]);
//		}
		for (int i = 0; i < 	LightmapSettings.lightProbes.positions.Length	; i++)
		{
			print(i+":"+	LightmapSettings.lightProbes.positions[i]	);
		}
		 Shader.SetGlobalVectorArray("TestSHData",shdata);
	}
	
	 
}
