using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightProbeMake : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture3D volumeGI;
    void Start()
    {
         
    }
    float numin(Color c)
    {
		
        return Vector3.Dot(new Vector3(0.22f, 0.707f, 0.071f), new Vector3(c.r,c.g,c.b));
    }
    // Update is called once per frame
    [ContextMenu("makeText3d")]
    void makeText3d()
    {
        Dictionary<int,int> posIndexs=new Dictionary<int,int>();
        for (int i = 0; i <  LightmapSettings.lightProbes.positions.Length; i++)
        {
            var pos = LightmapSettings.lightProbes.positions[i];
           int index= ((int) pos.z) * 100 + ((int) pos.y) * 10 + (int) pos.x;
           posIndexs[index] = i;
        }
         volumeGI=new Texture3D(10,10,10,TextureFormat.ARGB32,false);
         Color[] data=new Color[10*10*10];
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    var pos =new Vector3(x+0.5f,y+0.5f,z+0.5f);
                    int index= ((int) pos.z) * 100 + ((int) pos.y) * 10 + (int) pos.x;
                 var sh=   LightmapSettings.lightProbes.bakedProbes[  posIndexs[index] ];
                 Color [] colors=new Color[4];
                 sh.Evaluate(new Vector3[]{Vector3.forward, Vector3.left, Vector3.back, Vector3.right}, colors);
                 Color clr=new Color(numin(colors[0]),numin(colors[1]),numin(colors[2]),numin(colors[3]));
                   
                   data[z * 100 + y * 10 + x] = clr;
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
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    list.Add(new Vector3(x+0.5f,y+0.5f,z+0.5f));
                    
                }
                
            }
        }

        lpg.probePositions = list.ToArray();
    }

    [ContextMenu("switchGIEnable")]
    void switchGIEnable()
    {
        Shader.SetGlobalFloat("_VolumeGIUsed",1- Shader.GetGlobalFloat("_VolumeGIUsed"));

    }
}
