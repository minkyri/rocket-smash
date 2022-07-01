using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
public static class Extensions
{
    public static Vector2 ComputeTotalImpulse(Collision2D collision)
    {
        Vector2 impulse = Vector2.zero;

        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            var contact = collision.GetContact(0);
            impulse += contact.normal * contact.normalImpulse;
            impulse.x += contact.tangentImpulse * contact.normal.y;
            impulse.y -= contact.tangentImpulse * contact.normal.x;
        }

        return impulse;
    }

    private static List<Collider2D> collidersCache = new List<Collider2D>();

    /// <include file='Rigidbody2DExtensions.xml' path='docs/AddExplosionForce'/>
    public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce, Vector2 explosionPosition, float explosionRadius) => AddExplosionForce(rigidbody2D, explosionForce, explosionPosition, explosionRadius, 0f);
    /// <include file='Rigidbody2DExtensions.xml' path='docs/AddExplosionForce'/>
    public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier) => AddExplosionForce(rigidbody2D, explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode2D.Force);
    /// <include file='Rigidbody2DExtensions.xml' path='docs/AddExplosionForce'/>
    public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode2D mode)
    {
        // Prepare values.
        explosionRadius = Mathf.Approximately(explosionRadius, 0f)
            ? float.PositiveInfinity
            : Mathf.Abs(explosionRadius);
        var forceWearoff = 1f;  // A value from 0 to 1 that is lower based on the explosions' distance from the rigidbody.
        bool isOverlapping;     // Is the rigidbody overlapping with the explosion or not.

        // Get bounds. We will be using this to get the same behaviour as Rigidbody.AddExplosionForce.
        Bounds bounds;
        var attachedColliderCount = rigidbody2D.GetAttachedColliders(collidersCache);
        if (attachedColliderCount == 0)
        {
            bounds = new Bounds(rigidbody2D.worldCenterOfMass, Vector3.zero);
        }
        else
        {
            bounds = collidersCache[0].bounds;
            for (int i = 1; i < attachedColliderCount; i++)
            {
                var collider = collidersCache[i];
                bounds.Encapsulate(collider.bounds);
            }
        }

        // If the explosionRadius is infinity we can skip calculating the force wearoff.
        if (!float.IsPositiveInfinity(explosionRadius))
        {
            // Get the closest point on the rigidbody2D for calculating the explosion distance.
            Vector2 closestPoint = bounds.ClosestPoint(explosionPosition);
            isOverlapping = attachedColliderCount > 0 && closestPoint == explosionPosition;

            // Get the explosion distance.
            var explosionDistance = (closestPoint - explosionPosition).magnitude;

            // If the explosion distance is further than the explosion radius, don't add an explosion force.
            if (explosionDistance > explosionRadius)
            {
                return;
            }

            forceWearoff -= explosionDistance / explosionRadius;
        }
        else
        {
            isOverlapping = rigidbody2D.OverlapPoint(explosionPosition);
        }

        // Get the force position.
        var upwardsExplosionPosition = explosionPosition + Vector2.down * upwardsModifier;
        var forcePosition = (Vector2)bounds.ClosestPoint(upwardsExplosionPosition);
        if (forcePosition == upwardsExplosionPosition)
        {
            forcePosition = rigidbody2D.worldCenterOfMass;
        }

        // Get the force direction.
        Vector2 forceDirection = forcePosition - upwardsExplosionPosition;
        if (forceDirection.sqrMagnitude <= float.Epsilon)
        {
            forceDirection = Vector2.up;    // Default the force direction to up.
        }
        else if (!isOverlapping)
        {
            forceDirection.Normalize();     // Only normalize the force direction if we aren't overlapping with the explosion.
        }

        // Apply the force at the force position.
        var force = explosionForce * forceWearoff * forceDirection;
        rigidbody2D.AddForceAtPosition(force, forcePosition, mode);
    }

    /// <include file='Rigidbody2DExtensions.xml' path='docs/ClosestPointOnBounds'/>
    public static Vector2 ClosestPointOnBounds(this Rigidbody2D rigidbody2D, Vector2 position)
    {
        var count = rigidbody2D.GetAttachedColliders(collidersCache);
        if (count == 0)
        {
            return position;
        }

        var bounds = collidersCache[0].bounds;
        for (int i = 1; i < count; i++)
        {
            var collider = collidersCache[i];
            bounds.Encapsulate(collider.bounds);
        }

        return bounds.ClosestPoint(position);
    }

    public static Vector3 RandomNavCircle(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitCircle * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public static bool CheckIfPathPossible(NavMeshAgent agent, Vector2 targetPosition)
    {

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);

        return path.status == NavMeshPathStatus.PathPartial;

    }

    public static GameObject[] FindGameObjectsWithLayer(int layer)
    {

        GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> goList = new List<GameObject>();

        for (var i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();

    }
    public static Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    
    public static IEnumerator Fade(RawImage img, float targetAlpha, float fadeDuration)
    {

        float timeElapsed = 0;

        Color screenInitialColour = img.color;
        Color screenTargetColour = new Color(img.color.r, img.color.g, img.color.b, targetAlpha);

        while (timeElapsed < fadeDuration)
        {

            img.color = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / fadeDuration);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;

        }
        img.color = screenTargetColour;

    }
    public static IEnumerator Fade(TextMeshPro img, float targetAlpha, float fadeDuration)
    {

        float timeElapsed = 0;

        Color screenInitialColour = img.color;
        Color screenTargetColour = new Color(img.color.r, img.color.g, img.color.b, targetAlpha);

        while (timeElapsed < fadeDuration)
        {

            img.color = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / fadeDuration);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;

        }
        img.color = screenTargetColour;

    }

    public static IEnumerator Fade(Button img, float targetAlpha, float fadeDuration)
    {

        ColorBlock colors = img.colors;

        float timeElapsed = 0;

        Color screenInitialColour = img.colors.normalColor;
        Color screenTargetColour = new Color(img.colors.normalColor.r, img.colors.normalColor.g, img.colors.normalColor.b, targetAlpha);

        while (timeElapsed < fadeDuration)
        {

            colors.normalColor = Color.Lerp(screenInitialColour, screenTargetColour, timeElapsed / fadeDuration);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;

        }
        colors.normalColor = screenTargetColour;

    }

}
