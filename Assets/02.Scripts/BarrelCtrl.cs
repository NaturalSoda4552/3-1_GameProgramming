using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public float radius = 5.0f;
    private Rigidbody rb;
    
    public Texture[] textures;
    private new MeshRenderer renderer;
    private int textureIndex = 0;

    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);

        foreach (var coll in colls)
        {
            rb = coll.GetComponent<Rigidbody>();
            rb.mass = 1.0f;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);
        }
    }
    
    public GameObject BoomEffect;
    int cnt = 0;
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Bullet"))
        {
            cnt++;
            if (cnt == 3)
            {
                ContactPoint cp = coll.GetContact(0);
                Quaternion rot = Quaternion.LookRotation(-cp.normal);
                GameObject boom = Instantiate(BoomEffect, cp.point, rot);
                Destroy(boom, 1.0f);
                
                
                Destroy(gameObject);
                
                IndirectDamage(transform.position);
                
                cnt = 0;
            }
        }
    }
    
    void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
        textureIndex = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[textureIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
