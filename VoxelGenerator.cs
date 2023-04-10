using System.Collections.Generic;
using UnityEngine;

namespace VoxelText.Baking
{
    public static class VoxelGenerator
    {
        const float BASE_X_SIZE = 55f;
        const float BASE_Y_SIZE = 5f;

        const float CUBE_X_SIZE = 1f;
        const float CUBE_Y_SIZE = 1f;
        const float CUBE_Z_SIZE = 1f;

        //static Vector2 TTL_RANGE = new Vector2(2f, 5f);
        static Dictionary<string, Mesh> _meshCache = new Dictionary<string, Mesh>();

        public const string PrefabName = "";

        static bool _inited = false;

        static GameObject _effectPrefab;
        public static GameObject EffectPrefab
        {
            get 
            { 
                Init(); 
                return _effectPrefab; 
            }
        }

        public static void Init()
        {
            if (_inited)
                return;

            //_effectPrefab = Resources.Load<GameObject>("entities/VoxelTextEffect");
            _effectPrefab = Resources.Load<GameObject>("Prefabs/VoxelText");

            _inited = true; 
        }

        public static Mesh GetMeshFromMatrix(bool[,] matrix)
        {
            Mesh res = null;
            if (matrix.Length <= 0 || matrix.Rank != 2)
                return res;
            var sizeX = matrix.GetLength(0);
            var sizeY = matrix.GetLength(1);
            if (sizeX <= 0 || sizeY <= 0)
                return res;
            res = new Mesh();
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (matrix[i, j])
                    {
                        AddCubeElement(ref res, GetCubeCenter(i, j), new Vector2((float)i / sizeX, (float)j / sizeY));
                    }
                }
            }

