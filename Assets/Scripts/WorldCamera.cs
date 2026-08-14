using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class PuzzlePhotoMechanic : MonoBehaviour
{
    [Header("Base Settings")]
    public float captureSizeWorldUnits = 10f; 
    public int photoResolution = 512; 
    public float visionAngle = 45f; 
    [Tooltip("Select layers allowed to be vaporized (e.g., Gameplay, Default)")]
    public LayerMask layersToVaporize;

    [Header("Controls")]
    public KeyCode captureKey = KeyCode.E;
    public KeyCode pasteKey = KeyCode.Q;
    public KeyCode selfieKey = KeyCode.F;

    [Header("References")]
    public Camera visualCamera;  
    public Camera physicsCamera; 
    public GameObject stickerPrefab; 
    [Tooltip("Drag your GameHUD Canvas object here")]
    public GameHUD hudManager; // --- NEW HUD REFERENCE ---
    
    // --- PHOTO SPRITES ---
    private Sprite visualSprite;      
    private Sprite grayscaleSprite;   
    private Sprite physicsSprite;
    
    // --- INTERNAL TRACKERS ---
    private LineRenderer viewCone;
    private GameObject pastePreview;
    private SpriteRenderer previewRenderer;
    private GameObject currentPastedSticker;
    private bool isPhotoEmpty = false;

    // --- CLONING MEMORY ---
    private struct CapturedClone
    {
        public GameObject prefab;
        public Vector3 relativePos;
        public bool isPermanent; 
    }
    private List<CapturedClone> capturedClones = new List<CapturedClone>();

    // --- POWER UP TRACKERS ---
    public bool hasExtendedTimer = false;
    private float currentPhotoLifetime = 10f; 
    
    public bool hasLensUpgrade = false;
    private bool isUsingTelephoto = false;
    private float baseCaptureSize; 

    public bool hasSelfieStick = false;
    private bool isSelfiePhoto = false;
    private bool selfieOnCooldown = false;
    public float selfieCooldown = 60f;

    void Start()
    {
        baseCaptureSize = captureSizeWorldUnits; 

        // Setup Aim Cone
        viewCone = GetComponent<LineRenderer>();
        viewCone.positionCount = 3;
        viewCone.loop = true; 
        viewCone.startWidth = 0.05f;
        viewCone.endWidth = 0.05f;
        viewCone.enabled = false; 

        // Setup Paste Preview Ghost
        pastePreview = new GameObject("PastePreviewGhost");
        previewRenderer = pastePreview.AddComponent<SpriteRenderer>();
        previewRenderer.sortingOrder = 20; 
        pastePreview.SetActive(false); 
    }

    void Update()
    {
        // ==========================================
        // 1. CAPTURE LOGIC (Normal Photo & Zoom)
        // ==========================================
        if (Input.GetKey(captureKey))
        {
            if (hasLensUpgrade)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (scroll > 0f) isUsingTelephoto = true; 
                else if (scroll < 0f) isUsingTelephoto = false; 
                
                captureSizeWorldUnits = isUsingTelephoto ? baseCaptureSize * 1.5f : baseCaptureSize;
            }

            viewCone.enabled = true;
            DrawViewCone();
        }
        else if (Input.GetKeyUp(captureKey))
        {
            viewCone.enabled = false;
            CaptureDoublePhoto();
        }

        // ==========================================
        // 2. SELFIE CAPTURE LOGIC
        // ==========================================
        else if (hasSelfieStick && !selfieOnCooldown)
        {
            if (Input.GetKey(selfieKey))
            {
                viewCone.enabled = true;
                DrawViewCone(); 
            }
            else if (Input.GetKeyUp(selfieKey))
            {
                viewCone.enabled = false;
                CaptureSelfie();
            }
        }
        else if (!Input.GetKey(captureKey) && !Input.GetKey(selfieKey))
        {
            viewCone.enabled = false; 
        }

        // ==========================================
        // 3. PASTE LOGIC
        // ==========================================
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
                
                if (isSelfiePhoto)
                {
                    TeleportPlayer();
                }
                else
                {
                    PastePhysicsPhoto();
                }
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
        isSelfiePhoto = false; 

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

        // THE CLONE SCANNER
        capturedClones.Clear(); 
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, captureSizeWorldUnits / 2f);
        foreach (Collider2D obj in nearbyObjects)
        {
            CloneablePhotoObject cloneable = obj.GetComponent<CloneablePhotoObject>();
            if (cloneable != null && cloneable.myPrefab != null)
            {
                Vector2 dirToObj = (obj.transform.position - transform.position).normalized;
                float objAngle = Mathf.Atan2(dirToObj.y, dirToObj.x) * Mathf.Rad2Deg;
                if (Mathf.Abs(Mathf.DeltaAngle(aimAngle, objAngle)) <= visionAngle / 2f)
                {
                    CapturedClone newClone;
                    newClone.prefab = cloneable.myPrefab;
                    newClone.relativePos = obj.transform.position - transform.position;
                    newClone.isPermanent = cloneable.isPermanent; 
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

        // --- NEW: SEND CAPTURED PHOTO TO HUD ---
        if (hudManager != null) hudManager.UpdatePhotoDisplay(visualSprite);
    }

    void CaptureSelfie()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        visualCamera.cullingMask |= (1 << playerLayer);

        visualCamera.aspect = 1f; 
        visualCamera.orthographicSize = captureSizeWorldUnits / 2f;
        
        RenderTexture visRT = new RenderTexture(photoResolution, photoResolution, 24);
        visualCamera.targetTexture = visRT;
        visualCamera.Render();
        RenderTexture.active = visRT;
        Texture2D visTex = new Texture2D(photoResolution, photoResolution, TextureFormat.RGBA32, false);
        visTex.ReadPixels(new Rect(0, 0, photoResolution, photoResolution), 0, 0);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = (mousePos - transform.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        Color[] vPixels = visTex.GetPixels();
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
                }
            }
        }

        visTex.SetPixels(vPixels);
        visTex.Apply();

        visualCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(visRT);
        visualCamera.cullingMask &= ~(1 << playerLayer); 

        float ppu = photoResolution / captureSizeWorldUnits;
        visualSprite = Sprite.Create(visTex, new Rect(0, 0, photoResolution, photoResolution), new Vector2(0.5f, 0.5f), ppu);
        
        isSelfiePhoto = true; 

        // --- NEW: SEND SELFIE PHOTO TO HUD ---
        if (hudManager != null) hudManager.UpdatePhotoDisplay(visualSprite);
    }

    void TeleportPlayer()
    {
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos.z = 0; 

        GameObject sticker = Instantiate(stickerPrefab, spawnPos, Quaternion.identity);
        SpriteRenderer visRenderer = sticker.GetComponent<SpriteRenderer>();
        visRenderer.sprite = visualSprite;
        visRenderer.sortingOrder = 10; 
        Destroy(sticker, 0.5f); 

        transform.position = spawnPos;

        LoseSavedPhoto();
        StartCoroutine(SelfieCooldownRoutine());
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

        // THE TILEMAP VAPORIZER
        PolygonCollider2D vaporizer = sticker.AddComponent<PolygonCollider2D>();
        vaporizer.isTrigger = true;
        Physics2D.SyncTransforms();

        List<Collider2D> thingsToDestroy = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layersToVaporize); 
        Physics2D.OverlapCollider(vaporizer, filter, thingsToDestroy);

        foreach(Collider2D obj in thingsToDestroy)
        {
            Tilemap map = obj.GetComponent<Tilemap>();
            if (map != null)
            {
                Bounds bounds = vaporizer.bounds;
                Vector3Int minCell = map.WorldToCell(bounds.min);
                Vector3Int maxCell = map.WorldToCell(bounds.max);

                for (int x = minCell.x - 1; x <= maxCell.x + 1; x++)
                {
                    for (int y = minCell.y - 1; y <= maxCell.y + 1; y++)
                    {
                        Vector3Int cellPos = new Vector3Int(x, y, 0);
                        Vector3 worldPos = map.GetCellCenterWorld(cellPos);
                        if (vaporizer.OverlapPoint(worldPos)) map.SetTile(cellPos, null);
                    }
                }
            }
            else
            {
                if (obj.transform.parent != null && obj.transform.parent.name.Contains("PuzzleSticker"))
                {
                    Destroy(obj.transform.parent.gameObject);
                }
                else
                {
                    Destroy(obj.gameObject); 
                }
            }
        }
        Destroy(vaporizer);

        // PASTE THE NEW LEVEL
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

        // SPAWN THE CLONES 
        foreach (CapturedClone clone in capturedClones)
        {
            GameObject spawnedClone = Instantiate(clone.prefab, spawnPos + clone.relativePos, Quaternion.identity);
            if (!clone.isPermanent)
            {
                spawnedClone.transform.SetParent(sticker.transform, true);
            }
        }
    }

    // ==========================================
    // METROIDVANIA UPGRADES & PENALTIES
    // ==========================================
    
    public void UpgradeTimer()
    {
        hasExtendedTimer = true;
        currentPhotoLifetime = 20f;
        Debug.Log("Power Up Collected! Photos now last 20 seconds!");
    }

    public void UnlockTelephotoLens()
    {
        hasLensUpgrade = true;
        Debug.Log("Telephoto Lens Collected! Scroll the mouse wheel while aiming to zoom!");
    }

    public void UnlockSelfieStick()
    {
        hasSelfieStick = true;
        Debug.Log("Selfie Stick Unlocked! Press F to snap a selfie and teleport.");
    }

    IEnumerator SelfieCooldownRoutine()
    {
        selfieOnCooldown = true;
        Debug.Log("Selfie Stick is on cooldown for 1 minute...");
        yield return new WaitForSeconds(selfieCooldown);
        selfieOnCooldown = false;
        Debug.Log("Selfie Stick is ready!");
    }

    public void LoseSavedPhoto()
    {
        visualSprite = null;
        physicsSprite = null;
        grayscaleSprite = null;
        capturedClones.Clear();
        isPhotoEmpty = true;

        if (pastePreview != null) pastePreview.SetActive(false);

        // --- NEW: REMOVE FROM HUD BECAUSE PHOTO WAS LOST ---
        if (hudManager != null) hudManager.UpdatePhotoDisplay(null);
        
        Debug.Log("Photo corrupted or consumed!");
    }
}