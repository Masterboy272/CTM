using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PuzzlePhotoMechanic : MonoBehaviour
{
    [Header("Settings")]
    public float captureSizeWorldUnits = 10f; 
    public int photoResolution = 512; 
    public float visionAngle = 45f; 

    public KeyCode captureKey = KeyCode.E;
    public KeyCode pasteKey = KeyCode.Q;

    [Header("References")]
    public Camera visualCamera;  
    public Camera physicsCamera; 
    public GameObject stickerPrefab; 
    
    private Sprite visualSprite;      
    private Sprite grayscaleSprite;   
    private Sprite physicsSprite;
    
    private LineRenderer viewCone;
    private GameObject pastePreview;
    private SpriteRenderer previewRenderer;

    private GameObject currentPastedSticker;
    private bool isPhotoEmpty = false;
    
    private bool hasExtendedTimer = false;
    private float currentPhotoLifetime = 10f;

    // --- NEW: A structure to remember what creatures we photographed and where they were! ---
    private struct CapturedClone
    {
        public GameObject prefab;
        public Vector3 relativePos;
    }
    private List<CapturedClone> capturedClones = new List<CapturedClone>();

    void Start()
    {
        viewCone = GetComponent<LineRenderer>();
        viewCone.positionCount = 3;
        viewCone.loop = true; 
        viewCone.startWidth = 0.05f;
        viewCone.endWidth = 0.05f;
        viewCone.enabled = false; 

        pastePreview = new GameObject("PastePreviewGhost");
        previewRenderer = pastePreview.AddComponent<SpriteRenderer>();
        previewRenderer.sortingOrder = 20; 
        pastePreview.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKey(captureKey))
        {
            viewCone.enabled = true;
            DrawViewCone();
        }
        else if (Input.GetKeyUp(captureKey))
        {
            viewCone.enabled = false;
            CaptureDoublePhoto();
        }
        else if (!Input.GetKey(captureKey))
        {
            viewCone.enabled = false; 
        }

        if (visualSprite != null) 
        {
            if (Input.GetKeyDown(pasteKey))
            {
                pastePreview.SetActive(true);
                previewRenderer.sprite = visualSprite;
                previewRenderer.color = new Color(1f, 1f, 1f, 0.5f); 
            }

            if (Input.GetKey(pasteKey))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0; 
                pastePreview.transform.position = mousePos;
            }

            if (Input.GetKeyUp(pasteKey))
            {
                pastePreview.SetActive(false);
                PastePhysicsPhoto();
            }
        }
    }

    void DrawViewCone()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        float radius = captureSizeWorldUnits / 2f;
        float leftAngle = (aimAngle + visionAngle / 2f) * Mathf.Deg2Rad;
        float rightAngle = (aimAngle - visionAngle / 2f) * Mathf.Deg2Rad;

        Vector3 leftPoint = transform.position + new Vector3(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle), 0) * radius;
        Vector3 rightPoint = transform.position + new Vector3(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle), 0) * radius;

        viewCone.SetPosition(0, transform.position); 
        viewCone.SetPosition(1, leftPoint);          
        viewCone.SetPosition(2, rightPoint);         
    }

    void CaptureDoublePhoto()
    {
        visualCamera.aspect = 1f; 
        visualCamera.orthographicSize = captureSizeWorldUnits / 2f;
        physicsCamera.aspect = 1f; 
        physicsCamera.orthographicSize = captureSizeWorldUnits / 2f;
        
        RenderTexture visRT = new RenderTexture(photoResolution, photoResolution, 24);
        visualCamera.targetTexture = visRT;
        visualCamera.Render();
        RenderTexture.active = visRT;
        Texture2D visTex = new Texture2D(photoResolution, photoResolution, TextureFormat.RGBA32, false);
        visTex.ReadPixels(new Rect(0, 0, photoResolution, photoResolution), 0, 0);

        RenderTexture physRT = new RenderTexture(photoResolution, photoResolution, 24);
        physicsCamera.targetTexture = physRT;
        physicsCamera.Render();
        RenderTexture.active = physRT;
        Texture2D physTex = new Texture2D(photoResolution, photoResolution, TextureFormat.RGBA32, false);
        physTex.ReadPixels(new Rect(0, 0, photoResolution, photoResolution), 0, 0);

        Texture2D grayTex = new Texture2D(photoResolution, photoResolution, TextureFormat.RGBA32, false);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        Color[] vPixels = visTex.GetPixels();
        Color[] pPixels = physTex.GetPixels();
        Color[] gPixels = new Color[vPixels.Length]; 
        
        Vector2 center = new Vector2(photoResolution / 2f, photoResolution / 2f);

        isPhotoEmpty = true; 

        for (int y = 0; y < photoResolution; y++)
        {
            for (int x = 0; x < photoResolution; x++)
            {
                int index = y * photoResolution + x;
                Vector2 pixelPos = new Vector2(x, y);
                Vector2 dirFromCenter = (pixelPos - center).normalized;
                
                float pixelAngle = Mathf.Atan2(dirFromCenter.y, dirFromCenter.x) * Mathf.Rad2Deg;
                float angleDiff = Mathf.DeltaAngle(aimAngle, pixelAngle);

                if (Mathf.Abs(angleDiff) > (visionAngle / 2f) || pixelPos == center)
                {
                    vPixels[index] = Color.clear;
                    pPixels[index] = Color.clear;
                    gPixels[index] = Color.clear;
                }
                else
                {
                    if (pPixels[index].a > 0.1f) isPhotoEmpty = false;

                    Color orig = vPixels[index];
                    float luminosity = (orig.r * 0.3f) + (orig.g * 0.59f) + (orig.b * 0.11f);
                    gPixels[index] = new Color(luminosity, luminosity, luminosity, orig.a);
                }
            }
        }

        visTex.SetPixels(vPixels);
        visTex.Apply();
        physTex.SetPixels(pPixels);
        physTex.Apply();
        grayTex.SetPixels(gPixels);
        grayTex.Apply();

        // --- NEW: THE CLONE SCANNER ---
        capturedClones.Clear(); // Empty out the old photo's memory
        
        // Draw a circle to find nearby objects
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, captureSizeWorldUnits / 2f);
        foreach (Collider2D obj in nearbyObjects)
        {
            CloneablePhotoObject cloneable = obj.GetComponent<CloneablePhotoObject>();
            if (cloneable != null && cloneable.myPrefab != null)
            {
                // Verify the creature was actually inside the 45-degree triangle
                Vector2 dirToObj = (obj.transform.position - transform.position).normalized;
                float objAngle = Mathf.Atan2(dirToObj.y, dirToObj.x) * Mathf.Rad2Deg;
                if (Mathf.Abs(Mathf.DeltaAngle(aimAngle, objAngle)) <= visionAngle / 2f)
                {
                    // It's in the photo! Save its prefab and exact position
                    CapturedClone newClone;
                    newClone.prefab = cloneable.myPrefab;
                    newClone.relativePos = obj.transform.position - transform.position;
                    capturedClones.Add(newClone);
                }
            }
        }

        visualCamera.targetTexture = null;
        physicsCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(visRT);
        Destroy(physRT);

        float ppu = photoResolution / captureSizeWorldUnits;
        visualSprite = Sprite.Create(visTex, new Rect(0, 0, photoResolution, photoResolution), new Vector2(0.5f, 0.5f), ppu);
        physicsSprite = Sprite.Create(physTex, new Rect(0, 0, photoResolution, photoResolution), new Vector2(0.5f, 0.5f), ppu);
        grayscaleSprite = Sprite.Create(grayTex, new Rect(0, 0, photoResolution, photoResolution), new Vector2(0.5f, 0.5f), ppu);
    }

    void PastePhysicsPhoto()
    {
        if (currentPastedSticker != null) Destroy(currentPastedSticker);

        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos.z = 0; 

        GameObject sticker = Instantiate(stickerPrefab, spawnPos, Quaternion.identity);
        currentPastedSticker = sticker;

        SpriteRenderer baseRenderer = sticker.GetComponent<SpriteRenderer>();
        baseRenderer.sprite = grayscaleSprite;
        baseRenderer.sortingOrder = 0; 

        GameObject colorChild = new GameObject("ColorVisual");
        colorChild.transform.SetParent(sticker.transform, false);
        SpriteRenderer colorRenderer = colorChild.AddComponent<SpriteRenderer>();
        colorRenderer.sprite = visualSprite;
        colorRenderer.sortingOrder = 1; 

        StickerDecay timerScript = sticker.AddComponent<StickerDecay>();
        timerScript.lifetime = currentPhotoLifetime;
        timerScript.colorRenderer = colorRenderer;

        Transform physicsChild = sticker.transform.GetChild(0); 
        SpriteRenderer physRenderer = physicsChild.GetComponent<SpriteRenderer>();
        physRenderer.sprite = physicsSprite;
        physRenderer.enabled = false; 

        if (!isPhotoEmpty)
        {
            physicsChild.gameObject.AddComponent<PolygonCollider2D>();
            physicsChild.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        // --- NEW: SPAWN THE CLONES! ---
        foreach (CapturedClone clone in capturedClones)
        {
            // 1. Spawn the creature relative to where you pasted the photo
            GameObject spawnedBug = Instantiate(clone.prefab, spawnPos + clone.relativePos, Quaternion.identity);
            
            // 2. We DELETE the SetParent line! 
            // By leaving it alone, the bug spawns independently in the world.
            // When the photo rots away, this bug will stay behind permanently.
        }
    }

    // --- NEW: THE UPGRADE METHOD ---
    public void UpgradeTimer()
    {
        hasExtendedTimer = true;
        currentPhotoLifetime = 20f;
        Debug.Log("Power Up Collected! Photos now last 20 seconds!");
    }
}