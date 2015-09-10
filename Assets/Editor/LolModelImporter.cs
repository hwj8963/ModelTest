using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;





class ImportSkn : ScriptableWizard
{
	public UnityEngine.Object sknFile;

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
		string path = AssetDatabase.GetAssetPath(sknFile);
		Debug.Log (path);


		//https://code.google.com/p/lolblender/wiki/fileFormats
		try {
			using(FileStream fs = File.OpenRead (path)) {
				byte[] headerBlock = new byte[8];
				fs.Read(headerBlock,0,8);
				int magicNumber = BitConverter.ToInt32(headerBlock,0);
				short numObjects = BitConverter.ToInt16(headerBlock,4);
				short matHeader = BitConverter.ToInt16(headerBlock,6);
				Debug.Log("magicNumber : " + magicNumber + "\nnumObjects : " + numObjects + "\nmatHeader : " + matHeader);

				if(matHeader == 1) {
					byte[] numMaterialBlock = new byte[4];
					fs.Read(numMaterialBlock,0,4);
					int numMaterials = BitConverter.ToInt32 (numMaterialBlock,0);
					Debug.Log ("numMaterial : " + numMaterials);

					byte[] materialBlock = new byte[80];
					for(int i=0;i<numMaterials;i++) {
						fs.Read(materialBlock,0,80);
						Debug.Log (materialBlock);
						string name = System.Text.Encoding.ASCII.GetString (materialBlock,0,64).TrimEnd('\0');
						Debug.Log (name.Length);
						int startVertex = BitConverter.ToInt32 (materialBlock,64);
						int numMaterialVertices = BitConverter.ToInt32 (materialBlock,68);
						int startIndex = BitConverter.ToInt32 (materialBlock,72);
						int numMaterialIndices = BitConverter.ToInt32(materialBlock,76);

						Debug.Log ("material : " +name + "\nstartVertex : " + startVertex + 
						           "\nnumVertices : " + numMaterialVertices + "\nstartIndex : " + startIndex + 
						           "\nnumIndices : " + numMaterialIndices);
					}
				}

				byte countBlock = new byte[8];
				fs.Read (countBlock,0,8);
				int numIndices = BitConverter.ToInt32 (countBlock,0);
				int numVertices = BitConverter.ToInt32 (countBlock,4);

				Debug.Log("numIndices : " + numIndices + "\nnumVertices : " + numVertices);

				short[] indices = new short[numIndices];
				byte[] indexBlock = new byte[2];
				for(int i=0;i<numIndices;i++) {
					fs.Read (indexBlock,0,2);
					indices[i] = BitConverter.ToInt16 (indexBlock,0);
				}

				Vector3[] vertexPositions = new Vector3[numVertices];
				BoneWeight[] boneWeights = new Vector3[numVertices];
				Vector2[] texcoords = new Vector3[numVertices];
				Vector3[] normals = new Vector3[numVertices];

				byte[] vertexBlock = new byte[52];
				for(int i=0;i<numVertices;i++) {
					fs.Read (vertexBlock,0,52);
					float x = BitConverter.ToSingle(vertexBlock,0);
					float y = BitConverter.ToSingle(vertexBlock,4);
					float z = BitConverter.ToSingle(vertexBlock,8);


				}

				Mesh mesh = new Mesh();
			}
		}
		catch(Exception e) {
			Debug.LogError(e.ToString());
		}



	}

}