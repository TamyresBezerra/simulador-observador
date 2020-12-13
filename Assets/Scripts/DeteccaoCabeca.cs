/*
 * Autora: Tamyres Bezerra de Souza
 */

using UnityEngine;
using VRTK;

//Classe responsável por tratar os eventos do ponteiro (laser) que sai da cabeça do usuário do simulador
public class DeteccaoCabeca : MonoBehaviour
{
    //ponteiro em que serão extraídos os eventos
    public VRTK_DestinationMarker pointer;
    //variável que faz referência a o texto que mostra a distância na tela
    public UnityEngine.UI.Text distance;
    //variável que faz referência a o texto que mostra o nome do objeto mirado na tela
    public UnityEngine.UI.Text objectName;
    //variável que faz referência a o texto que mostra a licalização do observador na tela
    public UnityEngine.UI.Text targetLocation;

    //método responsável por iniciar os métodos de eventos disparados pelo script do ponteiro (laser)
    protected virtual void OnEnable()
    {
        //obtendo o ponteiro usado no simulador a partir do script usado no componente da biblioteca (VRTK)
        pointer = (pointer == null ? GetComponent<VRTK_DestinationMarker>() : pointer);

        //Caso o ponteiro exista, vamos obter os eventos de hover do ponteiro
        if (pointer != null)
        {
            pointer.DestinationMarkerHover += DestinationMarkerHover;
        }
        else
        {
            //caso não exista, vamos emitir um alerta de erro
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTKExample_PointerObjectHighlighterActivator", "VRTK_DestinationMarker", "the Controller Alias"));
        }
    }

    //método responsável por remover os métodos de eventos disparados pelo script do ponteiro (laser)
    protected virtual void OnDisable()
    {
        if (pointer != null)
        {
            pointer.DestinationMarkerHover -= DestinationMarkerHover;
        }
    }

    //método que é disparado a cada frame no simulador
    private void Update()
    {
        //aqui atualizamos o texto de localização do usuário na tela, obtendo o x,y e z do mesmo
        targetLocation.text = "x: " + transform.position.x.ToString("F") +
            " y: " + transform.position.y.ToString("F") +
            " z: " + (transform.position.z - 0.7).ToString("F");
    }

    //evento disparado toda vez que o ponteiro mirar em algum objeto na tela, e também é disparado ao se mover na superfície do mesmo objeto
    private void DestinationMarkerHover(object sender, DestinationMarkerEventArgs e)
    {
        //se o evento está ativo, vamos setar os textos da distância e do nome do objeto na tela
       SetTexts(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER HOVER", e.target, e.raycastHit, e.distance, e.destinationPosition);
    }

    //método responsável por modificar os textos de distancia entre o usuário e o objeto mirado e o nome do objeto mirado
    private void SetTexts(uint index, string action, Transform target, RaycastHit raycastHit, float distance, Vector3 tipPosition)
    {
        //obtendo o nome do objeto mirado, caso não exista, é retornado <NO VALID TARGET>
        string targetName = (target ? target.name : "<NO VALID TARGET>");
        //uma condição para mostrar a informação na tela, que é apenas no caso em que o binóculo esteja em uso pelo usuário do simulador
        if (InteracaoBinoculo.zoomIn)
        {
            //caso esteja sendo usado, é modificado o texto da distancia e nome do objeto
            this.distance.text = this.GetDistance(distance);
            objectName.text = targetName;
        } else
        {
            //caso não esteja sendo usado, são limpados os textos
            this.distance.text = "";
            objectName.text = "";
        }
    }

    //método responsável por formatar a distância (texto mostrado na tela do usuário)
    private string GetDistance(float distance)
    {
        double result = ((distance - 0.7) * 10);
        if (result > 1000)
        {
            return (result / 1000).ToString("F") + " Km";
        }
        return (result).ToString("F") + " Metros";
    }
}