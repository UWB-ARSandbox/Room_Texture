using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace UWB_Texturing
{
    public static class CustomMesh
    {
        /// <summary>
        /// Sub-class used to hold constant strings for exception messages, 
        /// debug messages, and user messages.
        /// </summary>
        public static class Messages
        {
            /// <summary>
            /// Use when mesh array passed in from other source is invalid for use.
            /// </summary>
            public static string InvalidMeshArray = "Invalid mesh array. Mesh was null or had zero entries.";
        }

        #region Constants
        public static string Separator = "\n";
        public static string MeshID = "o ";
        public static string VertexID = "v ";
        public static string VertexNormalID = "vn ";
        public static string TriangleID = "f ";

        public static string SupplementaryInfoSeparator = "===";
        public static string PositionID = "MeshPositions";
        public static string RotationID = "MeshRotations";
        #endregion

        #region Methods
        #region Main Methods

        /// <summary>
        /// Iterates through an array of meshes to generate a text file 
        /// representing this mesh.
        /// 
        /// General structure:
        /// o MeshName
        /// v vertexX vertexY vertexZ
        /// 
        /// vn vertexNormalX vertexNormalY vertexNormalZ
        /// 
        /// f triangleVertex1//triangleVertex1 triangleVertex2//triangleVertex2 triangleVertex3//triangleVertex3
        /// 
        /// NOTE: Referenced from ExportOBJ file online @ http://wiki.unity3d.com/index.php?title=ExportOBJ
        /// </summary>
        /// <param name="meshes">
        /// A non-null array of meshes with vertices and triangles.
        /// </param>
        public static void SaveMesh(Mesh[] meshes)
        {
            if (meshes != null && meshes.Length > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < meshes.Length; i++)
                {
                    Mesh mesh = meshes[i];

                    sb.Append(Separator);
                    sb.Append(MeshID + "RoomMesh_" + i + '\n');
                    foreach (Vector3 vertex in mesh.vertices)
                    {
                        sb.Append(GetVertexString(vertex));
                    }
                    sb.Append(Separator);
                    foreach (Vector3 normal in mesh.normals)
                    {
                        sb.Append(GetVertexNormalString(normal));
                    }
                    sb.Append(Separator);
                    // Append triangles (i.e. which vertices each triangle uses)
                    for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                    {
                        sb.Append(Separator);
                        sb.Append("submesh" + submesh + "\n");
                        int[] triangles = mesh.GetTriangles(submesh);
                        for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex += 3)
                        {
                            sb.Append(string.Format("f {0}//{0} {1}//{1} {2}//{2}\n", triangles[triangleIndex], triangles[triangleIndex + 1], triangles[triangleIndex + 2]));
                        }
                    }
                }
                
                File.WriteAllText(CrossPlatformNames.Mesh.OutputFilepath, sb.ToString());

                //using (StreamWriter sw = new StreamWriter(OutputFilepath, false))
                //{
                //    sw.Write(sb.ToString());
                //}
            }
            else
            {
                Debug.Log(Messages.InvalidMeshArray);
            }
        }

        /// <summary>
        /// Turns a vertex Vector3 into a readable vertex string of "v vertexX 
        /// vertexY vertexZ".
        /// </summary>
        /// <param name="vertex">
        /// A Vector3 representing a mesh vertex.
        /// </param>
        /// <returns>
        /// A readable string representing the vertex's entry in the mesh text 
        /// file.
        /// </returns>
        public static string GetVertexString(Vector3 vertex)
        {
            string vertexString = "";
            vertexString += VertexID;
            vertexString += vertex.x.ToString() + ' ';
            vertexString += vertex.y.ToString() + ' ';
            vertexString += vertex.z.ToString() + '\n';

            return vertexString;
        }

        /// <summary>
        /// Turns a vertexNormal Vector3 into a readable vertexNormal string of
        /// "vn vertexNormalX vertexNormalY vertexNormalZ".
        /// </summary>
        /// <param name="vertexNormal">
        /// A Vector3 representing a vertex normal (the normal pointing away 
        /// from the vertex that is calculated by the mesh when instantiated).
        /// </param>
        /// <returns>
        /// A readable string representing the vertex's entry in the mesh text file.
        /// </returns>
        public static string GetVertexNormalString(Vector3 vertexNormal)
        {
            string vertexNormalString = "";
            vertexNormalString += VertexNormalID;
            vertexNormalString += vertexNormal.x.ToString() + ' ';
            vertexNormalString += vertexNormal.y.ToString() + ' ';
            vertexNormalString += vertexNormal.z.ToString() + '\n';

            return vertexNormalString;
        }
        
        /// <summary>
        /// Logic to load a mesh from a text file representing a mesh and submeshes.
        /// All meshes are turned into standalone meshes that are then parented by a 
        /// newly instantiated GameObject. Names the parent object "RoomMesh".
        /// Assumes that all meshes are demarcated by the MeshID for the beginning line
        /// and terminated by the end of the last TriangleID line.
        /// </summary>
        /// <param name="meshSupplementaryInfoTextAsset"></param>
        /// <returns></returns>
        public static GameObject LoadMesh(TextAsset roomMeshTextAsset, TextAsset meshSupplementaryInfoTextAsset)
        {
            Vector3[] positionArray;
            Quaternion[] rotationArray;

            // Load up the information stored from mesh supplementary information 
            // to correct for Hololens mesh translation and orientation.
            LoadSupplementaryInfo(true, out positionArray, out rotationArray, meshSupplementaryInfoTextAsset);

            // ID the markers at the beginning of each line
            string meshID = MeshID.TrimEnd();
            string vertexID = VertexID.TrimEnd();
            string vertexNormalID = VertexNormalID.TrimEnd();
            string triangleID = TriangleID.TrimEnd();

            // Initialize items used while reading the file
            Queue<GameObject> meshObjects = new Queue<GameObject>();
            GameObject mesh = new GameObject();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();
            bool MeshRead = false;

            //string[] fileContents = File.ReadAllLines(OutputFilepath);
            string[] fileContents = SplitTextAsset(roomMeshTextAsset);
            int lineIndex = 0;
            int meshCount = 0;
            while(lineIndex < fileContents.Length)
            { 
                string line = fileContents[lineIndex].Trim();
                string[] lineContents = line.Split(' ');
                if (lineContents.Length == 0)
                {
                    // Ignore blank lines
                    continue;
                }

                // ID the marker telling you what info the line contains
                string marker = lineContents[0];
                
                // marker = "o"
                if (marker.Equals(meshID))
                {
                    // Demarcates a new mesh object -> create a new mesh to store info
                    GameObject.DestroyImmediate(mesh);
                    mesh = new GameObject();
                    mesh.name = lineContents[1];
                }
                // marker = "v"
                else if (marker.Equals(vertexID))
                {
                    // IDs a vertex to read in
                    Vector3 vertex = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
                    vertices.Add(vertex);
                }
                // marker = "vn"
                else if (marker.Equals(vertexNormalID))
                {
                    // IDs a vertex normal to read in
                    Vector3 normal = new Vector3(float.Parse(lineContents[1]), float.Parse(lineContents[2]), float.Parse(lineContents[3]));
                    normals.Add(normal);
                }
                // marker = "f"
                else if (marker.Equals(triangleID))
                {
                    // IDs a set of vertices that make up a triangle
                    do
                    {
                        triangles.Add(int.Parse(lineContents[1].Split('/')[0]));
                        triangles.Add(int.Parse(lineContents[2].Split('/')[0]));
                        triangles.Add(int.Parse(lineContents[3].Split('/')[0]));

                        // Reset variables
                        ++lineIndex;
                        if (lineIndex < fileContents.Length)
                        {
                            line = fileContents[lineIndex];
                            lineContents = line.Split(' ');
                            marker = lineContents[0];
                        }
                        else
                        {
                            marker = "";
                        }
                    } while (marker.Contains(triangleID));
                    --lineIndex;

                    MeshRead = true;
                }

                ++lineIndex;
                if (MeshRead)
                {
                    // If the triangle list has been fully read, that means you 
                    // have all the info you need to form the mesh.
                    if (positionArray != null)
                    {
                        mesh.transform.position = positionArray[meshCount];
                    }
                    if (rotationArray != null)
                    {
                        mesh.transform.rotation = rotationArray[meshCount];
                    }
                    
                    // Add neccessary components to the mesh-containing gameobject
                    mesh.AddComponent<MeshFilter>();
                    mesh.GetComponent<MeshFilter>().sharedMesh = new Mesh();
                    Mesh m = mesh.GetComponent<MeshFilter>().sharedMesh;
                    
                    // Set appropriate values
                    m.SetVertices(vertices);
                    vertices = new List<Vector3>();
                    m.SetNormals(normals);
                    normals = new List<Vector3>();
                    m.SetTriangles(triangles.ToArray(), 0);
                    triangles = new List<int>();
                    mesh.AddComponent<MeshRenderer>();
                    m.RecalculateBounds();
                    m.RecalculateNormals();

                    // Push them into the queue of meshes to be parented by the 
                    // final room mesh parent game object
                    meshObjects.Enqueue(mesh);
                    mesh = new GameObject();
                    MeshRead = false;
                    ++meshCount;
                }
            }

            // Assign all submeshes to a parent mesh object
            while(meshObjects.Count > 0)
            {
                // Mesh should be an empty new GameObject by this point
                meshObjects.Dequeue().transform.parent = mesh.transform;
            }
            mesh.name = "RoomMesh";

            return mesh;
        }

        /// <summary>
        /// Save information associated with a mesh that is required for 
        /// ensuring that the correction position and rotations are applied 
        /// for each mesh generated. Writes all information to the 
        /// SupplementaryOutputFilePath.
        /// 
        /// NOTE: Loading from anything other than a TextAsset is not currently 
        /// supported by the system.
        /// </summary>
        /// <param name="positionArray">
        /// An array of Vector3's that represent the positions of the meshes 
        /// that will be instantiated.
        /// </param>
        /// <param name="rotationArray">
        /// An array of Quaternions that represent the rotations of the meshes
        /// that will be instantiated. (A quaternion uses the first three items 
        /// to formulate the axis of rotation, and the final item to determine 
        /// the degree of rotation around that axis.)
        /// </param>
        public static void SaveSupplementaryInfo(Vector3[] positionArray, Quaternion[] rotationArray)
        {
            string fileContents = "";

            // Save the positions of instantiated meshes
            fileContents += SupplementaryInfoSeparator;
            fileContents += '\n';
            fileContents += PositionID;
            fileContents += '\n';
            for (int i = 0; i < positionArray.Length; i++)
            {
                fileContents += SupplementaryInfoSeparator;
                fileContents += '\n';
                Vector3 pos = positionArray[i];

                fileContents += Vector3ToString(pos);
            }
            // Save the rotations of instantiated meshes
            fileContents += SupplementaryInfoSeparator;
            fileContents += '\n';
            fileContents += RotationID;
            fileContents += '\n';
            for (int i = 0; i < rotationArray.Length; i++)
            {
                fileContents += SupplementaryInfoSeparator;
                fileContents += '\n';
                Quaternion rot = rotationArray[i];

                fileContents += QuaternionToString(rot);
            }

            // Actually write the calculated string
            File.WriteAllText(CrossPlatformNames.Mesh.SupplementaryOutputFilepath, fileContents);
        }

        /// <summary>
        /// Load information associated with a mesh that is required for
        /// ensuring that the correct position and rotations are applied 
        /// for each mesh generated from the text file.
        /// </summary>
        /// <param name="fromAsset">
        /// Boolean determining if the passed in TextAsset file will be used.
        /// </param>
        /// <param name="PositionArray">
        /// The array of Vector3's representing positions of meshes.
        /// </param>
        /// <param name="RotationArray">
        /// The array of Quaternions representing rotations of meshes.
        /// </param>
        /// <param name="asset">
        /// The optional loaded Unity TextAsset to load supplementary information from.
        /// </param>
        public static void LoadSupplementaryInfo(bool fromAsset, out Vector3[] PositionArray, out Quaternion[] RotationArray, TextAsset asset)
        {
            // Load from a passed-in TextAsset
            if (fromAsset)
            {
                if (asset != null)
                {
                    DeriveSupplementaryStuff(SplitTextAsset(asset), out PositionArray, out RotationArray);
                }
                else
                {
                    PositionArray = null;
                    RotationArray = null;
                }
            }
            // Else load from a preset location
            else
            {
                // ERROR TESTING - Make it so that you can load directly from a file as well.
                PositionArray = null;
                RotationArray = null;
            }
        }
        
        #endregion

        #region Helper Functions

        /// <summary>
        /// Create a readable string representing the components of a Vector3. 
        /// Takes the format of "vectorX vectorY vectorZ" in floats.
        /// </summary>
        /// <param name="vec">
        /// The vector to create a readable string from.
        /// </param>
        /// <returns>
        /// A readable string representing the components of a Vector3.
        /// </returns>
        private static string Vector3ToString(Vector3 vec)
        {
            string vecString = vec.x.ToString() + ' ' + vec.y.ToString() + ' ' + vec.z.ToString() + '\n';

            return vecString;
        }

        /// <summary>
        /// Create a readable string representing the components of a Quaternion.
        /// Takes the format of "xAxis yAxis zAxis rotationDegrees".
        /// </summary>
        /// <param name="q">
        /// The quaternion to create a readable string from.
        /// </param>
        /// <returns>
        /// A readable string representing the components of a Quaternion.
        /// </returns>
        private static string QuaternionToString(Quaternion q)
        {
            string qString = q.x.ToString() + ' ' + q.y.ToString() + ' ' + q.z.ToString() + ' ' + q.w.ToString() + '\n';

            return qString;
        }

        /// <summary>
        /// Splits the contents of a TextAsset by the newline character.
        /// </summary>
        /// <param name="textAsset">
        /// The TextAsset to be split.
        /// </param>
        /// <returns>
        /// An array of strings (i.e. the lines) of the text asset.
        /// </returns>
        private static string[] SplitTextAsset(TextAsset textAsset)
        {
            return textAsset.text.Split('\n');
        }

        /// <summary>
        /// Process the logic of reading the lines of the saved text file 
        /// holding the supplementary information for the room mesh. This 
        /// includes extracting the positions of the meshes and rotations 
        /// of the meshes.
        /// </summary>
        /// <param name="fileLines">
        /// The strings representing the text file holding the supplementary info.
        /// </param>
        /// <param name="positionArray">
        /// The uninitialized array to hold the positions of the matrices.
        /// </param>
        /// <param name="rotationArray">
        /// The uninitialized array to hold the rotations of the matrices.
        /// </param>
        private static void DeriveSupplementaryStuff(string[] fileLines, out Vector3[] positionArray, out Quaternion[] rotationArray)
        {
            Queue<Vector3> posList = new Queue<Vector3>();
            Queue<Quaternion> rotList = new Queue<Quaternion>();

            bool usePosList = false;
            bool useRotList = false;

            int lineCount = 0;
            while (lineCount < fileLines.Length)
            {
                fileLines[lineCount] = fileLines[lineCount].TrimEnd();

                if (fileLines[lineCount].Contains(SupplementaryInfoSeparator))
                {
                    ++lineCount;

                    if (fileLines[lineCount].Contains(PositionID))
                    {
                        ++lineCount;
                        usePosList = true;
                        useRotList = false;
                    }
                    else if (fileLines[lineCount].Contains(RotationID))
                    {
                        ++lineCount;
                        usePosList = false;
                        useRotList = true;
                    }
                    else
                    {
                        // update position list
                        if (usePosList)
                        {
                            Vector3 pos = new Vector3();
                            string[] lineContents = fileLines[lineCount].Split(' ');

                            for (int i = 0; i < lineContents.Length; i++)
                            {
                                lineContents[i] = lineContents[i].TrimEnd();
                            }

                            if (!float.TryParse(lineContents[0], out pos.x))
                                pos.x = 0;
                            if (!float.TryParse(lineContents[1], out pos.y))
                                pos.y = 0;
                            if (!float.TryParse(lineContents[2], out pos.z))
                                pos.z = 0;

                            posList.Enqueue(pos);
                        }
                        // update rotation list
                        else //if (useRotList)
                        {
                            Quaternion rot = new Quaternion();
                            string[] lineContents = fileLines[lineCount].Split(' ');

                            for (int i = 0; i < lineContents.Length; i++)
                            {
                                lineContents[i] = lineContents[i].TrimEnd();
                            }

                            if (!float.TryParse(lineContents[0], out rot.x))
                                rot.x = 0;
                            if (!float.TryParse(lineContents[1], out rot.y))
                                rot.y = 0;
                            if (!float.TryParse(lineContents[2], out rot.z))
                                rot.z = 0;
                            if (!float.TryParse(lineContents[3], out rot.w))
                                rot.w = 0;

                            rotList.Enqueue(rot);
                        }
                    }
                }
                else
                {
                    ++lineCount;
                }
            }

            // Make the stuff
            positionArray = posList.ToArray();
            rotationArray = rotList.ToArray();
        }
        #endregion
        #endregion
    }
}