using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

class ImportAnm : ScriptableWizard
{
	public UnityEngine.Object anmFile;
	public Transform obj;
	public string name;

	[MenuItem("Custom/LOL/Import Anm")]
	static void CreateWizard() {
		ScriptableWizard.DisplayWizard<ImportAnm> ("Import Skl", "Import");
	}
	void OnWizardCreate() {
		//test create animation

		/*
		AnimationClip clip = new AnimationClip ();

		AnimationCurve curveX = new AnimationCurve ();
		AnimationCurve curveY = new AnimationCurve ();
		AnimationCurve curveZ = new AnimationCurve ();
		for (int i=0; i<=60; i++) {
			curveX.AddKey (i / 60f, i);
			curveY.AddKey (i / 60f, i);
			curveZ.AddKey (i / 60f, i);
		}
		clip.SetCurve ("root", typeof(Transform), "m_LocalPosition.x", curveX);
		clip.SetCurve ("root", typeof(Transform), "m_LocalPosition.y", curveY);
		clip.SetCurve ("root", typeof(Transform), "m_LocalPosition.z", curveZ);

		AssetDatabase.CreateAsset (clip, "Assets/CreatedAsset/testAnim.anim");

		*/
		if (anmFile == null) {
			Debug.LogError ("null skl file");
			return;
		}

		try {
			AnimationClip clip = new AnimationClip();

			string anmPath = AssetDatabase.GetAssetPath (anmFile);


			Dictionary<string,string> nameToPath = new Dictionary<string, string>();
			if(obj != null) {
				Transform[] children = obj.GetComponentsInChildren<Transform>();
				foreach(Transform child in children) {
					Transform cur = child;
					string path = child.name;
					while(cur != null && cur.parent != obj && cur.parent != null) {
						cur = cur.parent;
						path = cur.name + "/" + path;
					}
					nameToPath.Add (child.name,path);
				}

			}
			using (FileStream fs = File.OpenRead(anmPath)) {
				byte[] headerBlock = new byte[28];
				fs.Read (headerBlock, 0, 28);
				string id = System.Text.Encoding.ASCII.GetString (headerBlock, 0, 8).TrimEnd ('\0');
				int version = BitConverter.ToInt32 (headerBlock, 8);
				int dontknow = BitConverter.ToInt32 (headerBlock, 12);
				int numOfBones = BitConverter.ToInt32 (headerBlock, 16);
				int numOfFrames = BitConverter.ToInt32 (headerBlock, 20);
				int playBackFPS = BitConverter.ToInt32 (headerBlock, 24);

				//int boneBlockSize = 36 + (numOfFrames * 28);
				byte[] boneHeaderBlock = new byte[36];
				byte[] frameBlock = new byte[28];


				for (int i=0; i<numOfBones; i++) {
					fs.Read (boneHeaderBlock, 0, 36);
					string boneName = System.Text.Encoding.ASCII.GetString (boneHeaderBlock, 0, 32).TrimEnd ('\0');
					int dontknow2 = BitConverter.ToInt32 (boneHeaderBlock, 32);

					string path = boneName;
					if(nameToPath.ContainsKey(boneName)) {
						path = nameToPath[boneName];
					}

					AnimationCurve curveRX = new AnimationCurve();
					AnimationCurve curveRY = new AnimationCurve();
					AnimationCurve curveRZ = new AnimationCurve();
					AnimationCurve curveRW = new AnimationCurve();

					AnimationCurve curvePX = new AnimationCurve();
					AnimationCurve curvePY = new AnimationCurve();
					AnimationCurve curvePZ = new AnimationCurve();


					for (int j=0; j<numOfFrames; j++) {
						fs.Read (frameBlock, 0, 28);
						float rx = BitConverter.ToSingle (frameBlock, 0);
						float ry = BitConverter.ToSingle (frameBlock, 4);
						float rz = BitConverter.ToSingle (frameBlock, 8);
						float rw = BitConverter.ToSingle (frameBlock, 12);
						float px = BitConverter.ToSingle (frameBlock, 16);
						float py = BitConverter.ToSingle (frameBlock, 20);
						float pz = BitConverter.ToSingle (frameBlock, 24);
						float time = j/60f;
						curveRX.AddKey(time,rx);
						curveRY.AddKey(time,ry);
						curveRZ.AddKey (time,rz);
						curveRW.AddKey(time,rw);
						curvePX.AddKey(time,px);
						curvePY.AddKey(time,py);
						curvePZ.AddKey(time,pz);
					}
					clip.SetCurve(path,typeof(Transform),"m_LocalRotation.x",curveRX);
					clip.SetCurve(path,typeof(Transform),"m_LocalRotation.y",curveRY);
					clip.SetCurve(path,typeof(Transform),"m_LocalRotation.z",curveRZ);
					clip.SetCurve(path,typeof(Transform),"m_LocalRotation.w",curveRW);
					clip.SetCurve(path,typeof(Transform),"m_LocalPosition.x",curvePX);
					clip.SetCurve(path,typeof(Transform),"m_LocalPosition.y",curvePY);
					clip.SetCurve(path,typeof(Transform),"m_LocalPosition.z",curvePZ);

				}


				/*
				Debug.Log ("version : " + version + "\nnum of bones : " + numOfBones + "\nnum of frames : " + numOfFrames + "\nplaybackFPS : " + playBackFPS);

				int expectedsize = 28 + ((28 * numOfFrames) + 36) * numOfBones;
				Debug.Log ("Expected Size : " + expectedsize + "\nread Size : " + fs.Length);*/

				AssetDatabase.CreateAsset(clip,"Assets/CreatedAsset/"+name+".anim");

			}
		} catch (Exception e) {
			Debug.LogError(e);
		}
	}
}


