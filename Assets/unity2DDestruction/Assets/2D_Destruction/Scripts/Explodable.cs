using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class Explodable : MonoBehaviour, IResettable
{
    public System.Action<List<GameObject>> OnFragmentsGenerated;

    public int extraPoints = 0;
    public int subshatterSteps = 0;

    public string fragmentLayer = "Default";
    public string sortingLayerName = "Default";
    public int orderInLayer = 0;
    private bool hasExploded = false;
    public enum ShatterType
    {
        Triangle,
        Voronoi
    };
    public ShatterType shatterType;
    public List<GameObject> fragments = new List<GameObject>();
    private List<List<Vector2>> polygons = new List<List<Vector2>>();
   
    /// <summary>
    /// Creates fragments if necessary and destroys original gameobject
    /// </summary>
    public void explode()
    {
        hasExploded = true;
        //if fragments were not created before runtime then create them now
        if (fragments.Count == 0)
        {
            generateFragments();
        }
        //otherwise unparent and activate them
        else
        {
            foreach (GameObject frag in fragments)
            {
                frag.transform.parent = null;
                frag.SetActive(true);
                foreach (ExplodableAddon addon in GetComponents<ExplodableAddon>())
                {
                    if (addon.enabled)
                    {
                        print("addod spotted !!!!!");
                        addon.OnFragmentsExploded(fragments);
                    }
                }
            }
        }
        //if fragments exist destroy the original
        if (fragments.Count > 0)
        {
            print("ORIGINAL COLLIDER TURN OFF");
            GetComponent<Collider2D>().enabled = false;
            
            // make invis
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0f;
                sr.color = c;
            }

        }

    }
    /// <summary>
    /// Creates fragments and then disables them
    /// </summary>
    public void fragmentInEditor()
    {
        if (fragments.Count > 0)
        {
            deleteFragments();
        }
        generateFragments();
        setPolygonsForDrawing();
        foreach (GameObject frag in fragments)
        {
            frag.transform.parent = transform;
            frag.SetActive(false);
        }
    }
    public void deleteFragments()
    {
        bool calledFromUI = !Application.isPlaying;

        foreach (GameObject frag in fragments)
        {
            if (calledFromUI)
                DestroyImmediate(frag);
            else
                Destroy(frag);
        }

        fragments.Clear();
        polygons.Clear();
    }
    /// <summary>
    /// Turns Gameobject into multiple fragments
    /// </summary>
    private void generateFragments()
    {
        fragments = new List<GameObject>();
        switch (shatterType)
        {
            case ShatterType.Triangle:
                fragments = SpriteExploder.GenerateTriangularPieces(gameObject, extraPoints, subshatterSteps);
                break;
            case ShatterType.Voronoi:
                fragments = SpriteExploder.GenerateVoronoiPieces(gameObject, extraPoints, subshatterSteps);
                break;
            default:
                Debug.Log("invalid choice");
                break;
        }
        //sets additional aspects of the fragments
        foreach (GameObject p in fragments)
        {
            if (p != null)
            {
                p.layer = LayerMask.NameToLayer(fragmentLayer);
                p.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
                p.GetComponent<Renderer>().sortingOrder = orderInLayer;
                p.transform.SetParent(transform);
                p.SetActive(false);
            }
        }

        foreach (ExplodableAddon addon in GetComponents<ExplodableAddon>())
        {
            if (addon.enabled)
            {
                print("addod spotted !!!!!");
                addon.OnFragmentsGenerated(fragments);
            }
        }
    }
    private void setPolygonsForDrawing()
    {
        polygons.Clear();
        List<Vector2> polygon;

        foreach (GameObject frag in fragments)
        {
            polygon = new List<Vector2>();
            foreach (Vector2 point in frag.GetComponent<PolygonCollider2D>().points)
            {
                Vector2 offset = rotateAroundPivot((Vector2)frag.transform.position, (Vector2)transform.position, Quaternion.Inverse(transform.rotation)) - (Vector2)transform.position;
                offset.x /= transform.localScale.x;
                offset.y /= transform.localScale.y;
                polygon.Add(point + offset);
            }
            polygons.Add(polygon);
        }
    }
    private Vector2 rotateAroundPivot(Vector2 point, Vector2 pivot, Quaternion angle)
    {
        Vector2 dir = point - pivot;
        dir = angle * dir;
        point = dir + pivot;
        return point;
    }

    void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (polygons.Count == 0 && fragments.Count != 0)
            {
                setPolygonsForDrawing();
            }

            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector2 offset = (Vector2)transform.position * 0;
            foreach (List<Vector2> polygon in polygons)
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    if (i + 1 == polygon.Count)
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[0] + offset);
                    }
                    else
                    {
                        Gizmos.DrawLine(polygon[i] + offset, polygon[i + 1] + offset);
                    }
                }
            }
        }
    }

    public void ResetToInitialState()
    {
        GetComponent<Collider2D>().enabled = true;
        // make colorful again
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        if (hasExploded)
        {
            deleteFragments();
            generateFragments();
        }
        hasExploded = false;
    }

}
