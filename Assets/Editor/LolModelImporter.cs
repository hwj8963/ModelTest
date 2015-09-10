using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

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
						Debug.Log (materialBlock);
						string materialName = System.Text.Encoding.ASCII.GetString (materialBlock,0,64).TrimEnd('\0');
						Debug.Log (name.Length);
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
				for(int i=0;i<numVertices;i++) {
					fs.Read (vertexBlock,0,52);
					float x = BitConverter.ToSingle(vertexBlock,0);
					float y = BitConverter.ToSingle(vertexBlock,4);
					float z = BitConverter.ToSingle(vertexBlock,8);
					char boneIdx0 = BitConverter.ToChar(vertexBlock,12);
					char boneIdx1 = BitConverter.ToChar(vertexBlock,13);
					char boneIdx2 = BitConverter.ToChar(vertexBlock,14);
					char boneIdx3 = BitConverter.ToChar(vertexBlock,15);
					float boneWeight0 = BitConverter.ToSingle(vertexBlock,16);
					float boneWeight1 = BitConverter.ToSingle(vertexBlock,20);
					float boneWeight2 = BitConverter.ToSingle(vertexBlock,24);
					float boneWeight3 = BitConverter.ToSingle(vertexBlock,28);

					float normalX = BitConverter.ToSingle(vertexBlock,32);
					float normalY = BitConverter.ToSingle(vertexBlock,36);
					float normalZ = BitConverter.ToSingle(vertexBlock,40);

					float u = BitConverter.ToSingle(vertexBlock,44);
					float v = 1-BitConverter.ToSingle(vertexBlock,48);



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

				}


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