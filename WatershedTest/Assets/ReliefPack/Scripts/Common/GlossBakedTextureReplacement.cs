using UnityEngine;
using System.Collections;

//
// apply to object (or select material to work on) which you'd like to use prebaked (in ReliefTerrain script) gloss data affected by bumpmap variance
// select glossBakedData from previously saved file
// REMEMBER that originalTexture (taken from material) must be readabe
//
[AddComponentMenu("Relief Terrain/Helpers/Use baked gloss texture")]
[ExecuteInEditMode]
public class GlossBakedTextureReplacement : MonoBehaviour {
	public RTPGlossBaked glossBakedData;
	public bool RTPStandAloneShader=false;
	public int layerNumber=1;
	public Material CustomMaterial;
	public Texture2D originalTexture=null;
	public bool resetGlossMultAndShaping=false;
	[System.NonSerialized] public Texture2D bakedTexture=null;
	
	public GlossBakedTextureReplacement() {
		bakedTexture=originalTexture=null;
	}
	
	void Start () {
		Refresh();
	}
	
	void Update () {
		if (!Application.isPlaying) {
			Refresh();
			if (resetGlossMultAndShaping) {
				resetGlossMultAndShaping=false;
				resetGlossMultAndShapingFun();
			}
		}
	}

	public void resetGlossMultAndShapingFun() {
		if (glossBakedData==null) return;

		Material _mat;
		if (CustomMaterial!=null) {
			_mat=CustomMaterial;
		} else {
			if (!GetComponent<Renderer>()) return;
			_mat=GetComponent<Renderer>().sharedMaterial;
		}
		if (!_mat) return;
		if (RTPStandAloneShader) {
			Vector4 multVec=new Vector4(1,1,1,1);
			Vector4 shapingVec=new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
			if (_mat.HasProperty("RTP_gloss_mult0123")) {
				multVec=_mat.GetVector("RTP_gloss_mult0123");
				if (layerNumber>=1 && layerNumber<=4) {
					multVec[layerNumber-1]=1;
				}
				_mat.SetVector("RTP_gloss_mult0123", multVec);
			}
			if (_mat.HasProperty("RTP_gloss_shaping0123")) {
				shapingVec=_mat.GetVector("RTP_gloss_shaping0123");
				if (layerNumber>=1 && layerNumber<=4) {
					shapingVec[layerNumber-1]=0.5f;
				}
				_mat.SetVector("RTP_gloss_shaping0123", shapingVec);
			}
		} else {
			string shaderPropGlossMult="RTP_gloss_mult0";
			string shaderPropGlossShaping="RTP_gloss_shaping0";
			if (layerNumber==2) {
				shaderPropGlossMult="RTP_gloss_mult1";
				shaderPropGlossShaping="RTP_gloss_shaping1";
			}
			if (_mat.HasProperty(shaderPropGlossMult)) {
				_mat.SetFloat(shaderPropGlossMult, 1);
			}
			if (_mat.HasProperty(shaderPropGlossShaping)) {
				_mat.SetFloat(shaderPropGlossShaping, 0.5f);
			}
		}
	}
	
	public void Refresh() {
		if (glossBakedData==null) return;

		string shaderProp="_MainTex";
		if (RTPStandAloneShader) {
			shaderProp="_SplatA0";
			if (layerNumber==2) {
				shaderProp="_SplatA1";
			} else if (layerNumber==3) {
				shaderProp="_SplatA2";
			} else if (layerNumber==4) {
				shaderProp="_SplatA3";
			}
		} else {
			if (layerNumber==2) {
				shaderProp="_MainTex2";
			}
		}
		Material _mat;
		if (CustomMaterial!=null) {
			_mat=CustomMaterial;
		} else {
			if (!GetComponent<Renderer>()) return;
			_mat=GetComponent<Renderer>().sharedMaterial;
		}
		if (!_mat) return;

		// baked texture
		if (_mat.HasProperty(shaderProp)) {
			if (bakedTexture) {
				_mat.SetTexture(shaderProp, bakedTexture);
			} else {
				if (originalTexture==null) {
					originalTexture=(Texture2D)_mat.GetTexture(shaderProp);
				}
				if (originalTexture!=null) {
					if ( (glossBakedData!=null) && (!glossBakedData.used_in_atlas) && glossBakedData.CheckSize(originalTexture) ) {
						// mamy przygotowany gloss - zrób texturę tymczasową
						bakedTexture=glossBakedData.MakeTexture(originalTexture);
						// i zapodaj shaderowi
						if (bakedTexture) _mat.SetTexture(shaderProp, bakedTexture);
					}
				}
			}
		}

	}
}
