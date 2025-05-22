using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
public class AutoLevel : MonoBehaviour
{
    [Serializable]
    public struct Line
    {
        public Vector3 start;
        public Vector3 end;
        public int from;
        public int to;

    }

    [Serializable]
    public struct LevelBuilderInfo{
        public int uid;
        public object info;
        public bool flip;
    }

    public AutoLevelPrefabPack prefabs;
    public ProBuilderMesh mesh;

    List<Line> outsideEdges;

    List<Vertex> vertices;//list with all the unique vertices

    [HideInInspector]
    public List<Color> colors;//list with all the colors of the vertices

    public List<AutoLevelAnchor> anchors;
    public List<LevelBuilderInfo> info;
    
    public void UpdateMeshInfo(){
         //we want to get a list with all the vertices that are not shared so "unique positions"
        vertices = mesh.GetVertices().ToList();//get all vertices as a list
        colors = mesh.GetColors().ToList();//get all colors as a list
        outsideEdges = FindOutsideEdges();
    }

    [ContextMenu("Clear Level")]
    public void DeleteAllFeatures(){
        //find all children that are not named anchor
        var children = transform.Cast<Transform>().Where(c => !c.name.Contains("Anchor")).ToList();
        while(children.Count > 0)
        {
            DestroyImmediate(children[children.Count-1].gameObject);
            children.RemoveAt(children.Count-1);
        }
    }

    public void GenerateLevel()
    {
        UpdateMeshInfo();
        DeleteAllFeatures();

        var sharedVertices = mesh.sharedVertices.ToArray();

        //create "Point" features. this can be a corner, door, window, etc
        foreach (var shared in sharedVertices)
        {
            //lookup the index of this vertex
            var index = shared[0];
            var vertex = vertices[index];
            Vector3 worldPosition = transform.TransformPoint(vertex.position);

            //check if this vertex is an anchor and update the game object Position
            var anchorID = colors[index].b;
            if(anchorID > 9)
            {
                var anchor = anchors.Where(a => a.ID == anchorID).FirstOrDefault();
                if(anchor != null && anchor.gameObject != null)
                {
                    var rotated = transform.rotation * anchor.offsetPosition;
                    anchor.gameObject.transform.position = worldPosition-rotated;
                    anchor.gameObject.transform.parent = transform;
                    if(!anchor.gameObject.name.Contains("Anchor")){
                        anchor.gameObject.name = "Anchor " + anchor.gameObject.name;
                    }
                }
            }
            
            //check the color to see if we should ignore generating this point
            if(colors[index].a != 0)
            {
                var newObject = new GameObject("Point " + vertices.IndexOf(vertex).ToString());
                newObject.AddComponent<AutoLevelPoint>().Setup(worldPosition, index, this);
                newObject.transform.rotation  = transform.rotation;
                newObject.transform.SetParent(transform);
            }
        }
        
        //create "line" features this can be a wall, a fence, whatever that is a line
        foreach (Line edge in outsideEdges)
        {
            Vector3 worldPosition1 = transform.TransformPoint(edge.start);
            Vector3 worldPosition2 = transform.TransformPoint(edge.end);

            //check the color of both vertices to see if we should ignore generating this line
            if(colors[edge.from].a == 0 && colors[edge.to].a == 0)
            {
                continue;
            }
            
            var newObject = new GameObject( "Line " + outsideEdges.IndexOf(edge).ToString());
            newObject.AddComponent<AutoLevelLine>().setup(worldPosition1, worldPosition2, edge.from, edge.to, this);
            newObject.transform.SetParent(transform);
        }
    }

     List<Line> FindOutsideEdges()
    {
        //create a list where we will store the outside edges
        List<Line> outsideEdges = new List<Line>();

        //get the faces of the mesh
        var faces = mesh.faces;

        for(var i = 0; i < faces.Count; i++)
        {
            var edges = faces[i].edges;
            foreach(Edge edge in edges)
            {
                var newLine = new Line{
                    start = vertices[edge.a].position,
                    end = vertices[edge.b].position
                };
                var newLineInverded = new Line{
                    start = vertices[edge.b].position,
                    end = vertices[edge.a].position
                };

                var lineExists = outsideEdges.Where(l => l.start == newLine.start && l.end == newLine.end).ToList();
                var lineInvertedExists = outsideEdges.Where(l => l.start == newLineInverded.start && l.end == newLineInverded.end).ToList();

                if(lineExists.Count > 0 ){
                    foreach(var l in lineExists)
                    {
                        outsideEdges.Remove(l);
                    } 
                }

                else if(lineInvertedExists.Count > 0 ){
                    foreach(var l in lineInvertedExists)
                    {
                        outsideEdges.Remove(l);
                    } 
                }
                else
                {
                    newLine.from  = edge.a;
                    newLine.to = edge.b;
                    outsideEdges.Add(newLine);
                }
            }
        }
        return outsideEdges;
    }

    public void SaveLineType(int from, int lineType)
    {
        var c = colors[from];
        c.r = lineType;
        colors[from] =c;
        mesh.colors = colors.ToArray();
    }


    public void SaveLevelInfoId(int vertexIndex, int id){
        var v = colors[vertexIndex];
        v.b = id;
        colors[vertexIndex] =v;
        mesh.colors = colors.ToArray();
    }

    public void SavePointType(int vertexIndex, int pointType)
    {
        var c = colors[vertexIndex];
        c.g = pointType;
        colors[vertexIndex] =c;
        mesh.colors = colors.ToArray();
    }

