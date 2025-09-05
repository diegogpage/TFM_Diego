using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{

    [Header("Movimiento")]
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private float factorGravedad = -7f; // prueba entre -5 y -9.8
    [SerializeField] private float alturaDeSalto = 2f;

    private Vector2 direccionInput;
    private Vector2 direccionMovimiento;
    private Vector3 velocidadVertical; // y se usa para salto/gravedad
    private CharacterController controller;
    private Animator anim;

    [Header("Detección suelo")]
    [SerializeField] private Transform pies;
    [SerializeField] private float radioDeteccion;
    [SerializeField] private LayerMask queEsSuelo;

    [Header("Ataque")]
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private float timeBtwAttacks;
    [SerializeField] private LayerMask queEsEnemigo;
    [SerializeField] private float danhoAtaque;
    [SerializeField] private GameObject efectoDescarga;
    private bool estaAtacando = false;
    private float timer = 0f;
    private float timerBool = 0f;

    [Header("AtaqueEspecial")]
    [SerializeField] private float danhoAtaqueEspecial;
    [SerializeField] private float timeBtwAttackEspecial;
    [SerializeField] private GameObject efectoLatigo;
    [SerializeField] private Image barraAttEspecial;
    private float cargaActual;
    private bool ataqueCargado = false;

    [Header("Tail - Balanceo")]
    [SerializeField] private float rangoSwing;
    [SerializeField] private LayerMask queEsSwingPoint;
    [SerializeField] private float swingDamping;        // amortiguamiento (0 = sin fricción)
    [SerializeField] private float swingInputStrength;   // cuánto empuja el input
    [SerializeField] private float swingInitialImpulse;// pequeña patada inicial
    [SerializeField] private float swingMaxAngleDeg;    // ángulo máximo ± en grados
    [SerializeField] private float swingSpeedMult;       // multiplicador para reducir la velocidad de balanceo
    private bool seColumpia = false;
    private Transform swingPoint;     // pivot
    private float swingLength = 2f;   // L
    private float swingAngle = 0f;    // theta en radianes
    private float angularVelocity = 0f; // omega

    // lanzamiento tras soltar la liana
    private Vector3 launchVelocity = Vector3.zero;
    [SerializeField] private float launchDrag;

    [Header("Sonido")]
    [SerializeField] private AudioSource sonidoPisadas;
    [SerializeField] private AudioSource sonidoAtaque;
    [SerializeField] private AudioSource sonidoAtaqueEspecial;
    [SerializeField] private AudioSource sonidoBalanceo;
    

    private void OnEnable()
    {
        inputManager.OnSaltar += Saltar;
        inputManager.OnMover += Mover;
        inputManager.OnAtacar += Atacar;
        inputManager.OnTail += Tail; // tu evento de cola
    }

    private void OnDisable()
    {
        inputManager.OnSaltar -= Saltar;
        inputManager.OnMover -= Mover;
        inputManager.OnAtacar -= Atacar;
        inputManager.OnTail -= Tail;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        
        // Para cargar la posicion si fuera necesario
        if (SaveSystem.HayCheckpoint())
        {
            transform.position = SaveSystem.CargarCheckpoint();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (estaAtacando)
        {
            timerBool += Time.deltaTime;
            if (timerBool >= timeBtwAttacks)
            {
                estaAtacando = false;
                timerBool = 0f;
            }
        }

        if (!ataqueCargado)
        {
            cargaActual += Time.deltaTime;
            barraAttEspecial.fillAmount = Mathf.Clamp01(cargaActual / timeBtwAttackEspecial);

            if (cargaActual >= timeBtwAttackEspecial)
            {
                ataqueCargado = true;
            }
        }

        // Aplicar comportamiento según estado
        if (seColumpia)
        {
            BalanceoPendulo();
        }
        else
        {
            Movement();
        }
            

        // Reducir gradualmente la launchVelocity (drag)
        if (launchVelocity.sqrMagnitude > 0.0001f)
        {
            launchVelocity.x = Mathf.MoveTowards(launchVelocity.x, 0f, launchDrag * Time.deltaTime);
            // la componente y ya cae por gravedad (velocidadVertical) — la dejamos
        }
    }

    // -------------------------------
    // MOVIMIENTO NORMAL (usa launchVelocity)
    // -------------------------------
    private void Movement()
    {
        if (estaAtacando && EstoyEnSuelo())
        {
            direccionMovimiento = Vector2.zero;
        }
        else
        {
            direccionMovimiento = new Vector2(direccionInput.x, 0f);
        }

        // Combinar input horizontal con cualquier impulso horizontal residual
        float vx = direccionMovimiento.x * horizontalSpeed + launchVelocity.x;
        float vy = velocidadVertical.y + launchVelocity.y;

        Vector3 desplazamiento = new Vector3(vx, vy, 0f) * Time.deltaTime;
        controller.Move(desplazamiento);
        // Forzar Z = 0 para evitar resbalones en profundidad
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        // Anim y flip
        if (Mathf.Abs(direccionMovimiento.x) > 0.01f)
        {
            anim.SetBool("running", true);
            if (direccionMovimiento.x > 0) transform.eulerAngles = new Vector3(0, 90, 0);
            else transform.eulerAngles = new Vector3(0, -90, 0);

            //Sonido
            if (EstoyEnSuelo())
            {
                if (!sonidoPisadas.isPlaying) sonidoPisadas.Play();
            }
            else
            {
                if (sonidoPisadas.isPlaying) sonidoPisadas.Stop();
            }
            
        }
        else
        {
            anim.SetBool("running", false);

            if (sonidoPisadas.isPlaying) sonidoPisadas.Stop();
        }

        // Si estamos en suelo y cayendo, reseteamos vertical
        if (EstoyEnSuelo() && velocidadVertical.y < 0f)
        {
            velocidadVertical.y = 0f;
            anim.ResetTrigger("jump");
        }

        AplicarGravedad();
    }

    private void Mover(Vector2 ctx)
    {
        direccionInput = ctx;
    }

    private void Saltar()
    {
        if (!seColumpia && EstoyEnSuelo())
        {
            // reset antes de saltar
            velocidadVertical.y = 0f;
            launchVelocity.y = 0f;
            velocidadVertical.y = Mathf.Sqrt(-2f * factorGravedad * alturaDeSalto);
            anim.SetTrigger("jump");
        }
        else if (seColumpia)
        {
            // soltar desde la liana con impulso derivado del péndulo
            SoltarDesdeSwing();
        }
    }

    private void AplicarGravedad()
    {
        // Solo aplico la gravedad cuando dejo de tocar el suelo para evitar que se vaya acumulando y caiga de forma brusca
        if (!EstoyEnSuelo())
        {
            velocidadVertical.y += factorGravedad * Time.deltaTime;
            launchVelocity.y += factorGravedad * Time.deltaTime;
        }
    }

    private bool EstoyEnSuelo()
    {
        return Physics.CheckSphere(pies.position, radioDeteccion, queEsSuelo);
    }

    // -------------------------------
    // ATAQUE NORMAL
    // -------------------------------
    private void Atacar()
    {
        if (seColumpia) return;
        if (estaAtacando) return;

        estaAtacando = true;
        anim.SetTrigger("attack");
        //Invoke(nameof(ReproducirDescarga), 0.5f);

    }

    public void ReproducirDescarga()
    {
        if (efectoDescarga) 
        {
            Instantiate(efectoDescarga, puntoAtaque.position, Quaternion.identity);
            sonidoAtaque.Play();
        }

        Collider[] enemigosDetectados = Physics.OverlapSphere(puntoAtaque.position, radioAtaque, queEsEnemigo);
        if (enemigosDetectados.Length > 0)
        {
            Enemy enemigoScript = enemigosDetectados[0].GetComponent<Enemy>();
            if (enemigoScript != null)
            {
                enemigoScript.QuitarVida(danhoAtaque);
                timer = 0f;
            }
        }
    }


    // -------------------------------
    // TAIL: Swing o ataque especial
    // -------------------------------
    private void Tail()
    {
        if (!seColumpia)
        {
            Collider[] puntos = Physics.OverlapSphere(puntoAtaque.position, rangoSwing, queEsSwingPoint);
            if (puntos.Length > 0)
            {
                // iniciar péndulo: configurar con la posición real del jugador
                swingPoint = puntos[0].transform;
                seColumpia = true;
                anim.SetBool("isSwinging", true);

                if (!sonidoBalanceo.isPlaying) sonidoBalanceo.Play();

                swingLength = Vector3.Distance(transform.position, swingPoint.position);
                if (swingLength < 0.2f) swingLength = 0.5f;

                // calcular ángulo inicial (relación con vertical)
                float dx = transform.position.x - swingPoint.position.x;         // lateral
                float dy = swingPoint.position.y - transform.position.y;         // vertical "hacia abajo"
                swingAngle = Mathf.Atan2(dx, dy); // ángulo respecto a vertical hacia abajo

                // velocidad angular inicial (pequeño empujón)
                angularVelocity = swingInitialImpulse * Mathf.Sign(direccionInput.x != 0 ? direccionInput.x : dx);

                // limpiar launchVelocity para que no interfiera
                launchVelocity = Vector3.zero;
            }
            else
            {
                // no hay punto -> ataque especial con cola
                AtaqueEspecial();
            }
        }
        else
        {
            // si ya estaba columpiando -> soltar
            SoltarDesdeSwing();
        }
    }

    private void AtaqueEspecial()
    {
        if (!ataqueCargado) return;

        estaAtacando = true;
        anim.SetTrigger("tailAttack");
        //Invoke(nameof(ReproducirLatigo), 1.2f);
        ataqueCargado = false;
        cargaActual = 0f;
    }

    public void ReproducirLatigo()
    {
        if (efectoLatigo)
        {
            Instantiate(efectoLatigo, puntoAtaque.position, Quaternion.Euler(0, 0, -90));
            sonidoAtaqueEspecial.Play();
        }

        Collider[] enemigosDetectados = Physics.OverlapSphere(puntoAtaque.position, radioAtaque, queEsEnemigo);
        if (enemigosDetectados.Length > 0)
        {
            Enemy enemigoScript = enemigosDetectados[0].GetComponent<Enemy>();
            if (enemigoScript != null)
            {
                enemigoScript.QuitarVida(danhoAtaqueEspecial);
                timer = 0f;
            }
        }
    }

    // -------------------------------
    // BALANCEO (péndulo con límite de ángulo)
    // -------------------------------
    private void BalanceoPendulo()
    {
        if (swingPoint == null)
        {
            seColumpia = false;
            anim.SetBool("isSwinging", false);
            if (sonidoBalanceo.isPlaying) sonidoBalanceo.Stop();
            return;
        }

        float dt = Time.deltaTime;
        float g = Mathf.Abs(factorGravedad);
        float L = Mathf.Max(0.1f, swingLength);
        float theta = swingAngle;

        // ecuación angular (pendulum): alpha = -(g/L) * sin(theta)
        float alpha = -(g / L) * Mathf.Sin(theta);

        // input lateral añade torque (dirección del input)
        alpha += direccionInput.x * swingInputStrength;

        // integrar omega con damping
        angularVelocity += alpha * dt;
        angularVelocity *= (1f - swingDamping * dt);

        // integrar theta
        swingAngle += angularVelocity * dt * swingSpeedMult;

        // limitar ángulo a rango ±max (en radianes)
        float maxAngle = Mathf.Clamp(swingMaxAngleDeg, 5f, 179f) * Mathf.Deg2Rad;
        if (swingAngle > maxAngle)
        {
            swingAngle = maxAngle;
            if (angularVelocity > 0f) angularVelocity = -angularVelocity * 0.25f; // rebote apagado
        }
        else if (swingAngle < -maxAngle)
        {
            swingAngle = -maxAngle;
            if (angularVelocity < 0f) angularVelocity = -angularVelocity * 0.25f;
        }

        // calcular posición del jugador usando L y theta (en 2D X,Y)
        Vector3 r = new Vector3(Mathf.Sin(swingAngle) * L, -Mathf.Cos(swingAngle) * L, 0f);
        Vector3 targetPos = swingPoint.position + r;

        Vector3 delta = targetPos - transform.position;

        // limitar paso por frame para evitar teletransportes
        float maxStep = Mathf.Max(0.02f, horizontalSpeed * 4f * dt);
        if (delta.magnitude > maxStep)
            delta = delta.normalized * maxStep;

        controller.Move(delta);

        // flip del sprite según movimiento horizontal del delta
        if (delta.x < 0) transform.eulerAngles = new Vector3(0, -90, 0);
        else if (delta.x > 0) transform.eulerAngles = new Vector3(0, 90, 0);

    }

    // -------------------------------
    // SOLTAR: convertir omega en velocidad lineal y salir
    // -------------------------------
    private void SoltarDesdeSwing()
    {
        if (!seColumpia) return;

        // vector r actual
        float L = Mathf.Max(0.1f, swingLength);
        Vector3 r = new Vector3(Mathf.Sin(swingAngle) * L, -Mathf.Cos(swingAngle) * L, 0f);

        // velocidad lineal = omega x r (en 2D)
        Vector3 vLin = angularVelocity * new Vector3(-r.y, r.x, 0f);

        //Limito la velocidad máxima de lanzamiento
        float maxLaunchSpeed = 6f;
        if (vLin.magnitude > maxLaunchSpeed)
        {
            vLin = vLin.normalized * maxLaunchSpeed;
        }

        // asignamos launchVelocity (se usa en Movement)
        launchVelocity = vLin;

        // reset y normal de salto para que el impulso vertical tenga efecto
        velocidadVertical.y = 0f;

        // limpiar estado de swing
        seColumpia = false;
        anim.SetBool("isSwinging", false);
        if (sonidoBalanceo.isPlaying) sonidoBalanceo.Stop();
        swingPoint = null;
        swingLength = 0f;
        swingAngle = 0f;
        angularVelocity = 0f;

    }

    // -------------------------------
    // GIZMOS para debug
    // -------------------------------
    private void OnDrawGizmos()
    {
        if (puntoAtaque)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(puntoAtaque.position, rangoSwing);
        }

        if (pies)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pies.position, radioDeteccion);
        }

        if (seColumpia && swingPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(swingPoint.position, transform.position);
            Gizmos.DrawWireSphere(swingPoint.position, 0.08f);
        }
    }
}