            return res;
        }

        public static Mesh GetMeshForString(string text, float alphaValue)
        {
            if (_meshCache.ContainsKey(text))
            {
                return _meshCache[text];
            }
            var _res_tex = TextureConcatenator.GenerateSummaryTexture(text);
            var mesh = GetMeshFromTexture(_res_tex, alphaValue);
            _meshCache.Add(text, mesh);
            return mesh; 
        }

        public static Mesh GetMeshFromTexture(Texture2D texture, float alphaValue)
        {
            Mesh res = null;
            if (texture == null)
                return res;
            var sizeX = texture.width;
            var sizeY = texture.height;
            if (sizeX <= 0 || sizeY <= 0)
                return res;
            if (sizeX > BASE_X_SIZE * 2 || sizeY > BASE_Y_SIZE * 2)
                return res;
            res = new Mesh();
            var aValue = alphaValue * 255;
            var pixels = texture.GetPixels32();
            for (int i =0; i < pixels.Length; i++)
            {
                if (pixels[i].a > aValue)
                {
                    var x = i % sizeX;
                    var y = i / sizeX;
                    AddCubeElement(ref res, GetCubeCenter(x, y), new Vector2((float)x / sizeX, (float)y / sizeY));
                }
            }

            //for (int i = 0; i < sizeX; i++)
            //{
            //    for (int j = 0; j < sizeY; j++)
            //    {
            //        if (GetPixel(i, j).r > 0f)
            //        {
            //            AddCubeElement(ref res, GetCubeCenter(i, j), new Vector2(i / sizeX, j / sizeY));
            //        }
            //    }
            //}

            return res;
        }


        static Vector3 GetCubeCenter(int x, int y)
        {
            return new Vector3(x * CUBE_X_SIZE * 1.1f, 0f, y * CUBE_Y_SIZE * 1.1f);
        }

        static Vector3[] cubeVert = 
            {
            new Vector3(0, CUBE_Y_SIZE, 0),
            new Vector3(0, 0, 0),
            new Vector3(CUBE_X_SIZE, CUBE_Y_SIZE, 0),
            new Vector3(CUBE_X_SIZE, 0, 0),

            new Vector3(0, 0, CUBE_Z_SIZE),
            new Vector3(CUBE_X_SIZE, 0, CUBE_Z_SIZE),
            new Vector3(0, CUBE_Y_SIZE, CUBE_Z_SIZE),
            new Vector3(CUBE_X_SIZE, CUBE_Y_SIZE, CUBE_Z_SIZE),

            new Vector3(0, CUBE_Y_SIZE, 0),
            new Vector3(CUBE_X_SIZE, CUBE_Y_SIZE, 0),

            new Vector3(0, CUBE_Y_SIZE, 0),
            new Vector3(0, CUBE_Y_SIZE, CUBE_Z_SIZE),

            new Vector3(CUBE_X_SIZE, CUBE_Y_SIZE, 0),
            new Vector3(CUBE_X_SIZE, CUBE_Y_SIZE, CUBE_Z_SIZE),
        };

        
        static Vector2[] cubeUVs =
            {
            new Vector2 (1, 0),         //0+
            new Vector2 (1, 1),         //1+
            new Vector2 (0, 0),         //2+
            new Vector2 (0, 1),         //3+
            new Vector2 (1, 1),         //4+
            new Vector2 (1, 0),         //5+
            new Vector2 (0, 1),         //6+
            new Vector2 (0, 0),         //7+

            new Vector2 (1, 1),         //8+
            new Vector2 (1, 0),         //9+
            new Vector2 (0, 1),         //10
            new Vector2 (0, 0),         //11+
            new Vector2 (1, 1),         //12
            new Vector2 (0, 1),         //13
        };

        static int[] cubeTriangles = 
        {
            0, 2, 1, // front
			1, 2, 3,
            4, 5, 6, // back
			5, 7, 6,
            6, 7, 8, //top
			7, 9 ,8,
            1, 3, 4, //bottom
			3, 5, 4,
            1, 11,10,// left
			1, 4, 11,
            3, 12, 5,//right
			5, 12, 13
        };

        static void AddCubeElement(ref Mesh mesh, Vector3 center, Vector2 uv)
        {
            var verts = mesh.vertices;
            var trians = mesh.triangles;
            int vLength = verts.Length;
            int tLength = trians.Length;

            var norms = mesh.normals;
            int nLength = norms.Length;
            var uvs = mesh.uv;
            int uvLength = uvs.Length;
            var colors = mesh.colors;
            int clrLength = colors.Length;

            var nLen = cubeVert.Length;

            var newVerts = new Vector3[vLength + nLen];
            var newTrian = new int[tLength + cubeTriangles.Length];
            var newNorms = new Vector3[nLength + nLen];
            var newUVs = new Vector2[uvLength + nLen];
            var newColors = new Color[clrLength + nLen];

            verts.CopyTo(newVerts, 0);
            norms.CopyTo(newNorms, 0);
            uvs.CopyTo(newUVs, 0);
            colors.CopyTo(newColors, 0);

            //float ttl = UnityEngine.Random.Range(TTL_RANGE[0], TTL_RANGE[1]);

            var cubeCenter = new Vector3(CUBE_X_SIZE * 0.5f, CUBE_Y_SIZE * 0.5f, CUBE_Z_SIZE * 0.5f);

            for (int i = 0; i < cubeVert.Length; i++)
            {
                newVerts[vLength + i] = cubeVert[i] + center;
                newNorms[nLength + i] = (cubeVert[i] - cubeCenter).normalized;
                newUVs[uvLength + i] = cubeUVs[i];
                newColors[clrLength + i] = new Color(uv.x, uv.y, 0f, 0f);
            }

            trians.CopyTo(newTrian, 0);
            for (int i = 0; i < cubeTriangles.Length; i++)
            {
                newTrian[tLength + i] = cubeTriangles[i] + vLength;
            }
            
            mesh.SetVertices(newVerts);
            mesh.SetNormals(newNorms);
            mesh.SetColors(newColors);
            mesh.SetUVs(0, newUVs);

            mesh.SetTriangles(newTrian, 0);

            mesh.Optimize();
        }
    }
}