class ImportSkl : ScriptableWizard
{
	public UnityEngine.Object sklFile;
	public Mesh mesh;
	public string name;
	[MenuItem("Custom/LOL/Import Skl")]
	static void CreateWizard() {
		ScriptableWizard.DisplayWizard<ImportSkl> ("Import Skl", "Import");

	}

	void OnWizardCreate()
	{
		if (sklFile == null) {
			Debug.LogError("null skl file");
			return;
		}

		GameObject go = new GameObject(name);
		
		string sklPath = AssetDatabase.GetAssetPath(sklFile);
        using (FileStream fs = File.OpenRead(sklPath))
        {
            byte[] headerBlock = new byte[20];
            fs.Read(headerBlock, 0, 20);
            string magic = System.Text.Encoding.ASCII.GetString(headerBlock, 0, 8).TrimEnd('\0');
            int version = BitConverter.ToInt32(headerBlock, 8);
            int skeletonHash = BitConverter.ToInt32(headerBlock, 12);
            int numElements = BitConverter.ToInt32(headerBlock, 16);

            Transform[] bones = new Transform[numElements];
            for (int i = 0; i < numElements; i++)
            {
                bones[i] = new GameObject().transform;
            }

            byte[] boneBlock = new byte[88];
            Transform rootBone = null;
            for (int i = 0; i < numElements; i++)
            {
                fs.Read(boneBlock, 0, 88);
                string boneName = System.Text.Encoding.ASCII.GetString(boneBlock, 0, 32).TrimEnd('\0');



                int parent = BitConverter.ToInt32(boneBlock, 32);
                float scale = BitConverter.ToSingle(boneBlock, 36);
                float[,] matrix = new float[3, 4];
                for (int n = 0; n < 3; n++)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        matrix[n, m] = BitConverter.ToSingle(boneBlock, 40 + n * 16 + m * 4);
                    }
                }



                bones[i].gameObject.name = boneName;
                bones[i].parent = parent >= 0 ? bones[parent] : go.transform;


                if (boneName == "root" && parent < 0)
                {
                    rootBone = bones[i];
                }


                bones[i].position = new Vector3(matrix[0, 3], matrix[1, 3], matrix[2, 3]);


                //bones[i].localRotation = new Quaternion(qx,qy,qz,qw);

                Quaternion q = new Quaternion();
                q.w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix[0, 0] + matrix[1, 1] + matrix[2, 2])) / 2f;
                q.x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix[0, 0] - matrix[1, 1] - matrix[2, 2])) / 2f;
                q.y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix[0, 0] + matrix[1, 1] - matrix[2, 2])) / 2f;
                q.z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix[0, 0] - matrix[1, 1] + matrix[2, 2])) / 2f;
                q.x *= Mathf.Sign(q.x * (matrix[2, 1] - matrix[1, 2]));
                q.y *= Mathf.Sign(q.y * (matrix[0, 2] - matrix[2, 0]));
                q.z *= Mathf.Sign(q.z * (matrix[1, 0] - matrix[0, 1]));

                bones[i].rotation = q;


                float sx = (new Vector3(matrix[0, 0], matrix[1, 0], matrix[2, 0])).magnitude;
                float sy = (new Vector3(matrix[0, 1], matrix[1, 1], matrix[2, 1])).magnitude;
                float sz = (new Vector3(matrix[0, 2], matrix[1, 2], matrix[2, 2])).magnitude;

                bones[i].localScale = new Vector3(sx, sy, sz);
            }

            int numBones = numElements;
            Transform[] bones2 = null;

            
            if (version == 2)
            {
                byte[] numBoneIDsBlock = new byte[4];
                fs.Read(numBoneIDsBlock, 0, 4);
                int numBoneIDs = BitConverter.ToInt32(numBoneIDsBlock, 0);

                byte[] ReorderingBlock = new byte[4 * numBoneIDs];
                fs.Read(ReorderingBlock, 0, 4 * numBoneIDs);

                bones2 = new Transform[numBoneIDs];

                for (int i = 0; i < numBoneIDs; i++)
                {
                    int id = BitConverter.ToInt32(ReorderingBlock, i * 4);
                    bones2[i] = bones[id];
                }
                numBones = numBoneIDs;
            } else  {
                numBones = numElements;
                bones2 = bones;

            }



            Matrix4x4[] bindPoses = new Matrix4x4[numBones];
			for(int i=0;i< numBones; i++) {
				bindPoses[i] = bones2[i].worldToLocalMatrix * go.transform.localToWorldMatrix;
			}
			
			mesh.bindposes = bindPoses;

			GameObject rendererObj = new GameObject("renderer");
			rendererObj.transform.parent = go.transform;

			SkinnedMeshRenderer renderer = rendererObj.AddComponent<SkinnedMeshRenderer>();
			renderer.bones = bones2;
			renderer.sharedMesh = mesh;
			if(rootBone != null) {
				renderer.rootBone = rootBone;
			}
			
			
		}
		
		//AssetDatabase.CreateAsset(go,"Assets/CreatedAsset/" + name + ".prefab");
	}
}




