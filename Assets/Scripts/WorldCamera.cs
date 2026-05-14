using UnityEngine;

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
    private Sprite physicsSprite;
    private LineRenderer viewCone;

    // --- NEW: Variables for the Paste Preview ---
    private GameObject pastePreview;
    private SpriteRenderer previewRenderer;

    void Start()
    {
        // 1. Setup the Line Renderer (Hidden by default)
        viewCone = GetComponent<LineRenderer>();
        viewCone.positionCount = 3;
        viewCone.loop = true; 
        viewCone.startWidth = 0.05f;
        viewCone.endWidth = 0.05f;
        viewCone.enabled = false; 

        // 2. Setup the Paste Preview Ghost (Hidden by default)
        pastePreview = new GameObject("PastePreviewGhost");
        previewRenderer = pastePreview.AddComponent<SpriteRenderer>();
        previewRenderer.sortingOrder = 20; // Renders on top of everything so the player can see it easily
        pastePreview.SetActive(false); 
    }

    void Update()
    {
        // ==========================================
        // 1. CAPTURE LOGIC (Hold to aim, Release to take)
        // ==========================================
        
        if (Input.GetKey(captureKey))
        {
            // While holding the button, show and update the triangle
            viewCone.enabled = true;
            DrawViewCone();
        }
        else if (Input.GetKeyUp(captureKey))
        {
            // When letting go, hide the triangle and snap the photo!
            viewCone.enabled = false;
            CaptureDoublePhoto();
        }
        else if (!Input.GetKey(captureKey))
        {
            // Safety catch to ensure it stays hidden if we aren't pressing anything
            viewCone.enabled = false; 
        }

        // ==========================================
        // 2. PASTE LOGIC (Hold to preview, Release to paste)
        // ==========================================

        if (visualSprite != null) // Only allow pasting if we actually have a photo
        {
            if (Input.GetKeyDown(pasteKey))
            {
                // The moment the button is pressed down, activate the ghost preview
                pastePreview.SetActive(true);
                previewRenderer.sprite = visualSprite;
                
                // Make it 50% transparent (R, G, B, Alpha)
                previewRenderer.color = new Color(1f, 1f, 1f, 0.5f); 
            }

            if (Input.GetKey(pasteKey))
            {
                // While holding, make the ghost follow the mouse exactly
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0; 
                pastePreview.transform.position = mousePos;
            }

            if (Input.GetKeyUp(pasteKey))
            {
                // When letting go, hide the ghost and paste the real physical object!
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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        Color[] vPixels = visTex.GetPixels();
        Color[] pPixels = physTex.GetPixels();
        Vector2 center = new Vector2(photoResolution / 2f, photoResolution / 2f);

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
                }
            }
        }

        visTex.SetPixels(vPixels);
        visTex.Apply();
        physTex.SetPixels(pPixels);
        physTex.Apply();

        visualCamera.targetTexture = null;
        physicsCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(visRT);
        Destroy(physRT);

        float ppu = photoResolution / captureSizeWorldUnits;
        visualSprite = Sprite.Create(visTex, new Rect(0, 0, photoResolution, photoResolution), new Vector2(0.5f, 0.5f), ppu);
        physicsSprite = Sprite.Create(physTex, new Rect(0, 0, photoResolution, photoResolution), new Vector2(0.5f, 0.5f), ppu);
    }

    void PastePhysicsPhoto()
    {
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos.z = 0; 

        GameObject sticker = Instantiate(stickerPrefab, spawnPos, Quaternion.identity);
        SpriteRenderer visRenderer = sticker.GetComponent<SpriteRenderer>();
        visRenderer.sprite = visualSprite;
        visRenderer.sortingOrder = 0; 

        Transform physicsChild = sticker.transform.GetChild(0);
        SpriteRenderer physRenderer = physicsChild.GetComponent<SpriteRenderer>();
        physRenderer.sprite = physicsSprite;

        physicsChild.gameObject.AddComponent<PolygonCollider2D>();
        physicsChild.gameObject.layer = LayerMask.NameToLayer("Default");

        physRenderer.enabled = false; 
    }
}