using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(AudioSource))]
public class Destructible : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private AudioSource AudioSource;
    [SerializeField]
    private GameObject BrokenPrefab;
    [SerializeField]
    private AudioClip DestructionClip;
    [SerializeField]
    private float ExplosiveForce = 1000;
    [SerializeField]
    private float ExplosiveRadius = 2;
    [SerializeField]
    private float PieceFadeSpeed = 0.25f;
    [SerializeField]
    private float PieceDestroyDelay = 5f;
    [SerializeField]
    private float PieceSleepCheckDelay = 0.1f;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        AudioSource = GetComponent<AudioSource>();
    }

    public void Explode()
    {
        Destroy(Rigidbody);
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        if (DestructionClip != null)
        {
            AudioSource.PlayOneShot(DestructionClip);
        }

        GameObject brokenInstance = Instantiate(BrokenPrefab, transform.position, transform.rotation);

        Rigidbody[] rigidbodies = brokenInstance.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            if (Rigidbody != null)
            {
                // inherit velocities
                body.velocity = Rigidbody.velocity;
            }
            body.AddExplosionForce(ExplosiveForce, transform.position, ExplosiveRadius);
        }

        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }

    private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies)
    {
        WaitForSeconds Wait = new WaitForSeconds(PieceSleepCheckDelay);
        float activeRigidbodies = Rigidbodies.Length;

        while (activeRigidbodies > 0)
        {
            yield return Wait;

            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                if (rigidbody.IsSleeping())
                {
                    activeRigidbodies--;
                }
            }
        }


        yield return new WaitForSeconds(PieceDestroyDelay);

        float time = 0;
        Renderer[] renderers = Array.ConvertAll(Rigidbodies, GetRendererFromRigidbody);
        
        foreach(Rigidbody body in Rigidbodies)
        {
            Destroy(body.GetComponent<Collider>());
            Destroy(body);
        }

        while(time < 1)
        {
            float step = Time.deltaTime * PieceFadeSpeed; 
            foreach (Renderer renderer in renderers)
            {
                renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
            }

            time += step;
            yield return null;
        }

        foreach (Renderer renderer in renderers)
        {
            Destroy(renderer.gameObject);
        }
        Destroy(gameObject);
    }

    private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody)
    {
        return Rigidbody.GetComponent<Renderer>();
    }
}
