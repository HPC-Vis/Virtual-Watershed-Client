using UnityEngine;
using System.Collections;

[System.Serializable]
public class RTPGlossBaked : ScriptableObject {
	public byte[] glossDataMIP1;
	public byte[] glossDataMIP2;
	public byte[] glossDataMIP3;
	public byte[] glossDataMIP4;
	public byte[] glossDataMIP5;
	public byte[] glossDataMIP6;
	public byte[] glossDataMIP7;
	public byte[] glossDataMIP8;
	public byte[] glossDataMIP9;
	public byte[] glossDataMIP10;
	public byte[] glossDataMIP11;
	public byte[] glossDataMIP12;
	public int size;
	public bool baked;
	public float gloss_mult; // baking został preprowadzony dla takich paramaterów shapingu
	public float gloss_shaping;
	public bool used_in_atlas;
	
	public RTPGlossBaked() {
		Init(0);
	}
	public RTPGlossBaked(int size) {
		Init(size);
	}
	
	public void Init(int size) {
		this.size=size;
		baked=false;
		used_in_atlas=false;
		glossDataMIP1=glossDataMIP2=glossDataMIP3=glossDataMIP4=glossDataMIP5=glossDataMIP6=null;
		glossDataMIP7=glossDataMIP8=glossDataMIP9=glossDataMIP10=glossDataMIP11=glossDataMIP12=null;
		if (size>1) size=size>>1; else return;
		glossDataMIP1=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP2=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP3=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP4=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP5=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP6=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP7=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP8=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP9=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP10=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP11=new byte[size*size]; if (size>1) size=size>>1; else return;
		glossDataMIP12=new byte[size*size]; if (size>1) size=size>>1; else return;
	}
	
	public byte[] GetGlossMipLevel(int mipLevel) {
		switch(mipLevel) {
			case 1: return glossDataMIP1;
			case 2: return glossDataMIP2;
			case 3: return glossDataMIP3;
			case 4: return glossDataMIP4;
			case 5: return glossDataMIP5;
			case 6: return glossDataMIP6;
			case 7: return glossDataMIP7;
			case 8: return glossDataMIP8;
			case 9: return glossDataMIP9;
			case 10: return glossDataMIP10;
			case 11: return glossDataMIP11;
			case 12: return glossDataMIP12;
			default: return null;
		}
	}
	
	public bool CheckSize(Texture2D tex) {
		if (!baked) return false;
		if (tex==null) return false;
		if (glossDataMIP1==null) return false;
		if (tex.width==size) return true;
		return false;
	}
	
