using System.Collections;
using System.Collections.Generic;
using mset;
using UnityEngine;
using UnityEngine.Rendering;
[ExecuteInEditMode]
public class LightProbeMake : MonoBehaviour
{
    public Transform testProbeItem;
    // Start is called before the first frame update
    public Texture3D volumeGI;
 

    public bool useVolumeGI;
    public int cellX=10;
    public int cellY=10;
    public int cellZ=10;
    public float cellSize = 1;

    void Start()
    {
      //  Update2();

    }

    private void Update()
    {
        Shader.SetGlobalFloat("_VolumeGIUsed",useVolumeGI?1:0);
        Shader.SetGlobalVector("_VolumeGIBoxSize",new Vector4(cellX,cellY,cellZ,0)*cellSize);
   
    }

    [ContextMenu("clearLightmap")]
    void clearLightmap()
    {
      //  LightmapSettings.lightmaps=new LightmapData[0];
        LightmapSettings.lightProbes=new LightProbes();
 
    }

    float numin(Color c)
    {
		
        return Vector3.Dot(new Vector3(0.22f, 0.707f, 0.071f), new Vector3(c.r,c.g,c.b));
    }

    public static float GetCoefficient(SphericalHarmonicsL2 probe, int colorChannel, int index)
    {
        return probe[colorChannel,index];
    } // getFloatFromSH(probe,colorChannel*3 + index); }
  public   static  void GetShaderConstantsFromNormalizedSH(  SphericalHarmonicsL2  probe,out Vector4 [] outCoefficients)
    {
        outCoefficients=new Vector4[7];
        // Constant + Linear
        int iC = 0;
        for (iC =0; iC < 3; iC++)
        {
            // In the shader we multiply the normal is not swizzled, so it's normal.xyz.
            // Swizzle the coefficients to be in { x, y, z, DC } order.
            outCoefficients[iC].x =  GetCoefficient(probe,iC, 3);
            outCoefficients[iC].y =  GetCoefficient(probe,iC, 1);
            outCoefficients[iC].z =  GetCoefficient(probe,iC, 2);
            outCoefficients[iC].w =  GetCoefficient(probe,iC, 0) - GetCoefficient(probe,iC, 6);
        }

        // Quadratic polynomials
        for (iC = 0; iC < 3; iC++)
        {
            outCoefficients[iC + 3].x = GetCoefficient(probe,iC, 4);
            outCoefficients[iC + 3].y = GetCoefficient(probe,iC, 5);
            outCoefficients[iC + 3].z = GetCoefficient(probe,iC, 6) * 3.0f;
            outCoefficients[iC + 3].w = GetCoefficient(probe,iC, 7);
        }

        // Final quadratic polynomial
        outCoefficients[6].x =  GetCoefficient(probe,0, 8);
        outCoefficients[6].y =  GetCoefficient(probe,1, 8);
        outCoefficients[6].z =  GetCoefficient(probe,2, 8);
        outCoefficients[6].w = 1.0f;
    }
  public static float getFloatFromSH(SphericalHarmonicsL2 sh,int index)
  {
     // index = new int[] {0,1,2,4,5,6,8,9,10,12,13,14,16,17,18,20,21,22,24,25,26,3,7,11,15,19,23,27 }[index];
       if (index >= 27) return 0;
     //   rgb * 9 + coefficient
     int rgb = index / 9;
     int coefficient = index % 9;
     print("fetch:"+(rgb * 9 + coefficient));
     return sh[rgb, coefficient];
    }

    // Update is called once per frame
    [ContextMenu("makeText3d")]
    void makeText3d()
    {
        //这里用9张图 可以压缩到7张 7个float4 够存入27
  
        Dictionary<int,int> posIndexs=new Dictionary<int,int>();
        for (int i = 0; i <  LightmapSettings.lightProbes.positions.Length; i++)
        {
            var pos = LightmapSettings.lightProbes.positions[i]/cellSize;
             
           int index= ((int) pos.y) * cellX*cellZ + ((int) pos.z) * cellX + (int) pos.x;
           posIndexs[index] = i;
        }
        
        print(LightmapSettings.lightProbes.positions.Length);
      //  print(LightmapSettings.lightProbes.coefficients[0]);
         volumeGI=new Texture3D(cellX,cellZ,cellY*7,TextureFormat.RGBAFloat,false);
         volumeGI.filterMode = FilterMode.Trilinear;
         Color[] data=new Color[cellX*cellZ*cellY*7];
         int shWriteOffset = cellX * cellZ * cellY;
         for (int y = 0; y < cellY; y++)
         {
             for (int z = 0; z < cellZ; z++)
             {
         for (int x = 0; x < cellX; x++)
        {
           
                
                     int index= y * cellX*cellZ + z * cellX + x;
                     if(posIndexs.ContainsKey(index)==false) print(index +" not found");
                     int shIndex = posIndexs[index];
                 var sh=   LightmapSettings.lightProbes.bakedProbes[shIndex];
                 Vector4[] shdata=new Vector4[7];

                 LightProbeMake.GetShaderConstantsFromNormalizedSH(sh, out shdata);

                 for (int i = 0; i < 7; i++)
                 {
                     //shdata[i] = shdata[i] * 1.1f+Vector4.one*0.1f;
                     
//  
                     print(shdata[i]);
                 }
             
                
              
              // 如果贴图格式不支持负数 需要转到0-1
                 for (int i = 0; i < 7; i++)
                 {
                     data[index + shWriteOffset * (i)] = new Color(shdata[i].x, shdata[i].y, shdata[i].z, shdata[i].w);
                          
                 }
                   
                  
                }
                
            }
        }
        volumeGI.SetPixels(data);
        volumeGI.Apply();
        Shader.SetGlobalTexture("_VolumeGITex",volumeGI);
    }

    [ContextMenu("makeLP")]
    void makeLP()
    {
        var lpg=GetComponent<LightProbeGroup>();
        var list=new List<Vector3>();
        for (int x = 0; x < cellX; x++)
        {
            for (int y = 0; y < cellY; y++)
            {
                for (int z = 0; z < cellZ; z++)
                {
                    list.Add(new Vector3(x+0.5f,y+0.5f,z+0.5f) *cellSize);
                    
                }
                
            }
        }

        lpg.probePositions = list.ToArray();
    }
 
}