class ImportSkn : ScriptableWizard
{
	public UnityEngine.Object sknFile;
	//public UnityEngine.Object sklFile;
	public string name;
	[MenuItem ("Custom/LOL/Import Skn")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard<ImportSkn>("Import Skn","Import");
	}

	void OnWizardCreate() 
	{
		if(sknFile == null) {
			Debug.LogError("null skn file");
			return;
		}

		//https://code.google.com/p/lolblender/wiki/fileFormats
		try {

			Mesh mesh = null;

			string sknPath = AssetDatabase.GetAssetPath(sknFile);
			using(FileStream fs = File.OpenRead (sknPath)) {
				byte[] headerBlock = new byte[8];
				fs.Read(headerBlock,0,8);
				int magicNumber = BitConverter.ToInt32(headerBlock,0);
				short numObjects = BitConverter.ToInt16(headerBlock,4);
				short matHeader = BitConverter.ToInt16(headerBlock,6);
				//Debug.Log("magicNumber : " + magicNumber + "\nnumObjects : " + numObjects + "\nmatHeader : " + matHeader);

				if(matHeader == 1) {
					byte[] numMaterialBlock = new byte[4];
					fs.Read(numMaterialBlock,0,4);
					int numMaterials = BitConverter.ToInt32 (numMaterialBlock,0);
					//Debug.Log ("numMaterial : " + numMaterials);

					byte[] materialBlock = new byte[80];
					for(int i=0;i<numMaterials;i++) {
						fs.Read(materialBlock,0,80);
						//Debug.Log (materialBlock);
						string materialName = System.Text.Encoding.ASCII.GetString (materialBlock,0,64).TrimEnd('\0');
						//Debug.Log (name.Length);
						int startVertex = BitConverter.ToInt32 (materialBlock,64);
						int numMaterialVertices = BitConverter.ToInt32 (materialBlock,68);
						int startIndex = BitConverter.ToInt32 (materialBlock,72);
						int numMaterialIndices = BitConverter.ToInt32(materialBlock,76);

						//Debug.Log ("material : " +materialName + "\nstartVertex : " + startVertex + 
						//           "\nnumVertices : " + numMaterialVertices + "\nstartIndex : " + startIndex + 
						//           "\nnumIndices : " + numMaterialIndices);
					}
				}

				byte[] countBlock = new byte[8];
				fs.Read (countBlock,0,8);
				int numIndices = BitConverter.ToInt32 (countBlock,0);
				int numVertices = BitConverter.ToInt32 (countBlock,4);

				//Debug.Log("numIndices : " + numIndices + "\nnumVertices : " + numVertices);

				int[] indices = new int[numIndices];
				byte[] indexBlock = new byte[2];
				for(int i=0;i<numIndices;i++) {
					fs.Read (indexBlock,0,2);
					indices[i] = (int)BitConverter.ToInt16 (indexBlock,0);
				}

				Vector3[] vertices = new Vector3[numVertices];
				BoneWeight[] boneWeights = new BoneWeight[numVertices];
				Vector2[] uv = new Vector2[numVertices];
				Vector3[] normals = new Vector3[numVertices];

				byte[] vertexBlock = new byte[52];

				//for debugging
				//System.Collections.Generic.Dictionary<int,int> boneIdxNums = new System.Collections.Generic.Dictionary<int, int>();
				////

				for(int i=0;i<numVertices;i++) {
					fs.Read (vertexBlock,0,52);
					float x = BitConverter.ToSingle(vertexBlock,0);
					float y = BitConverter.ToSingle(vertexBlock,4);
					float z = BitConverter.ToSingle(vertexBlock,8);
					int boneIdx0 = (int)vertexBlock[12];
					int boneIdx1 = (int)vertexBlock[13];
					int boneIdx2 = (int)vertexBlock[14];
					int boneIdx3 = (int)vertexBlock[15];
					float boneWeight0 = BitConverter.ToSingle(vertexBlock,16);
					float boneWeight1 = BitConverter.ToSingle(vertexBlock,20);
					float boneWeight2 = BitConverter.ToSingle(vertexBlock,24);
					float boneWeight3 = BitConverter.ToSingle(vertexBlock,28);

					float normalX = BitConverter.ToSingle(vertexBlock,32);
					float normalY = BitConverter.ToSingle(vertexBlock,36);
					float normalZ = BitConverter.ToSingle(vertexBlock,40);

					float u = BitConverter.ToSingle(vertexBlock,44);
					float v = 1-BitConverter.ToSingle(vertexBlock,48);


					//////basic skin blitzcrank bone idx matching error temporary fix >22, +=2
					//////custom skin blitzcrank bone idx matching error temporary fix >20 +=2
                    /*
					if(boneIdx0 >= 20) {
						boneIdx0 +=2;
					}
					if(boneIdx1 >= 20) {
						boneIdx1 +=2;
					}
					if(boneIdx2 >= 20) {
						boneIdx2 += 2;
					}
					if(boneIdx3 >= 20) {
						boneIdx3 += 2;
					}*/
					//////////

					vertices[i] = new Vector3(x,y,z);
					boneWeights[i].boneIndex0 = (int)boneIdx0;
					boneWeights[i].boneIndex1 = (int)boneIdx1;
					boneWeights[i].boneIndex2 = (int)boneIdx2;
					boneWeights[i].boneIndex3 = (int)boneIdx3;
					boneWeights[i].weight0 = boneWeight0;
					boneWeights[i].weight1 = boneWeight1;
					boneWeights[i].weight2 = boneWeight2;
					boneWeights[i].weight3 = boneWeight3;
					normals[i] = new Vector3(normalX,normalY,normalZ);
					uv[i] = new Vector2(u,v);

					//for debugging
					/*
					if(boneWeight0 > 0) {
						if(boneIdxNums.ContainsKey(boneIdx0)) {
							boneIdxNums[boneIdx0] = boneIdxNums[boneIdx0]+1;
						} else {
							boneIdxNums.Add (boneIdx0,1);
						}
					} 
					if(boneWeight1 > 0) {
						if(boneIdxNums.ContainsKey(boneIdx1)) {
							boneIdxNums[boneIdx1] = boneIdxNums[boneIdx1]+1;
						} else {
							boneIdxNums.Add (boneIdx1,1);
						}
					}
					if(boneWeight2 > 0) {
						if(boneIdxNums.ContainsKey(boneIdx2)) {
							boneIdxNums[boneIdx2] = boneIdxNums[boneIdx2]+1;
						} else {
							boneIdxNums.Add (boneIdx2,1);
						}
					}
					if(boneWeight3 > 0) {
						if(boneIdxNums.ContainsKey(boneIdx3)) {
							boneIdxNums[boneIdx3] = boneIdxNums[boneIdx3]+1;
						} else {
							boneIdxNums.Add (boneIdx3,1);
						}
					}*/
					///////


				}
				///////for debugging
				/*System.Text.StringBuilder sb = new System.Text.StringBuilder();

				System.Collections.Generic.List<int> keys = new System.Collections.Generic.List<int>(boneIdxNums.Keys);
				keys.Sort ();
				foreach(int idx in keys) {
					sb.Append (string.Format ("idx {0} : {1}\n",idx,boneIdxNums[idx]));
				}
				Debug.Log(sb.ToString());*/
				///////
				mesh = new Mesh();
				mesh.vertices = vertices;
				mesh.uv = uv;
				mesh.triangles = indices;
				mesh.boneWeights = boneWeights;
				mesh.normals = normals;	

				AssetDatabase.CreateAsset(mesh,"Assets/CreatedAsset/" + name + ".asset");

			}/*
			if(mesh == null) {
				Debug.LogError("null mesh");
				return;
			}

			GameObject go = new GameObject(name);

			string sklPath = AssetDatabase.GetAssetPath(sklFile);
			using(FileStream fs = File.OpenRead (sklPath)) {
				byte[] headerBlock = new byte[20];
				fs.Read(headerBlock,0,20);
				string version = System.Text.Encoding.ASCII.GetString (headerBlock,0,8).TrimEnd('\0');
				int numObjects = BitConverter.ToInt32(headerBlock,8);
				int skeletonHash = BitConverter.ToInt32 (headerBlock,12);
				int numElements = BitConverter.ToInt32 (headerBlock,16);

				Transform[] bones = new Transform[numElements];
				for(int i=0;i<numElements;i++) {
					bones[i] = new GameObject().transform;
				}

				byte[] boneBlock = new byte[88];
				for(int i=0;i<numElements;i++) {
					fs.Read (boneBlock,0,88);
					string boneName = System.Text.Encoding.ASCII.GetString (boneBlock,0,32).TrimEnd('\0');
					int parent = BitConverter.ToInt32 (boneBlock,32);
					float scale = BitConverter.ToSingle (boneBlock,36);
					float[,] matrix = new float[3,4];
					for(int n=0;n<3;n++) { 
						for(int m=0;m<4;m++) {
							matrix[n,m] = BitConverter.ToSingle(boneBlock,40 + n*16 + m*4);
						}
					}

					bones[i].gameObject.name = boneName;
					bones[i].parent = parent>=0?bones[parent]:go.transform;

					bones[i].localPosition = new Vector3(matrix[0,3],matrix[1,3],matrix[2,3]);

					float qw = Mathf.Sqrt (1f + matrix[0,0] + matrix[1,1] + matrix[2,2])/2f;
					float w  = 4f * qw;
					float qx = (matrix[2,1] - matrix[1,2])/w;
					float qy = (matrix[0,2] - matrix[2,0])/w;
					float qz = (matrix[1,0] - matrix[0,1])/w;

					bones[i].localRotation = new Quaternion(qx,qy,qz,qw);

					float sx = Mathf.Sqrt (matrix[0,0]*matrix[0,0] + matrix[0,1]*matrix[0,1] + matrix[0,2]*matrix[0,2]);
					float sy = Mathf.Sqrt (matrix[1,0]*matrix[1,0] + matrix[1,1]*matrix[1,1] + matrix[1,2]*matrix[1,2]);
					float sz = Mathf.Sqrt (matrix[2,0]*matrix[2,0] + matrix[2,1]*matrix[2,1] + matrix[2,2]*matrix[2,2]);

					bones[i].localScale = new Vector3(sx,sy,sz);
				}

				Matrix4x4[] bindPoses = new Matrix4x4[numElements];
				for(int i=0;i<numElements;i++) {
					bindPoses[i] = bones[i].worldToLocalMatrix * go.transform.localToWorldMatrix;
				}

				mesh.bindposes = bindPoses;

				SkinnedMeshRenderer renderer = go.AddComponent<SkinnedMeshRenderer>();
				renderer.bones = bones;
				renderer.sharedMesh = mesh;
			
				
			}

			AssetDatabase.CreateAsset(go,"Assets/CreatedAsset/" + name + ".prefab");

*/






		}
		catch(Exception e) {
			Debug.LogError(e.ToString());
			return;
		}





	}

}