	// atlas
	public static Texture2D MakeTexture(Texture2D sourceTexture, RTPGlossBaked[] tilesData) {
		Texture2D targetTexture=new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, true);
		for(int tile=0; tile<4; tile++) {
			int halfSize=sourceTexture.width>>1;
			int bx=(tile%2)*halfSize;
			int by=((tile%4)<2) ? 0 : halfSize;
			Color[] prevCols=sourceTexture.GetPixels(bx,by,halfSize,halfSize,0);
			// use gloss shaping for MIP0 (reszta już przygotowana do skopiowania)
			for(int i=0; i<prevCols.Length; i++) {
				float glossiness=Mathf.Clamp01(prevCols[i].a*tilesData[tile].gloss_mult);
				float gloss_shaped=glossiness;
				float gloss_shaped_inv=1-glossiness;
				gloss_shaped=gloss_shaped*gloss_shaped*gloss_shaped;
				gloss_shaped_inv=gloss_shaped_inv*gloss_shaped_inv*gloss_shaped_inv;
				float gls=Mathf.Lerp(gloss_shaped, 1-gloss_shaped_inv, tilesData[tile].gloss_shaping);
				gls=Mathf.Clamp01(gls);
				prevCols[i].a=gls;
			}
			targetTexture.SetPixels(bx,by,halfSize,halfSize,prevCols,0);
			for(int mip=1; mip<sourceTexture.mipmapCount-1; mip++) {
				int curSize=(targetTexture.width>>(mip+1));
				Color[] cols=new Color[curSize * curSize];
				byte[] glossMIPData=tilesData[tile].GetGlossMipLevel(mip);
				int len=glossMIPData.Length;
				for(int i=0; i<len; i++) {
					int ix=i%curSize;
					int iy=i/curSize;
					int idx0=iy*curSize*4+ix*2;
					int idx1=idx0+1;
					int idx2=idx0+curSize*2;
					int idx3=idx2+1;
					// musimy ręcznie obliczyć kolejne MIPmapy koloru, GetPixels(dla miplevel>0) potrafi wywalić Unity (coś z przydziałem pamięci poważnie zaczyna szwankować)
					cols[i].r=((prevCols[idx0].r+prevCols[idx1].r+prevCols[idx2].r+prevCols[idx3].r)/4);
					cols[i].g=((prevCols[idx0].g+prevCols[idx1].g+prevCols[idx2].g+prevCols[idx3].g)/4);
					cols[i].b=((prevCols[idx0].b+prevCols[idx1].b+prevCols[idx2].b+prevCols[idx3].b)/4);
					cols[i].a=glossMIPData[i]/255.0f;
				}
				prevCols=cols;
				bx=bx>>1;
				by=by>>1;
				halfSize=halfSize>>1;
				targetTexture.SetPixels(bx,by,halfSize,halfSize,cols, mip);
			}
		}
		targetTexture.Apply(false, false);
		targetTexture.Compress(true);
		targetTexture.Apply(false, true);
		targetTexture.wrapMode=sourceTexture.wrapMode;
		targetTexture.anisoLevel=sourceTexture.anisoLevel;
		targetTexture.filterMode=sourceTexture.filterMode;
		return targetTexture;	
	}
	
	// pojedyncza tekstura
	public Texture2D MakeTexture(Texture2D sourceTexture) {
		Texture2D targetTexture=new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, true);
		Color32[] prevCols=sourceTexture.GetPixels32(0);
		// use gloss shaping for MIP0 (reszta już przygotowana do skopiowania)
		for(int i=0; i<prevCols.Length; i++) {
			float glossiness=Mathf.Clamp01(prevCols[i].a/255.0f*gloss_mult);
			float gloss_shaped=glossiness;
			float gloss_shaped_inv=1-glossiness;
			gloss_shaped=gloss_shaped*gloss_shaped*gloss_shaped;
			gloss_shaped_inv=gloss_shaped_inv*gloss_shaped_inv*gloss_shaped_inv;
			float gls=Mathf.Lerp(gloss_shaped, 1-gloss_shaped_inv, gloss_shaping);
			gls=Mathf.Clamp01(gls);
			prevCols[i].a=(byte)(gls*255.0f);
		}
		targetTexture.SetPixels32(prevCols, 0);
		for(int mip=1; mip<sourceTexture.mipmapCount; mip++) {
			int curSize=(targetTexture.width>>mip);
			Color32[] cols=new Color32[curSize * curSize];
			byte[] glossMIPData=GetGlossMipLevel(mip);
			int len=glossMIPData.Length;
			for(int i=0; i<len; i++) {
				int ix=i%curSize;
				int iy=i/curSize;
				int idx0=iy*curSize*4+ix*2;
				int idx1=idx0+1;
				int idx2=idx0+curSize*2;
				int idx3=idx2+1;
				// musimy ręcznie obliczyć kolejne MIPmapy koloru, GetPixels(dla miplevel>0) potrafi wywalić Unity (coś z przydziałem pamięci poważnie zaczyna szwankować)
				cols[i].r=(byte)((prevCols[idx0].r+prevCols[idx1].r+prevCols[idx2].r+prevCols[idx3].r)>>2);
				cols[i].g=(byte)((prevCols[idx0].g+prevCols[idx1].g+prevCols[idx2].g+prevCols[idx3].g)>>2);
				cols[i].b=(byte)((prevCols[idx0].b+prevCols[idx1].b+prevCols[idx2].b+prevCols[idx3].b)>>2);
				cols[i].a=glossMIPData[i];
			}
			prevCols=cols;
			targetTexture.SetPixels32(cols, mip);
		}
		targetTexture.Apply(false, false);
		targetTexture.Compress(true);
		targetTexture.Apply(false, true);
		targetTexture.wrapMode=sourceTexture.wrapMode;
		targetTexture.anisoLevel=sourceTexture.anisoLevel;
		targetTexture.filterMode=sourceTexture.filterMode;
		return targetTexture;
	}
	
	public void GetMIPGlossMapsFromAtlas(Texture2D atlasTex, int tile) {
		if (atlasTex==null) return;
		int halfSize=atlasTex.width>>1;
		Init(halfSize);
		this.gloss_mult=1;
		this.gloss_shaping=0.5f;
		int bx=(tile%2)*halfSize;
		int by=((tile%4)<2) ? 0 : halfSize;
		Color[] MIP0Cols=atlasTex.GetPixels(bx,by,halfSize,halfSize,0);
		byte[] prevCols=new byte[halfSize*halfSize];
		for(int i=0; i<prevCols.Length; i++) {
			prevCols[i]=(byte)(MIP0Cols[i].a*255.0f);
		}
		for(int mip=1; mip<atlasTex.mipmapCount-1; mip++) {
			int curSize=(atlasTex.width>>(mip+1));
			byte[] glossMIPData=GetGlossMipLevel(mip);
			int len=glossMIPData.Length;
			for(int i=0; i<len; i++) {
				int ix=i%curSize;
				int iy=i/curSize;
				int idx0=iy*curSize*4+ix*2;
				int idx1=idx0+1;
				int idx2=idx0+curSize*2;
				int idx3=idx2+1;
				// musimy ręcznie obliczyć kolejne MIPmapy koloru, GetPixels(dla miplevel>0) potrafi wywalić Unity (coś z przydziałem pamięci poważnie zaczyna szwankować)
				glossMIPData[i]=(byte)((prevCols[idx0]+prevCols[idx1]+prevCols[idx2]+prevCols[idx3])>>2);
			}
			prevCols=glossMIPData;
			bx=bx>>1;
			by=by>>1;
			halfSize=halfSize>>1;
		}		
		baked=true;	
	}
	
	public void PrepareMIPGlossMap(Texture2D DiffuseSpecTexture, Texture2D NormalMap, float gloss_mult, float gloss_shaping, int layerNumForAtlas=-1) {
		if (DiffuseSpecTexture==null) return;
		Init(DiffuseSpecTexture.width);
		this.gloss_mult=gloss_mult;
		this.gloss_shaping=gloss_shaping;
		Color32[] GlossMapMIP0cols=DiffuseSpecTexture.GetPixels32(0);
		// use gloss shaping
		for(int i=0; i<GlossMapMIP0cols.Length; i++) {
			float glossiness=Mathf.Clamp01(GlossMapMIP0cols[i].a/255.0f*gloss_mult);
			float gloss_shaped=glossiness;
			float gloss_shaped_inv=1-glossiness;
			gloss_shaped=gloss_shaped*gloss_shaped*gloss_shaped;
			gloss_shaped_inv=gloss_shaped_inv*gloss_shaped_inv*gloss_shaped_inv;
			float gls=Mathf.Lerp(gloss_shaped, 1-gloss_shaped_inv, gloss_shaping);
			gls=Mathf.Clamp01(gls);
			GlossMapMIP0cols[i].a=(byte)(gls*255.0f);
		}
		Color32[] _NormalMap=new Color32[1];
		int nSize=0;
		if (NormalMap!=null) {
			if (layerNumForAtlas==-1) {
				Debug.Log ("Baking MIP gloss data for \""+DiffuseSpecTexture.name+"\" with normal variance from \""+NormalMap.name+"\"");
			} else {
				Debug.Log ("Baking MIP gloss data (atlased) for layer "+layerNumForAtlas+" with normal variance from \""+NormalMap.name+"\"");
			}
			nSize=NormalMap.width;
			_NormalMap=NormalMap.GetPixels32(0);
		}
		int size=DiffuseSpecTexture.width>>1;
		for(int mipLevel=1; size>0; mipLevel++) {
			int idx=0;
			byte[] glossMIPData=GetGlossMipLevel(mipLevel);
			for(int j=size-1; j>=0; j--) {
				for(int i=0; i<size; i++) {
					float glossiness=MedianGlossiness(i, j, mipLevel, GlossMapMIP0cols);
					if (NormalMap!=null) {
						glossMIPData[idx] = (byte)(BakeGlossinessVsVariance(i*1.0f/size, j*1.0f/size, mipLevel, glossiness, _NormalMap, nSize)*255.0f);
					} else {
						glossMIPData[idx] = (byte)(glossiness*255.0f); // median gloss only
					}
					idx++;
				}
			}
			size=size>>1;
		}
		baked=true;
	}
	
	float MedianGlossiness(int texelPosX, int texelPosY, int mipLevel, Color32[] cols) {
		int texelFootprint = 1 << mipLevel;
		texelPosX*=texelFootprint;
		texelPosY*=texelFootprint;
		int size=Mathf.FloorToInt(Mathf.Sqrt(1.0f*cols.Length));
		texelPosY=size-texelPosY-texelFootprint;
		if(mipLevel == 0) {
			return cols[texelPosY*size+texelPosX].a/255.0f;
		} else {
			int tpX = texelPosX;
			int tpY = texelPosY;
			float avg_specPower=0;
			for(int y = 0; y < texelFootprint; y++) {
				for(int x = 0; x < texelFootprint; x++) {
					int samplePosX = tpX + x;
					int samplePosY = tpY + y;
					float gloss=cols[samplePosY*size+samplePosX].a/255.0f;
					avg_specPower+=Mathf.Pow(2, gloss*10+1);
				}
			}
			avg_specPower -= 1.75f*(float)(texelFootprint * texelFootprint);
			avg_specPower /= (float)(texelFootprint * texelFootprint);
			avg_specPower += 1.75f;
			return (Mathf.Log(avg_specPower)-Mathf.Log(2))/(10*Mathf.Log(2));
		}
	}
	
	float BakeGlossinessVsVariance(float texelPosX, float texelPosY, int mipLevel, float glossiness, Color32[] NormalMap, int nSize) {
		if(mipLevel == 0) {
			return glossiness;
		} else {
			int texelFootprint = 1 << mipLevel;
			int tpX=Mathf.FloorToInt(texelPosX*nSize);
			int tpY=Mathf.FloorToInt((1-texelPosY)*nSize - texelFootprint);
			Vector3 avgNormal = Vector3.zero;
			for(int y = 0; y < texelFootprint; y++) {
				for(int x = 0; x < texelFootprint; x++) {
					int samplePosX = tpX + x;
					int samplePosY = tpY + y;
					Color32 normPix=NormalMap[(nSize-samplePosY-1)*nSize+samplePosX];
					Vector3 sampleNormal = new Vector3(normPix.a/255.0f*2-1, normPix.g/255.0f*2-1, 0);
					sampleNormal.z=1-sampleNormal.x*sampleNormal.x-sampleNormal.y*sampleNormal.y;
					if (sampleNormal.z<0) sampleNormal.z=0; else sampleNormal.z=Mathf.Sqrt(sampleNormal.z);
					sampleNormal.Normalize();
					avgNormal += sampleNormal;
				}
			}
			avgNormal /= (float)(texelFootprint * texelFootprint);
			float variance=0;
			for(int y = 0; y < texelFootprint; y++) {
				for(int x = 0; x < texelFootprint; x++) {
					int samplePosX = tpX + x;
					int samplePosY = tpY + y;
					Color32 normPix=NormalMap[(nSize-samplePosY-1)*nSize+samplePosX];
					Vector3 sampleNormal = new Vector3(normPix.a/255.0f*2-1, normPix.g/255.0f*2-1, 0);
					sampleNormal.z=1-sampleNormal.x*sampleNormal.x-sampleNormal.y*sampleNormal.y;
					if (sampleNormal.z<0) sampleNormal.z=0; else sampleNormal.z=Mathf.Sqrt(sampleNormal.z);
					sampleNormal.Normalize();
					float dot=Vector3.Dot(avgNormal, sampleNormal);
					//variance+=(1 - Mathf.Abs(dot));
					variance+=(1 - dot*dot);
				}
			}			
			variance /= (float)(texelFootprint * texelFootprint);
			float spec_power=Mathf.Pow(2, glossiness*10+1)-1.75f; // (glossiness*2) - reczny tweak, bo filtrowany gloss był nadal zbyt jasny
			float spec_powerMax=Mathf.Pow(2, 1*10+1)-1.75f;
			float varianceP=variance+1/(1+spec_power);
			float variancePP=Mathf.Clamp(varianceP, 1.0f/(1.0f+spec_powerMax), 0.5f);
			float new_glossiness=Mathf.Log(1.0f/variancePP - 1) / Mathf.Log(spec_powerMax);
			return new_glossiness;
		}
	}
	
}
