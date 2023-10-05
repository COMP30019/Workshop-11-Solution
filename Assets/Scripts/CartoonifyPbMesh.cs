// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2023

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class CartoonifyPbMesh : MonoBehaviour
{
    [SerializeField] private ProBuilderMesh pbMesh;

    private void Start()
    {
        Cartoonify();
    }

    public void Cartoonify()
    {
        var faces = pbMesh.faces;
        var edges = pbMesh.faces.SelectMany(face => face.edges).Distinct().ToList();

        // Bevel all edges so that each face is separated from its neighbours.
        float bevelAmount = Random.Range(0.10f, 0.15f);
        Bevel.BevelEdges(pbMesh, edges, bevelAmount);

        // Newly created faces are the bevel faces.
        var bevelFaces = pbMesh.faces.Except(faces).ToList();

        // Randomly rotate and scale faces relative to their normals.
        foreach (Face face in faces)
        {
            var faceVertices = pbMesh.GetVertices(face.distinctIndexes);
            var facePivot = Math.Average(faceVertices.Select(vertex => vertex.position).ToList());
            var faceOrientation = Math.NormalTangentBitangent(pbMesh, face);

            var rotation = Quaternion.AngleAxis(Random.Range(-8.0f, 8.0f), faceOrientation.normal);

            // Scale around normal
            var scale = Vector3.one +
                        (Vector3)faceOrientation.tangent * Random.Range(-0.1f, 0.1f) +
                        (Vector3)faceOrientation.bitangent * Random.Range(-0.1f, 0.1f);
            
            foreach (var index in face.distinctIndexes)
            {
                var vertexPosition = pbMesh.positions[index];
                var scaled = ScaleAround(vertexPosition, facePivot, scale);
                var rotated = RotateAround(scaled, facePivot, rotation);
                pbMesh.SetSharedVertexPosition(index, rotated);
            }
        }
        
        // Merge bevel-linked vertices that are near each other (post
        // randomisation) in order to minimise the risk of artefacts due to
        // overlapping faces.
        VertexEditing.WeldVertices(pbMesh, 
            bevelFaces.SelectMany(face => face.distinctIndexes).ToArray(), 0.2f);

        // Refresh mesh to apply the changes
        pbMesh.Refresh();
        pbMesh.ToMesh();
    }

    private Vector3 RotateAround(Vector3 position, Vector3 pivotPoint, Quaternion rotation)
    {
        return rotation * (position - pivotPoint) + pivotPoint;
    }

    private Vector3 ScaleAround(Vector3 position, Vector3 pivotPoint, Vector3 scale)
    {
        return Vector3.Scale(position - pivotPoint, scale) + pivotPoint;
    }
}
