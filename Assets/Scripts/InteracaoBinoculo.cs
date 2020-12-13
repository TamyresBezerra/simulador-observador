/*
 * Autora: Tamyres Bezerra de Souza
 */

using UnityEngine;
using UnityEngine.XR;
using VRTK;

//Classe responsável por controlar a interação entre o usuário e o binóculo
public class InteracaoBinoculo : MonoBehaviour
{
    //obtenção do script de interação do (VRTK) para analisar se o binóculo está sendo segurado ou não
    public VRTK_InteractableObject linkedObject;
    //obtenção do script seguir do (VRTK) para controlar se o binóculo ficará suspenso sobre o peitoral
    public VRTK_TransformFollow transformFollow;
    //Objeto responsável por ativar ou não a borda delimitadora do binóculo (a parte escura)
    public GameObject binoculoCamera;
    //Obtenção do modelo da mão esquerda do (VRTK), utilizado para deixa-lá invisível para não atrapalhar a utilização do binóculo
    public GameObject maoEsquerda;
    //Obtenção do modelo da mão direita do (VRTK), utilizado para deixa-lá invisível para não atrapalhar a utilização do binóculo
    public GameObject maoDireita;
    //Obtenção da Câmera do simulador para a aplicação do zoom
    public Camera cameraSet;
    //multiplicador de zoom da câmera
    private int smoth = 100;
    //variável que sinaliza se o binóculo está sendo usado ou não pelo usuário
    public static bool zoomIn = false;

    //método responsável por iniciar os métodos de eventos disparados quando o usuário segura o binóculo
    protected virtual void OnEnable()
    {
        //obtendo o objeto de interação usado no simulador a partir do script usado no componente da biblioteca (VRTK)
        linkedObject = (linkedObject == null ? GetComponent<VRTK_InteractableObject>() : linkedObject);

        //Caso exista, vamos obter os eventos de segurar ou não o binóculo
        if (linkedObject != null)
        {
            linkedObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
            linkedObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
        }
    }

    //método responsável por remover os métodos de eventos disparados quando o usuário segura ou solta o binóculo
    protected virtual void OnDisable()
    {
        if (linkedObject != null)
        {
            linkedObject.InteractableObjectGrabbed -= InteractableObjectGrabbed;
            linkedObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
        }
    }

    //método disparado quando o binóculo é segurado pelo usuário
    protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
    {
        //aqui desativamos a fixação do binóculo acima do peitoral do usuário
        transformFollow.followsPosition = false;
        transformFollow.followsRotation = false;
    }

    //método disparado quando o binóculo é solto pelo usuário
    protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
    {
        //aqui ativamos a fixação do binóculo acima do peitoral do usuário
        transformFollow.followsPosition = true;
        transformFollow.followsRotation = true;
    }

    //método disparado toda vez que o binóculo colide com algo dentro do simulador
    private void OnTriggerEnter(Collider collider)
    {
        //caso a colisão seja da cabeça do usuário no simulador
        if (collider.gameObject == GameObject.Find("[VRTK][AUTOGEN][HeadsetColliderContainer]"))
        {
            //caso o zoomIn já não esteja aplicado (isso evita a repetição do código, já que o OnTriggerEnter pode ser disparados diversas vezes)
            if (!zoomIn)
            {
                //modificações necessárias para a aplicação do zoom no momento da utilização do binóculo 
                XRDevice.fovZoomFactor = 2.5f;
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 10, Time.deltaTime * smoth);
                
                //aqui o binóculo fica "invisível", fazendo com que ele não atrapalhe a visão do usuário na sua utilização
                GameObject.Find("BinoPart1").GetComponentInChildren<MeshRenderer>().enabled = false;
                GameObject.Find("BinoPart2").GetComponentInChildren<MeshRenderer>().enabled = false;

                //aqui ativamos a borda delimitadora do binóculo
                binoculoCamera.SetActive(true);

                //aqui as mãos ficam "invisíveis", fazendo com que ele não atrapalhe a visão do usuário na sua utilização
                maoEsquerda.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                maoDireita.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                
                //variável utilizada para informar que o binóculo está em uso
                zoomIn = true;
            }
        }
    }

    //método disparado toda vez que o binóculo sai de uma colisão com algo dentro do simulador
    private void OnTriggerExit(Collider collider)
    {
        //verificando se o binóculo está em uso
        if (zoomIn)
        {
            //removendo modificações necessárias para a aplicação do zoom no momento da utilização do binóculo 
            XRDevice.fovZoomFactor = 1f;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * smoth);

            //revertendo a questão do binóculo ficar "invisível"
            GameObject.Find("BinoPart1").GetComponentInChildren<MeshRenderer>().enabled = true;
            GameObject.Find("BinoPart2").GetComponentInChildren<MeshRenderer>().enabled = true;

            //desativando a borda delimitadora do binóculo
            binoculoCamera.SetActive(false);

            //revertendo a questão das mãos ficarem "invisíveis"
            maoEsquerda.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            maoDireita.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

            //variável utilizada para informar que o binóculo não está em uso
            zoomIn = false;
        }
    }
}