    //this is a helper function to reset the values of the mesh
    public void ResetMeshValues(){
        //reset the colors of the mesh to (1,1,1,1)
        var newcolors = new Color[mesh.vertexCount];
        for(int i = 0; i < mesh.vertexCount; i++)
        {
            newcolors[i] = Color.white;
        }
        mesh.colors = newcolors;
        info = new List<LevelBuilderInfo>();
    }

    public void WriteAlphaToSelected(int new_alpha){
        //write new Alpha information to the vertices
        var sharedVertices = mesh.sharedVertices;
        var selectedVertices = mesh.selectedVertices;
        foreach(var v in selectedVertices)
        {
            //find all the vertices shared with this one: 
            foreach(var shared in sharedVertices)
            {
                if(shared.Contains(v))
                {
                    foreach(var s in shared)
                    {
                        var c = colors[s];
                        c.a = new_alpha;
                        colors[s] = c;
                    }
                }
            }
        }
        //ALWAYS REMEMBER TO WRITE BACK THE COLORS TO THE MESH
        mesh.colors = colors.ToArray();
    }

    public void AddAnchor()
    {   
        var selectedVertices = mesh.selectedVertices;
        if(selectedVertices.Count == 0)
        {
            Debug.LogWarning("No vertices selected");
            return;
        }
        if(selectedVertices.Count > 1)
        {
            Debug.LogWarning("Select only one vertex");
            return;
        }

        var newAnchor = new AutoLevelAnchor
        {
            vertex = selectedVertices[0],
            ID = anchors.Count + 10
        };
        anchors.Add(newAnchor);

        //save the anchor ID to the mesh: 
        var sharedVertices = mesh.sharedVertices;
        foreach(var shared in sharedVertices)
        {
                if(shared.Contains(selectedVertices[0]))
                {
                    foreach(var s in shared)
                    {
                        var c = colors[s];
                        c.b = newAnchor.ID;
                        colors[s] = c;
                    }
                }
        }
        //ALWAYS REMEMBER TO WRITE BACK THE COLORS TO THE MESH
        mesh.colors = colors.ToArray();
    }

    public void RemoveAnchor(){
        var selectedVertices = mesh.selectedVertices;
        if(selectedVertices.Count == 0)
        {
            Debug.LogWarning("No vertices selected");
            return;
        }
        if(selectedVertices.Count > 1)
        {
            Debug.LogWarning("Select only one vertex");
            return;
        }

        var vertex = selectedVertices[0];
        var anchorID = colors[vertex].b;
        if(anchorID < 10)
        {
            Debug.LogWarning("This vertex is not an anchor");
            return;
        }

        var anchor = anchors.Where(a => a.ID == anchorID).FirstOrDefault();
        if(anchor != null)
        {
            anchors.Remove(anchor);
        }

        //save the anchor ID to the mesh: 
        var sharedVertices = mesh.sharedVertices;
        foreach(var shared in sharedVertices)
        {
                if(shared.Contains(vertex))
                {
                    foreach(var s in shared)
                    {
                        var c = colors[s];
                        c.b = 0;
                        colors[s] = c;
                    }
                }
        }
        //ALWAYS REMEMBER TO WRITE BACK THE COLORS TO THE MESH
        mesh.colors = colors.ToArray();
    }

    public void updateAnchorOffsets()
    {
        foreach(var anchor in anchors)
        {
            var vertexWorldPosition = transform.TransformPoint(vertices[anchor.vertex].position);
            anchor.offsetPosition = vertexWorldPosition -anchor.gameObject.transform.position;
        }
    }

    void OnDrawGizmos()
    {
        if(vertices == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
        
        var sharedVertices = mesh.sharedVertices.ToArray();
        for(var i = 0; i < sharedVertices.Length; i++)
        {
            try{
                var v = vertices[sharedVertices[i][0]];
                Vector3 worldPosition = transform.TransformPoint(v.position);
                Gizmos.color = Color.green;

                //is this vertex ignored?
                if(colors[mesh.sharedVertices[i][0]].a == 0)
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireSphere(worldPosition, 0.5f);

                var anchorId = colors[mesh.sharedVertices[i][0]].b;
                //is this vertex an anchor?
                if(anchorId> 9)
                {
                    Gizmos.color = Color.yellow;
                    var anchor = anchors.Where(a => a.ID == anchorId).FirstOrDefault();
                    if(anchor!=null){
                        var start = worldPosition;

                        var rotated = transform.rotation * anchor.offsetPosition;
                        
                        var end = worldPosition - rotated;
                        
                        Gizmos.DrawSphere(start, 0.3f);
                        Gizmos.DrawLine(start, end);
                        Gizmos.DrawSphere(end, 0.3f);
                    }
                    Gizmos.DrawWireSphere(worldPosition+Vector3.up, 0.6f);
                    #if UNITY_EDITOR
                    Handles.Label(worldPosition, "Anchor " + colors[mesh.sharedVertices[i][0]].b.ToString());
                    #endif


                }
            }
            catch (Exception e)
            {
                continue;
            }
        }

        if(outsideEdges != null)
        {
            foreach(Line edge in outsideEdges)
            {
                Vector3 worldPosition1 = transform.TransformPoint(edge.start);
                Vector3 worldPosition2 = transform.TransformPoint(edge.end);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(worldPosition1, worldPosition2);
            }
        }
    }

}
