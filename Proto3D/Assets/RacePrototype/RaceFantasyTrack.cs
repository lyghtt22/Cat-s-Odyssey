using System.Collections.Generic;
using UnityEngine;

// Original runtime geometry. No scene, imported model, or third-party material is edited.
public class RaceFantasyTrack : MonoBehaviour
{
    public Vector3[] Route { get; private set; }
    public Vector3[] Gates { get; private set; }

    const int PointCount = 96;
    const float RoadHalfWidth = 14.5f;
    const float Height = 12f;
    const float CenterZ = 1200f;
    GameObject trackRoot;
    readonly List<Material> materials = new List<Material>();
    readonly List<Mesh> meshes = new List<Mesh>();

    public void Build()
    {
        if (trackRoot) return;
        Route = new Vector3[PointCount];
        Gates = new Vector3[8];
        for (int i = 0; i < PointCount; i++)
        {
            float a = -Mathf.PI * .5f + i * Mathf.PI * 2f / PointCount;
            Route[i] = new Vector3(230f * Mathf.Cos(a) + 10f * Mathf.Sin(2f * a), Height,
                CenterZ + 175f * Mathf.Sin(a) + 8f * Mathf.Sin(3f * a));
        }
        for (int i = 0; i < Gates.Length; i++) Gates[i] = Route[i * PointCount / Gates.Length];

        trackRoot = new GameObject("Circuito 2 - Islas de malvavisco");
        trackRoot.transform.SetParent(transform, false);
        trackRoot.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Material road = MakeMaterial("Azucar de arandano", new Color(.27f, .25f, .48f), .2f);
        Material cream = MakeMaterial("Glaseado de vainilla", new Color(1f, .92f, .72f), .32f);
        Material mint = MakeMaterial("Menta celeste", new Color(.44f, .88f, .87f), .38f);
        Material pink = MakeMaterial("Malvavisco de frambuesa", new Color(1f, .55f, .75f), .28f);
        Material lavender = MakeMaterial("Nube de lavanda", new Color(.71f, .65f, .92f), .25f);
        Material cocoa = MakeMaterial("Chocolate blanco tostado", new Color(.62f, .38f, .29f), .2f);
        Material white = MakeMaterial("Nube de nata", new Color(.96f, .96f, 1f), .18f);

        CreateRibbon("Carretera continua", -RoadHalfWidth, RoadHalfWidth, 0f, road, true);
        CreateRibbon("Borde de vainilla interior", -RoadHalfWidth, -RoadHalfWidth + 1.3f, .025f, cream, false);
        CreateRibbon("Borde de menta exterior", RoadHalfWidth - 1.3f, RoadHalfWidth, .025f, mint, false);
        CreateRibbon("Base flotante", -RoadHalfWidth - .6f, RoadHalfWidth + .6f, -.65f, lavender, false);
        CreateRail("Barrera interior", -RoadHalfWidth, cream);
        CreateRail("Barrera exterior", RoadHalfWidth, mint);
        CreateShortcut(cream);

        for (int i = 0; i < PointCount; i += 3)
        {
            Vector3 tangent = Tangent(i);
            var dash = Shape("Marca de azucar", PrimitiveType.Cube, Route[i] + Vector3.up * .045f,
                new Vector3(.35f, .035f, 3.6f), cream);
            dash.transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
        }

        // Sparse decorations have no physics colliders; only road, rails, and shortcut have colliders.
        for (int i = 0; i < 16; i++)
        {
            int k = i * PointCount / 16;
            Vector3 side = Vector3.Cross(Vector3.up, Tangent(k));
            Vector3 p = Route[k] + side * (i % 2 == 0 ? 30f : -31f);
            Material icing = i % 3 == 0 ? pink : i % 3 == 1 ? mint : lavender;
            Shape("Isla de galleta", PrimitiveType.Cylinder, p + Vector3.down * 4.5f,
                new Vector3(22f, 2.2f, 18f), cocoa);
            Shape("Glaseado de isla", PrimitiveType.Cylinder, p + Vector3.down * 2.25f,
                new Vector3(22.5f, .25f, 18.5f), icing);
            Shape("Malvavisco", PrimitiveType.Capsule, p + Vector3.up * 2.5f,
                new Vector3(6f, 3.5f, 6f), white);
            Shape("Corazon de caramelo", PrimitiveType.Sphere, p + Vector3.up * 7f,
                new Vector3(5f, 5f, 5f), icing);
        }

        for (int i = 0; i < Gates.Length; i++)
        {
            int k = i * PointCount / Gates.Length;
            Vector3 side = Vector3.Cross(Vector3.up, Tangent(k));
            for (int s = -1; s <= 1; s += 2)
            {
                Vector3 p = Gates[i] + side * s * (RoadHalfWidth + 1.6f);
                Shape("Baston de checkpoint", PrimitiveType.Cylinder, p + Vector3.up * 3f,
                    new Vector3(.7f, 3f, .7f), cream);
                Shape("Caramelo de checkpoint", PrimitiveType.Sphere, p + Vector3.up * 6.4f,
                    Vector3.one * 2f, i == 0 ? pink : mint);
            }
        }

        Vector3 forward = Tangent(0);
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        for (int i = 0; i < 12; i++)
        {
            Vector3 p = Gates[0] + right * (-13.2f + i * 2.4f) + Vector3.up * .06f;
            var tile = Shape("Meta de nata", PrimitiveType.Cube, p,
                new Vector3(2.4f, .05f, 1.1f), i % 2 == 0 ? white : cocoa);
            tile.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
        var sign = new GameObject("ISLAS DE MALVAVISCO");
        sign.transform.SetParent(trackRoot.transform, false);
        sign.transform.position = Gates[0] + Vector3.up * 9f;
        sign.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        var text = sign.AddComponent<TextMesh>();
        text.text = "ISLAS DE MALVAVISCO";
        text.fontSize = 64;
        text.characterSize = .18f;
        text.anchor = TextAnchor.MiddleCenter;
        text.color = new Color(1f, .94f, .83f);

        // A distinct floating fantasy backdrop, well outside the driving corridor.
        for (int i = 0; i < 9; i++)
        {
            float a = i * Mathf.PI * 2f / 9;
            Vector3 p = new Vector3(Mathf.Cos(a) * 325f, 19f + i % 3 * 9f,
                CenterZ + Mathf.Sin(a) * 270f);
            Shape("Nube de azucar", PrimitiveType.Sphere, p, new Vector3(48f, 14f, 26f),
                i % 2 == 0 ? lavender : white);
        }
    }

    public void SetVisible(bool visible)
    {
        if (visible && !trackRoot) Build();
        if (trackRoot) trackRoot.SetActive(visible);
    }

    Vector3 Tangent(int i)
    {
        return (Route[(i + 1) % PointCount] - Route[(i + PointCount - 1) % PointCount]).normalized;
    }

    Material MakeMaterial(string name, Color color, float smoothness)
    {
        var material = new Material(Shader.Find("Standard"));
        material.name = name;
        material.color = color;
        material.SetFloat("_Glossiness", smoothness);
        materials.Add(material);
        return material;
    }

    GameObject Shape(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(trackRoot.transform, false);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        var collider = obj.GetComponent<Collider>();
        if (collider) { collider.enabled = false; Release(collider); }
        return obj;
    }

    void CreateRibbon(string name, float left, float right, float rise, Material material, bool collision)
    {
        var vertices = new Vector3[PointCount * 2];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[PointCount * 6];
        float distance = 0f;
        for (int i = 0; i < PointCount; i++)
        {
            Vector3 side = Vector3.Cross(Vector3.up, Tangent(i));
            vertices[i * 2] = trackRoot.transform.InverseTransformPoint(Route[i] + side * left + Vector3.up * rise);
            vertices[i * 2 + 1] = trackRoot.transform.InverseTransformPoint(Route[i] + side * right + Vector3.up * rise);
            uv[i * 2] = new Vector2(0f, distance / 8f);
            uv[i * 2 + 1] = new Vector2(1f, distance / 8f);
            distance += Vector3.Distance(Route[i], Route[(i + 1) % PointCount]);
            int a = i * 2, b = a + 1, c = (i + 1) % PointCount * 2, d = c + 1, t = i * 6;
            triangles[t] = a; triangles[t + 1] = c; triangles[t + 2] = b;
            triangles[t + 3] = b; triangles[t + 4] = c; triangles[t + 5] = d;
        }
        CreateMesh(name, vertices, triangles, uv, material, collision);
    }

    void CreateRail(string name, float offset, Material material)
    {
        var vertices = new Vector3[PointCount * 4];
        // The golden chord joins points 13 and 23, between checkpoints 12 and 24.
        // Open only its ten adjacent inside rail segments; the outside rail stays continuous.
        var triangles = new int[(PointCount - (offset < 0f ? 10 : 0)) * 24];
        int triangleIndex = 0;
        for (int i = 0; i < PointCount; i++)
        {
            Vector3 side = Vector3.Cross(Vector3.up, Tangent(i));
            for (int corner = 0; corner < 4; corner++)
            {
                float sideOffset = offset + (corner == 0 || corner == 3 ? -.3f : .3f);
                float rise = corner < 2 ? .08f : 1.15f;
                vertices[i * 4 + corner] = trackRoot.transform.InverseTransformPoint(
                    Route[i] + side * sideOffset + Vector3.up * rise);
            }
            if (offset < 0f && i >= 13 && i < 23) continue;
            for (int face = 0; face < 4; face++)
            {
                int a = i * 4 + face, b = i * 4 + (face + 1) % 4;
                int c = (i + 1) % PointCount * 4 + face;
                int d = (i + 1) % PointCount * 4 + (face + 1) % 4;
                int t = triangleIndex;
                triangleIndex += 6;
                triangles[t] = a; triangles[t + 1] = b; triangles[t + 2] = c;
                triangles[t + 3] = b; triangles[t + 4] = d; triangles[t + 5] = c;
            }
        }
        CreateMesh(name, vertices, triangles, null, material, true);
    }

    void CreateShortcut(Material cream)
    {
        Material gold = MakeMaterial("Caramelo dorado del atajo", new Color(1f, .7f, .18f), .32f);
        Vector3 entry = Route[13], exit = Route[23];
        Vector3 forward = (exit - entry).normalized;
        Vector3 side = Vector3.Cross(Vector3.up, forward);
        Vector3 rise = Vector3.up * .075f;
        var vertices = new[] {
            trackRoot.transform.InverseTransformPoint(entry - side * 3f + rise),
            trackRoot.transform.InverseTransformPoint(entry + side * 3f + rise),
            trackRoot.transform.InverseTransformPoint(exit - side * 3f + rise),
            trackRoot.transform.InverseTransformPoint(exit + side * 3f + rise)
        };
        CreateMesh("Puente real - atajo de caramelo", vertices, new[] { 0, 2, 1, 1, 2, 3 },
            new[] { Vector2.zero, Vector2.right, Vector2.up * 16f, new Vector2(1f, 16f) }, gold, true);
        for (int i = 1; i <= 7; i++)
        {
            Vector3 p = Vector3.Lerp(entry, exit, i / 8f) + Vector3.up * .11f;
            for (int s = -1; s <= 1; s += 2)
            {
                var arrow = Shape("Flecha del atajo", PrimitiveType.Cube, p + side * s * .7f,
                    new Vector3(.28f, .035f, 2f), cream);
                arrow.transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(0f, s * 45f, 0f);
            }
        }
        var sign = new GameObject("ATAJO - puente de caramelo");
        sign.transform.SetParent(trackRoot.transform, false);
        sign.transform.position = entry - Tangent(13) * 5f + Vector3.up * 4.5f;
        sign.transform.rotation = Quaternion.LookRotation(Tangent(13), Vector3.up);
        var text = sign.AddComponent<TextMesh>();
        text.text = "ATAJO >>>";
        text.fontSize = 64;
        text.characterSize = .14f;
        text.anchor = TextAnchor.MiddleCenter;
        text.color = new Color(1f, .83f, .25f);
    }

    void CreateMesh(string name, Vector3[] vertices, int[] triangles, Vector2[] uv, Material material, bool collision)
    {
        var mesh = new Mesh();
        mesh.name = name;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        if (uv != null) mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshes.Add(mesh);
        var obj = new GameObject(name);
        obj.transform.SetParent(trackRoot.transform, false);
        obj.AddComponent<MeshFilter>().sharedMesh = mesh;
        obj.AddComponent<MeshRenderer>().sharedMaterial = material;
        if (collision) obj.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    void OnDestroy()
    {
        if (trackRoot) Release(trackRoot);
        foreach (var mesh in meshes) if (mesh) Release(mesh);
        foreach (var material in materials) if (material) Release(material);
        meshes.Clear();
        materials.Clear();
    }

    static void Release(Object obj)
    {
        if (Application.isPlaying) Destroy(obj); else DestroyImmediate(obj);
    }
}
