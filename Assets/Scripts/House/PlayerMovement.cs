using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public static PlayerMovement Instance { get; private set; }

    Vector3 targetPosition, furnitureTarget, lastPositionBeforeInteraction;
    bool isMoving, isGoingToFurniture, isInteracting;
    IFurnitureInteraction currentFurniture;
    Camera cam;
    Animator anim;
    PlayerControls controls;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
        anim = GetComponent<Animator>();
        controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controls.Player.TouchPosition.performed += ctx =>
        {
            Vector2 touchPos = ctx.ReadValue<Vector2>();
            if (IsPointerOverUI(touchPos)) return;
            Vector3 worldPos = cam.ScreenToWorldPoint(touchPos);
            Vector2 worldPos2D = new(worldPos.x, worldPos.y);
            var furnitureHit = Physics2D.Raycast(worldPos2D, Vector2.zero, 0f, LayerMask.GetMask("DetectingFurniture"));
            var groundHit = Physics2D.Raycast(worldPos2D, Vector2.zero, 0f, LayerMask.GetMask("Ground"));
            if (isInteracting)
            {
                if (furnitureHit.collider && furnitureHit.collider.GetComponent<IFurnitureInteraction>() == currentFurniture) return;
                if (furnitureHit.collider)
                {
                    currentFurniture.OnPlayerDepart();
                    ExitInteraction();
                    MoveToFurniture(furnitureHit.collider.GetComponent<IFurnitureInteraction>());
                    return;
                }
                if (groundHit.collider)
                {
                    currentFurniture.OnPlayerDepart();
                    ExitInteraction();
                    targetPosition = new Vector3(worldPos.x, transform.position.y, transform.position.z);
                    isMoving = true;
                    transform.localScale = (targetPosition.x > transform.position.x) ? Vector3.one : new Vector3(-1, 1, 1);
                    return;
                }
                return;
            }
            if (furnitureHit.collider)
            {
                MoveToFurniture(furnitureHit.collider.GetComponent<IFurnitureInteraction>());
                return;
            }
            if (groundHit.collider && !isGoingToFurniture)
            {
                targetPosition = new Vector3(worldPos.x, transform.position.y, transform.position.z);
                isMoving = true;
                transform.localScale = (targetPosition.x > transform.position.x) ? Vector3.one : new Vector3(-1, 1, 1);
            }
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (isGoingToFurniture)
        {
            transform.position = Vector3.MoveTowards(transform.position, furnitureTarget, speed * Time.deltaTime);
            anim.Play("Walk");
            transform.localScale = (furnitureTarget.x > transform.position.x) ? Vector3.one : new Vector3(-1, 1, 1);
            if (Vector3.Distance(transform.position, furnitureTarget) < 0.05f)
            {
                isGoingToFurniture = false;
                anim.Play("Idle");
                if (currentFurniture != null)
                {
                    currentFurniture.OnPlayerArrive();
                    isInteracting = true;
                    spriteRenderer.enabled = false;
                }
            }
            return;
        }
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            anim.Play("Walk");
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;
                anim.Play("Idle");
            }
        }
        else if (!isInteracting) anim.Play("Idle");
    }

    public void MoveToFurniture(IFurnitureInteraction furniture)
    {
        lastPositionBeforeInteraction = transform.position;
        furnitureTarget = new Vector3(furniture.InteractionX, transform.position.y, transform.position.z);
        isGoingToFurniture = true;
        isMoving = false;
        currentFurniture = furniture;
    }

    void ExitInteraction()
    {
        isInteracting = false;
        spriteRenderer.enabled = true;
        currentFurniture = null;
    }

    bool IsPointerOverUI(Vector2 screenPos)
    {
        var pointerData = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }

    public void HandleShutdown()
    {
        if (isInteracting && currentFurniture != null)
        {
            currentFurniture.OnPlayerDepart();
            ExitInteraction();
        }
    }
}
